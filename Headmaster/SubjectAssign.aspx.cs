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

            string id = Request.QueryString["id"];
            LoadAll2(id);

            lblSubjectId.Text = id;

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
           
            drpTeacher.Items.Clear();
            drpTeacher.Items.Add("");
            string sql3 = @"Select UserId, Fullname from Users where SchoolId = @schoolId and (RoleId = 3 or RoleId = 6)";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@schoolId", lblSchoolId.Text.Trim());
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {

                drpTeacher.Items.Add(dr3.GetInt32(0) + "|" + dr3.GetString(1));
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

    public void LoadAll2(string subjectId)
    {
        try
        {
            string sql3 = @"Select S.[SubjectName] from Subjects as S
            Join Classes as C on S.ClassId = C.ClassID where S.SubjectId = @subjectId";
            SqlCommand cmd3 = new SqlCommand(sql3, appconSQL2);
            cmd3.Parameters.AddWithValue("@subjectId", subjectId);
            SqlDataReader dr3 = cmd3.ExecuteReader();
            while (dr3.Read())
            {
                txtSubjectName.Text = dr3.GetString(0);
               
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
            string[] thisTeacher = drpTeacher.Text.Split('|');
            int teacherId = Convert.ToInt32(thisTeacher[0]);

                //update subjects
                string sql = @"Update [Subjects] set [TeacherId] = @teacherId where [SubjectId] = @subjectId ";
                SqlCommand cmd = new SqlCommand(sql, appconSQL2);

                cmd.Parameters.AddWithValue("@teacherId", teacherId);
                cmd.Parameters.AddWithValue("@subjectId", lblSubjectId.Text.Trim());
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                              
                Reset();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccessAlert", "showAlert('success', 'Subject Allocated to Teacher Successfully!')", true);
            
        }
        catch (Exception ex)
        {
            lblError.Text = "Saving error!" + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowToast", "showErrorToast('" + lblError.Text + "');", true);
        }
    }

    public void Reset()
    {
        drpTeacher.SelectedIndex = -1;
        txtSubjectName.Text = string.Empty;        
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Subjects.aspx");
    }
}