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

public partial class ExamMark : System.Web.UI.Page
{
    // Connection variables
    private string server, appdb, user, password, version;
    private SqlConnection appconSQL2;

    // Connection string property
    public string ConnectionString { get; private set; }

    private void ReadConfig()
    {
        try
        {
            string configPath = Server.MapPath("../dbconn.ini");
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Configuration file not found: " + configPath);
            }

            using (StreamReader sr = File.OpenText(configPath))
            {
                string line;
                char splitChar = '=';

                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                        continue;

                    string[] parts = line.Split(splitChar);
                    if (parts.Length < 2) continue;

                    string key = parts[0].Trim().ToLower();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "server":
                            server = value;
                            break;
                        case "user":
                            user = value;
                            break;
                        case "password":
                            password = value;
                            break;
                        case "appdb":
                            appdb = value;
                            break;
                        case "version":
                            version = value;
                            break;
                    }
                }
            }

            // Build connection string
            ConnectionString ="Data Source= "  + server + ";Initial Catalog=" + appdb +";User ID= " + user +" ;Password=" + password + ";Max Pool Size=65536;";
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading configuration: " + ex.Message, ex);
        }
    }

    private void OpenConnection()
    {
        try
        {
            if (appconSQL2 == null || appconSQL2.State != ConnectionState.Open)
            {
                ReadConfig();
                appconSQL2 = new SqlConnection(ConnectionString);
                appconSQL2.Open();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error opening database connection: " + ex.Message, ex);
        }
    }

    private void CloseConnection()
    {
        try
        {
            if (appconSQL2 != null && appconSQL2.State == ConnectionState.Open)
            {
                appconSQL2.Close();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw - this is cleanup
            System.Diagnostics.Debug.WriteLine("Error closing connection: " + ex.Message);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Check authentication
            if (Session["USER"] == null)
            {
                Response.Redirect("../CommonPages/Login.aspx");
                return;
            }

            lblSession.Text = Session["USER"].ToString();

            if (!IsPostBack)
            {
                OpenConnection();
                LoadUserInfo();

                // Debug info
              //  string debugInfo = "Session - Exam: {Session["exam"]}, Subject: {Session["subject"]}, Class: {Session["classId"]}";
              //  System.Diagnostics.Debug.WriteLine(debugInfo);

                // Set hidden field values
                hdnExamId.Value = Session["exam"].ToString() ?? "0";
                hdnSubjectId.Value = Session["subject"].ToString() ?? "0";
                hdnClassId.Value = Session["classId"].ToString() ?? "0";
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Page load error: " + ex.Message;
            lblError.CssClass = "toast-error show";
        }
        finally
        {
            // Don't close connection here as we need it for LoadExams()
        }
    }

    private void LoadUserInfo()
    {
        try
        {
            string sql = "SELECT Fullname, SchoolId, UserId FROM Users WHERE Username = @username";
            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblUser.Text = dr.GetString(0);
                        lblSchoolId.Text = dr.GetInt32(1).ToString();
                        lblTeacherId.Text = dr.GetInt32(2).ToString();

                        // Set initials
                        lblInitials.Text = GetFirstLetters(lblUser.Text.Trim());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading user info: " + ex.Message;
            lblError.CssClass = "toast-error show";
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

        return result.ToString().ToUpper();
    }

    public void LoadExams()
    {
        try
        {
            OpenConnection();

            // Get required IDs
            string examId = Session["exam"].ToString() ?? "0";
            string subjectId = Session["subject"].ToString() ?? "0";
            string classId = Session["classId"].ToString() ?? "0";
            string schoolId = lblSchoolId.Text.Trim();

            if (examId == "0" || subjectId == "0" || classId == "0")
            {
                Response.Write("<tr><td colspan='6' style='color: red; text-align: center;'>Error: Missing exam, subject, or class information. Please go back and try again.</td></tr>");
                return;
            }

            // Get existing marks if any
            Dictionary<int, decimal> existingMarks = new Dictionary<int, decimal>();

            string getMarksQuery = @"
                SELECT StudentId, Marks 
                FROM Grades 
                WHERE ExamId = @examId 
                  AND SubjectId = @subjectId 
                  AND SchoolId = @schoolId";

            using (SqlCommand marksCmd = new SqlCommand(getMarksQuery, appconSQL2))
            {
                marksCmd.Parameters.AddWithValue("@examId", examId);
                marksCmd.Parameters.AddWithValue("@subjectId", subjectId);
                marksCmd.Parameters.AddWithValue("@schoolId", schoolId);

                using (SqlDataReader marksReader = marksCmd.ExecuteReader())
                {
                    while (marksReader.Read())
                    {
                        int studentId = marksReader.GetInt32(0);
                        decimal marks = marksReader.IsDBNull(1)
                                ? 0
                                : Convert.ToDecimal(marksReader["Marks"]);

                        existingMarks[studentId] = marks;
                    }
                }
            }

            // Get students
            string studentQuery = @"
                SELECT StudentID, FirstName, Middlename, LastName 
                FROM Students 
                WHERE CurrentClassID = @classId 
                  AND SchoolId = @schoolId 
                  AND Status = 'Active'
                  AND IsDeleted = 0
                ORDER BY LastName, FirstName";

            using (SqlCommand studentCmd = new SqlCommand(studentQuery, appconSQL2))
            {
                studentCmd.Parameters.AddWithValue("@classId", classId);
                studentCmd.Parameters.AddWithValue("@schoolId", schoolId);

                using (SqlDataReader reader = studentCmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Response.Write("<tr><td colspan='6' style='color: orange; text-align: center;'>No students found in this class.</td></tr>");
                        return;
                    }

                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(0);
                        decimal existingMark = existingMarks.ContainsKey(studentId) ? existingMarks[studentId] : 0;

                        Response.Write("<tr>");
                        Response.Write("<td>" + studentId + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["FirstName"].ToString()) + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["Middlename"].ToString()) + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["LastName"].ToString()) + "</td>");

                        // Editable marks cell
                        Response.Write("<td class='editable-cell'>");
                        Response.Write("<input type='number' class='marks-input' " +
                                       "data-student-id='" + studentId + "' " +
                                       "value='" + existingMark + "' " +
                                       "min='0' max='100' step='0.01' " +
                                       "placeholder='Enter marks' />");
                        Response.Write("</td>");

                        // Status cell
                        Response.Write("<td class='status-cell'></td>");
                        Response.Write("</tr>");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("<tr><td colspan='6' style='color: red; text-align: center;'>Error loading data: " + HttpUtility.HtmlEncode(ex.Message) + "</td></tr>");
        }
        finally
        {
            CloseConnection();
        }
    }

    public string GetExamName()
    {
        try
        {
            OpenConnection();

            string examId = Session["exam"].ToString();
            if (string.IsNullOrEmpty(examId) || examId == "0")
                return "N/A";

            string sql = "SELECT Exam_name FROM Exams WHERE ExamsId = @examId";
            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                cmd.Parameters.AddWithValue("@examId", examId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "N/A";
            }
        }
        catch
        {
            return "N/A";
        }
        finally
        {
            CloseConnection();
        }
    }

    public string GetClassName()
    {
        try
        {
            OpenConnection();

            string classId = Session["classId"].ToString();
            if (string.IsNullOrEmpty(classId) || classId == "0")
                return "N/A";

            string sql = "SELECT ClassName FROM Classes WHERE ClassId = @classId";
            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                cmd.Parameters.AddWithValue("@classId", classId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "N/A";
            }
        }
        catch
        {
            return "N/A";
        }
        finally
        {
            CloseConnection();
        }
    }

    public string GetSubjectName()
    {
        try
        {
            OpenConnection();

            string subjectId = Session["subject"].ToString();
            if (string.IsNullOrEmpty(subjectId) || subjectId == "0")
                return "N/A";

            string sql = "SELECT SubjectName FROM Subjects WHERE SubjectId = @subjectId";
            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                cmd.Parameters.AddWithValue("@subjectId", subjectId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "N/A";
            }
        }
        catch
        {
            return "N/A";
        }
        finally
        {
            CloseConnection();
        }
    }

    [WebMethod]
    public static object TestConnection()
    {
        try
        {
            return new
            {
                success = true,
                message = "WebMethods are working correctly",
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                server = HttpContext.Current.Server.MachineName
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                message = "Error: " + ex.Message,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }

    [WebMethod]
    public static object SaveGrade(int studentId, decimal marks, int examId, int subjectId, int teacherId, int schoolId)
    {
        try
        {
            // Create instance to get connection string
            ExamMark pageInstance = new ExamMark();
            pageInstance.ReadConfig();
            string connectionString = pageInstance.ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if grade already exists
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM Grades 
                    WHERE ExamId = @examId 
                      AND SubjectId = @subjectId 
                      AND StudentId = @studentId 
                      AND SchoolId = @schoolId";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@examId", examId);
                    checkCmd.Parameters.AddWithValue("@subjectId", subjectId);
                    checkCmd.Parameters.AddWithValue("@studentId", studentId);
                    checkCmd.Parameters.AddWithValue("@schoolId", schoolId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Update existing grade
                        string updateQuery = @"
                            UPDATE Grades 
                            SET Marks = @marks, 
                                TeacherId = @teacherId,
                                [DateSubmitted] = GETDATE()
                            WHERE ExamId = @examId 
                              AND SubjectId = @subjectId 
                              AND StudentId = @studentId 
                              AND SchoolId = @schoolId";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@marks", marks);
                            updateCmd.Parameters.AddWithValue("@teacherId", teacherId);
                            updateCmd.Parameters.AddWithValue("@examId", examId);
                            updateCmd.Parameters.AddWithValue("@subjectId", subjectId);
                            updateCmd.Parameters.AddWithValue("@studentId", studentId);
                            updateCmd.Parameters.AddWithValue("@schoolId", schoolId);

                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            return new
                            {
                                success = true,
                                message = "Marks updated successfully!",
                                rowsAffected = rowsAffected,
                                action = "update"
                            };
                        }
                    }
                    else
                    {
                        // Insert new grade
                        string insertQuery = @"
                            INSERT INTO Grades 
                            (ExamId, SubjectId, Marks, StudentId, TeacherId, SchoolId, DateSubmitted)
                            VALUES (@examId, @subjectId, @marks, @studentId, @teacherId, @schoolId, GETDATE())";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@examId", examId);
                            insertCmd.Parameters.AddWithValue("@subjectId", subjectId);
                            insertCmd.Parameters.AddWithValue("@marks", marks);
                            insertCmd.Parameters.AddWithValue("@studentId", studentId);
                            insertCmd.Parameters.AddWithValue("@teacherId", teacherId);
                            insertCmd.Parameters.AddWithValue("@schoolId", schoolId);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            return new
                            {
                                success = true,
                                message = "Marks saved successfully!",
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
            // Log the error
            System.Diagnostics.Debug.WriteLine("SaveGrade Error:" + ex.Message  + "\n" + ex.StackTrace );

            return new
            {
                success = false,
                message = "Error saving marks: " + ex,
                errorDetails = ex.ToString()
            };
        }
    }

    [WebMethod]
    public static object SaveAllGrades(List<StudentMark> students, int examId, int subjectId, int teacherId, int schoolId)
    {
        try
        {
            // Create instance to get connection string
            ExamMark pageInstance = new ExamMark();
            pageInstance.ReadConfig();
            string connectionString = pageInstance.ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use transaction for batch operations
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int updatedCount = 0;
                        int insertedCount = 0;

                        foreach (var student in students)
                        {
                            // Check if grade exists
                            string checkQuery = @"
                                SELECT COUNT(*) 
                                FROM Grades 
                                WHERE ExamId = @examId 
                                  AND SubjectId = @subjectId 
                                  AND StudentId = @studentId 
                                  AND SchoolId = @schoolId";

                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@examId", examId);
                                checkCmd.Parameters.AddWithValue("@subjectId", subjectId);
                                checkCmd.Parameters.AddWithValue("@studentId", student.StudentId);
                                checkCmd.Parameters.AddWithValue("@schoolId", schoolId);

                                int count = (int)checkCmd.ExecuteScalar();

                                if (count > 0)
                                {
                                    // Update existing
                                    string updateQuery = @"
                                        UPDATE Grades 
                                        SET Marks = @marks, 
                                            TeacherId = @teacherId
                                          
                                        WHERE ExamId = @examId 
                                          AND SubjectId = @subjectId 
                                          AND StudentId = @studentId 
                                          AND SchoolId = @schoolId";

                                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection, transaction))
                                    {
                                        updateCmd.Parameters.AddWithValue("@marks", student.Marks);
                                        updateCmd.Parameters.AddWithValue("@teacherId", teacherId);
                                        updateCmd.Parameters.AddWithValue("@examId", examId);
                                        updateCmd.Parameters.AddWithValue("@subjectId", subjectId);
                                        updateCmd.Parameters.AddWithValue("@studentId", student.StudentId);
                                        updateCmd.Parameters.AddWithValue("@schoolId", schoolId);

                                        updateCmd.ExecuteNonQuery();
                                        updatedCount++;
                                    }
                                }
                                else
                                {
                                    // Insert new
                                    string insertQuery = @"
                                        INSERT INTO Grades 
                                        (ExamId, SubjectId, Marks, StudentId, TeacherId, SchoolId, [DateSubmitted])
                                        VALUES (@examId, @subjectId, @marks, @studentId, @teacherId, @schoolId, GETDATE())";

                                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction))
                                    {
                                        insertCmd.Parameters.AddWithValue("@examId", examId);
                                        insertCmd.Parameters.AddWithValue("@subjectId", subjectId);
                                        insertCmd.Parameters.AddWithValue("@marks", student.Marks);
                                        insertCmd.Parameters.AddWithValue("@studentId", student.StudentId);
                                        insertCmd.Parameters.AddWithValue("@teacherId", teacherId);
                                        insertCmd.Parameters.AddWithValue("@schoolId", schoolId);

                                        insertCmd.ExecuteNonQuery();
                                        insertedCount++;
                                    }
                                }
                            }
                        }

                        transaction.Commit();

                        return new
                        {
                            success = true,
                            message = "Successfully saved {students.Count} marks! (Updated: {updatedCount}, Inserted: {insertedCount})",
                            updated = updatedCount,
                            inserted = insertedCount,
                            total = students.Count
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        // Log the error
                        System.Diagnostics.Debug.WriteLine("SaveAllGrades Error: {ex.Message}\n{ex.StackTrace}");

                        return new
                        {
                            success = false,
                            message = "Error saving marks: " + ex.Message,
                            errorDetails = ex.ToString()
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("SaveAllGrades Outer Error: {ex.Message}\n{ex.StackTrace}");

            return new
            {
                success = false,
                message = "System error: " + ex.Message,
                errorDetails = ex.ToString()
            };
        }
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        CloseConnection();
    }
}

// Helper class for batch operations
public class StudentMark
{
    public int StudentId { get; set; }
    public decimal Marks { get; set; }
}