<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResetPassword.aspx.cs" Inherits="LTG.ResetPassword" %>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta content="width=device-width, initial-scale=1.0" name="viewport">
    <title>Vivify | Reset Password</title>

    <link href="https://fonts.gstatic.com" rel="preconnect">
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600|Nunito:300,400,600|Poppins:300,400,500,600" rel="stylesheet">
    <link href="assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">
    <link href="assets/css/style.css" rel="stylesheet">

   <style>
  /* Basic reset and body styles */
html, body {
    height: 100%;  /* Ensures full height of the viewport */
    margin: 0;     /* Remove default margin */
    padding: 0;    /* Remove default padding */
    display: flex;
    flex-direction: column; /* Arrange children (main and footer) vertically */
}

/* Main content area */
main {
    flex: 1;  /* Takes up the remaining space in the body */
    display: flex;
    flex-direction: column;  /* Ensure content is laid out vertically */
}

/* Home banner area */
.home_banner_area {
    display: flex;
    justify-content: center;
    align-items: center;
    flex: 1;
    flex-direction: column;
    padding: 0;
    margin: 0;
}

/* Logo styling */
.logo_area img {
    max-height: 100px;
    width: 100%;
    max-width: 400px;
}
 body {
     background: url(assets/img/airport3.jpeg) no-repeat center center;
     background-size: cover;
 }
/* Form area styling */
.formarea {
    width: 90%;
    max-width: 400px;
    background-color: hsla(191, 97%, 37%, 0.7);
    border-radius: 10px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
    padding: 15px;
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-top: 5%;
}
.form-label{
    color:black;
    font-weight:bold;
}

/* Form container for reduced space */
.form-container {
    width: 100%;
    display: flex;
    flex-direction: column;
}

.form-container .col-12 {
    margin-bottom: 5px;
}

/* Footer styling */
.footer {
    text-align: center;
    padding: 10px;
    background: rgba(255, 255, 255, 0.8);
    width: 100%;
    margin-top: 10px;
    position: relative;
    bottom: 0; /* Ensure it stays at the bottom */
}

/* Responsive mobile styling */
@media (max-width: 576px) {
    .formarea {
        margin: 5% auto;
        padding: 12px;
        width: 95%;
    }

    .logo_area img {
        max-height: 80px;
    }

    .footer {
        font-size: 0.85rem;
    }
}

@media (max-width: 768px) {
    .formarea {
        margin-top: 20px;
    }

    .form-control {
        height: 45px;
    }
}


</style>
</head>

<body>
    <main>
        <div class="home_banner_area">
            <div class="logo_area">
                <a class="navbar-brand logo_h" href="#"><img src="assets/img/BgLogo.png" alt="Logo"></a>
            </div>

            <div class="formarea">
                <div class="form-container">
                    <h5 class="text-center pb-3" style="padding-top:4px; font-weight:bolder; color:ghostwhite">Reset Your Password</h5>

                    <form runat="server">
                        <!-- New Password -->
                        <div class="col-12">
                            <label for="txtNewPassword" class="form-label">New Password</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" class="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="reqNewPassword" ControlToValidate="txtNewPassword" ForeColor="OrangeRed" ErrorMessage="Please enter a new password" />
                        </div>

                        <!-- Confirm Password -->
                        <div class="col-12">
                            <label for="txtConfirmPassword" class="form-label">Confirm Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" class="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="reqConfirmPassword" ControlToValidate="txtConfirmPassword" ForeColor="OrangeRed" ErrorMessage="Please confirm your password" />
                        </div>

                        <!-- Hidden Field for Token -->
                        <div class="col-12">
                            <asp:HiddenField ID="hfToken" runat="server" Value="" />
                        </div>

                        <!-- Message Label -->
                        <div class="col-12">
                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                        </div>

                        <!-- Submit Button -->
                        <div class="col-12">
                            <asp:Button ID="btnResetPassword" runat="server" class="btn btn-primary w-100" OnClick="btnResetPassword_Click" Text="Reset Password" style="background-color:#3f418d;" />
                        </div>
                          <div class="col-12 text-center mt-3">
            <asp:Button 
                ID="btnBack" 
                runat="server" 
                Text="Back" 
                CssClass="btn btn-secondary" 
                OnClick="btnBack_Click" 
                style="margin-top: 20px;" />
        </div>


                    </form>
                </div>
            </div>
        </div>
    </main>

    <footer class="footer">
        <p>&copy; 2024 Vivify Technocrats. All rights reserved.</p>
    </footer>

    <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="assets/js/main.js"></script>
</body>

</html>
