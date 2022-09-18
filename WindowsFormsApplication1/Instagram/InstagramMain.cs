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
using System.Collections;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using log4net;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

using WindowsFormsApplication1;

namespace Total
{
    public class InstagramMain
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstagramMain));

        ChromeDriverService driverService = null;
        ChromeOptions options = null;
        Random rand = new Random();
        IWebDriver driver = null;
        bool scrollRunning;

        class HrefState
        {
            public DateTime currentDate;
            public bool isComplete;

            public HrefState(DateTime currentDate, bool isComplete)
            {
                this.currentDate = currentDate;
                this.isComplete = isComplete;
            }
        }

        public void StartInstagram(string inputName, int start, int end)
        {
            Common.PrintInfo("[start] instagram", StartPoint.rtb, typeof(InstagramMain));

            //string test = @"#치맥🍻 \n\n집앞에생겨 02-123-4567 080-2344-4576 호다닥가본\n\n#구도로통닭 \n\n통닭이 넘 보드랍구 맛난게\n\n매일 뛰나갈듯🏃‍♀️🏃🏃‍♀️\n\n가오픈이라 못먹어본게 마나아쉽\n\n다시가보는걸루\n\n#다여트중인데\n#매일먹는다\n#그와중에4키로빠짐\n#나도신기할따름\n#먹기위해운동하는여자 admin@email.com";
            //string a = DataPreprocessing(test);

            try
            {
                Common.DisableButtonWhenStart();
                Common.successBoardCount = 0;

                // cmd창 cd chrome.exe 경로에서
                // chrome.exe --remote-debugging-port=9222 --user-data-dir="C:\Users\minsukim\dev\ChromeTEMP"
                options = new ChromeOptions();
                options.DebuggerAddress = "127.0.0.1:9222"; // 인스타그램 로그인때 주석치기

                if (start > end)
                {
                    throw new Exception("시작날짜는 끝나는 날짜보다 빨라야합니다.");
                }

                driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(driverService, options);

                bool isLogin = true;

                //bool isLogin = false;
                //isLogin = LoginInstagram();

                if (isLogin)
                {
                    CrawlingInstagram(inputName, start, end);
                }
                else
                {
                    isLogin = false;
                    throw new Exception("로그인 에러");
                }

                Common.PrintInfo("총 게시물 수집 갯수: " + Common.successBoardCount, StartPoint.rtb, typeof(InstagramMain));
                Common.DisableButtonWhenEnd();

                // 크롬드라이버 끄기
                driver.Quit();
                Common.PrintInfo("[end] 인스타그램 수집 종료", StartPoint.rtb, typeof(InstagramMain));
                StartPoint.instagramT.Abort();
            }
            catch (ThreadAbortException tae)
            {
                Common.PrintWarn(tae.Message, StartPoint.rtb, typeof(InstagramMain));
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
            }
        }

        private bool LoginInstagram()
        {
            try
            {
                string loginUrl = "https://www.instagram.com/accounts/login/";

                driver.Navigate().GoToUrl(loginUrl);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                bool isLogin;
                Common.SleepProgramSeconds(10);

                var inputId = driver.FindElement(By.Name(Constants.instaInputId));
                inputId.SendKeys(Constants.instaMyId);
                Common.SleepProgramSeconds(5);

                var inputPw = driver.FindElement(By.Name(Constants.instaInputPw));
                inputPw.SendKeys(Constants.instaMyPw);
                Common.SleepProgramSeconds(7);

                var loginButton = driver.FindElement(By.CssSelector(Constants.instaLoginButton));
                loginButton.Click();
                isLogin = true;

                Common.SleepProgramSeconds(10);
                return isLogin;
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return false;
            }
        }
        
        private void CrawlingInstagram(string inputName, int start, int end)
        {
            string tagUrl = "https://www.instagram.com/explore/tags/" + inputName;
            bool isNextButton = true;
            scrollRunning = true;

            try
            {
                driver.Navigate().GoToUrl(tagUrl);
                Common.SleepProgramSeconds(rand.Next(10, 15));

                Dictionary<string, bool> dict = new Dictionary<string, bool>();

                int i = 0;
                // a링크 수집하고 크롤링하고 스크롤 내리고 무한반복
                while (scrollRunning)
                {
                    try
                    {
                        //driver.FindElement(By.CssSelector(Constants.instaCssFirstImg)).Click();

                        // bfs OR dfs
                        //var doc = new HtmlAgilityPack.HtmlDocument();
                        //doc.LoadHtml(driver.PageSource);

                        //var article = doc.DocumentNode.Descendants("article").First();


                        var obj = driver.FindElements(By.TagName("a"));
                        //Common.PrintInfo("[count]: " + obj.Count, StartPoint.rtb, typeof(InstagramMain));


                        //dict.Add("https://www.instagram.com/p/Ch1I8WLLhih/", false);

                        foreach (var item in obj)
                        {
                            try
                            {
                                if (i < 9)
                                {
                                    i++;
                                    continue;
                                }

                                if (dict.ContainsKey(item.GetAttribute("href")))
                                {
                                    continue;
                                }

                                dict.Add(item.GetAttribute("href"), false);
                            }
                            catch (Exception exc)
                            {
                                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                    }

                    Common.successBoardCount = 0;
                    CrawlingEachTab(dict, inputName, start, end);

                    if (!scrollRunning)
                    {
                        break;
                    }

                    Common.PrintInfo("총 게시물 수집 갯수: " + Common.successBoardCount, StartPoint.rtb, typeof(InstagramMain));

                    Common.SleepProgramSeconds(rand.Next(7, 10));

                    ((IJavaScriptExecutor)driver)
                        .ExecuteScript("window.scrollBy(0,800)");
                    Common.SleepProgramSeconds(rand.Next(3, 5));
                    ((IJavaScriptExecutor)driver)
                        .ExecuteScript("window.scrollBy(0,800)");
                    Common.SleepProgramSeconds(rand.Next(3, 5));
                    ((IJavaScriptExecutor)driver)
                        .ExecuteScript("window.scrollBy(0,800)");
                    Common.SleepProgramSeconds(rand.Next(3, 5));
                    ((IJavaScriptExecutor)driver)
                        .ExecuteScript("window.scrollBy(0,800)");
                    Common.SleepProgramSeconds(rand.Next(3, 5));
                }

                //while (true)
                //{
                //    string url = driver.Url;

                //    if (currentCount > totalCount)
                //    {
                //        //driver.Close();
                //        //driver.Quit();
                //        break;
                //    }

                //    if (!isNextButton)
                //    {
                //        break;
                //    }

                //    Common.SleepProgramSeconds(5);
                //    string created = GetCrawlingCreated();

                //    string[] dateArr = created.Split(' ')[0].Split('-');
                //    int crawlDate = Convert.ToInt32(dateArr[0] + dateArr[1] + dateArr[2]);

                //    if (crawlDate < start || end < crawlDate)
                //    {
                //        break;
                //    }

                //    Common.SleepProgramSeconds(5);
                //    string writer = GetCrawlingWriter();
                //    string[] contents = GetCrawlingContents();
                //    //string[] tags = GetCrawlingTags();
                //    Common.CommentObject[] comments = GetCrawlingComments();
                //    int boardLikes = GetBoardLikes();

                //    Guid checkCommentId = Common.insertBoard(
                //        "instagram", "main",
                //        writer, contents[0], created,
                //        contents[1], commentCount, url, boardLikes, -1);

                //    Common.insertComment(
                //        "instagram", "comment",
                //        checkCommentId, comments,
                //        url, boardLikes);

                //    Common.SleepProgramSeconds(5);
                //    driver.FindElement(By.CssSelector(Constants.instaCssNextButton)).Click();
                //    currentCount++;
                //    log.Info("[Current Count]: " + currentCount);
                //}
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                isNextButton = false;
                scrollRunning = false;
            }
        }

        private string UnixTimestampToDateTime(string str)
        {
            try
            {
                // 한국시간은 created_at_utc
                // 현재시각 2022년 8월 22일 9시 44분
                // 4분전
                // 2022-08-22T00:40:32.000Z

                // created_at 1661103632 2022-08-22 02:40:32.000
                // created_at_utc 1661128832 2022-08-22 09:40:32.000

                double uts = Convert.ToInt64(str);

                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dt = dt.AddSeconds(uts).ToLocalTime();

                return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return "";
            }
        }

        private string[] SplitContentsAndTag(string str)
        {
            try
            {
                Regex regex = new Regex(@"(#[\d|A-Z|a-z|ㄱ-ㅎ|ㅏ-ㅣ|가-힣]*)");
                //string hashtag = "#치맥🍻 \n\n집앞에생겨 호다닥가본\n\n#구도로통닭 \n\n통닭이 넘 보드랍구 맛난게\n\n매일 뛰나갈듯🏃‍♀️🏃🏃‍♀️\n\n가오픈이라 못먹어본게 마나아쉽\n\n다시가보는걸루\n\n#다여트중인데\n#매일먹는다\n#그와중에4키로빠짐\n#나도신기할따름\n#먹기위해운동하는여자";
                string[] arr = regex.Split(str);

                string contents = "";
                string tag = "";
                foreach (string item in arr)
                {
                    try
                    {
                        if (item == "" || item == "\n")
                        {
                            continue;
                        }

                        if (item[0] == '#')
                        {
                            tag += item;
                            tag += " ";
                            continue;
                        }

                        contents += item;
                    }
                    catch (Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                    }
                }

                contents = contents.Trim();
                tag = tag.Trim();

                return new string[] { contents, tag };
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return new string[] { };
            }
        }

        

        private void CrawlingEachTab(Dictionary<string, bool> dict, string inputName, int start, int end)
        {
            bool running = true;

            try
            {
                for (int i = 0; i < dict.Count; i++)
                {
                    try
                    {
                        string href = dict.ElementAt(i).Key;
                        //string href = "https://www.instagram.com/p/Chm5mn4vv8X/";

                        // 방문안한것만 실행한다.
                        if (!dict.ElementAt(i).Value && href.Contains("https://www.instagram.com/p/"))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                            driver.SwitchTo().Window(driver.WindowHandles.Last());

                            Common.SleepProgramSeconds(rand.Next(2, 8));
                            driver.Navigate().GoToUrl(href);
                            Common.SleepProgramSeconds(rand.Next(2, 8));

                            //int boardLikes = -1;
                            //bool isVideoLikesExist = false;
                            //int videoLikes = GetVideoLikes();

                            //if(videoLikes >= 0)
                            //{
                            //    boardLikes = videoLikes;
                            //    isVideoLikesExist = true;
                            //}

                            string[] mainPicture = GetMainPicture();
                            string pictureAndVideo = MergeUrl(mainPicture);

                            string metaId = driver.FindElement(By
                                .XPath("//meta[@property='al:ios:url']"))
                                .GetAttribute("content")
                                .Split('?')[1]
                                .Substring(3);

                            JObject json = HttpReqCrawling(
                                    "https://i.instagram.com/api/v1/media/" +
                                    metaId +
                                    "/comments/?can_support_threading=true" +
                                    "&permalink_enabled=false"
                                );

                            string minId = json["next_min_id"]?.ToString();

                            // https://www.instagram.com/p/Chm5mn4vv8X/
                            int totalCommentCount = Convert.ToInt32(json["comment_count"]?.ToString());

                            string writer = json["caption"]?["user"]?["username"]?.ToString();
                            string fullName;
                            string contents;
                            string createdAtUtc;
                            string profilePicture;
                            string[] dateArr;
                            int crawlDate;
                            string[] splited;
                            Guid checkCommentId;

                            int boardLikes = GetBoardLikes();

                            //if (!isVideoLikesExist)
                            //{
                            //    boardLikes = GetBoardLikes();
                            //}

                            // 댓글을 중지해놓은경우 다른 request로 다시 보내야한다.
                            if (writer == null)
                            {
                                json = HttpReqCrawling(
                                    "https://i.instagram.com/api/v1/media/" +
                                    metaId +
                                    "/info/"
                                );
                                writer = json["items"]?[0]["caption"]?["user"]?["username"]?.ToString();
                                fullName = json["items"]?[0]["caption"]?["user"]?["full_name"]?.ToString();
                                contents = json["items"]?[0]["caption"]?["text"]?.ToString();
                                createdAtUtc = json["items"]?[0]["caption"]?["created_at_utc"]?.ToString();
                                profilePicture = json["items"]?[0]["caption"]?["user"]?["profile_pic_url"]?.ToString();

                                dateArr = UnixTimestampToDateTime(createdAtUtc)
                                    .Split(' ')[0]
                                    .Split('-');

                                crawlDate = Convert.ToInt32(dateArr[0] + dateArr[1] + dateArr[2]);

                                // 1일 정도 여유주기
                                if (crawlDate < start - 1 || end + 1 < crawlDate)
                                {
                                    scrollRunning = false;
                                    driver.Close();
                                    driver.SwitchTo().Window(driver.WindowHandles.First());
                                    break;
                                }

                                splited = SplitContentsAndTag(contents);

                                // 내용 없을때는 넣지않는다.
                                if(splited[0] == "")
                                {
                                    Common.PrintWarn("내용이 없습니다. 데이터를 수집하지 않습니다.", StartPoint.rtb, typeof(InstagramMain));

                                    // visited true
                                    dict[href] = true;

                                    driver.Close();
                                    driver.SwitchTo().Window(driver.WindowHandles.First());
                                    Common.SleepProgramSeconds(rand.Next(10, 20));

                                    continue;
                                }

                                checkCommentId = Common.insertBoard(
                                        "instagram", "main",
                                        writer, Common.DataPreprocessing(splited[0]), UnixTimestampToDateTime(createdAtUtc),
                                        splited[1], totalCommentCount,
                                        href, profilePicture, boardLikes, 0, inputName, pictureAndVideo);
                                Common.PrintInfo("[DB] success insert <main>", StartPoint.rtb, typeof(InstagramMain));

                                // visited true
                                dict[href] = true;

                                driver.Close();
                                driver.SwitchTo().Window(driver.WindowHandles.First());
                                Common.SleepProgramSeconds(rand.Next(10, 20));

                                continue;
                            }

                            fullName = json["caption"]?["user"]?["full_name"]?.ToString();
                            contents = json["caption"]?["text"]?.ToString();
                            createdAtUtc = json["caption"]?["created_at_utc"]?.ToString();
                            profilePicture = json["caption"]?["user"]?["profile_pic_url"]?.ToString();

                            dateArr = UnixTimestampToDateTime(createdAtUtc)
                                .Split(' ')[0]
                                .Split('-');

                            crawlDate = Convert.ToInt32(dateArr[0] + dateArr[1] + dateArr[2]);

                            // 1일 정도 여유주기
                            if (crawlDate < start - 1 || end + 1 < crawlDate)
                            {
                                scrollRunning = false;
                                driver.Close();
                                driver.SwitchTo().Window(driver.WindowHandles.First());
                                break;
                            }

                            splited = SplitContentsAndTag(contents);

                            // 내용 없을때는 넣지않는다.
                            if (splited[0] == "")
                            {
                                Common.PrintWarn("내용이 없습니다. 데이터를 수집하지 않습니다.", StartPoint.rtb, typeof(InstagramMain));

                                // visited true
                                dict[href] = true;

                                driver.Close();
                                driver.SwitchTo().Window(driver.WindowHandles.First());
                                Common.SleepProgramSeconds(rand.Next(10, 20));

                                continue;
                            }

                            checkCommentId = Common.insertBoard(
                                    "instagram", "main",
                                    writer, Common.DataPreprocessing(splited[0]), UnixTimestampToDateTime(createdAtUtc),
                                    splited[1], totalCommentCount,
                                    href, profilePicture, boardLikes, 0, inputName, pictureAndVideo);
                            Common.PrintInfo("[DB] success insert <main>", StartPoint.rtb, typeof(InstagramMain));

                            JToken[] arr = json["comments"].ToArray();
                            int insertCount = 0;

                            Common.successCommentCount = 0;
                            ProcessComment(metaId, arr, href, checkCommentId, ref insertCount, inputName);

                            // 댓글 더보기 (+)
                            while (running)
                            {
                                try
                                {
                                    if (minId == null)
                                    {
                                        break;
                                    }
                                    Common.SleepProgramSeconds(rand.Next(10, 17));

                                    json = HttpReqCrawling(
                                        "https://i.instagram.com/api/v1/media/" +
                                        metaId +
                                        "/comments/?can_support_threading=true" +
                                        "&min_id=" + minId);

                                    minId = json["next_min_id"]?.ToString();

                                    arr = json["comments"].ToArray();
                                    ProcessComment(metaId, arr, href, checkCommentId, ref insertCount, inputName);
                                }
                                catch (Exception exc)
                                {
                                    Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                                }
                            }

                            // visited true
                            dict[href] = true;
                            Common.PrintInfo(href + " => 총 댓글 수집 갯수: " + Common.successCommentCount, StartPoint.rtb, typeof(InstagramMain));

                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles.First());
                            Common.SleepProgramSeconds(rand.Next(10, 17));
                        }
                    }
                    catch (Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                        running = false;
                    }
                }
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
            }
        }

        // 중복키(URL) 삽입 불가이므로 공백으로 구분해서 넣는다.
        private string MergeUrl(string[] mainPicture)
        {
            try
            {
                string result = "";
                
                foreach (string item in mainPicture)
                {
                    result += item;
                    result += " ";
                }

                return result.Trim();
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return "";
            }
        }

        //private int GetVideoLikes()
        //{
        //    int res = 0;
        //    try
        //    {
        //        string str = driver
        //            .FindElement(By.CssSelector(Constants.instaCssVideoLikes))
        //            .Text
        //            .Replace(",", "");

        //        res = Convert.ToInt32(str);
        //    }
        //    catch (NoSuchElementException nse)
        //    {
        //        // 조회수가 없다면
        //        Common.PrintWarn("조회수: 0회", StartPoint.rtb, typeof(InstagramMain));
        //        //return 0;
        //    }
        //    catch (Exception exc)
        //    {
        //        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
        //        //return -999;
        //    }
        //    return res;
        //}

        //private string GetVideoUrl()
        //{
        //    try
        //    {
        //        var test = driver
        //        .FindElement(By.CssSelector(Constants.instaCssVideo));
        //        string url = test.GetAttribute("poster");

        //        return url;
        //    }
        //    catch(Exception exc)
        //    {
        //        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
        //        return "";
        //    }
        //}

        // 여기서 비디오 썸네일까지 다 가져온다.
        private string[] GetMainPicture()
        {
            bool running = true;
            // TODO: 클릭하고 Set에 저장하고 반복한다.
            HashSet<string> set = new HashSet<string>();

            try
            {
                while (running)
                {
                    try
                    {
                        // ===============
                        // 이미지가 1개 있을때를 대비한다.
                        var img = driver
                            .FindElements(By.CssSelector(Constants.instaCssMainImgOnlyOne));
                        var video = driver
                            .FindElements(By.CssSelector(Constants.instaCssVideo));

                        foreach (var item in img)
                        {
                            try
                            {
                                set.Add(item.GetAttribute("src"));
                            }
                            catch (Exception exc)
                            {
                                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                                running = false;
                            }
                        }

                        foreach (var item in video)
                        {
                            try
                            {
                                set.Add(item.GetAttribute("poster"));
                            }
                            catch (Exception exc)
                            {
                                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                                running = false;
                            }
                        }
                        // ==================

                        var nextBtn = driver.FindElement(By.CssSelector(Constants.instaCssMainImgNextButton));
                        nextBtn.Click();
                        Common.SleepProgramSeconds(rand.Next(1, 3));

                        if (nextBtn == null)
                        {
                            break;
                        }

                        img = driver
                            .FindElements(By.CssSelector(Constants.instaCssMainImg));
                        video = driver
                            .FindElements(By.CssSelector(Constants.instaCssVideo));

                        foreach (var item in img)
                        {
                            try
                            {
                                set.Add(item.GetAttribute("src"));
                            }
                            catch (Exception exc)
                            {
                                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                                running = false;
                            }
                        }

                        foreach (var item in video)
                        {
                            try
                            {
                                set.Add(item.GetAttribute("poster"));
                            }
                            catch (Exception exc)
                            {
                                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                                running = false;
                            }
                        }
                    }
                    catch(NoSuchElementException nse)
                    {
                        Common.PrintWarn("다음 버튼 없음", StartPoint.rtb, typeof(InstagramMain));
                        running = false;
                        break;
                    }
                    catch(Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                        running = false;
                    }
                }

                string[] result = new string[set.Count];
                int i = 0;
                foreach(var item in set)
                {
                    try
                    {
                        result[i++] = item;
                    }
                    catch(Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                        return new string[] { };
                    }
                }

                return result;
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                running = false;
                return new string[] { };
            }
        }

        private void ProcessComment(string metaId, JToken[] arr, string href, Guid checkCommentId, ref int insertCount, string inputName)
        {
            try
            {
                foreach (JToken item in arr)
                {
                    try
                    {
                        OnlyComment(item, checkCommentId, href, ref insertCount, inputName);

                        // 대댓글
                        int childCommentCount = Convert.ToInt32(item["child_comment_count"].ToString());
                        if (childCommentCount > 0)
                        {
                            Common.SleepProgramSeconds(rand.Next(10, 15));

                            JObject json = HttpReqCrawling(
                                "https://i.instagram.com/api/v1/media/" +
                                metaId +
                                "/comments/" +
                                item["pk"]?.ToString() +
                                "/child_comments/?max_id="
                            );

                            arr = json["child_comments"].ToArray();

                            foreach (JToken item2 in arr)
                            {
                                try
                                {
                                    OnlyComment(item2, checkCommentId, href, ref insertCount, inputName);
                                }
                                catch (Exception exc)
                                {
                                    Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                    }
                }
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
            }
        }

        private void OnlyComment(JToken jt, Guid checkCommentId, string href, ref int insertCount, string inputName)
        {
            try
            {
                string writer = jt["user"]["username"]?.ToString();
                string fullName = jt["user"]["full_name"]?.ToString();
                string contents = jt["text"]?.ToString();
                string createdAtUtc = jt["created_at_utc"]?.ToString();
                string profilePicture = jt["user"]["profile_pic_url"]?.ToString();
                int likes = Convert.ToInt32(jt["comment_like_count"].ToString());

                Common.insertComment(
                    "instagram", "comment",
                    checkCommentId,
                    writer, Common.DataPreprocessing(contents), UnixTimestampToDateTime(createdAtUtc),
                    href, profilePicture, likes, inputName);
                //Common.PrintInfo("[DB] success insert <comment>, count: " + ++insertCount, StartPoint.rtb, typeof(InstagramMain));
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
            }
        }

        private JObject HttpReqCrawling(string reqUrl)
        {
            try
            {
                // datr || dpr
                string mid = driver.Manage().Cookies.GetCookieNamed("mid").Value;
                string ig_did = driver.Manage().Cookies.GetCookieNamed("ig_did").Value;
                string ig_nrcb = driver.Manage().Cookies.GetCookieNamed("ig_nrcb").Value;
                string csrftoken = driver.Manage().Cookies.GetCookieNamed("csrftoken").Value;
                string ds_user_id = driver.Manage().Cookies.GetCookieNamed("ds_user_id").Value;
                string dpr = driver.Manage().Cookies.GetCookieNamed("dpr").Value;
                string sessionid = driver.Manage().Cookies.GetCookieNamed("sessionid").Value;
                string rur = driver.Manage().Cookies.GetCookieNamed("rur").Value;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUrl);
                string cookieValue =
                    "mid = " + mid + ";" +
                    "ig_did = " + ig_did + ";" +
                    "ig_nrcb = " + ig_nrcb + ";" +
                    "csrftoken = " + csrftoken + ";" +
                    "ds_user_id = " + ds_user_id + ";" +
                    "dpr = " + dpr + ";" +
                    "sessionid = " + sessionid + ";" +
                    "rur = " + rur;

                req.Headers.Add("Cookie", cookieValue);
                req.Headers.Add("sec-ch-ua", "\"Chromium\"; v = \"104\", \" Not A; Brand\"; v = \"99\", \"Google Chrome\"; v = \"104\"");
                req.Headers.Add("sec-ch-ua-mobile", "?0");
                req.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                req.Headers.Add("sec-fetch-dest", "empty");
                req.Headers.Add("sec-fetch-mode", "cors");
                req.Headers.Add("sec-fetch-site", "same-site");
                req.Headers.Add("x-asbd-id", "198387");
                req.Headers.Add("x-csrftoken", csrftoken);
                req.Headers.Add("x-ig-app-id", "936619743392459");
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "GET";

                WebResponse res = req.GetResponse();
                Common.PrintInfo("[http response status]: " + ((HttpWebResponse)res).StatusDescription, StartPoint.rtb, typeof(InstagramMain));

                Stream stReadData = res.GetResponseStream();
                StreamReader sr = new StreamReader(stReadData, Encoding.UTF8, true);

                string strResult = sr.ReadToEnd();
                JObject result = JObject.Parse(strResult);

                res.Close();

                return result;
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return null;
            }
        }

        private int GetBoardLikes()
        {
            int result = 0;
            try
            {
                string str = driver
                    .FindElement(By.CssSelector(Constants.instaCssBoardLikes))
                    .Text
                    .Replace(",", "");

                result = Convert.ToInt32(str);
                return result;
            }
            catch (NoSuchElementException)
            {
                try
                {
                    string str = driver
                        .FindElement(By.CssSelector(Constants.instaCssVideoViews))
                        .Text
                        .Replace(",", "");

                    result = Convert.ToInt32(str);
                    return result;
                }
                catch (NoSuchElementException nse)
                {
                    // 좋아요가 없다면
                    Common.PrintWarn("좋아요: 0개", StartPoint.rtb, typeof(InstagramMain));
                }

                return result;
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return result;
            }
        }

        //private Common.CommentObject[] GetCrawlingComments()
        //{
        //    try
        //    {
        //        ClickMoreComments();

        //        var writer = driver.FindElements(By.CssSelector(Constants.instaCssCommentWriter));
        //        var contents = driver.FindElements(By.CssSelector(Constants.instaCssCommentContents));
        //        var created = driver.FindElements(By.CssSelector(Constants.instaCssCommentCreated));
        //        commentCount = created.Count;

        //        Common.CommentObject[] arr = new Common.CommentObject[commentCount];

        //        for (int i = 0; i < commentCount; i++)
        //        {
        //            arr[i] = new Common.CommentObject(
        //                writer[i].Text,
        //                contents[i].Text,
        //                created[i].GetAttribute("datetime"));
        //        }

        //        return arr;
        //    }
        //    catch (Exception exc)
        //    {
        //        log.Error(exc.Message);
        //        return null;
        //    }
        //}

        private void ClickMoreComments()
        {
            try
            {
                var obj = driver.FindElements(By.CssSelector(Constants.instaCssMoreComment));

                foreach (var item in obj)
                {
                    try
                    {
                        if (item.Text.Contains("답글 보기"))
                        {
                            item.Click();
                            Common.SleepProgramSeconds(1);
                        }
                    }
                    catch (Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                    }
                }
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
            }
        }

        private string[] GetCrawlingTags()
        {
            try
            {
                var obj = driver.FindElements(By.CssSelector(Constants.instaCssTag));
                int tagCount = obj.Count;
                string[] arr = new string[tagCount];

                for (int i = 0; i < tagCount; i++)
                {
                    arr[i] = obj[i].Text.Replace("#", "");
                }

                return arr;
            }
            catch (Exception exc)
            {
                log.Error(exc.Message);
                return null;
            }
        }

        private string GetCrawlingCreated()
        {
            try
            {
                string str = driver.FindElement(By.CssSelector(Constants.instaCssCreated))
                    .GetAttribute("datetime")
                    .Replace("T", " ")
                    .Replace("Z", "");

                return str;
            }
            catch (NoSuchElementException nse)
            {
                Common.PrintWarn(nse.Message, StartPoint.rtb, typeof(InstagramMain));
                return "";
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return "ERROR";
            }
        }

        private string[] GetCrawlingContents()
        {
            try
            {
                string str = driver.FindElement(By.CssSelector(Constants.instaCssContents))
                    .Text;

                string contents = "";
                string tags = "";
                bool isTagStart = false;

                foreach (char c in str)
                {
                    try
                    {
                        if (c == '#')
                        {
                            isTagStart = true;
                        }

                        if (isTagStart)
                        {
                            tags += c;
                        }
                        else
                        {
                            contents += c;
                        }
                    }
                    catch (Exception exc)
                    {
                        Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                    }
                }

                return new string[] { contents, tags };
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return null;
            }
        }

        private string GetCrawlingWriter()
        {
            try
            {
                string str = driver.FindElement(By.CssSelector(Constants.instaCssWriter)).Text;
                return str;
            }
            catch (Exception exc)
            {
                Common.PrintError(exc.Message, StartPoint.rtb, typeof(InstagramMain));
                return "";
            }
        }
    }
}
