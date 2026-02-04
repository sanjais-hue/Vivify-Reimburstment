<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Vivify.ForgotPassword" %>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta content="width=device-width, initial-scale=1.0" name="viewport">
    <title>Vivify | Forgot Password</title>
    <meta content="" name="description">
    <meta content="" name="keywords">

    <!-- Google Fonts -->
    <link href="https://fonts.gstatic.com" rel="preconnect">
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600|Nunito:300,400,600|Poppins:300,400,500,600" rel="stylesheet">

    <!-- Vendor CSS Files -->
    <link href="assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">
    <link href="assets/vendor/bootstrap-icons/bootstrap-icons.css" rel="stylesheet">
  

    <style>
        /* Custom styles for the page */
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
            display: flex;
            flex-direction: column;
        }

        main {
            flex: 1;
            display: flex;
            flex-direction: column;
        }

        body {
            background: url(assets/img/airport3.jpeg) no-repeat center center;
            background-size: cover;
        }

        .home_banner_area {
            display: flex;
            justify-content: center;
            align-items: center;
            flex: 1;
            flex-direction: column;
            padding: 0 !important; /* Remove padding */
            
        }

        .logo_area img {
            max-height: 100px;
            width: 100%;
            max-width: 500px;
            margin-top: 10px;
        }

        .formarea {
            width: 90%;
            max-width: 400px;
            background-color: hsla(191, 97%, 37%, 0.7);
            border-radius: 10px;
            padding: 10px;
            display: flex;
            flex-direction: column;
            margin-top: 2%;
            margin-bottom: 5%;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
            color: black;
            font-weight: bold;
        }

        .footer {
            background-color: rgb(249, 243, 243); /* Footer background color */
            text-align: center; /* Center footer text */
            padding: 10px; /* Padding for footer */
            color: ghostwhite; /* Footer text color */
            margin: 0; /* Remove all margins */
        }

        .footer a {
            color: midnightblue; /* Link color in footer */
            text-decoration: none; /* Remove underline from links */
        }

        .footer a:hover {
            text-decoration: underline; /* Underline on hover */
        }

        .form-control {
            color: darkblue;
        }

        .forgot-password {
            margin-top: 10px;
            font-size: 14px;
            color: black;
            text-decoration: none;
            transition: color 0.3s;
        }

        .forgot-password:hover {
            color: ghostwhite;
            text-decoration: underline;
        }

        /* Placeholder styling */
        ::-webkit-input-placeholder {
            color: grey; /* Chrome/Opera/Safari */
        }
        ::-moz-placeholder {
            color: grey; /* Firefox 19+ */
        }
        :-ms-input-placeholder {
            color: grey; /* Internet Explorer 10-11 */
        }
        :-moz-placeholder {
            color: grey; /* Firefox < 19 */
        }

        /* Responsive Design */
        @media (max-width: 576px) {
            .formarea {
                width: 95%;
                margin: 5% auto;
            }

            .logo_area img {
                max-height: 80px;
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
                    <h5 class="text-center pb-3" style="padding-top:4px; font-weight:bolder; color:ghostwhite">Forgot Password</h5>

                    <form runat="server">
                        <!-- Email Field -->
                        <div class="col-12">
                            <label for="txtOfficialEmail" class="form-label">Official Mail </label>
                            <asp:TextBox ID="txtOfficialEmail" runat="server" CssClass="form-control" placeholder="Enter your official email"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="reqOfficialEmail" ControlToValidate="txtOfficialEmail" ErrorMessage="Official email is required" ForeColor="Red" />
                        </div>

                        <!-- New Password -->
                        <div class="col-12">
                            <label for="txtNewPassword" class="form-label">New Password</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your new password"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="reqNewPassword" ControlToValidate="txtNewPassword" ErrorMessage="Password is required" ForeColor="Red" />
                        </div>

                        <!-- Confirm Password -->
                        <div class="col-12">
                            <label for="txtConfirmPassword" class="form-label">Confirm Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirm your password"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="reqConfirmPassword" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm your password" ForeColor="Red" />
                        </div>

                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary w-100" Text="Reset Password" OnClick="btnSubmit_Click" style="background-color:#3f418d;" />

                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
                        <div class="col-12 mt-3">
                            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary w-100" Text="Back to Login" OnClick="btnBack_Click" CausesValidation="false" />
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </main>

    <footer id="footer" class="footer">
        <div class="credits" style="color:orangered; padding:0; margin:0;">
            Designed by <a href="https://www.vivifytec.in/">Vivify Technocrats</a>
        </div>
    </footer>

    <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="assets/js/main.js"></script>
</body>

</html>
