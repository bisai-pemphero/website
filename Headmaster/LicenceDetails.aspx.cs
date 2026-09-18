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

public partial class LicenceDetails : System.Web.UI.Page
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

    string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
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
            
            LoadSchoolDetails(id);

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

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("LicenceGen.aspx");
    }

    private void LoadSchoolDetails(string schoolId)
    {
        string query = @"Select A.School_name, A.Slogan, A.Logo,  SL.[Status], SL.[Mode], SL.[DateUpdated], 
        SL.[ExpiryDate] from [School_Licence] as SL Join AllSchools as A on A.SchoolId = SL.SchoolId
        where SL.SchoolId = @schoolId";

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@schoolId", schoolId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    litSchoolName.Text = reader["School_name"].ToString();
                    litSlogan.Text = reader["Slogan"].ToString();
                    litStatus.Text = reader["Status"].ToString();
                    litMode.Text = reader["Mode"].ToString();
                    litStart.Text = reader["DateUpdated"].ToString();
                    litExpiry.Text = reader["ExpiryDate"].ToString();

                    if (reader["Logo"] != DBNull.Value)
                    {
                        
                        string logo = reader["Logo"].ToString();
                        imgSchoolLogo.ImageUrl = "~/img/Schooldocs/" + logo;
                        imgSchoolLogo.Visible = true;
                    }
                    else
                    {
                        imgSchoolLogo.ImageUrl = "../img/logo 2.png";
                    }
                }
                reader.Close();
            }
        }
    }
}