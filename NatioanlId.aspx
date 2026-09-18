<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NatioanlId.aspx.cs" Inherits="Fees" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fees</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="keywords" content="sms" />
    <meta name="description" content="my School porta;" />
    <meta name='copyright' content='' />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="icon" href="img/favicon.png" />

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css?family=Poppins:200i,300,300i,400,400i,500,500i,600,600i,700,700i,800,800i,900,900i&display=swap" rel="stylesheet" />

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="~/css/bootstrap.min.css" />
    <!-- Nice Select CSS -->
    <link rel="stylesheet" href="~/css/nice-select.css" />
    <!-- Font Awesome CSS -->
    <link rel="stylesheet" href="~/css/font-awesome.min.css" />
    <!-- icofont CSS -->
    <link rel="stylesheet" href="~/css/icofont.css" />
    <!-- Slicknav -->
    <link rel="stylesheet" href="~/css/slicknav.min.css" />
    <!-- Owl Carousel CSS -->
    <link rel="stylesheet" href="~/css/owl-carousel.css" />
    <!-- Datepicker CSS -->
    <link rel="stylesheet" href="~/css/datepicker.css" />
    <!-- Animate CSS -->
    <link rel="stylesheet" href="~/css/animate.min.css" />
    <!-- Magnific Popup CSS -->
    <link rel="stylesheet" href="~/css/magnific-popup.css" />

    <!-- Medipro CSS -->
    <link rel="stylesheet" href="~/css/normalize.css" />
    <link rel="stylesheet" href="~/style.css" />
    <link rel="stylesheet" href="~/css/responsive.css" />

    <!--Fo data tables-->
    <link href="https://cdn.jsdelivr.net/npm/simple-datatables@7.1.2/dist/style.min.css" rel="stylesheet" />
    <link href="ccss/styles.css" rel="stylesheet" />
    <script src="https://use.fontawesome.com/releases/v6.3.0/js/all.js" crossorigin="anonymous"></script>
    <link href="https://cdn.jsdelivr.net/npm/simple-datatables@7.1.2/dist/style.min.css" rel="stylesheet" />


    <!--Modal links-->

    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />

    <!-- jQuery -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>

    <!-- Bootstrap JavaScript -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <script type="text/javascript" language="javascript"></script>

    <style>
        /* Style for a professional-looking table */
        .table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
            font-family: Arial, sans-serif;
            border-radius: 5px;
        }

            .table thead {
                background-color: #0056b3; /* Bootstrap primary color */
                color: white;
            }

            .table th, .table td {
                padding: 12px 15px;
                border: 1px solid #dee2e6; /* Light gray borders for a clean look */
               
                font-weight: normal;
            }

        .table-striped tbody tr:nth-of-type(odd) {
            background-color: #f9f9f9;
        }

        .table-hover tbody tr:hover {
            background-color: #f1f1f1; /* Slightly darker on hover */
        }

        .table-bordered {
            border: 1px solid #dee2e6;
        }

        .table-primary th {
            background-color: #0056b3;
            color: #fff;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 30px">

            <section class="contact-us section">
                <div class="container">
                    <div class="inner">

                        <div class="row">

                            <div class="col-lg-8">
                                <div class="contact-us-form">

                                   
                                    <asp:Label runat="server" ID="lblError" Font-Bold="true" Font-Size="Large" ForeColor="#ff0000">
                                       
                                    </asp:Label>
                                    <asp:Label runat="server" ID="lblSuccess" Font-Bold="true" Font-Size="Large" ForeColor="#3366ff"></asp:Label>
                                    <!-- Form -->
                                    <br />
                                    <br />



                                    <hr />

                                    <div class="form-group row">

                                        <div class="col-sm-4">
                                          Scan Here
                                            <strong>
                                                <asp:TextBox CssClass="form-control" ID="txtPhone" runat="server"></asp:TextBox>
                                            </strong>
                                        </div>
                                      
                                      
                                    <br />

                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-group login-btn">
                                                <asp:Button runat="server" ID="btnSend" Text="Send" CssClass="btn btn-primary"  OnClick="btnSend_Click"/>
                                            </div>
                                        </div>
                                    </div>

                                    <!--/ End Form -->
                                </div>
                            </div>
                          
                        </div>


                    </div>

                </div>


                <!--Other useful links-->
              
             
        </div>

    </form>
</body>
</html>
