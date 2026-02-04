<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="TrainingAssign.aspx.cs" Inherits="Vivify.TrainingAssign" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(function () {
            // Initialize date pickers
            $("#<%= txtFromDate.ClientID %>").datepicker({
                dateFormat: "yy-mm-dd"
            });
            $("#<%= txtToDate.ClientID %>").datepicker({
                dateFormat: "yy-mm-dd"
            });

            // Function to get today's date in yyyy-mm-dd format
            function getTodayDate() {
                var today = new Date();
                var day = ("0" + today.getDate()).slice(-2);
                var month = ("0" + (today.getMonth() + 1)).slice(-2);
                var year = today.getFullYear();
                return year + "-" + month + "-" + day;
            }

            // Validate dates
            $("#<%= txtFromDate.ClientID %>, #<%= txtToDate.ClientID %>").change(function () {
                var fromDate = $("#<%= txtFromDate.ClientID %>").val();
            var toDate = $("#<%= txtToDate.ClientID %>").val();

            // Clear error messages initially
            $("#errorFromDate").hide();
            $("#errorToDate").hide();

            // Validate From Date - just check if it's entered
            if (!fromDate) {
                $("#errorFromDate").text("Please enter a From Date.").show();
                $("#btnService").prop("disabled", true);  // Disable submit button
            } else {
                $("#btnService").prop("disabled", false); // Enable submit button
            }


            if (fromDate && toDate && toDate < fromDate) {
                $("#errorToDate").text("To Date cannot be earlier than From Date.").show();
                $("#btnService").prop("disabled", true);  // Disable submit button
            } else {
                $("#btnService").prop("disabled", false); // Enable submit button
            }
        });
        });
</script>

    <style>
           .main {
     
       background-color: #cadcfc;
      
   }
        .form-group {
            margin-bottom: 20px;
        }

        .form-control {
            width: 100%;
            padding: 5px;
            border-radius: 4px;
            border: 2px solid darkblue;
        }

        .form-label {
            font-weight: bold;
            color: black;
        }

        .form-container {
            padding: 20px;
            background-color: #f8f9fa;
            border-radius: 5px;
            box-shadow: 0 2px 10px darkblue;
        }

        .form-control[readonly] {
            background-color: #e9ecef;
        }

        .error-message {
            color: red;
            font-size: 14px;
        }
    </style>

    <body>
        <main id="main" class="main">
            <div class="formarea">
                <section class="section dashboard">
                    <div class="row">
                        <div class="col">
                            <div class="card">
                                <h5 class="card-title" style="text-align:center; background-color:#3f418d; color:white">
                                    Training Assignment
                                </h5>
                                <asp:HiddenField ID="hfBranchId" runat="server" />
<asp:HiddenField ID="hfEmployeeId" runat="server" />

                                <section class="form-container">
                                    <!-- Employee Name -->
                                    <div class="form-group">
                                        <label for="txtEmployeeName">Employee Name</label>
                                        <asp:TextBox 
                                            ID="txtEmployeeName" 
                                            runat="server" 
                                            CssClass="form-control" 
                                            ReadOnly="true" 
                                            placeholder="Employee name will display here">
                                        </asp:TextBox>
                                    </div>

                                    <!-- From Date -->
                                    <div class="form-group">
                                        <label for="txtFromDate">From Date</label>
                                        <asp:TextBox 
                                            ID="txtFromDate" 
                                            runat="server" 
                                            CssClass="form-control" 
                                            TextMode="Date">
                                        </asp:TextBox>
                                    </div>

                                    <!-- To Date -->
                                    <div class="form-group">
                                        <label for="txtToDate">To Date</label>
                                        <asp:TextBox 
    ID="txtToDate" 
    runat="server" 
    CssClass="form-control" 
    TextMode="Date" 
    AutoPostBack="true" 
    OnTextChanged="txtToDate_TextChanged">
</asp:TextBox>
   <asp:Label ID="errorToDate" runat="server" ForeColor="Red" Visible="false" />
                                    </div>

                                    <!-- Training Details -->
                                    <div class="form-group">
                                        <label for="txtTrainingDetails">Training Details</label>
                                        <asp:TextBox 
                                            ID="txtTrainingDetails" 
                                            runat="server" 
                                            CssClass="form-control" 
                                            placeholder="Enter training details">
                                        </asp:TextBox>
                                    </div>

                                    <!-- Submit Button -->
                                    <div class="form-group">
                                        <asp:Button 
                                            ID="btnSubmit" 
                                            runat="server" 
                                            CssClass="btn btn-primary" 
                                            Text="Submit" 
                                            OnClick="btnSubmit_Click" />
                                    </div>

                                    <!-- Error Message -->
                                    <asp:Label ID="errorMessage" runat="server" ForeColor="Red" />

                                </section>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </main>
    </body>
</asp:Content>