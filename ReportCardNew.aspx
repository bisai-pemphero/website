<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportCard.aspx.cs" Inherits="ReportCard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report Card - Student Results</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=yes" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    
    <!-- Favicon -->
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    
    <!-- Font Awesome 6 -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=Pacifico&display=swap" rel="stylesheet" />
    
    <!-- PDF Libraries (Lazy Loaded) -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js" defer></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js" defer></script>

    <style>
        /* ===== EXAM PORTAL MODERN STYLING ===== */
        :root {
            --primary: #0ea5e9;
            --primary-dark: #0284c7;
            --primary-light: #e0f2fe;
            --secondary: #8b5cf6;
            --success: #10b981;
            --warning: #f59e0b;
            --danger: #ef4444;
            --bg: #f8fafc;
            --card: #ffffff;
            --text: #0f172a;
            --text-light: #64748b;
            --border: #e2e8f0;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.05);
            --shadow-md: 0 4px 12px rgba(0,0,0,0.05);
            --shadow-lg: 0 10px 30px rgba(0,0,0,0.05);
            --radius-sm: 12px;
            --radius: 20px;
            --radius-lg: 30px;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(145deg, #f8fafc, #f1f5f9);
            color: var(--text);
            min-height: 100vh;
            padding: 20px;
            line-height: 1.6;
        }

        .container {
            max-width: 1100px;
            margin: 0 auto;
        }

        /* ===== HEADER WITH GLASSMORPHISM ===== */
        .header-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            flex-wrap: wrap;
            gap: 15px;
            background: rgba(255, 255, 255, 0.9);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            padding: 18px 25px;
            border-radius: 50px;
            box-shadow: var(--shadow-md);
            border: 1px solid rgba(255,255,255,0.5);
        }

        .page-title {
            margin: 0;
            font-size: 1.6rem;
            font-weight: 800;
            background: linear-gradient(135deg, var(--primary-dark), var(--primary));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            letter-spacing: -0.5px;
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .page-title i {
            background: linear-gradient(135deg, var(--primary), var(--primary-dark));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            font-size: 1.8rem;
        }

        /* ===== MODERN BUTTONS ===== */
        .btn {
            background: white;
            color: var(--text);
            border: 1px solid var(--border);
            padding: 12px 24px;
            border-radius: 50px;
            font-weight: 600;
            font-size: 0.95rem;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 10px;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: var(--shadow-sm);
            text-decoration: none;
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: var(--shadow-lg);
            border-color: var(--primary);
            color: var(--primary-dark);
        }

        .btn-primary {
            background: linear-gradient(145deg, var(--primary), var(--primary-dark));
            color: white;
            border: none;
            box-shadow: 0 8px 16px -4px rgba(14, 165, 233, 0.3);
        }

        .btn-primary:hover {
            background: linear-gradient(145deg, var(--primary-dark), var(--primary));
            color: white;
            transform: translateY(-3px);
            box-shadow: 0 12px 20px -6px rgba(14, 165, 233, 0.4);
        }

        .btn-primary i {
            color: white;
        }

        /* ===== MAIN REPORT CARD - GLASS CARD ===== */
        .report-card {
            background: var(--card);
            border-radius: var(--radius);
            box-shadow: var(--shadow-lg);
            overflow: hidden;
            margin-bottom: 30px;
            border: 1px solid rgba(255,255,255,0.8);
            backdrop-filter: blur(10px);
            animation: fadeInUp 0.6s ease-out;
        }

        /* ===== SCHOOL HEADER - GRADIENT MODERN ===== */
        .school-header {
            background: linear-gradient(145deg, #1e3a8a, #0e5cad);
            color: white;
            padding: clamp(25px, 5vw, 35px);
            display: flex;
            align-items: center;
            gap: 30px;
            flex-wrap: wrap;
            border-bottom: 4px solid var(--warning);
            position: relative;
            overflow: hidden;
        }

        .school-header::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: radial-gradient(circle at 20% 50%, rgba(255,255,255,0.1) 0%, transparent 50%);
            pointer-events: none;
        }

        .school-logo-container {
            width: 110px;
            height: 110px;
            background: white;
            border-radius: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 15px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.2);
            border: 3px solid rgba(255,255,255,0.3);
            backdrop-filter: blur(5px);
            flex-shrink: 0;
        }

        .school-logo {
            width: 75px;
            height: 75px;
            object-fit: contain;
        }

        .school-info {
            flex: 1;
            position: relative;
            z-index: 2;
        }

        .school-name {
            font-size: clamp(1.5rem, 5vw, 2rem);
            font-weight: 800;
            margin-bottom: 8px;
            letter-spacing: -0.5px;
            text-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .school-motto {
            font-size: clamp(0.95rem, 3vw, 1.1rem);
            font-weight: 500;
            margin-bottom: 15px;
            color: rgba(255, 255, 255, 0.95);
            font-style: italic;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .school-motto i {
            color: var(--warning);
        }

        .school-details {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            font-size: 0.9rem;
            color: rgba(255, 255, 255, 0.9);
        }

        .school-details div {
            display: flex;
            align-items: center;
            gap: 8px;
            background: rgba(255,255,255,0.1);
            padding: 6px 16px;
            border-radius: 50px;
            backdrop-filter: blur(5px);
        }

        .school-details i {
            color: var(--warning);
        }

        /* ===== STUDENT DETAILS CARD ===== */
        .section {
            padding: clamp(20px, 4vw, 30px);
            border-bottom: 1px solid var(--border);
        }

        .section:last-of-type {
            border-bottom: none;
        }

        .section-title {
            color: var(--primary-dark);
            font-size: 1.3rem;
            font-weight: 700;
            margin-bottom: 25px;
            padding-bottom: 12px;
            border-bottom: 3px solid var(--primary-light);
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .section-title i {
            color: var(--primary);
            font-size: 1.5rem;
        }

        /* Student Grid - Modern Cards */
        .student-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 16px;
        }

        .detail-item {
            background: linear-gradient(145deg, var(--bg), white);
            padding: 18px;
            border-radius: 16px;
            border-left: 5px solid var(--primary);
            box-shadow: var(--shadow-sm);
            transition: all 0.3s ease;
            border: 1px solid var(--border);
        }

        .detail-item:hover {
            transform: translateY(-3px);
            box-shadow: var(--shadow-md);
            border-color: var(--primary-light);
        }

        .detail-label {
            font-size: 0.75rem;
            color: var(--text-light);
            margin-bottom: 6px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.8px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .detail-label i {
            color: var(--primary);
            font-size: 0.8rem;
        }

        .detail-value {
            font-size: 1.1rem;
            font-weight: 700;
            color: var(--text);
            word-break: break-word;
        }

        /* ===== MODERN TABLES ===== */
        .table-responsive {
            width: 100%;
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
            margin: 15px 0;
            border-radius: 16px;
            box-shadow: var(--shadow-sm);
        }

        .results-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.95rem;
            min-width: 500px;
            background: white;
        }

        .results-table th {
            background: linear-gradient(145deg, var(--primary), var(--primary-dark));
            color: white;
            padding: 16px 15px;
            text-align: left;
            font-weight: 600;
            font-size: 0.95rem;
            white-space: nowrap;
        }

        .results-table th:first-child {
            border-radius: 12px 0 0 0;
        }

        .results-table th:last-child {
            border-radius: 0 12px 0 0;
        }

        .results-table td {
            padding: 14px 15px;
            border-bottom: 1px solid var(--border);
            color: var(--text);
        }

        .results-table tbody tr {
            transition: background 0.2s ease;
        }

        .results-table tbody tr:nth-child(even) {
            background-color: #fafcff;
        }

        .results-table tbody tr:hover {
            background-color: var(--primary-light);
        }

        /* ===== GRADE BADGES - MODERN ===== */
        .grade-badge {
            display: inline-block;
            padding: 6px 16px;
            border-radius: 50px;
            font-weight: 700;
            font-size: 0.85rem;
            text-align: center;
            min-width: 70px;
            box-shadow: var(--shadow-sm);
        }

        .grade-A {
            background: linear-gradient(145deg, #d1fae5, #a7f3d0);
            color: #065f46;
            border: 1px solid #6ee7b7;
        }

        .grade-B {
            background: linear-gradient(145deg, #dbeafe, #bfdbfe);
            color: #1e40af;
            border: 1px solid #93c5fd;
        }

        .grade-C {
            background: linear-gradient(145deg, #fef3c7, #fde68a);
            color: #92400e;
            border: 1px solid #fcd34d;
        }

        .grade-D {
            background: linear-gradient(145deg, #fee2e2, #fecaca);
            color: #991b1b;
            border: 1px solid #fca5a5;
        }

        .grade-F {
            background: linear-gradient(145deg, #f3f4f6, #e5e7eb);
            color: #374151;
            border: 1px solid #d1d5db;
        }

        /* ===== REMARKS SECTION - MODERN CARDS ===== */
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
            background: linear-gradient(145deg, var(--bg), white);
            padding: 22px;
            border-radius: 20px;
            border-top: 6px solid var(--primary);
            box-shadow: var(--shadow-md);
            border: 1px solid var(--border);
        }

        .remark-title {
            font-weight: 700;
            color: var(--primary-dark);
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 1.1rem;
        }

        .remark-title i {
            color: var(--primary);
            font-size: 1.2rem;
        }

        .remark-content {
            line-height: 1.7;
            color: var(--text);
            font-size: 0.95rem;
            padding: 16px;
            background: white;
            border-radius: 14px;
            min-height: 100px;
            border: 1px solid var(--border);
            box-shadow: inset 0 2px 4px rgba(0,0,0,0.02);
        }

        /* ===== SIGNATURE SECTION ===== */
        .signature-row {
            display: flex;
            justify-content: space-between;
            margin-top: 35px;
            flex-wrap: wrap;
            gap: 25px;
        }

        .signature-box {
            flex: 1;
            min-width: 200px;
            text-align: center;
            background: linear-gradient(145deg, var(--bg), white);
            padding: 25px 20px;
            border-radius: 20px;
            box-shadow: var(--shadow-sm);
            border: 1px solid var(--border);
        }

        .signature-text {
            font-family: 'Pacifico', cursive;
            font-size: 1.8rem;
            color: var(--primary-dark);
            margin: 10px 0;
            border-bottom: 2px dashed var(--primary-light);
            padding-bottom: 15px;
        }

        .signature-line {
            border-top: 2px solid var(--border);
            width: 80%;
            margin: 15px auto;
        }

        .signature-label {
            color: var(--primary-dark);
            font-weight: 700;
            margin-top: 10px;
            font-size: 0.95rem;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .signature-date {
            color: var(--text-light);
            font-size: 0.85rem;
            margin-top: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .signature-date i {
            color: var(--primary);
        }

        /* ===== FOOTER ===== */
        .footer {
            text-align: center;
            padding: 25px;
            color: var(--text-light);
            font-size: 0.85rem;
            border-top: 1px solid var(--border);
            background: linear-gradient(145deg, var(--bg), white);
        }

        .footer i {
            color: var(--success);
            margin: 0 5px;
        }

        /* ===== PDF BUTTON SPECIAL ===== */
        .pdf-button {
            background: linear-gradient(145deg, var(--danger), #dc2626);
            color: white;
            border: none;
            padding: 12px 28px;
            border-radius: 50px;
            font-weight: 600;
            font-size: 0.95rem;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 12px;
            transition: all 0.3s ease;
            box-shadow: 0 8px 16px -4px rgba(220, 38, 38, 0.3);
        }

        .pdf-button:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 20px -6px rgba(220, 38, 38, 0.4);
        }

        .pdf-button i {
            color: white;
            font-size: 1.1rem;
        }

        /* ===== ERROR MESSAGE ===== */
        .error-message {
            background: linear-gradient(145deg, #fee2e2, #fecaca);
            color: #991b1b;
            padding: 15px 20px;
            border-radius: 50px;
            margin-bottom: 20px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 12px;
            border-left: 6px solid var(--danger);
            animation: shake 0.5s ease;
        }

        .error-message i {
            font-size: 1.2rem;
        }

        /* ===== ANIMATIONS ===== */
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-10px); }
            75% { transform: translateX(10px); }
        }

        @keyframes slideIn {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }

        @keyframes slideOut {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }

        /* ===== PRINT STYLES ===== */
        @media print {
            body {
                background: white;
                padding: 10px;
            }

            .header-container {
                display: none;
            }

            .report-card {
                box-shadow: none;
                border: 1px solid #ddd;
                animation: none;
            }

            .pdf-button, .btn {
                display: none;
            }

            .school-header {
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            .results-table th {
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
        }

        /* ===== MOBILE OPTIMIZATIONS ===== */
        @media (max-width: 640px) {
            body {
                padding: 12px;
            }

            .header-container {
                flex-direction: column;
                align-items: stretch;
                border-radius: 20px;
                padding: 18px;
            }

            .page-title {
                font-size: 1.3rem;
                justify-content: center;
            }

            .school-header {
                flex-direction: column;
                text-align: center;
                padding: 25px;
            }

            .school-logo-container {
                margin-bottom: 10px;
            }

            .school-details {
                justify-content: center;
            }

            .student-grid {
                grid-template-columns: 1fr;
            }

            .signature-row {
                flex-direction: column;
            }

            .signature-box {
                width: 100%;
            }

            .btn, .pdf-button {
                width: 100%;
                justify-content: center;
            }
        }

        /* ===== LOADING SPINNER ===== */
        .spinner {
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 3px solid rgba(255,255,255,0.3);
            border-radius: 50%;
            border-top-color: white;
            animation: spin 1s ease-in-out infinite;
        }

        @keyframes spin {
            to { transform: rotate(360deg); }
        }

        /* ===== NOTIFICATION TOAST ===== */
        .toast {
            position: fixed;
            top: 20px;
            right: 20px;
            left: 20px;
            max-width: 400px;
            margin: 0 auto;
            padding: 16px 24px;
            background: white;
            color: var(--text);
            border-radius: 50px;
            font-weight: 600;
            z-index: 10000;
            box-shadow: var(--shadow-lg);
            animation: slideIn 0.3s ease-out;
            display: flex;
            align-items: center;
            gap: 12px;
            border-left: 6px solid var(--success);
        }

        .toast.error {
            border-left-color: var(--danger);
        }

        .toast i {
            font-size: 1.2rem;
        }

        .toast.success i {
            color: var(--success);
        }

        .toast.error i {
            color: var(--danger);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Modern Header with Glass Effect -->
            <div class="header-container">
                <h1 class="page-title">
                    <i class="fas fa-graduation-cap"></i> 
                    Student Report Card
                </h1>
                <div style="display: flex; gap: 12px; flex-wrap: wrap;">
                   <%-- <button type="button" id="exportPdf" class="pdf-button">
                        <i class="fas fa-file-pdf"></i> 
                        Save PDF
                    </button>--%>
                    <asp:Button runat="server" 
                        CssClass="btn" 
                        ID="btnBack" 
                        Text="← Back" 
                        OnClick="btnBack_Click" />
                </div>
            </div>

            <!-- Error Message with Icon -->
            <asp:Label runat="server" ID="lblError" 
                CssClass="error-message" 
                Visible="false">
            </asp:Label>

            <!-- Main Report Card -->
            <div class="report-card" id="reportCard">
                <!-- School Header with Gradient -->
                <div class="school-header">
                    <div class="school-logo-container">
                        <img id="imgLogo" runat="server" 
                            src="" 
                            class="school-logo" 
                            alt="School Logo" 
                            loading="lazy" />
                    </div>
                    <div class="school-info">
                        <h1 class="school-name" runat="server" id="lblSchoolName">
                            ABC SECONDARY SCHOOL
                        </h1>
                        <p class="school-motto" runat="server" id="lblSchoolMoto">
                            <i class="fas fa-quote-left"></i> Knowledge Is Power
                        </p>
                        <div class="school-details">
                            <div runat="server" id="lblAddress">
                                <i class="fas fa-map-marker-alt"></i> Private Bag 123, Lilongwe
                            </div>
                            <div runat="server" id="lblContact">
                                <i class="fas fa-phone"></i> +265 999 123 456
                            </div>
                            <div runat="server" id="lblEmail">
                                <i class="fas fa-envelope"></i> info@abcschool.mw
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Student Details Section -->
                <div class="section">
                    <h2 class="section-title">
                        <i class="fas fa-user-graduate"></i> 
                        Student Information
                    </h2>
                    <div class="student-grid">
                        <div class="detail-item">
                            <div class="detail-label">
                                <i class="fas fa-user"></i> Student Name
                            </div>
                            <div class="detail-value" runat="server" id="lblstudentName">
                                John Banda
                            </div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">
                                <i class="fas fa-school"></i> Class
                            </div>
                            <div class="detail-value" runat="server" id="lblClassName">
                                Form 2
                            </div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">
                                <i class="fas fa-calendar"></i> Term
                            </div>
                            <div class="detail-value" runat="server" id="lblTerm">
                                Term 1
                            </div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">
                                <i class="fas fa-calendar-alt"></i> Academic Year
                            </div>
                            <div class="detail-value" runat="server" id="lblAcademicYear">
                                2025
                            </div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">
                                <i class="fas fa-trophy"></i> Position
                            </div>
                            <div class="detail-value" runat="server" id="lblPosition">
                                -
                            </div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">
                                <i class="fas fa-comment"></i> Remarks
                            </div>
                            <div class="detail-value" runat="server" id="lblRemarks">
                                -
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Subject Results Section -->
                <div class="section">
                    <h2 class="section-title">
                        <i class="fas fa-chart-line"></i> 
                        Examination Results
                    </h2>
                    <div class="table-responsive">
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
                </div>

                <!-- Grading System Section -->
                <div class="section">
                    <h2 class="section-title">
                        <i class="fas fa-star"></i> 
                        Grading System
                    </h2>
                    <div class="table-responsive">
                        <table class="results-table">
                            <thead>
                                <tr>
                                    <th>Grade</th>
                                    <th>Minimum Mark</th>
                                    <th>Maximum Mark</th>
                                    <th>Interpretation</th>
                                </tr>
                            </thead>
                            <tbody>
                                <% LoadGradingSystem(); %>
                            </tbody>
                        </table>
                    </div>
                </div>

                <!-- Remarks & Signatures Section -->
                <div class="section">
                    <h2 class="section-title">
                        <i class="fas fa-comment-dots"></i> 
                        Teacher's Remarks
                    </h2>

                    <div class="remarks-container">
                        <div class="remark-box">
                            <div class="remark-title">
                                <i class="fas fa-chalkboard-teacher"></i> 
                                Class Teacher's Remark
                            </div>
                            <div class="remark-content" runat="server" id="lblteachersRemarks">
                                NA
                            </div>
                        </div>
                        <div class="remark-box">
                            <div class="remark-title">
                                <i class="fas fa-user-tie"></i> 
                                Head Teacher's Remark
                            </div>
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
                            <div class="signature-label">
                                <i class="fas fa-pen"></i> Class Teacher
                            </div>
                            <div class="signature-date" runat="server" id="lblDate">
                                <i class="fas fa-calendar-day"></i> 
                            </div>
                        </div>
                        <div class="signature-box">
                            <p runat="server" id="lblheadteacherSignature" class="signature-text"></p>
                            <div class="signature-line"></div>
                            <div class="signature-label">
                                <i class="fas fa-pen-fancy"></i> Head Teacher
                            </div>
                            <div class="signature-date" runat="server" id="lblDate2">
                                <i class="fas fa-calendar-day"></i>
                            </div>
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
                    <p>
                        <i class="fas fa-exclamation-circle"></i> 
                        This document is system generated and does not require a stamp.
                    </p>
                    <p runat="server" id="lblfooterSchoolName">
                        <i class="fas fa-copyright"></i> Official Report Card
                    </p>
                </div>
            </div>
        </div>
    </form>

    <!-- Optimized PDF Generation Script -->
    <script>
        (function () {
            'use strict';

            const exportButton = document.getElementById('exportPdf');
            if (!exportButton) return;

            // Toast notification system
            function showToast(message, type = 'success') {
                const toast = document.createElement('div');
                toast.className = `toast ${type}`;
                toast.innerHTML = `
                    <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
                    <span>${message}</span>
                `;

                document.body.appendChild(toast);

                setTimeout(() => {
                    toast.style.animation = 'slideOut 0.3s forwards';
                    setTimeout(() => toast.remove(), 300);
                }, 3000);
            }

            // PDF Generation with optimized settings
            // ULTRA HIGH QUALITY PDF GENERATION
            async function generatePDF() {
                const element = document.getElementById('reportCard');

                try {
                    showToast('Generating high quality PDF...', 'info');

                    // FORCE DARK TEXT before capture
                    const originalColors = [];
                    const textElements = element.querySelectorAll('*');
                    textElements.forEach((el, index) => {
                        originalColors[index] = el.style.color;
                        if (window.getComputedStyle(el).color.includes('rgb')) {
                            el.style.color = '#000000'; // Force black
                        }
                    });

                    // HIGH RESOLUTION capture
                    const canvas = await html2canvas(element, {
                        scale: 4.0, // EXTRA HIGH resolution
                        useCORS: true,
                        logging: false,
                        backgroundColor: '#ffffff',
                        allowTaint: true,
                        imageTimeout: 0,
                        windowWidth: element.scrollWidth,
                        windowHeight: element.scrollHeight,
                        onclone: function (clonedDoc) {
                            const clonedElement = clonedDoc.getElementById('reportCard');
                            if (clonedElement) {
                                clonedElement.style.color = '#000000';
                                clonedElement.style.fontWeight = '500';
                            }
                        }
                    });

                    // RESTORE original colors
                    textElements.forEach((el, index) => {
                        el.style.color = originalColors[index] || '';
                    });

                    // USE PNG with NO compression
                    const imgData = canvas.toDataURL('image/png');

                    const { jsPDF } = window.jspdf;
                    const pdf = new jsPDF({
                        orientation: 'p',
                        unit: 'mm',
                        format: 'a4',
                        compress: false,
                        precision: 32
                    });

                    const pdfWidth = pdf.internal.pageSize.getWidth();
                    const pdfHeight = pdf.internal.pageSize.getHeight();

                    const imgWidth = canvas.width;
                    const imgHeight = canvas.height;
                    const ratio = Math.min(pdfWidth / imgWidth, pdfHeight / imgHeight);
                    const imgX = (pdfWidth - imgWidth * ratio) / 2;

                    // ADD IMAGE with BEST QUALITY
                    pdf.addImage(imgData, 'PNG', imgX, 10, imgWidth * ratio, imgHeight * ratio, undefined, 'NONE');

                    // ENHANCE CONTRAST
                    pdf.setDrawColor(0, 0, 0);
                    pdf.setTextColor(0, 0, 0);

                    pdf.save(`ReportCard_${new Date().getTime()}.pdf`);

                    showToast('PDF downloaded successfully!', 'success');
                } catch (error) {
                    console.error('PDF Error:', error);
                    showToast('Failed to generate PDF. Please try again.', 'error');
                }
            }

            // Load script dynamically
            function loadScript(src) {
                return new Promise((resolve, reject) => {
                    const script = document.createElement('script');
                    script.src = src;
                    script.onload = resolve;
                    script.onerror = reject;
                    document.head.appendChild(script);
                });
            }

            // Event listener with loading state
            exportButton.addEventListener('click', async function (e) {
                e.preventDefault();

                const originalHTML = this.innerHTML;
                this.innerHTML = '<span class="spinner"></span> Generating PDF...';
                this.disabled = true;

                await generatePDF();

                this.innerHTML = originalHTML;
                this.disabled = false;
            });

            // Auto-apply grade badges
            document.addEventListener('DOMContentLoaded', function () {
                const gradeCells = document.querySelectorAll('.results-table td:nth-child(3)');
                gradeCells.forEach(cell => {
                    const grade = cell.textContent.trim().toUpperCase();
                    if (grade) {
                        let gradeClass = 'grade-badge ';
                        if (grade.startsWith('A')) gradeClass += 'grade-A';
                        else if (grade.startsWith('B')) gradeClass += 'grade-B';
                        else if (grade.startsWith('C')) gradeClass += 'grade-C';
                        else if (grade.startsWith('D')) gradeClass += 'grade-D';
                        else if (grade === 'F') gradeClass += 'grade-F';

                        cell.innerHTML = `<span class="${gradeClass}">${grade}</span>`;
                    }
                });
            });
        })();
    </script>
</body>
</html>