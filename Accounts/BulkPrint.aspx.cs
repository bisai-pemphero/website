using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class BulkPrint : System.Web.UI.Page
{
    private string connectionString;
    private List<ReceiptData> receipts = new List<ReceiptData>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            connectionString = ConfigurationManager.ConnectionStrings["Myschools"].ConnectionString;

            // Get parameters
            string dateRange = Request.QueryString["date"];
            string classId = Request.QueryString["class"];
            string transactionIds = Request.QueryString["transactions"];
            string autoPrint = Request.QueryString["autoprint"];

            if (!string.IsNullOrEmpty(dateRange))
            {
                LoadBulkReceiptsByDate(dateRange);
            }
            else if (!string.IsNullOrEmpty(classId))
            {
                LoadBulkReceiptsByClass(classId);
            }
            else if (!string.IsNullOrEmpty(transactionIds))
            {
                LoadBulkReceiptsByTransactionIds(transactionIds);
            }
            else
            {
                lblError.Text = "No valid parameters provided for bulk printing";
                lblError.Visible = true;
            }

            // Update summary
            UpdateSummary();
        }
    }

    
    private void LoadBulkReceiptsByDate(string date)
    {
        try
        {


            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT P.TransanctionId, CONVERT(DATE, P.datePaid) as PaymentDate, 
                           P.paidBy, P.paymentMode, P.Paid, P.Balance, T.TermName, 
                           S.FirstName, S.Middlename, S.LastName, P.postedBy, 
                           FC.CategoryName, F.SchoolId, P.receipNumber, C.ClassName,
                           SCH.School_name, SCH.PhoneNumber, SCH.Slogan, SCH.Logo, SCH.School_address
                    FROM Payments AS P 
                    JOIN Fees as F on P.FeesId = F.FeesId
                    JOIN SchoolTerm AS T ON F.TermId = T.TermId
                    JOIN Students as S on S.StudentID = F.studentId
                    JOIN Classes as C on C.ClassID = S.CurrentClassID 
                    JOIN FeesCategory as FC on FC.CategoryId = F.fees_Category
                    JOIN AllSchools as SCH on SCH.SchoolId = F.SchoolId
                    WHERE CONVERT(DATE, P.datePaid) = @date
                    ORDER BY P.datePaid, P.receipNumber";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@date", SqlDbType.Date).Value = DateTime.Parse(date);
                   

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receipts.Add(CreateReceiptDataFromReader(reader));
                        }
                    }
                }
            }

            GenerateThermalReceipts();
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading receipts: " + ex.Message;
            lblError.Visible = true;
        }
    }

    private void LoadBulkReceiptsByClass(string classId)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT P.TransanctionId, CONVERT(DATE, P.datePaid) as PaymentDate, 
                           P.paidBy, P.paymentMode, P.Paid, P.Balance, T.TermName, 
                           S.FirstName, S.Middlename, S.LastName, P.postedBy, 
                           FC.CategoryName, F.SchoolId, P.receipNumber, C.ClassName,
                           SCH.School_name, SCH.PhoneNumber, SCH.Slogan, SCH.Logo, SCH.School_address
                    FROM Payments AS P 
                    JOIN Fees as F on P.FeesId = F.FeesId
                    JOIN SchoolTerm AS T ON F.TermId = T.TermId
                    JOIN Students as S on S.StudentID = F.studentId
                    JOIN Classes as C on C.ClassID = S.CurrentClassID 
                    JOIN FeesCategory as FC on FC.CategoryId = F.fees_Category
                    JOIN AllSchools as SCH on SCH.SchoolId = F.SchoolId
                    WHERE C.ClassID = @ClassId
                    ORDER BY S.LastName, S.FirstName, P.datePaid";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ClassId", SqlDbType.Int).Value = int.Parse(classId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receipts.Add(CreateReceiptDataFromReader(reader));
                        }
                    }
                }
            }

            GenerateThermalReceipts();
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading receipts: " + ex.Message;
            lblError.Visible = true;
        }
    }

    private void LoadBulkReceiptsByTransactionIds(string transactionIds)
    {
        try
        {
            string[] ids = transactionIds.Split(',');

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT P.TransanctionId, CONVERT(DATE, P.datePaid) as PaymentDate, 
                           P.paidBy, P.paymentMode, P.Paid, P.Balance, T.TermName, 
                           S.FirstName, S.Middlename, S.LastName, P.postedBy, 
                           FC.CategoryName, F.SchoolId, P.receipNumber, C.ClassName,
                           SCH.School_name, SCH.PhoneNumber, SCH.Slogan, SCH.Logo, SCH.School_address
                    FROM Payments AS P 
                    JOIN Fees as F on P.FeesId = F.FeesId
                    JOIN SchoolTerm AS T ON F.TermId = T.TermId
                    JOIN Students as S on S.StudentID = F.studentId
                    JOIN Classes as C on C.ClassID = S.CurrentClassID 
                    JOIN FeesCategory as FC on FC.CategoryId = F.fees_Category
                    JOIN AllSchools as SCH on SCH.SchoolId = F.SchoolId
                    WHERE P.TransanctionId IN ({0})
                    ORDER BY P.datePaid";

                string parameterNames = string.Join(",", ids.Select((id, index) => "@id{index}"));
                query = string.Format(query, parameterNames);

                using (var command = new SqlCommand(query, connection))
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        command.Parameters.Add("@id{i}", SqlDbType.NVarChar).Value = ids[i];
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receipts.Add(CreateReceiptDataFromReader(reader));
                        }
                    }
                }
            }

            GenerateThermalReceipts();
        }
        catch (Exception ex)
        {
            lblError.Text = "Error loading receipts: " + ex.Message;
            lblError.Visible = true;
        }
    }

    private ReceiptData CreateReceiptDataFromReader(SqlDataReader reader)
    {
        return new ReceiptData
        {
            TransactionId = reader["TransanctionId"].ToString(),
            PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
            PaidBy = reader["paidBy"].ToString(),
            PaymentMode = reader["paymentMode"].ToString(),
            AmountPaid = Convert.ToDouble(reader["Paid"]),
            Balance = Convert.ToDouble(reader["Balance"]),
            Term = reader["TermName"].ToString(),
            StudentName = reader["FirstName"]  + " " + reader["Middlename"] + "" + reader["LastName"],
            PostedBy = reader["postedBy"].ToString(),
            Category = reader["CategoryName"].ToString(),
            ReceiptNumber = reader["receipNumber"].ToString(),
            ClassName = reader["ClassName"].ToString(),
            SchoolName = reader["School_name"].ToString(),
            PhoneNumber = reader["PhoneNumber"].ToString(),
            Slogan = reader["Slogan"].ToString(),
            Logo = reader["Logo"].ToString(),
            Address = reader["School_address"].ToString()
        };
    }

    private void GenerateThermalReceipts()
    {
        if (receipts.Count == 0)
        {
            lblError.Text = "No receipts found for the selected criteria";
            lblError.Visible = true;
            return;
        }

        foreach (var receipt in receipts)
        {
            AddThermalReceiptToContainer(receipt);
        }
    }

    private void AddThermalReceiptToContainer(ReceiptData receipt)
    {
        // Create receipt container
        HtmlGenericControl receiptDiv = new HtmlGenericControl("div");
        receiptDiv.Attributes["class"] = "receipt";

        // Header
        HtmlGenericControl headerDiv = new HtmlGenericControl("div");
        headerDiv.Attributes["class"] = "header";

        //school logo
        HtmlImage logo = new HtmlImage();
        logo.Src = "~/img/Schooldocs/" + receipt.Logo;
        logo.Alt = "School Logo";
        logo.Attributes["class"] = "logo";
        headerDiv.Controls.Add(logo);

        // School Name
        HtmlGenericControl schoolName = new HtmlGenericControl("div");
        schoolName.Attributes["class"] = "school-name";
        schoolName.InnerText = receipt.SchoolName.ToUpper();
        headerDiv.Controls.Add(schoolName);

        // School Address
        HtmlGenericControl schoolAddress = new HtmlGenericControl("div");
        schoolAddress.Attributes["class"] = "school-address";
        schoolAddress.InnerText = receipt.Address;
        headerDiv.Controls.Add(schoolAddress);

        // School Contact
        HtmlGenericControl schoolContact = new HtmlGenericControl("div");
        schoolContact.Attributes["class"] = "school-contact";
        schoolContact.InnerText = "Tel: " + receipt.PhoneNumber;
        headerDiv.Controls.Add(schoolContact);

        receiptDiv.Controls.Add(headerDiv);

        // Receipt Title
        HtmlGenericControl receiptTitle = new HtmlGenericControl("div");
        receiptTitle.Attributes["class"] = "receipt-title";
        receiptTitle.InnerText = "PAYMENT RECEIPT";
        receiptDiv.Controls.Add(receiptTitle);

        // Details Container
        HtmlGenericControl detailsDiv = new HtmlGenericControl("div");
        detailsDiv.Attributes["class"] = "details";

        // Add receipt details
        AddThermalDetailLine(detailsDiv, "Receipt No:", receipt.ReceiptNumber);
        AddThermalDetailLine(detailsDiv, "Date:", receipt.PaymentDate.ToString("dd/MM/yyyy"));
        //AddThermalDetailLine(detailsDiv, "Time:", receipt.PaymentDate.ToString("HH:mm"));
        AddThermalDetailLine(detailsDiv, "Cashier:", receipt.PostedBy);
        // Separator
        HtmlGenericControl separator1 = new HtmlGenericControl("div");
        separator1.Attributes["class"] = "separator";
        detailsDiv.Controls.Add(separator1);

        AddThermalDetailLine(detailsDiv, "Student:", receipt.StudentName);
        AddThermalDetailLine(detailsDiv, "Class:", receipt.ClassName);
        AddThermalDetailLine(detailsDiv, "Term:", receipt.Term);
        AddThermalDetailLine(detailsDiv, "Category:", receipt.Category);

        // Separator
        HtmlGenericControl separator2 = new HtmlGenericControl("div");
        separator2.Attributes["class"] = "separator";
        detailsDiv.Controls.Add(separator2);

        AddThermalDetailLine(detailsDiv, "Payment Mode:", receipt.PaymentMode);
        AddThermalDetailLine(detailsDiv, "Paid By:", receipt.PaidBy);

        // Separator
        HtmlGenericControl separator3 = new HtmlGenericControl("div");
        separator3.Attributes["class"] = "separator";
        detailsDiv.Controls.Add(separator3);

        // Amount
        AddThermalDetailLine(detailsDiv, "Amount Paid:", "MK" + receipt.AmountPaid);
        AddThermalDetailLine(detailsDiv, "Balance:", "MK" + receipt.Balance);
       

        receiptDiv.Controls.Add(detailsDiv);

        // Add Barcode/QR Code
        AddBarcode(receiptDiv, receipt);

        // Thank you message
        HtmlGenericControl thankYou = new HtmlGenericControl("div");
        thankYou.Attributes["class"] = "thank-you";
        thankYou.InnerText = "THANK YOU FOR YOUR PAYMENT!";
        receiptDiv.Controls.Add(thankYou);

        // Footer
        HtmlGenericControl footerDiv = new HtmlGenericControl("div");
        footerDiv.Attributes["class"] = "footer";
        footerDiv.InnerText = receipt.Slogan;
        receiptDiv.Controls.Add(footerDiv);

        // Add to container
        receiptsContainer.Controls.Add(receiptDiv);
    }

    private void AddThermalDetailLine(HtmlGenericControl container, string label, string value)
    {
        HtmlGenericControl line = new HtmlGenericControl("div");
        line.Attributes["class"] = "detail-line";

        HtmlGenericControl labelSpan = new HtmlGenericControl("span");
        labelSpan.Attributes["class"] = "label";
        labelSpan.InnerText = label;

        HtmlGenericControl valueSpan = new HtmlGenericControl("span");
        valueSpan.Attributes["class"] = "value";
        valueSpan.InnerText = value;

        line.Controls.Add(labelSpan);
        line.Controls.Add(valueSpan);
        container.Controls.Add(line);
    }

    private void AddBarcode(HtmlGenericControl container, ReceiptData receipt)
    {
        try
        {
            // For thermal printers, use simpler barcode data
            string barcodeData = receipt.ReceiptNumber;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(barcodeData, QRCodeGenerator.ECCLevel.L); // Lower ECC for smaller size
            QRCode qrCode = new QRCode(qrCodeData);

            // Smaller bitmap for thermal printer
            Bitmap qrCodeImage = qrCode.GetGraphic(2); // Smaller pixel size

            string base64String;
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                base64String = Convert.ToBase64String(ms.ToArray());
            }

            HtmlGenericControl barcodeContainer = new HtmlGenericControl("div");
            barcodeContainer.Attributes["class"] = "barcode-container";

            HtmlImage barcodeImg = new HtmlImage();
            barcodeImg.Src = "data:image/png;base64," + base64String;
            barcodeImg.Alt = "Receipt Barcode";
            barcodeImg.Attributes["class"] = "barcode";

            barcodeContainer.Controls.Add(barcodeImg);
            container.Controls.Add(barcodeContainer);
        }
        catch (Exception ex)
        {
            // If QR code fails, just show receipt number
            HtmlGenericControl barcodeText = new HtmlGenericControl("div");
            barcodeText.Attributes["class"] = "barcode-container";
            barcodeText.InnerText = "RECEIPT: " + receipt.ReceiptNumber;
            barcodeText.Style.Add("text-align", "center");
            barcodeText.Style.Add("font-weight", "bold");
            container.Controls.Add(barcodeText);
        }
    }

    private void UpdateSummary()
    {
        if (receipts.Count > 0)
        {
            double totalAmount = receipts.Sum(r => r.AmountPaid);
            lblSummary.Text = receipts.Count + " thermal receipts ready";
        }
        else
        {
            lblSummary.Text = "No receipts to print";
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("BulkReceipt.aspx");
    }
}

public class ReceiptData
{
    public string TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaidBy { get; set; }
    public string PaymentMode { get; set; }
    public double AmountPaid { get; set; }
    public double Balance { get; set; }
    public string Term { get; set; }
    public string StudentName { get; set; }
    public string PostedBy { get; set; }
    public string Category { get; set; }
    public string ReceiptNumber { get; set; }
    public string ClassName { get; set; }
    public string SchoolName { get; set; }
    public string PhoneNumber { get; set; }
    public string Slogan { get; set; }
    public string Logo { get; set; }
    public string Address { get; set; }
}