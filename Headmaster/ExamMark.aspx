<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="ExamMark.aspx.cs" Inherits="ExamMark" %>

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
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        /* Action buttons */
        .action-buttons {
            display: flex;
            gap: 10px;
            margin-top: 20px;
            justify-content: flex-end;
        }

        .btn-save-all {
            background: #27ae60;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 8px;
            font-weight: 500;
            transition: background 0.3s;
        }

        .btn-save-all:hover {
            background: #219653;
        }

        .btn-save-all:disabled {
            background: #95a5a6;
            cursor: not-allowed;
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
                    <i class="fa-solid fa-angle-right"></i><span>Enter Marks</span>
                </div>

                <div class="form-container">
                    <h2 class="form-header">Enter Exam Marks</h2>

                    <!-- Exam Information -->
                    <div class="exam-info">
                        <div class="info-grid">
                            <div class="info-item">
                                <span class="info-label">Exam:</span>
                                <span class="info-value" id="examNameDisplay"><%= GetExamName() %></span>
                            </div>
                            <div class="info-item">
                                <span class="info-label">Class:</span>
                                <span class="info-value" id="classNameDisplay"><%= GetClassName() %></span>
                            </div>
                            <div class="info-item">
                                <span class="info-label">Subject:</span>
                                <span class="info-value" id="subjectNameDisplay"><%= GetSubjectName() %></span>
                            </div>
                            <div class="info-item">
                                <span class="info-label">Teacher:</span>
                                <span class="info-value"><%= lblUser.Text %></span>
                            </div>
                        </div>
                    </div>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>

                    <!-- Save Status -->
                    <div id="saveStatus" class="save-status"></div>

                    <div class="card-body">
                        <div class="modern-table-container">
                            <table id="examTable" class="modern-table">
                                <thead>
                                    <tr>
                                        <th>Student ID</th>
                                        <th>First Name</th>
                                        <th>Middle Name</th>
                                        <th>Last Name</th>
                                        <th>Marks</th>
                                        <th>Status</th>
                                    </tr>
                                </thead> 
                                <tbody>
                                    <% LoadExams(); %>
                                </tbody>
                            </table>
                        </div>

                     
                    </div>

                    <!-- Hidden Fields -->
                    <asp:HiddenField runat="server" ID="hdnExamId" Value='<%= Session["exam"] != null ? Session["exam"].ToString() : "" %>' />
                    <asp:HiddenField runat="server" ID="hdnSubjectId" Value='<%= Session["subject"] != null ? Session["subject"].ToString() : "" %>' />
                    <asp:HiddenField runat="server" ID="hdnClassId" Value='<%= Session["classId"] != null ? Session["classId"].ToString() : "" %>' />
                    
                 <asp:Label runat="server" ID="lblSchoolId" Visible="true" ForeColor="White"></asp:Label> <br />
                  <asp:Label runat="server" ID="lblTeacherId" Visible="true" ForeColor="White"></asp:Label>
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
            // Store marks data
            var marksData = {};
            var isSaving = false;
            var debugMode = true;

            // Debug logging function
            function logDebug(message, data) {
                if (debugMode) {
                    console.log(new Date().toISOString(), message, data || '');
                }
            }

            // Test WebMethod connection
            function testWebMethodConnection() {
                $.ajax({
                    type: "POST",
                    url: "ExamMark.aspx/TestConnection",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        logDebug("WebMethod test successful:", response);
                    },
                    error: function (xhr, status, error) {
                        logDebug("WebMethod test failed:", { status: status, error: error, response: xhr.responseText });
                        showErrorToast("WebMethods not working. Check console for details.");
                    }
                });
            }

            $(document).ready(function () {
                logDebug("Page loaded, testing connection...");
                testWebMethodConnection();

                // Initialize DataTable
                var table = $('#examTable').DataTable({
                    "lengthChange": true,
                    "searching": true,
                    "ordering": true,
                    "info": true,
                    "autoWidth": false,
                    "responsive": true,
                    "columnDefs": [
                        { "orderable": false, "targets": [4, 5] } // Don't allow ordering on Marks and Status columns
                    ]
                });

                // Initialize marksData from existing inputs
                $('.marks-input').each(function () {
                    var studentId = $(this).data('student-id');
                    marksData[studentId] = $(this).val();
                });

                // Handle individual mark changes
                $(document).on('blur', '.marks-input', function () {
                    var studentId = $(this).data('student-id');
                    var marks = $(this).val().trim();
                    var input = $(this);

                    // Validate marks
                    if (marks === '') {
                        showInputError(input, 'Marks cannot be empty');
                        return;
                    }

                    if (isNaN(marks) || marks < 0 || marks > 100) {
                        showInputError(input, 'Please enter valid marks (0-100)');
                        return;
                    }

                    // Update marksData
                    marksData[studentId] = marks;

                    // Save individual mark
                    saveIndividualMark(studentId, marks, input);
                });

                // Handle Enter key press
                $(document).on('keypress', '.marks-input', function (e) {
                    if (e.which === 13) { // Enter key
                        $(this).blur();
                        e.preventDefault();
                    }
                });

                // Save All button click
                $('#btnSaveAll').click(function () {
                    saveAllMarks();
                });

                // Update save button state
                updateSaveButtonState();
            });

            function saveIndividualMark(studentId, marks, inputElement) {
                // Show saving state
                inputElement.addClass('saving');
                inputElement.removeClass('saved error');

                // Update status cell
                var statusCell = inputElement.closest('tr').find('.status-cell');
                statusCell.html('<i class="fas fa-spinner fa-spin saving-icon"></i> Saving...');

                // Get all required parameters
                var examId = $('#<%= hdnExamId.ClientID %>').val();
                var subjectId = $('#<%= hdnSubjectId.ClientID %>').val();
                var teacherId = $('#<%= lblTeacherId.ClientID %>').text().trim();
                var schoolId = $('#<%= lblSchoolId.ClientID %>').text().trim();
                
                logDebug("Saving mark parameters:", {
                    studentId: studentId,
                    marks: marks,
                    examId: examId,
                    subjectId: subjectId,
                    teacherId: teacherId,
                    schoolId: schoolId
                });

                // Validate all parameters
                if (!examId || examId === '0') {
                    showInputError(inputElement, 'Exam ID is missing. Please refresh the page.');
                    return;
                }
                
                if (!subjectId || subjectId === '0') {
                    showInputError(inputElement, 'Subject ID is missing. Please refresh the page.');
                    return;
                }
                
                if (!teacherId || teacherId === '') {
                    showInputError(inputElement, 'Teacher ID is missing. Please refresh the page.');
                    return;
                }
                
                if (!schoolId || schoolId === '') {
                    showInputError(inputElement, 'School ID is missing. Please refresh the page.');
                    return;
                }

                // Prepare data
                var data = {
                    studentId: parseInt(studentId),
                    marks: parseFloat(marks),
                    examId: parseInt(examId),
                    subjectId: parseInt(subjectId),
                    teacherId: parseInt(teacherId),
                    schoolId: parseInt(schoolId)
                };

                logDebug("Sending AJAX request...", data);
                
                // AJAX call with enhanced error handling
                $.ajax({
                    type: "POST",
                    url: "ExamMark.aspx/SaveGrade",
                    data: JSON.stringify(data),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    timeout: 15000, // 15 second timeout
                    success: function(response) {
                        logDebug("AJAX response received:", response);
                        
                        if (response && response.d) {
                            if (response.d.success) {
                                // Show success state
                                inputElement.removeClass('saving');
                                inputElement.addClass('saved');
                                statusCell.html('<i class="fas fa-check saved-icon"></i> Saved');
                                
                                // Remove success state after 2 seconds
                                setTimeout(function() {
                                    inputElement.removeClass('saved');
                                    statusCell.html('');
                                }, 2000);
                                
                                // Update save button state
                                updateSaveButtonState();
                            } else {
                                showInputError(inputElement, response.d.message || 'Error saving marks');
                                statusCell.html('<i class="fas fa-times error-icon"></i> Error');
                            }
                        } else {
                            showInputError(inputElement, 'Invalid response format from server');
                            statusCell.html('<i class="fas fa-times error-icon"></i> Error');
                        }
                    },
                    error: function(xhr, status, error) {
                        logDebug("AJAX error occurred:", {
                            status: status,
                            error: error,
                            statusText: xhr.statusText,
                            statusCode: xhr.status,
                            responseText: xhr.responseText
                        });
                        
                        var errorMessage = 'Network error occurred. ';
                        
                        if (status === "timeout") {
                            errorMessage = 'Request timeout. Please try again.';
                        } else if (status === "parsererror") {
                            errorMessage = 'Error parsing server response. Check if WebMethod exists.';
                        } else if (xhr.status === 404) {
                            errorMessage = 'Page not found. Check the WebMethod URL.';
                        } else if (xhr.status === 500) {
                            errorMessage = 'Server error (500). Please contact administrator.';
                        } else if (xhr.status === 403) {
                            errorMessage = 'Access forbidden. Please check permissions.';
                        }
                        
                        showInputError(inputElement, errorMessage);
                        statusCell.html('<i class="fas fa-times error-icon"></i> Error');
                    },
                    complete: function() {
                        logDebug("AJAX request completed");
                    }
                });
            }

            function saveAllMarks() {
                if (isSaving) {
                    showErrorToast("Already saving. Please wait...");
                    return;
                }
                
                var btn = $('#btnSaveAll');
                btn.prop('disabled', true);
                btn.html('<i class="fas fa-spinner fa-spin"></i> Saving...');
                isSaving = true;
                
                var students = [];
                var hasErrors = false;
                
                // Validate all inputs first
                $('.marks-input').each(function() {
                    var studentId = $(this).data('student-id');
                    var marks = $(this).val().trim();
                    
                    // Validate each input
                    if (marks === '') {
                        showInputError($(this), 'Marks cannot be empty');
                        hasErrors = true;
                        return;
                    }
                    
                    if (isNaN(marks) || marks < 0 || marks > 100) {
                        showInputError($(this), 'Invalid marks (0-100 only)');
                        hasErrors = true;
                        return;
                    }
                    
                    students.push({
                        StudentId: parseInt(studentId),
                        Marks: parseFloat(marks)
                    });
                });
                
                if (hasErrors) {
                    btn.prop('disabled', false);
                    btn.html('<i class="fas fa-save"></i> Save All Marks');
                    isSaving = false;
                    showErrorToast("Please fix all errors before saving.");
                    return;
                }
                
                // Get all required parameters
                var examId = $('#<%= hdnExamId.ClientID %>').val();
                var subjectId = $('#<%= hdnSubjectId.ClientID %>').val();
                var teacherId = $('#<%= lblTeacherId.ClientID %>').text().trim();
                var schoolId = $('#<%= lblSchoolId.ClientID %>').text().trim();
                
                logDebug("Saving all marks parameters:", {
                    examId: examId,
                    subjectId: subjectId,
                    teacherId: teacherId,
                    schoolId: schoolId,
                    studentCount: students.length
                });

                // Prepare data
                var data = {
                    students: students,
                    examId: parseInt(examId),
                    subjectId: parseInt(subjectId),
                    teacherId: parseInt(teacherId),
                    schoolId: parseInt(schoolId)
                };
                
                // AJAX call for batch save
                $.ajax({
                    type: "POST",
                    url: "ExamMark.aspx/SaveAllGrades",
                    data: JSON.stringify(data),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    timeout: 30000, // 30 second timeout for batch operation
                    success: function(response) {
                        logDebug("Batch save response:", response);
                        
                        if (response && response.d) {
                            if (response.d.success) {
                                // Show success message
                                showSaveStatus('All marks saved successfully!', 'success');
                                
                                // Update all inputs with saved state
                                $('.marks-input').addClass('saved');
                                $('.status-cell').html('<i class="fas fa-check saved-icon"></i>');
                                
                                // Reset after 3 seconds
                                setTimeout(function() {
                                    $('.marks-input').removeClass('saved');
                                    $('.status-cell').html('');
                                    $('#saveStatus').hide();
                                    updateSaveButtonState();
                                }, 3000);
                            } else {
                                showSaveStatus(response.d.message || 'Error saving marks', 'error');
                            }
                        } else {
                            showSaveStatus('Invalid response from server', 'error');
                        }
                    },
                    error: function(xhr, status, error) {
                        logDebug("Batch save error:", { status: status, error: error });
                        showSaveStatus('Network error. Please try again.', 'error');
                    },
                    complete: function() {
                        btn.prop('disabled', false);
                        btn.html('<i class="fas fa-save"></i> Save All Marks');
                        isSaving = false;
                    }
                });
            }

            function showInputError(inputElement, message) {
                inputElement.removeClass('saving');
                inputElement.addClass('error');
                
                // Show tooltip or alert
                Swal.fire({
                    icon: 'error',
                    title: 'Validation Error',
                    text: message,
                    timer: 3000,
                    showConfirmButton: false
                });
                
                // Remove error class after 3 seconds
                setTimeout(function() {
                    inputElement.removeClass('error');
                }, 3000);
            }

            function showSaveStatus(message, type) {
                var statusDiv = $('#saveStatus');
                statusDiv.removeClass('success error').addClass(type);
                statusDiv.text(message).show();
                
                // Auto-hide after 5 seconds
                setTimeout(function() {
                    if (type === 'success') {
                        statusDiv.hide();
                    }
                }, 5000);
            }

            function updateSaveButtonState() {
                // Check if any marks have been modified
                var hasChanges = false;
                $('.marks-input').each(function() {
                    var studentId = $(this).data('student-id');
                    var currentValue = $(this).val();
                    var originalValue = marksData[studentId] || '';
                    
                    if (currentValue !== originalValue) {
                        hasChanges = true;
                        return false; // Break loop
                    }
                });
                
                $('#btnSaveAll').prop('disabled', !hasChanges);
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
        </script>
    </form>
</body>
</html>