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
       

            double total = Convert.ToDouble(txtTotal.Text.Trim());

            string sql5 = @"Update sms set Purchased = @purchased, 
            Used = 0, Balance = @balance where SchoolId = @schoolId";
            SqlCommand cmd5 = new SqlCommand(sql5, appconSQL2);
            cmd5.Parameters.AddWithValue("@purchased", total);
            cmd5.Parameters.AddWithValue("@balance", total);
            cmd5.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());          
            cmd5.ExecuteNonQuery();
            cmd5.Dispose();

            Reset();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Sms Added Successfully!')", true);
        
    }

    public void Reset()
    {

        txtTotal.Text = string.Empty;
        lblError.Text = string.Empty;
    }

   
}