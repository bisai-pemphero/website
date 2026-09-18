<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="LoginPage" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login</title>
    <!-- Fonts & icons -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" integrity="sha512-SnH5WK+bZxgPHs44uWIX+LLJAJ9/2PkPKZ5QiAj6Ta86w+fsb2TkcmfRyVX3pBnMFcV7oQPJkl9QevSCWr3W6A==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        :root {
            --bg: #f4f6f9;
            --card: #ffffff;
            --muted: #6b7280;
            --text: #111827;
            --primary: #0ea5e9;
            --secondary: #0369a1;
            --secondary-hover: #075985;
            --radius: 14px;
            --shadow: 0 8px 24px rgba(0,0,0,.06);
        }
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }
        html, body {
            height: 100%;
            font-family: 'Inter', system-ui, -apple-system, sans-serif;
            background: var(--bg);
            color: var(--text);
        }

        /* --- LOGIN WRAPPER: standard size, modest padding --- */
        .login-container {
            display: flex;
            min-height: 100vh;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        /* --- CARD: standard login dimensions, compact but comfortable --- */
        .login-card {
            background: var(--card);
            border-radius: var(--radius);
            box-shadow: var(--shadow);
            width: 100%;
            max-width: 380px;          /* reduced from 420px → standard size */
            padding: 28px 28px 32px;   /* slightly tighter padding, still airy */
            text-align: center;
        }

        .login-logo {
            margin-bottom: 6px;
        }
        .login-logo img {
            width: 80px !important;     /* controlled logo size */
            height: auto;
            display: inline-block;
        }

        .login-subtitle {
            color: var(--muted);
            margin-bottom: 22px;
            font-size: 13px;
            line-height: 1.4;
        }
        .login-subtitle .aspLabel {
            font-size: 14px;
        }

        .form-group {
            margin-bottom: 16px;
            text-align: left;
        }

        .form-label {
            display: block;
            margin-bottom: 6px;
            font-weight: 500;
            font-size: 13px;
        }

        .form-control {
            width: 100%;
            padding: 10px 14px;
            border: 1px solid #e5e7eb;
            border-radius: var(--radius);
            font-family: inherit;
            font-size: 13px;
            transition: border-color 0.2s;
        }
        .form-control:focus {
            outline: none;
            border-color: var(--primary);
        }

        .input-with-icon {
            position: relative;
        }
        .input-with-icon i.fa-user,
        .input-with-icon i.fa-lock {
            position: absolute;
            left: 14px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--muted);
            font-size: 14px;
            pointer-events: none;
        }
        .input-with-icon input {
            padding-left: 38px;
            padding-right: 38px;
        }

        /* password toggle */
        .password-toggle {
            position: absolute;
            right: 14px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--muted);
            cursor: pointer;
            background: none;
            border: none;
            padding: 0;
            font-size: 14px;
            z-index: 2;
        }
        .password-toggle:hover {
            color: var(--primary);
        }

        .remember-me {
            display: flex;
            align-items: center;
            margin-bottom: 18px;
            font-size: 13px;
        }
        .remember-me input {
            margin-right: 8px;
            width: 15px;
            height: 15px;
            accent-color: var(--primary);
        }
        .remember-me label {
            color: var(--text);
        }

        /* buttons */
        .btn {
            display: inline-block;
            width: 100%;
            padding: 10px 12px;
            border: none;
            border-radius: var(--radius);
            font-weight: 600;
            font-size: 14px;
            cursor: pointer;
            transition: all 0.2s;
            text-decoration: none;
            text-align: center;
            line-height: 1.3;
        }
        .btn-primary {
            background: var(--primary);
            color: white;
        }
        .btn-primary:hover {
            background: #0d94d4;
        }
        .btn-secondary {
            background: var(--secondary);
            color: white;
        }
        .btn-secondary:hover {
            background: var(--secondary-hover);
        }

        /* secondary button spacing */
        .btn-secondary {
            margin-top: 12px;
        }

        .login-footer {
            margin-top: 16px;
            font-size: 12px;
            color: var(--muted);
        }
        .login-footer a {
            color: var(--primary);
            text-decoration: none;
        }

        .error-message {
            color: #ef4444;
            font-size: 12px;
            margin-top: 4px;
            display: none;
        }

        /* responsive */
        @media (max-width: 480px) {
            .login-card {
                padding: 22px 18px 26px;
                max-width: 340px;
            }
            .login-logo img {
                width: 70px !important;
            }
        }

        /* utility: to match original asp label styles */
        .asp-label-large {
            font-size: 15px;
            font-weight: 700;
        }
        .error-asp {
            color: #ef4444;
            font-size: 13px;
        }

        /* ensure the form doesn't add extra width */
        form {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-card">
                <!-- Logo -->
                <div class="login-logo">
                    <img src="../img/logo 2.png" alt="Logo" style="width: 80px;" />
                </div>

                <!-- Greeting / error labels (standard size) -->
                <p class="login-subtitle">
                    <asp:Label runat="server" ID="lblGreeting" Font-Bold="true" Font-Size="Medium" CssClass="asp-label-large"></asp:Label>
                    <br /><br />
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" CssClass="error-asp"></asp:Label>
                </p>

                <!-- Username -->
                <div class="form-group">
                    <label class="form-label" for="username">Username</label>
                    <div class="input-with-icon">
                        <i class="fas fa-user"></i>
                        <asp:TextBox ID="username" runat="server" CssClass="form-control" placeholder="Username"></asp:TextBox>
                    </div>
                    <div class="error-message" id="username-error">Please enter your username</div>
                </div>

                <!-- Password -->
                <div class="form-group">
                    <label class="form-label" for="password">Password</label>
                    <div class="input-with-icon">
                        <i class="fas fa-lock"></i>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password"></asp:TextBox>
                        <button type="button" class="password-toggle" id="togglePassword" aria-label="Toggle password visibility">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                    <div class="error-message" id="password-error">Please enter your password</div>
                </div>

                <!-- Remember me -->
                <div class="remember-me">
                    <asp:CheckBox ID="rememberMe" runat="server" />
                    <label for="rememberMe">Remember me</label>
                </div>

                <!-- Login button -->
                <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-primary" OnClick="btnLogin_Click" />

                <!-- Home button (secondary) -->
                <a class="btn btn-secondary" href="../index.html">Home</a>
            </div>
        </div>
    </form>

    <script>
        // Client-side validation
        document.getElementById('<%= btnLogin.ClientID %>').addEventListener('click', function (e) {
            let isValid = true;
            const username = document.getElementById('<%= username.ClientID %>');
            const password = document.getElementById('<%= txtPassword.ClientID %>');
            const usernameError = document.getElementById('username-error');
            const passwordError = document.getElementById('password-error');

            usernameError.style.display = 'none';
            passwordError.style.display = 'none';

            if (!username.value.trim()) {
                usernameError.style.display = 'block';
                isValid = false;
            }
            if (!password.value.trim()) {
                passwordError.style.display = 'block';
                isValid = false;
            }
            if (!isValid) {
                e.preventDefault();
            }
        });

        // Icon focus color
        const inputs = document.querySelectorAll('.form-control');
        inputs.forEach(input => {
            input.addEventListener('focus', function () {
                const icon = this.parentElement.querySelector('i');
                if (icon) icon.style.color = 'var(--primary)';
            });
            input.addEventListener('blur', function () {
                const icon = this.parentElement.querySelector('i');
                if (icon) icon.style.color = 'var(--muted)';
            });
        });

        // Password toggle
        const togglePassword = document.getElementById('togglePassword');
        const passwordField = document.getElementById('<%= txtPassword.ClientID %>');

        togglePassword.addEventListener('click', function () {
            const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordField.setAttribute('type', type);

            const icon = this.querySelector('i');
            if (type === 'password') {
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
                this.setAttribute('aria-label', 'Show password');
            } else {
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
                this.setAttribute('aria-label', 'Hide password');
            }
        });
    </script>
</body>
</html>
