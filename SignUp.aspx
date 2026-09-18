<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SignUp.aspx.cs" Inherits="SignUp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register School</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="keywords" content="mySchool" />
    <meta name="description" content="my School porta;" />
    <meta name='copyright' content='' />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="icon" href="img/favicon.png" />

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css?family=Poppins:200i,300,300i,400,400i,500,500i,600,600i,700,700i,800,800i,900,900i&display=swap" rel="stylesheet" />

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="css/bootstrap.min.css" />
    <!-- Nice Select CSS -->
    <link rel="stylesheet" href="css/nice-select.css" />
    <!-- Font Awesome CSS -->
    <link rel="stylesheet" href="css/font-awesome.min.css" />
    <!-- icofont CSS -->
    <link rel="stylesheet" href="css/icofont.css" />
    <!-- Slicknav -->
    <link rel="stylesheet" href="css/slicknav.min.css" />
    <!-- Owl Carousel CSS -->
    <link rel="stylesheet" href="css/owl-carousel.css" />
    <!-- Datepicker CSS -->
    <link rel="stylesheet" href="css/datepicker.css" />
    <!-- Animate CSS -->
    <link rel="stylesheet" href="css/animate.min.css" />
    <!-- Magnific Popup CSS -->
    <link rel="stylesheet" href="css/magnific-popup.css" />

    <!-- Medipro CSS -->
    <link rel="stylesheet" href="css/normalize.css" />
    <link rel="stylesheet" href="style.css" />
    <link rel="stylesheet" href="css/responsive.css" />


    <!--Modal links-->

    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />

    <!-- jQuery -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>

    <!-- Bootstrap JavaScript -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <script type="text/javascript" language="javascript"></script>



</head>
<body>
    <form id="form1" runat="server">
        <div>
            <header class="header">
                <!-- Topbar -->
                <div class="topbar">
                    <div class="container">
                        <div class="row">
                            <div class="col-lg-6 col-md-5 col-12">
                                <!-- Contact -->
                                <!--<ul class="top-link">
				<li><a href="#">About</a></li>
				<li><a href="#">Schools</a></li>
				<li><a href="#">Contact Us</a></li>
				<li><a href="#">FAQ</a></li>
			</ul>-->
                                <!-- End Contact -->
                            </div>
                            <div class="col-lg-6 col-md-7 col-12">
                                <!-- Top Contact -->
                                <ul class="top-contact">
                                    <li>
                                        <i class="fa fa-phone"></i>
                                        <a href="tel:+265997614876">+265 997 614 876</a> / 
                                        <a href="tel:+265999513300">+265 999 513 300</a>
                                    </li>
                                    <!--  <li><i class="fa fa-envelope"></i><a href="mailto:support@yourmail.com">mySchool@gmail.com</a></li> -->
                                </ul>
                                <!-- End Top Contact -->
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Topbar -->
                <!-- Header Inner -->
                <div class="header-inner">
                    <div class="container">
                        <div class="inner">
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-12">
                                    <!-- Start Logo -->
                                    <div class="logo">
                                        <a href="index.html">
                                            <img src="img/logo.png" alt="#" /></a>
                                    </div>
                                    <!-- End Logo -->
                                    <!-- Mobile Nav -->
                                    <div class="mobile-nav"></div>
                                    <!-- End Mobile Nav -->
                                </div>
                                <div class="col-lg-7 col-md-9 col-12">
                                    <!-- Main Menu -->
                                    <div class="main-menu">
                                        <nav class="navigation">
                                            <ul class="nav menu">
                                                <li><a href="index.html">Home</a></li>
                                                <li><a href="sms/mySchoolLogin.aspx">Schools </a></li>
                                                <li><a href="sms/Parent_Login.aspx">Parents </a></li>
                                                <!--      <li><a href="#">Schools </a></li>
                                                <li><a href="#">Parents </a></li>
                                                <li><a href="#">Students </a></li>
                                                <li>
                                                    <a href="#">My Account <i class="icofont-rounded-down"></i></a>
                                                    <ul class="dropdown">
                                                        <li><a href="sms/Login.aspx">Sign In</a></li>
                                                        <li><a href="SignUp.aspx">Sign Up</a></li>
                                                    </ul>
                                                </li> -->

                                            </ul>
                                        </nav>
                                    </div>
                                    <!--/ End Main Menu -->
                                </div>
                                <div class="col-lg-2 col-12">
                                    <div class="get-quote">
                                        <a href="SignUp.aspx" class="btn">Try for Free</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!--/ End Header Inner -->
            </header>
            <!-- End Header Area -->




            <section class="contact-us section">
                <div class="container">
                    <div class="inner">

                        <div class="row">
                            <div class="col-lg-2">
                            </div>
                            <div class="col-lg-8">
                                <div class="contact-us-form">
                                    <h2>Register Your Details</h2>
                                    <p>Complete this form to start Using mySchool</p>

                                    <asp:Label runat="server" ID="lblError" Font-Bold="true" Font-Size="Large" ForeColor="#ff0000"></asp:Label>
                                    <asp:Label runat="server" ID="lblSuccess" Font-Bold="true" Font-Size="Large" ForeColor="#3366ff"></asp:Label>
                                    <!-- Form -->
                                    <br />
                                    <br />


                                    <div class="row">

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblHeader1" Font-Bold="true" Font-Size="Large" ForeColor="Black">  School Details</asp:Label>

                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtSchoolName" Placeholder=" School Name" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtSchoolEmail" Placeholder=" School Email" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtAddress" Placeholder=" Your School Address" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                 <asp:TextBox runat="server" ID="txtSchoolPhone" Placeholder=" School Phone Number" CssClass="form-control"></asp:TextBox>
            
                                            </div>
                                        </div>
                                         <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtSlogan" Placeholder=" Your School Moto / Slogan" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                 
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" ID="lblHeader2" Font-Bold="true" Font-Size="Large" ForeColor="Black">  Your Details</asp:Label>

                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtAdminName" Placeholder=" Your Name" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtphone" Placeholder=" Your Phone Number" TextMode="Number" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtEmail" Placeholder=" Your Email Address" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox runat="server" ID="txtPassword" Placeholder=" Create New Password" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12">
                                            <div class="form-group login-btn">
                                                <asp:Button runat="server" ID="btnSave" Text="Submit School Details" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                                                <asp:Label runat="server" ID="lblPhoneNumber" Visible="false"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:Label runat="server" ID="lblLoginEmail" Visible="false"><a href="https://apps.innosoftmw.com/myschool/sms/mySchoolLogin.aspx" ></a> </asp:Label>

                                    <!--/ End Form -->
                                </div>
                            </div>
                        </div>


                    </div>

                </div>

            </section>

            <div class="breadcrumbs overlay">
                <div class="container">
                    <div class="bread-inner">
                        <div class="row">
                            <div class="col-12">
                                <h2>REGISTER YOUR SCHOOL</h2>
                                <ul class="bread-list">
                                    <li><a href="index.html">Home</a></li>
                                    <li><i class="icofont-simple-right"></i></li>
                                    <li class="active">Sign Up</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--Modal for saved pop up-->
            <!-- Modal -->
            <!-- Modal -->
            <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" style="font-size: 24px; color: steelblue" id="exampleModalLabel">Success</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="signle-icon" style="font-size: 72px; color: steelblue; text-align: center">
                                <i class="icofont icofont-check-circled"></i>
                            </div>
                            <p style="text-align: center">
                                Your School has been Registered Successfully!
                     <br />
                                We will call you for System Installation at your school.
                     <br />
                                <strong><span>Thank you.</span>  </strong>
                            </p>
                        </div>
                        <div class="modal-footer">
                            <a href="index.html" type="button" class="btn btn-primary" style="color: white">Done </a>
                            <button type="button" class="btn btn-success" data-dismiss="modal">Register Another School</button>
                            <!-- Additional buttons can go here -->
                        </div>
                    </div>
                </div>
            </div>



        </div>

    </form>
</body>
</html>
