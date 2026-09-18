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
          
                lblError.Text = "This Class has already been Registered!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
               "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Class Saved Successfully!')", true); 
    }

    public void LoadClasses()
    {

        drpClass.Items.Clear();
        drpClass.Items.Add("");
        drpClass.Items.Add("0|Graduate");

        drpNewClass.Items.Clear();
        drpNewClass.Items.Add("");
        drpNewClass.Items.Add("0|Graduate");

        string sql = @"SELECT  ClassID, ClassName FROM   Classes where SchoolId = @schoolId ORDER BY className";
        SqlCommand cmd = new SqlCommand(sql, appconSQL2);
        cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
        SqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            drpClass.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
            drpNewClass.Items.Add(dr.GetInt32(0) + "|" + dr.GetString(1));
        }

        dr.Close();

    }

   

    //Getting contacts 
    public class Contact
    {
        public string Name { get; set; }
        public string StudentId { get; set; }

        public bool IsSelected { get; set; }
    }



    static List<Contact> GetContacts(string connectionString, int schoolId, string className)
    {
        List<Contact> contacts = new List<Contact>();

        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"Select FirstName, Middlename, LastName, 
                StudentID from Students where [SchoolId] = @schoolId
                and CurrentClassID = @currentClass";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@schoolId", schoolId);
            command.Parameters.AddWithValue("@currentClass", className);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Contact contact = new Contact
                {
                    Name = reader["FirstName"] + " " + reader["Middlename"] + " " + reader["LastName"],
                    StudentId = reader["StudentId"].ToString() 
                };
                contacts.Add(contact);
            }
            reader.Close();
        }
        return contacts;
    }

    public void SelectData()
    {
        // Assuming schoolId is obtained dynamically, hardcoded for now
        int schoolId = Convert.ToInt32(lblSchoolId.Text.Trim());
        string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
        string[] studentClass = drpClass.Text.Split('|');
        string className = studentClass[0];


        // Populate the CheckBoxList with students
        chkStudents.DataSource = GetContacts(connectionString, schoolId, className);
        chkStudents.DataTextField = "Name";
        chkStudents.DataValueField = "StudentId";
        chkStudents.DataBind();
    }


    protected void drpPrevClass_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectData();
    }

    public void UpdateClassForSelectedStudents()
    {
        if (drpNewClass.Text == "Graduate")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
            int schoolId = Convert.ToInt32(lblSchoolId.Text.Trim());

            string[] studentNewClass = drpNewClass.Text.Split('|');
            string[] studentPrevClass = drpClass.Text.Split('|');

            string newClass = studentNewClass[0];
            string prevClass = studentPrevClass[0];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (ListItem item in chkStudents.Items)
                {
                    if (item.Selected)
                    {
                        string studentId = item.Value;  // The StudentId of the selected student

                        string updateQuery = @"UPDATE Students SET CurrentClassID = @newClass, 
                        PreviousclassID = @previousClass, Status = 'Graduated' 
                        WHERE StudentId = @studentId  AND SchoolId = @schoolId";
                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@newClass", newClass);
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.Parameters.AddWithValue("@schoolId", schoolId);
                            command.Parameters.AddWithValue("@previousClass", prevClass);

                            command.ExecuteNonQuery();  // Update the class for the selected student
                        }
                    }
                }
            }

            // After updating, you can give a confirmation message
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Students successfully promoted to the new class!')", true);
          }
        else
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;
            int schoolId = Convert.ToInt32(lblSchoolId.Text.Trim());

            string[] studentNewClass = drpNewClass.Text.Split('|');
            string[] studentPrevClass = drpClass.Text.Split('|');

            string newClass = studentNewClass[0];
            string prevClass = studentPrevClass[0];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (ListItem item in chkStudents.Items)
                {
                    if (item.Selected)
                    {
                        string studentId = item.Value;  // The StudentId of the selected student

                        string updateQuery = @"UPDATE Students SET CurrentClassID = @newClass, 
                                        PreviousclassID = @previousClass WHERE StudentId = @studentId
                                        AND SchoolId = @schoolId";
                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@newClass", newClass);
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.Parameters.AddWithValue("@schoolId", schoolId);
                            command.Parameters.AddWithValue("@previousClass", prevClass);

                            command.ExecuteNonQuery();  
                        }
                    }
                }
            }

           
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Students successfully promoted to the new class!')", true);
            
        }

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (chkStudents.Items.Cast<ListItem>().Any(item => item.Selected))
        {
            UpdateClassForSelectedStudents();
            chkStudents.DataSource = "";
            chkStudents.DataBind();

            drpClass.SelectedIndex = -1;
            drpNewClass.SelectedIndex = -1;
        }
        else
        {
            lblError.Text = "Please select at least one student!";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast",
              "showErrorToast('" + lblError.Text.Replace("'", "\\'") + "');", true);
        }
    }

}