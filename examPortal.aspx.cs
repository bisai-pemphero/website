using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class viewExams : System.Web.UI.Page
{
    // Connection strings
    private string conString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
    private string appconStr;
    private string server, appdb, user, password, version;
    private SqlConnection appconSQL2;

    #region Database Configuration
    private void readConf()
    {
        try
        {
            using (System.IO.StreamReader sr = System.IO.File.OpenText(Server.MapPath("dbconn.ini")))
            {
                string s = "";
                string[] rfInfo = new string[2];
                char SplitChar = '=';

                while ((s = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(s.Trim()) && !s.StartsWith("#"))
                    {
                        rfInfo = s.Split(SplitChar);
                        if (rfInfo.Length >= 2)
                        {
                            switch (rfInfo[0].Trim().ToLower())
                            {
                                case "server": server = rfInfo[1].Trim(); break;
                                case "user": user = rfInfo[1].Trim(); break;
                                case "password": password = rfInfo[1].Trim(); break;
                                case "appdb": appdb = rfInfo[1].Trim(); break;
                                case "version": version = rfInfo[1].Trim(); break;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to read database configuration: " + ex.Message);
        }
    }

    private void dbconnect()
    {
        appconStr = "Data Source=" + server + ";user id=" + user + ";password=" + password +
                    ";max pool size=65536;Initial Catalog=" + appdb + ";";
        appconSQL2 = new SqlConnection(appconStr);
        appconSQL2.Open();
    }
    #endregion

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            readConf();
            dbconnect();

            // Check if student is logged in
            if (Session["StudentID"] == null)
            {
                Response.Redirect("Results.aspx");
            }

            if (!IsPostBack)
            {
                LoadStudentInfo();
                LoadExams();
            }
        }
        catch (Exception ex)
        {
            ShowErrorAlert("System Error", "Failed to load page: " + ex.Message);
        }
    }

    protected void rptExams_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            LinkButton lnkViewResults = (LinkButton)e.Item.FindControl("lnkViewResults");
            if (lnkViewResults != null)
            {
                // Add any additional attributes if needed
                lnkViewResults.ToolTip = "Click to view your results for this examination";
            }
        }
    }
    #endregion

    #region Student Information
    private void LoadStudentInfo()
    {
        try
        {
            int studentId = Convert.ToInt32(Session["StudentID"]);
            SqlCommand cmd = null;
            SqlDataReader dr = null;

            try
            {
                string sql = @"
                    SELECT 
                        S.FirstName, 
                        S.Middlename, 
                        S.LastName, 
                        C.ClassName, 
                        C.Level
                    FROM Students S
                    JOIN Classes C ON C.ClassID = S.CurrentClassID
                    WHERE S.StudentID = @studentId";

                cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string firstName = dr["FirstName"].ToString();
                    string lastName = dr["LastName"].ToString();
                    string className = dr["ClassName"].ToString();
                    string level = dr["Level"].ToString();

                    lblStudentName.InnerText = "Welcome, " + firstName + " " + lastName + "!";
                    lblClassName.InnerText = className;
                    lblLevelDisplay.InnerText = level;
                    lblLevel.Text = level; // Store for command use
                    lblStudentId.Text = studentId.ToString();
                }
            }
            finally
            {
                if (dr != null) dr.Close();
                if (cmd != null) cmd.Dispose();
            }
        }
        catch (Exception ex)
        {
            ShowErrorAlert("Student Info Error", "Could not load student information: " + ex.Message);
        }
    }
    #endregion

    #region Exams Loading
    private void LoadExams()
    {
        try
        {
            int studentId = Convert.ToInt32(Session["StudentID"]);
            int schoolId = 0;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // 1. Get SchoolId
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT SchoolId FROM Students WHERE StudentID = @studentId", con))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    object result = cmd.ExecuteScalar();
                    schoolId = result != null ? Convert.ToInt32(result) : 0;
                }

                if (schoolId > 0)
                {
                    // 2. Get Exams for this school
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT DISTINCT 
                            E.ExamsId,
                            CONCAT(DATENAME(YEAR, A.Start_year), '-', YEAR(A.End_year)) AS AcademicYear,
                            T.TermName,
                            E.Exam_name
                        FROM Exams E
                        JOIN Academic_Year A ON E.AcademicYear = A.AcademicyearId
                        JOIN SchoolTerm T ON E.TermId = T.TermId
                        JOIN ExamMarked EM ON E.ExamsId = EM.ExamId
                        WHERE E.SchoolId = @schoolId", con))
                    {
                        cmd.Parameters.AddWithValue("@schoolId", schoolId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptExams.DataSource = dt;
                                rptExams.DataBind();
                                lblEmpty.Visible = false;
                            }
                            else
                            {
                                rptExams.DataSource = null;
                                rptExams.DataBind();
                                lblEmpty.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    lblEmpty.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ShowErrorAlert("Load Exams Error", "Failed to load examinations: " + ex.Message);
            lblEmpty.Visible = true;
        }
    }
    #endregion

    #region View Results Command
    protected void lnkViewResults_Command(object sender, CommandEventArgs e)
    {
        try
        {
            string examId = e.CommandArgument.ToString();
            string classLevel = lblLevel != null ? lblLevel.Text : "";
            int studentId = Convert.ToInt32(Session["StudentID"]);

            // Check fees balance
            DataTable dtFeesBalance = GetStudentFeesBalance(studentId);

            if (dtFeesBalance != null && dtFeesBalance.Rows.Count > 0)
            {
                // Student has outstanding balance - Show modal with fees details
                ShowFeesBalanceModal(dtFeesBalance);
            }
            else
            {
                // No balance - Proceed to report card
                Session["ExamsId"] = examId;
                Session["StudentID"] = studentId;

                if (classLevel == "Primary" || classLevel == "Secondary - Junior")
                {
                    Response.Redirect("ReportCard.aspx");
                }
                else if (classLevel == "Secondary - Senior")
                {
                    Response.Redirect("ReportCard2.aspx");
                }
                else
                {
                    Response.Redirect("ReportCard.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            ShowErrorAlert("Command Error", "Failed to process request: " + ex.Message);
        }
    }
    #endregion

    #region Fees Balance Check
    private DataTable GetStudentFeesBalance(int studentId)
    {
        DataTable dt = new DataTable();
        SqlConnection conn = null;
        SqlCommand cmd = null;
        SqlDataAdapter da = null;

        try
        {
            conn = new SqlConnection(conString); // Use the correct connection string
            conn.Open();

            string query = @"
                SELECT 
                    T.TermName, 
                    FC.CategoryName, 
                    ISNULL(F.amountPaid, 0) AS amountPaid, 
                    ISNULL(F.balance, 0) AS balance
                FROM Fees F
                INNER JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category
                INNER JOIN SchoolTerm T ON F.TermId = T.TermId
                WHERE F.studentId = @studentId 
                AND ISNULL(F.balance, 0) > 0
                ORDER BY T.TermName DESC, FC.CategoryName";

            cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception ex)
        {
            // Log error but return empty table - don't block access if fees check fails
            System.Diagnostics.Debug.WriteLine("Fees Balance Error: " + ex.Message);
            return new DataTable();
        }
        finally
        {
            if (da != null) da.Dispose();
            if (cmd != null) cmd.Dispose();
            if (conn != null) conn.Dispose();
        }

        return dt;
    }

    private void ShowFeesBalanceModal(DataTable dtFees)
    {
        // Calculate total balance
        decimal totalBalance = 0;
        foreach (DataRow row in dtFees.Rows)
        {
            totalBalance += Convert.ToDecimal(row["balance"]);
        }

        // Build HTML table for modal - Simplified to avoid escaping issues
        StringBuilder tableHtml = new StringBuilder();
        tableHtml.Append("<div style='font-family: Inter, sans-serif; max-height: 400px; overflow-y: auto; padding: 5px;'>");
        tableHtml.Append("<h4 style='color: #ef4444; margin-bottom: 20px; border-bottom: 2px solid #ef4444; padding-bottom: 12px; display: flex; align-items: center; gap: 10px;'>");
        tableHtml.Append("<i class='fas fa-exclamation-triangle'></i> Outstanding Fees Balance</h4>");

        tableHtml.Append("<table style='width: 100%; border-collapse: collapse; font-size: 14px;'>");
        tableHtml.Append("<thead>");
        tableHtml.Append("<tr style='background: linear-gradient(135deg, #1e3a8a, #3b82f6); color: white;'>");
        tableHtml.Append("<th style='padding: 12px; text-align: left; border-radius: 8px 0 0 0;'>Term</th>");
        tableHtml.Append("<th style='padding: 12px; text-align: left;'>Fee Category</th>");
        tableHtml.Append("<th style='padding: 12px; text-align: right;'>Amount Paid</th>");
        tableHtml.Append("<th style='padding: 12px; text-align: right; border-radius: 0 8px 0 0;'>Balance</th>");
        tableHtml.Append("</tr>");
        tableHtml.Append("</thead>");
        tableHtml.Append("<tbody>");

        foreach (DataRow row in dtFees.Rows)
        {
            decimal amountPaid = Convert.ToDecimal(row["amountPaid"]);
            decimal balance = Convert.ToDecimal(row["balance"]);

            tableHtml.Append("<tr style='border-bottom: 1px solid #e2e8f0;'>");
            tableHtml.Append("<td style='padding: 12px;'>" + row["TermName"].ToString() + "</td>");
            tableHtml.Append("<td style='padding: 12px;'>" + row["CategoryName"].ToString() + "</td>");
            tableHtml.Append("<td style='padding: 12px; text-align: right;'>MWK " + amountPaid.ToString("N0") + "</td>");
            tableHtml.Append("<td style='padding: 12px; text-align: right; color: #ef4444; font-weight: 600;'>MWK " + balance.ToString("N0") + "</td>");
            tableHtml.Append("</tr>");
        }

        tableHtml.Append("</tbody>");
        tableHtml.Append("<tfoot>");
        tableHtml.Append("<tr style='background: #f8fafc; font-weight: 700;'>");
        tableHtml.Append("<td colspan='3' style='padding: 15px 12px; text-align: right;'>Total Outstanding Balance:</td>");
        tableHtml.Append("<td style='padding: 15px 12px; text-align: right; color: #ef4444; font-size: 1.1em;'>MWK " + totalBalance.ToString("N0") + "</td>");
        tableHtml.Append("</tr>");
        tableHtml.Append("</tfoot>");
        tableHtml.Append("</table>");

        tableHtml.Append("<div style='background: #fffbeb; border-left: 4px solid #f59e0b; padding: 16px; margin-top: 20px; border-radius: 8px; display: flex; align-items: center; gap: 12px;'>");
        tableHtml.Append("<i class='fas fa-info-circle' style='color: #d97706; font-size: 20px;'></i>");
        tableHtml.Append("<span style='color: #92400e;'>Please clear the outstanding balance to access your report card.</span>");
        tableHtml.Append("</div>");
        tableHtml.Append("</div>");

        // Escape single quotes for JavaScript
        string escapedHtml = tableHtml.ToString().Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ");

        // Get session values safely
        string studentId = Session["StudentID"] != null ? Session["StudentID"].ToString() : "";
        string amountValue = totalBalance.ToString("N0").Replace(",", "");

        // SweetAlert2 Modal Script
        string script = @"
            Swal.fire({
                title: '<span style=""color: #ef4444;""><i class=""fas fa-lock""></i> Access Restricted</span>',
                html: '" + escapedHtml + @"',
                icon: 'warning',
                showCancelButton: true,
                
                cancelButtonColor: '#64748b',
               
                cancelButtonText: '<i class=""fas fa-times""></i> Close',
                showCloseButton: true,
                allowOutsideClick: false,
                width: '800px',
                padding: '1.5rem',
                customClass: {
                    popup: 'animated fadeInDown faster',
                    title: 'swal2-title-custom',
                    htmlContainer: 'swal2-html-custom'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = 'examPortal.aspx"  + @"';
                }
            });";

        // Register script with ScriptManager for UpdatePanel compatibility
        ScriptManager.RegisterStartupScript(this, GetType(), "ShowFeesModal", script, true);
    }
    #endregion

    #region Helper Methods
    private void ShowErrorAlert(string title, string message)
    {
        string script = @"
            Swal.fire({
                title: '" + title.Replace("'", "\\'") + @"',
                text: '" + message.Replace("'", "\\'") + @"',
                icon: 'error',
                confirmButtonColor: '#0ea5e9'
            });";

        ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
    }

    private void ShowSuccessAlert(string message)
    {
        string script = @"
            Swal.fire({
                title: 'Success!',
                text: '" + message.Replace("'", "\\'") + @"',
                icon: 'success',
                timer: 3000,
                showConfirmButton: false
            });";

        ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert", script, true);
    }
    #endregion

    #region Cleanup
    protected void Page_Unload(object sender, EventArgs e)
    {
        if (appconSQL2 != null && appconSQL2.State == ConnectionState.Open)
        {
            appconSQL2.Close();
            appconSQL2.Dispose();
        }
    }
    #endregion
}