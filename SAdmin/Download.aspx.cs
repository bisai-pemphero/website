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
        }

        LoadDet();
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


    //Load general Report
    public DataTable LoadDetailedReport()
    {
        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;


        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"SELECT [School_name], [Admin_name], [School_address], 
             [Admin_phone], [Admin_email], [School_email], [PhoneNumber] FROM [AllSchools]",
                connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }
        return dt;
    }

    public void LoadDet()
    {
        DataTable generalReport = LoadDetailedReport();
        if (generalReport.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in generalReport.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["School_name"] + "</td>");
                sb.Append("<td>" + row["Admin_name"] + "</td>");
                sb.Append("<td>" + row["School_address"] + "</td>");
                sb.Append("<td>" + row["Admin_phone"] + "</td>");
                sb.Append("<td>" + row["Admin_email"] + "</td>");
                sb.Append("<td>" + row["School_email"] + "</td>");
                sb.Append("<td>" + row["PhoneNumber"] + "</td>");
                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            generalReportPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            generalReportPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }


    protected void btnAddNewSchool_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Admin/AddSchool.aspx");
    }
}