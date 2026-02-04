<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Vivify.Login" %>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta content="width=device-width, initial-scale=1.0" name="viewport">
    <title>Vivify | Login</title>
    <meta content="" name="description">
    <meta content="" name="keywords">
    <link href="assets/img/favicon.ico" rel="icon">

    <!-- Google Fonts -->
    <link href="https://fonts.gstatic.com" rel="preconnect">
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600|Nunito:300,400,600|Poppins:300,400,500,600" rel="stylesheet">

    <!-- Vendor CSS Files -->
    <link href="assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">
    <link href="assets/vendor/bootstrap-icons/bootstrap-icons.css" rel="stylesheet">

    <style>
        /* Basic reset and body styles */
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
            position: relative;
        }

        .home_banner_area {
            display: flex;
            justify-content: center;
            align-items: center;
            flex: 1;
            flex-direction: column;
            padding: 0;
            margin: 0;
            margin-bottom: 15px;
            position: relative;
        }

        .formarea {
            width: 90%;
            max-width: 400px;
            background-color: hsla(191, 97%, 37%, 0.7);
            border-radius: 10px;
            padding: 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
            color: black;
            font-weight: bold;
        }

        body {
            background: url(assets/img/airport3.jpeg) no-repeat center center;
            background-size: cover;
        }

        .footer {
            background-color: rgb(249, 243, 243);
            padding: 10px;
            text-align: center;
        }

        .footer a {
            color: midnightblue;
            text-decoration: none;
        }

        .footer a:hover {
            text-decoration: underline;
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

        /* Navbar styling */
        .navbar {
            background-color: #003366;
           
        }

        .navbar-nav .nav-link {
            color: white !important;
            font-weight: 600;
            padding: 10px 15px;
             margin-right:60px;
        }

        .navbar-nav .nav-link:hover {
            color: #ffcc00 !important;
        }

        .navbar-brand img {
            height: 40px;
        }

        /* Responsive Design */
        @media (max-width: 576px) {
            .formarea {
                width: 95%;
                margin: 5% auto;
            }

            .footer {
                font-size: 0.85rem;
            }
        }
    </style>
</head>

<body>

    <!-- Navigation Bar -->
    <nav class="navbar navbar-expand-lg">
        <div class="container">
         
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item">
                        <a class="nav-link" href="Home.aspx"><i class="bi bi-house-door"></i> Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link"href="https://vivifysoft.in/#missionvission"><i class="bi bi-bullseye"></i> Our Mission & Vision</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link"href="https://vivifysoft.in/#about"><i class="bi bi-info-circle"></i> About Us</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="https://vivifysoft.in/#products&services"><i class="bi bi-gear"></i> Products & Services</a>
                    </li>
 
                    <li class="nav-item">
                        <a class="nav-link"href="https://vivifysoft.in/#contact"><i class="bi bi-envelope"></i> Contact</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>

    <main>
        <div class="home_banner_area">
            <div class="logo_area">
                <a class="navbar-brand logo_h" href="#"><img src="assets/img/BgLogo.png" alt="Logo"></a>
            </div>
            <div class="formarea">
                <div class="form-container">
                    <h5 class="text-center pb-3" style="padding-top:4px; font-weight:bolder; color:ghostwhite">Login to Your Account</h5>

                    <form runat="server">
                        <div class="col-12 p-0 mb-0">
                            <label for="txtUsername" class="form-label">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" class="form-control" placeholder="Enter your username"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="reqName" ControlToValidate="txtUsername" ForeColor="OrangeRed" ErrorMessage="Please enter your username" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtUsername" CssClass="required-field" ErrorMessage="Invalid email format!" 
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">
                            </asp:RegularExpressionValidator>
                        </div>

                        <div class="col-12 p-0 mb-0">
                            <label for="txtPassword" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" placeholder="Enter your password"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtPassword" ForeColor="OrangeRed" ErrorMessage="Please enter your password" />
                        </div>

                        <div class="col-12 p-0 mb-0">
                            <asp:Button ID="btnLogin1" runat="server" class="btn btn-primary w-100" OnClick="btnLogin1_Click" Text="Login" style="background-color:#3f418d;" />
                        </div>

                        <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false" />

                        <div class="col-12 text-center">
                            <a href="ForgotPassword.aspx" class="forgot-password">Forgot Password?</a>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </main>

    <footer id="footer" class="footer">
        <div class="credits">
            <span style="color:darkblue; font-weight:500; font-size:18px;">&copy;</span> Designed by <a href="https://www.vivifysoft.in/">Vivify Soft</a>
        </div>
    </footer>

    <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
</body>

</html>
