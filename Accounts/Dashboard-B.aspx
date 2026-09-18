<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard-B.aspx.cs" Inherits="Dashboard" %>

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
            width: 40px; height: 40px;
            background: #38bdf8;
            border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            color: #fff; font-weight: 600;
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
            width: 38px; height: 38px;
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
            grid-template-columns: repeat(auto-fit, minmax(420px, 1fr));
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
        .totals span {
            display: inline-block;
            margin-right: 18px;
            font-weight: 600;
            color: #475569;
        }

        /* Tables */
        .table {
            width: 100%;
            border-collapse: collapse;
            margin: 15px 0;
            font-size: 14px;
        }
        .table th, .table td {
            padding: 10px 12px;
            border-bottom: 1px solid #e2e8f0;
        }
        .table-striped tr:nth-child(even) {
            background: #f9fafb;
        }
        .table th {
            text-align: left;
            background: #f1f5f9;
            font-weight: 600;
        }

        /* Chart */
        canvas {
            margin-top: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="app">
            <!-- SIDEBAR -->
            <aside class="sidebar">
                <div class="brand">
                    <i class="fa-solid fa-school"></i> SMS
                </div>

                <div class="userpanel">
                    <div class="avatar"><asp:Label runat="server" ID="lblInitials"></asp:Label></div>
                    <div>
                        <div><asp:Label runat="server" ID="lblUser"></asp:Label></div>
                        <div class="status"><i class="fa-solid fa-circle" style="font-size:8px;color:#10b981"></i> Online</div>
                    </div>
                </div>

                <nav class="nav">
                    <h5>Main Navigation</h5>
                    <a class="active" href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i> Dashboard</a>
                    <a href="ViewStudent.aspx"><i class="fa-solid fa-users"></i> Students</a>
                    <a href="#"><i class="fa-solid fa-bank"></i> Fees</a>
                    <a href="#"><i class="fa-solid fa-file"></i> Reports</a>
                    <a href="#"><i class="fa-solid fa-gear"></i> Settings</a>
                </nav>
            </aside>

            <!-- MAIN -->
            <div>
                <!-- TOPBAR -->
                <header class="topbar">
                    <div class="left">
                        <button class="icon-btn"><i class="fas fa-bars"></i></button>
                        <span>Dashboard</span>
                    </div>
                    <div class="right">
                        <div class="profile">
                            <img src="../img/user.png" alt="avatar" />
                        </div>
                    </div>
                </header>

                <!-- CONTENT -->
                <main class="content">
                    <section class="details-grid">

                        <!-- Fees Summary -->
                        <article class="card">
                            <div class="card-header"><i class="fa-solid fa-chart-simple"></i> Overall Fees Summary</div>
                            <div class="card-body">
                                <div class="totals">
                                    <span>Total Expected: <asp:Label runat="server" ID="lblOverallExpected"></asp:Label></span>
                                    <span>Total Collected: <asp:Label runat="server" ID="lblOverallCollected"></asp:Label></span>
                                    <span>Total Balance: <asp:Label runat="server" ID="lblOverallBalance"></asp:Label></span>
                                </div>
                                <h5>By Class</h5>
                                <asp:GridView runat="server" ID="gvOverallByClass" CssClass="table table-striped" AutoGenerateColumns="true"></asp:GridView>
                                <h5>By Fee Category</h5>
                                <asp:GridView runat="server" ID="gvOverallByCategory" CssClass="table table-striped" AutoGenerateColumns="true"></asp:GridView>
                                <canvas id="chartOverallCategory" height="200"></canvas>
                            </div>
                        </article>

                        <!-- Today's Collections -->
                        <article class="card">
                            <div class="card-header"><i class="fa-solid fa-calendar-day"></i> Today's Collections</div>
                            <div class="card-body">
                                <div class="totals">
                                    <span>Total Collected Today: <asp:Label runat="server" ID="lblTodayCollected"></asp:Label></span>
                                </div>
                                <h5>By Class</h5>
                                <asp:GridView runat="server" ID="gvTodayByClass" CssClass="table table-striped" AutoGenerateColumns="true"></asp:GridView>
                                <h5>By Fee Category</h5>
                                <asp:GridView runat="server" ID="gvTodayByCategory" CssClass="table table-striped" AutoGenerateColumns="true"></asp:GridView>
                                <h5>By Payment Mode</h5>
                                <asp:GridView runat="server" ID="gvTodayByPayment" CssClass="table table-striped" AutoGenerateColumns="true"></asp:GridView>
                                <canvas id="chartTodayCategory" height="200"></canvas>
                            </div>
                        </article>
                    </section>

                    <asp:Label runat="server" ID="lblError" Font-Bold="true" ForeColor="Red"></asp:Label>


                     <asp:Label runat="server" ID="lblSession" Visible="false" ForeColor="Red"></asp:Label>
                     <asp:Label runat="server" ID="lblSchoolId" Visible="false" ForeColor="Red"></asp:Label>
                   
                </main>
            </div>
        </div>

        <!-- Charts -->
        <script>
            const overallCtx = document.getElementById('chartOverallCategory').getContext('2d');
            const overallData = <%= ChartOverallCategoryData %>;
            new Chart(overallCtx, {
                type:'bar',
                data: overallData,
                options:{ responsive:true, plugins:{legend:{display:false}}, scales:{y:{beginAtZero:true}} }
            });

            const todayCtx = document.getElementById('chartTodayCategory').getContext('2d');
            const todayData = <%= ChartTodayCategoryData %>;
            new Chart(todayCtx, {
                type: 'bar',
                data: todayData,
                options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
            });
        </script>
    </form>
</body>
</html>
