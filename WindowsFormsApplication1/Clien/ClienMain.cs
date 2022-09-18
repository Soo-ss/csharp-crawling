using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using HtmlAgilityPack;
using log4net;

using WindowsFormsApplication1;

namespace Total
{
    public class ClienMain
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClienMain));
        int commentCount = 0;
        Random rand = new Random();

        public void StartClien(string inputName, int start, int end)
        {
            Common.PrintInfo("[start] clien", StartPoint.rtb, typeof(ClienMain));

            try
            {
                Common.DisableButtonWhenStart();
                List<string> aLinkList = new List<string>();

                if (start > end)
                {
                    throw new Exception("시작날짜는 끝나는 날짜보다 빨라야합니다.");
                }

                Common.successBoardCount = 0;
                CrawlingAllClienPages(aLinkList, inputName, start, end);
                ProcessBoardData(aLinkList, inputName);

                Common.PrintInfo("총 게시물 수집 갯수: " + Common.successBoardCount, StartPoint.rtb, typeof(ClienMain));

                Common.DisableButtonWhenEnd();
                Common.PrintInfo("[end] 클리앙 수집 종료", StartPoint.rtb, typeof(ClienMain));
                StartPoint.clienT.Abort();
            }
            catch (ThreadAbortException tae)
            {
                Common.PrintWarn(tae.Message, StartPoint.rtb, typeof(ClienMain));
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
            }
        }

        private void CrawlingAllClienPages(List<string> aLinkList, string keyword, int start, int end)
        {
            try
            {
                string home = "https://www.clien.net";
                bool isEnd = false;

                for (int i = 0; i <= 50; i++)
                {
                    var html = home + "/service/search?q="
                            + keyword
                            + "&sort=recency&p=" + i + "&boardCd=&isBoard=false";

                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(html);
                    var total = htmlDoc.DocumentNode
                        .Descendants("div")
                        .Where(no => no
                        .GetAttributeValue("data-role", "")
                        .Contains("list-row")
                        );

                    Common.SleepProgramSeconds(rand.Next(1, 2));

                    //var total = htmlDoc.DocumentNode
                    //    .SelectSingleNode("//*[@id='div_content']/div[5]")
                    //    .SelectNodes("div");

                    isEnd = CrawlingALink(aLinkList, start, end, home, isEnd, total);

                    if (isEnd)
                    {
                        break;
                    }
                }

                Common.PrintInfo("수집할 전체 URL (a link) 수: " + aLinkList.Count, StartPoint.rtb, typeof(ClienMain));
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
            }
        }

        private bool CrawlingALink(List<string> aLinkList, int start, int end, 
            string home, bool isEnd, IEnumerable<HtmlNode> total)
        {
            try
            {
                foreach (var item in total)
                {
                    // data-role = "list-row"에서 긁어오니깐 필요없다.
                    //if (item.SelectSingleNode("div[2]")
                    //        .InnerText
                    //        .Trim() == "관리자 삭제된 게시물입니다.")
                    //{
                    //    continue;
                    //}

                    string date = GetCrawlingDate(item);

                    string[] dateArr = date.Split(' ')[0].Split('-');
                    int crawlDate = Convert.ToInt32(dateArr[0] + dateArr[1] + dateArr[2]);

                    if (crawlDate < start || end < crawlDate)
                    {
                        isEnd = true;
                        break;
                    }

                    Common.PrintInfo("[date]: " + date, StartPoint.rtb, typeof(ClienMain));
                    aLinkList.Add(home + GetCrawlingHref(item));
                }

                return isEnd;
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return true;
            }
        }

        private void ProcessBoardData(List<string> aLinkList, string inputName)
        {
            try
            {
                foreach (var item in aLinkList)
                {
                    Common.PrintInfo("=============== [start page] ===============", StartPoint.rtb, typeof(ClienMain));

                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(item);

                    string title = GetCrawlingTitle(htmlDoc);
                    string contents = GetCrawlingContents(htmlDoc);
                    string writer = GetCrawlingWriter(htmlDoc);
                    int hits = GetCrawlingHits(htmlDoc);
                    string created = GetCrawlingCreated(htmlDoc);
                    Common.CommentObject[] comments = GetCrawlingComments(htmlDoc);
                    int likes = GetBoardLikes(htmlDoc);

                    Common.SleepProgramSeconds(rand.Next(1, 2));

                    //Common.PrintInfo("[title]: " + title, StartPoint.rtb, typeof(ClienMain));
                    //Common.PrintInfo("[contents]: " + contents, StartPoint.rtb, typeof(ClienMain));
                    //Common.PrintInfo("[writer]: " + writer, StartPoint.rtb, typeof(ClienMain));
                    //Common.PrintInfo("[hits]: " + hits, StartPoint.rtb, typeof(ClienMain));
                    //Common.PrintInfo("[created]: " + created, StartPoint.rtb, typeof(ClienMain));
                    //Common.PrintInfo("[likes]: " + likes, StartPoint.rtb, typeof(ClienMain));

                    Guid checkCommentId = Common.insertBoard(
                            "clien", "main",
                            writer, Common.DataPreprocessing(contents), created,
                            "", commentCount, 
                            item, "", likes, hits, inputName, "");

                    if (comments == null)
                    {
                        continue;
                    }

                    Common.successCommentCount = 0;
                    Common.insertCommentClien(
                        "clien", "comment",
                        checkCommentId, comments,
                        item, "", inputName);
                    
                    Common.PrintInfo(item + " => 총 댓글 수집 갯수: " + Common.successCommentCount, StartPoint.rtb, typeof(ClienMain));
                }
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
            }
        }

        private int GetBoardLikes(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                var str = htmlDoc.DocumentNode
                .Descendants("a")
                .Where(i => i
                .GetAttributeValue("class", "")
                .Contains("symph_count"))
                .First()
                .FirstChild
                .InnerText;

                return Convert.ToInt32(str);
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return 0;
            }

            //var str = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='comment-head']/a")
            //    .InnerText
            //    .Replace("명", "");

            //return Convert.ToInt32(str);
        }

        private string GetCrawlingTitle(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            // title: div[2]/h3
            //var title = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='div_content']/div[2]/h3/span[1]")
            //    .InnerText
            //    .Replace("\t", "")
            //    .Trim();

            try
            {
                var title = htmlDoc.DocumentNode
                .Descendants("h3")
                .Where(i => i
                .GetAttributeValue("class", "")
                .Contains("post_subject"))
                .First()
                .FirstChild
                .NextSibling
                .InnerText;

                if (title.Equals("판매") ||
                    title.Equals("거래완료") ||
                    title.Equals("질문") ||
                    title.Equals("휴대폰") ||
                    title.Equals("잡담") ||
                    title.Equals("워치") ||
                    title.Equals("자유"))
                {
                    return htmlDoc.DocumentNode
                    .Descendants("h3")
                    .Where(i => i
                    .GetAttributeValue("class", "")
                    .Contains("post_subject"))
                    .First()
                    .FirstChild
                    .NextSibling
                    .NextSibling
                    .NextSibling
                    .InnerText;
                }

                return title;
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return "";
            }
        }

        private string GetCrawlingContents(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode
                .Descendants("div")
                .Where(i => i
                .GetAttributeValue("class", "")
                .Contains("post_article"))
                .First()
                .InnerText
                .Replace("\t", "")
                .Replace("&nbsp;", "")
                .Trim();
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return "";
            }

            //var contents = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='div_content']/div[4]/div[contains(@class, 'post_content')]")
            //    .InnerText
            //    .Replace("&nbsp;", "")
            //    .Trim();

            //return contents;
        }

        private string GetCrawlingWriter(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                // writer: div[3]/div[1]
                var writer = htmlDoc.DocumentNode
                    .Descendants("span")
                    .Where(i => i
                    .GetAttributeValue("class", "")
                    .Contains("nickname"))
                    .First()
                    .FirstChild
                    .NextSibling
                    .InnerText;

                if (writer == "")
                {
                    return htmlDoc.DocumentNode
                    .Descendants("span")
                    .Where(i => i
                    .GetAttributeValue("class", "")
                    .Contains("nickname"))
                    .First()
                    .FirstChild
                    .NextSibling
                    .GetAttributeValue("alt", "");
                }

                return writer;
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return "";
            }
        }

        private int GetCrawlingHits(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                // hits: div[3]/div[2]/span/strong
                var hits = htmlDoc.DocumentNode
                    .Descendants("span")
                    .Where(i => i
                    .GetAttributeValue("class", "")
                    .Contains("view_count"))
                    .First()
                    .FirstChild
                    .NextSibling
                    .NextSibling
                    .NextSibling
                    .InnerText
                    .Replace(",", "");

                //var hits = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='div_content']/div[3]/div[2]/span[1]/strong")
                //    .InnerText
                //    .Replace(",", "")
                //    .Trim();

                return Convert.ToInt32(hits);
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return 0;
            }
        }

        private string GetCrawlingCreated(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode
                .Descendants("div")
                .Where(i => i
                .GetAttributeValue("class", "")
                .Contains("post_author"))
                .First()
                .SelectSingleNode("span")
                .GetDirectInnerText()
                .Trim();
                
                // dateContents: div[4]/div[1]/span[1]
                //var date = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='div_content']/div[4]/div[1]/span[1]")
                //    .GetDirectInnerText()
                //    .Trim();

                //return date;
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return "";
            }
        }

        private Common.CommentObject[] GetCrawlingComments(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                var total = htmlDoc.DocumentNode
                        .Descendants("div")
                        .Where(no => no
                        .GetAttributeValue("data-role", "")
                        .Contains("comment-row"));

                //var total = htmlDoc.DocumentNode
                //.SelectSingleNode("//*[@id='div_content']/div[5]/div[2]")
                //.SelectNodes("div");

                if (total == null)
                {
                    return null;
                }

                commentCount = total.Count();

                Common.CommentObject[] arr = new Common.CommentObject[commentCount];

                if (total == null)
                {
                    return null;
                }

                int i = 0;
                try
                {
                    foreach (var item in total)
                    {
                        string before = item.InnerText.Trim();

                        if (before.Contains("삭제 되었습니다.") ||
                            before.Contains("관리자에 의해 삭제되었습니다."))
                        {
                            arr[i++] = new Common.CommentObject("NONE", "NONE", "NONE", -999);
                            continue;
                        }

                        string writer = item
                            .Descendants("span")
                            .Where(a => a
                            .GetAttributeValue("class", "")
                            .Contains("nickname"))
                            .First()
                            .FirstChild
                            .NextSibling
                            .InnerText;

                        if (writer == "")
                        {
                            writer = item
                            .Descendants("span")
                            .Where(a => a
                            .GetAttributeValue("class", "")
                            .Contains("nickname"))
                            .First()
                            .FirstChild
                            .NextSibling
                            .GetAttributeValue("alt", "");
                        }

                        //string writer = item.SelectSingleNode("div[1]/div[1]/span[1]/span").InnerText.Trim();

                        //if (writer == "")
                        //{
                        //    writer = item.SelectSingleNode("div[1]/div[1]/span[1]/span/img")
                        //        .Attributes["alt"]
                        //        .Value
                        //        .Trim();
                        //}

                        string contents = item
                            .Descendants("div")
                            .Where(a => a
                            .GetAttributeValue("class", "")
                            .Contains("comment_view"))
                            .First()
                            .Descendants("input")
                            .First()
                            .GetAttributeValue("value", "");

                        string created = item
                            .Descendants("span")
                            .Where(a => a
                            .GetAttributeValue("class", "")
                            .Contains("timestamp"))
                            .First()
                            .InnerText
                            .Trim();

                        string likes = item
                            .Descendants("button")
                            .Where(a => a
                            .GetAttributeValue("class", "")
                            .Contains("comment_symph"))
                            .First()
                            .InnerText
                            .Trim();

                        //string contents = item.SelectSingleNode("div[3]").InnerText.Trim();
                        //string created = item.SelectSingleNode("div[1]/div[2]/div[3]/span").InnerText.Trim();

                        // 수정일
                        if (created.Length > 19)
                        {
                            created = created.Split('/')[0].Trim();
                        }

                        arr[i++] = new Common.CommentObject(writer, contents, created, Convert.ToInt32(likes));
                    }
                }
                catch(Exception exc)
                {
                    Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                    return null;
                }

                return arr;
            }
            catch(Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return null;
            }
        }

        private string GetCrawlingHref(HtmlNode item)
        {
            try
            {
                return item.Descendants("a").Where(n => n
                    .GetAttributeValue("href", "")
                    .Contains("/service"))
                    .FirstOrDefault()
                    .GetAttributeValue("href", "");

                //return item.SelectSingleNode("div[5]/span/span")
                //    .InnerText;

                //return item.SelectSingleNode("div[2]/span/a")
                //        .Attributes["href"]
                //        .Value
                //        .Trim();
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return "";
            }
        }

        private string GetCrawlingDate(HtmlNode item)
        {
            try
            {
                return item
                .Descendants("span")
                .Where(i => i
                .GetAttributeValue("class", "")
                .Contains("timestamp"))
                .First()
                .InnerText;

                //var date = item.SelectSingleNode("div[5]")
                //    .InnerText
                //    .Trim();

                //return date.Substring(5);
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(ClienMain));
                return "";
            }
        }
    }
}
