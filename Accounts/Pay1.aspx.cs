using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class Fees : System.Web.UI.Page
{
    string appconStr;
    string server, appdb, user, password, version;
    SqlConnection appconSQL2;

    #region Page lifecycle
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            readConf();
            dbconnect();

            if (Session["USER"] != null) lblSession.Text = Session["USER"].ToString();
            else
            {
                Response.Redirect("../CommonPages/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                lblFeesId.Text = id;

                LoadUsername();
                LoadDetails();
                LoadFee();
                Payment_plan();
                LoadSms();
            }
        }
        catch (Exception ex)
        {
            // Top-level safety net
            LogError("Page load error", ex);
            lblError.Text = "An error occurred while loading the page.";
        }
    }

    // Ensure the connection is closed/disposed when the page ends
    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
        try
        {
            if (appconSQL2 != null)
            {
                if (appconSQL2.State != ConnectionState.Closed)
                {
                    appconSQL2.Close();
                }
                appconSQL2.Dispose();
                appconSQL2 = null;
            }
        }
        catch (Exception ex)
        {
            TraceError("Error closing DB connection", ex);
        }
    }
    #endregion

    #region Configuration & DB
    private void readConf()
    {
        try
        {
            string path = Server.MapPath("../dbconn.ini");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Configuration file not found: " + path);
            }

            using (var sr = File.OpenText(path))
            {
                string s;
                char SplitChar = '=';
                while ((s = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    string trimmed = s.Trim();
                    if (trimmed.StartsWith("#")) continue; // skip comments

                    string[] rfInfo = trimmed.Split(new[] { SplitChar }, 2);
                    if (rfInfo.Length < 2) continue;

                    string key = rfInfo[0].Trim().ToLower();
                    string value = rfInfo[1].Trim();

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
        }
        catch (Exception ex)
        {
            LogError("Failed to read configuration", ex);
            throw; // rethrow so caller knows config failed
        }
    }

    private void dbconnect()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(appdb))
            {
                throw new InvalidOperationException("Database configuration incomplete.");
            }

            appconStr = "Data Source= " + server + ";user id= " + user + ";password= " +password + ";max pool size=65536;Initial Catalog= " + appdb + ";";
            appconSQL2 = new SqlConnection(appconStr);
            appconSQL2.Open();
        }
        catch (SqlException ex)
        {
            LogError("Database connection error", ex);
            throw; // let Page_Load catch and show message
        }
        catch (Exception ex)
        {
            LogError("Unexpected error while connecting to database", ex);
            throw;
        }
    }
    #endregion

    #region Loaders
    public void LoadSms()
    {
        try
        {
            if (appconSQL2 == null) throw new InvalidOperationException("DB connection is not available.");
            if (string.IsNullOrWhiteSpace(lblSchoolId.Text)) return;

            string sqlBalance = @"Select [Balance] from sms where [SchoolId] = @schoolId";
            using (SqlCommand cmdBalance = new SqlCommand(sqlBalance, appconSQL2))
            {
                cmdBalance.Parameters.Add("@schoolId", SqlDbType.Int).Value = lblSchoolId.Text;
                using (SqlDataReader drBalance = cmdBalance.ExecuteReader())
                {
                    if (drBalance.Read() && !drBalance.IsDBNull(0))
                    {
                        lblsmsCurrentBalance.Text = drBalance.GetInt32(0).ToString();
                    }
                    else
                    {
                        lblsmsCurrentBalance.Text = "0";
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            LogError("SMS DB error", ex);
            lblError.Text = "sms error - database problem.";
        }
        catch (Exception ex)
        {
            LogError("SMS loading error", ex);
            lblError.Text = "sms error.";
        }
    }

    public void LoadUsername()
    {
        try
        {
            if (appconSQL2 == null) throw new InvalidOperationException("DB connection is not available.");
            if (string.IsNullOrWhiteSpace(lblSession.Text)) return;

            string sqlUser = @"select Fullname, SchoolId from Users where Username = @username";
            using (SqlCommand cmdUser = new SqlCommand(sqlUser, appconSQL2))
            {
                cmdUser.Parameters.Add("@username", SqlDbType.VarChar, 100).Value = lblSession.Text.Trim();
                using (SqlDataReader drUser = cmdUser.ExecuteReader())
                {
                    if (drUser.Read())
                    {
                        lblUser.Text = drUser.IsDBNull(0) ? "" : drUser.GetString(0);
                        lblSchoolId.Text = drUser.IsDBNull(1) ? "" : drUser.GetInt32(1).ToString();
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            LogError("Username DB error", ex);
            lblError.Text = "Username error - database problem.";
        }
        catch (Exception ex)
        {
            LogError("LoadUsername error", ex);
            lblError.Text = "Username error.";
        }
    }

    public void LoadDetails()
    {
        try
        {
            if (appconSQL2 == null) throw new InvalidOperationException("DB connection is not available.");
            if (string.IsNullOrWhiteSpace(lblFeesId.Text)) return;

            string sqlDetail = @"SELECT S.[FirstName], S.Middlename, S.LastName, F.[studentId],
                F.TermId, F.fees_Category,  P.[PhoneNumber], F.[totalFees], F.[amountPaid],
                T.TermName, F.balance, FC.CategoryName
                FROM [Students] AS S
                JOIN [StudentParent] as P on S.ParentID = P.ParentID
                JOIN Fees as F on F.[studentId] = S.StudentID
                Join SchoolTerm as T on T.TermId = F.TermId
                Join FeesCategory as FC on FC.CategoryId = F.fees_Category
                WHERE F.FeesId = @feesId";

            using (SqlCommand cmdDetail = new SqlCommand(sqlDetail, appconSQL2))
            {
                cmdDetail.Parameters.Add("@feesId", SqlDbType.VarChar, 50).Value = lblFeesId.Text.Trim();
                using (SqlDataReader drDetail = cmdDetail.ExecuteReader())
                {
                    if (drDetail.Read())
                    {
                        lblStudentName.Text = SafeGetString(drDetail, 0) + " " + SafeGetString(drDetail, 1) + " " + SafeGetString(drDetail, 2);
                        lblStudentID.Text = SafeGetString(drDetail, 3);
                        lblTermId.Text = SafeGetString(drDetail, 4);
                        lblCategory.Text = SafeGetString(drDetail, 11);
                        lblParentContact.Text = SafeGetString(drDetail, 6);
                        lblTotalFees.Text = SafeGetDoubleAsString(drDetail, 7);
                        lblAmountPaid.Text = SafeGetDoubleAsString(drDetail, 8);
                        lblTermName.Text = SafeGetString(drDetail, 9);
                        lblBalance.Text = SafeGetDoubleAsString(drDetail, 10);
                        lblShowBalance.Text = "K" + SafeGetDoubleAsString(drDetail, 10, "N2");
                        lblCategoryId.Text = SafeGetString(drDetail, 5);
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            LogError("Load details DB error", ex);
            lblError.Text = "Load details error (database).";
        }
        catch (Exception ex)
        {
            LogError("Load details error", ex);
            lblError.Text = "Load details error.";
        }
    }
    #endregion

    #region Utilities
    public void LoadReceiptNo()
    {
        try
        {
            if (appconSQL2 == null) throw new InvalidOperationException("DB connection is not available.");
            string schoolName = "";
            string sqlReceipt = "Select [School_name] from [AllSchools] where [SchoolId] = @Id";
            using (SqlCommand cmdReceipt = new SqlCommand(sqlReceipt, appconSQL2))
            {
                cmdReceipt.Parameters.Add("@Id", SqlDbType.Int).Value = lblSchoolId.Text;
                using (SqlDataReader drReceipt = cmdReceipt.ExecuteReader())
                {
                    if (drReceipt.Read() && !drReceipt.IsDBNull(0))
                    {
                        schoolName = drReceipt.GetString(0);
                        lblSchoolName.Text = schoolName;
                    }
                }
            }

            string Name = GetFirstTwoAndLastChar(schoolName);
            DateTime Date = DateTime.Now;
            string today = Date.ToString("ddMMyy");
            string time = Date.ToString("HHmm");
            string seconds = Date.ToString("ss");
            lblReceiptNumber.Text = Name + today + "-" + time + seconds;
        }
        catch (Exception ex)
        {
            LogError("LoadReceiptNo error", ex);
            lblError.Text = "Receipt generation error.";
        }
    }

    static string GetFirstTwoAndLastChar(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str ?? "";
        }

        if (str.Length < 3)
        {
            return str;
        }

        string firstTwo = str.Substring(0, 2);
        // original method returned only firstTwo; preserving that behavior
        return firstTwo;
    }

    private static string SafeGetString(SqlDataReader dr, int index)
    {
        return (dr.IsDBNull(index)) ? "" : dr.GetString(index);
    }

    private static string SafeGetDoubleAsString(SqlDataReader dr, int index, string format = null)
    {
        if (dr.IsDBNull(index)) return "0";
        double val = dr.GetDouble(index);
        return format == null ? val.ToString() : val.ToString(format);
    }

   

    private void LogError(string message, Exception ex)
    {
        // Use Trace for server-side logging; replace with your favorite logging (Serilog/NLog/etc.)
        System.Diagnostics.Trace.TraceError("message: ex");
        // You could also write to a text file or event log here if desired.
    }

    private void TraceError(string message, Exception ex)
    {
        System.Diagnostics.Trace.TraceError("message: ex");
    }
    #endregion

    #region UI helpers
    public void SendSmsNotification()
    {
        // This method previously composed message but did not send. Keep it as composition helper if required.
        try
        {
            double currentBalance = Convert.ToDouble(lblBalance.Text);
            double AmountPaid = Convert.ToDouble(lblAmountPaid.Text);
            double amountPaying = Convert.ToDouble(txtAmount.Text);

            double newbalance = currentBalance - amountPaying;
            double newTotal = AmountPaid + amountPaying;

            string Name = lblStudentName.Text.Trim();
            string Term = lblTermName.Text.Trim();
            string parentName = txtbyname.Text.Trim();
            string Amount = txtAmount.Text.Trim();
            string categoryName = lblCategory.Text.Trim();
            string balance = newbalance.ToString();
            DateTime now = DateTime.Now;
            string date = now.ToString();
            string schoolName = lblSchoolName.Text;
            string Phone = lblParentContact.Text;
            string receipt = lblReceiptNumber.Text.Trim();

            string Message = "Dear " +parentName + " you have paid K" + Amount + " to " + schoolName +" as " + categoryName +" for " +Name + ", for " +Term + " " + balance + "is K" + balance + "";
            // If you need to display, set to a label or return the string
            // lblSmsPreview.Text = Message;
        }
        catch (Exception ex)
        {
            LogError("SendSmsNotification compose error", ex);
            lblError.Text = "Error composing SMS notification.";
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoice.aspx");
    }

    private void Payment_plan()
    {
        try
        {
            drpcashcheque.Items.Clear();
            drpcashcheque.Items.Add("");
            drpcashcheque.Items.Add("Cash");
            drpcashcheque.Items.Add("Bank Deposit");
            drpcashcheque.Items.Add("Mobile Money");
            drpcashcheque.Items.Add("Cheque");
        }
        catch (Exception ex)
        {
            LogError("Payment_plan error", ex);
        }
    }
    #endregion

    #region Fee details grid
    public DataTable LoadFeesDetails()
    {
        DataTable dt = new DataTable();

        try
        {
            string[] details = lblStudentID.Text.Split('|');
            string studentId = details.Length > 0 ? details[0] : lblStudentID.Text.Trim();

            string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"
                    Select F.amountPaid, F.balance, ST.[TermName],
                    CONCAT(DATENAME(MONTH, ST.[Startdate]), ' ', YEAR(ST.[Enddate])) as Period
                    From Fees as F
                    Join SchoolTerm as ST on ST.TermId = F.TermId
                    where F.SchoolId = @schoolId and F.studentId = @studentId
                    and F.fees_Category = @category", connection))
                {
                    command.Parameters.Add("@category", SqlDbType.VarChar).Value = lblCategoryId.Text;
                    command.Parameters.Add("@studentId", SqlDbType.VarChar).Value = studentId;
                    command.Parameters.Add("@schoolId", SqlDbType.Int).Value = lblSchoolId.Text.Trim();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError("LoadFeesDetails error", ex);
            lblError.Text = "Could not load fee details.";
        }

        return dt;
    }

    public void LoadFee()
    {
        try
        {
            DataTable generalReport = LoadFeesDetails();
            if (generalReport.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRow row in generalReport.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(row["TermName"]) + " (" + HttpUtility.HtmlEncode(row["Period"]) + " )" + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(row["amountPaid"]) + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(row["balance"]) + "</td>");
                    sb.Append("</tr>");
                }

                feesDetails.InnerHtml = sb.ToString();
            }
            else
            {
                feesDetails.InnerHtml = "<tr><td colspan='3'>No data found.</td></tr>";
            }
        }
        catch (Exception ex)
        {
            LogError("LoadFee error", ex);
            lblError.Text = "Error rendering fee table.";
        }
    }
    #endregion

    #region Save Payment
    protected async void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (appconSQL2 == null) throw new InvalidOperationException("DB connection is not available.");

            string existingRecordCheck = @"SELECT COUNT(*) FROM [Fees] WHERE studentId = @studentId 
                    AND TermId = @term AND fees_category = @feesCategory and [SchoolId] = @schoolId ";

            using (SqlCommand cmdCheck = new SqlCommand(existingRecordCheck, appconSQL2))
            {
                cmdCheck.Parameters.Add("@studentId", SqlDbType.VarChar).Value = lblStudentID.Text.Trim();
                cmdCheck.Parameters.Add("@feesCategory", SqlDbType.VarChar).Value = lblCategoryId.Text.Trim();
                cmdCheck.Parameters.Add("@term", SqlDbType.VarChar).Value = lblTermId.Text.Trim();
                cmdCheck.Parameters.Add("@schoolId", SqlDbType.Int).Value = lblSchoolId.Text.Trim();

                int matchingRecords = Convert.ToInt32(cmdCheck.ExecuteScalar());

                if (matchingRecords <= 0)
                {
                    lblError.Text = "No matching fee record found to update.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                        "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
                    return;
                }
            }

            // Start update inside a transaction to keep DB consistent
            using (SqlTransaction tran = appconSQL2.BeginTransaction())
            {
                try
                {
                    LoadReceiptNo();

                    double currentBalance = Convert.ToDouble(lblBalance.Text);
                    double AmountPaid = Convert.ToDouble(lblAmountPaid.Text);
                    double amountPaying = Convert.ToDouble(txtAmount.Text);

                    if (amountPaying <= 0)
                    {
                        lblError.Text = "Amount must be greater than zero.";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                            "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
                        tran.Rollback();
                        return;
                    }

                    double newbalance = currentBalance - amountPaying;
                    double newTotal = AmountPaid + amountPaying;
                    string ReceiptNo = lblReceiptNumber.Text.Trim();

                    // Update Fees
                    string update = @"Update Fees set [FeesStatus] = @feesStatus, [amountPaid] = @amount,
                                [balance] = @balance where FeesId = @feesId";
                    using (SqlCommand cmdupdate = new SqlCommand(update, appconSQL2, tran))
                    {
                        cmdupdate.Parameters.Add("@feesId", SqlDbType.VarChar).Value = lblFeesId.Text.Trim();
                        cmdupdate.Parameters.Add("@feesStatus", SqlDbType.VarChar).Value = (newbalance > 0) ? "Pending" : "Completed";
                        cmdupdate.Parameters.Add("@amount", SqlDbType.Decimal).Value = Convert.ToDecimal(newTotal);
                        cmdupdate.Parameters.Add("@balance", SqlDbType.Decimal).Value = Convert.ToDecimal(newbalance);

                        cmdupdate.ExecuteNonQuery();
                    }

                    // check if transaction already saved first
                    string sql = @"Select COUNT(*) from Payments where FeesId = @Id and Paid = @paid and paymentMode = @mode and datePaid = @datePaid";
                    using (SqlCommand cmd = new SqlCommand(sql, appconSQL2, tran))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = lblFeesId.Text.Trim();
                        cmd.Parameters.Add("@paid", SqlDbType.Decimal).Value = Convert.ToDecimal(amountPaying);
                        cmd.Parameters.Add("@mode", SqlDbType.VarChar).Value = drpcashcheque.Text.Trim();
                        cmd.Parameters.Add("@datePaid", SqlDbType.VarChar).Value = txtDate.Text.Trim();

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            tran.Rollback();
                            lblError.Text = "Payment already made, please check!";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                                "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
                            return;
                        }
                    }

                    // Insert payment
                    string feeTransaction = @"Insert into Payments ([FeesId], [Paid], [Balance], 
                        [paymentMode], [paidBy], [paymentReference], [receipNumber], 
                        [datePaid], [postedBy], [dateRegistered], IsDeleted)
                        VALUES (@feesId, @amount, @balance, @mode, @paidby, @reference,
                        @receiptNumber, @datePaid, @registeredBy, GETDATE(), 0)";
                    using (SqlCommand cmdFee = new SqlCommand(feeTransaction, appconSQL2, tran))
                    {
                        cmdFee.Parameters.Add("@feesId", SqlDbType.VarChar).Value = lblFeesId.Text.Trim();
                        cmdFee.Parameters.Add("@amount", SqlDbType.Decimal).Value = Convert.ToDecimal(amountPaying);
                        cmdFee.Parameters.Add("@balance", SqlDbType.Decimal).Value = Convert.ToDecimal(newbalance);
                        cmdFee.Parameters.Add("@mode", SqlDbType.VarChar).Value = drpcashcheque.Text.Trim();
                        cmdFee.Parameters.Add("@paidBy", SqlDbType.VarChar).Value = txtbyname.Text.Trim();
                        cmdFee.Parameters.Add("@reference", SqlDbType.VarChar).Value = txtReference.Text.Trim();
                        cmdFee.Parameters.Add("@receiptNumber", SqlDbType.VarChar).Value = ReceiptNo;
                        cmdFee.Parameters.Add("@datePaid", SqlDbType.VarChar).Value = txtDate.Text.Trim();
                        cmdFee.Parameters.Add("@registeredBy", SqlDbType.VarChar).Value = lblUser.Text.Trim();

                        cmdFee.ExecuteNonQuery();
                    }

                    // get transaction id
                    int transId = 0;
                    string sqlTrans = @"Select TransanctionId from Payments where [receipNumber] = @receipt";
                    using (SqlCommand cmdTrans = new SqlCommand(sqlTrans, appconSQL2, tran))
                    {
                        cmdTrans.Parameters.Add("@receipt", SqlDbType.VarChar).Value = ReceiptNo;
                        using (SqlDataReader drTrans = cmdTrans.ExecuteReader())
                        {
                            if (drTrans.Read() && !drTrans.IsDBNull(0))
                            {
                                transId = drTrans.GetInt32(0);
                            }
                        }
                    }

                    // commit
                    tran.Commit();

                    // Send SMS if required (send outside transaction ideally)
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(lblParentContact.Text))
                        {
                            int smsBalance = Convert.ToInt32(lblsmsCurrentBalance.Text.Trim());
                            if (smsBalance > 0)
                            {
                                var smsResult = await SendSmsAsync();
                                // optionally use smsResult
                            }
                        }
                    }
                    catch (Exception exSms)
                    {
                        // SMS failure should not block receipt creation; just log and continue
                        LogError("SMS send error", exSms);
                    }

                    // Redirect after successful commit
                    Response.Redirect("Receipt.aspx?id=" + transId);
                }
                catch (Exception exInner)
                {
                    try { tran.Rollback(); } catch { /* ignore rollback errors */ }
                    LogError("Error saving payment", exInner);
                    lblError.Text = "Fees payment error.";
                }
            }
        }
        catch (SqlException ex)
        {
            LogError("Saving error - DB", ex);
            lblError.Text = "Saving error: database problem.";
        }
        catch (FormatException ex)
        {
            LogError("Saving error - format", ex);
            lblError.Text = "Saving error: invalid numeric format.";
        }
        catch (Exception ex)
        {
            LogError("Saving error", ex);
            lblError.Text = "Saving error.";
        }
    }
    #endregion

    #region Send SMS
    private async Task<string> SendSmsAsync()
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://sms.cloud265.com");

                var request = new HttpRequestMessage(HttpMethod.Post, "/public/sms/9800");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "myApi");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                double currentBalance = Convert.ToDouble(lblBalance.Text.Trim());
                double AmountPaid = Convert.ToDouble(lblAmountPaid.Text.Trim());
                double amountPaying = Convert.ToDouble(txtAmount.Text.Trim());
                double newbalance = currentBalance - amountPaying;
                double newTotal = AmountPaid + amountPaying;

                string parentName = txtbyname.Text.Trim();
                string Amount = txtAmount.Text.Trim();
                string categoryName = lblCategory.Text.Trim();
                string category = categoryName;
                string balance = newbalance.ToString();
                string schoolName = lblSchoolName.Text;
                string receipt = lblReceiptNumber.Text.Trim();
                string Message = "Dear " + parentName + " you have paid K" + Amount + " to " + schoolName + " as " + category + " for " + lblStudentName.Text + "," + " for " + lblTermName.Text + "  balance is K" + balance;

                string theTo = lblParentContact.Text;
                string theMessage = Message;

                var json = "{ \"to\": \"" + theTo + "\", \"message\": \"" + theMessage.Replace("\"", "'") + "\", \"from\": \"Deltacomm\" }";
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();

                    string sql4 = @"Update sms set Used = Used + 1, Balance = Balance - 1 where SchoolId = @schoolId";
                    using (SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2))
                    {
                        cmd4.Parameters.Add("@schoolId", SqlDbType.Int).Value = lblSchoolId.Text.Trim();
                        cmd4.ExecuteNonQuery();
                    }

                    return "SMS sent successfully: " + res;
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    LogError("SMS provider error", new Exception(err));
                    return "Failed to send SMS. Status:" + response.StatusCode + " Details:" + err;
                }
            }
        }
        catch (HttpRequestException ex)
        {
            LogError("SMS HTTP error", ex);
            return "Failed to send SMS (network).";
        }
        catch (Exception ex)
        {
            LogError("SMS error", ex);
            return "Failed to send SMS.";
        }
    }
    #endregion

    protected void btnNo_Click(object sender, EventArgs e)
    {
        Response.Redirect("FeesPayment.aspx");
    }
}
