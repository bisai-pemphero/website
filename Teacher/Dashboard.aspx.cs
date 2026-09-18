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

    string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

    public string GenderLabels = "[]";
    public string GenderCounts = "[]";
    public string ClassLabels = "[]";
    public string ClassCounts = "[]";

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

            LoadDashboard();
            LoadCharts();
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
                lblUserId.Text = dr.GetInt32(2).ToString();
            }

            dr.Close();

            string sql2 = @"Select Count([StudentID]) 
            from Students where Status = 'Active' 
            and IsDeleted = 0 
            and SchoolId = @schoolId";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            cmd2.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
               
                lblStudentsTotal.Text = dr2.GetInt32(0).ToString();
            }

            dr2.Close();

            string classId = "";

            string sql3 = @"Select [ClassId] from FormTeachers where TeacherId =@teacherId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@teacherId", lblUserId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {

                classId = dr3.GetString(0);

                if (!string.IsNullOrEmpty(classId))
                {
                    finalMarkSelect.Visible = true;
                    remarkSelect.Visible = true;
                }
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

    private void LoadDashboard()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 1. Active Students
                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) FROM Students WHERE IsDeleted=0 
                AND Status='Active' AND SchoolId = @schoolId", con);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                lblStudentsTotal.Text = cmd.ExecuteScalar().ToString();

            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    private void LoadCharts()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Students by Gender
                SqlCommand cmd = new SqlCommand(@"
                    SELECT Gender, COUNT(StudentID) AS Total
                    FROM Students
                    WHERE IsDeleted=0
                    AND SchoolId = @schoolId
                    GROUP BY Gender", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                DataTable dt = new DataTable();
                da.Fill(dt);

                StringBuilder genderLabels = new StringBuilder("[");
                StringBuilder genderCounts = new StringBuilder("[");

                foreach (DataRow row in dt.Rows)
                {
                    genderLabels.Append("'" + row["Gender"].ToString() + "',");
                    genderCounts.Append(row["Total"].ToString() + ",");
                }

                if (dt.Rows.Count > 0)
                {
                    genderLabels.Length--; genderCounts.Length--;
                }

                genderLabels.Append("]");
                genderCounts.Append("]");

                GenderLabels = genderLabels.ToString();
                GenderCounts = genderCounts.ToString();

                // Students by Class
                cmd = new SqlCommand(@"
                    SELECT c.ClassName, COUNT(s.StudentID) AS Total
                    FROM Students s
                    JOIN Classes c ON s.CurrentClassID = c.ClassID
                    WHERE s.IsDeleted = 0
                    AND s.SchoolId = @schoolId
                    GROUP BY c.ClassName
                    ORDER BY c.ClassName", con);

                da = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                dt = new DataTable();
                da.Fill(dt);

                StringBuilder classLabels = new StringBuilder("[");
                StringBuilder classCounts = new StringBuilder("[");

                foreach (DataRow row in dt.Rows)
                {
                    classLabels.Append("'" + row["ClassName"].ToString() + "',");
                    classCounts.Append(row["Total"].ToString() + ",");
                }

                if (dt.Rows.Count > 0)
                {
                    classLabels.Length--; classCounts.Length--;
                }

                classLabels.Append("]");
                classCounts.Append("]");

                ClassLabels = classLabels.ToString();
                ClassCounts = classCounts.ToString();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
}