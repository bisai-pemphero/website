<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeFile="Pay1.aspx.cs" Inherits="Fees" %>

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
        <div class="container py-5">
            
            <!-- Page Header -->
            <div class="page-header">
                <h2><i class="fas fa-credit-card me-2"></i> Fee Collection</h2>
                <p class="text-muted">Record and manage student payments</p>
            </div>

            <div class="row">
                <!-- Left Side: Payment Form -->
                <div class="col-lg-8 mb-4">
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

                        <!-- Form Fields -->
                        <div class="form-row">
                            <div class="form-group col-md-4">
                                <label>Amount (<asp:Label runat="server" ForeColor="Red" ID="lblShowBalance"></asp:Label>)</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-money-bill-wave"></i></div>
                                    <asp:TextBox CssClass="form-control" ID="txtAmount" runat="server"></asp:TextBox>
                                </div>
                                  <div class="error-message" id="amount-error">Please enter Amount Paying</div>
                            </div>

                            <div class="form-group col-md-4">
                                <label>Method</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-wallet"></i></div>
                                    <asp:DropDownList CssClass="form-control" ID="drpcashcheque" runat="server"></asp:DropDownList>
                                </div>
                                <div class="error-message" id="method-error">Please select Payment Method</div>
                            </div>

                            <div class="form-group col-md-4">
                                <label>Reference</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-file-invoice"></i></div>
                                    <asp:TextBox CssClass="form-control" ID="txtReference" runat="server"></asp:TextBox>
                                </div>
                                <div class="error-message" id="reference-error">Please enter Payment Reference</div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <label>Paid By</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-user"></i></div>
                                    <asp:TextBox CssClass="form-control" ID="txtbyname" runat="server"></asp:TextBox>
                                </div>
                                 <div class="error-message" id="paidby-error">Please enter paid by name </div>
                            </div>

                            <div class="form-group col-md-6">
                                <label>Date of Payment</label>
                                <div class="input-group">
                                    <div class="form-icon"><i class="fas fa-calendar-alt"></i></div>
                                    <asp:TextBox CssClass="form-control" ID="txtDate" runat="server" TextMode="Date"></asp:TextBox>
                                </div>
                                 <div class="error-message" id="date-error">Please enter payment Date</div>
                            </div>
                        </div>

                        <!-- Buttons -->
                        <div class="mt-4">
                            <asp:Button runat="server" ID="btnSave" Text="Save Transaction" CssClass="btn btn-primary me-2"  OnClientClick="return validateForm();" OnClick="btnSave_Click" />
                            <asp:Button runat="server" ID="btnBack" Text="Back" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                        </div>
                    </div>
                </div>

                <!-- Right Side: Fee Summary -->
                <div class="col-lg-4">
                    <div class="card p-3">
                        <h6 class="mb-3"><i class="fas fa-list-alt me-2"></i> Fee Summary</h6>
                        <div class="table-responsive">
                            <table id="datatablesSimple" class="table table-striped table-hover table-bordered">
                                <thead>
                                    <tr>
                                        <th>Term</th>
                                        <th>Paid</th>
                                        <th>Balance</th>
                                    </tr>
                                </thead>
                                <tbody id="feesDetails" runat="server"></tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Hidden fields -->
            <asp:Label runat="server" ID="lblSession" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblFeesId" Visible="false"></asp:Label>
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

             <asp:Label runat="server" ID="lblsmsCurrentBalance" Visible="false"></asp:Label>
           

        </div>


          <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
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


                  // Client-side validation and alert handling
                  function validateForm() {
                      // Perform client-side validation
                      let isValid = true;

                      // Reset error messages
                      document.querySelectorAll('.error-message').forEach(el => {
                          el.style.display = 'none';
                      });


          if (!document.getElementById('<%= txtAmount.ClientID %>').value.trim()) {
              document.getElementById('amount-error').style.display = 'block';
              isValid = false;
          }


          if (!document.getElementById('<%= txtbyname.ClientID %>').value) {
              document.getElementById('paidby-error').style.display = 'block';
              isValid = false;
          }


          if (!document.getElementById('<%= txtDate.ClientID %>').value.trim()) {
              document.getElementById('date-error').style.display = 'block';
              isValid = false;
          }


          if (!document.getElementById('<%= txtReference.ClientID %>').value.trim()) {
              document.getElementById('reference-error').style.display = 'block';
              isValid = false;
          }


          if (!document.getElementById('<%= drpcashcheque.ClientID %>').value.trim()) {
              document.getElementById('method-error').style.display = 'block';
              isValid = false;
          }


          document.getElementById('<%= txtAmount.ClientID %>').addEventListener('input', function () {
              document.getElementById('amount-error').style.display = 'none';
          });

          document.getElementById('<%= txtbyname.ClientID %>').addEventListener('change', function () {
              document.getElementById('paidby-error').style.display = 'none';
          });

          document.getElementById('<%= txtDate.ClientID %>').addEventListener('input', function () {
              document.getElementById('date-error').style.display = 'none';
          });

          document.getElementById('<%= txtReference.ClientID %>').addEventListener('input', function () {
              document.getElementById('reference-error').style.display = 'none';
          });

          document.getElementById('<%= drpcashcheque.ClientID %>').addEventListener('input', function () {
              document.getElementById('method-error').style.display = 'none';
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
