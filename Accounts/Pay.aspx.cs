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
            string id = Request.QueryString["id"];

            lblFeesId.Text = id;

            LoadUsername();
            LoadDetails();
            LoadFee();
            Payment_plan();
            LoadSms();
        }
    }

    public void LoadSms()
    {
        try
        {
           
            string sqlBalance = @"Select [Balance] from sms where [SchoolId] = @schoolId";
            SqlCommand cmdBalance = new SqlCommand(sqlBalance, appconSQL2);
            cmdBalance.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader drBalance = cmdBalance.ExecuteReader();
            while (drBalance.Read())
            {
                lblsmsCurrentBalance.Text = drBalance.GetInt32(0).ToString();
            }
            drBalance.Close();
            drBalance.Dispose();
        }
        catch (Exception ex)
        {
            lblError.Text = "sms error" + ex;
        }
        finally
        {
            //
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sqlUser = @"select Fullname, SchoolId from Users where Username = @username";
            SqlCommand cmdUser = new SqlCommand(sqlUser, appconSQL2);
            cmdUser.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader drUser = cmdUser.ExecuteReader();
            while (drUser.Read())
            {
                lblUser.Text = drUser.GetString(0);
                lblSchoolId.Text = drUser.GetInt32(1).ToString();
            }
            drUser.Close();
            drUser.Dispose();

           
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

    public void LoadDetails()
    {
        try
        {
            //get name and contact
            string sqlDetail = @"SELECT S.[FirstName], S.Middlename, S.LastName, F.[studentId],
            F.TermId, F.fees_Category,  P.[PhoneNumber], F.[totalFees], F.[amountPaid],
            T.TermName, F.balance, FC.CategoryName
			 FROM [Students] AS S  
			join [StudentParent] as P 
            on S.ParentID = P.ParentID 
			join Fees as F on F.[studentId] = S.StudentID 
            Join SchoolTerm as T on T.TermId = F.TermId
			Join FeesCategory as FC on FC.CategoryId = F.fees_Category WHERE F.FeesId = @feesId";
            SqlCommand cmdDetail = new SqlCommand(sqlDetail, appconSQL2);
            cmdDetail.Parameters.AddWithValue("@feesId", lblFeesId.Text.Trim());
            SqlDataReader drDetail = cmdDetail.ExecuteReader();
            while (drDetail.Read())
            {
                lblStudentName.Text = drDetail.GetString(0) + " " + drDetail.GetString(1) + " " + drDetail.GetString(2);
                lblStudentID.Text = drDetail.GetString(3);
                lblTermId.Text = drDetail.GetString(4);
                lblCategory.Text = drDetail.GetString(11);
                lblParentContact.Text = drDetail.GetString(6);
                lblTotalFees.Text = drDetail.GetDouble(7).ToString();
                lblAmountPaid.Text = drDetail.GetDouble(8).ToString();
                lblTermName.Text = drDetail.GetString(9);
                lblBalance.Text = drDetail.GetDouble(10).ToString();
                
                lblShowBalance.Text = "K" + drDetail.GetDouble(10).ToString("N2");
                lblCategoryId.Text = drDetail.GetString(5);
            }

            drDetail.Close();
            drDetail.Dispose(); 
        }
        catch(Exception ex)
        {
            lblError.Text = "Load details error: " + ex;
        }
    }
    public void LoadReceiptNo()
    {
        string SchoolName = "";
        string sqlReceipt = " Select [School_name] from [AllSchools] where [SchoolId] = @Id";
        SqlCommand cmdReceipt = new SqlCommand(sqlReceipt, appconSQL2);
        cmdReceipt.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
        SqlDataReader drReceipt = cmdReceipt.ExecuteReader();

        while (drReceipt.Read())
        {
            SchoolName = drReceipt.GetString(0);
            lblSchoolName.Text = drReceipt.GetString(0);
        }

        drReceipt.Close();
        drReceipt.Dispose();

        string Name = GetFirstTwoAndLastChar(SchoolName);

        DateTime Date = DateTime.Now;
        string today = Date.ToString("ddMMyy");
        string time = Date.ToString("HHmm");
        string seconds = Date.ToString("ss");
        lblReceiptNumber.Text = Name + today + "-" + time + seconds;
    }

    static string GetFirstTwoAndLastChar(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str; // Return the original string if it is null or empty
        }

        if (str.Length < 3)
        {
            return str; // Return the original string if it has less than 3 characters
        }

        string firstTwo = str.Substring(0, 2);
        string lastChar = str.Substring(str.Length - 1);

        return firstTwo;
    }

   
    public void SendSmsNotification()
    {
        double currentBalance = Convert.ToDouble(lblBalance.Text.Trim());
        double AmountPaid = Convert.ToDouble(lblAmountPaid.Text.Trim());

        double amountPaying = Convert.ToDouble(txtAmount.Text.Trim());

        double newbalance = currentBalance - amountPaying;
        double newTotal = AmountPaid + amountPaying;

        string Name = lblStudentName.Text.Trim();
        string Term = lblTermName.Text.Trim();

       // strings for main message
        string parentName = txtbyname.Text.Trim();
        string Amount = txtAmount.Text.Trim();

        string categoryName = lblCategory.Text.Trim();
        string category = categoryName;
        string balance = newbalance.ToString();
        DateTime now = DateTime.Now;
        string date = now.ToString();
        string schoolName = lblSchoolName.Text;
        string Phone = lblParentContact.Text;
        string receipt = lblReceiptNumber.Text.Trim();

        string Message = "Dear " + parentName + " you have paid " + " K" + Amount + " to " + schoolName +" as " + category + " for " + Name + ",  for " + Term + " balance is K" + balance;   
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoice.aspx");
    }
    
    private void Payment_plan()
    {
        drpcashcheque.Items.Clear();
        drpcashcheque.Items.Add("");

        // Add predefined values here
        drpcashcheque.Items.Add("Cash");
        drpcashcheque.Items.Add("Bank Deposit");
        drpcashcheque.Items.Add("Mobile Money");
        drpcashcheque.Items.Add("Cheque");

    }

    //start here
    public DataTable  LoadFeesDetails()
    {
        string[] details = lblStudentID.Text.Split('|');

        string studentId = details[0];
        
      
        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"Select F.amountPaid, F.balance, 
        ST.[TermName], CONCAT(DATENAME(MONTH, ST.[Startdate]), ' ', YEAR(ST.[Enddate])) as Period From Fees as F
        Join SchoolTerm as ST on ST.TermId = F.TermId   where F.SchoolId = @schoolId and F.studentId = @studentId 
        and F.fees_Category = @category", connection);
            command.Parameters.AddWithValue("@category", lblCategoryId.Text);
            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadFee()
    {
        DataTable generalReport = LoadFeesDetails();
        if (generalReport.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in generalReport.Rows)
            {
                sb.Append("<tr>");
               
                sb.Append("<td>" + row["TermName"] +  " ("+  row["Period"] + " )" +  "</td>");
                sb.Append("<td>" + row["amountPaid"] + "</td>");
                sb.Append("<td>" + row["balance"] + "</td>");
                sb.Append("</tr>");
            }
            
            feesDetails.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            feesDetails.InnerHtml = "<tr><td colspan='3'>No data found.</td></tr>";
        }
    }

    //end here


    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            
            string existingRecordCheck = @"SELECT COUNT(*) FROM [Fees] WHERE studentId = @studentId 
                    AND TermId = @term AND fees_category = @feesCategory and [SchoolId] = @schoolId ";
            using (SqlCommand cmdCheck = new SqlCommand(existingRecordCheck, appconSQL2))
            {
                cmdCheck.Parameters.AddWithValue("@studentId", lblStudentID.Text.Trim());
                cmdCheck.Parameters.AddWithValue("@feesCategory", lblCategoryId.Text.Trim());
                cmdCheck.Parameters.AddWithValue("@term", lblTermId.Text.Trim());
                cmdCheck.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                int matchingRecords = Convert.ToInt32(cmdCheck.ExecuteScalar());

                if (matchingRecords > 0)
                {
                    //Update payments
                    try
                    {
                        //Start

                        LoadReceiptNo();

                        double currentBalance = Convert.ToDouble(lblBalance.Text.Trim());
                        double AmountPaid = Convert.ToDouble(lblAmountPaid.Text.Trim());

                        double amountPaying = Convert.ToDouble(txtAmount.Text.Trim());

                        double newbalance = currentBalance - amountPaying;
                        double newTotal = AmountPaid + amountPaying;


                        string ReceiptNo = lblReceiptNumber.Text.Trim();
                        string feesCategory = lblCategory.Text.Trim();


                        // Update  Fees Payments
                        string update = @"Update Fees set [FeesStatus] = @feesStatus, [amountPaid] = @amount,
                                [balance] = @balance where FeesId = @feesId";
                        using (SqlCommand cmdupdate = new SqlCommand(update, appconSQL2))
                        {
                            cmdupdate.Parameters.AddWithValue("@feesId", lblFeesId.Text.Trim());
                            if (newbalance > 0)
                            {
                                cmdupdate.Parameters.AddWithValue("@feesStatus", "Pending");
                            }
                            else
                            {
                                cmdupdate.Parameters.AddWithValue("@feesStatus", "Completed");
                            }
                            cmdupdate.Parameters.AddWithValue("@amount", newTotal);
                            cmdupdate.Parameters.AddWithValue("@balance", newbalance);
                            cmdupdate.ExecuteNonQuery();


                            // insert into FeesTransanctions

                            //check if transaction already saved first
                            string sql = @"  Select COUNT(*) from Payments where 
                              FeesId = @Id and 
                              Paid = @paid and
                              paymentMode = @mode and
                              datePaid = @datePaid";

                            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = lblFeesId.Text.Trim();
                                cmd.Parameters.Add("@paid", SqlDbType.VarChar).Value = amountPaying;
                                cmd.Parameters.Add("@mode", SqlDbType.VarChar).Value = drpcashcheque.Text.Trim();
                                cmd.Parameters.Add("@datePaid", SqlDbType.VarChar).Value = txtDate.Text.Trim();
                                
                                int count = (int)cmd.ExecuteScalar();

                                if (count > 0)
                                {
                                    lblError.Text = "Payment Already made, Please check!";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
                                    return;
                                }
                            }

                            string feeTransanction = @"Insert into Payments ([FeesId], [Paid], [Balance], 
                                                    [paymentMode], [paidBy], [paymentReference], [receipNumber], 
                                                    [datePaid], [postedBy], [dateRegistered], IsDeleted)
                                                    VALUES (@feesId, @amount, @balance, @mode, @paidby, @reference,
                                                    @receiptNumber, @datePaid, @registeredBy, GETDATE(), 0)";
                            using (SqlCommand cmdFee = new SqlCommand(feeTransanction, appconSQL2))
                            {
                                cmdFee.Parameters.AddWithValue("@feesId", lblFeesId.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@amount", amountPaying);
                                cmdFee.Parameters.AddWithValue("@balance", newbalance);
                                cmdFee.Parameters.AddWithValue("@mode", drpcashcheque.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@paidBy", txtbyname.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@reference", txtReference.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@receiptNumber", ReceiptNo);
                                cmdFee.Parameters.AddWithValue("@datePaid", txtDate.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@registeredBy", lblUser.Text.Trim());
                               
                                cmdFee.ExecuteNonQuery();

                                int transId = 0;

                                string sqlTrans = @"Select TransanctionId from Payments where 
                                        [receipNumber] = @receipt";
                                SqlCommand cmdTrans = new SqlCommand(sqlTrans, appconSQL2);
                                cmdTrans.Parameters.AddWithValue("@receipt", ReceiptNo);
                                SqlDataReader drTrans = cmdTrans.ExecuteReader();
                                while (drTrans.Read())
                                {
                                    transId = drTrans.GetInt32(0);
                                }
                                drTrans.Close();

                                if (lblParentContact.Text != null)
                                {
                                    Response.Redirect("Receipt.aspx?id=" + transId);
                                }
                                else
                                {
                                    Response.Redirect("Receipt.aspx?id=" + transId); 
                                }
                            }

                        }
                        //End here 
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = "Fees payment error" + ex;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error: " + ex;
        }
    }

    //Send sms
    private async Task<string> SendSmsAsync()
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("http://sms.cloud265.com");

            var request = new HttpRequestMessage(HttpMethod.Post, "/public/sms/9800");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "myApi");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //send here
            double currentBalance = Convert.ToDouble(lblBalance.Text.Trim());
            double AmountPaid = Convert.ToDouble(lblAmountPaid.Text.Trim());

            double amountPaying = Convert.ToDouble(txtAmount.Text.Trim());

            double newbalance = currentBalance - amountPaying;
            double newTotal = AmountPaid + amountPaying;

            string Name = lblStudentName.Text.Trim();
            string Term = lblTermName.Text.Trim();

            // strings for main message
            string parentName = txtbyname.Text.Trim();
            string Amount = txtAmount.Text.Trim();

            string categoryName = lblCategory.Text.Trim();
            string category = categoryName;
            string balance = newbalance.ToString();
            DateTime now = DateTime.Now;
            string date = now.ToString();
            string schoolName = lblSchoolName.Text;

            string receipt = lblReceiptNumber.Text.Trim();

            string Message = "Dear " + parentName + " you have paid " + " K" + Amount + " to " + schoolName + " as " + category + " for " + Name + ",  for " + Term + " balance is K" + balance;

            string theTo = lblParentContact.Text;
            string theMessage = Message;

            var json = "{ \"to\": \"" + theTo + "\", \"message\": \"" + theMessage + "\", \"from\": \"Deltacomm\" }";

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();

                string sql4 = @"Update sms set Used = Used + 1, 
                Balance = Balance - 1 where SchoolId = @schoolId";
                SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
                cmd4.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd4.ExecuteNonQuery();
                cmd4.Dispose();

                return "SMS sent successfully: " + res;
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                return "Failed to send SMS. Status:" + response.StatusCode + "<br/>Details:" + err;
            }
        }
    }
    protected void btnNo_Click(object sender, EventArgs e)
    {
        Response.Redirect("FeesPayment.aspx");
    }
}