<%@ Page Language="C#" AutoEventWireup="true" CodeFile="examPortal.aspx.cs" Inherits="viewExams" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Exam Portal - Student Results</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=yes" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    
    <!-- SweetAlert2 - LOAD EARLY -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- Font Awesome 6 (Free) -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet" />

    <style>
        /* CSS Variables - Light/Dark compatible */
        :root {
            --primary: #0ea5e9;
            --primary-dark: #0284c7;
            --primary-light: #e0f2fe;
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
            --radius: 20px;
            --radius-sm: 12px;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(145deg, var(--bg), #f1f5f9);
            color: var(--text);
            min-height: 100vh;
            line-height: 1.6;
        }

        /* Header - Glassmorphism */
        .header {
            background: rgba(255, 255, 255, 0.9);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            padding: 16px 24px;
            box-shadow: var(--shadow-sm);
            display: flex;
            justify-content: space-between;
            align-items: center;
            position: sticky;
            top: 0;
            z-index: 100;
            border-bottom: 1px solid rgba(255,255,255,0.3);
        }

        .header h1 {
            margin: 0;
            font-size: 1.35rem;
            font-weight: 700;
            background: linear-gradient(135deg, var(--primary-dark), var(--primary));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            letter-spacing: -0.5px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .header h1 i {
            background: linear-gradient(135deg, var(--primary), var(--primary-dark));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
        }

        /* Logout Button */
        .btn-logout {
            background: white;
            color: var(--text-light);
            padding: 10px 20px;
            border-radius: 50px;
            text-decoration: none;
            font-size: 0.9rem;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 8px;
            border: 1px solid var(--border);
            transition: all 0.3s ease;
            box-shadow: var(--shadow-sm);
        }

        .btn-logout:hover {
            background: #fef2f2;
            color: var(--danger);
            border-color: var(--danger);
            transform: translateY(-2px);
            box-shadow: var(--shadow-md);
        }

        .btn-logout i {
            font-size: 0.9rem;
        }

        /* Main Container */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 30px 20px;
        }

        /* Student Welcome Card */
        .welcome-card {
            background: linear-gradient(145deg, var(--card), #ffffff);
            border-radius: var(--radius);
            padding: 28px 32px;
            margin-bottom: 35px;
            box-shadow: var(--shadow-md);
            border: 1px solid rgba(255,255,255,0.5);
            display: flex;
            align-items: center;
            gap: 20px;
            flex-wrap: wrap;
        }

        .welcome-avatar {
            width: 70px;
            height: 70px;
            background: linear-gradient(145deg, var(--primary-light), #ffffff);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            border: 3px solid white;
            box-shadow: var(--shadow-md);
        }

        .welcome-avatar i {
            font-size: 35px;
            color: var(--primary-dark);
        }

        .welcome-text {
            flex: 1;
        }

        .welcome-text h2 {
            margin: 0 0 8px 0;
            font-size: 1.6rem;
            font-weight: 700;
            color: var(--text);
            letter-spacing: -0.5px;
        }

        .welcome-text p {
            margin: 0;
            color: var(--text-light);
            font-size: 0.95rem;
            display: flex;
            align-items: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        .badge {
            background: var(--primary-light);
            color: var(--primary-dark);
            padding: 6px 14px;
            border-radius: 50px;
            font-size: 0.8rem;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        /* Section Title */
        .section-title {
            font-size: 1.5rem;
            font-weight: 700;
            margin-bottom: 25px;
            color: var(--text);
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .section-title i {
            color: var(--primary);
            font-size: 1.6rem;
        }

        /* Exam Grid - Responsive */
        .exam-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 25px;
        }

        /* Exam Card */
        .exam-card {
            background: var(--card);
            border-radius: var(--radius);
            padding: 24px;
            box-shadow: var(--shadow-md);
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            border: 1px solid var(--border);
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            position: relative;
            overflow: hidden;
        }

        .exam-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: linear-gradient(90deg, var(--primary), var(--primary-dark));
            opacity: 0;
            transition: opacity 0.3s ease;
        }

        .exam-card:hover {
            transform: translateY(-8px);
            box-shadow: var(--shadow-lg);
            border-color: var(--primary-light);
        }

        .exam-card:hover::before {
            opacity: 1;
        }

        .exam-icon {
            width: 50px;
            height: 50px;
            background: var(--primary-light);
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 18px;
        }

        .exam-icon i {
            font-size: 26px;
            color: var(--primary-dark);
        }

        .exam-title {
            font-weight: 700;
            font-size: 1.25rem;
            margin-bottom: 12px;
            color: var(--text);
            line-height: 1.4;
        }

        .exam-meta {
            font-size: 0.9rem;
            color: var(--text-light);
            margin-bottom: 20px;
            background: var(--bg);
            padding: 12px;
            border-radius: var(--radius-sm);
            line-height: 1.8;
        }

        .exam-meta i {
            color: var(--primary);
            width: 20px;
            margin-right: 6px;
        }

        /* View Results Button */
        .btn-view {
            background: linear-gradient(145deg, var(--primary), var(--primary-dark));
            color: white;
            border: none;
            padding: 14px 20px;
            border-radius: 50px;
            font-size: 0.95rem;
            font-weight: 600;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            transition: all 0.3s ease;
            border: 1px solid rgba(255,255,255,0.2);
            text-decoration: none;
            width: 100%;
        }

        .btn-view:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 20px -5px rgba(14, 165, 233, 0.4);
        }

        .btn-view i {
            font-size: 0.9rem;
        }

        /* Empty State */
        .empty-state {
            text-align: center;
            padding: 60px 40px;
            background: white;
            border-radius: var(--radius);
            box-shadow: var(--shadow-md);
            margin-top: 30px;
        }

        .empty-state i {
            font-size: 70px;
            color: var(--text-light);
            opacity: 0.3;
            margin-bottom: 20px;
        }

        .empty-state h3 {
            font-size: 1.5rem;
            font-weight: 700;
            margin-bottom: 12px;
            color: var(--text);
        }

        .empty-state p {
            color: var(--text-light);
            max-width: 400px;
            margin: 0 auto;
        }

        /* Loading Spinner */
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

        /* SweetAlert Custom Styles */
        .swal2-popup {
            border-radius: var(--radius) !important;
            font-family: 'Inter', sans-serif !important;
        }

        .swal2-title {
            font-weight: 700 !important;
        }

        .swal2-html-container {
            margin: 1.5em 0 !important;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .header {
                padding: 14px 18px;
            }

            .header h1 {
                font-size: 1.1rem;
            }

            .container {
                padding: 20px 15px;
            }

            .welcome-card {
                padding: 20px;
            }

            .welcome-text h2 {
                font-size: 1.3rem;
            }

            .exam-grid {
                grid-template-columns: 1fr;
                gap: 18px;
            }

            .exam-card {
                padding: 20px;
            }
        }

        @media (max-width: 480px) {
            .welcome-card {
                flex-direction: column;
                text-align: center;
            }

            .welcome-text p {
                justify-content: center;
            }

            .header {
                flex-direction: column;
                gap: 12px;
            }

            .btn-logout {
                width: 100%;
                justify-content: center;
            }
        }

        /* Print Styles */
        @media print {
            .header, .btn-view, .btn-logout {
                display: none;
            }
        }

        /* Animations */
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .exam-card {
            animation: fadeInUp 0.5s ease-out forwards;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <!-- Header -->
        <div class="header">
            <h1>
                <i class="fas fa-graduation-cap"></i> 
                Exam Results Portal
            </h1>
            <a href="Results.aspx" class="btn-logout">
                <i class="fas fa-sign-out-alt"></i> 
                Logout
            </a>
        </div>

        <!-- Main Content -->
        <div class="container">
            <!-- Student Welcome Card -->
            <div class="welcome-card" id="studentWelcomeCard" runat="server">
                <div class="welcome-avatar">
                    <i class="fas fa-user-graduate"></i>
                </div>
                <div class="welcome-text">
                    <h2 runat="server" id="lblStudentName">Welcome, Student!</h2>
                    <p>
                        <span class="badge">
                            <i class="fas fa-book-open"></i> 
                            <span runat="server" id="lblClassName"></span>
                        </span>
                        <span class="badge">
                            <i class="fas fa-tag"></i> 
                            Level: <span runat="server" id="lblLevelDisplay"></span>
                        </span>
                    </p>
                </div>
            </div>

            <!-- Exams Section -->
            <div class="section-title">
                <i class="fas fa-file-alt"></i> 
                Available Examinations
            </div>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- Exams Repeater -->
                    <div class="exam-grid">
                        <asp:Repeater ID="rptExams" runat="server" OnItemDataBound="rptExams_ItemDataBound">
                            <ItemTemplate>
                                <div class="exam-card">
                                    <div>
                                        <div class="exam-icon">
                                            <i class="fas fa-file-invoice"></i>
                                        </div>
                                        <div class="exam-title">
                                            <%# Eval("Exam_name") %>
                                        </div>
                                        <div class="exam-meta">
                                            <div><i class="fas fa-calendar-alt"></i> Year: <%# Eval("AcademicYear") %></div>
                                            <div><i class="fas fa-clock"></i> Term: <%# Eval("TermName") %></div>
                                        </div>
                                    </div>
                                    
                                    <asp:LinkButton ID="lnkViewResults" runat="server" 
                                        CommandArgument='<%# Eval("ExamsId") %>'
                                        OnCommand="lnkViewResults_Command"
                                        CssClass="btn-view">
                                        <i class="fas fa-eye"></i> View Results
                                        <i class="fas fa-arrow-right"></i>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <!-- Empty State -->
                    <asp:Label ID="lblEmpty" runat="server" Visible="false">
                        <div class="empty-state">
                            <i class="fas fa-inbox"></i>
                            <h3>No Exams Available</h3>
                            <p>There are currently no examination results published for your account. Please check back later.</p>
                        </div>
                    </asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>

            <!-- Hidden Fields for Session Values -->
            <asp:Label ID="lblLevel" runat="server" Visible="false" />
            <asp:Label ID="lblStudentId" runat="server" Visible="false" />
        </div>

        <!-- Loading Overlay (Hidden by Default) -->
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                <div style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 9999;">
                    <div style="background: white; padding: 30px; border-radius: 20px; text-align: center;">
                        <div class="spinner" style="width: 40px; height: 40px; border-width: 4px;"></div>
                        <p style="margin-top: 15px; color: var(--text); font-weight: 500;">Loading exams...</p>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>

    <!-- Additional Scripts -->
    <script>
        // Global SweetAlert2 configuration
        Swal.mixin({
            confirmButtonColor: '#0ea5e9',
            cancelButtonColor: '#64748b',
            buttonsStyling: true,
            customClass: {
                confirmButton: 'btn-view',
                cancelButton: 'btn-logout'
            }
        });

        // Auto-hide notifications after 5 seconds
        function showNotification(type, message) {
            Swal.fire({
                icon: type,
                title: type === 'success' ? 'Success!' : 'Notice',
                text: message,
                timer: 5000,
                timerProgressBar: true,
                showConfirmButton: false,
                position: 'top-end',
                toast: true
            });
        }

        // Prevent double submission
        document.addEventListener('click', function (e) {
            if (e.target.classList.contains('btn-view')) {
                e.target.disabled = true;
                e.target.innerHTML = '<span class="spinner"></span> Processing...';
            }
        });
    </script>
</body>
</html>