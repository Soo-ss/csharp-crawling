using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;

using WindowsFormsApplication1;

namespace Total
{
    public class ConnectDB
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectDB));
        public static SqlConnection sqlConnection = new SqlConnection();
        public static SqlCommand sqlCommand = new SqlCommand();
        
        public static void connect()
        {
            try
            {
                // 22.09.07
                // 회사DB에서 로컬로 변경
                // 로컬로 안해봐서 테스트 필요함
                string strDataBase = "YOUR_DB";
                string strIP = "127.0.0.1";
                string strPort = "YOUR_PORT";
                string strID = "YOUR_ID";
                string strPW = "YOUR_PW";

                // DB 접속 정보
                string constring = "server=" + strIP + "," + strPort + ";database=" + strDataBase + ";uid=" + strID + ";pwd=" + strPW;
                // 접속정보를 적용
                sqlConnection.ConnectionString = constring;
                // DB연결
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                
                Common.PrintInfo("[DB CONNECTED]", StartPoint.rtb, typeof(ConnectDB));
            }
            catch (Exception exc)
            {
                Common.PrintError("[CONNECT DB ERROR]", StartPoint.rtb, typeof(ConnectDB));
                sqlConnection.Close();
            }
        }
    }
}
