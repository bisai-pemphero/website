<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Dashboard</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" />
    <link rel="stylesheet" href="../css/customCs.css" />

    <style>
        /* ===== Charts Section Styling ===== */
        .charts-section {
            margin-top: 40px;
            padding: 0 10px;
        }

        .charts-row {
            display: flex;
            flex-wrap: wrap;
            gap: 24px;
            justify-content: space-between;
        }

        .chart-card {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 3px 12px rgba(0, 0, 0, 0.08);
            padding: 20px 25px;
            flex: 1 1 48%;
            transition: all 0.3s ease;
            min-height: 380px;
            display: flex;
            flex-direction: column;
        }

            .chart-card.donut {
                align-items: center;
                justify-content: center;
            }

            .chart-card:hover {
                transform: translateY(-3px);
                box-shadow: 0 4px 14px rgba(0, 0, 0, 0.12);
            }

            .chart-card h4 {
                font-size: 16px;
                font-weight: 600;
                margin-bottom: 15px;
                color: #333;
                display: flex;
                align-items: center;
                gap: 8px;
            }

        .chart-wrapper {
            flex: 1;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .chart-card.donut canvas {
            width: 240px !important;
            height: 240px !important;
        }

        .chart-card canvas {
            width: 100% !important;
            height: 280px !important;
        }

        /* Responsive layout for smaller screens */
        @media (max-width: 500px) {
            .charts-row {
                flex-direction: column;
            }

            .chart-card {
                flex: 1 1 100%;
                min-height: 320px;
            }

                .chart-card.donut canvas {
                    width: 220px !important;
                    height: 220px !important;
                }
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
                            <asp:Label runat="server" ID="lblUser"></asp:Label>
                        </div>
                        <div class="status"><i class="fa-solid fa-circle" style="font-size: 8px"></i>Online</div>
                    </div>
                </div>

                <nav class="nav">
                    <h5>Main Navigation</h5>
                    <div class="dropdown"><a class="active" href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i>Dashboard</a> </div>
                    <div class="dropdown">
                        <a href="ViewStudent.aspx"><i class="fa-solid fa-users"></i>Student </a>
                        <div class="dropdown-menu"><a href="ViewStudent.aspx">All Students</a> </div>
                    </div>
                  
                    <div class="dropdown">
                        <a href="Subjects.aspx">
                            <i class="fa-solid fa-book"></i>Subjects</a>
                        <div class="dropdown-menu">
                            <a href="Subjects.aspx">All Subjects</a>
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="Exams.aspx">
                            <i class="fa-solid fa-chalkboard"></i>Exams</a>
                        <div class="dropdown-menu">
                            <a href="Exams.aspx">All Exams</a>
                             <a href="ExamSelect.aspx">Mark Exam</a>
                            <a href="ExamFinal.aspx" runat="server" id="finalMarkSelect" visible="false" >Marked Subjects</a>
                             <a href="ExamRemarkSelect.aspx" runat="server" id="remarkSelect" visible="false" >Exam Remarks</a>
                             <a href="ExamResults.aspx" runat="server" >Exam Results</a>
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
                            <a href="ChangePassword.aspx"><i class="fas fa-lock"></i>Change Password</a>
                            <a href="../CommonPages/Login.aspx"><i class="fas fa-sign-out-alt"></i>Sign out</a>
                        </div>
                    </div>
                </div>
            </header>

            <!-- MAIN CONTENT -->
            <main class="content">
                <section class="cards">
                    <article class="card metric blue">
                        <div class="icon"><i class="fa-solid fa-users"></i></div>
                        <div>
                            <div class="value">
                                <asp:Label runat="server" ID="lblStudentsTotal"></asp:Label>
                            </div>
                            <div class="label">Total Students</div>
                        </div>
                    </article>

                </section>


                <!-- Charts Section -->
                <section class="charts-section">
                    <div class="charts-row">
                        <div class="chart-card small">
                            <h4><i class="fa-solid fa-venus-mars"></i>Students by Gender</h4>
                            <canvas id="genderChart"></canvas>
                        </div>

                        <div class="chart-card small">
                            <h4><i class="fa-solid fa-school"></i>Students by Class</h4>
                            <canvas id="classChart"></canvas>
                        </div>
                    </div>
                </section>


                <asp:Label runat="server" ID="lblError" ForeColor="Red" Font-Bold="true"></asp:Label>
                <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
                 <asp:Label runat="server" ID="lblUserId" Visible="false"></asp:Label>
            </main>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
        <script>
            // GENDER CHART
            var genderChart = new Chart(document.getElementById('genderChart'), {
                type: 'doughnut',
                data: {
                    labels: <%= GenderLabels %>,
                    datasets: [{
                        data: <%= GenderCounts %>,
                        backgroundColor: ['#007bff', '#e83e8c']
                    }]
                }
            });

            // CLASS CHART
            var classChart = new Chart(document.getElementById('classChart'), {
                type: 'bar',
                data: {
                    labels: <%= ClassLabels %>,
                    datasets: [{
                        label: 'Students',
                        data: <%= ClassCounts %>,
                        backgroundColor: '#28a745'
                    }]
                }
            });

            document.addEventListener('DOMContentLoaded', function () {
                const profileBtn = document.getElementById('profileBtn');
                const profileMenu = document.getElementById('profileMenu');

                // Hide menu initially
                profileMenu.style.display = 'none';

                profileBtn.addEventListener('click', function (e) {
                    e.stopPropagation(); // prevent click from bubbling
                    if (profileMenu.style.display === 'none') {
                        profileMenu.style.display = 'block';
                    } else {
                        profileMenu.style.display = 'none';
                    }
                });

                // Hide dropdown when clicking outside
                document.addEventListener('click', function () {
                    profileMenu.style.display = 'none';
                });
            });
        </script>
    </form>
</body>
</html>
