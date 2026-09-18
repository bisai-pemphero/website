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

            string id = Request.QueryString["studentId"];
            lblStudentId.Text = id;

            LoadClasses();

            LoadStudentDetails();
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


    public void LoadClasses()
    {
        try
        {
            ddlCurrentClass.Items.Clear();
            ddlCurrentClass.Items.Add("");

            ddlPreviousClass.Items.Clear();
            ddlPreviousClass.Items.Add("");

            string sql = @"Select [ClassID], [ClassName] from [Classes] where [SchoolId] = @schoolId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ddlCurrentClass.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
                ddlPreviousClass.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Class error" + ex;
        }
        finally
        {
            //
        }
    }

    public void LoadStudentDetails()
    {
        try
        {
            string sql = @"Select [FirstName], [Middlename],
            [LastName], [Gender], [DateOfBirth], [AdmissionDate], [CurrentClassID], 
            [PreviousclassID], [PrevSchool], [SpecialNeeds], [ParentID]
            from Students where [StudentID] = @studentId";
            SqlCommand cmd = new SqlCommand(sql, appconSQL2);
            cmd.Parameters.AddWithValue("@studentId", lblStudentId.Text.Trim());
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                txtFirstName.Text = dr.IsDBNull(0) ? "" : dr.GetString(0);
                txtMiddleName.Text = dr.IsDBNull(1) ? "" : dr.GetString(1);
                txtLastName.Text = dr.IsDBNull(2) ? "" : dr.GetString(2);
                ddlStudentGender.Text = dr.IsDBNull(3) ? "" : dr.GetString(3);

                // Handle DateOfBirth (DateTime field)
                if (dr.IsDBNull(4))
                {
                    txtDateOfBirth.Text = "";
                }
                else
                {
                    // Use proper format for TextMode="Date"
                    txtDateOfBirth.Text = dr.GetDateTime(4).ToString("yyyy-MM-dd");
                }

                // Handle AdmissionDate (DateTime field)
                if (dr.IsDBNull(5))
                {
                    txtAdmissionDate.Text = "";
                }
                else
                {
                    txtAdmissionDate.Text = dr.GetDateTime(5).ToString("yyyy-MM-dd");
                }

                // Handle CurrentClass (integer field)
                if (dr.IsDBNull(6))
                {
                    ddlCurrentClass.SelectedIndex = -1; // Or 0 for default selection
                }
                else
                {
                    ddlCurrentClass.SelectedValue = dr.GetInt32(6).ToString();
                }

                // Handle PreviousClass (integer field)
                if (dr.IsDBNull(7))
                {
                    ddlPreviousClass.SelectedIndex = -1; // Or 0 for default selection
                }
                else
                {
                    ddlPreviousClass.SelectedValue = dr.GetInt32(7).ToString();
                }

                txtPrevSchool.Text = dr.IsDBNull(8) ? "" : dr.GetString(8);
                txtSpecialNeeds.Text = dr.IsDBNull(9) ? "" : dr.GetString(9);

                // Handle ParentId (integer field)
                if (dr.IsDBNull(10))
                {
                    lblParentId.Text = "";
                }
                else
                {
                    lblParentId.Text = dr.GetInt32(10).ToString();
                }
            }

            dr.Close();


            string sql1 = @"Select  [FullName], [Gender], [PhoneNumber], [AlternatePhone],
            [Email], [Address], [Relationship], [Occupation] from [StudentParent]
            where [ParentID] = @parentId";
            SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
            cmd1.Parameters.AddWithValue("@parentId", lblParentId.Text.Trim());
            SqlDataReader dr1 = cmd1.ExecuteReader();
            while (dr1.Read())
            {
                txtFullName.Text = dr1.IsDBNull(0) ? "" : dr1.GetString(0);
                ddlGender.Text = dr1.IsDBNull(1) ? "" : dr1.GetString(1);
                txtPhoneNumber.Text = dr1.IsDBNull(2) ? "" : dr1.GetString(2);
                txtAlternatePhone.Text = dr1.IsDBNull(3) ? "" : dr1.GetString(3);
                txtEmail.Text = dr1.IsDBNull(4) ? "" : dr1.GetString(4);
                txtAddress.Text = dr1.IsDBNull(5) ? "" : dr1.GetString(5);
                ddlRelationship.Text = dr1.IsDBNull(6) ? "" : dr1.GetString(6);
                txtOccupation.Text = dr1.IsDBNull(7) ? "" : dr1.GetString(7);
            }

            dr1.Close();
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
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (appconSQL2.State != ConnectionState.Open)
            {
                appconSQL2.Open();
            }

            //Update Parents
            string sql1 = @"Update StudentParent Set [FullName] = @fullname, [Gender] = @gender,
            [PhoneNumber] = @phone, [AlternatePhone] = @altenatePhone, [Email] = @email, 
            [Address] = @adress, [Relationship] = @relationship, [Occupation] = @occupation,
            [UpdatedAt] = GETDATE() Where [ParentID] = @parentId";
            SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
            cmd1.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim());
            cmd1.Parameters.AddWithValue("@gender", ddlGender.Text.Trim());
            cmd1.Parameters.AddWithValue("@phone", txtPhoneNumber.Text.Trim());
            cmd1.Parameters.AddWithValue("@altenatePhone", txtAlternatePhone.Text.Trim());
            cmd1.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
            cmd1.Parameters.AddWithValue("@adress", txtAddress.Text.Trim());
            cmd1.Parameters.AddWithValue("@relationship", ddlRelationship.Text.Trim());
            cmd1.Parameters.AddWithValue("@occupation", txtOccupation.Text.Trim());
            cmd1.Parameters.AddWithValue("@parentId", lblParentId.Text.Trim());
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();

            string[] currentClass = ddlCurrentClass.Text.Split('|');
            string classId = currentClass[0];

            string[] prevClass = ddlPreviousClass.Text.Split('|');
            string prevClassId = prevClass[0];

            DateTime dob = Convert.ToDateTime(txtDateOfBirth.Text.Trim());
            DateTime admissionDate = Convert.ToDateTime(txtAdmissionDate.Text.Trim());

            //update Student
            string sql4 = @"Update [Students] Set [FirstName] = @firstname, [Middlename] = @middleName, 
                    [LastName] = @lastName, [Gender] = @gender, [DateOfBirth] = @dob , [AdmissionDate] =  @admissionDate,
                    [CurrentClassID] = @currentClass, [PreviousclassID] = @preClass,
                    [PrevSchool] = @prevSchool, [SpecialNeeds] = @specialNeeds,
                    [UpdatedAt] = GETDATE() where [StudentID] = @studentId";
                    SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
                    
                    cmd4.Parameters.AddWithValue("@firstname", txtFirstName.Text.Trim());
                    cmd4.Parameters.AddWithValue("@middleName", txtMiddleName.Text.Trim());
                    cmd4.Parameters.AddWithValue("@lastName", txtLastName.Text.Trim());
                    cmd4.Parameters.AddWithValue("@gender", ddlStudentGender.Text.Trim());
                    cmd4.Parameters.AddWithValue("@dob", dob);
                    cmd4.Parameters.AddWithValue("@admissionDate", admissionDate);
                    cmd4.Parameters.AddWithValue("@currentClass", classId);
                    cmd4.Parameters.AddWithValue("@preClass", prevClassId);
                    cmd4.Parameters.AddWithValue("@prevSchool", txtPrevSchool.Text.Trim());
                    cmd4.Parameters.AddWithValue("@specialNeeds", txtSpecialNeeds.Text.Trim());
                    cmd4.Parameters.AddWithValue("@studentId", lblStudentId.Text.Trim());       
                    cmd4.ExecuteNonQuery();
                    cmd4.Dispose();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSuccess", "showAlert('success', 'Student Updated successfully!', 'ViewStudent.aspx');", true);    
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving Error: " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
        finally
        {
            if (appconSQL2.State == ConnectionState.Open)
            {
                appconSQL2.Close();
            }
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewStudent.aspx");
    }
}