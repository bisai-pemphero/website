using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Data;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using static QRCoder.PayloadGenerator.ShadowSocksConfig;
using System.Threading.Tasks;

public partial class Fees : System.Web.UI.Page
{
    string appconStr;
    string server, appdb, user, password, version;
    SqlConnection appconSQL2;

    private void readConf()
    {
        System.IO.StreamReader sr;
        {
            sr = System.IO.File.OpenText(Server.MapPath("dbconn.ini"));


            string s = "";
            string[] rfInfo = new string[2];
            char SplitChar = '=';
            while ((s = sr.ReadLine()) != null)
            {
                if (!(s.Trim() == "") || s.StartsWith("#"))
                {
                    rfInfo = s.Split(SplitChar);
                    switch (rfInfo[0].Trim().ToLower())
                    {
                        case "server":
                            server = rfInfo[1].Trim();
                            break;
                        case "user":
                            user = rfInfo[1].Trim();
                            break;
                        case "password":
                            password = rfInfo[1].Trim();
                            break;
                        case "appdb":
                            appdb = rfInfo[1].Trim();
                            break;
                        case "version":
                            version = rfInfo[1].Trim();
                            break;


                    }
                }
            }
        }
    }

    private void dbconnect()
    {
        appconStr = "Data Source=" + server + ";user id=" + user + ";password=" + password + ";max pool size= 65536;Initial Catalog=" + appdb + ";";
        appconSQL2 = new System.Data.SqlClient.SqlConnection(appconStr);
        appconSQL2.Open();

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        readConf();
        dbconnect();

       

        
    }

    public class SmsSender
    {
        public async Task<string> SendSmsAsync(string apiKey, string password, string text, string number, string senderId)
        {
            var client = new RestClient("https://telcomw.com");
            var request = new RestRequest("/api-v2/send", Method.Post);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("api_key", apiKey);
            request.AddParameter("password", password);
            request.AddParameter("text", text);
            request.AddParameter("numbers", number);
            request.AddParameter("from", senderId);

            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
           }
}