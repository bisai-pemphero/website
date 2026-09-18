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

    protected void SendSMS(string myPhone, string myMessage)
    {
        try
        {
            //school sender Id
            string sAPIKey = "ApiKey";
            string sNumber = myPhone;
            string sMessage = myMessage;
            string sSenderID = "SenderId";
            string sChannel = "promo";
            string sRoute = "15";
            string sURL = " https://sms.cloud265.com/api/mt/SendSMS?APIKEY=" + sAPIKey + "&senderid=" + sSenderID + "&channel=" + sChannel + "&DCS=0&flashsms=0&number=" + sNumber + "&text=" + sMessage + "&route=" + sRoute;
            string sResponse = ProcessSMS(sURL);
            lblSuccess.Text = "SUCCESSFULL";

        }
        catch// (Exception ex)
        {

        }
    }
    public static string ProcessSMS(string sURL)
    {
        string Responce = "";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
        request.MaximumAutomaticRedirections = 4;
        request.Credentials = CredentialCache.DefaultCredentials;
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream(
            );
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string sResponse = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            Responce = sResponse;
        }
        catch (Exception ex)
        {
            Responce = ex.ToString();
        }

        return Responce;
    }
    public class MessageStructure
    {
        public string sender { get; set; }
        public string numbers { get; set; }
        public string message { get; set; }
    }

    public void SendSmsNotification()
    {
        string Phone = "265882196556";
        string Message = "Dear Parent";

        SendSMS(Phone, Message);
    }




    protected void btnSend_Click(object sender, EventArgs e)
    {
        SendSmsNotification();
    }
}