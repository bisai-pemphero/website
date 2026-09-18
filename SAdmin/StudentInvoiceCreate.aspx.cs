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

public partial class StudentInvoiceCreate : System.Web.UI.Page
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
            LoadUsername();

            string initial = GetFirstLetters(lblUser.Text.Trim());
            lblInitials.Text = initial;

            LoadClasses();

            LoadAcademicYear();

            FeeCategory();
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
        }
        catch (Exception ex)
        {
            lblError.Text = "Username error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                    "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
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
            ddlClass.Items.Clear();
            ddlClass.Items.Add("");


            string sql = @"Select [ClassID], [ClassName] from [Classes] where [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ddlClass.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Class error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
            "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
        finally
        {
            //
        }
    }
    


    public void LoadAcademicYear()
    {
        try
        {
            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add("");
            string sql = @"SELECT [AcademicyearId],
                CONCAT(DATENAME(YEAR, [Start_year]), '-', YEAR([End_year])) as Academic_year
                 from [Academic_Year] where [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ddlAcademicYear.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " Academic Year");
            }

            dr.Close();


        }
        catch (Exception ex)
        {

            lblError.Text = "Load academic year error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                    "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Reset();
    }

    protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        SchoolTerm();
    }

    public void SchoolTerm()
    {
        try
        {
           
            string[] academicYear = ddlAcademicYear.Text.Split('|');
            string academicYearId = academicYear[0];

            ddlTerm.Items.Clear();
            ddlTerm.Items.Add("");

            string sql = @"SELECT [TermId], [TermName], CONCAT(DATENAME(MONTH, [Startdate]), 
            ' ', YEAR([Enddate])) as Period from [SchoolTerm] where [SchoolId] = @schoolId
             and [AcademicYearId] = @academicYearId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
            cmd.Parameters.AddWithValue("@academicYearId", academicYearId);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ddlTerm.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " (" + dr.GetString(2) + ")");
            }

            dr.Close();
        }
        catch (Exception ex)
        {

            lblError.Text = "Load academic year error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                    "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
    }

    public void Reset()
    {
        ddlAcademicYear.SelectedIndex = -1;
        ddlTerm.SelectedIndex = -1;
        ddlClass.SelectedIndex = -1;
        ddlCategory.SelectedIndex = -1;
    }

    public void FeeCategory()
    {
        try
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add("");

            string sql = @"Select [CategoryId], [CategoryName], [Amount] from [FeesCategory] where SchoolId = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ddlCategory.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + "-" + dr.GetDouble(2));
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


    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string[] category = ddlCategory.Text.Split('|');
            string[] categoryamount = ddlCategory.Text.Split('-');
            string[] term = ddlTerm.Text.Split('|');
            string[] className = ddlClass.Text.Split('|');
            
            string categoryId = category[0];
            double totalFees = Convert.ToDouble(categoryamount[1]);

            string termId = term[0];
            string schoolId = lblSchoolId.Text.Trim();
            string classId = className[0];


            //Insert into Student
            string sql4 = @"INSERT INTO Fees (
    [fees_Category], [TermId], [studentId], [SchoolId], [totalFees],
    [FeesStatus], [InitalPaymentDate], [amountPaid], [balance], [registeredBy],[dateRegistered]
)
SELECT 
    @feesCategory, @TermId, s.StudentID, @SchoolId, @TotalFees,
    @Feestatus, GETDATE(), @paid, @balance, @RegisteredBy, GETDATE()
    FROM Students s
    WHERE s.CurrentClassID = @class
    AND s.Status = @studentStatus 
    AND [IsDeleted] = @isdeleted         
    AND NOT EXISTS (           
    SELECT 1 
    FROM Fees f 
    WHERE f.studentId = s.StudentID 
    AND f.TermId = @TermId
    AND f.SchoolId = @SchoolId
    AND f.fees_Category = @feesCategory
)";
            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            cmd4.Parameters.AddWithValue("@feesCategory", categoryId);
            cmd4.Parameters.AddWithValue("@TermId", termId);
            cmd4.Parameters.AddWithValue("@SchoolId", schoolId);
            cmd4.Parameters.AddWithValue("@TotalFees", totalFees);
            cmd4.Parameters.AddWithValue("@Feestatus", "Pending");
            cmd4.Parameters.AddWithValue("@paid", 0);
            cmd4.Parameters.AddWithValue("@balance", totalFees);
            cmd4.Parameters.AddWithValue("@RegisteredBy", lblSession.Text.Trim());
            cmd4.Parameters.AddWithValue("@isdeleted", 0);
            cmd4.Parameters.AddWithValue("@studentStatus", "Active");
            cmd4.Parameters.AddWithValue("@class", classId);
            cmd4.ExecuteNonQuery();
            cmd4.Dispose();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccess", "showAlert('success', 'Student Invoice Generated Successfully!', 'StudentInvoice.aspx');", true);
        }
        catch (Exception ex)
        {
            lblError.Text = "Error: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
        finally
        {
            if (appconSQL2.State == ConnectionState.Open)
            {
                appconSQL2.Close();
            }
        }
    }
}