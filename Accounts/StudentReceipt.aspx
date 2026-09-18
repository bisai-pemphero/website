<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="StudentReceipt.aspx.cs" Inherits="StudentInvoice" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>School</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="keywords" content="sms" />
    <meta name="description" content="School Management System" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap" rel="stylesheet" />

    <!-- FontAwesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" />

    <!-- DataTables CSS -->
    <link rel="stylesheet" href="https://cdn.datatables.net/1.10.21/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/1.6.2/css/buttons.dataTables.min.css" />

    <!-- SweetAlert2 -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />

    <!-- Custom CSS -->
    <link rel="stylesheet" href="../css/customCs.css" />

    <style>
        /* keep your styles as is … shortened for clarity */
        .dt-buttons {
            margin-bottom: 15px;
            display: flex;
            gap: 10px;
        }

        .dt-button.btn {
            background: #3b82f6 !important;
            color: #fff !important;
            border-radius: 6px !important;
            border: none !important;
            padding: 8px 14px !important;
            font-size: 14px !important;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            cursor: pointer;
            transition: 0.2s ease-in-out;
        }

            .dt-button.btn:hover {
                background: #2563eb !important;
                transform: translateY(-1px);
            }

        /* Card Styling */
        .card {
            border-radius: 10px;
            overflow: hidden;
        }

        /* Card header customization */
        .card-header {
            font-size: 1rem;
            font-weight: 600;
            letter-spacing: 0.5px;
            display: flex;
            align-items: center;
        }

            /* Filter Container Styling */
      .filter-container {
          display: flex;
          flex-wrap: wrap;
          gap: 20px;
          margin: 30px 0;
          padding: 20px;
          background: #f8fafc;
          border-radius: 10px;
          box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
          border: 1px solid #e2e8f0;
      }

      .filter-item {
          flex: 1;
          min-width: 200px;
      }

      .form-group {
          margin-bottom: 0;
      }

      .form-label {
          display: flex;
          align-items: center;
          gap: 8px;
          font-weight: 600;
          color: #475569;
          margin-bottom: 8px;
          font-size: 14px;
      }

      .form-control {
          width: 100%;
          padding: 10px 12px;
          border: 1px solid #cbd5e1;
          border-radius: 6px;
          font-size: 14px;
          transition: all 0.2s;
          background-color: white;
      }

          .form-control:focus {
              outline: none;
              border-color: #3b82f6;
              box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
          }

      .filter-actions {
          display: flex;
          align-items: flex-end;
          gap: 10px;
          margin-top: 24px;
      }

      .btn {
          padding: 10px 16px;
          border-radius: 6px;
          font-weight: 500;
          cursor: pointer;
          display: inline-flex;
          align-items: center;
          gap: 6px;
          transition: all 0.2s;
          border: none;
          font-size: 14px;
      }

      .btn-primary {
          background-color: #3b82f6;
          color: white;
      }

          .btn-primary:hover {
              background-color: #2563eb;
          }

      .btn-secondary {
          background-color: #64748b;
          color: white;
      }

          .btn-secondary:hover {
              background-color: #475569;
          }

      /* Responsive adjustments */
      @media (max-width: 768px) {
          .filter-container {
              flex-direction: column;
              gap: 15px;
          }
          
          .filter-item {
              min-width: 100%;
          }
          
          .filter-actions {
              width: 100%;
              justify-content: flex-start;
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
                        <a class="active" href="Dashboard.aspx"><i class="fa-solid fa-gauge"></i>Dashboard</a>
                    </div>
                  
                </nav>
            </aside>

            <!-- TOPBAR -->
            <header class="topbar">
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

            <!-- CONTENT -->
            <main class="content">
                <div class="crumbs">
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a>
                    <i class="fa-solid fa-angle-right"></i><a href="StudentInvoice.aspx">Invoice</a>
                </div>

                <div class="form-container">
                    <h2 class="form-header">REPRINT RECEIPT</h2>
                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>

                   <div class="filter-container">
     <div class="filter-item">
         <div class="form-group">
             <label for="drpClass" class="form-label">
                 <i class="fa-solid fa-users"></i>Class
             </label>
             <asp:DropDownList CssClass="form-control" ID="drpClass" runat="server">
             </asp:DropDownList>
              <div class="error-message" id="class-error">Please Select Class</div>
         </div>
     </div>

     <div class="filter-item">
         <div class="form-group">
             <label for="drpCategory" class="form-label">
                 <i class="fa-solid fa-tags"></i>Fees Category
             </label>
             <asp:DropDownList ID="drpCategory" runat="server" CssClass="form-control">
             </asp:DropDownList>
             <div class="error-message" id="category-error">Please Select Fees Category</div>
         </div>
     </div>
     
     <div class="filter-item">
         <div class="form-group">
             <label for="drpAcademicYear" class="form-label">
                 <i class="fa-solid fa-calendar-alt"></i>Academic Year
             </label>
             <asp:DropDownList CssClass="form-control" ID="drpAcademicYear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpAcademicYear_SelectedIndexChanged">
             </asp:DropDownList>
             <div class="error-message" id="academic-error">Please Select Acedemic year</div>
         </div>
     </div>

     <div class="filter-item">
         <div class="form-group">
             <label for="drpTerm" class="form-label">
                 <i class="fa-solid fa-calendar-week"></i>Term
             </label>
             <asp:DropDownList ID="drpTerm" runat="server" CssClass="form-control">
             </asp:DropDownList>
             <div class="error-message" id="term-error">Please Select Term</div>
         </div>
     </div>
     
     <div class="filter-actions">
         <asp:Button ID="btnFilter" runat="server" Text="Apply Filters" CssClass="btn btn-primary" OnClick="btnFilter_Click" OnClientClick="return validateForm()"/>
         <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click"/>
     </div>
 </div>

                    <div class="card-body">
                        <table id="studentsTable" class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>FIRSTNAME</th>
                                    <th>MIDDLE NAME</th>
                                    <th>LASTNAME</th>
                                    <th>TERM</th>
                                    <th>CLASS</th>
                                    <th>CATEGORY</th>
                                    <th>PAID</th>
                                    <th>BALANCE</th>
                                    <th></th>

                                </tr>
                            </thead>
                            <tbody id="paidFeesPlaceholder" runat="server"></tbody>
                        </table>
                    </div>
                    <asp:Label runat="server" ID="lblSchoolId" Visible="false"></asp:Label>
                </div>
            </main>
        </div>

        <!-- JS Libraries -->
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="https://cdn.datatables.net/1.10.21/js/jquery.dataTables.min.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/dataTables.buttons.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/buttons.html5.min.js"></script>
        <script src="https://cdn.datatables.net/buttons/1.6.2/js/buttons.print.min.js"></script>

        <!-- SweetAlert -->
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

        <!-- DataTable Init -->
        <script>
            $(document).ready(function () {
                $('#studentsTable').DataTable({
                    
                   
                    paging: true,
                    lengthChange: true,
                    searching: true,
                    ordering: true,
                    info: true,
                    autoWidth: false,
                    responsive: true
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


                if (!document.getElementById('<%= drpAcademicYear.ClientID %>').value.trim()) {
                    document.getElementById('academic-error').style.display = 'block';
                    isValid = false;
                }


                if (!document.getElementById('<%= drpCategory.ClientID %>').value) {
                    document.getElementById('category-error').style.display = 'block';
                    isValid = false;
                }




                if (!document.getElementById('<%= drpClass.ClientID %>').value.trim()) {
                    document.getElementById('class-error').style.display = 'block';
                    isValid = false;
                }


                if (!document.getElementById('<%= drpTerm.ClientID %>').value) {
        document.getElementById('term-error').style.display = 'block';
        isValid = false;
    }

    document.getElementById('<%= drpAcademicYear.ClientID %>').addEventListener('input', function () {
        document.getElementById('academic-error').style.display = 'none';
    });

    document.getElementById('<%= drpCategory.ClientID %>').addEventListener('change', function () {
        document.getElementById('category-error').style.display = 'none';
    });

    document.getElementById('<%= drpClass.ClientID %>').addEventListener('input', function () {
        document.getElementById('class-error').style.display = 'none';
    });

                document.getElementById('<%= drpTerm.ClientID %>').addEventListener('input', function () {
                    document.getElementById('term-error').style.display = 'none';
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
        </script>
    </form>
</body>
</html>
