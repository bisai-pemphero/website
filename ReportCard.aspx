<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportCard.aspx.cs" Inherits="ReportCard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Report Card</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="keywords" content="sms" />
    <meta name="description" content="my School portal;" />
    <meta name='copyright' content='' />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="icon" href="img/favicon.png" />

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Pacifico&display=swap" rel="stylesheet"/>

    <style>
        :root {
            --primary-blue: #1e3a8a;
            --secondary-blue: #3b82f6;
            --light-blue: #eff6ff;
            --accent-gold: #f59e0b;
            --accent-green: #10b981;
            --dark-gray: #374151;
            --light-gray: #f9fafb;
            --border-radius: 10px;
            --shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
            --shadow-hover: 0 8px 24px rgba(0, 0, 0, 0.12);
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #f4f6f9 0%, #e8edf5 100%);
            color: #333;
            padding: 20px;
            min-height: 100vh;
        }

        .container {
            max-width: 1000px;
            margin: 0 auto;
        }

        /* Header with PDF button */
        .header-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 25px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .page-title {
            color: var(--primary-blue);
            font-size: 28px;
            font-weight: 700;
            letter-spacing: -0.5px;
        }

            .page-title i {
                margin-right: 10px;
                color: var(--accent-gold);
            }

        /* PDF Export Button */
        .pdf-button {
            background: linear-gradient(135deg, var(--primary-blue) 0%, var(--secondary-blue) 100%);
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: var(--border-radius);
            font-weight: 600;
            font-size: 16px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 10px;
            transition: all 0.3s ease;
            box-shadow: var(--shadow);
        }

            .pdf-button:hover {
                transform: translateY(-2px);
                box-shadow: var(--shadow-hover);
            }

            .pdf-button:active {
                transform: translateY(0);
            }

        /* Main Report Card */
        .report-card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            overflow: hidden;
            margin-bottom: 30px;
            border: 1px solid rgba(0, 0, 0, 0.05);
        }

        /* School Header */
        .school-header {
            background: linear-gradient(135deg, var(--primary-blue) 0%, #2563eb 100%);
            color: white;
            padding: 30px;
            display: flex;
            align-items: center;
            gap: 25px;
            border-bottom: 4px solid var(--accent-gold);
        }

        .school-logo-container {
            width: 100px;
            height: 100px;
            background: white;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
        }

        .school-logo {
            width: 80px;
            height: 80px;
            object-fit: contain;
        }

        .school-info {
            flex: 1;
        }

        .school-name {
            font-size: 28px;
            font-weight: 800;
            margin-bottom: 5px;
            letter-spacing: 0.5px;
        }

        .school-motto {
            font-size: 18px;
            font-weight: 500;
            margin-bottom: 10px;
            color: rgba(255, 255, 255, 0.9);
            font-style: italic;
        }

        .school-details {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            font-size: 14px;
            color: rgba(255, 255, 255, 0.85);
        }

            .school-details i {
                margin-right: 6px;
                color: var(--accent-gold);
            }

        /* Content sections */
        .section {
            padding: 25px;
            border-bottom: 1px solid #e5e7eb;
        }

            .section:last-of-type {
                border-bottom: none;
            }

        .section-title {
            color: var(--primary-blue);
            font-size: 20px;
            font-weight: 700;
            margin-bottom: 20px;
            padding-bottom: 8px;
            border-bottom: 2px solid var(--light-blue);
            display: flex;
            align-items: center;
            gap: 10px;
        }

            .section-title i {
                color: var(--secondary-blue);
            }

        /* Student Details Grid */
        .student-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
            gap: 15px;
        }

        .detail-item {
            background: var(--light-gray);
            padding: 15px;
            border-radius: 8px;
            border-left: 4px solid var(--secondary-blue);
        }

        .detail-label {
            font-size: 13px;
            color: var(--dark-gray);
            margin-bottom: 5px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .detail-value {
            font-size: 16px;
            font-weight: 600;
            color: var(--primary-blue);
        }

        /* Tables */
        .results-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            border-radius: 8px;
            overflow: hidden;
        }

            .results-table th {
                background: linear-gradient(to right, var(--primary-blue), var(--secondary-blue));
                color: white;
                text-align: left;
                padding: 16px 15px;
                font-weight: 600;
                font-size: 15px;
            }

            .results-table td {
                padding: 14px 15px;
                border-bottom: 1px solid #e5e7eb;
                font-size: 15px;
            }

            .results-table tbody tr:nth-child(even) {
                background-color: #f9fafb;
            }

            .results-table tbody tr:hover {
                background-color: var(--light-blue);
            }

        /* Grade badges */
        .grade-badge {
            display: inline-block;
            padding: 5px 12px;
            border-radius: 20px;
            font-weight: 700;
            font-size: 14px;
        }

        .grade-A {
            background-color: #d1fae5;
            color: #065f46;
        }

        .grade-B {
            background-color: #dbeafe;
            color: #1e40af;
        }

        .grade-C {
            background-color: #fef3c7;
            color: #92400e;
        }

        .grade-D {
            background-color: #fee2e2;
            color: #991b1b;
        }

        .grade-F {
            background-color: #f3f4f6;
            color: #374151;
        }

        /* Remarks */
        .remarks-container {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 25px;
        }

        @media (max-width: 768px) {
            .remarks-container {
                grid-template-columns: 1fr;
            }
        }

        .remark-box {
            background: var(--light-gray);
            padding: 20px;
            border-radius: 8px;
            border-top: 4px solid var(--secondary-blue);
        }

        .remark-title {
            font-weight: 700;
            color: var(--primary-blue);
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .remark-content {
            line-height: 1.6;
            color: var(--dark-gray);
            font-size: 15px;
            padding: 12px;
            background: white;
            border-radius: 6px;
            min-height: 80px;
        }

        /* Signature section */
        .signature-row {
            display: flex;
            justify-content: space-between;
            margin-top: 30px;
            flex-wrap: wrap;
            gap: 20px;
        }

        .signature-box {
            width: 45%;
            text-align: center;
        }

        @media (max-width: 768px) {
            .signature-box {
                width: 100%;
            }
        }

        .signature-line {
            border-top: 2px solid var(--dark-gray);
            width: 80%;
            margin: 0px auto 10px;
        }

        .signature-label {
            color: var(--primary-blue);
            font-weight: 600;
            margin-top: 5px;
            font-size: 15px;
        }

        .signature-date {
            color: var(--dark-gray);
            font-size: 14px;
            margin-top: 5px;
        }

        /* Footer */
        .footer {
            text-align: center;
            padding: 20px;
            color: #6b7280;
            font-size: 14px;
            border-top: 1px solid #e5e7eb;
            margin-top: 20px;
        }

            .footer i {
                color: var(--accent-green);
                margin: 0 5px;
            }

        /* Status indicators */
        .status-indicator {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
        }

        .status-excellent {
            background-color: #d1fae5;
            color: #065f46;
        }

        .status-good {
            background-color: #dbeafe;
            color: #1e40af;
        }

        /* Print styles */
        @media print {
            body {
                background: none;
                padding: 0;
            }

            .header-container {
                display: none;
            }

            .report-card {
                box-shadow: none;
                border: 1px solid #ddd;
            }

            .pdf-button {
                display: none;
            }
        }

        .signature-text {
    font-family: 'Pacifico', cursive;
    font-size: 24px;
    color: #000;
    margin-top: 10px;
}

    </style>


</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header-container">
                <h1 class="page-title"><i class="fas fa-graduation-cap"></i>Student Report Card</h1>
                <button id="exportPdf" class="pdf-button">
                    <i class="fas fa-file-pdf"></i> Save to Device
                </button>
                <asp:Button runat="server" CssClass="pdf-button" ID="btnBack" Text="Back" OnClick="btnBack_Click" />
            </div>

            <!-- Error -->
            <div>
                <asp:Label runat="server" ID="lblError" ForeColor="Red" Font-Bold="true"></asp:Label>
            </div>

            <div class="report-card" id="reportCard">
                <!-- School Header -->
                <div class="school-header">
                    <div class="school-logo-container">

                        <div class="school-logo" style="display: flex; align-items: center; justify-content: center; color: var(--primary-blue); font-size: 40px;">
                            <img id="imgLogo" runat="server" src="" class="logo" alt="Logo" style="height: 75px; width: 75px" />
                        </div>
                    </div>
                    <div class="school-info">
                        <h1 class="school-name" runat="server" id="lblSchoolName">ABC SECONDARY SCHOOL</h1>
                        <p class="school-motto" runat="server" id="lblSchoolMoto">"Knowledge Is Power"</p>
                        <div class="school-details">
                            <div runat="server" id="lblAddress"><i class="fas fa-map-marker-alt"></i>Private Bag 123, Lilongwe, Malawi</div>
                            <div runat="server" id="lblContact"><i class="fas fa-phone"></i>+265 999 123 456</div>
                            <div runat="server" id="lblEmail"><i class="fas fa-envelope"></i>info@abcschool.mw</div>
                        </div>
                    </div>
                </div>

                <!-- Student Details -->
                <div class="section">
                 
                    <div class="student-grid">
                        <div class="detail-item">
                            <div class="detail-label">Student Name</div>
                            <div class="detail-value" runat="server" id="lblstudentName">John Banda</div>
                        </div>

                        <div class="detail-item">
                            <div class="detail-label">Class</div>
                            <div class="detail-value" runat="server" id="lblClassName">Form 2</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Term</div>
                            <div class="detail-value" runat="server" id="lblTerm">Term 1</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Academic Year</div>
                            <div class="detail-value" runat="server" id="lblAcademicYear">2025</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Position In Class</div>
                            <div class="detail-value" runat="server" id="lblPosition"></div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Remarks</div>
                            <div class="detail-value" runat="server" id="lblRemarks"></div>
                        </div>

                    </div>
                </div>

                <!-- Subject Results -->
                <div class="section">
                   
                    <table class="results-table">
                        <thead>
                            <tr>
                                <th>Subject</th>
                                <th>Marks</th>
                                <th>Grade</th>
                                <th>Remark</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% LoadMarks(); %>
                        </tbody>
                    </table>
                </div>

                <!-- Grading System -->
                <div class="section">
                    <h2 class="section-title"><i class="fas fa-star"></i>Grading System</h2>
                    <table class="results-table">
                        <thead>
                            <tr>
                                <th>Grade</th>
                                <th>Minmum Mark</th>
                                <th>Maxmum Mark</th>
                                <th>Interpretation</th>
                            </tr>
                        </thead>
                        <tbody>
                              <% LoadGradingSystem(); %>
                        </tbody>
                    </table>
                </div>

                <!-- Remarks -->
                <div class="section">
                    <h2 class="section-title"><i class="fas fa-comment-dots"></i>Remarks</h2>

                    <div class="remarks-container">
                        <div class="remark-box">
                            <div class="remark-title"><i class="fas fa-chalkboard-teacher"></i>Class Teacher's Remark</div>
                            <div class="remark-content" runat="server" id="lblteachersRemarks">
                            NA
                            </div>
                        </div>

                        <div class="remark-box">
                            <div class="remark-title"><i class="fas fa-user-tie"></i>Head Teacher's Remark</div>
                            <div class="remark-content" runat="server" id="lblheadteachersRemarks">
                                NA
                            </div>
                        </div>
                    </div>

                    <!-- Signatures -->
                    <div class="signature-row">
                        <div class="signature-box">
                             <p runat="server" id="lblTeacherSignature" class="signature-text"></p>
                           <div class="signature-line"></div>
                            <div class="signature-label">Class Teacher's Signature</div>
                            <div class="signature-date" runat="server" id="lblDate"></div>
                        </div>
                        <div class="signature-box">
                          <p runat="server" id="lblheadteacherSignature" class="signature-text"></p> 
                            <div class="signature-line"></div>
                            <div class="signature-label">Head Teacher's Signature</div>
                            <div class="signature-date" runat="server" id="lblDate2"></div>
                        </div>
                    </div>
                </div>

                <asp:Label runat="server" ID="lblClassId" Visible="false"></asp:Label>
                <br />
                <asp:Label runat="server" ID="lblEnglishId" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblTotal" Visible="false"></asp:Label>
                  <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                <!-- Footer -->
                <div class="footer">
                    <p><i class="fas fa-exclamation-circle"></i>This document is system generated and does not require a stamp.</p>
                    <p runat="server" id="lblfooterSchoolName"></p>
                </div>
            </div>
        </div>

    </form>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const exportButton = document.getElementById('exportPdf');

            exportButton.addEventListener('click', function () {
                // Change button text to show loading state
                const originalText = exportButton.innerHTML;
                exportButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Generating PDF...';
                exportButton.disabled = true;

                // Generate PDF
                generatePDF();

                // Restore button after 2 seconds
                setTimeout(() => {
                    exportButton.innerHTML = originalText;
                    exportButton.disabled = false;
                }, 2000);
            });

            function generatePDF() {
                const { jsPDF } = window.jspdf;
                const element = document.getElementById('reportCard');

                // Use html2canvas to capture the report card as an image
                html2canvas(element, {
                    scale: 2, // Higher quality
                    useCORS: true,
                    logging: false,
                    backgroundColor: '#ffffff'
                }).then(canvas => {
                    const imgData = canvas.toDataURL('image/png');
                    const pdf = new jsPDF('p', 'mm', 'a4');
                    const pdfWidth = pdf.internal.pageSize.getWidth();
                    const pdfHeight = pdf.internal.pageSize.getHeight();

                    // Calculate dimensions to fit the content
                    const imgWidth = canvas.width;
                    const imgHeight = canvas.height;
                    const ratio = Math.min(pdfWidth / imgWidth, pdfHeight / imgHeight) * 0.95;
                    const imgX = (pdfWidth - imgWidth * ratio) / 2;
                    const imgY = 10;

                    // Add the image to PDF
                    pdf.addImage(imgData, 'PNG', imgX, imgY, imgWidth * ratio, imgHeight * ratio);

                    //// Add watermark
                    //const totalPages = pdf.internal.getNumberOfPages();
                    //for (let i = 1; i <= totalPages; i++) {
                    //    pdf.setPage(i);
                    //    pdf.setFontSize(10);
                    //    pdf.setTextColor(150, 150, 150);
                    //    pdf.text('ABC Secondary School - Official Report Card', pdfWidth / 2, pdfHeight - 10, { align: 'center' });
                    //    pdf.text(`Page ${i} of ${totalPages}`, pdfWidth - 20, pdfHeight - 10);
                    //}

                    // Save the PDF
                    pdf.save('ReportCard.pdf');

                    // Show success message
                    showNotification('PDF generated successfully!', 'success');
                }).catch(error => {
                    console.error('Error generating PDF:', error);
                    showNotification('Failed to generate PDF. Please try again.', 'error');
                });
            }

            function showNotification(message, type) {
                // Create notification element
                const notification = document.createElement('div');
                notification.style.cssText = `
                 position: fixed;
                 top: 20px;
                 right: 20px;
                 padding: 15px 20px;
                 border-radius: 8px;
                 color: white;
                 font-weight: 600;
                 z-index: 10000;
                 box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                 animation: slideIn 0.3s ease-out;
             `;

                if (type === 'success') {
                    notification.style.backgroundColor = '#10b981';
                } else {
                    notification.style.backgroundColor = '#ef4444';
                }

                notification.textContent = message;
                document.body.appendChild(notification);

                // Remove notification after 3 seconds
                setTimeout(() => {
                    notification.style.animation = 'slideOut 0.3s ease-in';
                    setTimeout(() => {
                        document.body.removeChild(notification);
                    }, 300);
                }, 3000);
            }

            // Add CSS for notification animations
            const style = document.createElement('style');
            style.textContent = `
             @keyframes slideIn {
                 from { transform: translateX(100%); opacity: 0; }
                 to { transform: translateX(0); opacity: 1; }
             }
             @keyframes slideOut {
                 from { transform: translateX(0); opacity: 1; }
                 to { transform: translateX(100%); opacity: 0; }
             }
         `;
            document.head.appendChild(style);
        });
    </script>
</body>
</html>
