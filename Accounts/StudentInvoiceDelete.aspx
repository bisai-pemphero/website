<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StudentInvoiceDelete.aspx.cs" Inherits="StudentInvoiceCreate" %>

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
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a> <i class="fa-solid fa-angle-right"></i><a>Batch Invoice</a>
                </div>

                <div class="form-container">
                    <h2 class="form-header">Delete Class Invoice</h2>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>

                    <div class="form-grid">

                        <div class="form-group">
                            <label class="form-label" for="academic_year">Academic Year</label>
                            <div class="input-with-icon">
                                <i class="fas fa-university"></i>
                                <asp:DropDownList runat="server" ID="ddlAcademicYear" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="error-message" id="academic-year-error">Please select academic year</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Term">Term</label>
                            <div class="input-with-icon">
                                <i class="fas fa-calendar-day"></i>
                                <asp:DropDownList ID="ddlTerm" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="term-error">Please select Term</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="class">Class</label>
                            <div class="input-with-icon">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:DropDownList ID="ddlClass" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="class-error">Please Select Class</div>
                        </div>
                        <div class="form-group">
                            <label class="form-label" for="category">Fees Category</label>
                            <div class="input-with-icon">
                                <i class="fas fa-layer-group"></i>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="category-error">Please Select Fees Category</div>
                        </div>

                    </div>

                    <div class="button-container">
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn-secondary" Text="Clear" OnClick="btnCancel_Click" />
                        <asp:Button runat="server" ID="btnSave" CssClass="btn-primary" Text="Delete Invoice" OnClick="btnSave_Click" OnClientClick="return validateForm()" />
                    </div>
                </div>

                <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>

                <asp:Label runat="server" ID="lblParentId" Visible="false"></asp:Label>
            </main>


            <!-- Content ends here  -->
        </div>



        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
        <script src="../js/customJs.js"></script>

        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>


        <script>

            //inputs
            // Toggle password visibility
            function togglePassword(inputId, button) {
                const input = document.getElementById(inputId);
                const icon = button.querySelector('i');

                if (input.type === 'password') {
                    input.type = 'text';
                    icon.classList.remove('fa-eye');
                    icon.classList.add('fa-eye-slash');
                } else {
                    input.type = 'password';
                    icon.classList.remove('fa-eye-slash');
                    icon.classList.add('fa-eye');
                }
            }

            // Confirm cancel action
            function confirmCancel() {
                Swal.fire({
                    title: 'Are you sure?',
                    text: 'You have unsaved changes that will be lost',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: 'var(--primary)',
                    cancelButtonColor: 'var(--muted)',
                    confirmButtonText: 'Yes, cancel'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = 'Schools.aspx';
                    }
                });
                return false;
            }


            //sweet alerts

            // Modal functionality
            const modal = document.getElementById('studentModal');
            const openModalBtn = document.getElementById('addStudentBtn');
            const closeModalBtns = document.querySelectorAll('.close-modal');

            openModalBtn.addEventListener('click', () => {
                modal.style.display = 'flex';
            });

            closeModalBtns.forEach(btn => {
                btn.addEventListener('click', () => {
                    modal.style.display = 'none';
                });
            });

            // Close modal when clicking outside
            window.addEventListener('click', (e) => {
                if (e.target === modal) {
                    modal.style.display = 'none';
                }
            });

            // SweetAlert examples
            document.getElementById('saveBtn').addEventListener('click', () => {
                Swal.fire({
                    title: 'Success!',
                    text: 'Student information saved successfully',
                    icon: 'success',
                    confirmButtonColor: 'var(--primary)'
                });
            });

            document.getElementById('confirmAdd').addEventListener('click', () => {
                modal.style.display = 'none';
                Swal.fire({
                    title: 'Are you sure?',
                    text: "You're about to add a new student",
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: 'var(--primary)',
                    cancelButtonColor: 'var(--muted)',
                    confirmButtonText: 'Yes, add it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        Swal.fire(
                            'Added!',
                            'The student has been added.',
                            'success'
                        );
                    }
                });
            });

            // Delete confirmation
            document.querySelectorAll('.btn-danger').forEach(btn => {
                btn.addEventListener('click', function () {
                    const row = this.closest('tr');
                    const studentName = row.querySelector('td:nth-child(2)').textContent;

                    Swal.fire({
                        title: 'Delete Student?',
                        html: `You're about to delete <strong>${studentName}</strong>`,
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: 'var(--red)',
                        cancelButtonColor: 'var(--muted)',
                        confirmButtonText: 'Yes, delete it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            row.remove();
                            Swal.fire(
                                'Deleted!',
                                'The student record has been deleted.',
                                'success'
                            );
                        }
                    });
                });
            });


            // Client-side validation and alert handling
            function validateForm() {
                // Perform client-side validation
                let isValid = true;

                // Reset error messages
                document.querySelectorAll('.error-message').forEach(el => {
                    el.style.display = 'none';
                });


                if (!document.getElementById('<%= ddlAcademicYear.ClientID %>').value.trim()) {
                    document.getElementById('academic-year-error').style.display = 'block';
                    isValid = false;
                }


                if (!document.getElementById('<%= ddlTerm.ClientID %>').value) {
                    document.getElementById('term-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= ddlClass.ClientID %>').value.trim()) {
                    document.getElementById('class-error').style.display = 'block';
                    isValid = false;
                }

                if (!document.getElementById('<%= ddlCategory.ClientID %>').value.trim()) {
                    document.getElementById('category-error').style.display = 'block';
                    isValid = false;
                }

                //clear 
                document.getElementById('<%= ddlAcademicYear.ClientID %>').addEventListener('input', function () {
                    document.getElementById('academic-year-erro').style.display = 'none';
                });

                document.getElementById('<%= ddlTerm.ClientID %>').addEventListener('change', function () {
                    document.getElementById('term-error').style.display = 'none';
                });

                document.getElementById('<%= ddlClass.ClientID %>').addEventListener('input', function () {
                    document.getElementById('class-error').style.display = 'none';
                });

                document.getElementById('<%= ddlCategory.ClientID %>').addEventListener('input', function () {
                    document.getElementById('category-error').style.display = 'none';
                });

                if (!isValid) {
                    Swal.fire({
                        title: 'Validation Error',
                        text: 'Please fill in all required fields',
                        icon: 'error',
                        confirmButtonColor: 'var(--primary)'
                    });
                    return false;
                }
            }

            //success
            function showAlert(type, message, redirectUrl = null) {
                Swal.fire({
                    title: type === 'success' ? 'Success!' : 'Error!',
                    text: message,
                    icon: type,
                    confirmButtonColor: 'var(--primary)',
                    didClose: () => {
                        if (type === 'success' && redirectUrl) {
                            window.location.href = redirectUrl;
                        }
                    }
                });
            }

            //Custom Error Toast
            function showErrorToast(message) {
                var toast = document.getElementById('<%= lblError.ClientID %>');
                toast.innerHTML = message;
                toast.classList.remove('hide');
                toast.classList.add('show');

                // Auto-hide after 10 seconds
                setTimeout(function () {
                    hideErrorToast();
                }, 10000);
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
