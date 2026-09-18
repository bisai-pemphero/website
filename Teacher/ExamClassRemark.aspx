<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="ExamClassRemark.aspx.cs" Inherits="ViewStudent" %>

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

    <!-- DataTables -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.10.21/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/1.6.2/css/buttons.dataTables.min.css" />

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

        /* Action Buttons */
        .actions-cell {
            white-space: nowrap;
        }

        .action-buttons {
            display: flex;
            gap: 8px;
        }

        .btn-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 32px;
            height: 32px;
            border-radius: 6px;
            background: #f1f5f9;
            color: #64748b;
            transition: all 0.2s ease;
            border: none;
            cursor: pointer;
        }

        .btn-view:hover {
            background: #3b82f6;
            color: white;
        }

        .btn-delete:hover {
            background: #ef4444;
            color: white;
        }

        /* Modern Pagination */
        .modern-pagination {
            display: flex;
            padding: 16px;
            justify-content: center;
            list-style: none;
            background: white;
            border-top: 1px solid #f1f5f9;
        }

            .modern-pagination a {
                color: #64748b;
                padding: 8px 12px;
                text-decoration: none;
                border-radius: 6px;
                margin: 0 2px;
                font-size: 14px;
                transition: all 0.2s ease;
                border: 1px solid transparent;
            }

                .modern-pagination a:hover {
                    background: #f1f5f9;
                    color: #3b82f6;
                }

            .modern-pagination span {
                padding: 8px 12px;
                background-color: #3b82f6;
                color: white;
                border-radius: 6px;
                margin: 0 2px;
                font-size: 14px;
                border: 1px solid #3b82f6;
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
        }
/* Auto-save feedback */
.remarks-input.saving {
    background-color: #fffde7;
    border-color: #ffc107;
}

.remarks-input.saved {
    background-color: #e8f5e9;
    border-color: #4caf50;
    animation: savedPulse 0.5s ease;
}

.remarks-input.error {
    background-color: #ffebee;
    border-color: #f44336;
}

@keyframes savedPulse {
    0% { box-shadow: 0 0 0 0 rgba(76, 175, 80, 0.4); }
    70% { box-shadow: 0 0 0 6px rgba(76, 175, 80, 0); }
    100% { box-shadow: 0 0 0 0 rgba(76, 175, 80, 0); }
}

/* Save status */
.save-status {
    margin-top: 10px;
    padding: 10px;
    border-radius: 4px;
    display: none;
    font-size: 13px;
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

.save-status.info {
    background-color: #d1ecf1;
    color: #0c5460;
    border: 1px solid #bee5eb;
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
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a>
                </div>

                <div class="form-container">
                    <h2 class="form-header">Exam Remarks</h2>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>


                  
                    <!-- Table here  -->
                    <div class="card-body">
                       

                        <div id="saveStatus" class="save-status" style="display: none;"></div>
                        <br />
                        <table id="examResults" class="modern-table">
                            <thead>
                                <tr>
                                    <th>POSITION</th>
                                    <th>FIRSTNAME</th>
                                    <th>MIDDLE NAME</th>
                                    <th>LASTNAME</th>
                                    <th>BEST SIX</th>
                                    <th>ENGLISH</th>
                                    <th>REMARK</th>
                                    <th>CLASS RANK</th>
                                    <th>RESULT</th>
                                    <th>TEACHERS REMARK</th>
                                </tr>
                            </thead>
                            <tbody>
                                <% LoadResults(); %>
                            </tbody>
                        </table>
                    </div>



                    <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                    <asp:Label runat="server" ID="lblParaId" Visible="false"></asp:Label>

                 
                </div>
            </main>
            <!-- Content Ends Here  -->
        </div>

        <!-- DataTables JS -->
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="https://cdn.datatables.net/1.10.21/js/jquery.dataTables.min.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/dataTables.buttons.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/buttons.html5.min.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/buttons.print.min.js"></script>

       

    <script>
        $(document).ready(function () {
            var remarksData = {};
            var isSaving = false;
            var saveTimeouts = {};
            var lastSaved = {};

            // Store original remarks for comparison
            $('.remarks-input').each(function () {
                var studentId = $(this).data('student-id');
                var currentVal = $(this).val().trim();
                remarksData[studentId] = currentVal;
                lastSaved[studentId] = currentVal;
            });

            // Initialize DataTable
            var table = $('#examResults').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'csvHtml5',
                        text: '<i class="fa-solid fa-file-csv"></i> CSV',
                        className: 'btn btn-primary',
                        title: 'ExamResults'
                    },
                    {
                        extend: 'pdfHtml5',
                        text: '<i class="fa-solid fa-file-pdf"></i> PDF',
                        className: 'btn btn-secondary',
                        title: 'ExamResults',
                        orientation: 'landscape',
                        pageSize: 'A4',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print',
                        text: '<i class="fa-solid fa-print"></i> Print',
                        className: 'btn btn-warning',
                        title: 'ExamResults'
                    }
                ],
                "paging": true,
                "lengthChange": true,
                "searching": true,
                "ordering": true,
                "info": true,
                "autoWidth": false,
                "responsive": true
            });

            // Save All Remarks button
            $('#btnSaveRemarks').click(function () {
                var unsavedChanges = [];

                $('.remarks-input').each(function () {
                    var studentId = $(this).data('student-id');
                    var currentValue = $(this).val().trim();
                    var studentName = $(this).closest('tr').find('td:eq(1)').text() + ' ' +
                        $(this).closest('tr').find('td:eq(3)').text();

                    if (currentValue !== lastSaved[studentId]) {
                        unsavedChanges.push({
                            studentId: studentId,
                            remark: currentValue,
                            studentName: studentName
                        });
                    }
                });

                if (unsavedChanges.length > 0) {
                    showSaveStatus('Saving ' + unsavedChanges.length + ' remarks...', 'info');

                    // Save each unsaved change
                    var savedCount = 0;
                    var totalToSave = unsavedChanges.length;

                    unsavedChanges.forEach(function (item) {
                        saveIndividualRemark(item.studentId, item.remark, function () {
                            savedCount++;
                            if (savedCount === totalToSave) {
                                showSaveStatus('All ' + totalToSave + ' remarks saved successfully!', 'success');
                            }
                        });
                    });
                } else {
                    showSaveStatus('No changes to save', 'info');
                }
            });

            // Handle individual remark changes with auto-save
            $(document).on('input', '.remarks-input', function () {
                var $input = $(this);
                var studentId = $input.data('student-id');
                var remark = $input.val().trim();

                // Clear any existing timeout for this student
                if (saveTimeouts[studentId]) {
                    clearTimeout(saveTimeouts[studentId]);
                }

                // Set a timeout to auto-save after 2 seconds of inactivity
                saveTimeouts[studentId] = setTimeout(function () {
                    if (remark !== lastSaved[studentId]) {
                        saveIndividualRemark(studentId, remark);
                    }
                }, 2000);

                // Visual feedback
                $input.addClass('saving').removeClass('saved error');
            });

            // Also save on blur (when user leaves the field)
            $(document).on('blur', '.remarks-input', function () {
                var $input = $(this);
                var studentId = $input.data('student-id');
                var remark = $input.val().trim();

                // Clear the timeout since we're saving now
                if (saveTimeouts[studentId]) {
                    clearTimeout(saveTimeouts[studentId]);
                }

                if (remark !== lastSaved[studentId]) {
                    saveIndividualRemark(studentId, remark);
                }
            });

            function saveIndividualRemark(studentId, remark, callback) {
                if (isSaving && !callback) return;

                var $input = $('#remark-input-' + studentId);
                var studentName = $input.closest('tr').data('student-name') ||
                    $input.closest('tr').find('td:eq(1)').text() + ' ' +
                    $input.closest('tr').find('td:eq(3)').text();

                isSaving = true;
                $input.addClass('saving').removeClass('saved error');

                // Get required parameters
                var examId = '<%= Session["exam"] ?? "0" %>';
            var schoolId = '<%= lblSchoolId.Text.Trim() %>';
            var postedBy = '<%= lblSession.Text.Trim() %>';

            // Prepare data
            var data = {
                studentId: parseInt(studentId),
                remark: remark,
                examId: parseInt(examId),
                schoolId: parseInt(schoolId),
                postedBy: postedBy
            };

            // AJAX call
            $.ajax({
                type: "POST",
                url: "ExamClassRemark.aspx/SaveTeacherRemark",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 10000, // 10 seconds timeout
                success: function (response) {
                    if (response && response.d) {
                        if (response.d.success) {
                            // Update last saved value
                            lastSaved[studentId] = remark;
                            
                            // Visual feedback
                            $input.removeClass('saving').addClass('saved');
                            showSaveStatus('Remark saved for ' + studentName, 'success');
                            
                            // Remove success class after 2 seconds
                            setTimeout(function() {
                                $input.removeClass('saved');
                            }, 2000);
                        } else {
                            $input.removeClass('saving').addClass('error');
                            showSaveStatus('Error saving for ' + studentName + ': ' + response.d.message, 'error');
                        }
                    } else {
                        $input.removeClass('saving').addClass('error');
                        showSaveStatus('Invalid response from server', 'error');
                    }
                },
                error: function (xhr, status, error) {
                    $input.removeClass('saving').addClass('error');
                    showSaveStatus('Network error for ' + studentName, 'error');
                },
                complete: function () {
                    isSaving = false;
                    if (callback) callback();
                }
            });
        }

        function showSaveStatus(message, type) {
            var statusDiv = $('#saveStatus');
            statusDiv.removeClass('success error info').addClass(type);
            statusDiv.text(message).show();

            // Auto-hide after appropriate time
            var hideTime = type === 'error' ? 8000 : 3000;
            setTimeout(function () {
                if (type !== 'error' || !$('.remarks-input.error').length) {
                    statusDiv.fadeOut();
                }
            }, hideTime);
        }

        // Custom Error Toast
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

        // Show initial info message
        showSaveStatus('Remarks field is ready for editing. Changes are auto-saved after 2 seconds of inactivity.', 'info');
    });
    </script>

    </form>
</body>
</html>
