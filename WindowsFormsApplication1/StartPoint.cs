using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using log4net;

using Total;

namespace WindowsFormsApplication1
{
    public partial class StartPoint : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StartPoint));
        public static Thread instagramT;
        public static Thread clienT;
        public static RichTextBox rtb;
        public static TextBox tb;
        public static DateTimePicker dtpStart;
        public static DateTimePicker dtpEnd;
        public static Panel pan;
        public static Button btStart;
        public static Button btEnd;

        public StartPoint()
        {
            InitializeComponent();
            rtb = logBox;
            tb = inputName;
            dtpStart = startDate;
            dtpEnd = endDate;
            pan = panel1;
            btStart = submit;
            btEnd = stopBtn;
            Common.DisableButtonWhenEnd();

            try
            {
                ConnectDB.connect();
                Common.PrintInfo("Started Application", rtb, typeof(StartPoint));
                
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, rtb, typeof(StartPoint));
            }
        }
        
        public void SubmitClick(object sender, EventArgs e)
        {
            string crawlingChannel = GetChannel();
            
            if(crawlingChannel == "instagram")
            {
                instagramT = new Thread(InstagramThread);
                instagramT.Start();
            }
            else if(crawlingChannel == "clien")
            {
                clienT = new Thread(ClienThread);
                clienT.Start();
            }
            else
            {
                Common.PrintWarn("잘못된 입력입니다.", rtb, typeof(StartPoint));
            }
        }
        private void InstagramThread()
        {
            InstagramMain im = new InstagramMain();
            im.StartInstagram(inputName.Text, GetStartDate(), GetEndDate());
        }

        private void ClienThread()
        {
            ClienMain cm = new ClienMain();
            cm.StartClien(inputName.Text, GetStartDate(), GetEndDate());
        }

        private string GetChannel()
        {
            if (instagramRadioButton.Checked)
            {
                return instagramRadioButton.Text;
            }
            
            return clienRadioButton.Text;
        }

        public void StopClick(object sender, EventArgs e)
        {
            try
            {
                Common.DisableButtonWhenEnd();

                if (instagramRadioButton.Checked)
                {
                    instagramT.Abort();
                    Common.PrintInfo("Abort instagram thread", rtb, typeof(StartPoint));
                    return;
                }

                clienT.Abort();
                Common.PrintInfo("Abort clien thread", rtb, typeof(StartPoint));
            }
            catch(ThreadAbortException tae)
            {
                Common.PrintWarn(tae.Message, rtb, typeof(StartPoint));
            }
        }

        private int GetStartDate()
        {
            string[] arr = startDate.Text
                    .Replace("년", "")
                    .Replace("월", "")
                    .Replace("일", "")
                    .Split(' ');

            if (arr[1].Length == 1)
            {
                arr[1] = "0" + arr[1];
            }
            if (arr[2].Length == 1)
            {
                arr[2] = "0" + arr[2];
            }

            return Convert.ToInt32(arr[0] + arr[1] + arr[2]);
        }

        private int GetEndDate()
        {
            string[] arr = endDate.Text
                    .Replace("년", "")
                    .Replace("월", "")
                    .Replace("일", "")
                    .Split(' ');

            if (arr[1].Length == 1)
            {
                arr[1] = "0" + arr[1];
            }
            if (arr[2].Length == 1)
            {
                arr[2] = "0" + arr[2];
            }

            return Convert.ToInt32(arr[0] + arr[1] + arr[2]);
        }
    }
}
