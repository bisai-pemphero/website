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

using System.Threading.Tasks;

public partial class ReportCard : System.Web.UI.Page
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

        LoadSchoolDetails();
        LoadStudentDetails();
        LoadRemarks();
    }

    public void LoadSchoolDetails()
    {
        try
        {
            string schoolId = Request.QueryString["schoolId"];
            string studentId = Request.QueryString["studentId"];
            string examId = Request.QueryString["examId"]; 

          
            string Logoname = "";
            string sql0 = @"select [School_name],[PhoneNumber],[Slogan], [Logo], [School_address], [School_email] from AllSchools 
                where [SchoolId] = @Id";
            SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
            cmd0.Parameters.AddWithValue("@Id", schoolId);
            SqlDataReader dr0 = cmd0.ExecuteReader();
            while (dr0.Read())
            {
                DateTime thisYear = DateTime.Now;
                lblSchoolName.InnerText = dr0.GetString(0);
                lblContact.InnerText = dr0.GetString(1);
                lblSchoolMoto.InnerText = dr0.GetString(2);
                Logoname = dr0.GetString(3);
                lblAddress.InnerText = dr0.GetString(4);
                lblEmail.InnerText = dr0.GetString(5);
                lblfooterSchoolName.InnerText = "©  " + thisYear.Year  + " " + dr0.GetString(0) + " All rights reserved.";
            }
            dr0.Close();

            imgLogo.Src = "~/img/Schooldocs/" + Logoname;


            string sql2 = @"Select [CurrentClassID] from Students where StudentID = @Id";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            cmd2.Parameters.AddWithValue("@Id", studentId);
            SqlDataReader dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                lblClassId.Text =  dr2.GetInt32(0).ToString();
            }
            dr2.Close();

            string sql1 = @"Select [SubjectId] from [Subjects] where
            ClassId = @classId and SubjectName = @subject";
            SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
            cmd1.Parameters.AddWithValue("@classId", lblClassId.Text.Trim());
            cmd1.Parameters.AddWithValue("@subject", "English");
            SqlDataReader dr1 = cmd1.ExecuteReader();
            while (dr1.Read())
            {
               lblEnglishId.Text = dr1.GetInt32(0).ToString();
            }
            dr1.Close();

            string sql3 = @"Select CONCAT(DATENAME(YEAR, A.[Start_year]), '-', YEAR(A.[End_year])) as AcademicYear, T.TermName 
       from Exams as E Join Academic_Year as A on E.AcademicYear = A.AcademicyearId 
       Join SchoolTerm as T on E.TermId = T.TermId
       where E.ExamsId = @examId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@examId", examId);
            
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                lblAcademicYear.InnerText = dr3.GetString(0);
                lblTerm.InnerText = dr3.GetString(1);
            }
            dr3.Close();

            string sql4 = @"Select Count(StudentID) from Students where 
                CurrentClassID = @classId
                and Status = 'Active' and IsDeleted = 0";
            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            cmd4.Parameters.AddWithValue("@classId", lblClassId.Text.Trim());

            SqlDataReader dr4 = cmd4.ExecuteReader();
            while (dr4.Read())
            {
                lblTotal.Text = dr4.GetInt32(0).ToString();
            }
            dr4.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading details error" + ex.Message;
        }
    }


    public void LoadRemarks()
    {
        try
        {
            string schoolId = Request.QueryString["schoolId"];
            string studentId = Request.QueryString["studentId"];
            string examId = Request.QueryString["examId"];

            string sql3 = @"
            Select TR.[Remark], TR.DateCreated, U.Fullname from [ExamRemarksTeacher] as TR
            Join Users as U on U.Username = TR.Postedby
            Where TR.ExamId = @examId and TR.StudentId = @studentId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@examId", examId);
            cmd3.Parameters.AddWithValue("@studentId", studentId);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                lblteachersRemarks.InnerText = dr3.GetString(0);
                lblDate.InnerText = dr3.GetDateTime(1).ToString();
                lblTeacherSignature.InnerText = dr3.GetString(2);
            }
            dr3.Close();

            string sql4 = @"Select TR.[Remark], TR.DateCreated, U.Fullname from ExamRemarksHeadTeacher as TR
            Join Users as U on U.Username = TR.Postedby
            Where TR.ExamId = @examId and TR.StudentId = @studentId";
            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            cmd4.Parameters.AddWithValue("@examId", examId);
            cmd4.Parameters.AddWithValue("@studentId", studentId);
            SqlDataReader dr4 = cmd4.ExecuteReader();
            while (dr4.Read())
            {
                lblheadteachersRemarks.InnerText = dr4.GetString(0);
                lblDate2.InnerText = dr4.GetDateTime(1).ToString();
                lblheadteacherSignature.InnerText = dr4.GetString(2);
            }
            dr4.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading remarks error" + ex.Message;
        }
    }

    public void LoadStudentDetails()
    {
        try
        {
            string schoolId = Request.QueryString["schoolId"];
            string studentId = Request.QueryString["studentId"];
            string examId = Request.QueryString["examId"];

            string studentClassId = lblClassId.Text.Trim();
            string subjectId = lblEnglishId.Text.Trim();

            string sql0 = @"
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
        gs.Grade AS SubjectGrade,
        gs.Remark AS SubjectRemark,
        CASE 
            WHEN g.SubjectId = @EnglishSubjectId AND g.Marks > 40 THEN 1
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
      AND s.CurrentClassID = @classId
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
    SELECT StudentID, ClassID, Level, Marks,
           EnglishPassed, SubjectId, SubjectGrade, SubjectRemark
    FROM English
    UNION ALL
    SELECT StudentID, ClassID, Level, Marks,
           0 AS EnglishPassed, SubjectId, SubjectGrade, SubjectRemark
    FROM BestFiveOthers
),

Totals AS (
    SELECT
        b.StudentID,
        b.ClassID,
        b.Level,
        COUNT(b.SubjectId) AS SubjectsCount,
        SUM(b.Marks) AS TotalMarks,
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
    HAVING COUNT(b.SubjectId) = 6
),

FinalResults AS (
    SELECT
        t.StudentID,
		
        s.FirstName,
        s.Middlename,
        s.LastName,
        c.ClassName,
        t.Level,
        t.TotalMarks,
        t.Percentage,

        CASE 
            WHEN t.EnglishMarks > 40 THEN 'PASSED'
            ELSE 'FAILED'
        END AS EnglishResult,

        gs.Grade AS FinalGrade,
        gs.Remark AS FinalRemark,

        CASE 
            WHEN t.OverallPass = 1 THEN
                DENSE_RANK() OVER (
                    PARTITION BY t.ClassID
                    ORDER BY t.TotalMarks DESC
                )
        END AS PositionInPassedStudents,

        DENSE_RANK() OVER (
            PARTITION BY t.ClassID
            ORDER BY t.TotalMarks DESC
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
)

SELECT *
FROM FinalResults
WHERE StudentID = @StudentId
ORDER BY TotalMarks DESC
";
            SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
            cmd0.Parameters.AddWithValue("@EnglishSubjectId", subjectId);
            cmd0.Parameters.AddWithValue("@ExamId", examId);
            cmd0.Parameters.AddWithValue("@SchoolId", schoolId);
            cmd0.Parameters.AddWithValue("@classId", studentClassId);
            cmd0.Parameters.AddWithValue("@StudentId", studentId);
            SqlDataReader dr0 = cmd0.ExecuteReader();
            while (dr0.Read())
            {
                lblstudentName.InnerHtml = dr0.GetString(1) + " " + dr0.GetString(2) + " " + dr0.GetString(3);
                lblClassName.InnerHtml = dr0.GetString(4);
                lblRemarks.InnerText = dr0.GetString(13);
                lblPosition.InnerText = dr0.GetInt64(12).ToString() + " / " + lblTotal.Text.Trim();

            }
            dr0.Close();

          
        }
        catch (Exception ex)
        {
            lblError.Text = "Receipt profile error" + ex;
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("ExamResults.aspx");
    }


    public void LoadMarks()
    {
        string studentId = Request.QueryString["studentId"];
        string examId = Request.QueryString["examId"];
        string studentClassId = lblClassId.Text.Trim();

        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(@"
SELECT 
    S.SubjectId,
    S.SubjectName,
    G.Marks,
    GS.Grade,
    GS.Remark
FROM Grades AS G
INNER JOIN Subjects AS S 
    ON S.SubjectId = G.SubjectId
INNER JOIN Classes AS C  
    ON S.ClassId = C.ClassID
INNER JOIN GradingSystem AS GS 
    ON GS.Level = C.Level
    AND GS.SchoolId = C.SchoolId
    AND G.Marks BETWEEN GS.Minimum_Mark AND GS.Maxmum_Mark
WHERE G.ExamId =	@examId
  AND G.StudentId = @studentId
  AND C.ClassID =   @classId
  Order by s.SubjectName ASC
", connection);
            command.Parameters.AddWithValue("@examId", examId);
            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@classId", studentClassId);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
               
                Response.Write("<tr>");
                Response.Write("<td>" + reader["SubjectName"] + "</td>");
                Response.Write("<td>" + reader["Marks"] + "</td>");
                Response.Write("<td>" + reader["Grade"] + "</td>");
                Response.Write("<td>" + reader["Remark"] + "</td>");
             
                Response.Write("</tr>");
            }

            reader.Close();
        }
    }

    public void LoadGradingSystem()
    {
        string classId = lblClassId.Text.Trim();

        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(@"
SELECT 
    G.Grade,
    G.Minimum_Mark,
    G.Maxmum_Mark,
    G.Remark
FROM GradingSystem AS G
WHERE G.Level = (
    SELECT Level 
    FROM Classes 
    WHERE ClassID = @classId
)
AND G.SchoolId = (
    SELECT SchoolId 
    FROM Classes 
    WHERE ClassID = @classId
)
ORDER BY G.Minimum_Mark DESC
", connection);
            command.Parameters.AddWithValue("@classId", classId);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Response.Write("<tr>");
                Response.Write("<td>" + reader["Grade"] + "</td>");
                Response.Write("<td>" + reader["Minimum_Mark"] + "</td>");
                Response.Write("<td>" + reader["Maxmum_Mark"] + "</td>");
                Response.Write("<td>" + reader["Remark"] + "</td>");

                Response.Write("</tr>");
            }

            reader.Close();
        }
    }
}