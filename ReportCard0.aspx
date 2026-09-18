<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportCard0.aspx.cs" Inherits="ReportCard" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report Card</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=yes" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    
    <!-- Critical CSS inline - Minimal, optimized for mobile -->
    <style>
        /* CSS Reset & Variables - Minimal footprint */
        *{margin:0;padding:0;box-sizing:border-box}
        :root{--primary:#1e3a8a;--secondary:#3b82f6;--gold:#f59e0b;--light:#f9fafb;--gray:#6b7280;--radius:8px}
        
        body{font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,sans-serif;background:#f0f4f8;padding:12px;line-height:1.5}
        .container{max-width:800px;margin:0 auto}
        
        /* Mobile-First Header */
        .header-flex{display:flex;flex-wrap:wrap;gap:12px;justify-content:space-between;align-items:center;margin-bottom:16px}
        .page-title{color:var(--primary);font-size:clamp(1.2rem,5vw,1.8rem);font-weight:700;display:flex;align-items:center;gap:8px}
        .page-title i{color:var(--gold)}
        
        /* Button - Optimized touch target */
        .btn{background:linear-gradient(145deg,var(--primary),var(--secondary));color:#fff;border:none;padding:12px 20px;border-radius:var(--radius);
             font-weight:600;font-size:clamp(14px,4vw,16px);display:inline-flex;align-items:center;gap:8px;cursor:pointer;
             box-shadow:0 2px 8px rgba(0,0,0,0.1);transition:all 0.2s;min-height:44px;border:1px solid rgba(255,255,255,0.2)}
        .btn:hover,.btn:active{background:var(--primary);transform:translateY(-1px)}
        .btn i{font-size:1.1em}
        
        /* Report Card - Optimized for mobile */
        .report-card{background:#fff;border-radius:var(--radius);box-shadow:0 4px 12px rgba(0,0,0,0.05);overflow:hidden;margin-bottom:20px}
        
        /* School Header - Mobile optimized */
        .school-header{background:linear-gradient(145deg,var(--primary),#2563eb);color:#fff;padding:clamp(16px,4vw,24px);display:flex;
                       flex-wrap:wrap;gap:16px;align-items:center;border-bottom:3px solid var(--gold)}
        .school-logo-wrap{width:clamp(60px,15vw,80px);height:clamp(60px,15vw,80px);background:#fff;border-radius:50%;display:flex;
                          align-items:center;justify-content:center;flex-shrink:0;box-shadow:0 4px 8px rgba(0,0,0,0.2)}
        .school-logo{width:clamp(45px,12vw,65px);height:clamp(45px,12vw,65px);object-fit:contain}
        .school-info{flex:1;min-width:200px}
        .school-name{font-size:clamp(1.2rem,5vw,1.8rem);font-weight:800;margin-bottom:4px}
        .school-motto{font-size:clamp(0.85rem,4vw,1rem);opacity:0.9;margin-bottom:8px;font-style:italic}
        .school-details{display:flex;flex-wrap:wrap;gap:clamp(8px,3vw,16px);font-size:clamp(0.75rem,3.5vw,0.85rem)}
        
        /* Student Grid - Mobile first, switches to grid on larger screens */
        .student-grid{display:flex;flex-direction:column;gap:8px}
        @media (min-width:640px){.student-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr))}}
        .detail-item{background:var(--light);padding:clamp(12px,3vw,16px);border-radius:6px;border-left:4px solid var(--secondary)}
        .detail-label{font-size:clamp(0.7rem,3vw,0.75rem);color:var(--gray);text-transform:uppercase;letter-spacing:0.5px;margin-bottom:4px}
        .detail-value{font-size:clamp(1rem,4vw,1.1rem);font-weight:700;color:var(--primary);word-break:break-word}
        
        /* Section spacing */
        .section{padding:clamp(16px,4vw,24px);border-bottom:1px solid #eef2f6}
        .section:last-child{border-bottom:none}
        .section-title{color:var(--primary);font-size:clamp(1.1rem,5vw,1.3rem);font-weight:700;margin-bottom:16px;
                       padding-bottom:8px;border-bottom:2px solid #e6f0ff;display:flex;align-items:center;gap:8px}
        
        /* Table - Horizontal scroll on mobile, enhanced readability */
        .table-responsive{width:100%;overflow-x:auto;-webkit-overflow-scrolling:touch;margin:12px 0;border-radius:8px}
        .results-table{width:100%;border-collapse:collapse;font-size:clamp(13px,3.8vw,14px);min-width:450px}
        .results-table th{background:var(--primary);color:#fff;padding:clamp(10px,3vw,14px);text-align:left;font-weight:600}
        .results-table td{padding:clamp(10px,3vw,12px);border-bottom:1px solid #eef2f6}
        .results-table tbody tr:nth-child(even){background:#fafcff}
        
        /* Grade badges - Compact for mobile */
        .grade{display:inline-block;padding:4px 12px;border-radius:20px;font-weight:700;font-size:clamp(12px,3.5vw,13px)}
        .grade-A{background:#d1fae5;color:#065f46}
        .grade-B{background:#dbeafe;color:#1e40af}
        .grade-C{background:#fef3c7;color:#92400e}
        .grade-D{background:#fee2e2;color:#991b1b}
        .grade-F{background:#f3f4f6;color:#374151}
        
        /* Remarks grid - Stack on mobile */
        .remarks-grid{display:flex;flex-direction:column;gap:16px}
        @media (min-width:640px){.remarks-grid{display:grid;grid-template-columns:1fr 1fr}}
        .remark-box{background:var(--light);padding:16px;border-radius:8px;border-top:4px solid var(--secondary)}
        .remark-content{background:#fff;padding:12px;border-radius:6px;min-height:60px;font-size:clamp(13px,4vw,14px)}
        
        /* Signatures - Stack on mobile */
        .signature-flex{display:flex;flex-direction:column;gap:20px;margin-top:24px}
        @media (min-width:640px){.signature-flex{flex-direction:row;justify-content:space-between}}
        .signature-box{width:100%;text-align:center}
        @media (min-width:640px){.signature-box{width:45%}}
        .signature-line{border-top:2px solid #333;width:80%;margin:10px auto}
        .signature-text{font-family:'Pacifico',cursive;font-size:clamp(18px,5vw,22px);margin:8px 0}
        
        /* Footer */
        .footer{text-align:center;padding:16px;color:var(--gray);font-size:clamp(11px,3.5vw,12px);border-top:1px solid #eef2f6}
        
        /* Print styles - Optimized */
        @media print{.header-flex{display:none}.report-card{box-shadow:none;border:1px solid #ddd}}
        
        /* Notification - Mobile friendly */
        .toast{position:fixed;top:16px;right:16px;left:16px;margin:0 auto;max-width:320px;padding:12px 16px;background:#10b981;
               color:#fff;border-radius:8px;font-weight:500;box-shadow:0 4px 12px rgba(0,0,0,0.15);z-index:10000;
               animation:slideIn 0.3s;display:flex;align-items:center;gap:8px}
        .toast.error{background:#ef4444}
        @keyframes slideIn{from{transform:translateY(-100%);opacity:0}to{transform:translateY(0);opacity:1}}
        @keyframes slideOut{from{transform:translateY(0);opacity:1}to{transform:translateY(-100%);opacity:0}}
        
        /* Utility */
        .hidden{display:none}
        .w-100{width:100%}
        .text-center{text-align:center}
    </style>
    
   
    <!-- Favicon -->
    <link rel="icon" href="data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><text y=%22.9em%22 font-size=%2290%22>🎓</text></svg>"/>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Header with Buttons -->
            <div class="header-flex">
                <h1 class="page-title"><i class="fas fa-graduation-cap"></i>Report Card</h1>
                <div style="display:flex; gap:8px; width:100%; max-width:320px;">
                    <button type="button" id="exportPdf" class="btn" style="flex:2">
                        <i class="fas fa-file-pdf"></i><span>Save PDF</span>
                    </button>
                    <asp:Button runat="server" CssClass="btn" ID="btnBack" Text="Back" OnClick="btnBack_Click" style="flex:1" />
                </div>
            </div>

            <!-- Error Message -->
            <asp:Label runat="server" ID="lblError" ForeColor="Red" Font-Bold="true" CssClass="w-100" style="display:block; margin-bottom:12px;"></asp:Label>

            <!-- Main Report Card -->
            <div class="report-card" id="reportCard">
                <!-- School Header -->
                <div class="school-header">
                    <div class="school-logo-wrap">
                        <img id="imgLogo" runat="server" src="" class="school-logo" alt="Logo" loading="lazy" />
                    </div>
                    <div class="school-info">
                        <h1 class="school-name" runat="server" id="lblSchoolName">ABC SECONDARY SCHOOL</h1>
                        <p class="school-motto" runat="server" id="lblSchoolMoto">"Knowledge Is Power"</p>
                        <div class="school-details">
                            <span runat="server" id="lblAddress"><i class="fas fa-map-marker-alt"></i> Private Bag 123</span>
                            <span runat="server" id="lblContact"><i class="fas fa-phone"></i> +265 999 123 456</span>
                            <span runat="server" id="lblEmail"><i class="fas fa-envelope"></i> info@school.mw</span>
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
                            <div class="detail-label">Year</div>
                            <div class="detail-value" runat="server" id="lblAcademicYear">2025</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Position</div>
                            <div class="detail-value" runat="server" id="lblPosition">-</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Remarks</div>
                            <div class="detail-value" runat="server" id="lblRemarks">-</div>
                        </div>
                    </div>
                </div>

                <!-- Subject Results -->
                <div class="section">
                    <div class="table-responsive">
                        <table class="results-table">
                            <thead>
                                <tr><th>Subject</th><th>Marks</th><th>Grade</th><th>Remark</th></tr>
                            </thead>
                            <tbody><% LoadMarks(); %></tbody>
                        </table>
                    </div>
                </div>

                <!-- Grading System -->
                <div class="section">
                    <h2 class="section-title"><i class="fas fa-star"></i>Grading System</h2>
                    <div class="table-responsive">
                        <table class="results-table">
                            <thead>
                                <tr><th>Grade</th><th>Min</th><th>Max</th><th>Interpretation</th></tr>
                            </thead>
                            <tbody><% LoadGradingSystem(); %></tbody>
                        </table>
                    </div>
                </div>

                <!-- Remarks & Signatures -->
                <div class="section">
                    <h2 class="section-title"><i class="fas fa-comment-dots"></i>Remarks</h2>
                    <div class="remarks-grid">
                        <div class="remark-box">
                            <div class="remark-title"><i class="fas fa-chalkboard-teacher"></i> Class Teacher</div>
                            <div class="remark-content" runat="server" id="lblteachersRemarks">NA</div>
                        </div>
                        <div class="remark-box">
                            <div class="remark-title"><i class="fas fa-user-tie"></i> Head Teacher</div>
                            <div class="remark-content" runat="server" id="lblheadteachersRemarks">NA</div>
                        </div>
                    </div>

                    <div class="signature-flex">
                        <div class="signature-box">
                            <p runat="server" id="lblTeacherSignature" class="signature-text"></p>
                            <div class="signature-line"></div>
                            <div class="detail-label">Class Teacher</div>
                            <div class="detail-value" style="font-size:0.9rem;" runat="server" id="lblDate"></div>
                        </div>
                        <div class="signature-box">
                            <p runat="server" id="lblheadteacherSignature" class="signature-text"></p>
                            <div class="signature-line"></div>
                            <div class="detail-label">Head Teacher</div>
                            <div class="detail-value" style="font-size:0.9rem;" runat="server" id="lblDate2"></div>
                        </div>
                    </div>
                </div>

                <!-- Hidden Fields -->
                <asp:Label runat="server" ID="lblClassId" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblEnglishId" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblTotal" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>

                <!-- Footer -->
                <div class="footer">
                    <p><i class="fas fa-exclamation-circle"></i> System generated - No stamp required</p>
                    <p runat="server" id="lblfooterSchoolName"></p>
                </div>
            </div>
        </div>
    </form>

    <!-- Optimized PDF Script - Lazy loaded, smaller footprint -->
    <script>
        (function() {
            'use strict';
            
            let exportButton = document.getElementById('exportPdf');
            if (!exportButton) return;

            // Lazy load heavy PDF libraries only when needed
            function loadPDFLibraries(callback) {
                const jsPDFLoaded = !!window.jspdf;
                const html2canvasLoaded = !!window.html2canvas;
                
                if (jsPDFLoaded && html2canvasLoaded) {
                    callback();
                    return;
                }

                let loadedCount = 0;
                
                function checkLoaded() {
                    loadedCount++;
                    if (loadedCount === 2) callback();
                }

                // Load jsPDF
                if (!jsPDFLoaded) {
                    const script1 = document.createElement('script');
                    script1.src = 'https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js';
                    script1.integrity = 'sha512-x5+f8QpPlm9sL5I2bJiCMwWk33nJNljisI4Vg7pNpC8Ob+eBjq0GqUhsDR3R6NaOyxI4O2sX3O/UmO0p9KIDtg==';
                    script1.crossOrigin = 'anonymous';
                    script1.referrerPolicy = 'no-referrer';
                    script1.onload = checkLoaded;
                    document.head.appendChild(script1);
                }

                // Load html2canvas
                if (!html2canvasLoaded) {
                    const script2 = document.createElement('script');
                    script2.src = 'https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js';
                    script2.integrity = 'sha512-BNaRQnYJfiPSkHXLkM4L/Wq/6Z8kY89+W4F8Y2Zab32gcAIPLK/xS9M1MzFUHpE1e7y9s8jW6iW8gNPIcdHjLg==';
                    script2.crossOrigin = 'anonymous';
                    script2.referrerPolicy = 'no-referrer';
                    script2.onload = checkLoaded;
                    document.head.appendChild(script2);
                }
            }

            // Optimized PDF generation with compression
            function generateOptimizedPDF() {
                const element = document.getElementById('reportCard');
                
                // Show loading state
                const originalHTML = exportButton.innerHTML;
                exportButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Generating...';
                exportButton.disabled = true;

                loadPDFLibraries(function() {
                    const { jsPDF } = window.jspdf;
                    
                    // Use lower scale for smaller file size, but still readable on mobile
                    html2canvas(element, {
                        scale: window.innerWidth < 600 ? 1.5 : 1.8, // Optimize for mobile vs desktop
                        useCORS: true,
                        logging: false,
                        backgroundColor: '#ffffff',
                        allowTaint: false,
                        imageTimeout: 15000
                    }).then(canvas => {
                        const imgData = canvas.toDataURL('image/jpeg', 0.85); // JPEG with compression
                        const pdf = new jsPDF({
                            orientation: 'p',
                            unit: 'mm',
                            format: 'a4',
                            compress: true // Enable compression
                        });
                        
                        const pdfWidth = pdf.internal.pageSize.getWidth();
                        const pdfHeight = pdf.internal.pageSize.getHeight();
                        
                        const imgWidth = canvas.width;
                        const imgHeight = canvas.height;
                        const ratio = Math.min(pdfWidth / imgWidth, pdfHeight / imgHeight) * 0.95;
                        const imgX = (pdfWidth - imgWidth * ratio) / 2;
                        
                        pdf.addImage(imgData, 'JPEG', imgX, 10, imgWidth * ratio, imgHeight * ratio);
                        pdf.save('ReportCard.pdf');
                        
                        // Restore button
                        exportButton.innerHTML = originalHTML;
                        exportButton.disabled = false;
                        
                        showToast('PDF saved successfully', 'success');
                    }).catch(error => {
                        console.error('PDF Error:', error);
                        exportButton.innerHTML = originalHTML;
                        exportButton.disabled = false;
                        showToast('Failed to generate PDF', 'error');
                    });
                });
            }

            // Mobile-friendly toast notification
            function showToast(message, type = 'success') {
                const toast = document.createElement('div');
                toast.className = `toast ${type}`;
                toast.setAttribute('role', 'alert');
                toast.innerHTML = `<i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>${message}`;
                
                document.body.appendChild(toast);
                
                setTimeout(() => {
                    toast.style.animation = 'slideOut 0.3s forwards';
                    setTimeout(() => toast.remove(), 300);
                }, 3000);
            }

            // Event listener
            exportButton.addEventListener('click', generateOptimizedPDF);
        })();
    </script>

    <!-- Grade badge styling script (lightweight) -->
    <script>
        (function () {
            // Auto-apply grade classes to grade cells - keeps markup clean
            window.addEventListener('DOMContentLoaded', function () {
                document.querySelectorAll('.results-table td:nth-child(3)').forEach(function (cell) {
                    const grade = cell.textContent.trim().toUpperCase();
                    if (grade === 'A' || grade === 'A+') cell.innerHTML = `<span class="grade grade-A">${grade}</span>`;
                    else if (grade === 'B' || grade === 'B+') cell.innerHTML = `<span class="grade grade-B">${grade}</span>`;
                    else if (grade === 'C') cell.innerHTML = `<span class="grade grade-C">${grade}</span>`;
                    else if (grade === 'D') cell.innerHTML = `<span class="grade grade-D">${grade}</span>`;
                    else if (grade === 'F') cell.innerHTML = `<span class="grade grade-F">${grade}</span>`;
                    else cell.innerHTML = `<span class="grade">${grade}</span>`;
                });
            });
        })();
    </script>
</body>
</html>