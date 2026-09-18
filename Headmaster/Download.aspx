<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="Download.aspx.cs" Inherits="AddSchool" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>School Management System - View Schools</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="keywords" content="sms" />
    <meta name="description" content="School Management System" />
    <meta name='copyright' content='' />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap" rel="stylesheet" />

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" integrity="sha512-SnH5WK+bZxgPHs44uWIX+LLJAJ9/2PkPKZ5QiAj6Ta86w+fsb2TkcmfRyVX3pBnMFcV7oQPJkl9QevSCWr3W6A==" crossorigin="anonymous" referrerpolicy="no-referrer" />

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />

    <link rel="stylesheet" href="../css/customCs.css" />

    <!-- PDF Export Library -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>

    <style>
        /* Table Styling to Match Dashboard */
        .data-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            margin: 20px 0;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
            background: #fff;
        }

            .data-table thead {
                background-color: #4e73df;
            }

            .data-table th {
                color: white;
                font-weight: 600;
                padding: 16px;
                text-align: left;
                font-size: 14px;
                border: none;
            }

            .data-table td {
                padding: 16px;
                border-bottom: 1px solid #e5e7eb;
                font-size: 14px;
                color: #374151;
            }

            .data-table tbody tr {
                transition: background-color 0.2s ease;
            }

                .data-table tbody tr:nth-child(even) {
                    background-color: #f9fafb;
                }

                .data-table tbody tr:hover {
                    background-color: #f3f4f6;
                }

                .data-table tbody tr:last-child td {
                    border-bottom: none;
                }

        .action-buttons {
            display: flex;
            gap: 8px;
        }

        .btn-icon {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 32px;
            height: 32px;
            border-radius: 6px;
            border: none;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-edit {
            background-color: #10b981;
            color: white;
        }

            .btn-edit:hover {
                background-color: #059669;
            }

        .btn-delete {
            background-color: #ef4444;
            color: white;
        }

            .btn-delete:hover {
                background-color: #dc2626;
            }

        .btn-view {
            background-color: #3b82f6;
            color: white;
        }

            .btn-view:hover {
                background-color: #2563eb;
            }

        .table-container {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
            padding: 20px;
            margin-top: 20px;
        }

        .table-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 1px solid #e5e7eb;
        }

        .table-title {
            font-size: 1.5rem;
            font-weight: 600;
            color: #111827;
        }

        .table-actions {
            display: flex;
            gap: 12px;
        }

        .btn-primary {
            background: #4e73df;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: 14px;
        }

            .btn-primary:hover {
                background: #2e59d9;
                transform: translateY(-1px);
            }

        .btn-export {
            background: #10b981;
        }

            .btn-export:hover {
                background: #059669;
            }

        .btn-pdf {
            background: #ef4444;
        }

            .btn-pdf:hover {
                background: #dc2626;
            }

        .status-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 100px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-active {
            background-color: #d1fae5;
            color: #065f46;
        }

        .status-inactive {
            background-color: #fee2e2;
            color: #991b1b;
        }

        /* Toast notification */
        .toast-error {
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 25px;
            background-color: #ffebee;
            color: #d32f2f;
            border-left: 5px solid #d32f2f;
            border-radius: 4px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            font-weight: bold;
            font-size: large;
            opacity: 0;
            transform: translateX(100%);
            transition: all 0.3s ease;
            z-index: 1000;
        }

            .toast-error.show {
                opacity: 1;
                transform: translateX(0);
            }

            .toast-error.hide {
                opacity: 0;
                transform: translateX(100%);
            }

        /* Empty state */
        .empty-state {
            text-align: center;
            padding: 40px 20px;
            color: #6b7280;
        }

            .empty-state i {
                font-size: 48px;
                margin-bottom: 16px;
                color: #d1d5db;
            }

            .empty-state p {
                font-size: 16px;
                margin-bottom: 20px;
            }

        /* PDF Report Styling */
        .pdf-header {
            text-align: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #4e73df;
            font-family: 'Inter', sans-serif;
        }

        .pdf-title {
            font-size: 24px;
            color: #4e73df;
            margin-bottom: 5px;
        }

        .pdf-subtitle {
            font-size: 14px;
            color: #6b7280;
        }

        .pdf-table {
            width: 100%;
            border-collapse: collapse;
            margin: 15px 0;
            font-size: 12px;
            font-family: 'Inter', sans-serif;
        }

            .pdf-table th {
                background-color: #4e73df;
                color: white;
                padding: 8px;
                text-align: left;
                border: 1px solid #ddd;
            }

            .pdf-table td {
                padding: 8px;
                border: 1px solid #ddd;
            }

        .pdf-footer {
            margin-top: 30px;
            text-align: right;
            font-size: 12px;
            color: #6b7280;
            font-family: 'Inter', sans-serif;
        }

        /* Hidden element for PDF generation */
        #pdfTemplate {
            display: none;
            padding: 20px;
            font-family: 'Inter', sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="app">
            <!-- SIDEBAR -->
            <aside class="sidebar" id="sidebar">
                <div class="brand">
                    <div class="logo"><i class="fa-solid fa-school"></i></div>
                    <h1>SMS</h1>
                </div>

                <div class="userpanel">
                    <div class="avatar">
                        <asp:Label runat="server" ID="lblInitials"></asp:Label>
                    </div>
                    <div>
                        <div style="font-weight: 600">
                            <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblUser"></asp:Label>
                        </div>
                        <div class="status"><i class="fa-solid fa-circle" style="font-size: 8px"></i>Online</div>
                    </div>
                </div>

                <nav class="nav">
                    <h5>Main Navigation</h5>

                    <div class="dropdown">
                        <a href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i>Dashboard</a>
                    </div>
                    <div class="dropdown">
                        <a class="active" href="#"><i class="fa-solid fa-school"></i>Schools </a>
                        <div class="dropdown-menu">
                            <a href="AddSchool.aspx">Add School</a>
                            <a href="ViewSchool.aspx">View Schools</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-table"></i>Licences</a>
                        <div class="dropdown-menu">
                            <a href="LicenceGen.aspx">New Lincence</a>
                         
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-chart-simple"></i>Payments</a>
                        <div class="dropdown-menu">
                            <a href="Invoice.aspx">Invoice</a>
                           <a href="Payments.aspx">Payment</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-user"></i>Users</a>
                        <div class="dropdown-menu">
                            <a href="#">Admin Users</a>
                            <a href="#">School Users</a>
                        </div>
                    </div>
                </nav>
            </aside>

            <!-- TOPBAR -->
            <header class="topbar">
                <div class="left">
                    <button class="icon-btn hamburger" id="btnHamburger" aria-label="Toggle sidebar">
                        <i class="fas fa-bars"></i>
                    </button>
                    <div style="font-weight: 700">View Schools</div>
                </div>
                <div class="right">
                    <div class="dropdown">
                        <div class="profile icon-btn" id="profileBtn" aria-label="Profile">
                            <img src="../img/user.png" alt="avatar" />
                        </div>
                        <div class="dropdown-menu" id="profileMenu">
                            <a href="#"><i class="fas fa-user"></i>Profile</a>
                            <a href="../CommonPages/Login.aspx"><i class="fas fa-sign-out-alt"></i>Sign out</a>
                        </div>
                    </div>
                </div>
            </header>

            <!-- CONTENT -->
            <main class="content">
                <div class="crumbs">
                    <i class="fa-solid fa-gauge"></i><a href="Dashboard.aspx">Dashboard</a>
                    <i class="fa-solid fa-angle-right"></i>
                    <i class="fa-solid fa-school"></i>View Schools
                </div>

                <div class="table-container">
                    <div class="table-header">
                        <div class="table-title">School Directory</div>
                        <div class="table-actions">
                            <asp:Button ID="btnAddNewSchool" runat="server" Text="Add New School" CssClass="btn-primary" OnClick="btnAddNewSchool_Click" />
                            <button type="button" class="btn-primary btn-export" onclick="exportToCSV()">
                                <i class="fas fa-file-export"></i>Export CSV
                            </button>
                          <%--  <button type="button" class="btn-primary btn-pdf" onclick="generatePDF()">
                                <i class="fas fa-file-pdf"></i>Export PDF
                            </button>--%>
                        </div>
                    </div>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>

                    <div class="table-responsive">
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>School Name</th>
                                    <th>Director</th>
                                    <th>Address</th>
                                    <th>Contact Number</th>
                                    <th>Email</th>
                                    <th>School Email</th>
                                    <th>School Contact</th>
                                </tr>
                            </thead>
                            <tbody id="generalReportPlaceholder" runat="server">
                                
                            </tbody>
                        </table>
                    </div>

                    <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                </div>
            </main>
        </div>

        <!-- Hidden template for PDF generation -->
        <div id="pdfTemplate">
            <div class="pdf-header">
                <div class="pdf-title">School Directory Report</div>
                <div class="pdf-subtitle">Generated on <span id="pdfDate"></span></div>
            </div>

            <table class="pdf-table" id="pdfTable">
                <thead>
                    <tr>
                        <th>School Name</th>
                        <th>Director</th>
                        <th>Address</th>
                        <th>Contact Number</th>
                        <th>Email</th>
                        <th>School Email</th>
                        <th>School Contact</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- PDF content will be populated here by JavaScript -->
                </tbody>
            </table>

            <div class="pdf-footer">
                Report generated by School Management System
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
        <script src="../js/customJs.js"></script>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <script>
            function exportToCSV() {
                let csv = [];
                const rows = document.querySelectorAll(".data-table tr");

                // Add headers
                const headers = Array.from(document.querySelectorAll(".data-table th"))
                    .map(header => `"${header.innerText.trim()}"`);
                csv.push(headers.join(","));

                // Add data rows
                for (let row of rows) {
                    if (row.querySelector('td')) {
                        let cols = row.querySelectorAll("td");
                        let rowData = Array.from(cols).map(col => `"${col.innerText.trim()}"`);
                        csv.push(rowData.join(","));
                    }
                }

                const csvContent = "data:text/csv;charset=utf-8," + csv.join("\n");
                const link = document.createElement("a");
                link.setAttribute("href", encodeURI(csvContent));
                link.setAttribute("download", "Schools.csv");
                document.body.appendChild(link);
                link.click();
            }

            function generatePDF() {
                // Show loading indicator
                Swal.fire({
                    title: 'Generating PDF',
                    text: 'Please wait while we prepare your report...',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading()
                    }
                });

                // Set the generation date
                document.getElementById('pdfDate').textContent = new Date().toLocaleDateString();

                // Copy table data to PDF template
                const pdfTableBody = document.querySelector('#pdfTable tbody');
                pdfTableBody.innerHTML = ''; // Clear previous content

                const rows = document.querySelectorAll('.data-table tbody tr');

                // Check if there's data to export
                if (rows.length === 0) {
                    Swal.close();
                    Swal.fire({
                        icon: 'warning',
                        title: 'No Data',
                        text: 'There is no school data to export.'
                    });
                    return;
                }

                // Copy data rows
                rows.forEach(row => {
                    const cols = row.querySelectorAll('td');
                    if (cols.length >= 7) { // Make sure we have enough columns
                        const newRow = document.createElement('tr');

                        // School Name
                        const nameCell = document.createElement('td');
                        nameCell.textContent = cols[0].textContent;
                        newRow.appendChild(nameCell);

                        // Director
                        const directorCell = document.createElement('td');
                        directorCell.textContent = cols[1].textContent;
                        newRow.appendChild(directorCell);

                        // Address
                        const addressCell = document.createElement('td');
                        addressCell.textContent = cols[2].textContent;
                        newRow.appendChild(addressCell);

                        // Contact Number
                        const contactCell = document.createElement('td');
                        contactCell.textContent = cols[3].textContent;
                        newRow.appendChild(contactCell);

                        // Email
                        const emailCell = document.createElement('td');
                        emailCell.textContent = cols[4].textContent;
                        newRow.appendChild(emailCell);

                        // School Email
                        const schoolEmailCell = document.createElement('td');
                        schoolEmailCell.textContent = cols[5].textContent;
                        newRow.appendChild(schoolEmailCell);

                        // School Contact
                        const schoolContactCell = document.createElement('td');
                        schoolContactCell.textContent = cols[6].textContent;
                        newRow.appendChild(schoolContactCell);

                        pdfTableBody.appendChild(newRow);
                    }
                });

                // Generate PDF after a short delay to allow DOM to update
                setTimeout(() => {
                    const element = document.getElementById('pdfTemplate');
                    const filename = 'Schools_' + new Date().toISOString().slice(0, 10) + '.pdf';

                    const options = {
                        margin: [10, 10, 10, 10],
                        filename: filename,
                        image: { type: 'jpeg', quality: 0.98 },
                        html2canvas: {
                            scale: 2,
                            useCORS: true,
                            logging: true,
                            letterRendering: true
                        },
                        jsPDF: {
                            unit: 'mm',
                            format: 'a4',
                            orientation: 'landscape'
                        }
                    };

                    html2pdf().set(options).from(element).save().then(() => {
                        Swal.close();
                    }).catch(error => {
                        Swal.close();
                        console.error('PDF generation error:', error);
                        Swal.fire({
                            icon: 'error',
                            title: 'PDF Generation Failed',
                            text: 'An error occurred while generating the PDF. Please try again.'
                        });
                    });
                }, 500);
            }

            

            // Custom Error Toast
            function showErrorToast(message) {
                var toast = document.getElementById('<%= lblError.ClientID %>');
                toast.innerHTML = message;
                toast.classList.remove('hide');
                toast.classList.add('show');

                // Auto-hide after 8 seconds
                setTimeout(function () {
                    hideErrorToast();
                }, 8000);
            }

            function hideErrorToast() {
                var toast = document.getElementById('<%= lblError.ClientID %>');
                toast.classList.remove('show');
                toast.classList.add('hide');
            }
        </script>
    </form>
</body>
</html>