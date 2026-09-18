
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
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.EnterpriseServices;
using System.Net;

public partial class FeesCategories : System.Web.UI.Page
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

    public void LoadAcademicYear()
    {

        drpUpsert.Items.Clear();
        drpUpsert.Items.Add("");
        drpUpsert.Items.Add("ADD NEW");
        string sql = @"SELECT [AcademicyearId], 
            CONCAT(DATENAME(YEAR, [Start_year]), '-', YEAR([End_year])) as Academic_year
             from [Academic_Year] where [SchoolId] = @schoolId";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text);
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            drpUpsert.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1) + " Academic Year");
        }

        dr.Close();

    }

    protected void drpUpsert_SelectedIndexChanged(object sender, EventArgs e)
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
                txtStartDate.Focus();
                btnsave.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnsave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;

                string[] academic = drpUpsert.Text.Split('|');
                string academicId = academic[0];

                //display details
                string sql1 = "SELECT [Start_year],[End_year] FROM [Academic_Year] " +
                    "WHERE [AcademicyearId] = @academicyearId";
                SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
                cmd1.Parameters.AddWithValue("@academicyearId", academicId);
                SqlDataReader dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {
                    txtStartDate.Text = dr1.GetString(0);
                    txtEndDate.Text = dr1.GetString(1);
                }
                dr1.Close();
            }
        }
        catch (Exception ex)
        {
            
            lblSuccess.Text = "";
            lblError.Text = "Upsert error " + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                    "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
    }



    private void Reset()
    {
        txtStartDate.Text = string.Empty;
       txtEndDate.Text = string.Empty;

        lblError.Text =  string.Empty;
        lblSuccess.Text =  string.Empty;
       
       
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Dashboard.aspx");
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {

        try
        {
            //Check if the username already exists
            string sql3 = "SELECT * FROM [Academic_Year]" +
                " WHERE [Start_year] = @start and [End_year] = @end and [SchoolId] = @schoolId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@start", txtStartDate.Text.Trim());
            cmd3.Parameters.AddWithValue("@end", txtEndDate.Text.Trim());
            cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {
                lblError.Text = "This School Year already exists!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

             
                dr3.Close();
                dr3.Dispose();
                return;
            }
            else
            {
                dr3.Close();
                dr3.Dispose();

                //Insert into schools table using parameterized query
                string sql2 = "Insert into Academic_Year ([Start_year], [End_year], [SchoolId]) " +
                    "VALUES (@start, @end, @schoolId) ";
                SqlCommand cmd2 = new SqlCommand(sql2, appconSQL2);
                cmd2.Parameters.AddWithValue("@start", txtStartDate.Text.Trim());
                cmd2.Parameters.AddWithValue("@end", txtEndDate.Text.Trim());
                cmd2.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd2.ExecuteNonQuery();
                cmd2.Dispose();

                Reset();
                LoadAcademicYear();
               

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Academic Year Saved Successfully!')", true);

            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
    }




    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            string[] Academic = drpUpsert.Text.Split('|');
            string Id = Academic[0];

            // Update user account using parameterized query
            string sql = "Update [Academic_Year] set Start_year = @start, End_year = @end" +
                " where [AcademicyearId] = @Id";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@start", txtStartDate.Text.Trim());
            cmd.Parameters.AddWithValue("@end", txtEndDate.Text.Trim());
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery();
            cmd.Dispose();


            Reset();
            lblError.Text = "";
            LoadAcademicYear();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Academic Year Updated Successfully!')", true);
  
        }
        catch (Exception ex)
        {
            lblError.Text = "There was an error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
        finally
        {
            //
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            string[] Academic = drpUpsert.Text.Split('|');
            string Id = Academic[0];

            string sql3 = "DELETE FROM Academic_Year WHERE [AcademicyearId] = @Id";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@Id", Id);
            cmd3.ExecuteNonQuery();
            cmd3.Dispose();


            Reset();
            lblError.Text = "";
            LoadAcademicYear();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Academic Year Deleted Successfully!')", true);
        }
        catch (Exception ex)
        {
            lblError.Text = "There was an Error! " + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }

    }

}