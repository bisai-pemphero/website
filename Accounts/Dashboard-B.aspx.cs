using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class Dashboard : System.Web.UI.Page
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


    private string connStr = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
    protected string ChartOverallCategoryData;
    protected string ChartTodayCategoryData;
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
            LoadUsername();

           string initial =  GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

          
            LoadOverallDetails();
            LoadTodayDetails();
            LoadCharts();

        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "select Fullname, SchoolId from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
                lblSchoolId.Text = dr.GetInt32(1).ToString();
            }

            dr.Close();

            

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

    

    private void LoadOverallDetails()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            // Overall totals
            SqlCommand cmd = new SqlCommand(@"
                SELECT FORMAT(SUM(f.totalFees),'N0') AS TotalExpected,
                       FORMAT(SUM(f.amountPaid),'N0') AS TotalCollected,
                       FORMAT(SUM(f.balance),'N0') AS TotalBalance
                FROM Fees f
                INNER JOIN SchoolTerm t ON f.TermId = t.TermId
                WHERE t.IsActive='True' AND f.SchoolId=1", con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                lblOverallExpected.Text = dr["TotalExpected"].ToString();
                lblOverallCollected.Text = dr["TotalCollected"].ToString();
                lblOverallBalance.Text = dr["TotalBalance"].ToString();
            }
            dr.Close();

            // Class-wise
            cmd.CommandText = @"
                SELECT c.ClassName, FORMAT(SUM(f.totalFees),'N0') AS TotalExpected,
                       FORMAT(SUM(f.amountPaid),'N0') AS TotalCollected,
                       FORMAT(SUM(f.balance),'N0') AS TotalBalance
                FROM Fees f
                INNER JOIN Students s ON f.studentId = s.StudentID
                INNER JOIN Classes c ON s.CurrentClassID = c.ClassID
                INNER JOIN SchoolTerm t ON f.TermId = t.TermId
                WHERE t.IsActive= 'True' AND f.SchoolId=1
                GROUP BY c.ClassName ORDER BY c.ClassName";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtClass = new DataTable();
            da.Fill(dtClass);
            gvOverallByClass.DataSource = dtClass;
            gvOverallByClass.DataBind();

            // Category-wise
            cmd.CommandText = @"
                SELECT fc.CategoryName, FORMAT(SUM(f.totalFees),'N0') AS TotalExpected,
                       FORMAT(SUM(f.amountPaid),'N0') AS TotalCollected,
                       FORMAT(SUM(f.balance),'N0') AS TotalBalance
                FROM Fees f
                INNER JOIN FeesCategory fc ON f.fees_Category = fc.CategoryId
                INNER JOIN SchoolTerm t ON f.TermId = t.TermId
                WHERE t.IsActive= 'True' AND f.SchoolId=1
                GROUP BY fc.CategoryName ORDER BY fc.CategoryName";
            da = new SqlDataAdapter(cmd);
            DataTable dtCategory = new DataTable();
            da.Fill(dtCategory);
            gvOverallByCategory.DataSource = dtCategory;
            gvOverallByCategory.DataBind();
        }
    }

    private void LoadTodayDetails()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(@"
                SELECT FORMAT(SUM(p.Paid),'N0') AS TotalCollectedToday
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                AND p.IsDeleted=0 AND f.SchoolId=1", con);
            lblTodayCollected.Text = cmd.ExecuteScalar().ToString();

            // Class-wise today
            cmd.CommandText = @"
                SELECT c.ClassName, FORMAT(SUM(p.Paid),'N0') AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                INNER JOIN Students s ON f.studentId=s.StudentID
                INNER JOIN Classes c ON s.CurrentClassID=c.ClassID
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId=1
                GROUP BY c.ClassName ORDER BY c.ClassName";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtClass = new DataTable();
            da.Fill(dtClass);
            gvTodayByClass.DataSource = dtClass;
            gvTodayByClass.DataBind();

            // Category-wise today
            cmd.CommandText = @"
                SELECT fc.CategoryName, FORMAT(SUM(p.Paid),'N0') AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                INNER JOIN FeesCategory fc ON f.fees_Category=fc.CategoryId
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId=1
                GROUP BY fc.CategoryName ORDER BY fc.CategoryName";
            da = new SqlDataAdapter(cmd);
            DataTable dtCat = new DataTable();
            da.Fill(dtCat);
            gvTodayByCategory.DataSource = dtCat;
            gvTodayByCategory.DataBind();

            // Payment Mode today
            cmd.CommandText = @"
                SELECT p.paymentMode, FORMAT(SUM(p.Paid),'N0') AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId=1
                GROUP BY p.paymentMode ORDER BY p.paymentMode";
            da = new SqlDataAdapter(cmd);
            DataTable dtPay = new DataTable();
            da.Fill(dtPay);
            gvTodayByPayment.DataSource = dtPay;
            gvTodayByPayment.DataBind();
        }
    }

    private void LoadCharts()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(@"
                SELECT fc.CategoryName, SUM(f.amountPaid) AS TotalCollected
                FROM Fees f
                INNER JOIN FeesCategory fc ON f.fees_Category=fc.CategoryId
                INNER JOIN SchoolTerm t ON f.TermId=t.TermId
                WHERE t.IsActive='True' AND f.SchoolId=1
                GROUP BY fc.CategoryName ORDER BY fc.CategoryName", con);
            SqlDataReader dr = cmd.ExecuteReader();
            var categories = new List<string>();
            var values = new List<decimal>();
            while (dr.Read()) { categories.Add(dr["CategoryName"].ToString()); values.Add(Convert.ToDecimal(dr["TotalCollected"])); }
            dr.Close();
            var chartOverall = new { labels = categories, datasets = new[] { new { label = "Collected", data = values } } };
            ChartOverallCategoryData = new JavaScriptSerializer().Serialize(chartOverall);

            cmd.CommandText = @"
                SELECT fc.CategoryName, SUM(p.Paid) AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                INNER JOIN FeesCategory fc ON f.fees_Category=fc.CategoryId
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId=1
                GROUP BY fc.CategoryName ORDER BY fc.CategoryName";
            dr = cmd.ExecuteReader();
            categories.Clear(); values.Clear();
            while (dr.Read()) { categories.Add(dr["CategoryName"].ToString()); values.Add(Convert.ToDecimal(dr["TotalCollected"])); }
            dr.Close();
            var chartToday = new { labels = categories, datasets = new[] { new { label = "Collected Today", data = values } } };
            ChartTodayCategoryData = new JavaScriptSerializer().Serialize(chartToday);
        }
    }
}