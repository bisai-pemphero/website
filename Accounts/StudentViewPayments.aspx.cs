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

            string id = Request.QueryString["id"];
            lblFeesId.Text = id;

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


   

    public void LoadStudents()
    {

        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(@"Select  
    S.FirstName, S.Middlename ,S.LastName, 
    ST.[TermName], C.ClassName,    
	CONCAT(DATENAME(YEAR, AY.[Start_year]), '-', YEAR(AY.[End_year])) as Academic_year,
	FC.CategoryName, 
	P.Paid,
	P.Balance,
	P.paymentMode,
	P.dateRegistered,
    P.TransanctionId from Payments as P
	Join Fees AS F on P.FeesId = F.FeesId
  Join SchoolTerm as ST on F.TermId = ST.[TermId]
  Join [Academic_Year] as AY on ST.[AcademicYearId] = AY.AcademicyearId
  Join Students as S on F.studentId = S.StudentID
  Join Classes as C on C.ClassID = S.CurrentClassID
  Join FeesCategory as FC on FC.CategoryId = F.fees_Category
  WHERE S.StudentID = @Id and P.IsDeleted = 0", connection);
            command.Parameters.AddWithValue("@Id", lblFeesId.Text.Trim());
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
               
                string id = reader["TransanctionId"].ToString();

                Response.Write("<tr>");
                Response.Write("<td>" + reader["FirstName"] + " "+ reader["Middlename"] + "" + reader["LastName"] + "</td>");
                Response.Write("<td>" + reader["ClassName"] + "</td>");
                Response.Write("<td>" + reader["TermName"] + " (" + reader["Academic_year"] + " )" + "</td>");
                Response.Write("<td>" + reader["CategoryName"] + "</td>");
                Response.Write("<td>" + reader["Paid"] + "</td>");
                Response.Write("<td>" + reader["Balance"] + "</td>");
                Response.Write("<td>" + reader["paymentMode"] + "</td>");
                Response.Write("<td>" + reader["dateRegistered"] + "</td>");
                Response.Write("<td>");

                Response.Write("<a href='PayEdit.aspx?id=" + id + "' class='btn btn-warning'>");
                Response.Write("<i class='fas fa-edit'></i> Edit</a>");
                Response.Write("</td>");
                Response.Write("<td>");
                Response.Write("<a href='Receipt.aspx?id=" + id + "' class='btn btn-info'>");
                Response.Write("<i class='fas fa-eye'></i> Receipt</a> ");

                Response.Write("</td>");
                Response.Write("</tr>");
            }

            reader.Close();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoice.aspx");
    }
}