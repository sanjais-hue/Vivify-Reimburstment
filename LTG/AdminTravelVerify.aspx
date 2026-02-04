<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="AdminTravelVerify.aspx.cs" Inherits="Vivify.AdminTravelVerify" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    
    <style>
        /* === COMPACT STYLING === */
        
        /* Layout and Container Styles */
        .main-container {
            padding: 10px;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            min-height: 80vh;
            margin-left: 250px;
            width: calc(100% - 250px);
            font-size: 12px;
            margin-top:50px;
        }

        #main {
            width: 100% !important;
            padding: 0 !important;
        }
.amount-input.editing {
    border-color: #ffc107;
    background-color: #fffbf0;
    box-shadow: 0 0 5px rgba(255, 193, 7, 0.5);
}

.btn-edit.saving {
    background: #28a745;
    color: white;
}
.btn-refresh {
    background: linear-gradient(135deg, #6c757d 0%, #495057 100%);
    color: white;
}
        /* Sidebar Styles */
        .sidebar {
            background-color: #3f418d;
            padding: 10px;
            width: 250px;
            min-width: 250px;
            max-width: 250px;
            box-shadow: 0 2px 10px rgba(63, 65, 141, 0.3);
            position: fixed;
            height: 100vh;
            overflow-y: auto;
            z-index: 1000;
        }

        .sidebar-nav .nav-link {
            display: flex;
            align-items: center;
            padding: 6px 10px;
            border-radius: 4px;
            color: #222b65;
            background-color: white;
            font-size: 11px;
            margin-bottom: 4px;
            transition: all 0.3s ease;
        }

        .sidebar-nav .nav-link i {
            font-size: 12px;
            margin-right: 5px;
        }

        /* Card Styles */
        .custom-card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 4px 16px rgba(63, 65, 141, 0.2);
            border: none;
            margin-top: 8px;
            overflow: hidden;
            width: 100%;
            max-width: 1200px;
            font-size: 12px;
        }

        .card-header-custom {
            background: linear-gradient(135deg, #3f418d 0%, #2a2c6b 100%);
            color: white;
            padding: 8px 15px;
            border-bottom: none;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .card-title {
            font-size: 1rem !important;
            font-weight: 600;
            margin: 0;
            color: white !important;
        }

        /* Employee Badge */
        .employee-badge {
            background: rgba(255, 255, 255, 0.2);
            color: white;
            font-weight: bold;
            font-size: 11px;
            padding: 4px 10px;
            border-radius: 15px;
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        /* GridView Styling - COMPACT */
        .grid-container {
            max-height: 400px;
            overflow-y: auto;
            border: 1px solid #e0e0e0;
            border-radius: 6px;
            margin: 10px 0;
            width: 100%;
            font-size: 11px;
        }

        .modern-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 11px;
            background: white;
        }

        .modern-grid th {
            background: linear-gradient(135deg, #3f418d 0%, #2a2c6b 100%);
            color: white;
            padding: 6px 4px !important;
            font-weight: 600;
            text-align: center;
            position: sticky;
            top: 0;
            border: 1px solid #ddd;
            font-size: 10px;
            white-space: nowrap;
        }

        .modern-grid td {
            padding: 4px 3px !important;
            border: 1px solid #e0e0e0;
            text-align: center;
            vertical-align: middle;
            font-size: 10px;
            white-space: nowrap;
        }

        .modern-grid tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        .modern-grid tr:hover {
            background-color: #e3f2fd;
        }

        .hidden-column {
            display: none;
        }

        /* Amount Controls - COMPACT */
        .amount-control {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 4px;
            min-height: 30px;
        }

        .amount-input {
            width: 70px;
            padding: 2px 4px;
            border: 1px solid #3f418d;
            border-radius: 4px;
            text-align: center;
            font-weight: 500;
            background: #fafbff;
            font-size: 10px;
            height: 24px;
        }

        /* Checkbox Styling */
        .form-check-input {
            width: 16px;
            height: 16px;
            font-size: 10px;
        }

        .form-check-input:checked {
            background-color: #28a745;
            border-color: #28a745;
        }

        /* Button Styling - COMPACT */
        .btn-custom {
            padding: 4px 12px;
            border-radius: 4px;
            font-weight: 600;
            border: none;
            transition: all 0.3s ease;
            min-width: 80px;
            font-size: 10px;
            height: 26px;
        }

        .btn-submit {
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
            color: white;
        }

        .btn-export {
            background: linear-gradient(135deg, #17a2b8 0%, #6f42c1 100%);
            color: white;
        }

        .btn-cancel {
            background: linear-gradient(135deg, #dc3545 0%, #e83e8c 100%);
            color: white;
        }

        .btn-edit {
            background: #ffc107;
            color: #212529;
            border: none;
            padding: 2px 6px;
            border-radius: 3px;
            font-size: 9px;
            font-weight: 500;
            height: 20px;
            min-width: 40px;
        }

        /* Totals Display - COMPACT LIKE IMAGE */
        .totals-container {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 8px 12px;
            border-radius: 6px;
            margin: 12px 0;
            font-size: 11px;
        }

        .total-amount {
            font-size: 0.9rem;
            font-weight: 700;
            background: rgba(255, 255, 255, 0.2);
            padding: 4px 8px;
            border-radius: 4px;
            border: 1px solid rgba(255, 255, 255, 0.3);
            width: 100px;
            text-align: center;
        }

        .total-label {
            font-size: 10px;
            font-weight: 600;
            margin-bottom: 2px;
        }

        /* Button Container - COMPACT */
        .button-container {
            margin-top: 10px;
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 6px;
        }

        .button-container .btn {
            min-width: 90px;
            max-width: 120px;
            padding: 4px 8px !important;
            font-size: 10px;
            height: 28px;
        }

        /* Status Indicators */
        .status-badge {
            padding: 2px 6px;
            border-radius: 10px;
            font-size: 9px;
            font-weight: 600;
            text-transform: uppercase;
        }

        .status-pending {
            background: #fff3cd;
            color: #856404;
            border: 1px solid #ffeaa7;
        }

        .status-verified {
            background: #d1edff;
            color: #004085;
            border: 1px solid #b3d9ff;
        }

        /* Back Button */
        .back-btn {
            background: #6c757d;
            color: white;
            border: none;
            width: 30px;
            height: 30px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
        }

        /* Claim/Non-Claim Display - LIKE IMAGE */
        .claim-display {
            background: white;
            border: 1px solid #dee2e6;
            border-radius: 6px;
            padding: 8px;
            margin: 8px 0;
            font-size: 11px;
        }

        .claim-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 4px 0;
            border-bottom: 1px solid #f8f9fa;
        }

        .claim-item:last-child {
            border-bottom: none;
        }

        .claim-type {
            font-weight: 600;
            color: #3f418d;
            font-size: 10px;
        }

        .claim-amount {
            font-weight: 700;
            color: #28a745;
            font-size: 10px;
        }

        /* Scrollbar Styling */
        .grid-container::-webkit-scrollbar {
            width: 6px;
        }

        .grid-container::-webkit-scrollbar-track {
            background: #f1f1f1;
            border-radius: 3px;
        }

        .grid-container::-webkit-scrollbar-thumb {
            background: #3f418d;
            border-radius: 3px;
        }

        /* Responsive Design */
        @media (max-width: 1200px) {
            .main-container {
                margin-left: 250px;
                width: calc(100% - 250px);
            }
        }

        @media (max-width: 992px) {
            .main-container {
                margin-left: 0;
                width: 100%;
                padding: 8px;
            }
        }

        @media (max-width: 768px) {
            .card-header-custom {
                flex-direction: column;
                gap: 6px;
                text-align: center;
                padding: 6px 10px;
            }
            
            .amount-control {
                flex-direction: column;
                gap: 2px;
            }
            
            .amount-input {
                width: 60px;
            }
            
            .btn-custom {
                min-width: 70px;
                padding: 3px 8px;
                font-size: 9px;
            }
            
            .modern-grid {
                font-size: 9px;
            }
            
            .modern-grid th,
            .modern-grid td {
                padding: 3px 2px !important;
            }
            
            .sidebar {
                width: 200px;
            }
            
            .sidebar-nav .nav-link {
                font-size: 10px;
                padding: 4px 8px;
            }

            .employee-badge {
                font-size: 10px;
                padding: 3px 8px;
            }

            .totals-container {
                padding: 6px 8px;
            }

            .total-amount {
                font-size: 0.8rem;
                width: 80px;
            }
        }

        @media (max-width: 576px) {
            .main-container {
                padding: 5px;
            }
            
            .totals-container .row {
                flex-direction: column;
                gap: 8px;
            }
            
            .button-container {
                flex-direction: column;
                align-items: center;
                gap: 4px;
            }
            
            .btn-custom {
                width: 100%;
                max-width: 150px;
            }

            .grid-container {
                max-height: 300px;
            }
        }
    </style>

    <!-- Sidebar -->
    <aside id="sidebar" class="sidebar">
        <ul class="sidebar-nav" id="sidebar-nav">
            <li class="nav-item">
                <a class="nav-link" href="AdminPage.aspx">
                    <i class="bi bi-pc-display"></i>
                    <span>Expense Page</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="TravelExpensePage.aspx">
                    <i class="bi bi-pc-display"></i>
                    <span>Travel Expense Page</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="Employeecreation.aspx">
                    <i class="bi bi-person-circle"></i>
                    <span>Employee Creation</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="AdminCustomer_Creation.aspx">
                    <i class="bi bi-person-workspace"></i>
                    <span>Customer Creation</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="AdminService_Assign.aspx">
                    <i class="bi bi-diagram-3"></i>
                    <span>Service Assignment</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="RefreshmentVerify.aspx">
                    <i class="bi bi-file-check"></i>
                    <span>Refreshment Verify</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="Reportform.aspx">
                    <i class="bi bi-filetype-exe"></i>
                    <span>Expense Report</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="CombinedReport.aspx">
                    <i class="bi bi-folder-fill"></i>
                    <span>Combined Report</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="SmithReport.aspx">
                    <i class="bi bi-pc-display"></i>
                    <span>Smith Report</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="DocView.aspx">
                    <i class="bi bi-file-earmark-pdf-fill"></i>
                    <span>Attachment</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="AdminTraining.aspx">
                    <i class="bi bi-person-rolodex"></i>
                    <span>Training Page</span>
                </a>
            </li>
        </ul>
    </aside>

    <!-- Main Content -->
  <div class="main-container">
        <div class="container-fluid">
            <div class="row">
                <div class="col-12">
                    <div class="card custom-card">
                        <div class="card-header-custom">
                            <h5 class="card-title">
                                <i class="fas fa-file-invoice-dollar me-2"></i>
                                Admin Verification
                            </h5>
                            <div class="employee-badge">
                                <i class="fas fa-user me-2"></i>
                                <asp:Label ID="lblEmployeeName" runat="server" Text="Employee: "></asp:Label>
                            </div>
                        </div>

                        <div class="card-body p-2">
                            <!-- GridView -->
                            <div class="grid-container">
                                <asp:GridView ID="ExpenseGridView" runat="server" AutoGenerateColumns="False" 
                                    DataKeyNames="Id" CssClass="modern-grid" OnRowCommand="ExpenseGridView_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="hidden-column" HeaderStyle-CssClass="hidden-column" />
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" ItemStyle-CssClass="hidden-column" HeaderStyle-CssClass="hidden-column"/>
                                        <asp:BoundField DataField="BranchName" HeaderText="Branch" />
                                        <asp:BoundField DataField="TransportType" HeaderText="Transport Type" />
                                        <asp:BoundField DataField="TravelDate" HeaderText="Travel Date" />
                                        <asp:BoundField DataField="FromPlace" HeaderText="From Place" />
                                        <asp:BoundField DataField="ToPlace" HeaderText="To Place" />
                                        <asp:BoundField DataField="FromTime" HeaderText="From Time" />
                                        <asp:BoundField DataField="ToTime" HeaderText="To Time" />
                                        <asp:BoundField DataField="Particulars" HeaderText="Particulars" ItemStyle-Width="120px" />
                                        <asp:BoundField DataField="WBS" HeaderText="WBS" />
                                        <asp:BoundField DataField="SAP" HeaderText="SAP" />
                                        <asp:BoundField DataField="ReportingManager" HeaderText="Manager" />
                                        
                                      <asp:TemplateField HeaderText="Refreshment Amount (₹)">
    <ItemTemplate>
        <div class="amount-control">
            <asp:TextBox ID="txtRefreshAmount" runat="server"
                Text='<%# string.Format("{0:0.00}", Eval("RefreshAmnt")) %>'
                ReadOnly="true"
                CssClass="amount-input"
                data-original-value='<%# string.Format("{0:0.00}", Eval("RefreshAmnt")) %>' />
            <asp:Button ID="btnEditRefresh" runat="server"
                Text="Edit"
                CommandName="EditRefresh"
                CommandArgument='<%# Container.DataItemIndex %>'
                CssClass="btn-edit" />
        </div>
    </ItemTemplate>
    <ItemStyle Width="120px" />
</asp:TemplateField>

                                        <asp:TemplateField HeaderText="Claimable">
                                            <ItemTemplate>
                                                <div class="form-check form-switch d-flex justify-content-center">
                                                    <asp:CheckBox ID="chkConveyanceClaimable" runat="server"
                                                        CssClass="form-check-input claimable-checkbox"
                                                        Checked='<%# Eval("IsClaimable") != DBNull.Value && Convert.ToBoolean(Eval("IsClaimable")) %>'
                                                        AutoPostBack="true"
                                                        OnCheckedChanged="chkClaimable_CheckedChanged" />
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="60px" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Amount (₹)">
                                            <ItemTemplate>
                                                <div class="amount-control">
                                                    <asp:TextBox ID="txtConveyanceAmount" runat="server"
                                                        Text='<%# string.Format("{0:0.00}", Eval("Amount")) %>'
                                                        ReadOnly="true"
                                                        CssClass="amount-input"
                                                        data-original-value='<%# string.Format("{0:0.00}", Eval("Amount")) %>' />
                                                    <asp:Button ID="btnEditConveyance" runat="server"
                                                        Text="Edit"
                                                        CommandName="EditRow"
                                                        CommandArgument='<%# Container.DataItemIndex %>'
                                                        CssClass="btn-edit" />
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="120px" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="text-center py-3" style="font-size: 11px;">
                                            <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                                            <h6 class="text-muted">No travel expenses found</h6>
                                            <p class="text-muted">There are no expenses to verify for the selected criteria.</p>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>

                            <!-- TOTALS DISPLAY -->
                            <div class="claim-display">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="claim-item">
                                            <span class="claim-type">Total Claimed:</span>
                                            <span class="claim-amount">
                                                ₹ <asp:Label ID="lblDisplayClaimed" runat="server" Text="0.00"></asp:Label>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="claim-item">
                                            <span class="claim-type">Total Non-Claimed:</span>
                                            <span class="claim-amount" style="color: #dc3545;">
                                                ₹ <asp:Label ID="lblDisplayNonClaimed" runat="server" Text="0.00"></asp:Label>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Buttons -->
                            <div class="button-container">
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                                    OnClick="btnSubmit_Click" CssClass="btn btn-custom btn-submit" />
                                <asp:Button ID="Button1" runat="server" Text="Export to Excel" 
                                    OnClick="btnExportToExcel_Click" CssClass="btn btn-custom btn-export" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                                    OnClick="btncancel_Click" CssClass="btn btn-custom btn-cancel" />
                            </div>

                            <!-- Messages -->
                            <div class="text-center mt-2">
                                <asp:Label ID="lblMessage" runat="server" Visible="false" 
                                    CssClass="alert alert-info d-inline-block p-1" style="font-size: 10px;"></asp:Label>
                            </div>

                            <!-- Back Button -->
                            <div class="text-end mt-2">
                                <button id="btnBack" runat="server" onserverclick="btnBack_Click" class="back-btn">
                                    <i class="fas fa-arrow-left"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- JavaScript for Enhanced Interactivity -->
 
</asp:Content>