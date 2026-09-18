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

public partial class Licence : System.Web.UI.Page
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
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if(txtPassword.Text.Trim() != txtPasswordConfirm.Text.Trim())
        {
            lblError.Text = "Passwords do not Match";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
            return;
        }

        // Get the uploaded files from the HTML5 inputs
        HttpPostedFile logoFile = Request.Files["FULogo"];
        HttpPostedFile letterheadFile = Request.Files["FULetterhead"];

        // Validate both files
        if ((logoFile != null && logoFile.ContentLength > 0)
            && (letterheadFile != null && letterheadFile.ContentLength > 0))
        {
            try
            {
                // Save school logo and letter head 
                string logoFileName = Path.GetFileName(logoFile.FileName);
                string logoSaveLocation = Server.MapPath("~/img/Schooldocs/") + logoFileName;

                string letterheadFileName = Path.GetFileName(letterheadFile.FileName);
                string letterheadSaveLocation = Server.MapPath("~/img/Schooldocs/") + letterheadFileName;

                // Ensure valid formats (jpg, png)
                string logoFileFormat = Path.GetExtension(logoFileName).ToLower();
                string letterheadFileFormat = Path.GetExtension(letterheadFileName).ToLower();

                if (((logoFileFormat == ".jpg" || logoFileFormat == ".png" || logoFileFormat == ".jpeg"))
                    && ((letterheadFileFormat == ".jpg" || letterheadFileFormat == ".png" || letterheadFileFormat == ".jpeg")))

                {
                    // Compress and save the Passport Photo
                    CompressAndSaveImage(logoFile, logoSaveLocation, 50L);
                    CompressAndSaveImage(letterheadFile, letterheadSaveLocation, 50L);
          try
            {
            string email = txtEmail.Text.Trim();
            if (EmailValidator.IsValidEmail(email))
            {
                //proceed
                string sql3 = "select * from AllSchools where School_name = @name and Admin_name = @adminName ";

                using (SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2))
                {
                    cmd3.Parameters.AddWithValue("@name", txtSchoolName.Text.Trim());
                    cmd3.Parameters.AddWithValue("@adminName", txtAdminName.Text.Trim());
                    SqlDataReader dr3 = cmd3.ExecuteReader();
                    if (dr3.HasRows)
                    {

                     lblError.Text = "This School Already exists, if you are registering multiple schools, add extension to the school name";
                     ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                    
                        txtSchoolName.Focus();
                        dr3.Close();

                        return;
                    }
                    dr3.Close();
                }


                // Check if the directors already exists
                string sqlEmail = @"SELECT * FROM Users WHERE [Username]=@name";
                using (SqlCommand cmdEmail = new SqlCommand(sqlEmail, appconSQL2))
                {
                    cmdEmail.Parameters.AddWithValue("@name", txtEmail.Text.Trim());

                    SqlDataReader readerEmail = cmdEmail.ExecuteReader();
                    if (readerEmail.HasRows)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowErrorAlert",
                        "showAlert('error', 'Please use a different Email address');", true);

                        
                        txtEmail.Focus();
                        readerEmail.Close();
                        return;
                    }
                    readerEmail.Close();
                }

                //Insert into Directors
                string sql = @"Insert into AllSchools ([School_name], [School_email], [PhoneNumber], [School_address],
                [Slogan], [Logo], [Letterhead], [Admin_name], [Admin_email], [Admin_phone], [Admin_password], [Date_Registered], [RegisteredBy])
                VALUES (@schoolname, @schoolemail, @schoolPhone, @schoolAddress, @slogan, @logo, @letterhead, @adminame, @adminemail, @adminphone,
                        @adminPassword, GETDATE(), @user)";

                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@schoolname", txtSchoolName.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolemail", txtSchoolEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolPhone", txtBusinessContact.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolAddress", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@slogan", txtMoto.Text.Trim());
                cmd.Parameters.AddWithValue("@logo", logoFileName);
                cmd.Parameters.AddWithValue("@letterhead", letterheadFileName);   
                cmd.Parameters.AddWithValue("@adminame", txtAdminName.Text.Trim());
                cmd.Parameters.AddWithValue("@adminemail", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@adminphone", txtphone.Text.Trim());
                cmd.Parameters.AddWithValue("@adminPassword", txtPassword.Text.Trim());
                cmd.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();


                //Get School Id
               
                string sql0 = @"SELECT  [SchoolId]  FROM   AllSchools where [School_name] = @name
                                and [School_email] = @email and [Admin_phone] = @number";
                SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
                cmd0.Parameters.AddWithValue("@name", txtSchoolName.Text.Trim());
                cmd0.Parameters.AddWithValue("@email", txtSchoolEmail.Text.Trim());
                cmd0.Parameters.AddWithValue("@number", txtphone.Text.Trim());
                SqlDataReader dr0 = cmd0.ExecuteReader();
                while (dr0.Read())
                {
                    lblSchoolId.Text = dr0.GetInt32(0).ToString();
                }
                  dr0.Close();



                            //moved them here
                            string Userpassword = Encrypt(txtPassword.Text.Trim(), true);

                            //Insert into Users
                            string sql2 = @"INSERT INTO Users ([Username], 
                   [Password], [Fullname], [RoleId], [Created_On], [SchoolId])
                   VALUES (@username, @password, @fullname, @RoleId, GETDATE(), @schoolId)";
                            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
                            cmd2.Parameters.AddWithValue("@username", txtEmail.Text.Trim());
                            cmd2.Parameters.AddWithValue("@password", Userpassword);
                            cmd2.Parameters.AddWithValue("@RoleId", 2);
                            cmd2.Parameters.AddWithValue("@fullname", txtAdminName.Text.Trim());
                            cmd2.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                            cmd2.ExecuteNonQuery();
                            cmd2.Dispose();

                            //Get date and expiry 
                            DateTime date1 = DateTime.Now;
                            DateTime Expiry = date1.AddDays(100);
                            //creating license for free users
                            string Licence = lblSchoolId.Text.Trim();

                            string key = date1 + Licence + Expiry;

                            string FullLicence = Encrypt(key, true);

                            //Insert into school licences
                            string sql4 = @"Insert Into [School_Licence] ([LicenceKey], [Status],
                   [Mode], [SchoolId], [Directors_Email], [DateUpdated], [ExpiryDate], [Postedby],[DatePosted])
                   VALUES (@key, @status, @mode, @schoolId, @email, @date1, @Expiry, @user, GETDATE())";
                            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
                            cmd4.Parameters.AddWithValue("@key", FullLicence);
                            cmd4.Parameters.AddWithValue("@status", "Active");
                            cmd4.Parameters.AddWithValue("@mode", "Freemium");
                            cmd4.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                            cmd4.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmd4.Parameters.AddWithValue("@date1", date1);
                            cmd4.Parameters.AddWithValue("@Expiry", Expiry);
                            cmd4.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                            cmd4.ExecuteNonQuery();
                            cmd4.Dispose();

                            //SendText();
                            //sendEmail();
                            Reset();

                            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'School Registered Successfully!')", true);

                        }
                        else
              {
                lblError.Text = "Please enter a valid Email.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);

                txtEmail.Focus();
                return;
              }
        }
        catch (Exception ex)
        {
                        lblError.Text = "Saving Error!" + ex;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                }
                //end saving here 
                }
                else
                {
                    lblError.Text = "Invalid file format. Please upload JPG or PNG.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);

                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error uploading files!" + ex;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);

            }
        }
        else
        {
            lblError.Text = "Please select file to Upload";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);

            FULogo.Focus();
        }
    }

    //image processing
    private void CompressAndSaveImage(HttpPostedFile uploadedFile, string savePath, long quality)
    {
        using (Bitmap originalBitmap = new Bitmap(uploadedFile.InputStream))
        {
            // Set up the image encoder parameters
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder qualityEncoder = Encoder.Quality;
            EncoderParameters encoderParams = new EncoderParameters(1);

            // Set the quality parameter (0L = max compression, 100L = least compression)
            EncoderParameter qualityParam = new EncoderParameter(qualityEncoder, quality);
            encoderParams.Param[0] = qualityParam;

            // Save the compressed image to the server
            originalBitmap.Save(savePath, jpgEncoder, encoderParams);
        }
    }

    private ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    public class EmailValidator
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                // Try to create a MailAddress object with the input email
                MailAddress mail = new MailAddress(email);
                return true; // If it succeeds, the email is valid
            }
            catch (FormatException)
            {
                return false; // If it fails, the email is invalid
            }
        }
    }
    public void Reset()
    {
        txtphone.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtSchoolName.Text = string.Empty;
        txtPassword.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtAdminName.Text = string.Empty;
        txtAddress.Text = string.Empty;
        lblError.Text = string.Empty;
        txtSchoolEmail.Text = string.Empty;
        txtMoto.Text = string.Empty;
        txtBusinessContact.Text = string.Empty;
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
        string fromMail = "pempherobisai@gmail.com";
        string fromPassword = "tgwgtzvmvzcanuux";
        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);

        message.Subject = "Your school has been registered successfully!";
        string mailAddress = txtEmail.Text.Trim();

        message.To.Add(new MailAddress(mailAddress));

        string Client = txtAdminName.Text.Trim();
        string School = txtSchoolName.Text.Trim();
        string Username = txtEmail.Text.Trim();
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
          <li><strong>School Name:</strong> " + School + @"</li>
          <li><strong>Email/Username:</strong> " + Username + @"</li>
          <li><strong>Password:</strong> " + Password + @"</li>
        </ul>
        <p>Please click the link below to login to the portal:</p>
        <p><a href='https://apps.innosoftmw.com/myschool/sms/mySchoolLogin.aspx' style='background-color: #007BFF; color: #fff; padding: 10px 15px; text-decoration: none; border-radius: 5px;'>Login to mySchool Portal</a></p>
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

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Reset();
        txtSchoolName.Focus();
    }
}