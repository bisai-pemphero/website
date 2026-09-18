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

            LoadAll();
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

    public void LoadAll()
    {
        try
        {
           
            drpClass.Items.Clear();
            drpClass.Items.Add("");
            string sql3 = @"Select [ClassID], [ClassName] from Classes where SchoolId = @schoolId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                drpClass.Items.Add(dr3.GetInt32(0) + "|" + dr3.GetString(1));
            }

            dr3.Close();

          
        }
        catch (Exception ex)
        {
            lblError.Text = "Load details error" + ex;
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
            string[] thisClass = drpClass.Text.Split('|');
            int classId = Convert.ToInt32(thisClass[0]);
            string className = thisClass[1];

            //Check if already exist
            string sql3 = @" Select * from Subjects where SubjectName = @subjecName and ClassId = @classId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@subjecName", txtSubjectName.Text.Trim());
            cmd3.Parameters.AddWithValue("@classId", classId);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {
                lblError.Text = "Subject already added please Update!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                return;
            }
            else
            {
                dr3.Close();
                dr3.Dispose();            

                //insert into subjects
                string sql = @"insert into [Subjects] ([SubjectName], [ClassId], [Createdby], [DateCreated]) 
                VALUES (@name, @classId, @user, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
              
                cmd.Parameters.AddWithValue("@name", txtSubjectName.Text);
                cmd.Parameters.AddWithValue("@classId", classId);
                cmd.Parameters.AddWithValue("@user", lblSession.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                              
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Subject added Successfully!')", true);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }

    public void Reset()
    {
        drpClass.SelectedIndex = -1;
        txtSubjectName.Text = string.Empty;        
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Subjects.aspx");
    }
}