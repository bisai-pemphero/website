<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AcademicTerm.aspx.cs" Inherits="FeesCategories" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Academic Year</title>
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
            margin-bottom: 30px;
        }

            .page-header h2 {
                font-weight: 600;
                color: #0056b3;
            }

        .card {
            border-radius: 16px;
            box-shadow: 0 4px 18px rgba(0,0,0,0.08);
            transition: all 0.3s ease;
        }

            .card:hover {
                transform: translateY(-4px);
                box-shadow: 0 6px 20px rgba(0,0,0,0.12);
            }

        .form-icon {
            width: 45px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #e9ecef;
            color: #0056b3;
            border-top-left-radius: .25rem;
            border-bottom-left-radius: .25rem;
        }

        .btn {
            padding: 10px 20px;
            border-radius: 8px;
            font-weight: 500;
        }

            .btn i {
                margin-right: 6px;
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
        <div class="container py-5 d-flex justify-content-center align-items-center" style="min-height: 100vh;">
            <div class="col-md-8 col-lg-6">

       <!-- Page Header -->
                <div class="page-header mb-4">
                    <h2><i class="fas fa-calendar-alt me-2"></i>School Term / Semester</h2>
                    <p class="text-muted">Add or Update School Terms or Semester</p>
                </div>

                <!-- Card -->
                <div class="card p-4">
                    <!-- Status Labels -->
                    <asp:Label runat="server" ID="lblError" CssClass="status-label error"></asp:Label>
                    <asp:Label runat="server" ID="lblSuccess" CssClass="status-label success"></asp:Label>
                    <hr />

                    <!-- Form Fields -->
                    <div class="form-group">
                        <label><i class="fas fa-tasks me-1"></i>Select Academic Year</label>
                        <div class="input-group">
                            <div class="form-icon"><i class="fas fa-gear"></i></div>
                            <asp:DropDownList class="form-control" ID="drpAcademicYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpAcademicYear_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="error-message" id="academicYear-error">Please select option</div>
                    </div>
                    <div class="form-group">
                        <label><i class="fas fa-tasks me-1"></i>Select Action</label>
                        <div class="input-group">
                            <div class="form-icon"><i class="fas fa-gear"></i></div>
                            <asp:DropDownList CssClass="form-control" ID="drpUpsert" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpUpsert_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="error-message" id="upsert-error">Please select option</div>
                    </div>
                    <div class="form-group">
                        <label><i class="fas fa-tasks me-1"></i>Select Term / Semester</label>
                        <div class="input-group">
                            <div class="form-icon"><i class="fas fa-gear"></i></div>
                            <asp:DropDownList class="form-control" ID="drpTerm" runat="server" CssClass="form-control">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Term 1" Text="Term 1"></asp:ListItem>
                                <asp:ListItem Value="Term 2" Text="Term 2"></asp:ListItem>
                                <asp:ListItem Value="Term 3" Text="Term 3"></asp:ListItem>
                                <asp:ListItem Value="Semister 1" Text="Semister 1"></asp:ListItem>
                                <asp:ListItem Value="Semister 2" Text="Semister 2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="error-message" id="term-error">Please select option</div>
                    </div>

                    <div class="form-group">
                        <label><i class="fas fa-hourglass-start me-1"></i>Start Year</label>
                        <div class="input-group">
                            <div class="form-icon"><i class="fas fa-calendar-days"></i></div>
                            <asp:TextBox ID="txtStartDate" TextMode="Date" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="error-message" id="startdate-error">Please select start date</div>
                    </div>

                    <div class="form-group">
                        <label><i class="fas fa-hourglass-end me-1"></i>End Year</label>
                        <div class="input-group">
                            <div class="form-icon"><i class="fas fa-calendar-check"></i></div>
                            <asp:TextBox ID="txtEndDate" TextMode="Date" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="error-message" id="enddate-error">Please select end year</div>
                    </div>
                                        <div class="form-group">
    <label><i class="fas fa-tasks me-1"></i> Status</label>
    <div class="input-group">
        <div class="form-icon"><i class="fas fa-gear"></i></div>
        <asp:DropDownList CssClass="form-control" ID="drpStatus" runat="server">
            <asp:ListItem Value="" Text=""></asp:ListItem>
            <asp:ListItem Value="False" Text="In Active"></asp:ListItem>
            <asp:ListItem Value="True" Text="Active"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="error-message" id="drpStatus-error">Please select option</div>
</div>

                    <!-- Buttons -->
                    <div class="mt-4 text-center">
                        <asp:Button ID="btnsave" runat="server" Text="Save" CssClass="btn btn-primary me-2" OnClientClick="return validateForm();" OnClick="btnsave_Click" />
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success me-2" OnClientClick="return validateForm();" OnClick="btnUpdate_Click" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger me-2" OnClientClick="return validateForm2();" OnClick="btnDelete_Click" />
                        <asp:Button ID="Button1" runat="server" Text="Back" CssClass="btn btn-secondary" OnClick="Button1_Click" />
                    </div>
                </div>

                <!-- Hidden fields -->
                <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
                <asp:Label runat="server" ID="lblUser" Visible="false"></asp:Label>

            </div>
        </div>

        <!-- SweetAlert -->
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script>
            function validateForm() {
                let isValid = true;
                document.querySelectorAll('.error-message').forEach(el => el.style.display = 'none');

                if (!document.getElementById('<%= drpAcademicYear.ClientID %>').value.trim()) {
                    document.getElementById('academicYear-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= drpUpsert.ClientID %>').value.trim()) {
                    document.getElementById('upsert-error').style.display = 'block';
                    isValid = false;
                }
                if (!document.getElementById('<%= drpTerm.ClientID %>').value.trim()) {
                    document.getElementById('term-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= txtStartDate.ClientID %>').value.trim()) {
                    document.getElementById('startdate-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= txtEndDate.ClientID %>').value.trim()) {
                    document.getElementById('enddate-error').style.display = 'block';
                    isValid = false;
                }
                if (!document.getElementById('<%= drpStatus.ClientID %>').value.trim()) {
                    document.getElementById('drpStatus-error').style.display = 'block';
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

            function validateForm2() {
                let isValid = true;
                document.querySelectorAll('.error-message').forEach(el => el.style.display = 'none');

                if (!document.getElementById('<%= drpUpsert.ClientID %>').value.trim()) {
                    document.getElementById('upsert-error').style.display = 'block';
                    isValid = false;
                }

                if (!isValid) {
                    Swal.fire({
                        title: 'Validation Error',
                        text: 'Please select an action',
                        icon: 'error',
                        confirmButtonColor: '#0056b3'
                    });
                    return false;
                }
            }

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

            //success
            function showAlert(type, message) {
                Swal.fire({
                    title: type === 'success' ? 'Success!' : 'Error!',
                    text: message,
                    icon: type,
                    confirmButtonColor: 'var(--primary)'
                });
            }
        </script>
    </form>
</body>
</html>
