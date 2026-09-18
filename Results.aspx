<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Results.aspx.cs" Inherits="Results" %>

<!DOCTYPE html>
<html>
<head runat="server">

     <link rel="icon" type="image/png" href="../img/logo-2-mob.png" />
    <title>Student Portal Login</title>

    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Bootstrap -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>

<style>

body {
    background: linear-gradient(135deg, #0ea5e9, #ffffff);
;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    font-family: 'Poppins', sans-serif;
}

.login-card {
    background: #ffffff;
    padding: 40px;
    width: 420px;
    border-radius: 15px;
    box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}

.login-card h2 {
    font-weight: 700;
    margin-bottom: 10px;
}

.login-card p {
    color: #6c757d;
    margin-bottom: 25px;
}

.form-control {
    border-radius: 8px;
    height: 45px;
}

.input-error {
    border: 2px solid #e74a3b !important;
}

.btn-login {
    background: #4e73df;
    border: none;
    height: 45px;
    border-radius: 8px;
    font-weight: 600;
    color: white;
}

.btn-login:hover {
    background: #2e59d9;
}

.toast-message {
    position: fixed;
    top: 20px;
    right: 20px;
    padding: 15px 20px;
    border-radius: 6px;
    color: #fff;
    display: none;
    z-index: 9999;
}

.toast-error { background: #e74a3b; }
.toast-success { background: #1cc88a; }

@keyframes shake {
    0% { transform: translateX(0); }
    20% { transform: translateX(-10px); }
    40% { transform: translateX(10px); }
    60% { transform: translateX(-10px); }
    80% { transform: translateX(10px); }
    100% { transform: translateX(0); }
}

.shake {
    animation: shake 0.4s;
}

</style>

</head>

<body>
<form id="form1" runat="server">

<div class="login-card">

    <h2>Student Portal</h2>
    <p>Login to access your examination results</p>

    <div class="form-group">
        <asp:TextBox runat="server" ID="txtStudentId"
            CssClass="form-control"
            placeholder="Admission Number"></asp:TextBox>
    </div>

    <div class="form-group">
        <asp:TextBox runat="server" ID="txtDateofBirth"
            TextMode="Date"
            CssClass="form-control"></asp:TextBox>
    </div>

    <div class="form-check mb-3">
        <input type="checkbox" id="chkRemember" class="form-check-input" />
        <label class="form-check-label">Remember Me</label>
    </div>

    <button type="button" id="btnLogin" class="btn btn-login btn-block">
        <span id="btnText">Login</span>
        <span id="btnSpinner" class="spinner-border spinner-border-sm" style="display:none;"></span>
    </button>

</div>

<div id="toastMessage" class="toast-message"></div>

<script>

    $(document).ready(function () {

        $("#btnLogin").click(function () {

            var studentId = $("#<%= txtStudentId.ClientID %>");
        var dob = $("#<%= txtDateofBirth.ClientID %>");

        studentId.removeClass("input-error");
        dob.removeClass("input-error");

        if (studentId.val().trim() === "") {
            showError(studentId, "Admission number is required");
            return;
        }

        if (dob.val().trim() === "") {
            showError(dob, "Date of Birth is required");
            return;
        }

        $("#btnText").hide();
        $("#btnSpinner").show();

        $.ajax({
            type: "POST",
            url: "Results.aspx/LoginStudent",
            data: JSON.stringify({
                admissionNo: studentId.val(),
                dateofBirth: dob.val(),
                rememberMe: $("#chkRemember").is(":checked")
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                $("#btnText").show();
                $("#btnSpinner").hide();

                if (response.d.status === "success") {

                    showToast("Login Successful", "success");

                    setTimeout(function () {
                        window.location = "examPortal.aspx";
                    }, 1200);

                } else {

                    $(".login-card").addClass("shake");
                    setTimeout(() => $(".login-card").removeClass("shake"), 400);

                    showToast(response.d.message, "error");
                }
            }
        });

    });

});

    function showError(element, message) {
        element.addClass("input-error");
        showToast(message, "error");
    }

    function showToast(message, type) {
        var toast = $("#toastMessage");
        toast.removeClass("toast-error toast-success");
        toast.addClass(type === "success" ? "toast-success" : "toast-error");
        toast.text(message).fadeIn();

        setTimeout(() => toast.fadeOut(), 4000);
    }

</script>

</form>
</body>
</html>
