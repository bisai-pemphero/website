<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddStudent.aspx.cs" Inherits="AddStudent" %>

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
                    <i class="fa-solid fa-school"></i><a href="Dashboard.aspx">Dashboard</a> <i class="fa-solid fa-angle-right"></i><a>Add Student</a>
                </div>

                <div class="form-container">
                    <h2 class="form-header">Register New Student</h2>

                    <asp:Label runat="server" ID="lblError" CssClass="toast-error" Font-Bold="true" ForeColor="Red" Font-Size="Large"></asp:Label>

                    <div class="form-grid">
                        <h2 class="form-header">Parent</h2>
                        <div class="form-group">
                            <label class="form-label" for="FullName">Parent/Guardian Name</label>
                            <div class="input-with-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Enter Full Name"></asp:TextBox>
                            </div>
                            <div class="error-message" id="fullname-error">Please enter full name</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Gender">Gender</label>
                            <div class="input-with-icon">
                                <i class="fas fa-venus-mars"></i>
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Gender</asp:ListItem>
                                    <asp:ListItem Value="Male">Male</asp:ListItem>
                                    <asp:ListItem Value="Female">Female</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="gender-error">Please select gender</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="PhoneNumber">Phone Number</label>
                            <div class="input-with-icon">
                                <i class="fas fa-phone"></i>
                                <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" placeholder="Enter Phone Number" TextMode="Phone"></asp:TextBox>
                            </div>
                            <div class="error-message" id="phone-error">Please enter valid phone number</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="AlternatePhone">Alternate Phone (Optional)</label>
                            <div class="input-with-icon">
                                <i class="fas fa-phone-alt"></i>
                                <asp:TextBox ID="txtAlternatePhone" runat="server" CssClass="form-control" placeholder="Enter Alternate Phone" TextMode="Phone"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Email">Email Address (Optional)</label>
                            <div class="input-with-icon">
                                <i class="fas fa-envelope"></i>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email Address" TextMode="Email"></asp:TextBox>
                            </div>
                            <div class="error-message" id="email-error">Please enter valid email address</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Address">Address</label>
                            <div class="input-with-icon">
                                <i class="fas fa-map-marker-alt"></i>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter Full Address"></asp:TextBox>
                            </div>
                            <div class="error-message" id="address-error">Please enter address</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Relationship">Relationship to Student</label>
                            <div class="input-with-icon">
                                <i class="fas fa-heart"></i>
                                <asp:DropDownList ID="ddlRelationship" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Relationship</asp:ListItem>
                                    <asp:ListItem Value="Father">Father</asp:ListItem>
                                    <asp:ListItem Value="Mother">Mother</asp:ListItem>
                                    <asp:ListItem Value="Guardian">Guardian</asp:ListItem>
                                    <asp:ListItem Value="Sibling">Sibling</asp:ListItem>
                                    <asp:ListItem Value="Other">Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="relationship-error">Please select relationship</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Occupation">Occupation</label>
                            <div class="input-with-icon">
                                <i class="fas fa-briefcase"></i>
                                <asp:TextBox ID="txtOccupation" runat="server" CssClass="form-control" placeholder="Enter Occupation"></asp:TextBox>
                            </div>
                            <div class="error-message" id="occupation-error">Please enter occupation</div>
                        </div>
                    </div>
                    <div class="form-grid">
                        <!--check here-->
                        <h2 class="form-header">Student</h2>
                        <div class="form-group">
                            <label class="form-label" for="FirstName">First Name</label>
                            <div class="input-with-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter First Name"></asp:TextBox>
                            </div>
                            <div class="error-message" id="firstname-error">Please enter first name</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="MiddleName">Middle Name (Optional)</label>
                            <div class="input-with-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" placeholder="Enter Middle Name"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="LastName">Last Name</label>
                            <div class="input-with-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter Last Name"></asp:TextBox>
                            </div>
                            <div class="error-message" id="lastname-error">Please enter last name</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="Gender">Gender</label>
                            <div class="input-with-icon">
                                <i class="fas fa-venus-mars"></i>
                                <asp:DropDownList ID="ddlStudentGender" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Gender</asp:ListItem>
                                    <asp:ListItem Value="Male">Male</asp:ListItem>
                                    <asp:ListItem Value="Female">Female</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="studentgender-error">Please select gender</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="DateOfBirth">Date of Birth</label>
                            <div class="input-with-icon">
                                <i class="fas fa-calendar"></i>
                                <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" placeholder="Select Date of Birth" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="error-message" id="dob-error">Please select valid date of birth</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="AdmissionDate">Admission Date</label>
                            <div class="input-with-icon">
                                <i class="fas fa-calendar-check"></i>
                                <asp:TextBox ID="txtAdmissionDate" runat="server" CssClass="form-control" placeholder="Select Admission Date" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="error-message" id="admissiondate-error">Please select admission date</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="CurrentClassID">Current Class</label>
                            <div class="input-with-icon">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:DropDownList ID="ddlCurrentClass" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Current Class</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="error-message" id="currentclass-error">Please select current class</div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="PreviousClassID">Previous Class (Optional)</label>
                            <div class="input-with-icon">
                                <i class="fas fa-graduation-cap"></i>
                                <asp:DropDownList ID="ddlPreviousClass" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Previous Class</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="PrevSchool">Previous School (Optional)</label>
                            <div class="input-with-icon">
                                <i class="fas fa-school"></i>
                                <asp:TextBox ID="txtPrevSchool" runat="server" CssClass="form-control" placeholder="Enter Previous School Name"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label" for="SpecialNeeds">Special Needs (Optional)</label>
                            <div class="input-with-icon">
                                <i class="fas fa-wheelchair"></i>
                                <asp:TextBox ID="txtSpecialNeeds" runat="server" CssClass="form-control" placeholder="Enter any special needs" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="button-container">
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn-secondary" Text="Clear" OnClick="btnCancel_Click" />
                        <asp:Button runat="server" ID="btnSave" CssClass="btn-primary" Text="Register Student" OnClick="btnSave_Click" OnClientClick="return validateForm()" />
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

          
                if (!document.getElementById('<%= txtFullName.ClientID %>').value.trim()) {
                    document.getElementById('fullname-error').style.display = 'block';
                    isValid = false;
                }

              
                if (!document.getElementById('<%= ddlGender.ClientID %>').value) {
                    document.getElementById('gender-error').style.display = 'block';
                    isValid = false;
                }

               
                var phoneNumber = document.getElementById('<%= txtPhoneNumber.ClientID %>').value.trim();
                if (!phoneNumber) {
                    document.getElementById('phone-error').style.display = 'block';
                    isValid = false;
                } else if (!/^[\d\s\-\+\(\)]{10,15}$/.test(phoneNumber)) {
                    document.getElementById('phone-error').textContent = 'Please enter a valid phone number (10-15 digits)';
                    document.getElementById('phone-error').style.display = 'block';
                    isValid = false;
                }

              
               <%-- var email = document.getElementById('<%= txtEmail.ClientID %>').value.trim();
                if (!email) {
                    document.getElementById('email-error').style.display = 'block';
                    isValid = false;
                } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                    document.getElementById('email-error').textContent = 'Please enter a valid email address';
                    document.getElementById('email-error').style.display = 'block';
                    isValid = false;
                }--%>

                if (!document.getElementById('<%= txtAddress.ClientID %>').value.trim()) {
                    document.getElementById('address-error').style.display = 'block';
                    isValid = false;
                }

               
                if (!document.getElementById('<%= ddlRelationship.ClientID %>').value) {
                    document.getElementById('relationship-error').style.display = 'block';
                    isValid = false;
                }

              
                if (!document.getElementById('<%= txtOccupation.ClientID %>').value.trim()) {
                    document.getElementById('occupation-error').style.display = 'block';
                    isValid = false;
                }

              
                if (!document.getElementById('<%= txtFirstName.ClientID %>').value.trim()) {
                    document.getElementById('firstname-error').style.display = 'block';
                    isValid = false;
                }

               
                if (!document.getElementById('<%= txtLastName.ClientID %>').value.trim()) {
                    document.getElementById('lastname-error').style.display = 'block';
                    isValid = false;
                }

               
                if (!document.getElementById('<%= ddlStudentGender.ClientID %>').value) {
                    document.getElementById('studentgender-error').style.display = 'block';
                    isValid = false;
                }

               
                var dob = document.getElementById('<%= txtDateOfBirth.ClientID %>').value;
                if (!dob) {
                    document.getElementById('dob-error').style.display = 'block';
                    isValid = false;
                } else {
                    var birthDate = new Date(dob);
                    var today = new Date();
                    if (birthDate >= today) {
                        document.getElementById('dob-error').textContent = 'Date of birth must be in the past';
                        document.getElementById('dob-error').style.display = 'block';
                        isValid = false;
                    }
                }

                var admissionDate = document.getElementById('<%= txtAdmissionDate.ClientID %>').value;
                if (!admissionDate) {
                    document.getElementById('admissiondate-error').style.display = 'block';
                    isValid = false;
                } else {
                    var admDate = new Date(admissionDate);
                    var today = new Date();
                    if (admDate > today) {
                        document.getElementById('admissiondate-error').textContent = 'Admission date cannot be in the future';
                        document.getElementById('admissiondate-error').style.display = 'block';
                        isValid = false;
                    }
                }

             
                if (!document.getElementById('<%= ddlCurrentClass.ClientID %>').value) {
                    document.getElementById('currentclass-error').style.display = 'block';
                    isValid = false;
                }

               
                document.getElementById('<%= txtFullName.ClientID %>').addEventListener('input', function () {
                    document.getElementById('fullname-error').style.display = 'none';
                });

                document.getElementById('<%= ddlGender.ClientID %>').addEventListener('change', function () {
                    document.getElementById('gender-error').style.display = 'none';
                });

                document.getElementById('<%= txtPhoneNumber.ClientID %>').addEventListener('input', function () {
                    document.getElementById('phone-error').style.display = 'none';
                });

                document.getElementById('<%= txtEmail.ClientID %>').addEventListener('input', function () {
                    document.getElementById('email-error').style.display = 'none';
                });

                document.getElementById('<%= txtAddress.ClientID %>').addEventListener('input', function () {
                    document.getElementById('address-error').style.display = 'none';
                });

                document.getElementById('<%= ddlRelationship.ClientID %>').addEventListener('change', function () {
                    document.getElementById('relationship-error').style.display = 'none';
                });

                document.getElementById('<%= txtOccupation.ClientID %>').addEventListener('input', function () {
                    document.getElementById('occupation-error').style.display = 'none';
                });

              
                document.getElementById('<%= txtFirstName.ClientID %>').addEventListener('input', function () {
                    document.getElementById('firstname-error').style.display = 'none';
                });

                document.getElementById('<%= txtLastName.ClientID %>').addEventListener('input', function () {
                    document.getElementById('lastname-error').style.display = 'none';
                });

                document.getElementById('<%= ddlStudentGender.ClientID %>').addEventListener('change', function () {
                    document.getElementById('studentgender-error').style.display = 'none';
                });

                document.getElementById('<%= txtDateOfBirth.ClientID %>').addEventListener('change', function () {
                    document.getElementById('dob-error').style.display = 'none';
                });

                document.getElementById('<%= txtAdmissionDate.ClientID %>').addEventListener('change', function () {
                    document.getElementById('admissiondate-error').style.display = 'none';
                });

                document.getElementById('<%= ddlCurrentClass.ClientID %>').addEventListener('change', function () {
                    document.getElementById('currentclass-error').style.display = 'none';
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
