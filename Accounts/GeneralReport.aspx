<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GeneralReport.aspx.cs" Inherits="Test" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
     <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Financial Report</title>

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">

    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>

    <style>
        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            background-color: #f4f6f9;
        }

        .page-header {
            background: linear-gradient(90deg, #007bff, #0056b3);
            color: white;
            padding: 25px 20px;
            border-radius: 8px;
            margin-bottom: 25px;
            text-align: center;
            box-shadow: 0px 3px 6px rgba(0,0,0,0.1);
        }

        .page-header h1 {
            font-size: 30px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .card {
            border-radius: 12px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.05);
            margin-bottom: 25px;
        }

        .card-header {
            background-color: #f8f9fa;
            font-weight: 600;
            font-size: 16px;
            color: #333;
        }

        .action-bar {
            margin-bottom: 20px;
            text-align: right;
        }

        .btn i {
            margin-right: 6px;
        }

        table th {
            background-color: #007bff;
            color: #fff;
            font-size: 14px;
        }

        table td {
            font-size: 13px;
        }

        .summary-label {
            font-weight: 600;
            font-size: 15px;
            margin-top: 8px;
        }

        .summary-label span {
            font-weight: bold;
            margin-left: 5px;
        }

        .summary-blue { color: #007bff; }
        .summary-green { color: #28a745; }
        .summary-red { color: #dc3545; }
    </style>
</head>
<body>

    <div class="container my-4" id="container_content">

        <!-- Header -->
        <div class="page-header">
            <h1>Financial Report</h1>
            <p>Date Range: <asp:Label runat="server" ID="lblDateRange"></asp:Label></p>
        </div>

        <!-- Action Bar -->
       <!-- Action Bar -->
<div class="action-bar">
    <button id="download-pdf" type="button" class="btn btn-primary" onclick="generatePDF()">
        <i class="fa-solid fa-file-pdf"></i> Download PDF
    </button>
    <button id="download-csv" type="button" class="btn btn-success" onclick="exportToCSV()">
        <i class="fa-solid fa-file-excel"></i> Export to Excel
    </button>
    <a class="btn btn-secondary" href="Dashboard.aspx">
        <i class="fa-solid fa-arrow-left"></i> Back to Dashboard
    </a>
</div>


        <form runat="server">
            <asp:Label runat="server" ID="lblError"></asp:Label>
            <!-- Filters -->
            <div class="card">
                <div class="card-header">Filter Options</div>
                <div class="card-body">
                    <div class="form-row">
                        <div class="form-group col-md-3">
                            <label for="txtStart">Start Date</label>
                            <asp:TextBox CssClass="form-control" TextMode="Date" ID="txtStart" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3">
                            <label for="txtEnd">End Date</label>
                            <asp:TextBox CssClass="form-control" TextMode="Date" ID="txtEnd" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3">
                            <label for="drpAcademicYear">Academic Year</label>
                            <asp:DropDownList CssClass="form-control" ID="drpAcademicYear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpAcademicYear_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3">
                            <label for="drpTerm">Term</label>
                            <asp:DropDownList CssClass="form-control" ID="drpTerm" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpTerm_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Summary -->
            <div class="card">
                <div class="card-body">
                    <div class="summary-label summary-blue">Total Collected: 
                        <span><asp:Label runat="server" ID="lblTotal"></asp:Label></span>
                    </div>
                    <div class="summary-label summary-red">Petty Cash (Expenses): 
                        <span><asp:Label runat="server" ID="lblPettyCash"></asp:Label></span>
                    </div>
                    <div class="summary-label summary-green">Balance: 
                        <span><asp:Label runat="server" ID="lblCash"></asp:Label></span>
                    </div>
                </div>
            </div>

            <!-- Data Sections -->
            <div class="card">
                <div class="card-header">Fees Category Breakdown</div>
                <div class="card-body p-0">
                    <table class="table table-bordered mb-0">
                        <thead>
                            <tr>
                                <th>Category Name</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody id="FeeCategoryBreakdownPlaceholder" runat="server"></tbody>
                    </table>
                </div>
            </div>

            <div class="card">
                <div class="card-header">Payment Mode Breakdown</div>
                <div class="card-body p-0">
                    <table class="table table-bordered mb-0">
                        <thead>
                            <tr>
                                <th>Mode</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody id="paymentBreakdownPlaceholder" runat="server"></tbody>
                    </table>
                </div>
            </div>

            <div class="card">
                <div class="card-header">Class Breakdown</div>
                <div class="card-body p-0">
                    <table class="table table-bordered mb-0">
                        <thead>
                            <tr>
                                <th>Class Name</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody id="classBreakdownPlaceholder" runat="server"></tbody>
                    </table>
                </div>
            </div>

            <div class="card">
                <div class="card-header">General Fees Collection</div>
                <div class="card-body p-0">
                    <table class="table table-bordered mb-0">
                        <thead>
                            <tr>
                                <th>Full Name</th>
                                <th>Amount Paid</th>
                                <th>Fees For</th>
                                <th>Class</th>
                                <th>Payment Mode</th>
                                <th>Paid By</th>
                                <th>Date Paid</th>
                            </tr>
                        </thead>
                        <tbody id="generalReportPlaceholder" runat="server"></tbody>
                    </table>
                </div>
            </div>

            <div class="card">
                <div class="card-header">Petty Cash (Expenses)</div>
                <div class="card-body p-0">
                    <table class="table table-bordered mb-0">
                        <thead>
                            <tr>
                                <th>Description</th>
                                <th>Amount</th>
                                <th>Collected By</th>
                                <th>Date</th>
                            </tr>
                        </thead>
                        <tbody id="pettycashPlaceholder" runat="server"></tbody>
                    </table>
                </div>
            </div>

            <!-- Hidden Labels -->
            <asp:Label runat="server" ID="lblUser" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblTermId" Visible="false"></asp:Label>
        </form>
    </div>

    <!-- Scripts -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.2/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <script>
        function generatePDF() {
            var dateRange = document.getElementById('<%= lblDateRange.ClientID %>').innerText || 'NoDateRange';
        dateRange = dateRange.replace(/[\/\\:]/g, '-').trim();
        var filename = 'Financial_Report_as_of_' + dateRange + '.pdf';

            // Get the buttons container
            var actionBar = document.querySelector('.action-bar');
            if (actionBar) actionBar.style.display = 'none'; // Hide buttons

        const element = document.getElementById('container_content');
        var opt = {
            margin: 0.3,
            filename: filename,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
        };

        html2pdf().set(opt).from(element).save();
    }

        function exportToCSV() {
            var csv = [];

            // ✅ Loop through each card that contains a table
            var cards = document.querySelectorAll(".card");
            cards.forEach(function (card) {
                var header = card.querySelector(".card-header");
                var table = card.querySelector("table");

                if (header && table) {
                    // Add section title
                    csv.push('"' + header.innerText.trim() + '"');
                    csv.push(""); // blank line after title

                    // Extract table rows
                    var rows = table.querySelectorAll("tr");
                    for (var i = 0; i < rows.length; i++) {
                        var row = [], cols = rows[i].querySelectorAll("td, th");
                        for (var j = 0; j < cols.length; j++) {
                            var text = cols[j].innerText.replace(/"/g, '""').trim();
                            row.push('"' + text + '"');
                        }
                        csv.push(row.join(","));
                    }

                    // Add extra blank line between tables
                    csv.push("");
                    csv.push("");
                }
            });

            // ✅ Generate CSV file
            var csvContent = "data:text/csv;charset=utf-8," + csv.join("\n");
            var encodedUri = encodeURI(csvContent);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);

            var dateRange = document.getElementById('<%= lblDateRange.ClientID %>').innerText || 'NoDateRange';
            dateRange = dateRange.replace(/[\/\\:]/g, '-').trim();
            link.setAttribute("download", "Financial_Report_as_of_" + dateRange + ".csv");

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }


    </script>


</body>
</html>
