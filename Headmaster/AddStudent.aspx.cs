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

            LoadClasses();
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
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (appconSQL2.State != ConnectionState.Open)
            {
                appconSQL2.Open();
            }

            string sql = @"SELECT COUNT(*) FROM Students 
                     WHERE [FirstName] = @firstname 
                     AND [Middlename] = @middlename 
                     AND [LastName] = @lastname 
                     AND [Gender] = @gender
                     AND [DateOfBirth] = @dob 
                     AND [SchoolId] = @schoolId";

            using (SqlCommand cmd = new SqlCommand(sql, appconSQL2))
            {
                cmd.Parameters.Add("@firstname", SqlDbType.VarChar).Value = txtFirstName.Text.Trim();
                cmd.Parameters.Add("@middlename", SqlDbType.VarChar).Value = txtMiddleName.Text.Trim();
                cmd.Parameters.Add("@lastname", SqlDbType.VarChar).Value = txtLastName.Text.Trim();
                cmd.Parameters.Add("@gender", SqlDbType.VarChar).Value = ddlStudentGender.Text.Trim();
                cmd.Parameters.Add("@dob", SqlDbType.Date).Value = DateTime.Parse(txtDateOfBirth.Text.Trim());
                cmd.Parameters.Add("@schoolId", SqlDbType.Int).Value = int.Parse(lblSchoolId.Text.Trim());

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    lblError.Text = "This student already exists, try searching him/her";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
                     "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
                    return;
                }
            }

            //Insert into Parents
            string sql1 = @"Insert into StudentParent ([FullName], [Gender], [PhoneNumber],
            [AlternatePhone], [Email], [Address], [Relationship], [Occupation], [CreatedAt],
            [UpdatedAt]) VALUES (@fullname, @gender, @phone, @altenatePhone, @email, @adress, 
            @relationship, @occupation, GETDATE(), GETDATE())";
            SqlCommand cmd1 = new SqlCommand(sql1, appconSQL2);
            cmd1.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim());
            cmd1.Parameters.AddWithValue("@gender", ddlGender.Text.Trim());
            cmd1.Parameters.AddWithValue("@phone", txtPhoneNumber.Text.Trim());
            cmd1.Parameters.AddWithValue("@altenatePhone", txtAlternatePhone.Text.Trim());
            cmd1.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
            cmd1.Parameters.AddWithValue("@adress", txtAddress.Text.Trim());
            cmd1.Parameters.AddWithValue("@relationship", ddlRelationship.Text.Trim());
            cmd1.Parameters.AddWithValue("@occupation", txtOccupation.Text.Trim());
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();

            //get parentId
            string sql0 = @"Select [ParentID] from [StudentParent] where [FullName] = @fullname
            and [Gender] = @gender and [PhoneNumber] = @phone and [Relationship] = @relationship";
            SqlCommand cmd0 = new SqlCommand(sql0, appconSQL2);
            cmd0.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim());
            cmd0.Parameters.AddWithValue("@gender", ddlGender.Text.Trim());
            cmd0.Parameters.AddWithValue("@phone", txtPhoneNumber.Text.Trim());
            cmd0.Parameters.AddWithValue("@relationship", ddlRelationship.Text.Trim());
            SqlDataReader dr0 = cmd0.ExecuteReader();
            while (dr0.Read())
            {
                lblParentId.Text = dr0.GetInt32(0).ToString();
            }
            dr0.Close();

            string SchoolName = "";
            string sql5 = "Select [School_name] from AllSchools where SchoolId = @Id";
            SqlCommand cmd5 = new SqlCommand(sql5, appconSQL2);
            cmd5.Parameters.AddWithValue("@Id", lblSchoolId.Text.Trim());
            SqlDataReader dr5 = cmd5.ExecuteReader();

            while (dr5.Read())
            {
                SchoolName = dr5.GetString(0);
            }

            dr5.Close();

            string Name = GetFirstTwoAndLastChar(SchoolName);

            //Generating random code
            string[] currentClass = ddlCurrentClass.Text.Split('|');
            string classId = currentClass[0];

            string[] prevClass = ddlPreviousClass.Text.Split('|');
            string prevClassId = prevClass[0];

            string code = GenerateRandomCode(3);
            string fullCode = Name + "-" + classId + code;

            // Checking if code already exists
            string checkCodeQuery = @"SELECT * FROM [Students] WHERE [AdmissionNo] = @code
                                    and [SchoolId] = @schoolId";
            SqlCommand commandCheck = new SqlCommand(checkCodeQuery, appconSQL2);
            commandCheck.Parameters.AddWithValue("@code", fullCode);
            commandCheck.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());


            using (SqlDataReader dataCheck = commandCheck.ExecuteReader())
            {
                if (dataCheck.HasRows)
                {
                    fullCode = Name + "-" + classId + code;
                    return;
                }
                else
                {
                    dataCheck.Close();

                    //Insert into Student
                    string sql4 = @"Insert into [Students] ([AdmissionNo], [FirstName], [Middlename], 
                    [LastName], [Gender], [DateOfBirth], [AdmissionDate], [CurrentClassID]
		            ,[PreviousclassID], [PrevSchool], [SpecialNeeds],[SchoolId], [Status], 
                    [ParentID], [IsDeleted], [CreatedAt],[UpdatedAt], [RegisteredBy])
		            
                    VALUES (@admissionNo, @firstname, @middleName, @lastName, @gender, @dob, 
                    @admissionDate, @currentClass, @preClass, @prevSchool, @specialNeeds,
		            @schoolId, @status, @parentId, @isDeleted, GETDATE(), GETDATE(), @user)";
                    SqlCommand cmd4 = new SqlCommand(sql4, appconSQL2);
                    cmd4.Parameters.AddWithValue("@admissionNo", fullCode);
                    cmd4.Parameters.AddWithValue("@firstname", txtFirstName.Text.Trim());
                    cmd4.Parameters.AddWithValue("@middleName", txtMiddleName.Text.Trim());
                    cmd4.Parameters.AddWithValue("@lastName", txtLastName.Text.Trim());
                    cmd4.Parameters.AddWithValue("@gender", ddlStudentGender.Text.Trim());
                    cmd4.Parameters.AddWithValue("@dob", txtDateOfBirth.Text.Trim());
                    cmd4.Parameters.AddWithValue("@admissionDate", txtAdmissionDate.Text.Trim());
                    cmd4.Parameters.AddWithValue("@currentClass", classId);
                    cmd4.Parameters.AddWithValue("@preClass", prevClassId);
                    cmd4.Parameters.AddWithValue("@prevSchool", txtPrevSchool.Text.Trim());
                    cmd4.Parameters.AddWithValue("@specialNeeds", txtSpecialNeeds.Text.Trim());
                    cmd4.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                    cmd4.Parameters.AddWithValue("@status", "Active");
                    cmd4.Parameters.AddWithValue("@parentId", lblParentId.Text.Trim());
                    cmd4.Parameters.AddWithValue("@isDeleted", 0);
                    cmd4.Parameters.AddWithValue("@user", lblSession.Text.Trim());       
                    cmd4.ExecuteNonQuery();
                    cmd4.Dispose();

                    Reset();

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Student Registered Successfully!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error: " + ex.Message;
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

    public void Reset()
    {
        txtAddress.Text = string.Empty;
        txtAdmissionDate.Text = string.Empty;
        txtAlternatePhone.Text = string.Empty;
        txtDateOfBirth.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtFirstName.Text = string.Empty;
        txtFullName.Text = string.Empty;
        txtLastName.Text = string.Empty;
        txtMiddleName.Text = string.Empty;
        txtOccupation.Text = string.Empty;
        txtPhoneNumber.Text = string.Empty;
        txtPrevSchool.Text = string.Empty;
        txtSpecialNeeds.Text = string.Empty;
        ddlCurrentClass.SelectedIndex = -1;
        ddlGender.SelectedIndex = -1;
        ddlPreviousClass.SelectedIndex = -1;
        ddlRelationship.SelectedIndex = -1;
        ddlStudentGender.SelectedIndex = -1;
    }

    static string GenerateRandomCode(int length)
    {
        const string chars = "0123456789";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    static string GetFirstTwoAndLastChar(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str; // Return the original string if it is null or empty
        }

        if (str.Length < 3)
        {
            return str; // Return the original string if it has less than 3 characters
        }

        string firstTwo = str.Substring(0, 2);
        string lastChar = str.Substring(str.Length - 1);

        return firstTwo + lastChar;
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Reset();
        txtFullName.Focus();
    }
}