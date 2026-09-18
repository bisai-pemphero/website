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

public partial class Dashboard : System.Web.UI.Page
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

           string initial =  GetFirstLetters(lblUser.Text.Trim());
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

            string sql2 = "Select Count([School_name]) from AllSchools";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            SqlDataReader dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
               
                lblTotalSchools.Text = dr2.GetInt32(0).ToString();
            }

            dr2.Close();

            string sql3 = "Select Count([Username]) from [Users]";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                lblTotalUsers.Text = dr3.GetInt32(0).ToString();
            }

            dr3.Close();

            //students
            //string sql4 = "Select Count([Username]) from [Users]";
            //SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            //SqlDataReader dr4 = cmd4.ExecuteReader();
            //while (dr4.Read())
            //{
            //    lblTotalUsers.Text = dr4.GetInt32(0).ToString();
            //}

            //dr4.Close();

            //active schools
            string sql5 = "  Select Count([LicenceKey]) from School_Licence where Status = 'Active'";
            SqlCommand cmd5 = new SqlCommand(sql5, appconSQL2);
            SqlDataReader dr5 = cmd5.ExecuteReader();
            while (dr5.Read())
            {
                lblTotalActive.Text = dr5.GetInt32(0).ToString();
            }

            dr5.Close();

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
}