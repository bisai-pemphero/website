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

public partial class UsersView : System.Web.UI.Page
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

            BindUsersGrid();
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


    private void BindUsersGrid()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"Select U.[UserId], U.[Username], U.[Fullname], R.RoleName, AL.School_name
                from [Users] as U Join AllSchools as AL on U.SchoolId = AL.SchoolId
                Join Roles as R on R.RoleId = U.RoleId";

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
            lblError.Text = "Error loading Users: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }


    protected void gvSchools_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            string userId = e.CommandArgument.ToString();
            Response.Redirect("UserEdit.aspx?userId=" + userId);
        }
    }

    protected void gvSchools_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            string userId = gvSchools.DataKeys[e.RowIndex].Value.ToString();
          
            // Now delete the database record
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM Users WHERE UserId = @userId";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, con);
                deleteCmd.Parameters.AddWithValue("@userId", userId);

                con.Open();
                int result = deleteCmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                {
                    // Delete the associated images
                    ScriptManager.RegisterStartupScript(this, GetType(), "showDeleteSuccess", "showDeleteSuccess();", true);
                    BindUsersGrid();
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error deleting user: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorToast", "showErrorToast();", true);
        }
    }

    protected void gvSchools_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvSchools.PageIndex = e.NewPageIndex;
        BindUsersGrid();
    }

    private void BindUsersGrid(string searchTerm = "")
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"Select U.[UserId], U.[Username], U.[Fullname], R.RoleName, AL.School_name
                from [Users] as U Join AllSchools as AL on U.SchoolId = AL.SchoolId
                Join Roles as R on R.RoleId = U.RoleId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " WHERE Fullname LIKE @SearchTerm OR Username LIKE @SearchTerm OR RoleName LIKE @SearchTerm ";
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
            lblError.Text = "Error loading users: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorToast", "showErrorToast();", true);
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        BindUsersGrid(txtSearch.Text.Trim());
    }


    protected void btnAddUser_Click(object sender, EventArgs e)
    {
        Response.Redirect("UserAdd.aspx");
    }
}