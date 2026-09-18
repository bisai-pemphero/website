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

public partial class AddSchool : System.Web.UI.Page
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

            BindSchoolsGrid();
        }
    }

    public void LoadUsername()
    {
        try
        {
            string sql = "select Fullname from Users where Username = @username";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@username", lblSession.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lblUser.Text = dr.GetString(0);
               
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


    protected void btnAddNewSchool_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddSchool.aspx");
    }

    private void BindSchoolsGrid()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT [SchoolId], [School_name], [School_address], [Admin_name] FROM [AllSchools]";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSchools.DataSource = dt;
                gvSchools.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading schools: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }


    protected void gvSchools_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            string schoolId = e.CommandArgument.ToString();
            Response.Redirect("UpDel.aspx?SchoolId=" + schoolId);
        }
        if (e.CommandName == "View")
        {
            string schoolId = e.CommandArgument.ToString();
            Response.Redirect("SchoolDetails.aspx?SchoolId=" + schoolId);
        }
    }

    protected void gvSchools_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            string schoolId = gvSchools.DataKeys[e.RowIndex].Value.ToString();
            string logoFileName = string.Empty;
            string letterheadFileName = string.Empty;

            // First, retrieve the image filenames before deleting the record
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Get the image filenames first
                string selectQuery = "SELECT Logo, Letterhead FROM [AllSchools] WHERE SchoolId = @SchoolId";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@SchoolId", schoolId);

                con.Open();
                using (SqlDataReader reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        logoFileName = reader["Logo"] != DBNull.Value ? reader["Logo"].ToString() : string.Empty;
                        letterheadFileName = reader["Letterhead"] != DBNull.Value ? reader["Letterhead"].ToString() : string.Empty;
                    }
                }
                con.Close();
            }

            // Now delete the database record
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM [AllSchools] WHERE SchoolId = @SchoolId";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, con);
                deleteCmd.Parameters.AddWithValue("@SchoolId", schoolId);

                con.Open();
                int result = deleteCmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                {
                    // Delete the associated images
                    DeleteSchoolImages(logoFileName, letterheadFileName);

                    ScriptManager.RegisterStartupScript(this, GetType(), "showDeleteSuccess", "showDeleteSuccess();", true);
                    BindSchoolsGrid();
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error deleting school: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorToast", "showErrorToast();", true);
        }
    }

    public void DeleteSchoolImages(string logo, string letterhead)
    {
        try
        {
            // Only attempt deletion if filenames are not empty
            if (!string.IsNullOrEmpty(logo))
            {
                string logoPath = Server.MapPath("~/img/Schooldocs/") + logo;
                if (File.Exists(logoPath))
                {
                    File.Delete(logoPath);
                }
            }

            if (!string.IsNullOrEmpty(letterhead))
            {
                string letterheadPath = Server.MapPath("~/img/Schooldocs/") + letterhead;
                if (File.Exists(letterheadPath))
                {
                    File.Delete(letterheadPath);
                }
            }
        }
        catch (IOException ioEx)
        {
            // Log the error but don't stop the database deletion
            System.Diagnostics.Trace.TraceError("File deletion error:" + ioEx.Message);
            // You can choose to show a warning or just log it silently
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Unexpected file deletion error:" +  ex.Message);
        }
    }

    protected void gvSchools_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvSchools.PageIndex = e.NewPageIndex;
        BindSchoolsGrid();
    }

    private void BindSchoolsGrid(string searchTerm = "")
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT [SchoolId], [School_name], [School_email], [PhoneNumber], 
                            [School_address], [Admin_name], [Admin_email] FROM [AllSchools]";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " WHERE School_name LIKE @SearchTerm OR School_email LIKE @SearchTerm OR Admin_name LIKE @SearchTerm";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSchools.DataSource = dt;
                gvSchools.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading schools: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorToast", "showErrorToast();", true);
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        BindSchoolsGrid(txtSearch.Text.Trim());
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        Response.Redirect("Download.aspx");
    }
}