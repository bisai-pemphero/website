<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Dashboard</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Fonts & Icons -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" />

    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

    <style>
        /* Global */
        body {
            font-family: 'Inter', sans-serif;
            margin: 0;
            background: #f4f6f9;
            color: #333;
        }

        .app {
            display: grid;
            grid-template-columns: 250px 1fr;
            min-height: 100vh;
        }

        /* Sidebar */
        .sidebar {
            background: #1e293b;
            color: #fff;
            display: flex;
            flex-direction: column;
            padding: 20px 0;
            box-shadow: 2px 0 6px rgba(0,0,0,0.1);
        }

            .sidebar .brand {
                display: flex;
                align-items: center;
                justify-content: center;
                margin-bottom: 30px;
                font-size: 20px;
                font-weight: 700;
                color: #fff;
            }

                .sidebar .brand i {
                    margin-right: 8px;
                    color: #38bdf8;
                }

            .sidebar .userpanel {
                padding: 15px;
                background: rgba(255,255,255,0.05);
                border-radius: 10px;
                margin: 0 15px 30px;
                display: flex;
                align-items: center;
                gap: 12px;
            }

            .sidebar .avatar {
                width: 40px;
                height: 40px;
                background: #38bdf8;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                color: #fff;
                font-weight: 600;
            }

            .sidebar .nav {
                flex: 1;
                padding: 0 10px;
            }

                .sidebar .nav h5 {
                    margin: 10px 15px;
                    font-size: 12px;
                    text-transform: uppercase;
                    color: #94a3b8;
                }

                .sidebar .nav a {
                    display: block;
                    padding: 12px 15px;
                    border-radius: 8px;
                    color: #cbd5e1;
                    text-decoration: none;
                    transition: all 0.2s;
                }

                    .sidebar .nav a.active,
                    .sidebar .nav a:hover {
                        background: #334155;
                        color: #fff;
                    }

        /* Topbar */
        .topbar {
            background: rgba(255,255,255,0.9);
            backdrop-filter: blur(10px);
            border-bottom: 1px solid #e2e8f0;
            padding: 12px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

            .topbar .left {
                display: flex;
                align-items: center;
                gap: 15px;
                font-weight: 700;
                font-size: 18px;
            }

        .icon-btn {
            background: none;
            border: none;
            cursor: pointer;
            font-size: 18px;
        }

        .topbar .profile img {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            border: 2px solid #e2e8f0;
        }

        /* Content */
        .content {
            padding: 25px;
            background: #f4f6f9;
        }

        .details-grid {
            display: grid;
            grid-template-rows: repeat(auto-fit, minmax(420px, 1fr));
            gap: 25px;
        }

        /* Cards */
        .card {
            background: #fff;
            border-radius: 14px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.06);
            padding: 20px;
            transition: transform 0.2s;
        }

            .card:hover {
                transform: translateY(-3px);
            }

        .card-header {
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 15px;
            color: #1e293b;
        }

            .card-header i {
                color: #38bdf8;
            }

        .totals {
            margin-bottom: 20px;
        }

            .totals span {
                display: inline-block;
                margin-right: 18px;
                font-weight: 600;
                color: #475569;
            }

        /* Chart Containers */
        .chart-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin: 20px 0;
        }

        .chart-container {
            background: #f8fafc;
            border-radius: 10px;
            padding: 15px;
            text-align: center;
        }

        .chart-title {
            font-size: 14px;
            font-weight: 600;
            margin-bottom: 10px;
            color: #475569;
        }

        .chart-wrapper {
            position: relative;
            height: 200px;
            margin: 0 auto;
        }

        /* Summary Stats */
        .summary-stats {
            display: grid;
            grid-template-rows: repeat(auto-fit, minmax(120px, 1fr));
            gap: 15px;
            margin: 20px 0;
        }

        .stat-item {
            text-align: center;
            padding: 15px;
            background: #f8fafc;
            border-radius: 10px;
        }

        .stat-value {
            font-size: 18px;
            font-weight: 700;
            color: #1e293b;
            margin-bottom: 5px;
        }

        .stat-label {
            font-size: 12px;
            color: #64748b;
            font-weight: 500;
        }

        /*kpi summary */
        /* Summary Cards */
        .summary-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 20px;
            margin-bottom: 25px;
        }

        .summary-card {
            background: linear-gradient(135deg, #fff 0%, #f8fafc 100%);
            border-radius: 16px;
            padding: 20px;
            display: flex;
            align-items: center;
            gap: 15px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
            border: 1px solid #e2e8f0;
            position: relative;
            overflow: hidden;
            transition: all 0.3s ease;
        }

            .summary-card:hover {
                transform: translateY(-5px);
                box-shadow: 0 8px 25px rgba(0, 0, 0, 0.12);
            }

            .summary-card::before {
                content: '';
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                height: 4px;
                background: linear-gradient(90deg, #38bdf8, #0ea5e9);
            }

            .summary-card.collected::before {
                background: linear-gradient(90deg, #10b981, #059669);
            }

            .summary-card.balance::before {
                background: linear-gradient(90deg, #f59e0b, #d97706);
            }

        .card-icon {
            width: 60px;
            height: 60px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            background: linear-gradient(135deg, #38bdf8, #0ea5e9);
            color: white;
            box-shadow: 0 4px 12px rgba(56, 189, 248, 0.3);
        }

        .summary-card.collected .card-icon {
            background: linear-gradient(135deg, #10b981, #059669);
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
        }

        .summary-card.balance .card-icon {
            background: linear-gradient(135deg, #f59e0b, #d97706);
            box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
        }

        .card-content {
            flex: 1;
        }

        .card-value {
            font-size: 24px;
            font-weight: 700;
            color: #1e293b;
            margin-bottom: 4px;
            line-height: 1.2;
        }

        .summary-card.expected .card-value {
            color: #0ea5e9;
        }

        .summary-card.collected .card-value {
            color: #059669;
        }

        .summary-card.balance .card-value {
            color: #d97706;
        }

        .card-label {
            font-size: 14px;
            font-weight: 600;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .card-badge {
            position: absolute;
            top: 15px;
            right: 15px;
            width: 32px;
            height: 32px;
            border-radius: 8px;
            background: #f1f5f9;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            color: #64748b;
        }

            .card-badge.success {
                background: #d1fae5;
                color: #059669;
            }

            .card-badge.warning {
                background: #fef3c7;
                color: #d97706;
            }

        /* Responsive Design */
        @media (max-width: 768px) {
            .summary-cards {
                grid-template-columns: 1fr;
                gap: 15px;
            }

            .summary-card {
                padding: 15px;
            }

            .card-icon {
                width: 50px;
                height: 50px;
                font-size: 20px;
            }

            .card-value {
                font-size: 20px;
            }
        }

        /* Responsive DropDown */
        .dropdown-menu {
            display: none;
            flex-direction: column;
            margin-left: 10px;
        }

            .dropdown-menu a {
                padding: 10px 15px;
                font-size: 14px;
                color: #cbd5e1;
            }

                .dropdown-menu a:hover {
                    background: #334155;
                    color: #fff;
                }

        .dropdown.active > .dropdown-menu {
            display: flex;
        }

        /* Profile Dropdown */
        .right .dropdown {
            position: relative;
        }

        #profileMenu {
            display: none;
            position: absolute;
            top: 50px;
            right: 0;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            min-width: 160px;
            z-index: 1000;
            flex-direction: column;
        }

            #profileMenu a {
                padding: 10px 15px;
                text-decoration: none;
                color: #1e293b;
                font-size: 14px;
                display: flex;
                align-items: center;
                gap: 8px;
                transition: background 0.2s;
            }

                #profileMenu a:hover {
                    background: #f1f5f9;
                    color: #0ea5e9;
                }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="app">
            <!-- SIDEBAR -->
            <aside class="sidebar">
                <div class="brand">
                    <i class="fa-solid fa-school"></i>SMS
                </div>

                <div class="userpanel">
                    <div class="avatar">
                        <asp:Label runat="server" ID="lblInitials"></asp:Label>
                    </div>
                    <div>
                        <div>
                            <asp:Label runat="server" ID="lblUser"></asp:Label>
                        </div>
                        <div class="status"><i class="fa-solid fa-circle" style="font-size: 8px; color: #10b981"></i>Online</div>
                    </div>
                </div>

                <nav class="nav">
                    <h5>Main Navigation</h5>

                    <div class="dropdown">
                        <a class="active" href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i> Dashboard</a>
                    </div>

                    <div class="dropdown">
                        <a href="javascript:void(0)" class="dropbtn"><i class="fa-solid fa-users"></i> Student <i class="fas fa-chevron-down" style="float: right"></i></a>
                        <div class="dropdown-menu">
                            <a href="ViewStudent.aspx">All Students</a>
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="javascript:void(0)" class="dropbtn"><i class="fa-solid fa-bank"></i> Fees <i class="fas fa-chevron-down" style="float: right"></i></a>
                        <div class="dropdown-menu">
                             <a href="StudentInvoiceCreate.aspx">Class Invoice</a>
                            <a href="StudentInvoiceSelect.aspx">Student Invoice</a>
                            <a href="StudentInvoice.aspx">Fees Payment</a>
                            <a href="PettyCash.aspx">Petty Cash</a>
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="javascript:void(0)" class="dropbtn"><i class="fa-solid fa-file"></i> Reports <i class="fas fa-chevron-down" style="float: right"></i></a>
                        <div class="dropdown-menu">
                            <a href="GeneralReport.aspx">Fees Report</a>
                            <a href="CompletedFees.aspx">Completed Fees</a>
                            <a href="BalanceFees.aspx">Fees Balances</a>
                        </div>
                    </div>

                     <div class="dropdown">
     <a href="javascript:void(0)" class="dropbtn"><i class="fa-solid fa-folder"></i> Receipts <i class="fas fa-chevron-down" style="float: right"></i></a>
     <div class="dropdown-menu">
         <a href="StudentReceipt.aspx"> Print Receipt</a>
           <a href="BulkReceipt.aspx"> Bulk Receipt</a>
     </div>
 </div>

                    <div class="dropdown">
                        <a href="javascript:void(0)" class="dropbtn"><i class="fa-solid fa-gear"></i> Settings <i class="fas fa-chevron-down" style="float: right"></i></a>
                        <div class="dropdown-menu">
                            <a href="ChangePassword.aspx"> Change Password</a>
                        </div>
                    </div>
                </nav>

            </aside>

            <!-- MAIN -->
            <div>
                <!-- TOPBAR -->
                <header class="topbar">
                    <div class="left">
                        <button class="icon-btn"><i class="fas fa-bars"></i></button>
                        <span> Dashboard</span>
                    </div>
                    <div class="right">

                        <div class="dropdown">
                            <div class="profile icon-btn" id="profileBtn" aria-label="Profile">
                                <img src="../img/user.png" alt="avatar" />

                            </div>
                            <div class="dropdown-menu" id="profileMenu">
                                <a href="ChangePassword.aspx"><i class="fas fa-user"></i> Change Password</a>
                                <a href="../CommonPages/Login.aspx"><i class="fas fa-sign-out-alt"></i>Sign out</a>
                            </div>
                        </div>

                    </div>
                </header>

                <!-- CONTENT -->
                <main class="content">
                    <section class="details-grid">
                        <!-- Today's Collections -->
                        <article class="card">
                            <div class="card-header"><i class="fa-solid fa-calendar-day"></i>Today's Collections</div>
                            <div class="card-body">
                                <div class="summary-cards">
                                    <div class="summary-card expected">
                                        <div class="card-icon">
                                            <i class="fa-solid fa-circle-check"></i>
                                        </div>
                                        <div class="card-content">
                                            <div class="card-value">
                                                <asp:Label runat="server" ID="lblTodayCollected"></asp:Label>
                                            </div>
                                            <div class="card-label">Total Collected</div>
                                        </div>
                                        <div class="card-badge">
                                            <i class="fa-solid fa-file-invoice"></i>
                                        </div>
                                    </div>

                                 <%--   <div class="summary-card collected">
                                        <div class="card-icon">
                                            <i class="fa-solid fa-shirt"></i>
                                        </div>
                                        <div class="card-content">
                                            <div class="card-value">
                                                <asp:Label runat="server" ID="lblUniformFee"></asp:Label>
                                            </div>
                                            <div class="card-label">Uniform Fee</div>
                                        </div>
                                        <div class="card-badge success">
                                            <i class="fa-solid fa-tag"></i>
                                        </div>
                                    </div>

                                    <div class="summary-card balance">
                                        <div class="card-icon">
                                            <i class="fa-solid fa-school"></i>
                                        </div>
                                        <div class="card-content">
                                            <div class="card-value">
                                                <asp:Label runat="server" ID="lblSchoolFee"></asp:Label>
                                            </div>
                                            <div class="card-label">Fees</div>
                                        </div>
                                        <div class="card-badge warning">
                                            <i class="fa-solid fa-money-bill"></i>
                                        </div>
                                    </div>
                                --%>
                                    </div>

                                <!--End here-->
                                <div class="chart-grid">
                                    <div class="chart-container">
                                        <div class="chart-title">Today by Class</div>
                                        <div class="chart-wrapper">
                                            <canvas id="chartTodayClass"></canvas>
                                        </div>
                                    </div>
                                    <div class="chart-container">
                                        <div class="chart-title">Today by Category</div>
                                        <div class="chart-wrapper">
                                            <canvas id="chartTodayCategory"></canvas>
                                        </div>
                                    </div>
                                </div>

                                <div class="chart-container" style="grid-column: 1 / -1; margin-top: 15px;">
                                    <div class="chart-title">Payment Methods Today</div>
                                    <div class="chart-wrapper">
                                        <canvas id="chartTodayPayment"></canvas>
                                    </div>
                                </div>

                                <div class="summary-stats" id="todayClassStats" runat="server">
                                    <!-- Today's class statistics will be populated here -->
                                </div>
                            </div>
                        </article>
                        <!-- Fees Summary -->




                    </section>

                    <asp:Label runat="server" ID="lblError" Font-Bold="true" ForeColor="Red"></asp:Label>
                    <asp:Label runat="server" ID="lblSession" Visible="false" ForeColor="Red"></asp:Label>
                    <asp:Label runat="server" ID="lblSchoolId" Visible="false" ForeColor="Red"></asp:Label>
                </main>
            </div>
        </div>

        <!-- Charts -->
        <script>

            // Today's Class Distribution
            const todayClassCtx = document.getElementById('chartTodayClass').getContext('2d');
            const todayClassData = <%= ChartTodayClassData %>;
            new Chart(todayClassCtx, {
                type: 'pie',
                data: todayClassData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                padding: 20,
                                usePointStyle: true
                            }
                        }
                    }
                }
            });

            // Today's Category Distribution
            const todayCategoryCtx = document.getElementById('chartTodayCategory').getContext('2d');
            const todayCategoryData = <%= ChartTodayCategoryData %>;
            new Chart(todayCategoryCtx, {
                type: 'pie',
                data: todayCategoryData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                padding: 20,
                                usePointStyle: true
                            }
                        }
                    }
                }
            });

            // Today's Payment Methods
            const todayPaymentCtx = document.getElementById('chartTodayPayment').getContext('2d');
            const todayPaymentData = <%= ChartTodayPaymentData %>;
            new Chart(todayPaymentCtx, {
                type: 'doughnut',
                data: todayPaymentData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                padding: 20,
                                usePointStyle: true
                            }
                        }
                    }
                }
            });

            // Handle sidebar dropdown toggles
            document.querySelectorAll('.dropbtn').forEach(btn => {
                btn.addEventListener('click', function () {
                    const parent = btn.parentElement;
                    parent.classList.toggle('active');

                    // Close other open dropdowns
                    document.querySelectorAll('.dropdown').forEach(drop => {
                        if (drop !== parent) drop.classList.remove('active');
                    });
                });
            });

            // Highlight active link
            const currentPage = window.location.pathname.split("/").pop();
            document.querySelectorAll('.nav a').forEach(link => {
                if (link.getAttribute('href') === currentPage) {
                    link.classList.add('active');
                }
            });


            const profileBtn = document.getElementById('profileBtn');
            const profileMenu = document.getElementById('profileMenu');

            profileBtn.addEventListener('click', () => {
                profileMenu.style.display = profileMenu.style.display === 'flex' ? 'none' : 'flex';
            });

            // Close dropdown if clicking outside
            document.addEventListener('click', (e) => {
                if (!profileBtn.contains(e.target) && !profileMenu.contains(e.target)) {
                    profileMenu.style.display = 'none';
                }
            });
        </script>
    </form>
</body>
</html>
