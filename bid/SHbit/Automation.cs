using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using tessnet2;
using System.IO;

namespace SHbit
{
    public class Automation
    {
        #region UI
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        #endregion

        //phase I
        private BidControl ph1InpPriceTxBox;
        private BidControl ph1DblInpPriceTxBox;
        private BidControl ph1SendPriceButton;

        //phase II
        private BidControl ph2SendPriceButton;     
        private BidControl increase300Button;
        private BidControl ph2InpPriceTxBox;
        private BidControl ph2IncresePriceTxBox;
        private BidControl ph2IncreseButton;

        //VerifyCode Window
        private BidControl vfyCdWndInpTxBox;
        private BidControl vfyCdWndYesBtn;
        private BidControl vfyCdWndNoBtn;
        private BidControl errorWindowButton;

        private BidControl loginUserText;
        private BidControl loginPassText;

        private string imageFullPath;
        private bool isSendPrice;
        private int internalMillSecs;
        private int positionX;
        private int positionY;
        private string warningPrice;
        private DateTime warningTime;
        private bool useNewStragety;

        public bool UseNewStragety
        {
            get { return useNewStragety; }
            set { useNewStragety = value; }
        }

        private const string PHASEII = "2";
        private const string PHASEIII = "3";

        public Automation()
        {
            ConfigurationManager.RefreshSection("appSettings");
            isSendPrice = bool.Parse(ConfigurationManager.AppSettings["isSendPrice"]);
            warningPrice = ConfigurationManager.AppSettings["warningPrice"];
            imageFullPath = Path.Combine(ConfigurationManager.AppSettings["imagePath"], "1.bmp");
            warningTime = DateTime.Parse(ConfigurationManager.AppSettings["warningTime"]);
            useNewStragety = true;
            internalMillSecs = 500;
            positionX = 220;
            positionY = 70;
            if (!Directory.Exists(ConfigurationManager.AppSettings["imagePath"]))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["imagePath"]);
            }

            getAllBidControls();
        }

        public string getLatestPrice()
        {
            string priceValue = null;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                string file = ConfigurationManager.AppSettings["latestPriceFile"];
                fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                priceValue = sr.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                MessageBox.Show("Failed to get latest price");
            }
            finally
            {
                fs.Close();
                sr.Close();
            }

            return priceValue;
        }

        private DateTime GetSecondBidTime()
        {
            return DateTime.Parse(Program.MyForm.SecBidTimeTextBox.Text).AddSeconds(double.Parse(Program.MyForm.SecBidAdSecsTextBox.Text) * (-1));
        }

        private DateTime GetThirdBidTime()
        {
            return DateTime.Parse(Program.MyForm.ThirdBidTimeTextBox.Text).AddSeconds(double.Parse(Program.MyForm.ThirdBidAdSecsTextBox.Text) * (-1));
        }

        private int GetSecondPreBidPrice()
        {
            return (int.Parse(getLatestPrice()) + int.Parse(Program.MyForm.DeltaPriceSecTextBox.Text));
        }

        private int GetSecondIncreasePriceValue()
        {
            return int.Parse(Program.MyForm.DeltaPriceSecTextBox.Text);
        }

        private int GetThirdPreBidPrice()
        {
            return (int.Parse(getLatestPrice()) + int.Parse(Program.MyForm.DeltaPriceThirdTextBox.Text));
        }

        private int GetThirdIncreasePriceValue()
        {
            return int.Parse(Program.MyForm.DeltaPriceThirdTextBox.Text);
        }

        private void CleanUpOldImage(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        private bool isSecondBidTimeArrived()
        {
            return (DateTime.Now >= GetSecondBidTime() ? true : false);
        }

        private bool isThirdBidTimeArrived()
        {
            return (DateTime.Now >= GetThirdBidTime() ? true : false);
        }

        private bool isWarningTimePassed()
        {
            return (DateTime.Now >= this.warningTime ? true : false);
        }

        public void BidFirstPrice()
        {
            CleanUpOldImage(this.imageFullPath);
            sendPh1Price();
        }

        public void BidSecondPrice()
        {
            CleanUpOldImage(this.imageFullPath);
            while (!isSecondBidTimeArrived())
            {
            }
            if (useNewStragety)
            {
                sendPrice(PHASEII);
            }
            else
            {
                sendPriceWithOldStrategy(PHASEII);
            }
        }

        public void BidThirdPrice()
        {
            CleanUpOldImage(this.imageFullPath);
            while (!isThirdBidTimeArrived())
            {

            }

            if (useNewStragety)
            {
                if (isWarningTimePassed())
                {
                    sendPriceWithIncrease300Strategy(PHASEIII);
                }
                else
                {
                    sendPrice(PHASEIII);
                }
            }
            else {
                sendPriceWithOldStrategy(PHASEIII);
            }

            
        }
        
        //Not implemented
        public void BidAll()
        {
            MessageBox.Show("Not implemented.");
        }

        private void sendPh1Price()
        {
            try
            {
                initialClicktoSetFocus(this.positionX, this.positionY);
                Thread.Sleep(internalMillSecs);

                getFocusOnPh1InputPriceTextBox();
                inputSomething(warningPrice);
                Thread.Sleep(internalMillSecs);

                getFocusOnPh1DblInputPriceTextBox();
                inputSomething(warningPrice);

                clickPh1SendPriceButton();
                inputVerifyCode();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }

        private void sendPrice(string phase)
        {
            try{
            initialClicktoSetFocus(this.positionX, this.positionY);

            //Thread.Sleep(internalMillSecs);
            //getFocusOnPh2InputPriceTextBox(); this is for input price text box
            //this is for increase price text box
            getFocusOnPh2IncreasePriceTextBox();
            switch (phase)
            {
                case PHASEII:
                    inputSomething(GetSecondIncreasePriceValue().ToString());
                    break;
                case PHASEIII:
                    inputSomething(GetThirdIncreasePriceValue().ToString());
                    break;
                default:
                    break;
            }
            
            Thread.Sleep(internalMillSecs);
            clickPh2IncreasePriceButton();
                //Do we need to click send price button again?
            clickPh2SendPriceButton();

            inputVerifyCode();
                        }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }

        public void ClickSendPrice()
        {
            try
            {
                clickPh2SendPriceButton();

                inputVerifyCode();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }

        private void sendPriceWithIncrease300Strategy(string phase)
        {
            try
            {
                initialClicktoSetFocus(this.positionX, this.positionY);
                switch (phase)
                {
                    case PHASEII:
                        click300Button();
                        break;
                    case PHASEIII:
                        click300Button();
                        break;
                    default:
                        break;
                }

                Thread.Sleep(internalMillSecs);
                clickPh2SendPriceButton();

                inputVerifyCode();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }

        private void sendPriceWithOldStrategy(string phase)
        {
            try
            {
                initialClicktoSetFocus(this.positionX, this.positionY);

                //Thread.Sleep(internalMillSecs);
                getFocusOnPh2InputPriceTextBox(); 

                switch (phase)
                {
                    case PHASEII:
                        inputSomething(GetSecondPreBidPrice().ToString());
                        break;
                    case PHASEIII:
                        inputSomething(GetThirdPreBidPrice().ToString());
                        break;
                    default:
                        break;
                }

                Thread.Sleep(internalMillSecs);
                clickPh2SendPriceButton();

                inputVerifyCode();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }

        private void inputVerifyCode()
        {
            try
            {
                while (true)
                {
                    if (File.Exists(this.imageFullPath))
                    {
                        break;
                    }
                }

                getFocusOnVerifyCodeWindowTextBox();
                inputSomething(getVerifyCode(this.imageFullPath));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
            //if (isSendPrice)
            //{
            //    clickLeftMouse(vfyCdWndYesBtn.XPoint, vfyCdWndYesBtn.YPoint);
            //}
        }

        private void getFocusOnPh2InputPriceTextBox()
        {
            clickLeftMouse(ph2InpPriceTxBox.XPoint, ph2InpPriceTxBox.YPoint);
            clickLeftMouse(ph2InpPriceTxBox.XPoint, ph2InpPriceTxBox.YPoint);
        }

        private void getFocusOnPh2IncreasePriceTextBox()
        {
            clickLeftMouse(ph2IncresePriceTxBox.XPoint, ph2IncresePriceTxBox.YPoint);
            clickLeftMouse(ph2IncresePriceTxBox.XPoint, ph2IncresePriceTxBox.YPoint);
        }

        private void getFocusOnVerifyCodeWindowTextBox()
        {
            clickLeftMouse(vfyCdWndInpTxBox.XPoint, vfyCdWndInpTxBox.YPoint);
            clickLeftMouse(vfyCdWndInpTxBox.XPoint, vfyCdWndInpTxBox.YPoint);
        }

        private void getFocusOnPh1InputPriceTextBox()
        {
            clickLeftMouse(ph1InpPriceTxBox.XPoint, ph1InpPriceTxBox.YPoint);
            clickLeftMouse(ph1InpPriceTxBox.XPoint, ph1InpPriceTxBox.YPoint);
        }

        private void getFocusOnPh1DblInputPriceTextBox()
        {
            clickLeftMouse(ph1DblInpPriceTxBox.XPoint, ph1DblInpPriceTxBox.YPoint);
            clickLeftMouse(ph1DblInpPriceTxBox.XPoint, ph1DblInpPriceTxBox.YPoint);
        }

        private void clickPh2SendPriceButton()
        {
            clickLeftMouse(ph2SendPriceButton.XPoint, ph2SendPriceButton.YPoint);
        }

        private void clickPh2IncreasePriceButton()
        {
            clickLeftMouse(ph2IncreseButton.XPoint, ph2IncreseButton.YPoint);
        }

        private void click300Button()
        {
            clickLeftMouse(increase300Button.XPoint, increase300Button.YPoint);
        }

        private void clickPh1SendPriceButton()
        {
            clickLeftMouse(ph1SendPriceButton.XPoint, ph1SendPriceButton.YPoint);
            
        }

        private void initialClicktoSetFocus(int x, int y)
        {
            clickLeftMouse(x, y);
        }

        //deprecated method
        public void BidPrice()
        {           
            sendCustomizePrice(isSendPrice);
        }

        //deprecated method
        private void sendCustomizePrice(bool isSendPrice)
        {
            CleanUpOldImage(this.imageFullPath);
            //set focus
            int internalMillSecs = 500;
            //AutomationElement ele = findAppByProcName(ConfigurationManager.AppSettings["procName"]);
            int x = 220;
            int y = 70;
            initialClicktoSetFocus(x, y);

            Thread.Sleep(internalMillSecs);
            //1. add price
            //Get latest price
            string latestPrice = getLatestPrice();
            string deltaPrice = ConfigurationManager.AppSettings["deltaPrice"];
            int sendPrice = int.Parse(latestPrice) + int.Parse(deltaPrice);
            
            //2. Click price text box
            clickLeftMouse(ph2InpPriceTxBox.XPoint, ph2InpPriceTxBox.YPoint);
            clickLeftMouse(ph2InpPriceTxBox.XPoint, ph2InpPriceTxBox.YPoint);
            inputSomething(sendPrice.ToString());

            //3. Click send price button
            Thread.Sleep(internalMillSecs);
            clickLeftMouse(ph2SendPriceButton.XPoint, ph2SendPriceButton.YPoint);
            Thread.Sleep(internalMillSecs);            

            while (true)
            {
                if (File.Exists(this.imageFullPath))
                {
                    break;
                }
            }

            Thread.Sleep(internalMillSecs);

            //3. analyze verify pic
            string verifyCode = getVerifyCode(this.imageFullPath);

            //4. input verify code
            clickLeftMouse(vfyCdWndInpTxBox.XPoint, vfyCdWndInpTxBox.YPoint);
            clickLeftMouse(vfyCdWndInpTxBox.XPoint, vfyCdWndInpTxBox.YPoint);
            Thread.Sleep(internalMillSecs);
            inputSomething(verifyCode);

            if (isSendPrice)
            {
                clickLeftMouse(vfyCdWndYesBtn.XPoint, vfyCdWndYesBtn.YPoint);
            }
       
        }

        public void UnitTest()
        {
            try
            {
                MessageBox.Show("Just to do unit test");
                CleanUpOldImage(this.imageFullPath);
                while (!isSecondBidTimeArrived())
                {
                }
                MessageBox.Show("System time now: " + DateTime.Now + "\n Sec Bid time: " + GetSecondBidTime());
                MessageBox.Show("Send out sec price: " + GetSecondPreBidPrice());
                while (!isThirdBidTimeArrived())
                {
                }
                MessageBox.Show("System time now: " + DateTime.Now + "\n Third Bid time: " + GetThirdBidTime());
                MessageBox.Show("Send out third price: " + GetThirdPreBidPrice());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }
        //deprecated method
        public void test()
        {
            TimeSpan startTime = DateTime.Now.TimeOfDay;
            //set focus
            int time = 500;
            AutomationElement ele = findAppByProcName(ConfigurationManager.AppSettings["procName"]);
            int x = 220;
            int y = 70;
            clickLeftMouse(x, y);

            //1. input price
            clickLeftMouse(ph1InpPriceTxBox.XPoint, ph1InpPriceTxBox.YPoint);
            inputSomething("80000");
            Thread.Sleep(time);

            //2. input price again
            clickLeftMouse(ph1DblInpPriceTxBox.XPoint, ph1DblInpPriceTxBox.YPoint);
            inputSomething("80000");
            Thread.Sleep(time);

            clickLeftMouse(ph2SendPriceButton.XPoint, ph2SendPriceButton.YPoint);
            Thread.Sleep(time);

            while (true)
            {
                if (File.Exists(this.imageFullPath))
                {
                    break;
                }
            }

            //3. analyze verify pic
            string verifyCode = getVerifyCode(this.imageFullPath);

            //4. input verify code
            clickLeftMouse(vfyCdWndInpTxBox.XPoint, vfyCdWndInpTxBox.YPoint);
            clickLeftMouse(vfyCdWndInpTxBox.XPoint, vfyCdWndInpTxBox.YPoint);

            inputSomething(verifyCode);
            TimeSpan endTime = DateTime.Now.TimeOfDay;

            MessageBox.Show("共耗时: " + endTime.Subtract(startTime) + "秒.");
        }

        /// <summary>
        /// 延时函数
        /// </summary>
        /// <param name="delayTime">需要延时多少秒</param>
        /// <returns></returns>
        /// deprecated method
        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                //Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }

        public void login()
        {
            int time = 500;
            AutomationElement ele = findAppByProcName(ConfigurationManager.AppSettings["procName"]);
            int x = 220;
            int y = 70;
            clickLeftMouse(x, y);

            string user = ConfigurationManager.AppSettings["user"];
            string pass = ConfigurationManager.AppSettings["password"];

            clickLeftMouse(loginUserText.XPoint, loginUserText.YPoint);
            Thread.Sleep(time);
            inputSomething(user);

            clickLeftMouse(loginPassText.XPoint, loginPassText.YPoint);
            Thread.Sleep(time);
            inputSomething(pass);
        }

        public void clickLeftMouse(int IncrementX, int IncrementY)
        {
            SetCursorPos(IncrementX, IncrementY);
            mouse_event(MOUSEEVENTF_LEFTDOWN, IncrementX, IncrementY, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, IncrementX, IncrementY, 0, 0);
        }

        public void inputSomething(string text)
        {
            SendKeys.SendWait(text);
            SendKeys.Flush();
        }

        //need to test
        public void sendBackspace()
        {
            SendKeys.SendWait("{Backspace}");
            SendKeys.SendWait("{Backspace}");
            SendKeys.SendWait("{Backspace}");
            SendKeys.SendWait("{Backspace}");
            SendKeys.SendWait("{Backspace}");
            SendKeys.Flush();
        }

        public AutomationElement findAppByProcId(int id)
        {
            Process p = Process.GetProcessById(id);
            SetForegroundWindow(p.MainWindowHandle);
            return AutomationElement.FromHandle(p.MainWindowHandle);
        }

        public AutomationElement findAppByProcName(string name)
        {
            Process p = Process.GetProcessesByName(name)[0];
            SetForegroundWindow(p.MainWindowHandle);
            return AutomationElement.FromHandle(p.MainWindowHandle);
        }

        public bool findWindowsElementTest(string caption)
        {
            AutomationElement root = AutomationElement.RootElement;
            try
            {
                AutomationElement verifyWindow = null;
                AutomationElement webBox = root.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Shanghai Bit"));
                while (verifyWindow == null)
                {
                    verifyWindow = webBox.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, " -- 网页对话框"));
                    if (verifyWindow != null)
                    {
                        MessageBox.Show(verifyWindow.ToString());
                        break;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public void getAllBidControls()
        {
            ph1InpPriceTxBox = new BidControl("ph1InpPriceTxBox");
            ph1DblInpPriceTxBox = new BidControl("ph1DblInpPriceTxBox");
            ph1SendPriceButton = new BidControl("ph1SendPriceButton");

            ph2SendPriceButton = new BidControl("ph2SendPriceButton");         
            increase300Button = new BidControl("increase300Button");
            ph2InpPriceTxBox = new BidControl("ph2InpPriceTxBox");
            ph2IncreseButton = new BidControl("ph2IncreseButton");
            ph2IncresePriceTxBox = new BidControl("ph2IncresePriceTxBox");

            vfyCdWndInpTxBox = new BidControl("verifyCodeWindowInputTextBox");
            vfyCdWndYesBtn = new BidControl("verifyCodeWindowYesButton");
            vfyCdWndNoBtn = new BidControl("verifyCodeWindowNoButton");
            errorWindowButton = new BidControl("errorWindowButton");

            loginUserText = new BidControl("loginUserPoint");
            loginPassText = new BidControl("passwordPoint");        
        }

        public String getVerifyCode(String sourcePic)
        {
            string code = string.Empty;
            DecodeHelper UnCodeExtendObj = new DecodeHelper();
            Bitmap image = (Bitmap)Bitmap.FromFile(sourcePic);
            var validIndexs = UnCodeExtendObj.GetValidCharIndex(image);
            Bitmap grayBitmap = UnCodeExtendObj.SetGrayByPixels(image);
            Bitmap noNoiseBitmap = UnCodeExtendObj.ClearNoise(grayBitmap);
            Bitmap validRegionBitmap = UnCodeExtendObj.GetValidRegionPic(noNoiseBitmap);
            List<Word> result = UnCodeExtendObj.GetWordFromImage(validRegionBitmap);
            if (result.Count >= 1)
            {
                string text = UnCodeExtendObj.GetExpectText(result[0].Text, validIndexs);
                code = text;
                if (text.Length == 4)
                {
                    Bitmap orignalImage = (Bitmap)Bitmap.FromFile(sourcePic);
                    string testResultDirectory = Path.Combine(@"D:\", "TestResult");
                    if (!Directory.Exists(testResultDirectory))
                    {
                        Directory.CreateDirectory(testResultDirectory);
                    }
                    orignalImage.Save(Path.Combine(testResultDirectory, text + " - " + Guid.NewGuid() + ".bmp"));
                }
                else
                {
                    Console.WriteLine(string.Format("cannot parse: {0}, didn't get enough text from word", sourcePic));
                }
            }
            else
            {
                Console.WriteLine(string.Format("cannot parse: {0}, didn't get word from image", sourcePic));
            }

            return code;
        }
    }
}
