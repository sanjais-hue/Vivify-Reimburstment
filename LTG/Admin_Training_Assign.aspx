<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Admin_Training_Assign.aspx.cs" Inherits="Vivify.Admin_Training_Assign" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <style>
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

        .btn {
        background-color: #3f418d; /* Set button color */
        color: white; /* Button text color */
        border: none;
        padding: 10px 20px;
        border-radius: 4px;
        cursor: pointer;
    }

    .btn:disabled {
        background-color: #3f418d; /* Maintain the same color when disabled */
        color: white; /* Maintain text color */
        cursor: not-allowed; /* Change cursor to indicate disabled */
        opacity: 1; /* Prevent faded appearance */
    }
        .error-message {
            color: red;
            font-size: 14px;
        }
    </style>

        <aside id="sidebar" class="sidebar" style="box-shadow: 0 2px 10px darkblue;">

   <ul class="sidebar-nav" id="sidebar-nav">
      <%-- <li class="nav-item">
    <a class="nav-link" href="Dashboard.aspx">
        <i class="bi bi-grid"></i>
        <span>Dashboard</span>
    </a>
</li>--%>
            
<%--              <li class="nav-item">
    <a class="nav-link" href="AdminPage.aspx">
       <i class="bi bi-pc-display"></i>
        <span>Expense Page</span>
    </a>
</li>
                    
  
          <li class="nav-item">
  <a class="nav-link " href="Employeecreation.aspx">
      <i class="bi bi-personbi bi-person-circle"></i><span>Employee Creation</span>
  </a>
</li>
                   
        
      <li class="nav-item">
    
            <a class="nav-link " href="AdminCustomer_Creation.aspx">
               <i class="bi bi-person-workspace"></i><span>Customer Creation</span>
            </a>
          </li>

                          <li class="nav-item">
  <a class="nav-link " href="AdminService_Assign.aspx">
      <i class="bi bi-diagram-3"></i><span>Service Assignment</span>
  </a>
</li>
       
                                 <li class="nav-item">
  <a class="nav-link " href="Reportform.aspx">
      <i class="bi bi-filetype-exe"></i><span>Expense Report</span>
  </a>

</li>

                                 <li class="nav-item">
  <a class="nav-link " href="CombinedReport.aspx">
      <i class="bi bi-folder-fill"></i><span>Combined Report</span>
  </a>

</li>
       
                <li class="nav-item">
  <a class="nav-link " href="DocView.aspx">
      <i class="bi bi-file-earmark-pdf-fill"></i><span> Attachment</span>
  </a>

</li>--%>
                            <li class="nav-item">
    <a class="nav-link" href="AdminTraining.aspx">
        <i class="bi bi-person-rolodex"></i>
        <span>Training Page</span>
    </a>
</li>
              <li class="nav-item">
    <a class="nav-link" href="Admin_Training_Assign.aspx">
        <i class="bi bi-person-plus-fill"></i>
        <span>Training Assignment</span>
    </a>
</li>     
                     <li class="nav-item">
    <a class="nav-link" href="Training_Report.aspx">
        <i class="bi bi-file-spreadsheet"></i>
        <span>Training Report</span>
    </a>
</li>     
   </ul>
        </aside>


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

                                <section class="form-container">
                                    <!-- Branch Dropdown -->
                                    <div class="form-group">
                                        <label for="ddlBranch">Branch</label>
                                        <asp:DropDownList 
                                            ID="ddlBranch" 
                                            runat="server" 
                                            CssClass="form-control" 
                                            OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" 
                                            AutoPostBack="true">
                                            <asp:ListItem Text="Select Branch" Value="" />
                                        </asp:DropDownList>
                                    </div>

                                    <!-- Employee Dropdown -->
                                    <div class="form-group">
                                        <label for="ddlEmployee">Employee Name</label>
                                        <asp:DropDownList 
                                            ID="ddlEmployee" 
                                            runat="server" 
                                            CssClass="form-control">
                                            <asp:ListItem Text="Select Employee" Value="" />
                                        </asp:DropDownList>
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
                                    <div id="errorFromDate" class="error-message" style="display:none;"></div>

                                    <!-- To Date -->
                                    <div class="form-group">
                                        <label for="txtToDate">To Date</label>
                                        <asp:TextBox 
                                            ID="txtToDate" 
                                            runat="server" 
                                            CssClass="form-control" 
                                            TextMode="Date">
                                        </asp:TextBox>
                                    </div>
                                    <div id="errorToDate" class="error-message" style="display:none;"></div>

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
                                            CssClass="btn" 
                                            Text="Submit" 
                                            OnClick="btnSubmit_Click" />
                                    </div>

                                    <!-- Error Message -->
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
                                </section>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </main>
    </body>

   <script>
       $(document).ready(function () {
           // Initialize datepickers
           $("#<%= txtFromDate.ClientID %>").datepicker({ dateFormat: "yy-mm-dd" });
        $("#<%= txtToDate.ClientID %>").datepicker({ dateFormat: "yy-mm-dd" });

        // Handle change event for From and To Date fields
        $("#<%= txtFromDate.ClientID %>, #<%= txtToDate.ClientID %>").change(function () {
            var fromDate = $("#<%= txtFromDate.ClientID %>").val();
            var toDate = $("#<%= txtToDate.ClientID %>").val();

            // Clear errors initially
            $("#errorFromDate").hide();
            $("#errorToDate").hide();

            // Validate From Date
            if (!fromDate) {
                $("#errorFromDate").text("Please enter a From Date.").show();
                $("#btnSubmit").prop("disabled", true); // Disable submit button
            } else {
                $("#errorFromDate").hide();
            }

            // Validate To Date
            if (fromDate && toDate && new Date(toDate) < new Date(fromDate)) {
                $("#errorToDate").text("To Date cannot be earlier than From Date.").show();
                $("#btnSubmit").prop("disabled", true); // Disable submit button
            } else {
                $("#errorToDate").hide();
                if (fromDate && toDate) {
                    $("#btnSubmit").prop("disabled", false); // Enable submit button
                }
            }
        });
    });

    // Function to clear all fields and reset form after submission
    function clearFormAndReenableSubmitButton() {
        // Clear all input fields
        $("#<%= txtFromDate.ClientID %>").val(""); // Clear From Date field
        $("#<%= txtToDate.ClientID %>").val(""); // Clear To Date field
        $("#<%= txtTrainingDetails.ClientID %>").val(""); // Clear Training Details field
        $("#<%= ddlBranch.ClientID %>").val(""); // Clear Branch dropdown
        $("#<%= ddlEmployee.ClientID %>").val(""); // Clear Employee dropdown

           // Hide any error messages
           $("#errorFromDate").hide();
           $("#errorToDate").hide();

           // Re-enable the submit button
           $("#btnSubmit").prop("disabled", false);  // Re-enable the submit button
       }
   </script>

</asp:Content>
