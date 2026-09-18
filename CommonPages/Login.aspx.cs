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

public partial class LoginPage : System.Web.UI.Page
{
    string appconStr;
    string server, appdb, user, password, version;
    SqlConnection appconSQL2;

    private void readConf()
    {
        System.IO.StreamReader sr;
        {
            sr = System.IO.File.OpenText(Server.MapPath("../dbconn.ini"));


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

        Greeting();
    }


    public void Greeting()
    {
        DateTime now = DateTime.Now;
        int hour = now.Hour;

        if (hour >= 5 && hour < 12)
        {
            lblGreeting.Text = "Welcome, Good morning!";
        }
        else if (hour >= 12 && hour < 17)
        {
            lblGreeting.Text = "Welcome back, Good Afternoon!";
        }
        else if (hour >= 17 && hour < 21)
        {
            lblGreeting.Text = "Hie there, Good Evening!";
        }
        else
        {
            lblGreeting.Text = "Welcome  back, Sign In";
        }

        lblError.Text = "Please enter your credentials.";
    }




    protected void btnLogin_Click(object sender, EventArgs e)
    {
      

        try
        {
            int Userlevel = 0;
            int schoolId = 0;
           
            // Get the password from the text box and decrypt it if necessary
            string UserPassword = Decrypt(txtPassword.Text.Trim(), true);

            // Get the username from the text box
            string userName = username.Text.Trim();

            // Create a parameterized SQL query
            string sql3 = "SELECT RoleId, SchoolId FROM Users WHERE Username=@Username AND Password=@Password";

            // Create a SqlCommand object with the parameterized query and the connection
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);

            // Add parameters to the SqlCommand
            cmd3.Parameters.AddWithValue("@Username", userName);
            cmd3.Parameters.AddWithValue("@Password", UserPassword);

            
            cmd3.CommandTimeout = 1000000;
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {
                while (dr3.Read())
                {
                    Userlevel = dr3.GetInt32(0);
                    schoolId = dr3.GetInt32(1);
                }
                Session["USER"] = username.Text;

                //update Logs
                dr3.Close();
                dr3.Dispose();

                string computername = System.Environment.MachineName;
                string myIP = Dns.GetHostByName(computername).AddressList[0].ToString();

                string sql1 = "INSERT INTO T_Sys_Operate_Log(Operator_No, Operator_Name, Operate_Type, Module_Name, User_No,  Meter_No, Log_Remark, IP_Address, Computer_Name, Station_No, Update_Flag, Update_Date,  Create_Date, School_Id ) VALUES('" + username.Text.Trim() + "','" + username.Text.Trim() + "', 'Login', 'User Login', 'none', 'None', 'Login Successfull', '" + myIP + "', '" + computername.ToString() + "', 'NULL', '0', GETDATE(), GETDATE(), '" + schoolId + "')";

                SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
                cmd1.CommandTimeout = 1000000;
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();

                if (Userlevel == 1)
                {
                    Response.Redirect("../Admin/Dashboard.aspx");
                }
                else if (Userlevel == 2)
                {
                    Response.Redirect("../SAdmin/Dashboard.aspx");
                }
                else if (Userlevel == 3)
                {
                    Response.Redirect("../Headmaster/Dashboard.aspx");
                }
                else if (Userlevel == 5)
                {
                    Response.Redirect("../Accounts/Dashboard.aspx");
                }
                else if (Userlevel == 6)
                {
                    Response.Redirect("../Teacher/Dashboard.aspx");
                }
                else
                {
                    //go to login
                    Response.Redirect("../CommonPages/Login.aspx");
                }
            }
            else
            {
                username.Text = "";
                txtPassword.Text = "";
                lblError.Text = "Incorrect login details! Try again";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalScript", "$('#myModal').modal('show');", true);
                return;
            }
            dr3.Close();
            dr3.Dispose();
            username.Text = "";
            txtPassword.Text = "";
        }
        catch (Exception ex)
        {
            lblError.Text = "There was an error!" + ex;
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalScript", "$('#myModal').modal('show');", true);
        }
    }

    public static string Decrypt(string toEncrypt, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = System.Text.UTF8Encoding.UTF8.GetBytes(toEncrypt);

        System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();

        string key = "06061982";

        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(key));


            hashmd5.Clear();
        }
        else
            keyArray = System.Text.UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

        tdes.Key = keyArray;

        tdes.Mode = CipherMode.ECB;


        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();

        byte[] resultArray =
          cTransform.TransformFinalBlock(toEncryptArray, 0,
          toEncryptArray.Length);

        tdes.Clear();

        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

}