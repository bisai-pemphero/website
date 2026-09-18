using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

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
    protected string ChartOverallClassData;
    protected string ChartOverallCategoryData;
    protected string ChartTodayClassData;
    protected string ChartTodayCategoryData;
    protected string ChartTodayPaymentData;

    //student class
    public string GenderLabels = "[]";
    public string GenderCounts = "[]";
    public string ClassLabels = "[]";
    public string ClassCounts = "[]";
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
            LoadOverallDetails();
            LoadTodayDetails();
            LoadCharts();
            LoadChartsStudent();
            LoadTotalStudents();
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
                WHERE f.SchoolId=@schoolId AND  t.IsActive = @status", con);
          cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@status", "True");
            SqlDataReader dr = cmd.ExecuteReader();
         
            if (dr.Read())
            {
                lblOverallExpected.Text = "K" + (dr["TotalExpected"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalExpected"])).ToString("N2");
                lblOverallCollected.Text = "K" + (dr["TotalCollected"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalCollected"])).ToString("N2");
                lblOverallBalance.Text = "K" + (dr["TotalBalance"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalBalance"])).ToString("N2");
            }
            dr.Close();
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
                AND p.IsDeleted=0 AND f.SchoolId= @schoolId", con);
                 cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            var result = cmd.ExecuteScalar();
             
            lblTodayCollected.Text = "K" + result != null ? "K" + result.ToString() : "0";
        }
    }

    private void LoadCharts()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();

            // Overall Class Distribution
            SqlCommand cmd = new SqlCommand(@"
                SELECT c.ClassName, SUM(f.amountPaid) AS TotalCollected
                FROM Fees f
                INNER JOIN Students s ON f.studentId = s.StudentID
                INNER JOIN Classes c ON s.CurrentClassID = c.ClassID
                INNER JOIN SchoolTerm t ON f.TermId = t.TermId
                WHERE f.SchoolId= @schoolId AND  t.IsActive = @status2
                GROUP BY c.ClassName ORDER BY c.ClassName", con);
                  cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@status2", "True");
            SqlDataReader dr = cmd.ExecuteReader();
            
            var classLabels = new List<string>();
            var classValues = new List<decimal>();
            var classColors = new List<string>();
            while (dr.Read())
            {
                classLabels.Add(dr["ClassName"].ToString());
                classValues.Add(Convert.ToDecimal(dr["TotalCollected"]));
                classColors.Add(GetRandomColor(classColors.Count));
            }
            dr.Close();

            var chartOverallClass = new
            {
                labels = classLabels,
                datasets = new[] {
                    new {
                        data = classValues,
                        backgroundColor = classColors,
                        borderColor = classColors.Select(c => c.Replace("0.7", "1")).ToArray(),
                        borderWidth = 2
                    }
                }
            };
            ChartOverallClassData = new JavaScriptSerializer().Serialize(chartOverallClass);

            // Overall Category Distribution
            cmd.CommandText = @"
                SELECT fc.CategoryName, SUM(f.amountPaid) AS TotalCollected
                FROM Fees f
                INNER JOIN FeesCategory fc ON f.fees_Category=fc.CategoryId
                INNER JOIN SchoolTerm t ON f.TermId=t.TermId
                WHERE f.SchoolId= @schoolId2 AND t.IsActive = @status
                GROUP BY fc.CategoryName ORDER BY fc.CategoryName";
           cmd.Parameters.AddWithValue("@schoolId2", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@status", "True");
            dr = cmd.ExecuteReader();
             
            var catLabels = new List<string>();
            var catValues = new List<decimal>();
            var catColors = new List<string>();
            while (dr.Read())
            {
                catLabels.Add(dr["CategoryName"].ToString());
                catValues.Add(Convert.ToDecimal(dr["TotalCollected"]));
                catColors.Add(GetRandomColor(catColors.Count));
            }
            dr.Close();

            var chartOverallCategory = new
            {
                labels = catLabels,
                datasets = new[] {
                    new {
                        data = catValues,
                        backgroundColor = catColors,
                        borderColor = catColors.Select(c => c.Replace("0.7", "1")).ToArray(),
                        borderWidth = 2
                    }
                }
            };
            ChartOverallCategoryData = new JavaScriptSerializer().Serialize(chartOverallCategory);

            // Today's Class Distribution
            cmd.CommandText = @"
                SELECT c.ClassName, SUM(p.Paid) AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                INNER JOIN Students s ON f.studentId=s.StudentID
                INNER JOIN Classes c ON s.CurrentClassID=c.ClassID
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId= @schoolId3
                GROUP BY c.ClassName ORDER BY c.ClassName";
               
              cmd.Parameters.AddWithValue("@schoolId3", lblSchoolId.Text.Trim());
            dr = cmd.ExecuteReader();
             
            var todayClassLabels = new List<string>();
            var todayClassValues = new List<decimal>();
            var todayClassColors = new List<string>();
            while (dr.Read())
            {
                todayClassLabels.Add(dr["ClassName"].ToString());
                todayClassValues.Add(Convert.ToDecimal(dr["TotalCollected"]));
                todayClassColors.Add(GetRandomColor(todayClassColors.Count));
            }
            dr.Close();

            var chartTodayClass = new
            {
                labels = todayClassLabels,
                datasets = new[] {
                    new {
                        data = todayClassValues,
                        backgroundColor = todayClassColors,
                        borderColor = todayClassColors.Select(c => c.Replace("0.7", "1")).ToArray(),
                        borderWidth = 2
                    }
                }
            };
            ChartTodayClassData = new JavaScriptSerializer().Serialize(chartTodayClass);

            // Today's Category Distribution
            cmd.CommandText = @"
                SELECT fc.CategoryName, SUM(p.Paid) AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                INNER JOIN FeesCategory fc ON f.fees_Category=fc.CategoryId
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId= @schoolId4
                GROUP BY fc.CategoryName ORDER BY fc.CategoryName";
                cmd.Parameters.AddWithValue("@schoolId4", lblSchoolId.Text.Trim());

            dr = cmd.ExecuteReader();
              
            var todayCatLabels = new List<string>();
            var todayCatValues = new List<decimal>();
            var todayCatColors = new List<string>();
            while (dr.Read())
            {
                todayCatLabels.Add(dr["CategoryName"].ToString());
                todayCatValues.Add(Convert.ToDecimal(dr["TotalCollected"]));
                todayCatColors.Add(GetRandomColor(todayCatColors.Count));
            }
            dr.Close();

            var chartTodayCategory = new
            {
                labels = todayCatLabels,
                datasets = new[] {
                    new {
                        data = todayCatValues,
                        backgroundColor = todayCatColors,
                        borderColor = todayCatColors.Select(c => c.Replace("0.7", "1")).ToArray(),
                        borderWidth = 2
                    }
                }
            };
            ChartTodayCategoryData = new JavaScriptSerializer().Serialize(chartTodayCategory);

            // Today's Payment Methods
            cmd.CommandText = @"
                SELECT p.paymentMode, SUM(p.Paid) AS TotalCollected
                FROM Payments p
                INNER JOIN Fees f ON p.FeesId=f.FeesId
                WHERE CAST(p.datePaid AS DATE)=CAST(GETDATE() AS DATE)
                  AND p.IsDeleted=0 AND f.SchoolId= @schoolId5
                GROUP BY p.paymentMode ORDER BY p.paymentMode";
                 cmd.Parameters.AddWithValue("@schoolId5", lblSchoolId.Text.Trim());

            dr = cmd.ExecuteReader();
             
            var paymentLabels = new List<string>();
            var paymentValues = new List<decimal>();
            var paymentColors = new List<string>();
            while (dr.Read())
            {
                paymentLabels.Add(dr["paymentMode"].ToString());
                paymentValues.Add(Convert.ToDecimal(dr["TotalCollected"]));
                paymentColors.Add(GetRandomColor(paymentColors.Count));
            }
            dr.Close();

            var chartTodayPayment = new
            {
                labels = paymentLabels,
                datasets = new[] {
                    new {
                        data = paymentValues,
                        backgroundColor = paymentColors,
                        borderColor = paymentColors.Select(c => c.Replace("0.7", "1")).ToArray(),
                        borderWidth = 2
                    }
                }
            };
            ChartTodayPaymentData = new JavaScriptSerializer().Serialize(chartTodayPayment);
        }
    }

    private string GetRandomColor(int index)
    {
        string[] colors = {
            "rgba(56, 189, 248, 0.7)",   // Blue
            "rgba(34, 197, 94, 0.7)",    // Green
            "rgba(249, 115, 22, 0.7)",   // Orange
            "rgba(168, 85, 247, 0.7)",   // Purple
            "rgba(239, 68, 68, 0.7)",    // Red
            "rgba(234, 179, 8, 0.7)",    // Yellow
            "rgba(14, 165, 233, 0.7)",   // Light Blue
            "rgba(20, 184, 166, 0.7)",   // Teal
            "rgba(139, 92, 246, 0.7)",   // Indigo
            "rgba(236, 72, 153, 0.7)"    // Pink
        };
        return colors[index % colors.Length];
    }


    private void LoadChartsStudent()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Students by Gender
                SqlCommand cmd = new SqlCommand(@"
                    SELECT Gender, COUNT(StudentID) AS Total
                    FROM Students
                    WHERE IsDeleted=0
                    AND SchoolId = @schoolId
                    GROUP BY Gender", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

                DataTable dt = new DataTable();
                da.Fill(dt);

                StringBuilder genderLabels = new StringBuilder("[");
                StringBuilder genderCounts = new StringBuilder("[");

                foreach (DataRow row in dt.Rows)
                {
                    genderLabels.Append("'" + row["Gender"].ToString() + "',");
                    genderCounts.Append(row["Total"].ToString() + ",");
                }

                if (dt.Rows.Count > 0)
                {
                    genderLabels.Length--; genderCounts.Length--;
                }

                genderLabels.Append("]");
                genderCounts.Append("]");

                GenderLabels = genderLabels.ToString();
                GenderCounts = genderCounts.ToString();

                // Students by Class
                cmd = new SqlCommand(@"
                    SELECT c.ClassName, COUNT(s.StudentID) AS Total
                    FROM Students s
                    JOIN Classes c ON s.CurrentClassID = c.ClassID
                    WHERE s.IsDeleted = 0
                    AND s.SchoolId = @schoolId
                    GROUP BY c.ClassName
                    ORDER BY c.ClassName", con);

                da = new SqlDataAdapter(cmd);
                  cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

                dt = new DataTable();
                da.Fill(dt);

                StringBuilder classLabels = new StringBuilder("[");
                StringBuilder classCounts = new StringBuilder("[");

                foreach (DataRow row in dt.Rows)
                {
                    classLabels.Append("'" + row["ClassName"].ToString() + "',");
                    classCounts.Append(row["Total"].ToString() + ",");
                }

                if (dt.Rows.Count > 0)
                {
                    classLabels.Length--; classCounts.Length--;
                }

                classLabels.Append("]");
                classCounts.Append("]");

                ClassLabels = classLabels.ToString();
                ClassCounts = classCounts.ToString();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    private void LoadTotalStudents()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Active Students
                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) FROM Students WHERE 
                IsDeleted=0 AND Status='Active' and SchoolId= @schoolId", con);
                  cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

                lblStudentsTotal.Text = cmd.ExecuteScalar().ToString();

            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
}