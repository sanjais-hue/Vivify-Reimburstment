<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="Training_Expense.aspx.cs" Inherits="Vivify.Training_Expense" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Hide sections by default
            $(".expense-section").hide();

            // Show relevant section when expense type changes
            $('#<%= ddlReimbursementType.ClientID %>').change(function () {
            var selectedValue = $(this).val();
            $('.expense-section').hide(); // Hide all sections by default
            if (selectedValue === 'Conveyance') {
                $('#conveyanceSection').show();
            } else if (selectedValue === 'Food') {
                $('#foodSection').show();
            }
        });

        // Show transport details based on selected transport type
        $('#<%= ddlTransportType.ClientID %>').change(function () {
                var selectedTransport = $(this).val();
                if (selectedTransport === 'Cab/Taxi' || selectedTransport === 'Auto') {
                    $('#transportDetails').show();
                } else {
                    $('#transportDetails').hide();
                }

                // Show Auto-specific fields if 'Auto' is selected
                if (selectedTransport === 'Auto') {
                    $('#autoDetails').show(); // Show additional fields for Auto
                } else {
                    $('#autoDetails').hide(); // Hide Auto-specific fields when other transport types are selected
                }
            });
        });

        function showAlert(message, type) {
            // Create the modal container
            var modalContainer = document.createElement('div');
            modalContainer.style.position = 'fixed';
            modalContainer.style.top = '0';
            modalContainer.style.left = '0';
            modalContainer.style.width = '100%';
            modalContainer.style.height = '100%';
            modalContainer.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
            modalContainer.style.display = 'flex';
            modalContainer.style.alignItems = 'center';
            modalContainer.style.justifyContent = 'center';
            modalContainer.style.zIndex = '1000';

            // Create the modal content
            var modalContent = document.createElement('div');
            modalContent.style.backgroundColor = '#fff';
            modalContent.style.padding = '20px';
            modalContent.style.borderRadius = '8px';
            modalContent.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.2)';
            modalContent.style.textAlign = 'center';
            modalContent.style.width = '300px';

            // Add the message
            var messageElement = document.createElement('p');
            messageElement.innerText = message;
            messageElement.style.fontSize = '16px';
            messageElement.style.marginBottom = '20px';
            modalContent.appendChild(messageElement);

            // Add the "OK" button
            var okButton = document.createElement('button');
            okButton.innerText = 'OK';
            okButton.style.padding = '10px 20px';
            okButton.style.backgroundColor = type === 'success' ? '#28a745' : '#dc3545';
            okButton.style.color = '#fff';
            okButton.style.border = 'none';
            okButton.style.borderRadius = '4px';
            okButton.style.cursor = 'pointer';
            okButton.style.fontSize = '16px';

            // Close the modal on button click
            okButton.addEventListener('click', function () {
                document.body.removeChild(modalContainer);
            });

            modalContent.appendChild(okButton);
            modalContainer.appendChild(modalContent);
            document.body.appendChild(modalContainer);
        }

        function confirmSave() {
            // Show confirmation dialog
            if (confirm('Are you sure you want to submit?')) {
                // If confirmed, trigger the postback
                return true;  // Proceed with the form submission
            }
            // If cancelled, return false to prevent form submission
            return false;
        }
        function showAlert(message, type) {
            // This can be a simple alert or you could use a custom modal/toast.
            if (type === "success") {
                alert("Success: " + message);  // You can replace this with a custom success modal or toast
            } else if (type === "error") {
                alert("Error: " + message);  // You can replace this with a custom error modal or toast
            }
        }

    </script>


    <style>
        /* Custom styles */
        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            font-weight: bold;
            color: #333;
        }

        .form-control[readonly] {
            cursor: not-allowed;
            background-color: #e9ecef;
        }

        .card-title {
            text-align: left;
            color: midnightblue;
            margin-bottom: 0;
            font-size: 1.5rem;
            font-family: 'Times New Roman', Times, serif;
            font-weight: bold;
            position: relative;
        }

        .card-title.underline::after {
            content: "";
            display: block;
            width: 100%;
            height: 2px;
            background-color: darkslategrey;
            position: absolute;
            left: 0;
            bottom: 10px;
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
        }

        .form-control {
            width: 100%;
            padding: 5px;
            margin-bottom: 15px;
            border-radius: 4px;
            border: 2px solid darkblue;
           
             
        }

        .btn-primary {
            background-color: #3f418d;
            color: white;
            border-radius: 10px;
            cursor: pointer;
        }

        .btn-primary:hover {
            background-color: #3f418d;
        }
        .label{
            font-weight:bold;
        }
       

        

        .form-container {
            padding: 20px;
        }

        .alert {
            padding: 15px;
            margin: 10px;
            border-radius: 5px;
            font-size: 16px;
            font-weight: bold;
            position: fixed;
            top: 10%;
            left: 50%;
            transform: translateX(-50%);
            z-index: 1000;
        }

       
        .footer {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    background-color: rgb(249, 243, 243); /* Footer background color */
    text-align: center; /* Center footer text */
    padding: 10px; /* Padding for footer */
    color: ghostwhite; /* Footer text color */
    z-index: 1000; /* Ensure it's above other content */
}

.footer a {
    color: midnightblue; /* Link color in footer */
    text-decoration: none; /* Remove underline from links */
}

.footer a:hover {
    text-decoration: underline; /* Underline on hover */
}
 main {
    flex-grow: 1; /* This ensures the content grows and pushes footer down */
}
 .button-container {
    display: flex;
    justify-content: center; /* Centers the buttons horizontally */
    gap: 10px;
}
  .btn-primary {
    background-color: #3f418d;
    color: white;
    border-radius: 10px;
    cursor: pointer;
}
   .btn-primary, .btnCancel {
    width: 120px; /* Set a specific width for the buttons */
    max-width: 150px; /* Optional: This limits the maximum width */
    padding: 10px; /* Adjust padding to ensure proper button sizing */
    border-radius: 10px;
    cursor: pointer;
}
    .btnCancel {
        background-color: rgb(223, 78, 78);
        color:white;
    }
    </style>

    <main id="main" class="main">
        <div class="formarea">
            <section class="section dashboard">
                <div class="row">
                    <div class="col">
                        <div class="card centered-card">
                            <h5 class="card-title" style="text-align:center;background-color:#3f418d;color:white">Training Expense</h5>
                            <section class="form-container">
                                <div class="form-group">
                                    <label class="form-label" for="ddlReimbursementType">Expense Type</label>
                                    <asp:DropDownList ID="ddlReimbursementType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                        <asp:ListItem Value="Conveyance">Conveyance</asp:ListItem>
                                        <asp:ListItem Value="Food">Food</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <!-- Conveyance Section -->
                                <div id="conveyanceSection" class="expense-section" style="display:none;">
                                    <div class="form-group">
                                        <label class="form-label" for="ddlTransportType">Transport Type</label>
                                        <asp:DropDownList ID="ddlTransportType" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="">-- Select --</asp:ListItem>
                                            <asp:ListItem Value="Cab/Taxi">Cab/Taxi</asp:ListItem>
                                            <asp:ListItem Value="Auto">Auto</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div id="transportDetails" style="display:none;">
                                        <div class="form-group">
                                            <label class="form-label" for="txtDistance">Distance</label>
                                            <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label class="form-label" for="txtAmountConveyance">Amount</label>
                                            <asp:TextBox ID="txtAmountConveyance" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label class="form-label" for="txtFromDate">From Date</label>
                                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label class="form-label" for="txtToDate">To Date</label>
                                            <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label class="form-label" for="txtFromTime">From Time</label>
                                            <asp:TextBox ID="txtFromTime" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label class="form-label" for="txtToTime">To Time</label>
                                            <asp:TextBox ID="txtToTime" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
                                        </div>

                                        <div class="form-group">
                                            <label class="form-label" for="txtParticularsConveyance">Particulars</label>
                                            <asp:TextBox ID="txtParticularsConveyance" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <!-- Food Section -->
                                <div id="foodSection" class="expense-section" style="display:none;">
                                    <div class="form-group">
                    <label  class="form-label" for="txtFromDateFood">From Date</label>
                    <asp:TextBox ID="txtFromDateFood" runat="server"  CssClass="form-control" TextMode="Date" placeholder="Choose From Date" />
                </div>

                                     <div class="form-group">
                    <label class="form-label" for="txtToDateFood">To Date</label>
                    <asp:TextBox ID="txtToDateFood" runat="server"  CssClass="form-control" TextMode="Date" placeholder="Choose To Date" />
                </div>

                                     <div class="form-group">
                    <label class="form-label" for="txtFromTimeFood">From Time</label>
                    <asp:TextBox ID="txtFromTimeFood" runat="server" CssClass="form-control" TextMode="Time" placeholder="Enter From Time" />
                </div>

                                                    <div class="form-group">
                    <label class="form-label" for="txtToTimeFood">To Time</label>
                    <asp:TextBox ID="txtToTimeFood" runat="server" CssClass="form-control" TextMode="Time"  placeholder="Enter To Time" />
                </div>




                                    <div class="form-group">
                                        <label  class="form-label" for="txtAmountFood">Amount</label>
                                        <asp:TextBox ID="txtAmountFood" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label  class="form-label" for="txtParticularsFood">Particulars</label>
                                        <asp:TextBox ID="txtParticularsFood" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Submit Button -->
                               <div class="form-group button-container">
                                   <asp:Button ID="btnSubmit" runat="server" CssClass="btn-primary" Text="Save" OnClick="btnSubmit_Click" 
            OnClientClick="return confirmSubmission();" />
                                    <asp:Button ID="btnSave" runat="server" CssClass="btn-primary" Text="Submit" OnClick="btnSave_Click" 
                OnClientClick="return confirmSave();" />
    
                
    <asp:Button ID="btnCancel" runat="server" CssClass="btnCancel" Text="Cancel" OnClientClick="clearFields();" />
</div>
                                
                            </section>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </main>
</asp:Content>