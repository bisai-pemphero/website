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

            LoadAcademicYear();

            LoadClass();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "select Fullname, SchoolId, UserId from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
                lblSchoolId.Text = dr.GetInt32(1).ToString();
                lblTeacherId.Text = dr.GetInt32(2).ToString();
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

   

    

    public void LoadAcademicYear()
    {
        try
        {
            drpAcademicYear.Items.Clear();
            drpAcademicYear.Items.Add("");
            string sql = "SELECT [AcademicyearId], " +
                "CONCAT(DATENAME(YEAR, [Start_year]), '-', YEAR([End_year])) as Academic_year" +
                " from [Academic_Year] where" +
                " [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpAcademicYear.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " Academic Year");
            }

            dr.Close();


        }
        catch (Exception ex)
        {
            lblError.Text = "Load academic year error" + ex;
        }
    }

    public void LoadAllTerms()
    {
        string[] academic = drpAcademicYear.Text.Split('|');
        int academicId = Convert.ToInt32(academic[0]);

        drpTerm.Items.Clear();
        drpTerm.Items.Add("");

        string sql = "SELECT [TermId], [TermName], " +
            "CONCAT(DATENAME(MONTH, [Startdate]), ' ', YEAR([Enddate])) as Period" +
            " from [SchoolTerm] where [SchoolId] = @schoolId and [AcademicYearId] = @academicYearId";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
        cmd.Parameters.AddWithValue("@academicYearId", academicId);
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            drpTerm.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " (" + dr.GetString(2) + ")");
        }

        dr.Close();
    }


    protected void drpAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAllTerms();
        drpTerm.Focus();
    }

  

    protected void drpTerm_SelectedIndexChanged(object sender, EventArgs e)
    {
        //load exams
        LoadExam();
        drpExam.Focus();
    }

    public void LoadExam()
    {
        try
        {

            drpExam.Items.Clear();
            drpExam.Items.Add("");

            string[] term = drpTerm.Text.Split('|');
            int termId = Convert.ToInt32((term[0]).Trim());

            string sql = @"Select ExamsId, Exam_name from Exams where
                SchoolId = @Id and TermId = @termId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@termId", termId);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpExam.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
            }

            dr.Close();
        }
        catch (Exception ex)
        {

            lblError.Text = "Load Exam error" + ex;
        }
        finally
        {
            //
        }
    }

    private void LoadClass()
    {
        drpClass.Items.Clear();
        drpClass.Items.Add("");

        string query = @"Select [ClassID], [ClassName] from Classes where SchoolId = @schoolId";
        SqlCommand command = new SqlCommand(query, appconSQL2);
        command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            drpClass.Items.Add(reader.GetInt32(0).ToString() + "|" + reader.GetString(1));
        }
        reader.Close();
    }

 

    protected void drpClass_SelectedIndexChanged(object sender, EventArgs e)
    {
        string[] year = drpAcademicYear.Text.Split('|');
        int yearId = int.Parse(year[0]);

        string[] term = drpTerm.Text.Split('|');
        int termId = int.Parse(term[0]);


        string[] exam = drpExam.Text.Split('|');
        int examId = int.Parse(exam[0]);


        string[] selectedClass = drpClass.Text.Split('|');
        int classId = int.Parse(selectedClass[0]);

        Session["exam"] = examId;
        Session["classId"] = classId;

        Response.Redirect("ExamCheck.aspx");
    }
}