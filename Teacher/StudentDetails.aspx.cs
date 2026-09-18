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

public partial class StudentDetails : System.Web.UI.Page
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

            string id = Request.QueryString["studentId"];

            LoadStudentDetails(id);
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
        Response.Redirect("ViewStudent.aspx");
    }

    private void LoadStudentDetails(string studentId)
    {
         string query = @"Select S.[AdmissionNo], S.[FirstName], S.[Middlename], S.[LastName],
	    S.[Gender], S.[DateOfBirth], C.[ClassName], SP.Fullname, SP.[PhoneNumber], SP.[AlternatePhone],
	    SP.[Email], SP.[Address], SP.[Relationship], SP.[Occupation],  S.[AdmissionDate]
	    from Students as S
	    Join Classes as C on C.ClassID = S.[CurrentClassID]
	    Join StudentParent as SP on SP.ParentId = S.ParentID
	    Where S.[IsDeleted] = 0 and S.Status = 'Active' And S.StudentID = @studentId";

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    litAdmission.Text = reader["AdmissionNo"].ToString();
                    litName.Text = reader["FirstName"].ToString() + " " + reader["Middlename"].ToString() + " " + reader["LastName"].ToString();
                    litGender.Text = reader["Gender"].ToString();
                    litDob.Text = reader["DateOfBirth"].ToString();
                    litClass.Text = reader["ClassName"].ToString();
                    litParent.Text = reader["Fullname"].ToString();
                    litPhone.Text = reader["PhoneNumber"].ToString() + "/" + reader["AlternatePhone"].ToString();
                    litEmail.Text = reader["Email"].ToString();
                    litAdress.Text = reader["Address"].ToString();
                    litRelationship.Text = reader["Relationship"].ToString();
                    litOccupation.Text = reader["Occupation"].ToString();
                    litAdmissionDate.Text = reader["AdmissionDate"].ToString();

                    //for pdf
                    litAdmissionPdf.Text = litAdmission.Text;
                    litNamePdf.Text = litName.Text;
                    litGenderPdf.Text = litGender.Text;
                    litDobPdf.Text = litDob.Text;
                    litClassPdf.Text = litClass.Text;
                    litParentPdf.Text = litParent.Text;
                    litPhonePdf.Text = litPhone.Text;
                    litEmailPdf.Text = litEmail.Text;
                    litAdressPdf.Text = litAdress.Text;
                    litRelationshipPdf.Text = litRelationship.Text;
                    litOccupationPdf.Text = litOccupation.Text;
                    litAdmissionDatePdf.Text = litAdmissionDate.Text;

                    // generation date
                    litGenDate.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
                }
                reader.Close();
            }
        }
    }
}