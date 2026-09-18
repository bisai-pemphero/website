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

            lblFeesId.Text = id;

            LoadUsername();
            LoadDetails(id);
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

    public void LoadDetails(string id)
    {
        try
        {
            //get details
            string sql3 = @"SELECT S.[FirstName], S.Middlename, S.LastName, F.[studentId],
            F.TermId, F.fees_Category,  P.[PhoneNumber], F.[totalFees], F.[amountPaid],
            T.TermName, F.balance, FC.CategoryName 
			 FROM [Students] AS S
			join [StudentParent] as P on S.ParentID = P.ParentID 
			join Fees as F on F.[studentId] = S.StudentID 
			
            Join SchoolTerm as T on T.TermId = F.TermId
			Join FeesCategory as FC on FC.CategoryId = F.fees_Category WHERE F.FeesId = @transId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@transId", id);
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
                
                lblAmount.Text = "K" + dr3.GetDouble(7).ToString("N2");

                lblCategoryId.Text = dr3.GetString(5);
               
               
              
               
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
    
   
    
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //delete transaction

        try
        {
            // Delete  Fees Payments
                string feeTransanction = @"Delete from Payments where  FeesId = @Id";
                using (SqlCommand cmdFee = new SqlCommand(feeTransanction, appconSQL2))
                {
                    cmdFee.Parameters.AddWithValue("@Id", lblFeesId.Text.Trim());
                    cmdFee.ExecuteNonQuery();
                }

            string feesInvoice = @"Delete from Fees where FeesId = @Id";
            using (SqlCommand cmdFeeInvoice = new SqlCommand(feesInvoice, appconSQL2))
            {
                cmdFeeInvoice.Parameters.AddWithValue("@Id", lblFeesId.Text.Trim());
                cmdFeeInvoice.ExecuteNonQuery();
            }

            Reset();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Invoice Deleted Successfully!')", true);
        }
        catch (Exception ex)
        {
            lblError.Text = "Delete error" + ex.Message;
        }
    }




    public void Reset()
    {
          
        lblAmountPaid.Text = string.Empty;
        lblBalance.Text = string.Empty;
        lblTransId.Text = string.Empty;
        lblTermName.Text = string.Empty;
        lblTermId.Text = string.Empty;
        lblStudentName.Text = string.Empty;
        lblStudentID.Text = string.Empty;
        lblAmount.Text = string.Empty;
    }
}