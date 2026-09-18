<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="ExamCheck.aspx.cs" Inherits="ExamMark" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>School - Exam Marks</title>
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

    <!-- DataTables -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.10.21/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/1.6.2/css/buttons.dataTables.min.css" />

    <!-- Font Awesome for icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />

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

        /* Info Section */
        .exam-info {
            background: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 25px;
            border-left: 4px solid #3498db;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
        }

        .info-item {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .info-label {
            font-weight: 600;
            color: #555;
            min-width: 80px;
        }

        .info-value {
            color: #2c3e50;
            font-weight: 500;
        }

        /* Editable Cells */
        .editable-cell {
            position: relative;
        }

        .marks-input {
            width: 100px;
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 4px;
            text-align: center;
            font-size: 14px;
            transition: all 0.3s ease;
        }

            .marks-input:focus {
                outline: none;
                border-color: #3498db;
                box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2);
            }

            .marks-input.saving {
                background-color: #fffde7;
                border-color: #ffc107;
            }

            .marks-input.saved {
                background-color: #e8f5e9;
                border-color: #4caf50;
            }

            .marks-input.error {
                background-color: #ffebee;
                border-color: #f44336;
            }

        /* Status indicators */
        .status-icon {
            margin-left: 5px;
            font-size: 12px;
        }

        .saving-icon {
            color: #ff9800;
            animation: spin 1s linear infinite;
        }

        .saved-icon {
            color: #4caf50;
        }

        .error-icon {
            color: #f44336;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
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




        /* Save status */
        .save-status {
            margin-top: 10px;
            padding: 10px;
            border-radius: 4px;
            display: none;
        }

            .save-status.success {
                background-color: #d4edda;
                color: #155724;
                border: 1px solid #c3e6cb;
            }

            .save-status.error {
                background-color: #f8d7da;
                color: #721c24;
                border: 1px solid #f5c6cb;
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

        /* tables */
        /* Search Box */
        .search-box-container {
            margin-bottom: 25px;
            max-width: 400px;
        }

        /* Modern Table Styles */
        .modern-table-container {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
            overflow: hidden;
        }

        .modern-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 14px;
        }

            .modern-table th {
                background-color: #f8fafc;
                color: #64748b;
                font-weight: 600;
                text-align: left;
                padding: 16px 20px;
                border-bottom: 1px solid #e2e8f0;
                text-transform: uppercase;
                font-size: 13px;
                letter-spacing: 0.5px;
            }

            .modern-table td {
                padding: 16px 20px;
                border-bottom: 1px solid #f1f5f9;
                color: #334155;
                vertical-align: middle;
            }

            .modern-table tr:last-child td {
                border-bottom: none;
            }

            .modern-table tr:hover td {
                background-color: #f8fafc;
            }

        /* Responsive Adjustments */
        @media (max-width: 768px) {
            .modern-table-container {
                overflow-x: auto;
            }

            .modern-table {
                min-width: 600px;
            }

            .search-box-container {
                max-width: 100%;
            }

            .marks-input {
                width: 80px;
            }
        }

         /* Error Message Styling */
 .error-message {
     color: var(--red);
     font-size: 13px;
     margin-top: 5px;
     display: none;
 }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

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
                            <asp:Label runat="server" ID="lblUser"></asp:Label>
                        </div>
                        <div class="status"><i class="fa-solid fa-circle" style="font-size: 8px"></i>Online</div>
                    </div>
                </div>

                <nav class="nav">
                    <h5>Main Navigation</h5>
                    <div class="dropdown">
                        <a href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i>Dashboard</a>
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
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a>
                    <i class="fa-solid fa-angle-right"></i><a href="Exams.aspx">Exams</a>
                    <i class="fa-solid fa-angle-right"></i><span>Check Marks</span>
                </div>

                <div class="form-container">
                    <h2 class="form-header">Check Marked Subjects</h2>

                    <!-- Exam Information -->
                    <div class="exam-info">
                        <div class="info-grid">
                            <div class="info-item">

                                <span class="info-value" id="examNameDisplay"><%= GetExamName() %></span>
                            </div>
                            <div class="info-item">

                                <span class="info-value" id="classNameDisplay"><%= GetClassName() %></span>
                            </div>
                            <div class="info-item">
                                <span class="info-label">Total Subjects:</span>
                                <span class="info-value" id="subjectTotalDisplay"><%= GetTotalSubjects() %></span>
                            </div>
                            <div class="info-item">
                                <span class="info-label">Class Teacher:</span>
                                <span class="info-value"><%= lblTeacherName.Text %></span>
                            </div>
                        </div>
                    </div>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>

                    <!-- Save Status -->
                    <div id="saveStatus" class="save-status"></div>
                    
                    <div class="button-container">
                        <asp:Button runat="server" ID="btnSave" CssClass="btn-primary" Text="Save Marked Subjects" OnClick="btnSave_Click" Visible="false" />
                        <asp:Button runat="server" ID="bdnDelete" CssClass="btn-secondary" Text="Delete Marked Subjects" OnClick="bdnDelete_Click" Visible="false" />
                    </div>
                    <br />
                    <div class="card-body">
                        <div class="modern-table-container">
                            <table id="examTable" class="modern-table">
                                <thead>
                                    <tr>
                                        <th>Subject </th>
                                        <th>Allocated To</th>
                                        <th>Class Total</th>
                                        <th>Total Marked</th>
                                        <th>Unmarked / Others</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% LoadMarked(); %>
                                </tbody>
                            </table>
                        </div>


                    </div>

                    <!-- Hidden Fields -->
                    <asp:HiddenField runat="server" ID="hdnExamId" Value='<%= Session["exam"] != null ? Session["exam"].ToString() : "" %>' />
                    <asp:HiddenField runat="server" ID="hdnSubjectId" Value='<%= Session["subject"] != null ? Session["subject"].ToString() : "" %>' />
                    <asp:HiddenField runat="server" ID="hdnClassId" Value='<%= Session["classId"] != null ? Session["classId"].ToString() : "" %>' />

                    <asp:Label runat="server" ID="lblSchoolId" Visible="true" ForeColor="White"></asp:Label>
                    <br />
                    <asp:Label runat="server" ID="lblTeacherId" Visible="true" ForeColor="White"></asp:Label>
                     <asp:Label runat="server" ID="lbluserId" Visible="true" ForeColor="White"></asp:Label>
                     <asp:Label runat="server" ID="lblTeacherName" Visible="true" ForeColor="White"></asp:Label>

                </div>
            </main>
            <!-- Content ends here  -->
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
        <script src="../js/customJs.js"></script>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <!-- DataTables JS -->
        <script src="https://cdn.datatables.net/1.10.21/js/jquery.dataTables.min.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/dataTables.buttons.min.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/buttons.print.min.js"></script>

        <script>
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
                        window.location.href = 'Schools.aspx'; // Redirect to schools page
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

            $(document).ready(function () {
                $('#examTable').DataTable({
                    "paging": true,
                    "lengthChange": true,
                    "searching": true,
                    "ordering": true,
                    "info": true,
                    "autoWidth": false,
                    "responsive": true
                });
            });
        </script>
    </form>
</body>
</html>
