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

    private void readConf()
    {
        System.IO.StreamReader sr;
        {
            sr = System.IO.File.OpenText(Server.MapPath("dbconn.ini"));

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
            sr.Close();
        }
    }

    private SqlConnection CreateConnection()
    {
        // Add MultipleActiveResultSets=true to allow multiple active result sets
        appconStr = "Data Source=" + server + ";user id=" + user + ";password=" + password +
                   ";max pool size=65536;MultipleActiveResultSets=true;Initial Catalog=" + appdb + ";";
        return new SqlConnection(appconStr);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        readConf();

        if (Session["StudentID"] == null && Session["ExamsId"] == null)
        {
            Response.Redirect("examPortal.aspx");
            return;
        }


        if (!IsPostBack)
        {

            LoadSchoolDetails();
            LoadStudentDetails();
            LoadRemarks();
        }
    }

    public void LoadSchoolDetails()
    {
        SqlConnection connection = null;
        SqlDataReader dr0 = null, dr1 = null, dr2 = null, dr5 = null, dr6 = null;

        try
        {
           
            string studentId = Session["StudentID"].ToString();
            string examId = Session["ExamsId"].ToString();

            connection = CreateConnection();
            connection.Open();

            // School details
            string Logoname = "";
            string sql0 = @" select A.[School_name], A.[PhoneNumber], A.[Slogan], A.[Logo], A.[School_address], 
            A.[School_email], A.SchoolId from AllSchools as A
            Join Students as S 
            On S.SchoolId = A.SchoolId 
            Where S.StudentID = @studentId";
            SqlCommand cmd0 = new SqlCommand(sql0, connection);
            cmd0.Parameters.AddWithValue("@studentId", studentId);
            dr0 = cmd0.ExecuteReader();
            if (dr0.Read())
            {
                DateTime thisYear = DateTime.Now;
                lblSchoolName.InnerText = dr0.GetString(0);
                lblContact.InnerText = dr0.GetString(1);
                lblSchoolMoto.InnerText = dr0.GetString(2);
                Logoname = dr0.GetString(3);
                lblAddress.InnerText = dr0.GetString(4);
                lblEmail.InnerText = dr0.GetString(5);
                lblfooterSchoolName.InnerText = "©  " + thisYear.Year + " " + dr0.GetString(0) + " All rights reserved.";
                lblSchoolId.Text = dr0.GetInt32(6).ToString();
            }
            dr0.Close();

            imgLogo.Src = "~/img/Schooldocs/" + Logoname;

            // Student's current class
            string sql2 = @"Select [CurrentClassID] from Students where StudentID = @Id";
            SqlCommand cmd2 = new SqlCommand(sql2, connection);
            cmd2.Parameters.AddWithValue("@Id", studentId);
            dr2 = cmd2.ExecuteReader();
            if (dr2.Read())
            {
                lblClassId.Text = dr2.GetInt32(0).ToString();
            }
            dr2.Close();

            // English subject ID
            string sql1 = @"Select [SubjectId] from [Subjects] where
            ClassId = @classId and SubjectName = @subject";
            SqlCommand cmd1 = new SqlCommand(sql1, connection);
            cmd1.Parameters.AddWithValue("@classId", lblClassId.Text.Trim());
            cmd1.Parameters.AddWithValue("@subject", "English");
            dr1 = cmd1.ExecuteReader();
            if (dr1.Read())
            {
                lblEnglishId.Text = dr1.GetInt32(0).ToString();
            }
            dr1.Close();

            // Academic year and term
            string sql5 = @"Select CONCAT(DATENAME(YEAR, A.[Start_year]), '-', YEAR(A.[End_year])) as AcademicYear, T.TermName 
       from Exams as E Join Academic_Year as A on E.AcademicYear = A.AcademicyearId 
       Join SchoolTerm as T on E.TermId = T.TermId
       where E.ExamsId = @examId";
            SqlCommand cmd5 = new SqlCommand(sql5, connection);
            cmd5.Parameters.AddWithValue("@examId", examId);
            dr5 = cmd5.ExecuteReader();
            if (dr5.Read())
            {
                lblTerm.InnerText = dr5.GetString(1) + " (" + dr5.GetString(0) + ")";
            }
            dr5.Close();

            // Total students in class
            string sql6 = @"Select Count(StudentID) from Students where 
                CurrentClassID = @classId
                and Status = 'Active' and IsDeleted = 0";
            SqlCommand cmd6 = new SqlCommand(sql6, connection);
            cmd6.Parameters.AddWithValue("@classId", lblClassId.Text.Trim());
            dr6 = cmd6.ExecuteReader();
            if (dr6.Read())
            {
                lblTotal.Text = dr6.GetInt32(0).ToString();
            }
            dr6.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading details error: " + ex.Message;
        }
        finally
        {
            // Ensure all readers are closed
            if (dr0 != null && !dr0.IsClosed) dr0.Close();
            if (dr1 != null && !dr1.IsClosed) dr1.Close();
            if (dr2 != null && !dr2.IsClosed) dr2.Close();
            if (dr5 != null && !dr5.IsClosed) dr5.Close();
            if (dr6 != null && !dr6.IsClosed) dr6.Close();

            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public void LoadRemarks()
    {
        SqlConnection connection = null;
        SqlDataReader dr7 = null, dr10 = null;

        try
        {
            string schoolId = lblSchoolId.Text.Trim();
            string studentId = Session["StudentID"].ToString();
            string examId = Session["ExamsId"].ToString();

            connection = CreateConnection();
            connection.Open();

            // Teacher's remarks
            string sql7 = @"
            Select TR.[Remark], TR.DateCreated, U.Fullname from [ExamRemarksTeacher] as TR
            Join Users as U on U.Username = TR.Postedby
            Where TR.ExamId = @examId and TR.StudentId = @studentId";
            SqlCommand cmd7 = new SqlCommand(sql7, connection);
            cmd7.Parameters.AddWithValue("@examId", examId);
            cmd7.Parameters.AddWithValue("@studentId", studentId);
            dr7 = cmd7.ExecuteReader();
            if (dr7.Read())
            {
                lblteachersRemarks.InnerText = dr7.GetString(0);
                lblDate.InnerText = dr7.GetDateTime(1).ToString("yyyy-MM-dd");
                lblTeacherSignature.InnerText = dr7.GetString(2);
            }
            dr7.Close();

            // Head teacher's remarks
            string sql10 = @"Select TR.[Remark], TR.DateCreated, U.Fullname from ExamRemarksHeadTeacher as TR
            Join Users as U on U.Username = TR.Postedby
            Where TR.ExamId = @examId and TR.StudentId = @studentId";
            SqlCommand cmd10 = new SqlCommand(sql10, connection);
            cmd10.Parameters.AddWithValue("@examId", examId);
            cmd10.Parameters.AddWithValue("@studentId", studentId);
            dr10 = cmd10.ExecuteReader();
            if (dr10.Read())
            {
                lblheadteachersRemarks.InnerText = dr10.GetString(0);
                lblDate2.InnerText = dr10.GetDateTime(1).ToString("yyyy-MM-dd");
                lblheadteacherSignature.InnerText = dr10.GetString(2);
            }
            dr10.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading remarks error: " + ex.Message;
        }
        finally
        {
            if (dr7 != null && !dr7.IsClosed) dr7.Close();
            if (dr10 != null && !dr10.IsClosed) dr10.Close();

            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public void LoadStudentDetails()
    {
        SqlConnection connection = null;
        SqlDataReader dr0 = null;

        try
        {
            string schoolId = lblSchoolId.Text.Trim();
            string studentId = Session["StudentID"].ToString();
            string examId = Session["ExamsId"].ToString();

            string studentClassId = lblClassId.Text.Trim();
            string subjectId = lblEnglishId.Text.Trim();

            connection = CreateConnection();
            connection.Open();

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
        CAST(gs.Grade AS INT) AS SubjectPoints,
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
    SELECT
        StudentID,
        ClassID,
        Level,
        SubjectId,
        Marks,
        SubjectGrade,
        SubjectRemark,
        SubjectPoints,
        EnglishPassed
    FROM English
    UNION ALL
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
        SUM(b.SubjectPoints) AS TotalPoints,
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
            WHEN t.EnglishMarks >= 40 THEN 'PASSED'
            ELSE 'FAILED'
        END AS EnglishResult,
        gs.Grade AS FinalGrade,
        gs.Remark AS FinalRemark,
        CASE 
            WHEN t.OverallPass = 1 THEN
                DENSE_RANK() OVER (
                    PARTITION BY t.ClassID
                    ORDER BY t.TotalPoints ASC
                )
        END AS PositionInPassedStudents,
        DENSE_RANK() OVER (
            PARTITION BY t.ClassID
            ORDER BY t.TotalPoints ASC
        ) AS ClassRank,
        CASE 
            WHEN t.OverallPass = 1 THEN 'PASS'
            ELSE 'FAIL'
        END AS FinalResult,
        t.TotalPoints AS PointsObtained
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
WHERE StudentID = @StudentId";

            SqlCommand cmd0 = new SqlCommand(sql0, connection);
            cmd0.Parameters.AddWithValue("@EnglishSubjectId", subjectId);
            cmd0.Parameters.AddWithValue("@ExamId", examId);
            cmd0.Parameters.AddWithValue("@SchoolId", schoolId);
            cmd0.Parameters.AddWithValue("@classId", studentClassId);
            cmd0.Parameters.AddWithValue("@StudentId", studentId);

            dr0 = cmd0.ExecuteReader();
            if (dr0.Read())
            {
                lblstudentName.InnerHtml = dr0.GetString(1) + " " + dr0.GetString(2) + " " + dr0.GetString(3);
                lblClassName.InnerHtml = dr0.GetString(4);

                lblPosition.InnerText = dr0.GetInt64(12).ToString() + " / " + lblTotal.Text.Trim();
                lblRemarks.InnerText = dr0.GetString(13);
                lblPoints.InnerText = dr0.GetInt32(14).ToString();
            }
            dr0.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Loading student details error: " + ex.Message;
        }
        finally
        {
            if (dr0 != null && !dr0.IsClosed) dr0.Close();

            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("examPortal.aspx");
    }

    public void LoadMarks()
    {
        string schoolId = lblSchoolId.Text.Trim();
        string studentId = Session["StudentID"].ToString();
        string examId = Session["ExamsId"].ToString();

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            SqlCommand command = new SqlCommand(@"SELECT  
    S.SubjectName,
    G.Marks,
    GS.Grade,
    GS.Remark
FROM Grades AS G
INNER JOIN Students AS ST 
    ON G.StudentId = ST.StudentId
INNER JOIN Subjects AS S 
    ON S.SubjectId = G.SubjectId
INNER JOIN Classes AS C 
    ON ST.CurrentClassID = C.ClassID
INNER JOIN GradingSystem AS GS 
    ON GS.Level = C.Level
   AND G.Marks BETWEEN GS.Minimum_Mark AND GS.Maxmum_Mark
WHERE G.ExamId = @examId
  AND G.StudentId = @studentId
  Order by s.SubjectName ASC", connection);
            command.Parameters.AddWithValue("@examId", examId);
            command.Parameters.AddWithValue("@studentId", studentId);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Response.Write("<tr>");
                    Response.Write("<td>" + reader["SubjectName"] + "</td>");
                    Response.Write("<td>" + reader["Marks"] + "</td>");
                    Response.Write("<td>" + reader["Grade"] + "</td>");
                    Response.Write("<td>" + reader["Remark"] + "</td>");
                    Response.Write("</tr>");
                }
            }
        }
    }

    public void LoadGradingSystem()
    {
        string classId = lblClassId.Text.Trim();

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            SqlCommand command = new SqlCommand(@"Select G.Grade,
         G.[Minimum_Mark],
         G.[Maxmum_Mark],
         G.[Remark] 
         from GradingSystem as G
  Join Classes as C on C.Level = G.Level
  Where C.ClassID = @classId", connection);
            command.Parameters.AddWithValue("@classId", classId);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Response.Write("<tr>");
                    Response.Write("<td>" + reader["Grade"] + "</td>");
                    Response.Write("<td>" + reader["Minimum_Mark"] + "</td>");
                    Response.Write("<td>" + reader["Maxmum_Mark"] + "</td>");
                    Response.Write("<td>" + reader["Remark"] + "</td>");
                    Response.Write("</tr>");
                }
            }
        }
    }
}