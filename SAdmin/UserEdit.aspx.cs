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
using System.Web.Security;
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
            string id = Request.QueryString["userId"];

            lblUserId.Text = id;
            
            RoadRoles();
            LoadUsername();

            string initial = GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

            
            LoadDetails(id);
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

    public void RoadRoles()
    {
        try
        {
            drpRole.Items.Clear();
            drpRole.Items.Add("");
            string sql3 = @"Select RoleId, RoleName from Roles 
            where RoleId != 1 AND RoleId != 2";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                drpRole.Items.Add(dr3.GetInt32(0) + "|" + dr3.GetString(1));
            }
            dr3.Close();
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


    public void LoadDetails(string id)
    {
        try
        {
            string sql = @"Select [Username], [Fullname], [RoleId]
            from Users where UserId = @id";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                txtUserEmail.Text = dr.GetString(0);
                txtFullname.Text = dr.GetString(1);
                drpRole.SelectedIndex = dr.GetInt32(2);
            }

            dr.Close();
        }
        catch (Exception ex)
        {

            lblError.Text = "User details error" + ex;
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
                string[] Roles = drpRole.Text.Split('|');
                string roleId = Roles[0];


                string schoolId = lblSchoolId.Text.Trim();

                string Userpassword = Encrypt(txtPassword.Text.Trim(), true);

                //insert into register
                string sql = @"Update Users set [Username] = @username,
                [Password] = @password, [Fullname] = @fullname, [RoleId] = @roleId where [UserId] = @Id";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@username", txtUserEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@password", Userpassword);
                cmd.Parameters.AddWithValue("@fullname", txtFullname.Text.Trim());
                cmd.Parameters.AddWithValue("@roleId", roleId);
                cmd.Parameters.AddWithValue("@Id", lblUserId.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //sendEmail();
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'User Updated Successfully!')", true);
            
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
        drpRole.SelectedIndex = -1;
       
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


    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //delete user
        string sql = @"DELETE FROM Users WHERE UserId = @Id";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        cmd.Parameters.AddWithValue("@Id", lblUserId.Text.Trim());
        cmd.ExecuteNonQuery();
        cmd.Dispose();

        Reset();
        ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'User Deleted Successfully!')", true);
    }
}