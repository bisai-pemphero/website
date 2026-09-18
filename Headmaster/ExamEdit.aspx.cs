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

            string id = Request.QueryString["id"];
            LoadExam(id);

            lblExamId.Text = id;

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

    public void LoadExam(string examID)
    {
        try
        {
            string sql = @"SELECT [ExamsId]
                  ,[Exam_name]
                  ,[Exam_start]
                  ,[Exam_End]
                  FROM [Exams] 
                  WHERE [ExamsId] = @examId AND [SchoolId] = @schoolId";

            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@examId", examID);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                txtExamName.Text = dr["Exam_name"].ToString();
                txtStart.Text = Convert.ToDateTime(dr["Exam_start"]).ToString("yyyy-MM-dd");
                txtEnddate.Text = Convert.ToDateTime(dr["Exam_End"]).ToString("yyyy-MM-dd");
            }
            else
            {
                lblError.Text = "Exam not found or you don't have permission to access it.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading exam error: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
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
        try
        {
            string[] academicYear = drpAcademicYear.Text.Split('|');
            int academicYearId = Convert.ToInt32(academicYear[0]);

            string[] schoolTerm = drpTerm.Text.Split('|');
            int termId = Convert.ToInt32(schoolTerm[0]);

            string sql = @"UPDATE Exams SET 
            [Exam_name] = @name, 
            [Exam_start] = @start, 
            [Exam_End] = @end, 
            [AcademicYear] = @year, 
            [TermId] = @term
            WHERE [ExamsId] = @examId AND [SchoolId] = @schoolId";

            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@name", txtExamName.Text.Trim());
            cmd.Parameters.AddWithValue("@start", txtStart.Text.Trim());
            cmd.Parameters.AddWithValue("@end", txtEnddate.Text.Trim());
            cmd.Parameters.AddWithValue("@year", academicYearId);
            cmd.Parameters.AddWithValue("@term", termId);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
    
            cmd.Parameters.AddWithValue("@examId", lblExamId.Text.Trim());

            int rowsAffected = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (rowsAffected > 0)
            {
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Exam has been updated Successfully!')", true);
            }
            else
            {
                lblError.Text = "Failed to update exam. Please try again.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }

    public void Reset()
    {
        txtStart.Text = string.Empty;
        txtEnddate.Text = string.Empty;
        txtExamName.Text = string.Empty;
        drpTerm.SelectedIndex = -1;
        drpAcademicYear.SelectedIndex = -1;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Exams.aspx");
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

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            
            string sql = @"Delete from Exams WHERE [ExamsId] = @examId AND [SchoolId] = @schoolId";

            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@examId", lblExamId.Text.Trim());
            int rowsAffected = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (rowsAffected > 0)
            {
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Exam has been Deleted Successfully!')", true);
            }
            else
            {
                lblError.Text = "Failed to Delete exam. Please try again.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Deleting error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }
}