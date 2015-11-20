using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using tessnet2;

namespace SHbit
{
        public class DecodeHelper
        {
            const double topbottomblankthreshold = 0.90;
            const string alphabetnum = "0123456789";

            /// <summary>
            /// 得到灰度图像前景背景的临界值 最大类间方差法
            /// </summary>
            /// <returns>前景背景的临界值</returns>
            public int GetDgGrayValue(Bitmap bitmap)
            {
                int[] pixelNum = new int[256];           //图象直方图，共256个点
                int n, n1, n2;
                int total;                              //total为总和，累计值
                double m1, m2, sum, csum, fmax, sb;     //sb为类间方差，fmax存储最大方差值
                int k, t, q;
                int threshValue = 1;                      // 阈值
                //int step = 1;
                //生成直方图
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        //返回各个点的颜色，以RGB表示
                        pixelNum[bitmap.GetPixel(i, j).R]++;            //相应的直方图加1
                    }
                }
                //直方图平滑化
                for (k = 0; k <= 255; k++)
                {
                    total = 0;
                    for (t = -2; t <= 2; t++)              //与附近2个灰度做平滑化，t值应取较小的值
                    {
                        q = k + t;
                        if (q < 0)                     //越界处理
                            q = 0;
                        if (q > 255)
                            q = 255;
                        total = total + pixelNum[q];    //total为总和，累计值
                    }
                    pixelNum[k] = (int)((float)total / 5.0 + 0.5);    //平滑化，左边2个+中间1个+右边2个灰度，共5个，所以总和除以5，后面加0.5是用修正值
                }
                //求阈值
                sum = csum = 0.0;
                n = 0;
                //计算总的图象的点数和质量矩，为后面的计算做准备
                for (k = 0; k <= 255; k++)
                {
                    sum += (double)k * (double)pixelNum[k];     //x*f(x)质量矩，也就是每个灰度的值乘以其点数（归一化后为概率），sum为其总和
                    n += pixelNum[k];                       //n为图象总的点数，归一化后就是累积概率
                }

                fmax = -1.0;                          //类间方差sb不可能为负，所以fmax初始值为-1不影响计算的进行
                n1 = 0;
                for (k = 0; k < 256; k++)                  //对每个灰度（从0到255）计算一次分割后的类间方差sb
                {
                    n1 += pixelNum[k];                //n1为在当前阈值遍前景图象的点数
                    if (n1 == 0) { continue; }            //没有分出前景后景
                    n2 = n - n1;                        //n2为背景图象的点数
                    if (n2 == 0) { break; }               //n2为0表示全部都是后景图象，与n1=0情况类似，之后的遍历不可能使前景点数增加，所以此时可以退出循环
                    csum += (double)k * pixelNum[k];    //前景的“灰度的值*其点数”的总和
                    m1 = csum / n1;                     //m1为前景的平均灰度
                    m2 = (sum - csum) / n2;               //m2为背景的平均灰度
                    sb = (double)n1 * (double)n2 * (m1 - m2) * (m1 - m2);   //sb为类间方差
                    if (sb > fmax)                  //如果算出的类间方差大于前一次算出的类间方差
                    {
                        fmax = sb;                    //fmax始终为最大类间方差（otsu）
                        threshValue = k;              //取最大类间方差时对应的灰度的k就是最佳阈值
                    }
                }
                return threshValue;
            }


            /// <summary>
            /// 3×3中值滤波除杂
            /// </summary>
            /// <param name="dgGrayValue"></param>
            public Bitmap ClearNoise(Bitmap bitmap)
            {
                int dgGrayValue = GetDgGrayValue(bitmap);

                int x, y;
                byte[] p = new byte[9]; //最小处理窗口3*3
                byte s;
                //byte[] lpTemp=new BYTE[nByteWidth*nHeight];
                int i, j;

                //--!!!!!!!!!!!!!!下面开始窗口为3×3中值滤波!!!!!!!!!!!!!!!!
                for (y = 1; y < bitmap.Height - 1; y++) //--第一行和最后一行无法取窗口
                {
                    for (x = 1; x < bitmap.Width - 1; x++)
                    {
                        //取9个点的值
                        p[0] = bitmap.GetPixel(x - 1, y - 1).R;
                        p[1] = bitmap.GetPixel(x, y - 1).R;
                        p[2] = bitmap.GetPixel(x + 1, y - 1).R;
                        p[3] = bitmap.GetPixel(x - 1, y).R;
                        p[4] = bitmap.GetPixel(x, y).R;
                        p[5] = bitmap.GetPixel(x + 1, y).R;
                        p[6] = bitmap.GetPixel(x - 1, y + 1).R;
                        p[7] = bitmap.GetPixel(x, y + 1).R;
                        p[8] = bitmap.GetPixel(x + 1, y + 1).R;
                        //计算中值
                        for (j = 0; j < 5; j++)
                        {
                            for (i = j + 1; i < 9; i++)
                            {
                                if (p[j] > p[i])
                                {
                                    s = p[j];
                                    p[j] = p[i];
                                    p[i] = s;
                                }
                            }
                        }
                        bitmap.SetPixel(x, y, Color.FromArgb(p[4], p[4], p[4]));    //给有效值付中值
                    }
                }
                return bitmap;
            }


            public Bitmap GetValidRegionPic(Bitmap bitmap)
            {
                return GetValidRegionOfImage(bitmap);
            }

            public int[] GetValidCharIndex(Bitmap bitmap)
            {
                List<int> validIndex = new List<int>();
                Bitmap[] pics = GetSplitPics(bitmap, 6, 1);     //分割
                for (int i = 0; i < pics.Length; i++)
                {
                    if (IsRedCountMoreThanGray(pics[i]))
                    {
                        validIndex.Add(i);
                    }
                }
                return validIndex.ToArray();
            }

            /// <summary>
            /// When say Valid, it's mean the red color count is more than gray color count
            /// </summary>
            /// <param name="bitmap"></param>
            /// <param name="threshold"></param>
            /// <returns></returns>
            private bool IsRedCountMoreThanGray(Bitmap bitmap)
            {
                int redCount = 0;
                int grayCount = 0;
                for (int left = 1; left < bitmap.Width; left++)
                {
                    for (int height = 1; height < bitmap.Height; height++)
                    {
                        var pixel = bitmap.GetPixel(left, height);
                        if (pixel.R > 200 && pixel.B < 50 && pixel.G < 50)
                        {
                            redCount++;
                        }
                        else if (pixel.R < 150 && pixel.B < 150 && pixel.G < 150)
                        {
                            grayCount++;
                        }
                    }
                }
                return redCount > grayCount;
            }

            public List<Word> GetWordFromImage(Bitmap bitmap)
            {
                Tesseract ocr = new Tesseract();
                ocr.SetVariable("tessedit_char_whitelist", alphabetnum);
                ocr.Init(null, "eng", false);
                List<tessnet2.Word> result = ocr.DoOCR(bitmap, Rectangle.Empty);
                return result;
            }

            public Bitmap GetValidRegionOfImage(Bitmap bitmap)
            {
                int top = GetCharsTop(bitmap, topbottomblankthreshold);
                int bottom = GetCharsBottom(bitmap, topbottomblankthreshold);

                Bitmap newImg = new Bitmap(bitmap.Width, bottom - top + 3);

                using (Graphics g = Graphics.FromImage(newImg))
                {
                    Rectangle rect = new Rectangle(1, top, bitmap.Width, bottom - top + 3);
                    g.DrawImage(bitmap, 0, 0, rect, GraphicsUnit.Pixel);
                }
                return newImg;
            }

            /// <summary>
            /// Check from top to bottom, get the start of color(either red or gray color)
            /// </summary>
            /// <param name="threshold"></param>
            /// <returns></returns>
            private int GetCharsTop(Bitmap bitmap, double threshold)
            {
                int y;
                for (y = 1; y < bitmap.Height; y++)
                {
                    int blankCount = 0;
                    for (int x = 1; x < bitmap.Width; x++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        if (pixel.R > 200 && pixel.B > 200 && pixel.G > 200)
                        {
                            blankCount++;
                        }
                    }
                    if ((double)blankCount / bitmap.Width < threshold)
                    {
                        break;
                    }
                }
                return y;
            }

            /// <summary>
            /// Check from bottom to top, get the start of color(either red or gray color)
            /// </summary>
            /// <param name="threshold"></param>
            /// <returns></returns>
            private int GetCharsBottom(Bitmap bitmap, double threshold)
            {
                int y;
                for (y = bitmap.Height - 1; y > 0; y--)
                {
                    int blankCount = 0;
                    for (int x = 1; x < bitmap.Width; x++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        if (pixel.R > 200 && pixel.B > 200 && pixel.G > 200)
                        {
                            blankCount++;
                        }
                    }
                    if ((double)blankCount / bitmap.Width < threshold)
                    {
                        break;
                    }
                }
                return y;
            }

            /// <summary>
            /// 根据RGB，计算灰度值
            /// </summary>
            /// <param name="posClr">Color值</param>
            /// <returns>灰度值，整型</returns>
            private int GetGrayNumColor(System.Drawing.Color posClr)
            {
                return (posClr.R * 19595 + posClr.G * 38469 + posClr.B * 7472) >> 16;
            }

            /// <summary>
            /// 灰度转换,逐点方式
            /// </summary>
            public Bitmap SetGrayByPixels(Bitmap bitmap)
            {
                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int j = 0; j < bitmap.Width; j++)
                    {
                        int tmpValue = GetGrayNumColor(bitmap.GetPixel(j, i));
                        bitmap.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
                    }
                }
                return bitmap;
            }

            /// <summary>
            /// 平均分割图片
            /// </summary>
            /// <param name="RowNum">水平上分割数</param>
            /// <param name="ColNum">垂直上分割数</param>
            /// <returns>分割好的图片数组</returns>
            public Bitmap[] GetSplitPics(Bitmap bitmap, int RowNum, int ColNum)
            {
                if (RowNum == 0 || ColNum == 0)
                    return null;
                int singW = bitmap.Width / RowNum;
                int singH = bitmap.Height / ColNum;
                Bitmap[] PicArray = new Bitmap[RowNum * ColNum];

                Rectangle cloneRect;
                for (int i = 0; i < ColNum; i++)      //找有效区
                {
                    for (int j = 0; j < RowNum; j++)
                    {
                        cloneRect = new Rectangle(j * singW, i * singH, singW, singH);
                        PicArray[i * RowNum + j] = bitmap.Clone(cloneRect, bitmap.PixelFormat);//复制小块图
                    }
                }
                return PicArray;
            }

            public string GetExpectText(string wordText, int[] expectIndexs)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var index in expectIndexs)
                {
                    if (wordText.Length > index) // to avoid out of array
                    {
                        sb.Append(wordText[index]);
                    }
                }
                return sb.ToString();
            }
        }
    
}
