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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Xml.Linq;
using Encoder = System.Drawing.Imaging.Encoder;

public partial class LicenceNew : System.Web.UI.Page
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

            string id = Request.QueryString["SchoolId"];
            lblSchoolId.Text = id;

            LoadSchools(id);

            LicenceCategory();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "select Fullname from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
                
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

    public void LoadSchools(string SchoolId)
    {
        try
        {
            string sql = @"Select  [School_name], [Admin_name], [Admin_email] from AllSchools
                        where [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", SchoolId);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                txtSchoolName.Text = dr.IsDBNull(0) ? string.Empty : dr.GetString(0);
                lblAdminName.Text = dr.IsDBNull(1) ? string.Empty : dr.GetString(1);
                txtEmail.Text = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
            }

            dr.Close();
        }
        catch (Exception ex)
        {

            lblError.Text = "Username error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
        finally
        {
            //
        }
    }

    private void LicenceCategory()
    {
        drpCategory.Items.Clear();
        drpCategory.Items.Add("");

        drpCategory.Items.Add("Freemium");
        drpCategory.Items.Add("One Month");
        drpCategory.Items.Add("Two Months");
        drpCategory.Items.Add("One Term");
        drpCategory.Items.Add("Two Terms");
        drpCategory.Items.Add("One School Year");

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        //
        try
        {
            string schoolId = lblSchoolId.Text.Trim();

            //Get date and expiry 
            DateTime date1 = Convert.ToDateTime(txtStartDate.Text.Trim());
            DateTime Expiry = Convert.ToDateTime(txtExpiryDate.Text.Trim());

            //Generating license for the users
            string Licence = schoolId.ToString();

            string FirstCharacter = "MI";
            string LastCharacter = "S";

            string key = FirstCharacter + Licence + Expiry + LastCharacter;

            string FullLicence = Encrypt(key, true);

            //check if licence already exists
            string sql3 = "select * from [T_Sys_Licence] where [LicenceKey] = @key";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@key", FullLicence);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {
                Reset();
                lblError.Text = "This Key Already exists, please check the School ID and the licence duration!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                
                txtStartDate.Focus();
                dr3.Close();
                dr3.Dispose();
                return;
            }
            else
            {
                dr3.Close();
                dr3.Dispose();

                //Insert into T_sys_Licence
                string sql = "Insert into [T_Sys_Licence] ([LicenceKey],[Status],[Category]," +
                    " [Used],[SchoolId],[DateUpdated],[ExpiryDate],[Postedby],[DateCreated])" +
                    " Values (@licence, @status, @category, @used, @schoolId, @dateupdated," +
                    " @expirydate, @user, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@licence", FullLicence);
                cmd.Parameters.AddWithValue("@status", "Active");
                cmd.Parameters.AddWithValue("@category", drpCategory.Text.Trim());
                cmd.Parameters.AddWithValue("@used", "false");
                cmd.Parameters.AddWithValue("@schoolId", schoolId);
                cmd.Parameters.AddWithValue("@dateupdated", txtStartDate.Text.Trim());
                cmd.Parameters.AddWithValue("@expirydate", txtExpiryDate.Text.Trim());
                cmd.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //invalidate all 
                string sql0 = @"UPDATE SL SET SL.LicenceKey = T_SL.LicenceKey, 
                    SL.Mode = T_SL.Category,   SL.DateUpdated = T_SL.DateUpdated,
                       SL.ExpiryDate = T_SL.ExpiryDate FROM School_Licence as SL 
                    INNER JOIN T_Sys_Licence AS T_SL ON SL.SchoolId = T_SL.SchoolId 
                    where SL.SchoolId = @Id and T_SL.LicenceKey = @key";

                SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
                cmd0.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
                cmd0.Parameters.AddWithValue("@key", FullLicence);
                cmd0.ExecuteNonQuery();
                cmd0.Dispose();

                string sql1 = @" Update [T_Sys_Licence] set Status = @status, Used = @used
                                where LicenceKey = @key";
                SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
                cmd1.Parameters.AddWithValue("@status", "Expired");
                cmd1.Parameters.AddWithValue("@used", "True");
                cmd1.Parameters.AddWithValue("@key", FullLicence);
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();

                sendEmail();
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Licence Updated Successfully!')", true);
            }

        }
        catch (Exception ex)
        {
            
            lblError.Text = "There was an error " + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }

    private void Reset()
    {
        txtStartDate.Text = string.Empty;
        txtExpiryDate.Text = string.Empty;
        drpCategory.SelectedIndex = -1;
    }

    //Encrypt Licence
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

        message.Subject = "mySchool License - " + txtStartDate.Text.Trim();
        string mailAddress = txtEmail.Text.Trim();

        message.To.Add(new MailAddress(mailAddress));
        string Client = lblAdminName.Text.Trim();
        string School = txtSchoolName.Text.Trim();
        string Category = drpCategory.Text.Trim();
        string Endate = txtExpiryDate.Text.Trim();

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
        <p>Licence for " + School + @" has been activated successfully!</p>
        <p>This licence is for " + Category + @" and will expire on " + Endate + @"</p>
      
      <!-- Footer -->
      <div style='background-color: #f4f4f4; padding: 10px; text-align: center; font-size: 12px;'>
        <p>&copy; 2026 Innosoft. All rights reserved.</p>
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
        Response.Redirect("LicenceGen.aspx");
    }
}