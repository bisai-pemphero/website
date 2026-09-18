
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
            LoadDet();
            FeesCategory();
            Upsert();
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
       drpClassName.SelectedIndex = -1;
        drpLevel.SelectedIndex = -1;
        lblError.Text =  string.Empty;
        lblSuccess.Text =  string.Empty;
       
        txtCategoryName.Text = string.Empty;
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Dashboard.aspx");
    }


    public void FeesCategory()
    {
        try
        {
            drpLevel.Items.Clear();
            drpLevel.Items.Add("");
            drpLevel.Items.Add("All");

            string sql = " Select Distinct Level from Classes where SchoolId = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                drpLevel.Items.Add(dr.GetString(0));
            }

            dr.Close();

            drpClassName.Items.Clear();
            drpClassName.Items.Add("");
            drpClassName.Items.Add("School");
            string sqlClass = @"Select Distinct ClassName from Classes where SchoolId = @schoolId";
            SqlCommand cmdClass = new SqlCommand(sqlClass, appconSQL2);
            cmdClass.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader drClass = cmdClass.ExecuteReader();
            while (drClass.Read())
            {
                drpClassName.Items.Add(drClass.GetString(0));
            }

            drClass.Close();



        }
        catch (Exception ex)
        {

            lblError.Text = "Fees Level error" + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                    "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
        finally
        {
            //
        }
    }

    //Load Registerd Categories
    public void Upsert()
    {
        drpUpsert.Items.Clear();
        drpUpsert.Items.Add("");
        drpUpsert.Items.Add("ADD NEW");
        string sql = @"Select CategoryId, [CategoryName] from FeesCategory
        where SchoolId = @schoolId order by CategoryName";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            drpUpsert.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
        }

        dr.Close();
    }

    //Load feescategory

    //check here
    protected void btnsave_Click(object sender, EventArgs e)
    {

        try
        {

            // Check if file already exists
            string sqlCheck = @"SELECT COUNT(*) FROM FeesCategory WHERE GivenCategoryName = @CategoryName
                            and SchoolId = @Id and ClassName = @className";
            using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, appconSQL2))
            {
                cmdCheck.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());
                cmdCheck.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
                cmdCheck.Parameters.AddWithValue("@className", drpClassName.Text.Trim());
                int count = (int)cmdCheck.ExecuteScalar();
                if (count > 0)
                {
                    lblError.Text = "This category is already set!";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
                    return;
                }
            }

            double amount = Convert.ToDouble(txtAmount.Text.Trim());
            // Save details

            string category = txtCategoryName.Text.Trim();
            string className = drpClassName.Text.Trim();
            string categoryName = className + " " + category;

            string sqlInsert = @"Insert into FeesCategory (CategoryName, Amount,
                AcademicLevel, SchoolId, ClassName, GivenCategoryName) 
                VALUES (@Name, @Amount, @Level, @Id, @className, @givenName)";
            using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, appconSQL2))
            {
                cmdInsert.Parameters.AddWithValue("@givenName", txtCategoryName.Text.Trim());
                cmdInsert.Parameters.AddWithValue("@Amount", amount);
                cmdInsert.Parameters.AddWithValue("@Level", drpLevel.Text.Trim());
                cmdInsert.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
                cmdInsert.Parameters.AddWithValue("@Name", categoryName);
                cmdInsert.Parameters.AddWithValue("@className", drpClassName.Text.Trim());
                cmdInsert.ExecuteNonQuery();
            }

          
            Upsert();
            drpUpsert.SelectedIndex = -1;
            LoadDet();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Fees Category Added Successfully!')", true);
           

        }
        catch (Exception ex)
        {
            lblError.Text = "Error: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
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
                txtCategoryName.Focus();
                btnsave.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnsave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;


                string[] category = drpUpsert.Text.Split('|');
                string categoryName = category[1];


                //display details
                string sql1 = @"Select [GivenCategoryName], [Amount],[AcademicLevel],
                    ClassName from [FeesCategory] where [CategoryName] = @Name 
                    and [SchoolId] = @Id";
                SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
                cmd1.Parameters.AddWithValue("@Name", categoryName);
                cmd1.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
                SqlDataReader dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {
                    txtCategoryName.Text = dr1.GetString(0);
                    txtAmount.Text = dr1.GetDouble(1).ToString();
                    drpLevel.Text = dr1.GetString(2);
                    drpClassName.Text = dr1.GetString(3);

                }
                dr1.Close();

            }
        }
        catch (Exception ex)
        {
            lblSuccess.Text = "";
            lblError.Text = "Load details error: " + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

        }

    }


    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            string[] upsert = drpUpsert.Text.Split('|');
            string upsertId = upsert[0];

            double amount = Convert.ToDouble(txtAmount.Text.Trim());
            // Save details


            string feescategory = txtCategoryName.Text.Trim();
            string className = drpClassName.Text.Trim();
            string categoryName = className + " " + feescategory;

            string sqlUpdate = @"Update FeesCategory set CategoryName =@Name,
                Amount = @Amount, AcademicLevel = @Level, [ClassName] =@classname,
                [GivenCategoryName] = @givenName where CategoryId = @Id
                and SchoolId = @SchoolId";
            using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, appconSQL2))
            {
                cmdUpdate.Parameters.AddWithValue("@givenName", txtCategoryName.Text.Trim());
                cmdUpdate.Parameters.AddWithValue("@Amount", amount);
                cmdUpdate.Parameters.AddWithValue("@Level", drpLevel.Text.Trim());
                cmdUpdate.Parameters.AddWithValue("@Id", upsertId);
                cmdUpdate.Parameters.AddWithValue("@SchoolId", lblSchoolId.Text.Trim());
                cmdUpdate.Parameters.AddWithValue("@classname", drpClassName.Text.Trim());
                cmdUpdate.Parameters.AddWithValue("@Name", categoryName);

                cmdUpdate.ExecuteNonQuery();
            }

            Reset();
            drpUpsert.SelectedIndex = -1;
            Upsert();
            LoadDet();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Fees Category Updated Successfully!')", true);
  
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

            string[] category = drpUpsert.Text.Split('|');
            string categoryId = category[0];

            string sqlUpdate = @"Delete from FeesCategory where CategoryId = @Id
                                and SchoolId = @SchoolId";
            using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, appconSQL2))
            {

                cmdUpdate.Parameters.AddWithValue("@Id", categoryId);
                cmdUpdate.Parameters.AddWithValue("@SchoolId", lblSchoolId.Text.Trim());
                cmdUpdate.ExecuteNonQuery();
            }

            Reset();
            drpUpsert.SelectedIndex = -1;
            Upsert();
            LoadDet();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Fees Category Deleted Successfully!')", true);
        }
        catch (Exception ex)
        {
            lblError.Text = "There was an Error! " + ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }

    }

    //categories table 
    public DataTable LoadCategoryTable()
    {


        DataTable dt = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(@"select [CategoryName],[Amount], 
            [AcademicLevel] from [FeesCategory] where [SchoolId] = @schoolId Order by CategoryName ",
                connection);
            command.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
        }

        return dt;
    }

    public void LoadDet()
    {
        DataTable categoriesTable = LoadCategoryTable();
        if (categoriesTable.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in categoriesTable.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + row["CategoryName"] + "</td>");
                sb.Append("<td>" + row["Amount"] + "</td>");
                sb.Append("<td>" + row["AcademicLevel"] + "</td>");
                sb.Append("</tr>");
            }
            // Assuming you have a placeholder for the rows
            feesCategoryPlaceholder.InnerHtml = sb.ToString();
        }
        else
        {
            // Handle the case when no records are found
            feesCategoryPlaceholder.InnerHtml = "<tr><td colspan='4'>No data found.</td></tr>";
        }
    }

}