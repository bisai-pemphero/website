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
            string initial = GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

            string id = Request.QueryString["Id"];
            lblPaymentId.Text = id;

            LoadSchoolDetails(id);

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

    public void LoadSchoolDetails(string paymentId)
    {
        try
        {
            string sql2 = @"  SELECT AL.[School_name], AL.Admin_email, AL.[Admin_name], AL.[School_address],
            AL.[PhoneNumber], SI.Description, SI.Total, SI.InvoiceNo, SP.PaymentMode, SP.Reference
            FROM [AllSchools] as AL Join SchoolInvoice as SI on SI.SchoolId = AL.SchoolId
			Join SchoolPayments as SP on SI.InvoiceId = SP.InvoiceId
			WHERE SP.PaymentId = @paymentId";
            SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
            cmd2.Parameters.AddWithValue("@paymentId", paymentId);
            SqlDataReader dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                
                lblSchoolName.Text = dr2.GetString(0);
                lblSchoolEmail.Text = dr2.GetString(1);
                lblSchoolAdmin.Text = dr2.GetString(2);
                lblSchoolAddress.Text = dr2.GetString(3);
                lblSchoolContact.Text = dr2.GetString(4);
                txtDescription.Text = dr2.GetString(5);
                txtTotal.Text = dr2.GetDouble(6).ToString();
                lblInvoiceNo.Text = dr2.GetString(7);
                drpPaymentMode.Text = dr2.GetString(8);
                txtReference.Text = dr2.GetString(9);
            }
            dr2.Close();
            dr2.Dispose();
        }
        catch (Exception ex)
        {
            lblError.Text = "Load Payment Details Error" + ex;
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

   

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
           
            string sql4 = @"Update SchoolPayments set [PaymentMode] = @mode, 
            [Reference] = @reference where [PaymentId] = @paymentId";
            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            cmd4.Parameters.AddWithValue("@paymentId", lblPaymentId.Text.Trim());
            cmd4.Parameters.AddWithValue("@mode", drpPaymentMode.Text.Trim());
            cmd4.Parameters.AddWithValue("@reference", txtReference.Text.Trim());
            cmd4.ExecuteNonQuery();
            cmd4.Dispose();

            Reset();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Payment Updated Successfully!')", true);

        }
        catch (Exception ex)
        {
            lblError.Text = "Updating error: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }

    public void Reset()
    {
        txtDescription.Text = string.Empty;
        txtTotal.Text = string.Empty;
        lblError.Text = string.Empty;
        drpPaymentMode.SelectedIndex = -1;
        txtReference.Text = string.Empty;
    }


    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Payments.aspx");
    }
}