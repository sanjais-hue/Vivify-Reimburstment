<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="ServiceAssignment.aspx.cs" Inherits="Vivify.ServiceAssignment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
      
        <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />

<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

        <style>
     .ui-datepicker {
         width: 150px;
         font-size: 12px;
     }
     .ui-datepicker-header {
         background: #007bff;
         color: white;
     }
     .ui-datepicker-calendar .ui-state-active {
         background: #007bff;
         color: white;
     }
     .ui-datepicker-calendar .ui-state-disabled {
         background: #f5f5f5;
         color: #ccc;
     }
     .form-select {
         width: 100%;
         padding: 5px;
      /*   margin-bottom: 15px;*/
         border-radius: 4px;
         border: 2px solid darkblue;
     }
     .form-control {
         width: 100%;
         padding: 5px;
        /* margin-bottom: 15px;*/
         border-radius: 4px;
         border: 2px solid darkblue;
     }
     .form-label {
         color: black;
         font-weight: bold;
     }
     .form-container {
         padding: 20px;
         background-color: #f8f9fa;
         border-radius: 5px;
         box-shadow: 0 2px 10px darkblue;
         margin-bottom:10px;
     }
             .content {
    margin-left: 10px; /* Adjust margin to fit sidebar */
    padding: 20px;
    background-color: #cadcfc;
    min-height: calc(100vh - 40px); /* Ensures content fills available height minus footer */
    padding-bottom: 50px; /* To ensure content doesn't overlap footer */
    box-sizing: border-box; /* Includes padding in height calculation */
    flex: 1;
}
     .error-message {
    color: red;
    font-size: 0.9em;
    display: none;
}
     .btn-primary{
         background-color:#3f418d;margin-top:10px;
     }
     .btn-primary:hover{
    background-color:#3f418d;margin-top:10px;
}
     .main {
    background-color: #cadcfc;
}
     /* === CUSTOM SELECT2 STYLING FOR MULTI-SELECT === */

/* === CHIP CONTAINER === */
.select2-selection--multiple .select2-selection__choice {
    background: none !important;
    border: none !important;
    color: black !important;
    padding: 4px 20px 4px 4px !important; /* right padding for × */
    margin: 0 !important; /* critical: no margin */
    font-size: 14px;
    display: inline-block !important;
    position: relative !important;
    white-space: nowrap;
    line-height: 1.4;
}

/* === × ICON (top-right, hover-only) === */
.select2-selection__choice__remove {
    position: absolute !important;
    top: -4px !important;
    right: 2px !important;
    color: red !important;
    font-weight: bold;
    cursor: pointer;
    font-size: 16px !important;
    font-family: Arial, sans-serif !important;
    padding: 0 !important;
    background: none !important;
    border: none !important;
    z-index: 10;
    opacity: 0;
    transition: opacity 0.2s ease;
        margin-left:55px
}

.select2-selection__choice:hover .select2-selection__choice__remove {
    opacity: 1;
}

/* === COMMA BETWEEN ITEMS (NO SPACE) === */
.select2-selection--multiple .select2-selection__choice:not(:last-child)::after {
    content: ",";
    color: black;
    font-weight: bold;
    /* No margin, no padding — attach directly */
    margin: 0 !important;
    padding: 0 !important;
}

/* 🔥 CRITICAL: Remove default spacing from Select2 container */
.select2-selection--multiple .select2-selection__rendered {
    display: inline !important;
}
.select2-container .select2-selection--multiple {
    width: 100% !important;
    padding: 5px !important;
    border-radius: 4px !important;
    border: 2px solid darkblue !important;
    font-size: 14px !important;
    background-color: white !important;
    outline: none !important;
}

.select2-selection--multiple .select2-selection__rendered li {
    display: inline-block !important;
    margin: 0 !important;
    padding: 1 !important;
}
     @media (max-width: 768px) {
         .form-container {
             margin: 10px;
             padding: 15px;
         }
         .form-select, .form-control {
             font-size: 14px;
         }
         .form-label {
             font-size: 14px;
         }
     }
 </style>
   <main id="main" class="main"> 
   
            <div class="formarea">
                <section class="section dashboard">
                    <div class="row">
                        <div class="col">
                            <div class="card">
                                <h5 class="card-title" style="text-align:center; background-color:#3f418d; color:white">Service Assignment</h5>
                                <section class="form-container section error-404 d-flex flex-column align-items-center justify-content-center" style="box-shadow: 0 2px 10px darkblue; margin:0px;">
                                    <div class="col-12">
                                        <label for="ddlBranch" class="form-label">Branch</label>
                                        <asp:DropDownList ID="ddlBranch" runat="server" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" AutoPostBack="true" class="form-select"></asp:DropDownList>
                                    </div>
                                   <div class="col-12">
    <label for="ddlCustId" class="form-label">Customer Name</label>
    <asp:ListBox ID="ddlCustId" runat="server" 
                 CssClass="form-select select2" 
                 SelectionMode="Multiple"
                 style="border: 2px solid darkblue !important;">
    </asp:ListBox>
</div>
                                    <div class="col-12">
                                        <label for="ddlEmpId" class="form-label">Emp Name & ID</label>
                                        <asp:DropDownList ID="ddlEmpId" runat="server" class="form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-12">
                                        <label for="ddlservice" class="form-label">Service Type</label>
                                        <asp:DropDownList ID="ddlservice" runat="server" class="form-select">
                                            <asp:ListItem Text="Select" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="CM" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="PM" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Installation" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Shifting" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="Others" Value="5"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-12">
                                        <label for="txtAdvance" class="form-label" autoComplete="off">Advance</label>
                                        <asp:TextBox ID="txtAdvance" runat="server" ValidationGroup="TimeSlot" ClientIDMode="Static" class="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-12">
                                        <label for="txtFromDate" class="form-label" >From Date</label>
                                        <asp:TextBox ID="txtFromDate" runat="server" class="form-control" ClientIDMode="Static" autoComplete="off"/>
                                        <div id="errorFromDate" class="error-message">From Date cannot be in the past.</div>
                                    </div>
                                    <div class="col-12">
                                        <label for="txtToDate" class="form-label" autoComplete="off">To Date</label>
                                        <asp:TextBox ID="txtToDate" runat="server" class="form-control" ClientIDMode="Static" autoComplete="off" />
                                        <div id="errorToDate" class="error-message">To Date cannot be earlier than From Date.</div>
                                    </div>
                                    <div class="col-12">
                                        <label for="ddldepartment" class="form-label">Department</label>
                                        <asp:DropDownList ID="ddldepartment" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Sales" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Service" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Project" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Refresh" Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-12">
                                        <label for="txtremarks" class="form-label" >Remarks</label>
                                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" ClientIDMode="Static" autoComplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12">
                                        <asp:Button ID="btnService" OnClick="btnService_Click" class="btn btn-primary w-100" Text="Submit" runat="server" style="background-color: #3f418d; margin-top:10px;" />
                                    </div>
                                </section>
                            </div>
                        </div>
                    </div>
                </section>
            </div>

    </main>


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
        $("#<%= txtFromDate.ClientID %>, #<%= txtToDate.ClientID %>").change(function() {
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
    $(document).ready(function () {
        $('.select2').select2({
            placeholder: "Select Customers",
            closeOnSelect: false,
            templateSelection: function (selected) {
                return selected.map(s => s.text).join(", ");
            }
        });
    });
    $(document).ready(function () {
        $('.select2').select2({
            placeholder: "Select Customers",
            allowClear: true
        });
    });

</script>

</asp:Content>