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

public partial class ViewStudent : System.Web.UI.Page
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

            LoadEnglish();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "Select Fullname, SchoolId from Users where Username = @username";
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

    public void LoadEnglish()
    {
        try
        {
            string classId = Session["classId"].ToString() ?? "0";

            string sql = "Select [SubjectId] from [Subjects] where ClassId = @classId and SubjectName = @subject";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@classId", classId);
            cmd.Parameters.AddWithValue("@subject", "English");
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblParaId.Text = dr.GetInt32(0).ToString();
            }
            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading English error" + ex;
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


    public void LoadResults()
    {
        //string subjectId = Session["subject"].ToString() ?? "0";
        string examId = Session["exam"].ToString() ?? "0";
        string classId = Session["classId"].ToString() ?? "0";
        string schoolId = lblSchoolId.Text.Trim();

        string sql3 = @" Select * from ExamMarked where ExamId = @examId 
        and ClassId = @classId and SchoolId = @schoolId";
        SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
        cmd3.Parameters.AddWithValue("@examId", examId);
        cmd3.Parameters.AddWithValue("@classId", classId);
        cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
        SqlDataReader dr3 = cmd3.ExecuteReader();
        if (dr3.HasRows)
        {
            try
            {


                int englishId = Convert.ToInt32(lblParaId.Text.Trim());

                string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(@"

WITH SubjectGrades AS (
    SELECT
        s.StudentID,
        s.FirstName,
        s.Middlename,
        s.LastName,
        s.CurrentClassID AS ClassID,
        c.Level,
        g.SubjectId,
        g.Marks,
        CAST(gs.Grade AS INT) AS SubjectGrade,       -- 1 to 9
        gs.Remark AS SubjectRemark,

        -- Grade itself is the POINT
        gs.Grade AS SubjectPoints,

        -- English pass condition
        CASE 
            WHEN g.SubjectId = @EnglishSubjectId AND g.Marks >= 40 THEN 1
            ELSE 0
        END AS EnglishPassed

    FROM Grades g
    JOIN Students s ON s.StudentID = g.StudentId
    JOIN Classes c ON c.ClassID = s.CurrentClassID
    JOIN GradingSystem gs
        ON g.Marks BETWEEN gs.Minimum_Mark AND gs.Maxmum_Mark
        AND gs.Level = c.Level
        AND gs.SchoolId = @SchoolId
    WHERE g.ExamId = @ExamId
      AND g.SchoolId = @SchoolId
      AND s.SchoolId = @SchoolId
      AND c.SchoolId = @SchoolId
      AND s.CurrentClassID = @ClassId
      AND s.Status = 'Active'
      AND s.IsDeleted = 0
),

English AS (
    SELECT *
    FROM SubjectGrades
    WHERE SubjectId = @EnglishSubjectId
),

OtherSubjects AS (
    SELECT *,
           ROW_NUMBER() OVER (
               PARTITION BY StudentID
               ORDER BY Marks DESC
           ) AS rn
    FROM SubjectGrades
    WHERE SubjectId <> @EnglishSubjectId
),

BestFiveOthers AS (
    SELECT *
    FROM OtherSubjects
    WHERE rn <= 5
),

BestSix AS (
    -- English (mandatory)
    SELECT
        StudentID,
        ClassID,
        Level,
        SubjectId,
        Marks,
        SubjectGrade,
        SubjectRemark,
        CAST(SubjectPoints AS INT) AS SubjectPoints ,
        EnglishPassed
    FROM English

    UNION ALL

    -- Best 5 other subjects
    SELECT
        StudentID,
        ClassID,
        Level,
        SubjectId,
        Marks,
        SubjectGrade,
        SubjectRemark,
        SubjectPoints,
        0 AS EnglishPassed
    FROM BestFiveOthers
),

Totals AS (
    SELECT
        b.StudentID,
        b.ClassID,
        b.Level,
        COUNT(*) AS SubjectsCount,
        SUM(b.Marks) AS TotalMarks,
        SUM(b.SubjectPoints) AS TotalPoints,      -- ⭐ BEST 6 POINTS
        (SUM(b.Marks) * 100.0 / 600) AS Percentage,
        MAX(b.EnglishPassed) AS PassedEnglish,

        CASE 
            WHEN MAX(b.EnglishPassed) = 1 AND SUM(b.Marks) >= 240 THEN 1
            ELSE 0
        END AS OverallPass,

        MAX(CASE WHEN b.SubjectId = @EnglishSubjectId THEN b.Marks END) AS EnglishMarks,
        MAX(CASE WHEN b.SubjectId = @EnglishSubjectId THEN b.SubjectGrade END) AS EnglishGrade

    FROM BestSix b
    GROUP BY b.StudentID, b.ClassID, b.Level
    HAVING COUNT(*) = 6
)

SELECT
    t.StudentID,
    s.FirstName,
    s.Middlename,
    s.LastName,
    c.ClassName,

    t.TotalMarks,
    t.TotalPoints AS PointsObtained,

    CASE 
        WHEN t.EnglishMarks >= 40 THEN 'PASSED'
        ELSE 'FAILED'
    END AS EnglishResult,

    -- Final grade based on overall percentage
    gs.Grade AS FinalGrade,
    gs.Remark AS FinalRemark,

    -- Rank PASSED students by LOWEST points
    CASE 
        WHEN t.OverallPass = 1 THEN
            DENSE_RANK() OVER (
                PARTITION BY t.ClassID
                ORDER BY t.TotalPoints ASC
            )
        ELSE NULL
    END AS PositionInPassedStudents,

    -- Overall class rank (all students)
    DENSE_RANK() OVER (
        PARTITION BY t.ClassID
        ORDER BY t.TotalPoints ASC
    ) AS ClassRank,

    CASE 
        WHEN t.OverallPass = 1 THEN 'PASS'
        ELSE 'FAIL'
    END AS FinalResult

FROM Totals t
JOIN Students s ON s.StudentID = t.StudentID
JOIN Classes c ON c.ClassID = t.ClassID
JOIN GradingSystem gs
    ON t.Percentage BETWEEN gs.Minimum_Mark AND gs.Maxmum_Mark
    AND gs.Level = t.Level
    AND gs.SchoolId = @SchoolId

ORDER BY t.TotalPoints ASC;



", connection);

                    command.Parameters.AddWithValue("@ExamId", examId);
                    command.Parameters.AddWithValue("@classId", classId);
                    command.Parameters.AddWithValue("@EnglishSubjectId", englishId);
                    command.Parameters.AddWithValue("@schoolId", schoolId);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string Id = reader["StudentID"].ToString();

                        Response.Write("<tr>");
                        Response.Write("<td>" + reader["PositionInPassedStudents"] + "</td>");
                        Response.Write("<td>" + reader["FirstName"] + "</td>");
                        Response.Write("<td>" + reader["Middlename"] + "</td>");
                        Response.Write("<td>" + reader["LastName"] + "</td>");

                        Response.Write("<td>" + reader["ClassName"] + "</td>");
                        Response.Write("<td>" + reader["TotalMarks"] + "</td>");
                      
                        Response.Write("<td>" + reader["EnglishResult"] + "</td>");
                       

                        Response.Write("<td>" + reader["FinalRemark"] + "</td>");
                      
                        Response.Write("<td>" + reader["PointsObtained"] + "</td>");
                        Response.Write("<td>" + reader["FinalResult"] + "</td>");

                        Response.Write("<td>");
                        Response.Write(
             "<a href='ReportCard2.aspx?studentId=" + Id +
             "&examId=" + examId +
             "&schoolId=" + schoolId +
             
             "' class='btn'>"
         );

                        Response.Write("<i class='fas fa-eye'></i></a>");

                        Response.Write("</td>");
                        Response.Write("</tr>");
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<tr><td colspan='6' style='color: red; text-align: center;'>Error loading data: " + HttpUtility.HtmlEncode(ex.Message) + "</td></tr>");
            }
            return;
        }
        else
        {
            dr3.Close();
            dr3.Dispose();

            Response.Write("<tr><td colspan='6' style='color: red; text-align: center;'> Exam Results Not ready "  + "</td></tr>");
        }


            
        
    }



   
}