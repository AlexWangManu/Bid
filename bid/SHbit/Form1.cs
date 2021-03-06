﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SHbit
{
    public partial class Form1 : Form
    {
        #region controls
        // The remaining code in this file provides basic form initialization and  
        // includes a Main method. If you use the Visual Studio designer to create 
        // your form, you can use the designer generated code instead of this code,  
        // but be sure to use the names shown in the variable declarations here, 
        // and be sure to attach the event handlers to the associated events.  

        private WebBrowser webBrowser1;

        private ToolStrip toolStrip1, toolStrip2;
        private ToolStripTextBox uriTextBox;

        public ToolStripTextBox UriTextBox
        {
            get { return uriTextBox; }
            set { uriTextBox = value; }
        }

        private ToolStripLabel secBidLabel;
        private ToolStripTextBox secBidTimeTextBox;

        public ToolStripTextBox SecBidTimeTextBox
        {
            get { return secBidTimeTextBox; }
            set { secBidTimeTextBox = value; }
        }
        private ToolStripTextBox secBidAdSecsTextBox;

        public ToolStripTextBox SecBidAdSecsTextBox
        {
            get { return secBidAdSecsTextBox; }
            set { secBidAdSecsTextBox = value; }
        }
        private ToolStripLabel thirdBidLabel;
        private ToolStripTextBox thirdBidTimeTextBox;

        public ToolStripTextBox ThirdBidTimeTextBox
        {
            get { return thirdBidTimeTextBox; }
            set { thirdBidTimeTextBox = value; }
        }
        private ToolStripTextBox thirdBidAdSecsTextBox;

        public ToolStripTextBox ThirdBidAdSecsTextBox
        {
            get { return thirdBidAdSecsTextBox; }
            set { thirdBidAdSecsTextBox = value; }
        }
        private ToolStripLabel intervalLabel1;
        private ToolStripLabel intervalLabel2;
        private ToolStripLabel deltaPriceSecLabel;
        private ToolStripLabel deltaPriceThirdLabel;
        private ToolStripTextBox deltaPriceSecTextBox;

        public ToolStripTextBox DeltaPriceSecTextBox
        {
            get { return deltaPriceSecTextBox; }
            set { deltaPriceSecTextBox = value; }
        }
        private ToolStripTextBox deltaPriceThirdTextBox;

        public ToolStripTextBox DeltaPriceThirdTextBox
        {
            get { return deltaPriceThirdTextBox; }
            set { deltaPriceThirdTextBox = value; }
        }

        private ToolStripLabel timelabel1;
        private ToolStripButton startBidButton;
        private ToolStripButton loginbutton1;
        private ToolStripButton testButton;
        private ToolStripButton BidFirstButton;
        private ToolStripButton BidSecButton;
        private ToolStripButton BidThirdButton;
        private ToolStripButton BidAllButton;

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        //private ToolStripButton goButton, backButton,
        //    forwardButton, stopButton, refreshButton,
        //    homeButton, searchButton, printButton;
        //private MenuStrip menuStrip1;
        //private ToolStripMenuItem fileToolStripMenuItem,
        //    saveAsToolStripMenuItem, printToolStripMenuItem,
        //    printPreviewToolStripMenuItem, exitToolStripMenuItem,
        //    pageSetupToolStripMenuItem, propertiesToolStripMenuItem;
        //private ToolStripSeparator toolStripSeparator1, toolStripSeparator2;
        #endregion

        public Form1()
        {
            // Create the form layout. If you are using Visual Studio,  
            // you can replace this code with code generated by the designer. 
            InitializeForm();
            InitializeComponent();
            // The following events are not visible in the designer, so  
            // you must associate them with their event-handlers in code.
            webBrowser1.CanGoBackChanged +=
                new EventHandler(webBrowser1_CanGoBackChanged);
            webBrowser1.CanGoForwardChanged +=
                new EventHandler(webBrowser1_CanGoForwardChanged);
            webBrowser1.DocumentTitleChanged +=
                new EventHandler(webBrowser1_DocumentTitleChanged);
            webBrowser1.StatusTextChanged +=
                new EventHandler(webBrowser1_StatusTextChanged);
            startBidButton.Click += new EventHandler(startBidButton_Click);
            loginbutton1.Click += new EventHandler(loginbutton1_Click);
            testButton.Click += new EventHandler(testButton_Click);
            BidSecButton.Click += BidSecButton_Click;
            BidThirdButton.Click += BidThirdButton_Click;
            BidAllButton.Click += BidAllButton_Click;
            BidFirstButton.Click += BidFirstButton_Click;
            // Load the user's home page.
           // webBrowser1.GoHome();
        }

        private void BidFirstButton_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                Thread t = new Thread(auto.BidFirstPrice);
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void BidAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                Thread t = new Thread(auto.ClickSendPrice);
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void BidThirdButton_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                auto.UseNewStragety = true;
                Thread t = new Thread(auto.BidThirdPrice);
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void BidSecButton_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                auto.UseNewStragety = true;
                Thread t = new Thread(auto.BidSecondPrice);
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                auto.UseNewStragety = false;
                Thread t = new Thread(auto.BidSecondPrice);
                t.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void startBidButton_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                Thread t = new Thread(auto.BidPrice);
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }

        }

        private void loginbutton1_Click(object sender, EventArgs e)
        {
            try
            {
                Automation auto = new Automation();
                auto.login();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        } 
     
        // Displays the Save dialog box. 
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowSaveAsDialog();
        }

        // Displays the Page Setup dialog box. 
        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPageSetupDialog();
        }

        // Displays the Print dialog box. 
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        // Displays the Print Preview dialog box. 
        private void printPreviewToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        // Displays the Properties dialog box. 
        private void propertiesToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            webBrowser1.ShowPropertiesDialog();
        }

        // Selects all the text in the text box when the user clicks it.  
        private void uriTextBox_Click(object sender, EventArgs e)
        {
            uriTextBox.SelectAll();
        }

        // Navigates to the URL in the address box when  
        // the ENTER key is pressed while the ToolStripTextBox has focus. 
        private void uriTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Navigate(uriTextBox.Text);
            }
        }

        // Navigates to the URL in the address box when  
        // the Go button is clicked. 
        private void goButton_Click(object sender, EventArgs e)
        {
            Navigate(uriTextBox.Text);
        }

        // Navigates to the given URL if it is valid. 
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://") && !address.Contains("C:") && !address.Contains("D:") && !address.Contains("E:") && !address.Contains("F:"))
            {
                address = "http://" + address;
            }
            try
            {
                
                webBrowser1.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }

        // Updates the URL in TextBoxAddress upon navigation. 
        private void webBrowser1_Navigated(object sender,
            WebBrowserNavigatedEventArgs e)
        {
            uriTextBox.Text = webBrowser1.Url.ToString();
        }

        // Navigates webBrowser1 to the previous page in the history. 
        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        // Disables the Back button at the beginning of the navigation history. 
        private void webBrowser1_CanGoBackChanged(object sender, EventArgs e)
        {
            //backButton.Enabled = webBrowser1.CanGoBack;
        }

        // Navigates webBrowser1 to the next page in history. 
        private void forwardButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        // Disables the Forward button at the end of navigation history. 
        private void webBrowser1_CanGoForwardChanged(object sender, EventArgs e)
        {
           // forwardButton.Enabled = webBrowser1.CanGoForward;
        }

        // Halts the current navigation and any sounds or animations on  
        // the page. 
        private void stopButton_Click(object sender, EventArgs e)
        {
            webBrowser1.Stop();
        }

        // Reloads the current page. 
        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Skip refresh if about:blank is loaded to avoid removing 
            // content specified by the DocumentText property. 
            if (!webBrowser1.Url.Equals("about:blank"))
            {
                webBrowser1.Refresh();
            }
        }

        // Navigates webBrowser1 to the home page of the current user. 
        private void homeButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        // Navigates webBrowser1 to the search page of the current user. 
        private void searchButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoSearch();
        }

        // Prints the current document using the current print settings. 
        private void printButton_Click(object sender, EventArgs e)
        {
            webBrowser1.Print();
        }

        // Updates the status bar with the current browser status text. 
        private void webBrowser1_StatusTextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = webBrowser1.StatusText;
        }

        // Updates the title bar with the current document title. 
        private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {
            //this.Text = webBrowser1.DocumentTitle;
        }

        // Exits the application. 
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timelabel1.Text = System.DateTime.Now.ToLocalTime().ToString();
        }

        private void InitializeForm()
        {
            webBrowser1 = new WebBrowser();
   
            #region deprecated
            //menuStrip1 = new MenuStrip();
            //fileToolStripMenuItem = new ToolStripMenuItem();
            //saveAsToolStripMenuItem = new ToolStripMenuItem();
            //toolStripSeparator1 = new ToolStripSeparator();
            //printToolStripMenuItem = new ToolStripMenuItem();
            //printPreviewToolStripMenuItem = new ToolStripMenuItem();
            //toolStripSeparator2 = new ToolStripSeparator();
            //exitToolStripMenuItem = new ToolStripMenuItem();
            //pageSetupToolStripMenuItem = new ToolStripMenuItem();
            //propertiesToolStripMenuItem = new ToolStripMenuItem();       

            //goButton = new ToolStripButton();
            //backButton = new ToolStripButton();
            //forwardButton = new ToolStripButton();
            //stopButton = new ToolStripButton();
            //refreshButton = new ToolStripButton();
            //homeButton = new ToolStripButton();
            //searchButton = new ToolStripButton();
            //printButton = new ToolStripButton();
            #endregion

            toolStrip1 = new ToolStrip();
            startBidButton = new ToolStripButton();
            loginbutton1 = new ToolStripButton();
            timelabel1 = new ToolStripLabel();
            testButton = new ToolStripButton();
            BidSecButton = new ToolStripButton();
            BidThirdButton = new ToolStripButton();
            BidAllButton = new ToolStripButton();
            BidFirstButton = new ToolStripButton();
            startBidButton.Text = "Start Bid";
            loginbutton1.Text = "Login";
            testButton.Text = "出价(PhaseII Old)";
            BidFirstButton.Text = "自动出价(第一次)";
            BidSecButton.Text = "自动出价(第二次)";
            BidThirdButton.Text = "自动出价(第三次)";
            BidAllButton.Text = "推送价格";
            timelabel1.Text = System.DateTime.Now.ToLocalTime().ToString();

            toolStrip2 = new ToolStrip();
            uriTextBox = new ToolStripTextBox();
            uriTextBox.Size = new System.Drawing.Size(120, 25);
            secBidLabel = new ToolStripLabel();
            secBidLabel.Text = "第二次(时间):";
            thirdBidLabel = new ToolStripLabel();
            thirdBidLabel.Text = "第三次（时间）:";
            secBidTimeTextBox = new ToolStripTextBox();
            secBidTimeTextBox.Size = new System.Drawing.Size(50, 25);
            thirdBidTimeTextBox = new ToolStripTextBox();
            thirdBidTimeTextBox.Size = new System.Drawing.Size(50, 25);
            secBidAdSecsTextBox = new ToolStripTextBox();
            secBidAdSecsTextBox.Size = new System.Drawing.Size(20, 25);
            thirdBidAdSecsTextBox = new ToolStripTextBox();
            thirdBidAdSecsTextBox.Size = new System.Drawing.Size(20, 25);
            intervalLabel1 = new ToolStripLabel();
            intervalLabel1.Text = "提前（秒）";
            intervalLabel2 = new ToolStripLabel();
            intervalLabel2.Text = "提前（秒）";
            deltaPriceSecLabel = new ToolStripLabel();
            deltaPriceSecLabel.Text = "加价";
            deltaPriceThirdLabel = new ToolStripLabel();
            deltaPriceThirdLabel.Text = "加价";
            deltaPriceSecTextBox = new ToolStripTextBox();
            deltaPriceSecTextBox.Size = new System.Drawing.Size(50, 25);
            deltaPriceThirdTextBox = new ToolStripTextBox();
            deltaPriceThirdTextBox.Size = new System.Drawing.Size(50, 25);

            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();


            toolStrip1.Items.AddRange(new ToolStripItem[] { BidFirstButton, loginbutton1, testButton, timelabel1, BidSecButton, BidThirdButton, BidAllButton });

            #region deprecated
            //toolStrip1.Items.AddRange(new ToolStripItem[] {
            //goButton, backButton, forwardButton, stopButton,
            //refreshButton, homeButton, searchButton, printButton});

            //menuStrip1.Items.Add(fileToolStripMenuItem);

            //fileToolStripMenuItem.DropDownItems.AddRange(
            //    new ToolStripItem[] {
            //    saveAsToolStripMenuItem, toolStripSeparator1, 
            //    pageSetupToolStripMenuItem, printToolStripMenuItem, 
            //    printPreviewToolStripMenuItem, toolStripSeparator2,
            //    propertiesToolStripMenuItem, exitToolStripMenuItem
            //});

            //fileToolStripMenuItem.Text = "&File";
            //saveAsToolStripMenuItem.Text = "Save &As...";
            //pageSetupToolStripMenuItem.Text = "Page Set&up...";
            //printToolStripMenuItem.Text = "&Print...";
            //printPreviewToolStripMenuItem.Text = "Print Pre&view...";
            //propertiesToolStripMenuItem.Text = "Properties";
            //exitToolStripMenuItem.Text = "E&xit";

            //printToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;

            //saveAsToolStripMenuItem.Click +=
            //    new System.EventHandler(saveAsToolStripMenuItem_Click);
            //pageSetupToolStripMenuItem.Click +=
            //    new System.EventHandler(pageSetupToolStripMenuItem_Click);
            //printToolStripMenuItem.Click +=
            //    new System.EventHandler(printToolStripMenuItem_Click);
            //printPreviewToolStripMenuItem.Click +=
            //    new System.EventHandler(printPreviewToolStripMenuItem_Click);
            //propertiesToolStripMenuItem.Click +=
            //    new System.EventHandler(propertiesToolStripMenuItem_Click);
            //exitToolStripMenuItem.Click +=
            //    new System.EventHandler(exitToolStripMenuItem_Click);

            //goButton.Text = "Go";
            //backButton.Text = "Back";
            //forwardButton.Text = "Forward";
            //stopButton.Text = "Stop";
            //refreshButton.Text = "Refresh";
            //homeButton.Text = "Home";
            //searchButton.Text = "Search";
            //printButton.Text = "Print";

            //backButton.Enabled = false;
            //forwardButton.Enabled = false;

            //goButton.Click += new System.EventHandler(goButton_Click);
            //backButton.Click += new System.EventHandler(backButton_Click);
            //forwardButton.Click += new System.EventHandler(forwardButton_Click);
            //stopButton.Click += new System.EventHandler(stopButton_Click);
            //refreshButton.Click += new System.EventHandler(refreshButton_Click);
            //homeButton.Click += new System.EventHandler(homeButton_Click);
            //searchButton.Click += new System.EventHandler(searchButton_Click);
            //printButton.Click += new System.EventHandler(printButton_Click);
            #endregion

            toolStrip2.Items.Add(uriTextBox);
            toolStrip2.Items.Add(secBidLabel);
            toolStrip2.Items.Add(secBidTimeTextBox);
            toolStrip2.Items.Add(deltaPriceSecLabel);
            toolStrip2.Items.Add(deltaPriceSecTextBox);
            toolStrip2.Items.Add(intervalLabel1);
            toolStrip2.Items.Add(secBidAdSecsTextBox);
            toolStrip2.Items.Add(thirdBidLabel);
            toolStrip2.Items.Add(thirdBidTimeTextBox);
            toolStrip2.Items.Add(deltaPriceThirdLabel);
            toolStrip2.Items.Add(deltaPriceThirdTextBox);
            toolStrip2.Items.Add(intervalLabel2);
            toolStrip2.Items.Add(thirdBidAdSecsTextBox);
           // toolStrip2.Items.AddRange(new ToolStripItem[] {uriTextBox,});
            
            uriTextBox.KeyDown +=
                new KeyEventHandler(uriTextBox_KeyDown);
            uriTextBox.Click +=
                new System.EventHandler(uriTextBox_Click);

            statusStrip1.Items.Add(toolStripStatusLabel1);

            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Navigated +=
                new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);

            Size browserSize = new System.Drawing.Size();
            browserSize.Height = 750;
            browserSize.Width = 1100;
            this.Size = browserSize;

            //toolStrip1,menuStrip1,menuStrip1
            Controls.AddRange(new Control[] {
            webBrowser1, toolStrip2, toolStrip1,
             statusStrip1 });

            Navigate(ConfigurationManager.AppSettings["bidUri"]);

        }

    }
}
