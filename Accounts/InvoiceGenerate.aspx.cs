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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Encoder = System.Drawing.Imaging.Encoder;

public partial class InvoiceGenerate : System.Web.UI.Page
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
            LoadInvoiceNo();

            string initial = GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

            string id = Request.QueryString["SchoolId"];
            lblSchoolId.Text = id;

            LoadUsername();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = @"select Fullname from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
            }
            dr.Close();
            dr.Dispose();

            string sql2 = @"select School_name, Admin_email, Admin_name, School_address, PhoneNumber
                            from AllSchools where SchoolId = @schoolId";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            cmd2.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                lblSchoolName.Text = dr2.GetString(0);
                lblSchoolEmail.Text = dr2.GetString(1);
                lblSchoolAdmin.Text = dr2.GetString(2);
                lblSchoolAddress.Text = dr2.GetString(3);
                lblSchoolContact.Text = dr2.GetString(4);
            }
            dr2.Close();
            dr2.Dispose();
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

    public void LoadInvoiceNo()
    {
        string sql = "SELECT   [InvoiceNo] + 1 AS Next FROM   ValueSequence";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            string invoice = "INVOICE";
            lblInvoiceNo.Text = invoice + "-" + dr.GetInt32(0).ToString();
        }
        dr.Close();
        dr.Dispose();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string invoiceNo = lblInvoiceNo.Text.Trim();

        //Check if the username already exists
        string sql3 = @"SELECT * FROM [SchoolInvoice] WHERE [InvoiceNo]= @invoiceNo 
            and [SchoolId] = @schoolId and [Description] = @description and [Total] = @total";
        SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
        cmd3.Parameters.AddWithValue("@invoiceNo", invoiceNo);
        cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
        cmd3.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
        cmd3.Parameters.AddWithValue("@total", txtTotal.Text.Trim());
        SqlDataReader dr3 = cmd3.ExecuteReader();
        if (dr3.HasRows)
        {
            lblError.Text = "This Invoice already exists!";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);

            dr3.Close();
            dr3.Dispose();
            return;
        }
        else
        {
            dr3.Close();
            dr3.Dispose();

            double amount = Convert.ToDouble(txtTotal.Text.Trim());

            string sql5 = @"Insert into SchoolInvoice ([InvoiceNo], [Description], [SchoolId], 
                            [Total], [Paid], [Balance_Due], [Date], [Postedby])
                            VALUES (@invoiceNo, @description, @schoolId, @total, @paid, @balance,
                            GETDATE(), @user)";
            SqlCommand cmd5 = new SqlCommand(sql5, appconSQL2);
            cmd5.Parameters.AddWithValue("@invoiceNo", invoiceNo);
            cmd5.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
            cmd5.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            cmd5.Parameters.AddWithValue("@total", amount);
            cmd5.Parameters.AddWithValue("@paid", 0);
            cmd5.Parameters.AddWithValue("@balance", amount);
            cmd5.Parameters.AddWithValue("@user", lblUser.Text.Trim());
            cmd5.ExecuteNonQuery();
            cmd5.Dispose();


            //update site code in vsequnce
            string sql2 = "UPDATE ValueSequence SET    InvoiceNo = InvoiceNo + 1 WHERE ID = 1";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            cmd2.ExecuteNonQuery();
            cmd2.Dispose();

            //sendEmail();
            Reset();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Invoice Saved Successfully!')", true);
        }
    }

    public void Reset()
    {
        txtDescription.Text = string.Empty;
        txtTotal.Text = string.Empty;
        lblError.Text = string.Empty;
    }

    public void sendEmail()
    {
        string fromMail = "pempherobisai@gmail.com";
        string fromPassword = "tgwgtzvmvzcanuux";
        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);

        message.Subject = "SCHOOL LICENCE INVOICE";
        string mailAddress = lblSchoolEmail.Text.Trim();

        message.To.Add(new MailAddress(mailAddress));

        string client = lblSchoolAdmin.Text.Trim();
        string School = lblSchoolName.Text.Trim();
        string address = lblSchoolAddress.Text.Trim();
        string contact = lblSchoolContact.Text.Trim();
        string invoiceNo = lblInvoiceNo.Text.Trim();
        DateTime today = DateTime.Now;
        string description = txtDescription.Text.Trim();
        string total = "MWK" + txtTotal.Text.Trim();
        string emailTo = lblSchoolEmail.Text.Trim();


        message.Body = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; margin: 0; padding: 20px; background-color: #f9f9f9; }
            .invoice-container { max-width: 800px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
            .header { text-align: center; margin-bottom: 30px; border-bottom: 2px solid #3498db; padding-bottom: 20px; }
            .header h1 { color: #2c3e50; margin: 0; font-size: 28px; }
            .invoice-info {
            display: flex;
            justify-content: space-between;
            margin-bottom: 30px;
        }
        .info-column {
            flex: 1;
        }
        .info-column:first-child {
            text-align: left;
        }
        .info-column:last-child {
            text-align: right;
        }
        .info-item {
            margin-bottom: 8px;
        }
        .info-label {
            font-weight: bold;
            color: #2c3e50;
            display: block;
            margin-bottom: 3px;
            font-size: 16px;
        }
            .section { margin-bottom: 25px; }
            .section-title { font-weight: bold; font-size: 18px; color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 8px; margin-bottom: 15px; }
            .offering-item { margin-bottom: 8px; padding-left: 15px; position: relative; }
            .offering-item:before { content: '-'; position: absolute; left: 0; }
            table { width: 100%; border-collapse: collapse; margin-bottom: 25px; }
            th { text-align: left; padding: 12px 15px; border-bottom: 2px solid #3498db; background-color: #f8f9fa; color: #2c3e50; }
            td { padding: 12px 15px; border-bottom: 1px solid #eee; }
            tr:last-child td { border-bottom: 2px solid #3498db; }
            .total-row { font-weight: bold; background-color: #f8f9fa; }
            .barcode { text-align: center; margin: 30px 0; padding: 20px; background-color: #f8f9fa; border-radius: 8px; border: 1px dashed #ccc; }
            .barcode-number { font-family: 'Courier New', monospace; font-size: 24px; letter-spacing: 3px; margin: 15px 0; font-weight: bold; }
            .thank-you { text-align: center; margin-top: 30px; padding-top: 20px; border-top: 2px solid #3498db; font-style: italic; color: #2c3e50; font-size: 18px; }
            .contact { text-align: center; margin-top: 15px; font-size: 16px; color: #7f8c8d; }
            .footer { text-align: center; margin-top: 20px; color: #7f8c8d; font-size: 14px; }
            .logo { text-align: center; margin-bottom: 20px; }
            .logo img { max-width: 180px; height: auto; }
        </style>
    </head>
    <body>
        <div class='invoice-container'>
            <div class='logo'>
                <img src='https://apps.innosoftmw.com/myschool/img/logo.png' alt='Innosoft' />
            </div>
            
            <div class='header'>
                <h1>School Licence Invoice</h1>
            </div>

            <div class='invoice-info'>
                <div class='info-column'>
                    <div class='info-item'>
                       
                    </div>
                </div>
                <br/><br/>
                <div class='info-column'>
                    <div class='info-item'>
                        <span class='info-label'>DATE  " + today+ @"</span>                   
                    </div>
                </div>
            </div>

            <div class='section'>
                <div class='section-title'>Billed to</div>
                 <div class='offering-item'>" + client+@"<div>
                <div class='offering-item'>" + address+@"<div>
                <div class='offering-item'>"+contact+ @"</div>
                <div class='offering-item'>"+emailTo+ @"</div>
            </div>

            <table>
                <thead>
                    <tr >
                        
                        <th colspan='5'>Description</th>
                        <th>Amount</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan='5'>" + description+@"</td>
                        <td>"+total+@"</td>
                 
                    <tr class='total-row'>
                        <td colspan='5'>TOTAL</td>
                        <td>"+total+@"</td>
                    </tr>
                </tbody>
            </table>

            <div class='barcode'>
               
                <div class='info-label'> INVOICE # "+invoiceNo+@"</div>
            </div>

            <div class='thank-you'>
                <p>Thank you for your order!</p>
            </div>

            <div class='contact'>
                <p>If you have questions about your order, you can email us at pempherobisai@gmail.com.</p>
            </div>
            
            <div class='footer'>
                <p>Invoice For School System v2.0 | Generated on "+today+@"</p>
            </div>
        </div>
    </body>
    </html>";

        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        smtpClient.Send(message);
    }
}