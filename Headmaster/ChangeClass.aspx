<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeClass.aspx.cs" Inherits="AddStudent" %>

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

        /* Button Base */
        button, .btn {
            border: none;
            padding: 12px 24px;
            border-radius: var(--radius, 6px);
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: 14px;
        }

        /* Primary */
        .btn-primary {
            background: var(--primary, #0ea5e9);
            color: #fff;
        }

            .btn-primary:hover {
                background: #0d94d4;
                transform: translateY(-1px);
            }

        /* Secondary */
        .btn-secondary {
            background: #e5e7eb;
            color: var(--text, #374151);
        }

            .btn-secondary:hover {
                background: #d1d5db;
                transform: translateY(-1px);
            }

        /* Warning */
        .btn-warning {
            background: #f59e0b;
            color: #fff;
        }

            .btn-warning:hover {
                background: #d97706;
                transform: translateY(-1px);
            }

        /* Danger */
        .btn-danger {
            background: #ef4444;
            color: #fff;
        }

            .btn-danger:hover {
                background: #dc2626;
                transform: translateY(-1px);
            }


            /* Page Header */
.page-header {
    margin: 20px 0;
}
.page-header h1 {
    font-size: 1.75rem;
    font-weight: 600;
    color: var(--text);
    margin-bottom: 6px;
}
.page-header .subtitle {
    font-size: 14px;
    color: var(--muted);
}

/* Info Box */
.info-box {
    background: #e0f2fe;
    border-left: 4px solid var(--primary);
    padding: 12px 16px;
    border-radius: var(--radius);
    color: #0369a1;
    font-size: 14px;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 10px;
}

/* Checkbox List */
.styled-checkboxlist input[type="checkbox"] {
    margin-right: 8px;
    accent-color: var(--primary);
}
.styled-checkboxlist label {
    display: flex;
    align-items: center;
    padding: 6px 0;
    font-size: 14px;
    color: var(--text);
}
.student-list {
    border: 1px solid #e5e7eb;
    border-radius: var(--radius);
    padding: 10px 15px;
    background: #fff;
    max-height: 200px;
    overflow-y: auto;
}
.full-width {
    grid-column: span 2;
}

/* ======= Modern Checkbox List Styling ======= */
.student-list {
    background: #fff;
    border: 1px solid #e5e7eb;
    border-radius: 10px;
    padding: 16px 20px;
    max-height: 250px;
    overflow-y: auto;
    box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

/* Remove default ASP.NET checkbox list layout */
.styled-checkboxlist input[type="checkbox"] {
    appearance: none;
    width: 18px;
    height: 18px;
    border: 2px solid #ccc;
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.2s ease;
    position: relative;
}

.styled-checkboxlist input[type="checkbox"]:checked {
    background-color: var(--primary, #2563eb);
    border-color: var(--primary, #2563eb);
}

.styled-checkboxlist input[type="checkbox"]:checked::after {
    content: "✔";
    position: absolute;
    color: #fff;
    font-size: 12px;
    top: 0;
    left: 3px;
}

/* Label alignment */
.styled-checkboxlist label {
    display: flex;
    align-items: center;
    gap: 10px;
    font-size: 15px;
    color: #333;
    padding: 8px 0;
    cursor: pointer;
    transition: background 0.2s ease;
}

.styled-checkboxlist label:hover {
    background: #f9fafb;
    border-radius: 6px;
}

/* Scrollbar customization */
.student-list::-webkit-scrollbar {
    width: 6px;
}
.student-list::-webkit-scrollbar-thumb {
    background: #cbd5e1;
    border-radius: 10px;
}

/* Label heading styling */
.form-label {
    font-weight: 600;
    color: #1f2937;
    margin-bottom: 8px;
    display: flex;
    align-items: center;
    gap: 8px;
}

/* Error message */
.error-message {
    color: #dc2626;
    font-size: 13px;
    margin-top: 5px;
    display: none;
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
    <!-- Breadcrumbs -->
    <div class="crumbs">
        <i class="fa-solid fa-school"></i>
        <a href="Dashboard.aspx">Dashboard</a> 
        <i class="fa-solid fa-angle-right"></i>
        <span>Change Class</span>
    </div>

    <!-- Page Title -->
    <div class="page-header">
        <h1><i class="fa-solid fa-chalkboard-user"></i> Change Student Class</h1>
        <p class="subtitle">Manage student promotions or transfers between classes.</p>
    </div>

    <!-- Info Notice -->
    <div class="info-box">
        <i class="fa-solid fa-circle-info"></i>
        <asp:Label runat="server" ID="lblMessage" 
            Text="NOTE: When updating classes, please start with the upper classes and move downward (e.g., Form 4 → 3 → 2 → 1).">
        </asp:Label>
    </div>

    <!-- Form Container -->
    <div class="form-container">
        <div class="form-grid">
            <!-- Current Class -->
            <div class="form-group">
                <label class="form-label" for="drpClass">Select Current Class</label>
                <div class="input-with-icon">
                    <i class="fas fa-layer-group"></i>
                    <asp:DropDownList CssClass="form-control" ID="drpClass" runat="server"></asp:DropDownList>
                </div>
                <div class="error-message" id="currentClass-error">Please select the current class</div>
            </div>

            <!-- New Class -->
            <div class="form-group">
                <label class="form-label" for="drpNewClass">New Class</label>
                <div class="input-with-icon">
                    <i class="fas fa-chalkboard"></i>
                    <asp:DropDownList CssClass="form-control" ID="drpNewClass" runat="server" 
                        AutoPostBack="true" OnSelectedIndexChanged="drpPrevClass_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="error-message" id="newClass-error">Please select a new class</div>
            </div>

            <!-- Students -->
           <div class="form-group full-width">
    <label class="form-label" for="chkStudents">
        <i class="fas fa-users"></i> Select Students
    </label>

    <div class="student-list">
        <asp:CheckBoxList ID="chkStudents" runat="server" CssClass="styled-checkboxlist"></asp:CheckBoxList>
    </div>

    <div class="error-message" id="student-error">Please select at least one student</div>
</div>

        </div>

        <!-- Actions -->
        <div class="button-container">
            <asp:Button runat="server" ID="btnRegister" CssClass="btn-primary" 
                Text="Update Class" OnClientClick="return validateForm()" OnClick="btnSave_Click" />
            <button class="btn-secondary" onclick="return confirmCancel();">
                <i class="fas fa-times-circle"></i> Back
            </button>
        </div>
    </div>

    <!-- Hidden Error Toast -->
    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true"></asp:Label>
    <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
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
                        window.location.href = 'Dashboard.aspx';
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


                if (!document.getElementById('<%= drpClass.ClientID %>').value.trim()) {
                    document.getElementById('currentClass-error').style.display = 'block';
                    isValid = false;
                }


                if (!document.getElementById('<%= drpNewClass.ClientID %>').value) {
                    document.getElementById('newClass-error').style.display = 'block';
                    isValid = false;
                }


                if (!document.getElementById('<%= chkStudents.ClientID %>').value.trim()) {
                    document.getElementById('student-error').style.display = 'block';
                    isValid = false;
                }

                document.getElementById('<%= drpClass.ClientID %>').addEventListener('input', function () {
                    document.getElementById('currentClass-error').style.display = 'none';
                });

                document.getElementById('<%= drpNewClass.ClientID %>').addEventListener('change', function () {
                    document.getElementById('newClass-error').style.display = 'none';
                });


                document.getElementById('<%= chkStudents.ClientID %>').addEventListener('input', function () {
                    document.getElementById('student-error').style.display = 'none';
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
            function showAlert(type, message) {
                Swal.fire({
                    title: type === 'success' ? 'Success!' : 'Error!',
                    text: message,
                    icon: type,
                    confirmButtonColor: 'var(--primary)'
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
