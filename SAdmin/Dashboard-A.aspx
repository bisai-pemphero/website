<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard-A.aspx.cs" Inherits="Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Dashboard</title>
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

    <link rel="stylesheet" href="../css/customCs.css" />



    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

    <style>
        body { font-family: 'Inter', sans-serif; }
        .cards-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; margin-bottom: 30px; }
        .details-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(500px, 1fr)); gap: 20px; }
        .card { background:#fff; border-radius:12px; box-shadow:0 4px 12px rgba(0,0,0,0.1); padding:20px; }
        .card-header { display:flex; align-items:center; gap:10px; font-weight:700; font-size:18px; margin-bottom:15px; }
        .totals span { display:inline-block; margin-right:20px; font-weight:600; }
        .table-sm { font-size:0.85rem; margin-bottom:15px; }
        .metric { display:flex; flex-direction:column; align-items:center; justify-content:center; text-align:center; padding:20px; color:#fff; border-radius:12px; }
        .metric .icon { font-size:32px; margin-bottom:10px; }
        .metric a { color:#fff; text-decoration:none; font-size:0.85rem; margin-top:5px; display:inline-block; }
        .metric.blue{background:#3498db;} .metric.green{background:#2ecc71;} .metric.orange{background:#e67e22;} .metric.red{background:#e74c3c;}
        .summary-card.blue{border-left:5px solid #3498db;} .summary-card.green{border-left:5px solid #2ecc71;}
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
                    <div class="dropdown">
                        <a href="ViewStudent.aspx"><i class="fa-solid fa-users"></i>Student </a>
                        <div class="dropdown-menu">
                            <a href="ViewStudent.aspx"> All Students</a>
                           
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-bank"></i>Fees</a>
                        <div class="dropdown-menu">
                            <a href="StudentInvoice.aspx">Pay Fees</a>
                            <a href="PettyCash.aspx">Petty Cash</a>
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="Invoice.aspx"><i class="fa-solid fa-file"></i>Reports</a>
                        <div class="dropdown-menu">
                             <a href="GeneralReport.aspx">Fees Report</a>
                            <a href="CompletedFees.aspx">Completed Fees</a>
                             <a href="BalanceFees.aspx">Fees Balances</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="Invoice.aspx"><i class="fa-solid fa-gear"></i>Settings</a>
                        <div class="dropdown-menu">
                            <a href="#">Change Password</a>
                            
                        </div>
                    </div>
                   
                </nav>
            </aside>

            <!-- TOPBAR -->
            <header class="topbar">
                <div class="left">
                    <button class="icon-btn hamburger" id="btnHamburger" aria-label="Toggle sidebar">
                        <i class="fas fa-bars"></i>
                        <!-- Fallback class -->
                    </button>
                    <div style="font-weight: 700">Dashboard</div>
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

            <!-- DETAILS SECTIONS -->
            <section class="details-grid">

                <!-- OVERALL FEES SUMMARY -->
                <article class="card summary-card blue">
                    <div class="card-header"><i class="fa-solid fa-chart-simple"></i><h3>Overall Fees Summary</h3></div>
                    <div class="card-body">
                        <div class="totals">
                            <span>Total Expected: <asp:Label runat="server" ID="lblOverallExpected"></asp:Label></span>
                            <span>Total Collected: <asp:Label runat="server" ID="lblOverallCollected"></asp:Label></span>
                            <span>Total Balance: <asp:Label runat="server" ID="lblOverallBalance"></asp:Label></span>
                        </div>
                        <h5>By Class</h5>
                        <asp:GridView runat="server" ID="gvOverallByClass" CssClass="table table-sm table-striped" AutoGenerateColumns="true"></asp:GridView>
                        <h5>By Fee Category</h5>
                        <asp:GridView runat="server" ID="gvOverallByCategory" CssClass="table table-sm table-striped" AutoGenerateColumns="true"></asp:GridView>
                        <canvas id="chartOverallCategory" style="height:200px;"></canvas>
                    </div>
                </article>

                <!-- TODAY COLLECTIONS -->
                <article class="card summary-card green">
                    <div class="card-header"><i class="fa-solid fa-calendar-day"></i><h3>Today's Collections</h3></div>
                    <div class="card-body">
                        <div class="totals">
                            <span>Total Collected Today: <asp:Label runat="server" ID="lblTodayCollected"></asp:Label></span>
                        </div>
                        <h5>By Class</h5>
                        <asp:GridView runat="server" ID="gvTodayByClass" CssClass="table table-sm table-striped" AutoGenerateColumns="true"></asp:GridView>
                        <h5>By Fee Category</h5>
                        <asp:GridView runat="server" ID="gvTodayByCategory" CssClass="table table-sm table-striped" AutoGenerateColumns="true"></asp:GridView>
                        <h5>By Payment Mode</h5>
                        <asp:GridView runat="server" ID="gvTodayByPayment" CssClass="table table-sm table-striped" AutoGenerateColumns="true"></asp:GridView>
                        <canvas id="chartTodayCategory" style="height:200px;"></canvas>
                    </div>
                </article>

            </section>

                <asp:Label runat="server" ID="lblError" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>
                <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
            </main>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
        <script src="../js/customJs.js"></script>


         <!-- CHARTS SCRIPT -->
        <script>
            const overallCtx = document.getElementById('chartOverallCategory').getContext('2d');
            const overallData = <%= ChartOverallCategoryData %>;
            new Chart(overallCtx, { type:'bar', data:overallData, options:{ responsive:true, plugins:{legend:{display:false}}, scales:{y:{beginAtZero:true}} } });

            const todayCtx = document.getElementById('chartTodayCategory').getContext('2d');
            const todayData = <%= ChartTodayCategoryData %>;
            new Chart(todayCtx, { type:'bar', data:todayData, options:{ responsive:true, plugins:{legend:{display:false}}, scales:{y:{beginAtZero:true}} } });
        </script>
    </form>
</body>
</html>
