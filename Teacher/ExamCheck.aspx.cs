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
                LoadClassTeacherInfo();

                // Debug info
                //  string debugInfo = "Session - Exam: {Session["exam"]}, Subject: {Session["subject"]}, Class: {Session["classId"]}";
                //  System.Diagnostics.Debug.WriteLine(debugInfo);

                // Set hidden field values
                hdnExamId.Value = Session["exam"].ToString() ?? "0";
                hdnClassId.Value = Session["classId"].ToString() ?? "0";


                if (lbluserId.Text.Trim() == lblTeacherId.Text.Trim())
                {
                    btnSave.Visible = true;
                    bdnDelete.Visible = true;
                }
                else
                {
                    btnSave.Visible = false;
                    bdnDelete.Visible = false;
                }
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



    private void LoadClassTeacherInfo()
    {
        try
        {
            string classId = Session["classId"].ToString();
           
           
            string sql = @"Select [TeacherId], [TeacherName] from FormTeachers where ClassId = @classId
            and SchoolId = @schoolId";
            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                cmd.Parameters.AddWithValue("@classId", classId);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                       lbluserId.Text = dr.GetInt32(0).ToString();
                       lblTeacherName.Text = dr.GetString(1);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading teacher info: " + ex.Message;
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

    public void LoadMarked()
    {
        try
        {
            OpenConnection();
            // Get required IDs
            string examId = Session["exam"].ToString() ?? "0";
            string classId = Session["classId"].ToString() ?? "0";
            string schoolId = lblSchoolId.Text.Trim();

            if (examId == "0" || classId == "0")
            {
                Response.Write("<tr><td colspan='6' style='color: red; text-align: center;'>Error: Missing exam, subject, or class information. Please go back and try again.</td></tr>");
                return;
            }


            //start here 
            string studentQuery = @"SELECT
    S.SubjectName AS Subject,
    U.Fullname AS SubjectTeacher,
	  TS.TotalStudents,
    SUM(CASE 
            WHEN G.Marks > 0 THEN 1 
            ELSE 0 
        END) AS TotalMarked,

  

    TS.TotalStudents -
    SUM(CASE 
            WHEN G.Marks > 0 THEN 1 
            ELSE 0 
        END) AS TotalUnmarked

FROM Subjects S
INNER JOIN Users U 
    ON U.UserId = S.TeacherId

CROSS APPLY (
    SELECT COUNT(St.StudentID) AS TotalStudents
    FROM Students St
    WHERE 
        St.CurrentClassID = @classId
        AND St.Status = 'Active'
        AND St.IsDeleted = 0
        AND St.SchoolId = @schoolId
) TS

LEFT JOIN Grades G 
    ON G.SubjectId = S.SubjectId
    AND G.ExamId = @examId

WHERE S.ClassId = @classId

GROUP BY 
    S.SubjectName,
    U.Fullname,
    TS.TotalStudents

ORDER BY 
    S.SubjectName;
";

            using (SqlCommand studentCmd = new SqlCommand(studentQuery, appconSQL2))
            {
                studentCmd.Parameters.AddWithValue("@classId", classId);
                studentCmd.Parameters.AddWithValue("@schoolId", schoolId);
                studentCmd.Parameters.AddWithValue("@examId", examId);

                using (SqlDataReader reader = studentCmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Response.Write("<tr><td colspan='6' style='color: orange; text-align: center;'>No data found.</td></tr>");
                        return;
                    }

                    while (reader.Read())
                    {
                       
                        Response.Write("<tr>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["Subject"].ToString()) + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["SubjectTeacher"].ToString()) + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["TotalStudents"].ToString()) + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["TotalMarked"].ToString()) + "</td>");
                        Response.Write("<td>" + HttpUtility.HtmlEncode(reader["TotalUnmarked"].ToString()) + "</td>");
                        Response.Write("</tr>");
                    }
                }
            }
            //end here
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

    
    public string GetTotalSubjects()
    {
        try
        {
            OpenConnection();

            string classId = Session["classId"].ToString();
            if (string.IsNullOrEmpty(classId) || classId == "0")
                return "N/A";

            string sql = "Select Count(SubjectId) as TotalSubjects from Subjects where ClassId = @classId";
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

  
  
    protected void Page_Unload(object sender, EventArgs e)
    {
        CloseConnection();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            OpenConnection();

            string examId = Session["exam"].ToString() ?? "0";
            string classId = Session["classId"].ToString() ?? "0";
            string schoolId = lblSchoolId.Text.Trim();


            if (examId == "0" || classId == "0")
            {
                lblError.Text = "Missing details! Please refresh your page.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                return;
            }

            //Check if already exist
            string sql3 = @"Select * from ExamMarked where ExamId = @examId and
    ClassId = @classId and [SchoolId] = @schoolId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@examId", examId);
            cmd3.Parameters.AddWithValue("@classId", classId);
            cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {
                lblError.Text = "Exam already marked!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                return;
            }
            else
            {
                dr3.Close();
                dr3.Dispose();

                //insert into exams
                string sql = @"Insert into ExamMarked ([ExamId], [ClassId], [SchoolId], [DateCreated], [DateUpdated], [CreatedBy])
        VALUES (@examId, @classId, @schoolId, GETDATE(), GETDATE(), @user)";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@examId", examId);
                cmd.Parameters.AddWithValue("@classId", classId);
                cmd.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Marked Subjects Saved Successfully!')", true);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
        finally
        {
            CloseConnection();
        }
    }

    protected void bdnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            OpenConnection();

            string examId = Session["exam"].ToString() ?? "0";
            string classId = Session["classId"].ToString() ?? "0";
            string schoolId = lblSchoolId.Text.Trim();


            if (examId == "0" || classId == "0")
            {
                lblError.Text = "Missing details! Please refresh your page.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                return;
            }

                //insert into exams
                string sql = @"Delete from ExamMarked where [ExamId] = @examId  and [ClassId] = @classId and [SchoolId] = @schoolId";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@examId", examId);
                cmd.Parameters.AddWithValue("@classId", classId);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Marked Exam Deleted Successfully!')", true);
            
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
        finally
        {
            CloseConnection();
        }
    }
}
