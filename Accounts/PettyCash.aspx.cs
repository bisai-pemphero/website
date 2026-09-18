
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Drawing;
using System.IO;
//for the decryption are the namespaces below
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.EnterpriseServices;
using System.Net;

public partial class AdminDashboard : System.Web.UI.Page
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
            this.Response.Redirect("mySchoolLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadUsername();
            LoadAcademicYear();
            LoadDet();
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

    private void Reset()
    {
        txtAmount.Text = string.Empty;
        txtApprovedby.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtRequestedby.Text = string.Empty;
        drpAcademicYear.SelectedIndex = -1;
        drpTerm.Items.Clear();
        lblError.Text =  string.Empty;
        lblSuccess.Text =  string.Empty;

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
        LoadAllTerms();
        drpTerm.Focus();
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {

        try
        {
            if (txtAmount.Text == string.Empty)
            {
                lblError.Text = "Please enter Amount for Petty Cash!";
                txtAmount.Focus();
                return;
            }
            if (txtApprovedby.Text == string.Empty)
            {
                lblError.Text = "Please enter Approver's name!";
                txtApprovedby.Focus();
                return;
            }
            if (txtDescription.Text == string.Empty)
            {
                lblError.Text = "Please enter Petty Cash Description!";
                txtDescription.Focus();
                return;
            }
            if (txtRequestedby.Text == string.Empty)
            {
                lblError.Text = "Please enter this field!";
                txtRequestedby.Focus();
                return;
            }
            if (drpTerm.Text == string.Empty)
            {
                lblError.Text = "Please Select Term!";
                drpTerm.Focus();
                return;
            }
            if (txtDate.Text == string.Empty)
            {
                lblError.Text = "Please Enter date of Transaction!";
                txtDate.Focus();
                return;
            }

            string[] term = drpTerm.Text.Split('|');
            int termId = Convert.ToInt32(term[0]);

            string amountText = txtAmount.Text.Trim();
            string cleanedAmountText = new string(amountText.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());

            double amount;
            if (double.TryParse(cleanedAmountText, out amount))
            {
                string sql4 = @"SELECT * FROM PettyCash WHERE [Description] = @description AND
                        [Amount] = @amount AND [Collectedby] = @collectedby 
                        AND [TermId] = @term AND [SchoolId] = @schoolId AND [Date] = @date";
                SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
                cmd4.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd4.Parameters.AddWithValue("@amount", amount);
                cmd4.Parameters.AddWithValue("@collectedby", txtRequestedby.Text.Trim());
                cmd4.Parameters.AddWithValue("@term", termId);
                cmd4.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd4.Parameters.AddWithValue("@date", txtDate.Text.Trim());

                SqlDataReader dr4 = cmd4.ExecuteReader();
                if (dr4.HasRows)
                {
                    Reset();
                    lblError.Text = "This entry was already entered!";
                    lblSuccess.Text = "";
                    dr4.Close();
                    return;
                }
                else
                {
                    dr4.Close();

                    string sql7 = @"INSERT INTO [PettyCash] ([Description], [Amount], [Collectedby],
                            [Approvedby], [TermId], [SchoolId], [Date], [Postedby])
                            VALUES (@description, @amount, @collectedby, @approved, @termId,
                            @schoolId, @date, @user)";
                    SqlCommand cmd7 = new SqlCommand(sql7, appconSQL2);
                    cmd7.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                    cmd7.Parameters.AddWithValue("@amount", amount);
                    cmd7.Parameters.AddWithValue("@collectedby", txtRequestedby.Text.Trim());
                    cmd7.Parameters.AddWithValue("@approved", txtApprovedby.Text.Trim());
                    cmd7.Parameters.AddWithValue("@termId", termId);
                    cmd7.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                    cmd7.Parameters.AddWithValue("@date", txtDate.Text.Trim());
                    cmd7.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                    cmd7.ExecuteNonQuery();

                    LoadDet();
                    Reset();
                    txtDate.Text = string.Empty;
                    drpUpsert.Items.Clear();
                    lblSuccess.Text = "PETTY CASH SAVED SUCCESSFULLY!";
                }
            }
            else
            {
                lblError.Text = "Please enter a valid amount!";
                txtAmount.Focus();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Save error" + ex; ;
        }

    }
    protected void drpUsert_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            lblSuccess.Text = "";

            if (string.IsNullOrEmpty(drpUpsert.Text))
            {
                Reset();
                btnsave.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else if (drpUpsert.Text == "ADD NEW")
            {
                Reset();
                drpAcademicYear.Focus();
                btnsave.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnsave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;

               

                string[] id = drpUpsert.Text.Split('|');
                int pettyCashId = Convert.ToInt32(id[0]);

              
                string sql1 = @"Select [Description], [Amount], [Collectedby], [Approvedby], [Date]
                        from PettyCash where [Id] = @pettyCashId";
                SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
                cmd1.Parameters.AddWithValue("@pettyCashId", pettyCashId);
             
                SqlDataReader dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {
                    txtDescription.Text = dr1.GetString(0);
                    txtAmount.Text = dr1.GetDouble(1).ToString();
                    txtRequestedby.Text = dr1.GetString(2);
                    txtApprovedby.Text = dr1.GetString(3);
                    txtDate.Text = dr1.GetString(4);

                }
                dr1.Close();
            }
        }
        catch (Exception ex)
        {
            // Handle the exception here, e.g., log it or display an error message.
            lblError.Text = "Loading Petty Cash." + ex;
            lblSuccess.Text = "";
        }
    }

    public void LoadUpsert()
    {
        try
        {
            //load the subjects
            drpUpsert.Items.Clear();
            drpUpsert.Items.Add("");
            drpUpsert.Items.Add("ADD NEW");
            string sql = @"SELECT [Id], [Collectedby] from [PettyCash] where SchoolId = @schoolId
                    and [Date] = @date ";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            cmd.Parameters.AddWithValue("@date", txtDate.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpUpsert.Items.Add(dr.GetInt32(0) + "| " + dr.GetString(1));
            }

            dr.Close();

        }
        catch (Exception ex)
        {
            lblError.Text = "Load upsert error" + ex;
        }

    }

    protected void txtDate_TextChanged(object sender, EventArgs e)
    {
        LoadUpsert();
        drpUpsert.Focus();
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(drpUpsert.Text))
            {
                lblError.Text = "Please select Petty cash transaction to update!";
                lblSuccess.Text = "";
                drpUpsert.Focus();
                return;
            }
            if (string.IsNullOrEmpty(drpTerm.Text))
            {
                lblError.Text = "Please select Term!";
                lblSuccess.Text = "";
                drpTerm.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtDate.Text))
            {
                lblError.Text = "Please enter date of transanction!";
                lblSuccess.Text = "";
                txtDate.Focus();
                return;
            }

            //Petty Cash id and name
            string[] id = drpUpsert.Text.Split('|');
            int pettyCash = Convert.ToInt32(id[0]);

            string[] term = drpTerm.Text.Split('|');
            int termId = Convert.ToInt32(term[0]);

            string amountText = txtAmount.Text.Trim();
            string cleanedAmountText = new string(amountText.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());

            double amount;
            if (double.TryParse(cleanedAmountText, out amount))
            {
                // Update PettyCash 
                string sql = @"Update PettyCash set [Description] = @description, [Amount] = @amount,
                        [Collectedby] = @collectedby, [TermId] = @term , [SchoolId] = @schoolId,
                        [Date] = @date, [Approvedby]= @approved  where Id = @Id";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@collectedby", txtRequestedby.Text.Trim());
                cmd.Parameters.AddWithValue("@approved", txtApprovedby.Text.Trim());
                cmd.Parameters.AddWithValue("@term", termId);
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd.Parameters.AddWithValue("@date", txtDate.Text.Trim());
                cmd.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                cmd.Parameters.AddWithValue("@Id", pettyCash);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                LoadDet();
                Reset();
                lblError.Text = "";
                txtDate.Text = "";
                drpUpsert.Items.Clear();
                lblSuccess.Text = "PETTY CASH UPDATED SUCCESSFULLY!";
               
            }
            else
            {
                lblError.Text = "Please enter a valid amount!";
                txtAmount.Focus();
            }
                
        }
        catch (Exception ex)
        {
            // Handle the exception here, e.g., log it or display an error message.
            lblError.Text = "Petty Cash Update error." + ex;
            lblSuccess.Text = "";
        }
    }


    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(drpUpsert.Text))
            {
                lblError.Text = "Please select Form Teacher to delete";
                lblSuccess.Text = "";
                drpUpsert.Focus();
                return;
            }

            string[] id = drpUpsert.Text.Split('|');
            int pettyCashId = Convert.ToInt32(id[0]);

            // Delete subjects

            string sql = "Delete from PettyCash where Id = @pettyCashId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@pettyCashId", pettyCashId);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            LoadDet();
            Reset();
            drpUpsert.Items.Clear();
            txtDate.Text = string.Empty;
            lblError.Text = "";
            lblSuccess.Text = "PETTY CASH DELETED SUCCESSFULLY!";
           
        }
        catch (Exception ex)
        {
            // Handle the exception here, e.g., log it or display an error message.
            lblError.Text = "Petty Cash delete error: " + ex;
            lblSuccess.Text = "";
        }
    }

    public DataTable LoadPettyCash()
    {
      

        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"
             Select [Description],[Amount] ,[Collectedby] from [PettyCash] where 
             Convert(DATE, Date)  = Convert(DATE, GETDATE()) AND [SchoolId] = @schoolId",
                connection);
            command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
         
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadDet()
    {
        DataTable generalReport = LoadPettyCash();
        if (generalReport.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in generalReport.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["Description"] + "</td>");
                sb.Append("<td>" + row["Amount"] + "</td>");
                sb.Append("<td>" + row["Collectedby"] + "</td>");
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

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Dashboard.aspx");
    }
}