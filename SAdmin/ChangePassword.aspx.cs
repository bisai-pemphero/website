using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class ChangingPassword : System.Web.UI.Page
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

        if (Session["USER"] != null) lblSession.Text = Session["USER"].ToString();
        else
        {
            this.Response.Redirect("../CommonPages/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadUsername();

            string initial = GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = @"select  Fullname, Password, SchoolId from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                txtFullname.Text = dr.GetString(0);
                lblOldpassword.Text = dr.GetString(1);
                lblUser.Text = dr.GetString(0);
                lblSchoolId.Text = dr.GetInt32(2).ToString();
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Username error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

        }
        finally
        {
            //
        }
    }

    public static string GetFirstLetters(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string[] words = input.Split(' ');
        StringBuilder result = new StringBuilder();

        foreach (string word in words)
        {
            if (word.Length > 0)
            {
                result.Append(word[0]);
            }
        }
        return result.ToString();
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        try
        {
            string Currentpassword = Encrypt(txtCurrentPassword.Text.Trim(), true);

            if (Currentpassword != lblOldpassword.Text.Trim())
            {
                lblError.Text = "Old Password Not Matching, Try Again";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                return;
            }

            string sql = @"UPDATE Users SET Password= @password  
                    WHERE  Username = @username and SchoolId =@Id ";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            cmd.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@password", Encrypt(txtNewPassword.Text.Trim(), true));
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            txtCurrentPassword.Text = "";
            txtNewPassword.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Password Updated Successfully!')", true);
          
        }
        catch (Exception ex)
        {
            // Handle the exception here
            lblError.Text = "Update Password Error." + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                 "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

        }
    }

    private void Reset()
    {
      //
    }

    public static string Encrypt(string toEncrypt, bool useHashing)
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