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

using WindowsFormsApplication1;

namespace Total
{
    public class Common
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Common));
        public static int successBoardCount = 0;
        public static int successCommentCount = 0;

        public class CommentObject
        {
            public string writer;
            public string contents;
            public string created;
            public int likes;

            public CommentObject(string writer, string contents, string created, int likes)
            {
                this.writer = writer;
                this.contents = contents;
                this.created = created;
                this.likes = likes;
            }

            public void printChild()
            {
                Console.WriteLine("[writer] " + writer);
                Console.WriteLine("[contents]: " + contents);
                Console.WriteLine("[created]: " + created);
                Console.WriteLine("[likes]: " + likes);
                Console.WriteLine();
            }
        }
        
        public static void SleepProgramSeconds(int v)
        {
            Thread.Sleep(v * 1000);
        }
        public static void DisableButtonWhenStart()
        {
            if (StartPoint.tb.InvokeRequired) { StartPoint.tb.Invoke((MethodInvoker)delegate { StartPoint.tb.Enabled = false; }); }
            else { StartPoint.tb.Enabled = false; }
            if (StartPoint.dtpStart.InvokeRequired) { StartPoint.dtpStart.Invoke((MethodInvoker)delegate { StartPoint.dtpStart.Enabled = false; }); }
            else { StartPoint.dtpStart.Enabled = false; }
            if (StartPoint.dtpEnd.InvokeRequired) { StartPoint.dtpEnd.Invoke((MethodInvoker)delegate { StartPoint.dtpEnd.Enabled = false; }); }
            else { StartPoint.dtpEnd.Enabled = false; }
            if (StartPoint.pan.InvokeRequired) { StartPoint.pan.Invoke((MethodInvoker)delegate { StartPoint.pan.Enabled = false; }); }
            else { StartPoint.pan.Enabled = false; }
            if (StartPoint.btStart.InvokeRequired) { StartPoint.btStart.Invoke((MethodInvoker)delegate { StartPoint.btStart.Enabled = false; }); }
            else { StartPoint.btStart.Enabled = false; }
            if (StartPoint.btEnd.InvokeRequired) { StartPoint.btEnd.Invoke((MethodInvoker)delegate { StartPoint.btEnd.Enabled = true; }); }
            else { StartPoint.btEnd.Enabled = true; }

            //StartPoint.tb.Enabled = false;
            //StartPoint.dtpStart.Enabled = false;
            //StartPoint.dtpEnd.Enabled = false;
            //StartPoint.pan.Enabled = false;
            //StartPoint.btStart.Enabled = false;
            //StartPoint.btEnd.Enabled = true;
        }

        public static void DisableButtonWhenEnd()
        {
            if (StartPoint.tb.InvokeRequired) { StartPoint.tb.Invoke((MethodInvoker)delegate { StartPoint.tb.Enabled = true; }); }
            else { StartPoint.tb.Enabled = true; }
            if (StartPoint.dtpStart.InvokeRequired) { StartPoint.dtpStart.Invoke((MethodInvoker)delegate { StartPoint.dtpStart.Enabled = true; }); }
            else { StartPoint.dtpStart.Enabled = true; }
            if (StartPoint.dtpEnd.InvokeRequired) { StartPoint.dtpEnd.Invoke((MethodInvoker)delegate { StartPoint.dtpEnd.Enabled = true; }); }
            else { StartPoint.dtpEnd.Enabled = true; }
            if (StartPoint.pan.InvokeRequired) { StartPoint.pan.Invoke((MethodInvoker)delegate { StartPoint.pan.Enabled = true; }); }
            else { StartPoint.pan.Enabled = true; }
            if (StartPoint.btStart.InvokeRequired) { StartPoint.btStart.Invoke((MethodInvoker)delegate { StartPoint.btStart.Enabled = true; }); }
            else { StartPoint.btStart.Enabled = true; }
            if (StartPoint.btEnd.InvokeRequired) { StartPoint.btEnd.Invoke((MethodInvoker)delegate { StartPoint.btEnd.Enabled = false; }); }
            else { StartPoint.btEnd.Enabled = false; }

            //StartPoint.tb.Enabled = true;
            //StartPoint.dtpStart.Enabled = true;
            //StartPoint.dtpEnd.Enabled = true;
            //StartPoint.pan.Enabled = true;
            //StartPoint.btStart.Enabled = true;
            //StartPoint.btEnd.Enabled = false;
        }

        public static void PrintInfo(string msg, RichTextBox rtb, Type className)
        {
            if(rtb.InvokeRequired == true)
            {
                rtb.Invoke((MethodInvoker)delegate
                {
                    rtb.SelectionColor = Color.Black;
                    rtb.AppendText(
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                        " [INFO] " +
                        className +
                        " - " +
                        msg);
                    rtb.AppendText(Environment.NewLine);
                });
            }
            else
            {
                rtb.SelectionColor = Color.Black;
                rtb.AppendText(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    " [INFO] " +
                    className +
                    " - " +
                    msg);
                rtb.AppendText(Environment.NewLine);
            }
        }

        public static void PrintWarn(string msg, RichTextBox rtb, Type className)
        {
            if (rtb.InvokeRequired == true)
            {
                rtb.Invoke((MethodInvoker)delegate
                {
                    rtb.SelectionColor = Color.Orange;
                    rtb.AppendText(
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                        " [WARN] " +
                        className +
                        " - " +
                        msg);
                    rtb.AppendText(Environment.NewLine);
                });
            }
            else
            {
                rtb.SelectionColor = Color.Orange;
                rtb.AppendText(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    " [WARN] " +
                    className +
                    " - " +
                    msg);
                rtb.AppendText(Environment.NewLine);
            }
        }

        public static void PrintError(string msg, RichTextBox rtb, Type className)
        {
            if(rtb.InvokeRequired == true)
            {
                rtb.Invoke((MethodInvoker)delegate
                {
                    rtb.SelectionColor = Color.Red;
                    rtb.AppendText(
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                        " [ERROR] " +
                        className +
                        " - " +
                        msg);
                    rtb.AppendText(Environment.NewLine);
                });
            }
            else
            {
                rtb.SelectionColor = Color.Red;
                rtb.AppendText(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    " [ERROR] " +
                    className +
                    " - " +
                    msg);
                rtb.AppendText(Environment.NewLine);
            }
        }

        public static string DataPreprocessing(string str)
        {
            try
            {
                string removePhone = Regex.Replace(str, @"\d{2,3}[- ]?\d{3,4}[- ]?\d{4}", "");
                string removeEmail = Regex.Replace(removePhone, @"\w+@\w+\.\w+", "");

                string notKorEngNum = Regex
                    .Replace(removeEmail, @"[^0-9a-zA-Z가-힣 ]", "")
                    .Trim();


                return notKorEngNum;
            }
            catch (Exception exc)
            {
                PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return "";
            }
        }

        public static Guid insertBoard(
            string channelType, string contentsType, 
            string writer, string contents, string created, 
            string tag, int commentCount, 
            string url, string profilePicture, int likes, int hits, string keyword, string pictureAndVideo)
        {
            try
            {
                SqlCommand sqlCommand = ConnectDB.sqlCommand;
                Guid checkCommentId = Guid.NewGuid();
                DateTime currentDate = DateTime.Now;

                string formatDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                sqlCommand.Parameters.Clear();

                string query =
                  "insert into dbo.tbmskim "
                + "(channelType, contentsType, checkCommentId, writer, contents, created, tag, commentCount, url, profilePicture, likes, crawlingDate, hits, keyword, pictureAndVideo)"
                + "values (@channelType, @contentsType, @checkCommentId, @writer, @contents, @created, @tag, @commentCount, @url, @profilePicture, @likes, GETDATE(), @hits, @keyword, @pictureAndVideo)";
                sqlCommand.CommandText = query;

                sqlCommand.Parameters.AddWithValue("@channelType", channelType);
                sqlCommand.Parameters.AddWithValue("@contentsType", contentsType);
                sqlCommand.Parameters.AddWithValue("@checkCommentId", checkCommentId);
                sqlCommand.Parameters.AddWithValue("@writer", writer);
                sqlCommand.Parameters.AddWithValue("@contents", contents);
                sqlCommand.Parameters.AddWithValue("@created", created);
                sqlCommand.Parameters.AddWithValue("@tag", tag);
                sqlCommand.Parameters.AddWithValue("@commentCount", commentCount);
                sqlCommand.Parameters.AddWithValue("@url", url);
                sqlCommand.Parameters.AddWithValue("@profilePicture", profilePicture);
                sqlCommand.Parameters.AddWithValue("@likes", likes);
                //sqlCommand.Parameters.AddWithValue("@crawlingDate", formatDate);
                sqlCommand.Parameters.AddWithValue("@hits", hits); // null 에러난다. 이 줄 빼고 해보기
                sqlCommand.Parameters.AddWithValue("@keyword", keyword);
                sqlCommand.Parameters.AddWithValue("@pictureAndVideo", pictureAndVideo);

                sqlCommand.ExecuteNonQuery();
                successBoardCount++;
                sqlCommand.Parameters.Clear();

                return checkCommentId;
            }
            catch (Exception exc)
            {
                PrintError(exc.Message, StartPoint.rtb, typeof(Common));
                return Guid.Empty;
            }
        }

        public static void insertComment(
            string channelType, string contentsType,
            Guid checkCommentId,
            string writer, string contents, string created,
            string url, string profilePicture, int likes, string keyword)
        {
            try
            {
                SqlCommand sqlCommand = ConnectDB.sqlCommand;
                DateTime currentDate = DateTime.Now;
                string formatDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                sqlCommand.Parameters.Clear();

                string query =
                  "insert into dbo.tbmskim "
                + "(channelType, contentsType, checkCommentId, writer, contents, created, tag, commentCount, url, profilePicture, likes, crawlingDate, hits, keyword, pictureAndVideo)"
                + "values (@channelType, @contentsType, @checkCommentId, @writer, @contents, @created, @tag, @commentCount, @url, @profilePicture, @likes, GETDATE(), @hits, @keyword, @pictureAndVideo)";
                sqlCommand.CommandText = query;

                sqlCommand.Parameters.AddWithValue("@channelType", channelType);
                sqlCommand.Parameters.AddWithValue("@contentsType", contentsType);
                sqlCommand.Parameters.AddWithValue("@checkCommentId", checkCommentId);
                sqlCommand.Parameters.AddWithValue("@writer", writer);
                sqlCommand.Parameters.AddWithValue("@contents", contents);
                sqlCommand.Parameters.AddWithValue("@created", created);
                sqlCommand.Parameters.AddWithValue("@tag", "");
                sqlCommand.Parameters.AddWithValue("@commentCount", 0);
                sqlCommand.Parameters.AddWithValue("@url", url);
                sqlCommand.Parameters.AddWithValue("@profilePicture", profilePicture);
                sqlCommand.Parameters.AddWithValue("@likes", likes);
                //sqlCommand.Parameters.AddWithValue("@crawlingDate", formatDate);
                sqlCommand.Parameters.AddWithValue("@hits", 0); // null 에러난다. 이 줄 빼고 해보기
                sqlCommand.Parameters.AddWithValue("@keyword", keyword);
                sqlCommand.Parameters.AddWithValue("@pictureAndVideo", "");

                sqlCommand.ExecuteNonQuery();
                successCommentCount++;
                sqlCommand.Parameters.Clear();
            }
            catch (Exception exc)
            {
                PrintError(exc.Message, StartPoint.rtb, typeof(Common));
            }
        }

        public static void insertCommentClien(
            string channelType, string contentsType,
            Guid checkCommentId, CommentObject[] comments,
            string url, string profilePicture, string keyword)
        {
            try
            {
                SqlCommand sqlCommand = ConnectDB.sqlCommand;
                DateTime currentDate = DateTime.Now;
                string formatDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                sqlCommand.Parameters.Clear();

                string query =
                  "insert into dbo.tbmskim "
                + "(channelType, contentsType, checkCommentId, writer, contents, created, tag, commentCount, url, profilePicture, likes, crawlingDate, hits, keyword, pictureAndVideo)"
                + "values (@channelType, @contentsType, @checkCommentId, @writer, @contents, @created, @tag, @commentCount, @url, @profilePicture, @likes, @crawlingDate, @hits, @keyword, @pictureAndVideo)";
                sqlCommand.CommandText = query;

                foreach (var item in comments)
                {
                    if (item.writer == "NONE")
                    {
                        continue;
                    }

                    sqlCommand.Parameters.AddWithValue("@channelType", channelType);
                    sqlCommand.Parameters.AddWithValue("@contentsType", contentsType);
                    sqlCommand.Parameters.AddWithValue("@checkCommentId", checkCommentId);
                    sqlCommand.Parameters.AddWithValue("@writer", item.writer);
                    sqlCommand.Parameters.AddWithValue("@contents", DataPreprocessing(item.contents));
                    sqlCommand.Parameters.AddWithValue("@created", item.created);
                    sqlCommand.Parameters.AddWithValue("@tag", "");
                    sqlCommand.Parameters.AddWithValue("@commentCount", 0);
                    sqlCommand.Parameters.AddWithValue("@url", url);
                    sqlCommand.Parameters.AddWithValue("@profilePicture", profilePicture);
                    sqlCommand.Parameters.AddWithValue("@likes", item.likes);
                    sqlCommand.Parameters.AddWithValue("@crawlingDate", formatDate);
                    sqlCommand.Parameters.AddWithValue("@hits", 0); // null 에러난다.
                    sqlCommand.Parameters.AddWithValue("@keyword", keyword);
                    sqlCommand.Parameters.AddWithValue("@pictureAndVideo", "");

                    sqlCommand.ExecuteNonQuery();
                    successCommentCount++;
                    sqlCommand.Parameters.Clear();
                }
            }
            catch (Exception exc)
            {
                PrintError(exc.Message, StartPoint.rtb, typeof(Common));
            }
        }
    }
}
