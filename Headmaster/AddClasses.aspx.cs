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

public partial class AddStudent : System.Web.UI.Page
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

            LoadUpsert();


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


    protected void btnRegister_Click(object sender, EventArgs e)
    {
        try
        {


            //Check if the class is already registered 
            string sql4 = @"  SELECT * FROM Classes WHERE ClassName = @class and 
            Level= @level and [SchoolId] = @schoolId";
            SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
            cmd4.Parameters.AddWithValue("@class", txtClassName.Text.Trim());
            cmd4.Parameters.AddWithValue("@level", drpLevel.Text.Trim());
            cmd4.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());


            SqlDataReader dr4 = cmd4.ExecuteReader();
            if (dr4.HasRows)
            {
                lblError.Text = "This Class has already been Registered!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);


                txtClassName.Focus();
                dr4.Close();
                dr4.Dispose();
                return;
            }
            else
            {
                dr4.Close();
                dr4.Dispose();

                //insert into Subjects
                string sql7 = @"Insert into Classes ([ClassName],Level,Section,
[SchoolId], CreatedBy) VALUES (@class, @level, @section, @schoolId, @user)";
                SqlCommand cmd7 = new SqlCommand(sql7, appconSQL2);
                cmd7.Parameters.AddWithValue("@class", txtClassName.Text.Trim());
                cmd7.Parameters.AddWithValue("@level", drpLevel.Text.Trim());
                cmd7.Parameters.AddWithValue("@section", txtSection.Text.Trim());
                cmd7.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd7.Parameters.AddWithValue("@user", lblUser.Text.Trim());
                cmd7.ExecuteNonQuery();
                cmd7.Dispose();

                LoadUpsert();

                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Class Saved Successfully!')", true);
                LoadUpsert();
            }
        }
        catch (Exception ex)
        {
            // Handle the exception here
            lblError.Text = "An error upon teacher assigment." + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                 "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

        }
    }

    private void Reset()
    {
        drpLevel.SelectedIndex = -1;
      txtSection.Text = string.Empty;
        txtClassName.Text = string.Empty ;
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(drpUpsert.Text))
            {
                lblError.Text = "Please select Class to update!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

                drpUpsert.Focus();
                return;
            }

            //Subject id and name
            string[] id = drpUpsert.Text.Split('|');
            int classId = Convert.ToInt32(id[0]);


            // Update staff 
            string sql = @"Update Classes set ClassName = @className,
Level = @Level, Section = @section where classId = @Id";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@className", txtClassName.Text.Trim());
            cmd.Parameters.AddWithValue("@Level", drpLevel.Text.Trim());
           cmd.Parameters.AddWithValue("@Section", txtSection.Text.Trim());
            cmd.Parameters.AddWithValue("@Id", classId);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            Reset();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Class Updated Successfully!')", true);
            LoadUpsert();
        }
        catch (Exception ex)
        {
            
            lblError.Text = "Update Class error." + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);


        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(drpUpsert.Text))
            {
                lblError.Text = "Please select Class to delete";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                 "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

                drpUpsert.Focus();
                return;
            }

            string[] id = drpUpsert.Text.Split('|');
            int classId = Convert.ToInt32(id[0]);

            // Delete classes

            string sql = "Delete from Classes where ClassID = @classId ";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@classId", classId);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            Reset();
            lblError.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Class Deleted Successfully!')", true);
            LoadUpsert();
        }
        catch (Exception ex)
        {
            // Handle the exception here, e.g., log it or display an error message.
            lblError.Text = "Delete Class error." + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

            // lblSuccess.Text = "";
        }
    }

    protected void drpUpsert_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            //lblSuccess.Text = "";

            if (string.IsNullOrEmpty(drpUpsert.Text))
            {
                Reset();
                btnRegister.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else if (drpUpsert.Text == "ADD NEW")
            {
                Reset();
                txtClassName.Focus();
                btnRegister.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnRegister.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;

                // drpSubject.Items.Clear();

                string[] id = drpUpsert.Text.Split('|');
                int classId = Convert.ToInt32(id[0]);

                //display class details
                string sql1 = @"Select ClassName, Level, Section from Classes where ClassID = @Id";
                SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
                cmd1.Parameters.AddWithValue("@Id", classId);
                SqlDataReader dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {
                    txtClassName.Text = dr1.GetString(0);
                    drpLevel.Text = dr1.GetString(1);   
                    txtSection.Text = dr1.GetString(2);                    
                }
                dr1.Close();
            }
        }
        catch (Exception ex)
        {
            // Handle the exception here, e.g., log it or display an error message.
            lblError.Text = "Loading class details error." + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

            // lblSuccess.Text = "";
        }
    }

    public void LoadUpsert()
    {
        try
        {
            //load the classes
            drpUpsert.Items.Clear();
            drpUpsert.Items.Add("");
            drpUpsert.Items.Add("ADD NEW");
            string sql = @"SELECT [ClassID], [ClassName] from [Classes] where [SchoolId] = @schoolId ";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
    }
}