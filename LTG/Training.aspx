<!-- ASP.NET Page -->
<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="Training.aspx.cs" Inherits="Vivify.Training" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(document).ready(function () {
            // Handle reimbursement type selection
            $('#<%= ddlReimbursementType.ClientID %>').change(function () {
                var selectedValue = $(this).val();
                $('.expense-section').hide(); // Hide all sections
                $('#transportDetails').hide(); // Hide transport details initially

                if (selectedValue === 'Conveyance') {
                    $('#conveyanceSection').show();  // Show Conveyance section
                } else if (selectedValue === 'Food') {
                    $('#foodSection').show();  // Show Food section
                }
            });

            // Handle transport type selection
            $('#<%= ddlTransportType.ClientID %>').change(function () {
                var transportType = $(this).val();
                if (transportType !== "") {
                    $('#transportDetails').show();  // Show transport details
                } else {
                    $('#transportDetails').hide();  // Hide transport details
                }
            });

            // Handle distance input for conveyance
            const ratePerKilometer = 13.5; // Rate per kilometer
            $('#<%= txtDistance.ClientID %>').on('input', function () {
                const distance = parseFloat($(this).val());
                if (!isNaN(distance) && distance > 0) {
                    const amount = (distance * ratePerKilometer).toFixed(2);
                    $('#<%= txtAmountConveyance.ClientID %>').val(amount);
                } else {
                    $('#<%= txtAmountConveyance.ClientID %>').val('');
                }
            });
        });
    </script>

    <style>
        .form-group { margin-bottom: 20px; }
        .form-control, .form-select {
            width: 100%;
            padding: 5px;
            border-radius: 4px;
            border: 2px solid darkblue;
        }
        .form-label { font-weight: bold; color: black; }
        .form-container {
            padding: 20px;
            background-color: #f8f9fa;
            border-radius: 5px;
            box-shadow: 0 2px 10px darkblue;
        }
        .form-control[readonly] { background-color: #e9ecef; }
    </style>

    <main id="main" class="main">
        <div class="formarea">
            <section class="section dashboard">
                <div class="row">
                    <div class="col">
                        <div class="card">
                            <h5 class="card-title" style="text-align:center; background-color:#3f418d; color:white">Training Reimbursement</h5>
                            <section class="form-container">
                                <!-- Employee Name -->
                                <div class="form-group">
                                    <label for="txtEmployeeName">Employee Name</label>
                                    <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Employee name will display here"></asp:TextBox>
                                </div>

                                <!-- Training Details -->
                                <div class="form-group">
                                    <label for="txtTrainingDetails">Training Details</label>
                                    <asp:TextBox ID="txtTrainingDetails" runat="server" CssClass="form-control" placeholder="Enter training details"></asp:TextBox>
                                </div>

                                <!-- Reimbursement Type -->
                                <div class="form-group">
                                    <label for="ddlReimbursementType">Reimbursement Type</label>
                                    <asp:DropDownList ID="ddlReimbursementType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                        <asp:ListItem Value="Conveyance">Conveyance</asp:ListItem>
                                        <asp:ListItem Value="Food">Food</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <!-- Conveyance Section -->
                                <div id="conveyanceSection" class="expense-section" style="display:none;">
                                    <div class="form-group">
                                        <label for="ddlTransportType">Transport Type</label>
                                        <asp:DropDownList ID="ddlTransportType" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="">-- Select --</asp:ListItem>
                                            <asp:ListItem Value="Cab/Taxi">Cab/Taxi</asp:ListItem>
                                            <asp:ListItem Value="Auto">Auto</asp:ListItem>
                                            <asp:ListItem Value="Other">Other</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div id="transportDetails" style="display:none;">
                                        <div class="form-group">
                                            <label for="txtFromDateConveyance">From Date</label>
                                            <asp:TextBox ID="txtFromDateConveyance" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label for="txtToDateConveyance">To Date</label>
                                            <asp:TextBox ID="txtToDateConveyance" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label for="txtDistance">Distance</label>
                                            <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label for="txtAmountConveyance">Amount</label>
                                            <asp:TextBox ID="txtAmountConveyance" runat="server" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <!-- Food Section -->
                                <div id="foodSection" class="expense-section" style="display:none;">
                                    <div class="form-group">
                                        <label for="txtFromDateFood">From Date</label>
                                        <asp:TextBox ID="txtFromDateFood" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtToDateFood">To Date</label>
                                        <asp:TextBox ID="txtToDateFood" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtAmountFood">Amount</label>
                                        <asp:TextBox ID="txtAmountFood" runat="server" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Submit Button -->
                                <div class="form-group">
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </main>
</asp:Content>
