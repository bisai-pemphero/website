<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png"/>
    <title>Admin - Dashboard</title>
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
                        <a href="ViewSchool.aspx"><i class="fa-solid fa-school"></i>Schools </a>
                        <div class="dropdown-menu">
                            <a href="AddSchool.aspx">Add School</a>
                            <a href="ViewSchool.aspx">View Schools</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="LicenceGen.aspx"><i class="fa-solid fa-table"></i>Licences</a>
                        <div class="dropdown-menu">
                            <a href="LicenceGen.aspx">New Lincence</a>
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="Invoice.aspx"><i class="fa-solid fa-chart-simple"></i>Payments</a>
                        <div class="dropdown-menu">
                            <a href="Invoice.aspx">Invoice</a>
                            <a href="Payments.aspx">Payment</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="UsersView.aspx"><i class="fa-solid fa-user"></i>Users</a>
                        <div class="dropdown-menu">
                            <a href="UsersView.aspx">Admin Users</a> 
                        </div>
                    </div>
                     <div class="dropdown">
     <a href="sms.aspx"><i class="fa-solid fa-message"></i>SMS</a>
     <div class="dropdown-menu">
         <a href="sms.aspx">SMS</a> 
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
                            <a href="#"><i class="fas fa-user"></i> Profile</a>
                            <a href="../CommonPages/Login.aspx"><i class="fas fa-sign-out-alt"></i> Sign out</a>
                        </div>
                    </div>

                </div>
            </header>

            <!-- CONTENT -->
            <main class="content">
              

                <!-- KPI CARDS -->
                <section class="cards">
                    <article class="card metric blue">
                        <div class="icon">
                            <i class="fa-solid fa-school"></i>
                        </div>
                        <div>
                            <div class="value" id="kpiOrders">
                                <asp:Label runat="server" ID="lblTotalSchools"></asp:Label>
                            </div>
                            <div class="label">Schools</div>
                            <a href="ViewSchool.aspx"><span>More info</span> <i class="fa-solid fa-circle-info"></i></a>
                        </div>
                    </article>
                    <article class="card metric green">
                        <div class="icon"><i class="fa-solid fa-users"></i></div>
                        <div>
                            <div class="value" id="kpiBounce">
                                 <asp:Label runat="server" ID="lblTotalStudents"></asp:Label>
                            </div>
                            <div class="label">Students</div>
                            <a href="#"><span>More info</span> <i class="fa-solid fa-circle-info"></i></a>
                        </div>
                    </article>
                    <article class="card metric orange">
                        <div class="icon"><i class="fa-solid fa-user-gear"></i></div>
                        <div>
                            <div class="value" id="kpiUsers">
                                 <asp:Label runat="server" ID="lblTotalUsers"></asp:Label>
                            </div>
                            <div class="label">Users</div>
                            <a href="ViewUsers.aspx"><span>More info</span> <i class="fa-solid fa-circle-info"></i></a>
                        </div>
                    </article>
                    <article class="card metric red">
                        <div class="icon"><i class="fa-solid fa-map-location-dot"></i></div>
                        <div>
                            <div class="value" id="kpiVisitors">
                                 <asp:Label runat="server" ID="lblTotalActive"></asp:Label>
                            </div>
                            <div class="label">Active</div>
                            <a href="LicenceGen.aspx"><span>More info</span> <i class="fa-solid fa-circle-info"></i></a>
                        </div>
                    </article>
                </section>

                <asp:Label runat="server" ID="lblError" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>
                 <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
            </main>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
        <script src="../js/customJs.js"></script>
    </form>
</body>
</html>
