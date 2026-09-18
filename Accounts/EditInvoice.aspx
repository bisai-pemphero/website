<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditInvoice.aspx.cs" Inherits="Fees" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Fee Collection</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />

    <!-- Bootstrap -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" rel="stylesheet" />

    <style>
        body {
            background-color: #f4f6f9;
            font-family: 'Poppins', sans-serif;
        }

        .page-header {
            text-align: center;
            margin-bottom: 40px;
        }

        .page-header h2 {
            font-weight: 700;
            color: #004085;
        }

        .page-header p {
            color: #6c757d;
            font-size: 15px;
        }

        .card {
            border-radius: 12px;
            border: none;
            box-shadow: 0 5px 15px rgba(0,0,0,.08);
        }

        .card h5 {
            font-weight: 600;
            color: #343a40;
        }

        .form-icon {
            width: 45px;
            background: #e9ecef;
            display: flex;
            align-items: center;
            justify-content: center;
            border-top-left-radius: .25rem;
            border-bottom-left-radius: .25rem;
            color: #004085;
            font-size: 18px;
        }

        .status-label {
            display: block;
            margin-top: 8px;
            font-weight: 600;
            font-size: 0.95rem;
        }

        .status-label.error { color: #dc3545; }
        .status-label.success { color: #28a745; }

        .btn {
            min-width: 160px;
            border-radius: 30px;
            font-weight: 500;
        }

        .btn i {
            margin-right: 6px;
        }

        .btn-primary {
            background-color: #004085;
            border: none;
        }

        .btn-primary:hover {
            background-color: #002752;
        }

        .btn-secondary {
            background-color: #6c757d;
            border: none;
        }

        .btn-secondary:hover {
            background-color: #565e64;
        }

        .form-label {
            font-weight: 500;
            color: #495057;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">

            <!-- Page Header -->
            <div class="page-header">
                <h2><i class="fas fa-credit-card"></i> Fee Collection</h2>
                <p>Record and manage student payments securely</p>
            </div>

            <div class="row justify-content-center">
                <div class="col-lg-8">
                    <div class="card p-4">
                        <h5 class="mb-3">
                            <asp:Label runat="server" ID="lblCategory" CssClass="font-weight-bold"></asp:Label>
                            for
                            <asp:Label runat="server" ID="lblStudentName" CssClass="font-weight-bold"></asp:Label>
                        </h5>

                        <!-- Status Messages -->
                        <asp:Label runat="server" ID="lblError" CssClass="status-label error"></asp:Label>
                        <asp:Label runat="server" ID="lblSuccess" CssClass="status-label success"></asp:Label>
                        <hr />

                        <!-- Invoice Info -->
                        <div class="form-group">
                            <label class="form-label"><i class="fas fa-money-bill-wave text-success"></i> Invoice Amount</label>
                            <div class="input-group">
                                <div class="form-icon"><i class="fas fa-dollar-sign"></i></div>
                                <div class="form-control bg-light">
                                    <asp:Label runat="server" ForeColor="Red" ID="lblAmount"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <!-- Buttons -->
                        <div class="mt-4 d-flex justify-content-between">
                            <asp:Button runat="server" ID="btnDelete" Text="Delete Transaction" CssClass="btn btn-primary" OnClick="btnDelete_Click" />
                            <asp:Button runat="server" ID="btnBack" Text="Back" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Hidden fields -->
            <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblFeesId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblTransId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblTermId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblTermName" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblSchoolName" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblReceiptNumber" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblStudentID" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblapiKey" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblSenderId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblParentContact" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblUser" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblTotalFees" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblAmountPaid" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblBalance" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblCategoryId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblReversedAmount" Visible="false"></asp:Label>

        </div>

        <!-- SweetAlert -->
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <script>
            // SweetAlert2 wrapper for showing alerts
            function showAlert(type, message) {
                Swal.fire({
                    title: type === 'success' ? 'Success!' : 'Error!',
                    text: message,
                    icon: type,
                    confirmButtonColor: '#004085'
                });
            }
        </script>

    </form>
</body>
</html>
