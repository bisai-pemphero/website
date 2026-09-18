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

            string id = Request.QueryString["SchoolId"];

            lblSchoolId.Text = id;

            LoadSchools(id);
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
            string sql = @"Select  [School_name], [School_email], [PhoneNumber], [School_address], 
                        [Slogan], [Logo], [Letterhead], [Admin_name], [Admin_email], [Admin_phone], 
                        [Admin_password] from AllSchools where [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", SchoolId);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                txtSchoolName.Text = dr.IsDBNull(0) ? string.Empty : dr.GetString(0);
                txtSchoolEmail.Text = dr.IsDBNull(1) ? string.Empty : dr.GetString(1);
                txtBusinessContact.Text = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                txtAddress.Text = dr.IsDBNull(3) ? string.Empty : dr.GetString(3);
                txtMoto.Text = dr.IsDBNull(4) ? string.Empty : dr.GetString(4);
                lblLogo.Text = dr.IsDBNull(5) ? string.Empty : dr.GetString(5);
                lblLetterhead.Text = dr.IsDBNull(6) ? string.Empty : dr.GetString(6);
                txtAdminName.Text = dr.IsDBNull(7) ? string.Empty : dr.GetString(7);
                txtEmail.Text = dr.IsDBNull(8) ? string.Empty : dr.GetString(8);
                txtphone.Text = dr.IsDBNull(9) ? string.Empty : dr.GetString(9);

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


    protected void btnSave_Click(object sender, EventArgs e)
    {
        string logo = lblLogo.Text;
        string letterhead = lblLetterhead.Text;

        DeleteClientImages(logo, letterhead);

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

                //Insert into Directors
                string sql = @"Update AllSchools set [School_name] = @schoolname, [School_email] = @schoolemail,
                [PhoneNumber] = @schoolPhone, [School_address] = @schoolAddress, [Slogan] = @slogan, 
                [Logo] = @logo, [Letterhead] = @letterhead, [Admin_name] = @adminame, [Admin_email] = @adminemail, 
                [Admin_phone] = @adminphone   where [SchoolId] = @schoolId";

                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@schoolname", txtSchoolName.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolemail", txtSchoolEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolPhone", "");
                cmd.Parameters.AddWithValue("@schoolAddress", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@slogan", txtMoto.Text.Trim());
                cmd.Parameters.AddWithValue("@logo", logoFileName);
                cmd.Parameters.AddWithValue("@letterhead", letterheadFileName);   
                cmd.Parameters.AddWithValue("@adminame", txtAdminName.Text.Trim());
                cmd.Parameters.AddWithValue("@adminemail", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@adminphone", lblPhoneNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                Reset();

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'School Updated Successfully!')", true);
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

    public void DeleteClientImages(string logo, string letterhead)
    {
        // physical paths

        string LogoPath = Server.MapPath("~/img/Schooldocs/") + logo;
        string LetterheadPath = Server.MapPath("~/img/Schooldocs/") + letterhead;

        try
        {
            if (File.Exists(LogoPath))
            {
                File.Delete(LogoPath);        // delete logo
            }

            if (File.Exists(LetterheadPath))
            {
                File.Delete(LetterheadPath);      // delete letter heard
            }
        }
        catch (IOException ioEx)
        {
            // log or surface the error as needed
            lblError.Text = "File‑system error: " + ioEx.Message;
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
        txtEmail.Text = string.Empty;
        txtAdminName.Text = string.Empty;
        txtAddress.Text = string.Empty;
        lblError.Text = string.Empty;
        txtSchoolEmail.Text = string.Empty;
        txtMoto.Text = string.Empty;
        txtBusinessContact.Text = string.Empty;
    }


}