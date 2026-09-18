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
using System.Web.Services;
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
        string examId = Session["exam"].ToString() ?? "0";
        string classId = Session["classId"].ToString() ?? "0";
        string schoolId = lblSchoolId.Text.Trim();
        string currentUser = lblSession.Text.Trim(); // Get current username

        // First check if exam is marked
        string sql3 = @"Select * from ExamMarked where ExamId = @examId 
    and ClassId = @classId and SchoolId = @schoolId";
        SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
        cmd3.Parameters.AddWithValue("@examId", examId);
        cmd3.Parameters.AddWithValue("@classId", classId);
        cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

        bool examIsMarked = false;
        using (SqlDataReader dr3 = cmd3.ExecuteReader())
        {
            examIsMarked = dr3.HasRows;
        } // DataReader is properly closed here

        if (examIsMarked)
        {
            try
            {
                int englishId = Convert.ToInt32(lblParaId.Text.Trim());

                // Get all teacher remarks for this exam to display
                Dictionary<int, string> teacherRemarks = new Dictionary<int, string>();

                // Need a new connection for the second query since we're still using appconSQL2
                using (SqlConnection remarksConnection = new SqlConnection(appconStr))
                {
                    remarksConnection.Open();

                    string remarksQuery = @"
                SELECT StudentId, Remark 
                FROM ExamRemarksTeacher 
                WHERE ExamId = @examId 
                  AND SchoolId = @schoolId";

                    using (SqlCommand remarksCmd = new SqlCommand(remarksQuery, remarksConnection))
                    {
                        remarksCmd.Parameters.AddWithValue("@examId", examId);
                        remarksCmd.Parameters.AddWithValue("@schoolId", schoolId);

                        using (SqlDataReader remarksReader = remarksCmd.ExecuteReader())
                        {
                            while (remarksReader.Read())
                            {
                                int studentId = remarksReader.GetInt32(0);
                                string remark = remarksReader.IsDBNull(1) ? "" : remarksReader["Remark"].ToString();
                                teacherRemarks[studentId] = remark;
                            }
                        }
                    }
                }

                // Get the exam results - need another connection
                using (SqlConnection resultsConnection = new SqlConnection(appconStr))
                {
                    resultsConnection.Open();

                    string query = @"
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
        -- Check if student passed English (>40 marks)
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
        -- Check overall pass condition: English >40 AND Total >=240
        CASE 
            WHEN MAX(b.EnglishPassed) = 1 AND SUM(b.Marks) >= 240 THEN 1
            ELSE 0
        END AS OverallPass,
        -- Get English marks separately for display
        MAX(CASE WHEN b.SubjectId = @EnglishSubjectId THEN b.Marks ELSE NULL END) AS EnglishMarks,
        MAX(CASE WHEN b.SubjectId = @EnglishSubjectId THEN b.SubjectGrade ELSE NULL END) AS EnglishGrade
    FROM BestSix b
    GROUP BY b.StudentID, b.ClassID, b.Level
    HAVING COUNT(b.SubjectId) = 6 -- Must have exactly 6 subjects (English + 5 others)
)

SELECT
    t.StudentID,
    s.FirstName,
    s.Middlename,
    s.LastName,
    c.ClassName,
    t.TotalMarks,
  
    CASE 
        WHEN t.EnglishMarks > 40 THEN 'PASSED'
        ELSE 'FAILED'
    END AS EnglishResult,

    -- FINAL GRADE FROM SAME GRADING SYSTEM
    gs.Grade AS FinalGrade,
    gs.Remark AS FinalRemark,

    -- Rank only students who passed both conditions: English > 40 AND Total >= 240
    CASE 
        WHEN t.OverallPass = 1 THEN
            DENSE_RANK() OVER (
                PARTITION BY t.ClassID
                ORDER BY t.TotalMarks DESC
            )
        ELSE NULL
    END AS PositionInPassedStudents,

    -- Overall class rank including all students
    DENSE_RANK() OVER (
        PARTITION BY t.ClassID
        ORDER BY t.TotalMarks DESC
    ) AS ClassRank,

    -- Pass/Fail status
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

ORDER BY t.OverallPass ASC, t.TotalMarks DESC";

                    using (SqlCommand command = new SqlCommand(query, resultsConnection))
                    {
                        command.Parameters.AddWithValue("@ExamId", examId);
                        command.Parameters.AddWithValue("@classId", classId);
                        command.Parameters.AddWithValue("@EnglishSubjectId", englishId);
                        command.Parameters.AddWithValue("@schoolId", schoolId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Response.Write("<tr><td colspan='10' style='color: orange; text-align: center;'>No results found for this exam.</td></tr>");
                                return;
                            }

                            while (reader.Read())
                            {
                                string Id = reader["StudentID"].ToString();
                                int studentId = Convert.ToInt32(Id);
                                string fullName = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();

                                // Get teacher's remark for this student
                                string teacherRemark = teacherRemarks.ContainsKey(studentId) ? teacherRemarks[studentId] : "";

                                Response.Write("<tr data-student-id='" + studentId + "' data-student-name='" +
                                              HttpUtility.HtmlAttributeEncode(fullName) + "'>");
                                Response.Write("<td>" + reader["PositionInPassedStudents"] + "</td>");
                                Response.Write("<td>" + reader["FirstName"] + "</td>");
                                Response.Write("<td>" + reader["Middlename"] + "</td>");
                                Response.Write("<td>" + reader["LastName"] + "</td>");

                                Response.Write("<td>" + reader["TotalMarks"] + "</td>");
                                Response.Write("<td>" + reader["EnglishResult"] + "</td>");

                                Response.Write("<td>" + reader["FinalRemark"] + "</td>");
                                Response.Write("<td>" + reader["ClassRank"] + "</td>");
                                Response.Write("<td>" + reader["FinalResult"] + "</td>");

                                // TEACHER'S REMARK COLUMN - ALWAYS SHOW INPUT FIELD (NO DISPLAY MODE)
                                Response.Write("<td class='remarks-cell'>");

                                // Show input field only (always visible and editable)
                                Response.Write("<textarea class='remarks-input' id='remark-input-" + studentId + "' ");
                                Response.Write("data-student-id='" + studentId + "' ");
                                Response.Write("style='width: 100%; min-width: 200px; padding: 8px 12px; border: 1px solid #e5e7eb; border-radius: 6px; font-family: Inter, sans-serif; font-size: 14px; transition: all 0.3s ease; resize: vertical;' ");
                                Response.Write("placeholder='Enter teacher remark here...' ");
                                Response.Write("maxlength='500' ");
                                Response.Write("rows='3' ");
                                Response.Write("onfocus=\"this.style.borderColor='#0ea5e9'; this.style.boxShadow='0 0 0 3px rgba(14, 165, 233, 0.1)';\" ");
                                Response.Write("onblur=\"this.style.borderColor='#e5e7eb'; this.style.boxShadow='none';\">");
                                Response.Write(HttpUtility.HtmlEncode(teacherRemark));
                                Response.Write("</textarea>");

                                // Small text showing character count
                                Response.Write("<div style='font-size: 11px; color: #6b7280; margin-top: 4px; text-align: right;'>");
                             //   Response.Write("<span id='char-count-" + studentId + "'>" + (teacherRemark.Length ?? 0) + "</span>/500 characters");
                                Response.Write("</div>");

                                Response.Write("</td>");

                                Response.Write("</tr>");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<tr><td colspan='10' style='color: red; text-align: center;'>Error loading data: " + HttpUtility.HtmlEncode(ex.Message) + "</td></tr>");
            }
        }
        else
        {
            Response.Write("<tr><td colspan='10' style='color: red; text-align: center;'> Exam Results Not ready </td></tr>");
        }
    }

    [WebMethod]
    public static object SaveTeacherRemark(int studentId, string remark, int examId, int schoolId, string postedBy)
    {
        try
        {
            // Create instance to get connection string
            string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if remark already exists for this student and exam
                string checkQuery = @"
                SELECT COUNT(*) 
                FROM ExamRemarksTeacher 
                WHERE ExamId = @examId 
                  AND StudentId = @studentId 
                  AND SchoolId = @schoolId";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@examId", examId);
                    checkCmd.Parameters.AddWithValue("@studentId", studentId);
                    checkCmd.Parameters.AddWithValue("@schoolId", schoolId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Update existing remark
                        string updateQuery = @"
                        UPDATE ExamRemarksTeacher 
                        SET Remark = @remark, 
                            DateCreated = GETDATE(),
                            Postedby = @postedBy
                        WHERE ExamId = @examId 
                          AND StudentId = @studentId 
                          AND SchoolId = @schoolId";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@remark", string.IsNullOrEmpty(remark) ? DBNull.Value : (object)remark);
                            updateCmd.Parameters.AddWithValue("@examId", examId);
                            updateCmd.Parameters.AddWithValue("@studentId", studentId);
                            updateCmd.Parameters.AddWithValue("@schoolId", schoolId);
                            updateCmd.Parameters.AddWithValue("@postedBy", postedBy);

                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            return new
                            {
                                success = true,
                                message = "Remark updated successfully!",
                                rowsAffected = rowsAffected,
                                action = "update"
                            };
                        }
                    }
                    else
                    {
                        // Insert new remark
                        string insertQuery = @"
                        INSERT INTO ExamRemarksTeacher 
                        (ExamId, StudentId, Remark, DateCreated, Postedby, SchoolId)
                        VALUES (@examId, @studentId, @remark, GETDATE(), @postedBy, @schoolId)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@examId", examId);
                            insertCmd.Parameters.AddWithValue("@studentId", studentId);
                            insertCmd.Parameters.AddWithValue("@remark", string.IsNullOrEmpty(remark) ? DBNull.Value : (object)remark);
                            insertCmd.Parameters.AddWithValue("@postedBy", postedBy);
                            insertCmd.Parameters.AddWithValue("@schoolId", schoolId);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            return new
                            {
                                success = true,
                                message = "Remark saved successfully!",
                                rowsAffected = rowsAffected,
                                action = "insert"
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("SaveTeacherRemark Error: " + ex.Message + "\n" + ex.StackTrace);

            return new
            {
                success = false,
                message = "Error saving remark: " + ex.Message,
                errorDetails = ex.ToString()
            };
        }
    }

    [WebMethod]
    public static object GetTeacherRemark(int studentId, int examId, int schoolId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                SELECT Remark, Postedby, DateCreated 
                FROM ExamRemarksTeacher 
                WHERE ExamId = @examId 
                  AND StudentId = @studentId 
                  AND SchoolId = @schoolId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@examId", examId);
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@schoolId", schoolId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string remark = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            string postedBy = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            DateTime dateCreated = reader.GetDateTime(2);

                            return new
                            {
                                success = true,
                                remark = remark,
                                postedBy = postedBy,
                                dateCreated = dateCreated.ToString("yyyy-MM-dd HH:mm:ss"),
                                exists = true
                            };
                        }
                        else
                        {
                            return new
                            {
                                success = true,
                                remark = "",
                                postedBy = "",
                                dateCreated = "",
                                exists = false
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                message = "Error getting remark: " + ex.Message
            };
        }
    }

    

    // Helper class for remarks
    public class StudentRemark
    {
        public int StudentId { get; set; }
        public string Remark { get; set; }
    }

}