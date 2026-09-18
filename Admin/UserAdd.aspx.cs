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
using Encoder = System.Drawing.Imaging.Encoder;

public partial class AddSchool : System.Web.UI.Page
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

            LoadSchools();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "select Fullname, SchoolId from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
                lblSchoolId.Text = dr.GetInt32(1).ToString();
            }

            dr.Close();
        }
        catch (Exception ex)
        {

            lblError.Text = "Username error" + ex;
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

    public void LoadSchools()
    {
        try
        {
           

            drpSchools.Items.Clear();
            drpSchools.Items.Add("");
            string sql2 = @"Select SchoolId, School_name from AllSchools Order by School_name";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            SqlDataReader dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                drpSchools.Items.Add(dr2.GetInt32(0) + "|" + dr2.GetString(1));
            }

            dr2.Close();

            drpRole.Items.Clear();
            drpRole.Items.Add("");
            string sql3 = @"Select RoleId, RoleName from Roles";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                drpRole.Items.Add(dr3.GetInt32(0) + "|" + dr3.GetString(1));
            }

            dr3.Close();

            drpLocation.Items.Clear();
            drpLocation.Items.Add("");
            string sql4 = @"Select [DistrictName] from Districts order by DistrictName";
            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            SqlDataReader dr4 = cmd4.ExecuteReader();
            while (dr4.Read())
            {
                drpLocation.Items.Add(dr4.GetString(0));
            }

            dr4.Close();
        }
        catch (Exception ex)
        {

            lblError.Text = "Username error" + ex;
        }
        finally
        {
            //
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            //Check if already exist
            string sql3 = @"SELECT * FROM  Users WHERE   username= @username";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@username", txtUserEmail.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {

                lblError.Text = "This username already exists!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                return;

            }
            else
            {
                dr3.Close();
                dr3.Dispose();

                string[] Roles = drpRole.Text.Split('|');
                string roleId = Roles[0];

                string[] School = drpSchools.Text.Split('|');
                string schoolId = School[0];

                string Userpassword = Encrypt(txtPassword.Text.Trim(), true);

                //insert into register
                string sql = @"Insert into Users ( [Username], [Password],[Fullname],[RoleId],
                [Created_On],[SchoolId], [Location], Position) VALUES (@username, @password, @fullname, @roleId,
                GETDATE(), @schoolId, @location, @position)";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@username", txtUserEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@password", Userpassword);
                cmd.Parameters.AddWithValue("@fullname", txtFullname.Text.Trim());
                cmd.Parameters.AddWithValue("@roleId", roleId);
                cmd.Parameters.AddWithValue("@schoolId", schoolId);
                cmd.Parameters.AddWithValue("@location", drpLocation.Text.Trim());
                cmd.Parameters.AddWithValue("@position", txtPosition.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sendEmail();
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'User Registered Successfully!')", true);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }

    public void Reset()
    {
        txtUserEmail.Text = string.Empty;
        txtFullname.Text = string.Empty;
        txtPassword.Text = string.Empty;
        txtPosition.Text = string.Empty;
        drpLocation.SelectedIndex = -1;
        drpRole.SelectedIndex = -1;
        drpSchools.SelectedIndex = -1;
    }

    //Encrypt User password
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

        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        tdes.Clear();

        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }


   public void sendEmail()
    {
         string fromMail = "inosoftmw@gmail.com";
        string fromPassword = "agcgzyefkyrdtmfa";
        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);

        message.Subject = "School System Login Details!";
        string mailAddress = txtUserEmail.Text.Trim();

        message.To.Add(new MailAddress(mailAddress));

        string Client = txtFullname.Text.Trim();
        string Username = txtUserEmail.Text.Trim();
        string Password = txtPassword.Text.Trim();

        string login = lblLoginEmail.Text.Trim();

        message.Body = @"
<html>
  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
    <div style='width: 100%; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 10px; overflow: hidden;'>

      <!-- Header with company logo -->
      <div style='background-color: white; padding: 20px; text-align: center;'>
        <img src='https://apps.innosoftmw.com/myschool/img/logo.png' alt='Innosoft' style='max-width: 150px; height: auto;' />
      </div>

      <!-- Email Content -->
      <div style='padding: 20px;'>
        <p>Dear <strong>" + Client + @"</strong>,</p>
        <p>Welcome to <strong>mySchool Portal</strong>! We are excited to have you on board.</p>
        <p>Your login credentials for accessing the portal are as follows:</p>
        <ul style='list-style-type: none; padding-left: 0;'>
         
          <li><strong>Email/Username:</strong> " + Username + @"</li>
          <li><strong>Password:</strong> " + Password + @"</li>
        </ul>
        <p>Please click the link below to login to the portal:</p>
        <p><a href='https://apps.innosoftmw.com/sukuluyanga/CommonPages/Login.aspx' style='background-color: #007BFF; color: #fff; padding: 10px 15px; text-decoration: none; border-radius: 5px;'>Login to mySchool Portal</a></p>
        <p>If you have any questions or need assistance, feel free to contact us on WhatsApp: <strong><a href='https://wa.me/265993744323' style='color: #007BFF;'>+265 993 744 323</a></strong> or via email at <a href='mailto:inosoftmw@gmail.com'>inosoftmw@gmail.com</a>.</p>
      </div>

      <!-- Footer -->
      <div style='background-color: #f4f4f4; padding: 10px; text-align: center; font-size: 12px;'>
        <p>&copy; 2025 Innosoft. All rights reserved.</p>
      </div>
    </div>
  </body>
</html>";

        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        smtpClient.Send(message);

    }

 

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("UsersView.aspx");
    }
}