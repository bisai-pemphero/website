using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

public partial class Results : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object LoginStudent(string admissionNo, string dateofBirth, bool rememberMe)
    {
        HttpContext context = HttpContext.Current;

        string attemptKey = "LOGIN_ATTEMPTS_" + admissionNo;
        string lockKey = "LOCKED_" + admissionNo;

        // Check if locked
        if (context.Application[lockKey] != null)
        {
            DateTime lockedUntil = (DateTime)context.Application[lockKey];

            if (DateTime.Now < lockedUntil)
            {
                return new
                {
                    status = "locked",
                    message = "Account locked. Try again after 5 minutes."
                };
            }
            else
            {
                context.Application.Remove(lockKey);
                context.Application.Remove(attemptKey);
            }
        }

        int attempts = context.Application[attemptKey] != null ?
                       (int)context.Application[attemptKey] : 0;

        string connectionString =
            ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"SELECT StudentID
                             FROM Students
                             WHERE AdmissionNo = @admissionNo
                             AND DateOfBirth = @dateofBirth";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@admissionNo", admissionNo);
                cmd.Parameters.AddWithValue("@dateofBirth", DateTime.Parse(dateofBirth));

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    context.Application.Remove(attemptKey);

                    context.Session["StudentID"] = result.ToString();

                    if (rememberMe)
                    {
                        HttpCookie cookie = new HttpCookie("StudentLogin");
                        cookie["AdmissionNo"] = admissionNo;
                        cookie.Expires = DateTime.Now.AddDays(30);
                        context.Response.Cookies.Add(cookie);
                    }

                    return new
                    {
                        status = "success",
                        message = "Login successful"
                    };
                }
                else
                {
                    attempts++;
                    context.Application[attemptKey] = attempts;

                    if (attempts >= 5)
                    {
                        context.Application[lockKey] =
                            DateTime.Now.AddMinutes(5);

                        return new
                        {
                            status = "locked",
                            message = "Too many failed attempts. Locked for 5 minutes."
                        };
                    }

                    return new
                    {
                        status = "fail",
                        message = "Invalid credentials. Attempt " + attempts + "/5"
                    };
                }
            }
        }
    }
}
