<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InvoiceAllDetail.aspx.cs" Inherits="InvoiceAllDetail" %>

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
            font-size: 1.5rem;
            font-weight: 600;
            color: var(--text);
            margin-bottom: 25px;
            padding-bottom: 15px;
            border-bottom: 1px solid #e5e7eb;
        }

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
                        <a href="#"><i class="fa-solid fa-school"></i>Schools </a>
                        <div class="dropdown-menu">
                            <a href="AddSchool.aspx">Add School</a>
                            <a href="ViewSchool.aspx">View Schools</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-table"></i>Licences</a>
                        <div class="dropdown-menu">
                            <a href="LicenceGen.aspx">New Lincence</a>
                        </div>
                    </div>

                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-chart-simple"></i>Payments</a>
                        <div class="dropdown-menu">
                            <a href="Invoice.aspx">Invoice</a>
                            <a href="Payments.aspx">Payment</a>
                        </div>
                    </div>
                    <div class="dropdown">
                        <a href="#"><i class="fa-solid fa-user"></i>Users</a>
                        <div class="dropdown-menu">
                            <a href="#">Admin Users</a>
                            <a href="#">School Users</a>
                        </div>
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
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a> <i class="fa-solid fa-angle-right"></i><a href="Invoice.aspx">Ivoice Detail</a>
                </div>

                <div class="form-container">



                    <!-- School Card Details here  -->

                    <div class="school-details-card">
                        <div class="card-header">
                            <div class="school-logo-container">
                                <asp:Image ID="imgSchoolLogo" runat="server" CssClass="school-logo" />
                            </div>
                            <div class="school-title">
                                <h3>
                                    <asp:Literal ID="litSchoolName" runat="server"></asp:Literal></h3>
                                <p class="slogan">
                                    <asp:Literal ID="litSlogan" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="details-grid">


                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-map-marker-alt"></i>Billed To</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litSchoolAddress" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-user-tie"></i>Contact Person</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litAdminName" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-file"></i>Description</span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litdescription" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-dollar"></i>Total </span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litTotal" runat="server"></asp:Literal>
                                    </p>
                                </div>

                                <div class="detail-group">
                                    <span class="detail-label"><i class="fas fa-dollar"></i>Paid </span>
                                    <p class="detail-value">
                                        <asp:Literal ID="litPaid" runat="server"></asp:Literal>
                                    </p>
                                </div>


                                                            <div class="button-container" style="margin-bottom: 20px;">
    <asp:Button ID="btnBack" runat="server" Text=" Back" CssClass="btn-primary" OnClick="btnBack_Click" />
</div>
                            </div>

                          
                        </div>
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
