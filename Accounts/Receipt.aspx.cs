using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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


        string id = Request.QueryString["id"];
        LoadReceiptProfile(id);
        //LoadClass(lblReceiptNo.Text);
    }

    public void LoadReceiptProfile(string myReceiptNo)
    {
        try
           
        {
            int schoolId = 0;
            string sql3 = @" SELECT CONVERT(DATE, P.datePaid), P.[paidBy],
            P.[paymentMode], P.Paid, P.Balance, T.TermName, S.FirstName, 
            S.Middlename, S.LastName, P.[postedBy], FC.CategoryName, 

            F.SchoolId, P.[receipNumber], C.ClassName 
            FROM [Payments] AS P 
			JOIN Fees as F on P.FeesId = F.FeesId
			JOIN [SchoolTerm] AS T ON F.TermId = T.[TermId]
			JOIN Students as S on S.StudentID = F.studentId
			JOIN Classes as C on C.ClassID = S.CurrentClassID 
			JOIN FeesCategory as FC on FC.CategoryId  = F.fees_Category
            WHERE P.[TransanctionId] = @Id";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@Id", myReceiptNo);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
               
                lblDate.Text = dr3.GetDateTime(0).ToShortDateString();
                lblReceivedFrom.Text = dr3.GetString(1);
                lblmethod.Text = dr3.GetString(2);
                lblamount.Text = "MK" + dr3.GetDouble(3).ToString("N2");
                lblbalance.Text = "MK" + dr3.GetDouble(4).ToString("N2");
                lblterm.Text = dr3.GetString(5);
                lblFor.Text = dr3.GetString(6) + " " + dr3.GetString(7) + " " + dr3.GetString(8);
                lblUser.Text = dr3.GetString(9);
                lblCategory.Text = dr3.GetString(10);

                schoolId = dr3.GetInt32(11);
              
                lblReceiptNo.Text = dr3.GetString(12);
                lblClass.Text = dr3.GetString(13);
            }
            dr3.Close();
            dr3.Dispose();

            //Load school details
            
            string Logoname = "";
            string sql0 = @"select [School_name],[PhoneNumber],[Slogan], [Logo], [School_address] from AllSchools 
                where [SchoolId] = @Id";
            SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
            cmd0.Parameters.AddWithValue("@Id", schoolId);
            SqlDataReader dr0 = cmd0.ExecuteReader();
            while (dr0.Read())
            {
                
                lblSchoolName.Text = dr0.GetString(0);
                lblPhoneNumber.Text = dr0.GetString(1);
                lblSlogan.Text = dr0.GetString(2);
                Logoname = dr0.GetString(3);
                lblAddress.Text = dr0.GetString(4);
            }
            dr0.Close();

            imgLogo.Src = "~/img/Schooldocs/" + Logoname;
        }
        catch(Exception ex)
        {
            lblError.Text = "Receipt profile error" + ex;
        }

    }
   

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("StudentInvoice.aspx");
    }
}