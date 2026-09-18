using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class Fees : System.Web.UI.Page
{
    string appconStr;
    string server, appdb, user, password, version;
    SqlConnection appconSQL2;

    private void readConf()
    {
        System.IO.StreamReader sr;
        {
            sr = System.IO.File.OpenText(Server.MapPath("dbconn.ini"));


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




    }




    protected void btnSend_Click(object sender, EventArgs e)
    {
        //view date
    }
    public class NationalIdData
    {
        public string DocumentNumber { get; set; }
        public string PersonalNumber { get; set; }
        public string Surname { get; set; }
        public string GivenNames { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Nationality { get; set; }
    }

    public class NationalIdParser
    {
        public static NationalIdData ParseNationalIdData(string idData)
        {
            if (string.IsNullOrEmpty(idData))
                throw new ArgumentException("ID data cannot be null or empty");

            var result = new NationalIdData();
            var segments = idData.Split('~');

            if (segments.Length < 4)
                throw new FormatException("Invalid ID data format");

            // Segment 1: Document and personal number
            ParseSegment1(segments[0], result);

            // Segment 2: Additional data (dates, nationality)
            ParseSegment2(segments[1], result);

            // Segment 3: Name information
            ParseSegment3(segments[2], result);

            // Segment 4: Additional personal data
            ParseSegment4(segments[3], result);

            return result;
        }

        private static void ParseSegment1(string segment, NationalIdData result)
        {
            // Format: 03~I<MWI0W8AN9Z1T9<<<<<<<<<<<<<<<
            var parts = segment.Split(new[] { '<' }, 2);
            if (parts.Length >= 2)
            {
                // Document number is usually the first part after initial codes
                result.DocumentNumber = parts[0].Substring(3).Trim(); // Remove "03~I"

                // Personal number (W8AN9Z1T) - extract from the second part
                var secondPart = parts[1];
                var personalNumberMatch = Regex.Match(secondPart, @"([A-Z0-9]{8,})");
                if (personalNumberMatch.Success)
                {
                    result.PersonalNumber = personalNumberMatch.Groups[1].Value;
                }
            }
        }

        private static void ParseSegment2(string segment, NationalIdData result)
        {
            // Format: ~9710201M3510203MWI<<<<<<<<<<<6~
            // This contains dates and nationality

            // Extract dates: 971020 (YYMMDD format)
            var dateMatch = Regex.Match(segment, @"(\d{6})[A-Z](\d{6})");
            if (dateMatch.Success)
            {
                result.DateOfBirth = ParseMrzDate(dateMatch.Groups[1].Value);
                result.ExpiryDate = ParseMrzDate(dateMatch.Groups[2].Value);
            }

            // Extract nationality (MWI - Malawi)
            var nationalityMatch = Regex.Match(segment, @"([A-Z]{3})<<");
            if (nationalityMatch.Success)
            {
                result.Nationality = nationalityMatch.Groups[1].Value;
            }
        }

        private static void ParseSegment3(string segment, NationalIdData result)
        {
            // Format: ~BISAI<<PEMPHERO<<<<<<<<<<<<<<<~
            var nameParts = segment.Split(new[] { '<' }, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length >= 2)
            {
                result.Surname = nameParts[0].Trim();
                result.GivenNames = nameParts[1].Trim();
            }
        }

        private static void ParseSegment4(string segment, NationalIdData result)
        {
            // Format: ~BISAI~W8AN9Z1T~PEMPHERO~~Male~20 Oct 1997~28 May 2017~
            var parts = segment.Split('~');

            for (int i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i])) continue;

                if (parts[i].Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                    parts[i].Equals("Female", StringComparison.OrdinalIgnoreCase))
                {
                    result.Gender = parts[i];
                }
                else if (DateTime.TryParse(parts[i], out DateTime date))
                {
                    if (!result.DateOfBirth.HasValue)
                        result.DateOfBirth = date;
                    else if (!result.IssueDate.HasValue)
                        result.IssueDate = date;
                    else if (!result.ExpiryDate.HasValue)
                        result.ExpiryDate = date;
                }
                else if (Regex.IsMatch(parts[i], @"^[A-Z0-9]{8,}$") && string.IsNullOrEmpty(result.PersonalNumber))
                {
                    result.PersonalNumber = parts[i];
                }
            }
        }

        private static DateTime? ParseMrzDate(string mrzDate)
        {
            if (string.IsNullOrEmpty(mrzDate) || mrzDate.Length != 6)
                return null;

            try
            {
                // MRZ dates are in YYMMDD format
                int year = int.Parse(mrzDate.Substring(0, 2));
                int month = int.Parse(mrzDate.Substring(2, 2));
                int day = int.Parse(mrzDate.Substring(4, 2));

                // Convert 2-digit year to 4-digit (assuming 2000s)
                year = year < 50 ? 2000 + year : 1900 + year;

                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }
    }
}