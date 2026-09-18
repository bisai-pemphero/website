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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Xml.Linq;
using Encoder = System.Drawing.Imaging.Encoder;

public partial class StudentInvoice : System.Web.UI.Page
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

    string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
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

            string initial = GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

            LoadAcademicYear();
            LoadClasses();
            FeeCategory();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "Select Fullname, SchoolId from Users where Username = @username";
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

    public void LoadClasses()
    {
        try
        {
            drpClass.Items.Clear();
            drpClass.Items.Add("");

            string sql = @"Select [ClassID], [ClassName] from [Classes] where [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpClass.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Class error" + ex;
        }
        finally
        {
            //
        }
    }

    public void FeeCategory()
    {
        try
        {
            drpCategory.Items.Clear();
            drpCategory.Items.Add("");

            string sql = @"Select [CategoryId], [CategoryName], [Amount] from [FeesCategory] where SchoolId = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpCategory.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + "-" + dr.GetDouble(2));
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Load Fees Category error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                    "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
    }

    protected void btnBatch_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoiceCreate.aspx");
    }

    public void LoadStudents()
    {

        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(@"Select F.FeesId, 
    S.FirstName, S.Middlename ,S.LastName, 
    ST.[TermName], C.ClassName,    
	CONCAT(DATENAME(YEAR, AY.[Start_year]), '-', YEAR(AY.[End_year])) as Academic_year,
	FC.CategoryName, 
	F.totalFees,
	F.amountPaid, 
	F.balance,
    S.StudentID
    from Fees as F
  Join SchoolTerm as ST on F.TermId = ST.[TermId]
  Join [Academic_Year] as AY on ST.[AcademicYearId] = AY.AcademicyearId
  Join Students as S on F.studentId = S.StudentID
  Join Classes as C on C.ClassID = S.CurrentClassID
  Join FeesCategory as FC on FC.CategoryId = F.fees_Category
    where F.balance <= 0 and F.SchoolId = @schoolId", connection);
            command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string Id = reader["FeesId"].ToString();
                string studentId = reader["StudentID"].ToString();

                Response.Write("<tr>");
                Response.Write("<td>" + reader["FirstName"] + "</td>");
                Response.Write("<td>" + reader["Middlename"] + "</td>");
                Response.Write("<td>" + reader["LastName"] + "</td>");
                Response.Write("<td>" + reader["ClassName"] + "</td>");
                Response.Write("<td>" + reader["TermName"] + " (" + reader["Academic_year"] + " )" + "</td>");
                Response.Write("<td>" + reader["CategoryName"] + "</td>");
                Response.Write("<td>" + reader["totalFees"] + "</td>");
                Response.Write("<td>" + reader["amountPaid"] + "</td>");
                Response.Write("<td>" + reader["balance"] + "</td>");
                Response.Write("<td>");

                Response.Write("<a href='Pay.aspx?id=" + Id + "' class='btn btn-warning'>");
                Response.Write("<i class='fas fa-wallet'></i>Pay</a>");
                Response.Write("</td>");
                Response.Write("<td>");
                Response.Write("<a href='StudentViewPayments.aspx?id=" + studentId + "' class='btn btn-info'>");
                Response.Write("<i class='fas fa-eye'></i>View</a>");

                Response.Write("</td>");
                Response.Write("</tr>");
            }

            reader.Close();
        }
    }


    protected void btnEditInvoice_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoiceEdit.aspx");
    }

    public void LoadAcademicYear()
    {
        try
        {
            drpAcademicYear.Items.Clear();
            drpAcademicYear.Items.Add("");
            string sql = "SELECT [AcademicyearId], " +
                "CONCAT(DATENAME(YEAR, [Start_year]), '-', YEAR([End_year])) as Academic_year" +
                " from [Academic_Year] where" +
                " [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpAcademicYear.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " Academic Year");
            }

            dr.Close();


        }
        catch (Exception ex)
        {
            lblError.Text = "Load academic year error" + ex;
        }
    }

    public void LoadAllTerms()
    {
        string[] academic = drpAcademicYear.Text.Split('|');
        int academicId = Convert.ToInt32(academic[0]);

        drpTerm.Items.Clear();
        drpTerm.Items.Add("");

        string sql = "SELECT [TermId], [TermName], " +
            "CONCAT(DATENAME(MONTH, [Startdate]), ' ', YEAR([Enddate])) as Period" +
            " from [SchoolTerm] where [SchoolId] = @schoolId and [AcademicYearId] = @academicYearId";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
        cmd.Parameters.AddWithValue("@academicYearId", academicId);
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            drpTerm.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " (" + dr.GetString(2) + ")");
        }

        dr.Close();
    }

    protected void drpAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        //load all terms
        LoadAllTerms();
        drpTerm.Focus();
    }




    //Load general Report
    public DataTable LoadDetailedReport()
    {
        string[] term = drpTerm.Text.Split('|');
        int termId = Convert.ToInt32(term[0]);

        string[] studentClass = drpClass.Text.Split('|');
        int classId = Convert.ToInt32(studentClass[0]);

        string[] feesCategory = drpCategory.Text.Split('|');
        int categoryId = Convert.ToInt32(feesCategory[0]);

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"Select F.FeesId, 
    S.FirstName, S.Middlename ,S.LastName, 
    ST.[TermName], C.ClassName,    
	CONCAT(DATENAME(YEAR, AY.[Start_year]), '-', YEAR(AY.[End_year])) as Academic_year,
	FC.CategoryName, 
	F.totalFees,
    S.StudentID
    from Fees as F
  Join SchoolTerm as ST on F.TermId = ST.[TermId]
  Join [Academic_Year] as AY on ST.[AcademicYearId] = AY.AcademicyearId
  Join Students as S on F.studentId = S.StudentID
  Join Classes as C on C.ClassID = S.CurrentClassID
  Join FeesCategory as FC on FC.CategoryId = F.fees_Category
  Where F.balance <= 0 and F.SchoolId = @schoolId and F.TermId = @termId
 and F.fees_Category = @feesCategory and S.CurrentClassID = @class",
                connection);
       
            command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            command.Parameters.AddWithValue("@termId", termId);
            command.Parameters.AddWithValue("@feesCategory", categoryId);
            command.Parameters.AddWithValue("@class", classId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadDet()
    {
        DataTable generalReport = LoadDetailedReport();
        if (generalReport.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in generalReport.Rows)
            {
                string studentId = row["StudentID"].ToString();

                sb.Append("<tr>");
                sb.Append("<td>" + row["FirstName"]  + "</td>");
                sb.Append("<td>" + row["Middlename"] +  "</td>");
                sb.Append("<td>" + row["Lastname"] + "</td>");
                sb.Append("<td>" + row["TermName"] + " (" + row["Academic_year"] + " )"+ "</td>");
                sb.Append("<td>" + row["ClassName"] + "</td>");
                sb.Append("<td>" + row["CategoryName"] + "</td>");
                sb.Append("<td>" + row["totalFees"] + "</td>");

                sb.Append("<td>");
                sb.Append("<a href='StudentViewPayments.aspx?id=" + studentId + "' class='btn btn-info'>");
                sb.Append("<i class='fas fa-eye'></i>View</a>");
                sb.Append("</td>");

                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            paidFeesPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            paidFeesPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        LoadDet();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        //reset filters
        drpAcademicYear.SelectedIndex = -1;
        drpCategory.SelectedIndex = -1;
        drpClass.SelectedIndex = -1;
        drpTerm.Items.Clear();

        paidFeesPlaceholder.InnerHtml = "<tr><td colspan='8'>No data found.</td></tr>";
    }
}