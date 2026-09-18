<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PettyCash.aspx.cs" Inherits="AdminDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Petty Cash</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />

    <!-- Bootstrap -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" rel="stylesheet" />

    <style>
        body {
            background-color: #f8f9fa;
            font-family: 'Poppins', sans-serif;
        }

        .page-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .page-header h2 {
            font-weight: 600;
            color: #0056b3;
        }

        .card {
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,.08);
        }

        .form-icon {
            width: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #f1f1f1;
            border-top-left-radius: .25rem;
            border-bottom-left-radius: .25rem;
            color: #0056b3;
        }

        .status-label {
            font-weight: bold;
            font-size: 1rem;
        }

        .status-label.error {
            color: #dc3545;
        }

        .status-label.success {
            color: #28a745;
        }

        .table thead {
            background-color: #0056b3;
            color: #fff;
        }

        .error-message {
            color: red;
            font-size: 13px;
            margin-top: 5px;
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">

            <!-- Page Header -->
            <div class="page-header">
                <h2><i class="fas fa-coins me-2"></i> Petty Cash</h2>
                <p class="text-muted">Manage and track petty cash transactions</p>
            </div>

            <div class="row">
                <!-- Left Side: Petty Cash Form -->
                <div class="col-lg-8 mb-4">
                    <div class="card p-4">
                        <!-- Status Messages -->
                        <asp:Label runat="server" ID="lblError" CssClass="status-label error"></asp:Label>
                        <asp:Label runat="server" ID="lblSuccess" CssClass="status-label success"></asp:Label>
                        <hr />

                        <!-- Form Fields -->
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <label>Date</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-calendar-alt"></i></div>
                                    <asp:TextBox CssClass="form-control" ID="txtDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>
                                </div>
                                <div class="error-message" id="date-error">Please select a date</div>
                            </div>

                            <div class="form-group col-md-6">
                                <label>Add New</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-plus-circle"></i></div>
                                    <asp:DropDownList CssClass="form-control" ID="drpUpsert" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpUsert_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="error-message" id="upsert-error">Please select option</div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <label>Academic Year</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-school"></i></div>
                                    <asp:DropDownList CssClass="form-control" ID="drpAcademicYear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpAcademicYear_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="error-message" id="year-error">Please select academic year</div>
                            </div>

                            <div class="form-group col-md-6">
                                <label>Term</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-list-ol"></i></div>
                                    <asp:DropDownList CssClass="form-control" ID="drpTerm" runat="server"></asp:DropDownList>
                                </div>
                                <div class="error-message" id="term-error">Please select term</div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>Description</label>
                            <div class="input-group">
                                <div class="form-icon"><i class="fas fa-align-left"></i></div>
                                <asp:TextBox CssClass="form-control" placeholder="Petty Cash for..." ID="txtDescription" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                            <div class="error-message" id="desc-error">Please enter description</div>
                        </div>

                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <label>Amount</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-money-bill"></i></div>
                                    <asp:TextBox CssClass="form-control" placeholder="Amount" ID="txtAmount" runat="server"></asp:TextBox>
                                </div>
                                <div class="error-message" id="amount-error">Please enter amount</div>
                            </div>

                            <div class="form-group col-md-6">
                                <label>Collected by</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-user"></i></div>
                                    <asp:TextBox CssClass="form-control" placeholder="Collected by" ID="txtRequestedby" runat="server"></asp:TextBox>
                                </div>
                                <div class="error-message" id="collected-error">Please enter collector name</div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>Approved by</label>
                            <div class="input-group">
                                <div class="form-icon"><i class="fas fa-user-check"></i></div>
                                <asp:TextBox CssClass="form-control" placeholder="Approved by" ID="txtApprovedby" runat="server"></asp:TextBox>
                            </div>
                            <div class="error-message" id="approved-error">Please enter approver name</div>
                        </div>

                        <!-- Buttons -->
                        <div class="mt-4">
                            <asp:Button ID="btnsave" runat="server" Text="Save Transaction" CssClass="btn btn-primary me-2" OnClientClick="return validateForm();" OnClick="btnsave_Click" />
                            <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success me-2" OnClientClick="return validateForm();" OnClick="btnUpdate_Click" />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
                             <asp:Button ID="Button1" runat="server" Text="Back to Dashboard" CssClass="btn btn-secondary" OnClick="Button1_Click" />
                        </div>
                    </div>
                </div>

                <!-- Right Side: Today's Petty Cash -->
                <div class="col-lg-4">
                    <div class="card p-3">
                        <h6 class="mb-3"><i class="fas fa-list me-2"></i> Petty Cash Today</h6>
                        <div class="table-responsive">
                            <table id="datatablesSimple" class="table table-striped table-hover table-bordered">
                                <thead>
                                    <tr>
                                        <th>Description</th>
                                        <th>Amount</th>
                                        <th>Collected by</th>
                                    </tr>
                                </thead>
                                <tbody id="pettycashPlaceholder" runat="server"></tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Hidden fields -->
            <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblUser" Visible="false"></asp:Label> 
        </div>

        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script>
            function validateForm() {
                let isValid = true;

                // Reset error messages
                document.querySelectorAll('.error-message').forEach(el => el.style.display = 'none');

                if (!document.getElementById('<%= txtDate.ClientID %>').value.trim()) {
                    document.getElementById('date-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= drpUpsert.ClientID %>').value.trim()) {
                    document.getElementById('upsert-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= drpAcademicYear.ClientID %>').value.trim()) {
                    document.getElementById('year-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= drpTerm.ClientID %>').value.trim()) {
                    document.getElementById('term-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= txtDescription.ClientID %>').value.trim()) {
                    document.getElementById('desc-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= txtAmount.ClientID %>').value.trim()) {
                    document.getElementById('amount-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= txtRequestedby.ClientID %>').value.trim()) {
                    document.getElementById('collected-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= txtApprovedby.ClientID %>').value.trim()) {
                    document.getElementById('approved-error').style.display = 'block';
                    isValid = false;
                }

                if (!isValid) {
                    Swal.fire({
                        title: 'Validation Error',
                        text: 'Please fill in all required fields',
                        icon: 'error',
                        confirmButtonColor: '#0056b3'
                    });
                    return false;
                }
            }

            // ✅ Trigger SweetAlerts from server-side labels
            window.onload = function () {
                var successMsg = document.getElementById('<%= lblSuccess.ClientID %>').innerText.trim();
                var errorMsg = document.getElementById('<%= lblError.ClientID %>').innerText.trim();

                if (successMsg) {
                    Swal.fire({
                        title: 'Success',
                        text: successMsg,
                        icon: 'success',
                        confirmButtonColor: '#28a745'
                    });
                }

                if (errorMsg) {
                    Swal.fire({
                        title: 'Error',
                        text: errorMsg,
                        icon: 'error',
                        confirmButtonColor: '#dc3545'
                    });
                }
            }
        </script>

      
    </form>
</body>
</html>
