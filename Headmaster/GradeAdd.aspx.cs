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

   

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            //Check if already exist
            string sql3 = @" Select * from GradingSystem where
  Grade = @grade 
  and [Minimum_Mark] = @min
  and [Maxmum_Mark] = @max
  and [Remark] = @remark
  and [SchoolId] = @schoolId
  and [Level] = @level";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@grade", txtGrade.Text.Trim());
            cmd3.Parameters.AddWithValue("@min", txtMinmumMark.Text.Trim());
            cmd3.Parameters.AddWithValue("@max", txtMaxmumMark.Text.Trim());
            cmd3.Parameters.AddWithValue("@remark", txtRemark.Text.Trim());
            cmd3.Parameters.AddWithValue("@level", drpLevel.Text.Trim());
            cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            if (dr3.HasRows)
            {
                lblError.Text = "Grade already set please Update!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
                return;
            }
            else
            {
                dr3.Close();
                dr3.Dispose();            

                //insert into exams
                string sql = @"  Insert into [GradingSystem] ([Grade], [Minimum_Mark],
                [Maxmum_Mark], [Remark], [SchoolId], [Level])
                VALUES (@grade, @min, @max, @remark, @schoolId, @level)";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);
                cmd.Parameters.AddWithValue("@grade", txtGrade.Text.Trim());
                cmd.Parameters.AddWithValue("@min", txtMinmumMark.Text.Trim());
                cmd.Parameters.AddWithValue("@max", txtMaxmumMark.Text.Trim());
                cmd.Parameters.AddWithValue("@remark", txtRemark.Text.Trim());
                cmd.Parameters.AddWithValue("@level", drpLevel.Text.Trim());
                cmd.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                              
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Grade saved Successfully!')", true);
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
        txtGrade.Text = string.Empty;
        txtMaxmumMark.Text = string.Empty;
        txtMinmumMark.Text = string.Empty;
        txtRemark.Text = string.Empty;
        drpLevel.SelectedIndex = -1;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Grades.aspx");
    }

   
}