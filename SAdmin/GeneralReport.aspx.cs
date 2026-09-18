using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class Test : System.Web.UI.Page
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
            LoadAcademicYear();
        }
       
    }


    protected void drpTerm_SelectedIndexChanged(object sender, EventArgs e)
    {

      LoadTotal();
        LoadDet();
        LoadFeeCat();
        LoadPayMode();
        LoadbyClass();

        PettyData();
        LoadPettyCash();

        LoadCash();
        lblDateRange.Text = txtStart.Text + " to " + txtEnd.Text;
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

   

    private void LoadTotal()
    {
        try
        {
            string[] term = drpTerm.Text.Split('|');
            int termId = Convert.ToInt32(((string)term[0]).Trim());

            string sql = @"SELECT SUM(P.Paid) as Total 
            FROM Payments as P
            JOIN Fees as F on P.FeesId = F.FeesId
            where F.[SchoolId]=@schoolId AND CONVERT(DATE, P.datePaid) 
            Between CONVERT(DATE, @date1) and CONVERT(DATE, @date2) 
            and F.[TermId] = @termId and [IsDeleted] = 0 ";

            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                // Set parameters
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd.Parameters.AddWithValue("@date1", txtStart.Text.Trim());
                cmd.Parameters.AddWithValue("@date2", txtEnd.Text.Trim());
                cmd.Parameters.AddWithValue("@termId", termId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            double totalAmount = dr.IsDBNull(0) ? 0 : dr.GetDouble(0);
                            lblTotal.Text = "MK " + totalAmount.ToString("N2");
                        }
                    }
                    else
                    {
                        // No records found
                        lblTotal.Text = "MK 0.00";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Total amount error! " + ex.Message;
        }
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

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"
              SELECT S.FirstName, S.Middlename, S.LastName, FT.Paid as Amount_Paid,
              FC.CategoryName AS Fees_For, CN.ClassName AS Class, 
              FT.paymentMode, FT.paidBy, FT.[receipNumber], FT.[datePaid]
              FROM Payments AS FT 
			  JOIN Fees as F on F.FeesId = FT.FeesId
              JOIN Students AS S ON S.StudentID = F.studentId
              JOIN [SchoolTerm] AS T ON T.TermId = F.TermId
              JOIN FeesCategory AS FC ON FC.CategoryId = F.fees_Category
			  JOIN Classes AS CN ON CN.ClassID = S.CurrentClassID
              
              WHERE F.TermId = @termId AND S.SchoolId = @Id and FT.IsDeleted = 0
              AND CONVERT(DATE, FT.datePaid) BETWEEN CONVERT(DATE, @date1) 
              AND CONVERT(DATE, @date2)
              GROUP BY S.FirstName, S.Middlename, S.LastName, F.fees_Category,
              FT.paymentMode, FT.paidBy, S.CurrentClassID, FT.Paid, FC.CategoryName,
              CN.ClassName, FT.[receipNumber], FT.[datePaid] 
              ORDER BY S.CurrentClassID",
                connection);
            command.Parameters.AddWithValue("@date1", txtStart.Text);
            command.Parameters.AddWithValue("@date2", txtEnd.Text);
            command.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            command.Parameters.AddWithValue("@termId", termId);
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
                sb.Append("<tr>");
                sb.Append("<td>" + row["FirstName"] + " " + row["Middlename"] +  " " + row["Lastname"] + "</td>");
                sb.Append("<td>" + row["Amount_Paid"] + "</td>");
                sb.Append("<td>" + row["Fees_For"] + "</td>");
                sb.Append("<td>" + row["Class"] + "</td>");
                sb.Append("<td>" + row["paymentMode"] + "</td>");
                sb.Append("<td>" + row["paidBy"] + "</td>");
                sb.Append("<td>" + row["datePaid"] + "</td>");
                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            generalReportPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            generalReportPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }

    //Load Fees Category Report
    public DataTable LoadFeeCategoryReport()
    {
        string[] term = drpTerm.Text.Split('|');
        int termId = Convert.ToInt32(term[0]);

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"SELECT FC.CategoryName, 
            SUM(P.Paid) AS TotalAmountPaid
			FROM [Payments] as P 
			JOIN Fees as F on P.FeesId = F.FeesId
			JOIN FeesCategory as FC on F.fees_Category = FC.CategoryId 
            WHERE F.TermId = @termId AND f.SchoolId = @Id and P.IsDeleted = 0
            AND CONVERT(DATE, P.datePaid) BETWEEN CONVERT(DATE, @date1) 
            AND CONVERT(DATE, @date2)GROUP BY FC.CategoryName",
                connection);
            command.Parameters.AddWithValue("@date1", txtStart.Text);
            command.Parameters.AddWithValue("@date2", txtEnd.Text);
            command.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            command.Parameters.AddWithValue("@termId", termId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadFeeCat()
    {
        DataTable feeCategoryRe = LoadFeeCategoryReport();
        if (feeCategoryRe.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in feeCategoryRe.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["CategoryName"] + "</td>");
                sb.Append("<td>" + row["TotalAmountPaid"] + "</td>");

                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            FeeCategoryBreakdownPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            FeeCategoryBreakdownPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }


    //Load by payment mode
    public DataTable LoadFeesPaymentMode()
    {
        string[] term = drpTerm.Text.Split('|');
        int termId = Convert.ToInt32(term[0]);

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@" SELECT P.[paymentMode], 
            SUM(P.Paid) AS TotalAmountPaid 
			FROM [Payments] as P
			JOIN Fees as F on F.FeesId = P.FeesId
            WHERE F.TermId = @termId AND F.SchoolId = @Id AND IsDeleted = 0
            AND CONVERT(DATE, P.datePaid) BETWEEN CONVERT(DATE, @date1) 
            AND CONVERT(DATE, @date2)GROUP BY P.[paymentMode]",
                connection);
            command.Parameters.AddWithValue("@date1", txtStart.Text);
            command.Parameters.AddWithValue("@date2", txtEnd.Text);
            command.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            command.Parameters.AddWithValue("@termId", termId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadPayMode()
    {
        DataTable paymentMode = LoadFeesPaymentMode();
        if (paymentMode.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in paymentMode.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["paymentMode"] + "</td>");
                sb.Append("<td>" + row["TotalAmountPaid"] + "</td>");

                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            paymentBreakdownPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            paymentBreakdownPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }

    //load Fees by classes 
    public DataTable LoadFeesbyClass()
    {
        string[] term = drpTerm.Text.Split('|');
        int termId = Convert.ToInt32(term[0]);

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"Select C.ClassName, 
            sum(distinct P.Paid) as TotalAmountPaid from Classes as C 
            join Students as S
	        on C.ClassID = S.CurrentClassID 
	        JOIN Fees as F on F.studentId = S.StudentID 
			JOIN Payments as P on P.FeesId = F.FeesId 
	        where F.TermId = @termId and F.schoolId = @Id and 
               CONVERT(DATE, P.datePaid) BETWEEN CONVERT(DATE, @date1) 
            AND CONVERT(DATE, @date2) AND P.IsDeleted = 0 group by C.ClassName ",
                connection);
            command.Parameters.AddWithValue("@date1", txtStart.Text);
            command.Parameters.AddWithValue("@date2", txtEnd.Text);
            command.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            command.Parameters.AddWithValue("@termId", termId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadbyClass()
    {
        DataTable feesbyClass = LoadFeesbyClass();
        if (feesbyClass.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in feesbyClass.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["ClassName"] + "</td>");
                sb.Append("<td>" + row["TotalAmountPaid"] + "</td>");

                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            classBreakdownPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            classBreakdownPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }

    private void LoadPettyCash()
    {
        try
        {
            string[] term = drpTerm.Text.Split('|');
            int termId = Convert.ToInt32(((string)term[0]).Trim());

            string sql = @"Select Sum(Amount) from PettyCash where [TermId] = @termId and 
                        [SchoolId] = @schoolId and CONVERT(DATE, [Date]) Between 
                        CONVERT(DATE, @date1) and  CONVERT(DATE, @date2)";

            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                //Set parameters
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd.Parameters.AddWithValue("@date1", txtStart.Text.Trim());
                cmd.Parameters.AddWithValue("@date2", txtEnd.Text.Trim());
                cmd.Parameters.AddWithValue("@termId", termId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            double totalAmount = dr.IsDBNull(0) ? 0 : dr.GetDouble(0);
                            lblPettyCash.Text = "MK " + totalAmount.ToString("N2");
                        }
                    }
                    else
                    {
                        // No records found
                        lblPettyCash.Text = "MK 0.00";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Total Petty Cash error! " + ex.Message;
        }
    }
    public void LoadCash()
    {
        string totalCollected = lblTotal.Text.Trim();
        string cleanedCollected = new string(totalCollected.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());

        string pettyCash = lblPettyCash.Text.Trim();
        string cleanedPettyCash = new string(pettyCash.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());




        double collected;
        double petty;
        if (double.TryParse(cleanedCollected, out collected) && double.TryParse(cleanedPettyCash, out petty))
        {
            double cash;

            cash = collected - petty;
            lblCash.Text = "MK" + cash.ToString("N2");
        }
    }
    public DataTable LoadPettyCashData()
    {
        string[] term = drpTerm.Text.Split('|');
        int termId = Convert.ToInt32(term[0]);

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"
             Select [Description],[Amount] ,[Collectedby],  CONVERT(DATE, Date) as Date from [PettyCash] where 
             CONVERT(DATE, Date) BETWEEN CONVERT(DATE, @date1) AND CONVERT(DATE, @date2)
             AND [SchoolId] = @schoolId and [TermId] = @termId",
                connection);
            command.Parameters.AddWithValue("@date1", txtStart.Text);
            command.Parameters.AddWithValue("@date2", txtEnd.Text);
            command.Parameters.AddWithValue("@termId", termId);
            command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void PettyData()
    {
        DataTable pdata = LoadPettyCashData();
        if (pdata.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in pdata.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["Description"] + "</td>");
                sb.Append("<td>" + row["Amount"] + "</td>");
                sb.Append("<td>" + row["Collectedby"] + "</td>");
                sb.Append("<td>" + row["Date"] + "</td>");
                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            pettycashPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            pettycashPlaceholder.InnerHtml = "<tr><td colspan='7'>No data found.</td></tr>";
        }
    }
}
