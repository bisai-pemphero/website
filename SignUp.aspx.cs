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

public partial class SignUp : System.Web.UI.Page
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


    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            PhoneNumber();

            if (txtSchoolName.Text == string.Empty)
            {
                lblError.Text = "Please enter School name";
                txtSchoolName.Focus();
                return;
            }
         
            if (txtSchoolEmail.Text == string.Empty)
            {
                lblError.Text = "Please enter Schools Email Address";
                txtSchoolEmail.Focus();
                return;
            }
            if (txtPassword.Text == string.Empty)
            {
                lblError.Text = "Please enter your Password";
                txtPassword.Focus();
                return;
            }
            if (txtEmail.Text == string.Empty)
            {
                lblError.Text = "Please enter your Email";
                txtEmail.Focus();
                return;
            }
            if (lblPhoneNumber.Text == string.Empty)
            {
                lblError.Text = "Please enter your Phone number";
                txtphone.Focus();
                return;
            }
            if (txtAdminName.Text == string.Empty)
            {
                lblError.Text = "Please enter your Name";
                txtAdminName.Focus();
                return;
            }
            if (txtAddress.Text == string.Empty)
            {
                lblError.Text = "Please enter your Address";
                txtAddress.Focus();
                return;
            }
            if (txtSchoolPhone.Text == string.Empty)
            {
                lblError.Text = "Please enter Schools primary contact";
                txtSchoolPhone.Focus();
                return;
            }
            if (txtSlogan.Text == string.Empty)
            {
                lblError.Text = "Please enter Schools Motto";
                txtSlogan.Focus();
                return;
            }

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
                lblSuccess.Text = "";
                lblError.Text = "This School Already exists, if you are registering multiple schools, add extension to the school name";
                lblSuccess.Text = "";
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
                        lblError.Text = "Please use a different Email address!";
                        lblSuccess.Text = "";
                        txtEmail.Focus();
                        readerEmail.Close();
                        return;
                    }
                    readerEmail.Close();
                }

                //Insert into Directors
                string sql = @"Insert into AllSchools ([School_name], [School_email], [PhoneNumber], [School_address],
                [Slogan], [Admin_name],[Admin_email], [Admin_phone], [Admin_password], [Date_Registered], [RegisteredBy])
                
                VALUES (@schoolname, @schoolemail, @schoolPhone, @schoolAddress, @slogan, @adminame, @adminemail, @adminphone,
                        @adminPassword, GETDATE(), @user)";

                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@schoolname", txtSchoolName.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolemail", txtSchoolEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolPhone", txtSchoolPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolAddress", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@slogan", txtSlogan.Text.Trim());
                cmd.Parameters.AddWithValue("@adminame", txtAdminName.Text.Trim());
                cmd.Parameters.AddWithValue("@adminemail", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@adminphone", lblPhoneNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@adminPassword", txtPassword.Text.Trim());
                cmd.Parameters.AddWithValue("@user", txtAdminName.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();

       
                //Get School Id
                int schoolId = 0;
                string sql0 = "SELECT  [SchoolId]  FROM   AllSchools where [School_name] = @name and [School_email] = @email and [Admin_phone] = @number";
                SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
                cmd0.Parameters.AddWithValue("@name", txtSchoolName.Text.Trim());
                cmd0.Parameters.AddWithValue("@email", txtSchoolEmail.Text.Trim());
                cmd0.Parameters.AddWithValue("@number", lblPhoneNumber.Text.Trim());
                SqlDataReader dr0 = cmd0.ExecuteReader();
                while (dr0.Read())
                {
                    schoolId = dr0.GetInt32(0);
                }
                dr0.Close();

                string Userpassword = Encrypt(txtPassword.Text.Trim(), true);

                //Insert into Users
                string sql2 = "INSERT INTO Users ([Username], " +
                    "[Password], [Fullname], [RoleId], [Created_On], [SchoolId])" +
                    "VALUES (@username, @password, @fullname, @RoleId, GETDATE(), @schoolId)";
                SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
                cmd2.Parameters.AddWithValue("@username", txtEmail.Text.Trim());
                cmd2.Parameters.AddWithValue("@password", Userpassword);
                cmd2.Parameters.AddWithValue("@RoleId", 2);
                cmd2.Parameters.AddWithValue("@fullname", txtAdminName.Text.Trim());
                cmd2.Parameters.AddWithValue("@schoolId", schoolId);
                cmd2.ExecuteNonQuery();
                cmd2.Dispose();


                //Get date and expiry 
                DateTime date1 = DateTime.Now;
                DateTime Expiry = date1.AddDays(30);

                //creating license for free users
                string Licence = schoolId.ToString();

                string key = date1 + Licence + Expiry;


                string FullLicence = Encrypt(key, true);


                //Insert into school licences
                string sql4 = "Insert Into [School_Licence] ([LicenceKey], [Status]," +
                    " [Mode], [SchoolId], [Directors_Email], [DateUpdated], [ExpiryDate], [Postedby],[DatePosted])" +
                    "VALUES (@key, @status, @mode, @schoolId, @email, @date1, @Expiry, @user, GETDATE())";
                SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
                cmd4.Parameters.AddWithValue("@key", FullLicence);
                cmd4.Parameters.AddWithValue("@status", "Active");
                cmd4.Parameters.AddWithValue("@mode", "Freemium");
                cmd4.Parameters.AddWithValue("@schoolId", schoolId);
                cmd4.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd4.Parameters.AddWithValue("@date1", date1);
                cmd4.Parameters.AddWithValue("@Expiry", Expiry);
                cmd4.Parameters.AddWithValue("@user", txtAdminName.Text.Trim());
                cmd4.ExecuteNonQuery();
                cmd4.Dispose();

                string sql5 = @" Insert into [InnosoftClients] ([Fullname],[ContactNumber], [BusinessContact], 
	            [PersonalEmail], [BusinessEmail] ,[Business_name], [Client_Address],[DateCreated], [Postedby])
	            VALUES (@full_name, @contact, @business_contact, @email, @business_email, @school_name, @address,
	            GETDATE(), @full_name)";
                SqlCommand cmd5 = new SqlCommand(sql5, appconSQL2);
                cmd5.Parameters.AddWithValue("@full_name", txtAdminName.Text.Trim());
                cmd5.Parameters.AddWithValue("@contact", txtphone.Text.Trim());
                cmd5.Parameters.AddWithValue("@business_contact", txtSchoolPhone.Text.Trim());
                cmd5.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd5.Parameters.AddWithValue("@business_email", txtSchoolEmail.Text.Trim());
                cmd5.Parameters.AddWithValue("@school_name", txtSchoolName.Text.Trim());
                cmd5.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                cmd5.ExecuteNonQuery();
                cmd5.Dispose();


                SendText();
                sendEmail();
                Reset();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalScript", "$('#myModal').modal('show');", true);
            }
            
            else
            {
                lblError.Text = "Please enter valid email!";
                txtEmail.Focus();
                return;
            }
        }

        catch(Exception ex)
        {
            lblSuccess.Text = string.Empty;
            lblError.Text= "There was an error " + ex;
        }

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
        lblSuccess.Text = string.Empty;
        txtSchoolEmail.Text = string.Empty;
    }

    static string GenerateOtp(int length)
    {
        const string chars = "0123456789";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }



    public void PhoneNumber()
    {

        string inputNumber = txtphone.Text.Trim(); // input number
        string outputNumber = "";

        // Remove any non-digit characters from the input number
        foreach (char c in inputNumber)
        {
            if (char.IsDigit(c))
            {
                outputNumber += c;
            }
        }


        // Check if the output number is in the desired format (12 digits)
        if (outputNumber.Length == 12)
        {
            lblPhoneNumber.Text = outputNumber.ToString();
        }
        else if (outputNumber.Length == 10)
        {

            outputNumber = "265" + outputNumber.Substring(1);
            lblPhoneNumber.Text = outputNumber.ToString();
        }
        else
        {
            lblError.Text = "Invalid number format!, Please enter Phone number in this format 265789789789";
            return;
        }
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

    //protected void txtphone_TextChanged(object sender, EventArgs e)
    //{
    //    PhoneNumber();
    //    txtEmail.Focus();
    //}



    protected void RedirectToWhatsApp(object sender, EventArgs e)
    {
        string phoneNumber = "+265993744323";  
        string message = "Hello, I need assistance.";  

        // Encode the message for the URL
        string encodedMessage = HttpUtility.UrlEncode(message);

        // WhatsApp link format
        string whatsappLink = "https://wa.me/" + phoneNumber + "?text=" + encodedMessage;

        // Redirect to WhatsApp
        Response.Redirect(whatsappLink);
    }

    public void sendEmail()
    {

        string fromMail = "inosoftmw@gmail.com";
        string fromPassword = "agcgzyefkyrdtmfa";
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
        <p>&copy; 2024 Innosoft. All rights reserved.</p>
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
        lblSuccess.Text = "Sent";
    }

    //Send sms
    //begin send sms

    protected void SendSMS(string myPhone, string myMessage)
    {
        try
        {
            //inosoft
             string sAPIKey = "ExdGaBpkqEKr6nqjIs9cUg";
 string sNumber = myPhone;
 string sMessage = myMessage;
 string sSenderID = "INNOSOFT";
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

    public void SendText()
    {
        string Director = txtAdminName.Text;
        string Phone =  lblPhoneNumber.Text.Trim();
        string Message = "Dear " + Director + ", Welcome onboard on mySchool. Follow instructions sent to you Email to use mySchool";

        SendSMS(Phone, Message);
    }
}