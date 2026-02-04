<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="LTG.ForgetPassword" %>

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
    <link href="assets/css/style.css" rel="stylesheet">

    <style>
        /* Custom styles for the page */
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
            overflow-y: auto;
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
            padding: 0;
            margin: 0;
        }

        .logo_area img {
            max-height: 100px;
            width: 100%;
            max-width: 500px;
            margin-top:10px;
        }

        .formarea {
            width: 90%;
            max-width: 400px;
            background-color: hsla(191, 97%, 37%, 0.7); /* Semi-transparent dark background */
            border-radius: 10px;
            padding: 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            margin-top: 5%;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
            color:black;
            font-weight:bold;
        }

        .footer {
            text-align: center;
            padding: 10px;
            background: rgba(255, 255, 255, 0.8);
            position: relative;
            bottom: 0;
            width: 100%;
          
        }
        .form-control{
            color:darkblue;
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

            .footer {
                font-size: 0.85rem;
            }
        }

    </style>
</head>

<body>
    <div class="home_banner_area">
        <div class="logo_area">
            <a class="navbar-brand logo_h" href="#"><img src="assets/img/BgLogo.png" alt="Logo"></a>
        </div>

        <div class="formarea">
            <form  runat="server">
                <h5 class="text-center " style="font-weight: bolder ; color:ghostwhite;font-weight:bold;">Reset Password?</h5>
                
                <!-- Email Input -->
                <div class="col-12">
                    <label for="txtEmail" class="form-label ">Email Address</label>
                    <asp:TextBox ID="txtEmail" runat="server" class="form-control" placeholder="Enter your email"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="reqEmail" ControlToValidate="txtEmail" ForeColor="OrangeRed" ErrorMessage="Please enter your email" />
                    <asp:RegularExpressionValidator 
                        runat="server" 
                        ControlToValidate="txtEmail" 
                        CssClass="required-field" 
                        ErrorMessage="Invalid email format!" 
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">
                    </asp:RegularExpressionValidator>
                </div>

                <!-- Submit Button -->
                <div class="col-12">
                    <asp:Button ID="btnSubmitEmail" runat="server" class="btn btn-primary w-100" OnClick="btnSubmitEmail_Click" Text="Send Reset Link" style="background-color:#035888;border:solid #035888;"/>
                </div>

                <!-- Message Display -->
                <div class="col-12">
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
                </div>
            </form>
        </div>
    </div>

    <footer class="footer">
        <p>&copy; 2024 Vivify Technocrats. All rights reserved.</p>
    </footer>

    <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="assets/js/main.js"></script>
</body>

</html>
