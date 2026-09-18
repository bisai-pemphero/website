<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StudentDetails.aspx.cs" Inherits="StudentDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>School</title>
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

    <!-- jsPDF library for PDF generation -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>


    <style>
        /*Inputs and buttons*/
        /* Form Container Styling */
        .form-container {
            background: var(--card);
            border-radius: var(--radius);
            box-shadow: var(--shadow);
            padding: 30px;
            margin-top: 20px;
        }

        .form-header {
            font-size: 1.5rem; font-weight: 600;  color: var(--text); margin-bottom: 25px;   padding-bottom: 15px; border-bottom: 1px solid #e5e7eb;        }

        /* Form Grid Layout */
        .form-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 20px;
        }

        /* Form Group Styling */
        .form-group {
            margin-bottom: 1.5rem;
        }

        .form-label {
            display: block;
            margin-bottom: 8px;
            font-weight: 500;
            color: var(--text);
            font-size: 14px;
        }

        .form-control {
            width: 100%;
            padding: 12px 16px 12px 44px;
            border: 1px solid #e5e7eb;
            border-radius: var(--radius);
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            transition: all 0.3s ease;
            background-color: #fff;
        }

            .form-control:focus {
                outline: none;
                border-color: var(--primary);
                box-shadow: 0 0 0 3px rgba(14, 165, 233, 0.1);
            }

        /* Input with Icon */
        .input-with-icon {
            position: relative;
        }

            .input-with-icon i {
                position: absolute;
                left: 16px;
                top: 50%;
                transform: translateY(-50%);
                color: var(--muted);
                font-size: 16px;
            }

        /* Password Toggle */
        .password-toggle {
            position: absolute;
            right: 16px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            color: var(--muted);
            cursor: pointer;
            padding: 0;
            font-size: 16px;
        }

            .password-toggle:hover {
                color: var(--primary);
            }

        /* Error Message Styling */
        .error-message {
            color: var(--red);
            font-size: 13px;
            margin-top: 5px;
            display: none;
        }

        /* Button Styling */
        .btn-primary {
            background: var(--primary);
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: var(--radius);
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

            .btn-primary:hover {
                background: #0d94d4;
                transform: translateY(-1px);
            }

        .btn-secondary {
            background: #e5e7eb;
            color: var(--text);
            border: none;
            padding: 12px 24px;
            border-radius: var(--radius);
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

            .btn-secondary:hover {
                background: #d1d5db;
                transform: translateY(-1px);
            }

        /* Button Container */
        .button-container {
            display: flex;
            gap: 15px;
            margin-top: 30px;
            justify-content: flex-end;
        }

        /* Responsive Adjustments */
        @media (max-width: 768px) {
            .form-grid {
                grid-template-columns: 1fr;
            }

            .button-container {
                flex-direction: column;
            }

            .btn-primary, .btn-secondary {
                width: 100%;
            }
        }


        /*for error*/
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

        /*Card*/
        /* School Details Card */
        .school-details-card {
            background: var(--card);
            border-radius: var(--radius);
            box-shadow: var(--shadow);
            overflow: hidden;
            margin-bottom: 30px;
        }

        .card-header {
            display: flex;
            align-items: center;
            padding: 25px;
            background: var(--primary);
            color: white;
            gap: 20px;
        }

        .school-logo-container {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            background: white;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        .school-logo {
            max-width: 100%;
            max-height: 100%;
            object-fit: contain;
        }

        .school-title h3 {
            font-size: 1.5rem;
            font-weight: 700;
            margin: 0 0 5px 0;
        }

        .school-title .slogan {
            font-size: 0.9rem;
            opacity: 0.9;
            margin: 0;
            font-style: italic;
        }

        .card-body {
            padding: 25px;
        }

        .details-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 20px;
        }

        .detail-group {
            margin-bottom: 15px;
        }

        .detail-label {
            display: block;
            font-size: 0.85rem;
            color: var(--muted);
            margin-bottom: 5px;
            font-weight: 500;
        }

            .detail-label i {
                margin-right: 8px;
                width: 18px;
                text-align: center;
            }

        .detail-value {
            margin: 0;
            padding: 8px 12px;
            background: #f8fafc;
            border-radius: var(--radius);
            font-size: 0.95rem;
            border-left: 3px solid var(--primary);
        }

        @media (max-width: 768px) {
            .card-header {
                flex-direction: column;
                text-align: center;
            }

            .details-grid {
                grid-template-columns: 1fr;
            }
        }

        .pdf-container {
            width: 210mm;
            min-height: 297mm;
            padding: 15mm;
            margin: 0 auto;
            background: white;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            box-sizing: border-box;
        }

        .pdf-header {
            text-align: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #4a6fc0;
        }

            .pdf-header h2 {
                color: #4a6fc0;
                margin: 0;
                font-size: 24px;
            }

            .pdf-header p {
                margin: 5px 0 0;
                color: #666;
            }

        .student-photo-section {
            text-align: center;
            margin: 15px 0;
        }

        .student-photo {
            width: 120px;
            height: 120px;
            border-radius: 5px;
            border: 1px solid #ddd;
            background-color: #f5f5f5;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            color: #999;
        }

        .details-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 12px;
            margin-bottom: 20px;
        }

        .detail-group {
            margin-bottom: 10px;
            break-inside: avoid;
        }

        .detail-label {
            font-weight: 600;
            color: #4a6fc0;
            margin-bottom: 4px;
            font-size: 14px;
        }

            .detail-label i {
                margin-right: 8px;
                width: 20px;
                text-align: center;
            }

        .detail-value {
            padding: 8px 12px;
            background: #f8fafc;
            border-radius: 4px;
            font-size: 14px;
            border-left: 3px solid #4a6fc0;
            min-height: 18px;
        }

        .signature-area {
            margin-top: 40px;
            display: flex;
            justify-content: space-between;
        }

        .signature-box {
            width: 200px;
            border-top: 1px solid #000;
            text-align: center;
            padding-top: 5px;
            font-size: 14px;
        }

        @media print {
            body * {
                visibility: hidden;
            }

            #pdfContent, #pdfContent * {
                visibility: visible;
            }

            #pdfContent {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                box-shadow: none;
            }
        }

        /* Button styles */
        .btn-download-pdf {
            background: #dc3545;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 6px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            margin-left: 10px;
        }

            .btn-download-pdf:hover {
                background: #bb2d3b;
                transform: translateY(-1px);
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
                            <asp:Label runat="server" ID="lblUser">
             
                            </asp:Label>
                        </div>
                        <div class="status"><i class="fa-solid fa-circle" style="font-size: 8px"></i>Online</div>
                    </div>
                </div>

                <nav class="nav">
                    <h5>Main Navigation</h5>

                    <div class="dropdown">
                        <a class="active" href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i>Dashboard</a>
                    </div>
                </nav>
            </aside>

            <!-- TOPBAR -->
            <header class="topbar">
                <div class="left">
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


            <!-- Content starts here  -->

            <main class="content">
                <div class="crumbs">
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a> <i class="fa-solid fa-angle-right"></i><a>Student</a>
                </div>

                <div class="form-container">
                   
                    <div id="pdfContent" class="pdf-container" style="display: none;">
                        <div class="pdf-header">
                            <h2>STUDENT INFORMATION RECORD</h2>
                            <p>Official School Document</p>
                        </div>

                        <div class="details-grid">
                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-id-card"></i>Admission #</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litAdmissionPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-user"></i>Full Name</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litNamePdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-venus-mars"></i>Gender</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litGenderPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-birthday-cake"></i>Date Of Birth</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litDobPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-graduation-cap"></i>Class</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litClassPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-users"></i>Parent Name</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litParentPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-phone"></i>Contacts</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litPhonePdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-envelope"></i>Email</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litEmailPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-map-marker-alt"></i>Address</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litAdressPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-heart"></i>Relationship</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litRelationshipPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-briefcase"></i>Occupation</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litOccupationPdf" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="detail-group">
                                <span class="detail-label"><i class="fas fa-calendar"></i>Admission Date</span>
                                <p class="detail-value">
                                    <asp:Literal ID="litAdmissionDatePdf" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>

                        <div style="text-align: center; margin-top: 30px; font-size: 12px; color: #666;">
                            Generated on:
                            <asp:Literal ID="litGenDate" runat="server"></asp:Literal>
                        </div>
                    </div>

                    <!-- School Card Details here (visible on screen) -->
                    <div class="school-details-card">
                        <div class="card-body">
                            <div class="details-grid">
                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-user"></i>Full Name</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litName" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-id-card"></i>Admission #</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litAdmission" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-venus-mars"></i>Gender</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litGender" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-birthday-cake"></i>Date Of Birth</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litDob" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-graduation-cap"></i>Class</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litClass" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-users"></i>Parent Name</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litParent" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-phone"></i>Contacts</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litPhone" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-envelope"></i>Email</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litEmail" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-map-marker-alt"></i>Address</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litAdress" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-heart"></i>Relationship</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litRelationship" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-briefcase"></i>Occupation</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litOccupation" runat="server"></asp:Literal>
                                    </p>
                                </div>
                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-calendar"></i>Admission Date</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litAdmissionDate" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="button-container" style="margin-bottom: 20px;">
                        <asp:Button ID="btnBack" runat="server" Text=" Back" CssClass="btn-primary" OnClick="btnBack_Click" />
                        <button type="button" id="btnDownloadPdf" class="btn-download-pdf">
                            <i class="fas fa-download"></i>Download as PDF
                        </button>
                    </div>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>
                    <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                </div>
            </main>
            <!-- Content ends here  -->
        </div>



        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
        <script src="../js/customJs.js"></script>

        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>


        <script>
            // Initialize jsPDF
            window.jsPDF = window.jspdf.jsPDF;

            document.getElementById('btnDownloadPdf').addEventListener('click', function () {
                // Show loading indicator
                Swal.fire({
                    title: 'Generating PDF',
                    text: 'Please wait...',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading()
                    }
                });

            // Copy values to PDF literals
                <% 
// This will be processed server-side to copy values
// In a real implementation, you might handle this differently
                %>

                // Make PDF content visible temporarily
                const pdfElement = document.getElementById('pdfContent');
                const originalDisplay = pdfElement.style.display;
                pdfElement.style.display = 'block';

                // Use html2canvas to capture the content
                html2canvas(pdfElement, {
                    scale: 2,
                    useCORS: true,
                    logging: false
                }).then(canvas => {
                    // Hide the PDF content again
                    pdfElement.style.display = originalDisplay;

                    // Create PDF
                    const imgData = canvas.toDataURL('image/jpeg', 1.0);
                    const pdf = new jsPDF('p', 'mm', 'a4');
                    const pdfWidth = pdf.internal.pageSize.getWidth();
                    const pdfHeight = pdf.internal.pageSize.getHeight();
                    const imgWidth = canvas.width;
                    const imgHeight = canvas.height;
                    const ratio = Math.min(pdfWidth / imgWidth, pdfHeight / imgHeight);
                    const imgX = (pdfWidth - imgWidth * ratio) / 2;
                    const imgY = 0;

                    pdf.addImage(imgData, 'JPEG', imgX, imgY, imgWidth * ratio, imgHeight * ratio);
                    pdf.save('student-record-' + new Date().toISOString().slice(0, 10) + '.pdf');

                    // Close loading indicator
                    Swal.close();
                }).catch(error => {
                    console.error('Error generating PDF:', error);
                    pdfElement.style.display = originalDisplay;
                    Swal.fire('Error', 'Could not generate PDF. Please try again.', 'error');
                });
            });

            //Custom Error Toast
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
