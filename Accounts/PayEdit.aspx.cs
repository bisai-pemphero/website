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

            lblTransId.Text = id;

            LoadUsername();

            LoadDetails();

            LoadFee();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = @"select Fullname, SchoolId from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
                lblSchoolId.Text = dr.GetInt32(1).ToString();
            }
            dr.Close();
            dr.Dispose();

            //string sql1 = @"  Select [SenderId], [ApiKey] from [SmsSenderDetails] where [SchoolId] = @schoolId";
            //SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
            //cmd1.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            //SqlDataReader dr1 = cmd1.ExecuteReader();
            //while (dr1.Read())
            //{
            //    lblSenderId.Text = dr1.GetString(0);
            //    lblapiKey.Text = dr1.GetString(1);
            //}
            //dr1.Close();
            //dr1.Dispose();

           
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

            //get details
            string sql3 = @"   SELECT S.[FirstName], S.Middlename, S.LastName, F.[studentId],
            F.TermId, F.fees_Category,  P.[PhoneNumber], F.[totalFees], F.[amountPaid],
            T.TermName, F.balance, FC.CategoryName, PC.Paid,
            PC.paidBy, PC.paymentMode, PC.paymentReference, 
            PC.datePaid, F.FeesId
			 FROM [Students] AS S
			join [StudentParent] as P on S.ParentID = P.ParentID 
			join Fees as F on F.[studentId] = S.StudentID 
			Join Payments as PC on PC.FeesId =   F.FeesId
            Join SchoolTerm as T on T.TermId = F.TermId
			Join FeesCategory as FC on FC.CategoryId = F.fees_Category WHERE PC.TransanctionId = @transId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@transId", lblTransId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                lblStudentName.Text = dr3.GetString(0) + " " + dr3.GetString(1) + " " + dr3.GetString(2);
                lblStudentID.Text = dr3.GetString(3);
                lblTermId.Text = dr3.GetString(4);
                lblCategory.Text = dr3.GetString(11);
                lblParentContact.Text = dr3.GetString(6);
                lblTotalFees.Text = dr3.GetDouble(7).ToString();
                lblAmountPaid.Text = dr3.GetDouble(8).ToString();
                lblTermName.Text = dr3.GetString(9);
                lblBalance.Text = dr3.GetDouble(10).ToString();
                
                lblShowBalance.Text = "K" + dr3.GetDouble(10).ToString("N2");

                lblCategoryId.Text = dr3.GetString(5);
                txtAmount.Text = dr3.GetDouble(12).ToString();
                lblReversedAmount.Text = dr3.GetDouble(12).ToString();
                txtbyname.Text = dr3.GetString(13);
                drpcashcheque.Text = dr3.GetString(14);
                txtReference.Text = dr3.GetString(15);
              
                lblFeesId.Text = dr3.GetInt32(17).ToString();
            }

            dr3.Close();
            dr3.Dispose(); 
        }
        catch(Exception ex)
        {
            lblError.Text = "Load details error: " + ex;
        }
    }

   

   
   
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoice.aspx");
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
                    //Update payments
                    try
                    {
                        //Start

                        double currentBalance = Convert.ToDouble(lblBalance.Text.Trim());
                       
                        double AmountPaid = Convert.ToDouble(lblAmountPaid.Text.Trim());
                        double ReversedAmount = Convert.ToDouble(lblReversedAmount.Text.Trim());
                        double amountPaying = Convert.ToDouble(txtAmount.Text.Trim());

                        double balance = currentBalance + ReversedAmount;
                       double oldAmount = AmountPaid - ReversedAmount;

                        double newbalance = balance - amountPaying;
                        double newTotal = oldAmount + amountPaying;

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

                           
                            string feeTransanction = @"Update Payments set [Paid] =  @amount, [Balance] = @balance, 
                                                    [paymentMode] = @mode, [paidBy] = @paidby, 
                                                    [paymentReference] = @reference, [datePaid] = @datePaid 
                                                    where [TransanctionId] = @Id";
                            using (SqlCommand cmdFee = new SqlCommand(feeTransanction, appconSQL2))
                            {
                                cmdFee.Parameters.AddWithValue("@Id", lblTransId.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@amount", amountPaying);
                                cmdFee.Parameters.AddWithValue("@balance", newbalance);
                                cmdFee.Parameters.AddWithValue("@mode", drpcashcheque.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@paidBy", txtbyname.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@reference", txtReference.Text.Trim());
                                cmdFee.Parameters.AddWithValue("@datePaid", txtDate.Text.Trim());
                                cmdFee.ExecuteNonQuery();
                                
                                int transId = Convert.ToInt32(lblTransId.Text.Trim());

                                if (lblParentContact.Text != null && lblapiKey.Text != null && lblSenderId.Text != null)
                                {
                                   // SendSmsNotification();
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
        catch (Exception ex)
        {
            lblError.Text = "Saving error: " + ex;
        } 
    }
            
    protected void btnNo_Click(object sender, EventArgs e)
    {
        Response.Redirect("FeesPayment.aspx");
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //delete transaction

        try
        {
            double currentBalance = Convert.ToDouble(lblBalance.Text.Trim());

            double AmountPaid = Convert.ToDouble(lblAmountPaid.Text.Trim());
            double ReversedAmount = Convert.ToDouble(lblReversedAmount.Text.Trim());
            double amountPaying = Convert.ToDouble(txtAmount.Text.Trim());

            double balance = currentBalance + ReversedAmount;
            double oldAmount = AmountPaid - ReversedAmount;


            // Update  Fees Payments
            string update = @"Update Fees set [FeesStatus] = @feesStatus, [amountPaid] = @amount,
                                [balance] = @balance where FeesId = @feesId";
            using (SqlCommand cmdupdate = new SqlCommand(update, appconSQL2))
            {
                cmdupdate.Parameters.AddWithValue("@feesId", lblFeesId.Text.Trim());
                if (balance > 0)
                {
                    cmdupdate.Parameters.AddWithValue("@feesStatus", "Pending");
                }
                else
                {
                    cmdupdate.Parameters.AddWithValue("@feesStatus", "Completed");
                }
                cmdupdate.Parameters.AddWithValue("@amount", oldAmount);
                cmdupdate.Parameters.AddWithValue("@balance", balance);
                cmdupdate.ExecuteNonQuery();


                string feeTransanction = @"Update Payments set IsDeleted = 1 where [TransanctionId] = @Id";
                using (SqlCommand cmdFee = new SqlCommand(feeTransanction, appconSQL2))
                {
                    cmdFee.Parameters.AddWithValue("@Id", lblTransId.Text.Trim());
                    cmdFee.ExecuteNonQuery();
                }
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Payment Deleted Successfully!')", true);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Delete error" + ex.Message;
        }
    }

    public void Reset()
    {
        txtAmount.Text = string.Empty;
        txtbyname.Text = string.Empty;
        txtDate.Text = string.Empty;
        txtReference.Text = string.Empty;

        drpcashcheque.SelectedIndex = -1;
        lblAmountPaid.Text = string.Empty;
        lblBalance.Text = string.Empty;
        lblTransId.Text = string.Empty;
        lblTermName.Text = string.Empty;
        lblTermId.Text = string.Empty;
        lblStudentName.Text = string.Empty;
        lblStudentID.Text = string.Empty;
        lblShowBalance.Text = string.Empty;
    }
}