<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="AdminVerify.aspx.cs" Inherits="Vivify.AdminVerify" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <main id="main" class="main container d-flex flex-column align-items-center">

        <style>
            /* === COMPACT STYLING ADJUSTMENTS === */
            .main.container {
                padding: 5px 5px !important;
            }

            .custom-card {
                width: 100%;
                max-width: 1200px;
                margin-top: 10px;
                padding: 8px;
            }


           .card-header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        background-color: #3f418d;
        color: white;
        padding: 12px 20px;
        border-radius: 8px 8px 0 0;
        width: 100%;
        box-sizing: border-box;
    }

   .card-title {
                padding: 8px 0 !important;
                font-size: 1.2rem !important;
                background-color: #3f418d;
                color: ghostwhite;
                text-align: center;
                margin-bottom: 0;
                margin-left:100px;
            }
    .employee-display-right {
        margin-left: auto;
        flex-shrink: 0;
        text-align: right;
    }

    .employee-name-badge {
        background-color: rgba(255, 255, 255, 0.2);
        color: white;
        font-weight: bold;
        font-size: 14px;
        padding: 8px 16px;
        border-radius: 20px;
        border: 1px solid rgba(255, 255, 255, 0.3);
        white-space: nowrap;
    }

            .scrollable-container {
                width: 95%;
                overflow-x: auto;
            }

            .table-responsive {
                display: block;
                max-width: 100%;
                overflow-x: auto;
                white-space: nowrap;
            }

            .mydatagrid {
                width: 100%;
                border-collapse: collapse;
                font-size: 0.85rem;
            }

            .mydatagrid th,
            .mydatagrid td {
                padding: 6px 4px !important;
                border: 1px solid black;
                font-size: 0.85rem;
            }

            .mydatagrid th {
                background-color: #3f418d;
                color: white;
                text-align: center;
                position: sticky;
                top: 0;
            }

            .amount-container {
                display: flex;
                align-items: center;
                margin-bottom: 6px;
            }

            .form-control {
                width: 80px;
                padding: 3px !important;
                border: 2px solid darkblue;
                border-radius: 4px;
                text-align: center;
                font-size: 0.8rem;
            }
            .main.container {
    padding: 5px !important; /* already set, but reinforced */
    max-width: none !important; /* override Bootstrap container fixed width */
    margin-left: 0 !important;
    margin-right: 0 !important;
}

/* Ensure full-width alignment without extra offsets */
#main {
    width: 100% !important;
    padding-left: 200px !important;
    padding-right: 0 !important;
}

/* Reduce space between sidebar and main content */
.main {
    width: calc(100% - 240px) !important; /* Adjust if sidebar is ~240px wide */
    padding-left: 8px !important; /* minimal gap from sidebar */
}

/* Optional: Reduce overall body/content wrapper if needed */
.main-content {
    padding-left: 0 !important;
    padding-right: 0 !important;
}

            .form-label {
                color: black;
                font-weight: bold;
            }

            .form-container {
                padding: 15px;
                background-color: #f8f9fa;
                border-radius: 5px;
                box-shadow: 0 2px 10px darkblue;
            }

            .sidebar {
                background-color: #3f418d;
                padding: 15px;
                box-shadow: 0 2px 10px darkblue;
            }

            .sidebar-nav .nav-link {
                display: flex;
                align-items: center;
                padding: 10px 15px;
                border-radius: 5px;
                color: #222b65;
                background-color: white;
                font-size: 0.9rem;
            }

            .sidebar-nav .nav-link.active {
                background-color: #222b65;
                color: white;
            }

            .sidebar-nav .nav-item {
                margin-bottom: 8px;
            }

            /* Smaller Edit button */
            .btn.btn-secondary {
                font-size: 0.75rem !important;
                padding: 2px 6px !important;
                min-width: auto !important;
                height: auto !important;
            }

            /* Button container spacing */
            .button-container {
                margin-top: 10px;
                display: flex;
                flex-wrap: wrap;
                justify-content: center;
                gap: 10px;
            }

            .button-container .btn {
                min-width: 100px;
                max-width: 160px;
                padding: 4px 8px !important;
                font-size: 0.85rem;
                text-align: center;
            }

            .amt-container {
                display: flex;
                flex-wrap: wrap;
                justify-content: center;
                align-items: center;
                gap: 10px;
                margin-top: 10px;
            }

            .total-amount {
                font-weight: bold;
                text-align: center;
                width: 100px;
                font-size: 0.9rem;
            }

            .hidden-column {
                display: none;
            }

            .containerback {
                margin-top: 10px;
                text-align: right;
                width: 100%;
            }

            .back-button {
                background: none;
                border: none;
                color: #3f418d;
                font-size: 1.1rem;
                cursor: pointer;
            }
                        .toggle-sidebar-btn {
     font-size: 40px;
     color:midnightblue;
 }
.sidebar {
     background-color:#3f418d;
     padding: 15px;
     width: 250px; /* Decreased sidebar width */
     min-width: 250px; /* Set minimum width */
     max-width: 250px; /* Set maximum width */
}

.sidebar-nav .nav-link {
    display: flex;
    align-items: center;
    padding: 8px 12px; /* Further reduced padding for smaller width */
    border-radius: 5px;
    color: #222b65;
    background-color:white;
    font-size: 12px; /* Slightly smaller font */
}

.sidebar-nav .nav-link i {
    font-size: 14px; /* Smaller icons */
    margin-right: 6px; /* Reduced spacing */
}

.sidebar-nav .nav-link span {
    white-space: nowrap; /* Prevent text wrapping */
    overflow: hidden;
    text-overflow: ellipsis; /* Add ellipsis if text is too long */
}

 .sidebar-nav .nav-link:hover {
        color: red;
        transition: background 0.3s, color 0.3s, border 0.3s; 
        border : 2px solid #222b65;
        box-shadow: 0 2px 10px #1f2b60;
    }

.sidebar-nav .nav-link.active {
    background-color: #222b65;
    color: white;
}

.sidebar-nav .nav-item {
    margin-bottom: 6px; /* Further reduced space */
}

.sidebar-nav {
    padding: 0;
    margin: 0;
    list-style: none;
}
                    .footer {
        background-color:rgb(249, 243, 243);
        text-align: center;
        padding:10px;
        color: ghostwhite;
        margin:0px;
    }
    
    .footer a {
        color: midnightblue;
        text-decoration: none;
    }
    
    .footer a:hover {
        text-decoration: underline;
    }

    .btn-primary{
        background-color:#3f418d;
    }
     .btn-primary:hover{
     background-color:#3f418d;
 }

/* Adjust main content area to accommodate smaller sidebar */
.main {
    margin-left: 200px; /* Match sidebar width */
}

/* For mobile responsiveness */
@media (max-width: 768px) {
    .sidebar {
        width: 180px; /* Even smaller on mobile */
        min-width: 180px;
        max-width: 180px;
    }
    
    .main {
        margin-left: 180px;
    }
    
    .sidebar-nav .nav-link {
        font-size: 11px;
        padding: 6px 10px;
    }
    
    .sidebar-nav .nav-link i {
        font-size: 13px;
    }
  /* Header bar with title and employee name */
.card-header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        background-color: #3f418d;
        color: white;
        padding: 12px 20px;
        border-radius: 8px 8px 0 0;
    }

    .card-title {
        margin: 0;
        font-size: 1.2rem !important;
        color: white !important;
    }

    .employee-display-right {
        margin-left: auto;
    }

    .employee-name-badge {
        background-color: rgba(255, 255, 255, 0.2);
        color: white;
        font-weight: bold;
        font-size: 14px;
        padding: 8px 16px;
        border-radius: 20px;
        border: 1px solid rgba(255, 255, 255, 0.3);
        white-space: nowrap;
    }

/* Title left-aligned */


}
        </style>

        <aside id="sidebar" class="sidebar">
            <ul class="sidebar-nav" id="sidebar-nav">
                <li class="nav-item">
                    <a class="nav-link" href="AdminPage.aspx">
                        <i class="bi bi-pc-display"></i>
                        <span>Expense Page</span>
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
                        <i class="bi bi-diagram-3"></i>
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
                    <a class="nav-link" href="DocView.aspx">
                        <i class="bi bi-file-earmark-pdf-fill"></i>
                        <span>Attachment</span>
                    </a>
                </li>
            </ul>
        </aside>
<div class="card custom-card">
    <div class="card-header-container">
        <h5 class="card-title">Admin Verification</h5>
        <div class="employee-display-right">
            <asp:Label ID="lblEmployeeName" runat="server" CssClass="employee-name-badge" Text="Employee: "></asp:Label>
        </div>
    </div>
</div>

<!-- ✅ Grid section OUTSIDE the card -->
<section class="section error-404 d-flex flex-column align-items-center justify-content-center" 
         style="box-shadow: 0 2px 10px #1f2b60;">

                <div class="scrollable-container">
                    <asp:GridView ID="ExpenseGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="ExpenseId" CssClass="mydatagrid" OnRowCommand="ExpenseGridView_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="ExpenseId" HeaderText="Id" ItemStyle-CssClass="hidden-column" HeaderStyle-CssClass="hidden-column" />
                            <asp:BoundField DataField="SourceTable" HeaderText="Source" />
                            <asp:BoundField DataField="ExpenseType" HeaderText="Expense Type" />
                            <asp:BoundField DataField="Date" HeaderText="Date" />
                            <asp:TemplateField HeaderText="From Time">
                                <ItemTemplate>
                                    <asp:Label ID="lblFromTime" runat="server" Text='<%# Eval("FromTime") != DBNull.Value ? Convert.ToDateTime(Eval("FromTime")).ToString("HH:mm") : string.Empty %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="To Time">
                                <ItemTemplate>
                                    <asp:Label ID="lblToTime" runat="server" Text='<%# Eval("ToTime") != DBNull.Value ? Convert.ToDateTime(Eval("ToTime")).ToString("HH:mm") : string.Empty %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Particulars" HeaderText="Particulars" />
                            <asp:BoundField DataField="Distance" HeaderText="Distance" />
                            <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                    <asp:TemplateField HeaderText="Attachment">
    <ItemTemplate>
        <!-- LinkButton for Viewing Image/Bill -->
        <asp:LinkButton ID="lnkViewImage" runat="server" 
            CommandName="ViewPDF" 
            CommandArgument='<%# Eval("ExpenseId") + "|" + Eval("SourceTable") %>' 
            CssClass="link-style" 
            Visible='<%# HasAttachment(Eval("ExpenseId"), Eval("SourceTable").ToString(), "Image") && 
                        new[] { "Lodging", "Others", "Conveyance", "Miscellaneous" }.Contains(Eval("SourceTable").ToString()) %>'>
            <i class="bi bi-file-earmark-image icon-large" aria-hidden="true" title="Bill"></i>
        </asp:LinkButton>

        <!-- LinkButton for Viewing Service Report -->
        <asp:LinkButton ID="lnkViewServiceReport" runat="server" 
            CommandName="ViewServiceReport" 
            CommandArgument='<%# Eval("ExpenseId") + "|" + Eval("SourceTable") %>' 
            CssClass="link-style" 
            Visible='<%# HasAttachment(Eval("ExpenseId"), Eval("SourceTable").ToString(), "ServiceReport") && 
                        new[] { "Lodging", "Others" }.Contains(Eval("SourceTable").ToString()) %>'>
            <i class="bi bi-file-earmark-text icon-large" aria-hidden="true" title="Service Report"></i>
        </asp:LinkButton>

        <!-- LinkButton for Viewing Approval Mail -->
        <asp:LinkButton ID="lnkViewApprovalMail" runat="server" 
            CommandName="ViewApprovalMail" 
            CommandArgument='<%# Eval("ExpenseId") + "|" + Eval("SourceTable") %>' 
            CssClass="link-style" 
            Visible='<%# HasAttachment(Eval("ExpenseId"), Eval("SourceTable").ToString(), "ApprovalMail") && 
                        new[] { "Lodging", "Others" }.Contains(Eval("SourceTable").ToString()) %>'>
            <i class="bi bi-envelope icon-large" aria-hidden="true" title="Work Order Mail"></i>
        </asp:LinkButton>

        <!-- Show message if no attachments available -->
        <asp:Label ID="lblNoAttachment" runat="server" 
            Text="No File" 
            Visible='<%# !HasAnyAttachment(Eval("ExpenseId"), Eval("SourceTable").ToString()) %>'
            CssClass="text-muted" 
            ToolTip="No attachments available">
        </asp:Label>
    </ItemTemplate>
</asp:TemplateField>

                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <div class="amount-container">
                                        <asp:CheckBox ID="chkConveyanceClaimable" runat="server"
                                            AutoPostBack="true"
                                            OnCheckedChanged="chkClaimable_CheckedChanged"
                                            Checked='<%# Eval("IsClaimable") != DBNull.Value && Convert.ToBoolean(Eval("IsClaimable")) %>'
                                            Visible='<%# !string.IsNullOrEmpty(Eval("Amount")?.ToString()) %>' />

                                        <asp:TextBox ID="txtConveyanceAmount" runat="server"
                                            Text='<%# Eval("Amount") %>'
                                            ReadOnly="true"
                                            CssClass="form-control mx-2"
                                            Visible='<%# !string.IsNullOrEmpty(Eval("Amount")?.ToString()) %>' />

                                        <asp:Button ID="btnEditConveyance" runat="server"
                                            Text="Edit"
                                            CommandName="EditRow"
                                            CommandArgument='<%# Container.DataItemIndex %>'
                                            CssClass="btn btn-secondary"
                                            OnClick="btnEditRow_Click"
                                            Visible='<%# !string.IsNullOrEmpty(Eval("Amount")?.ToString()) %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="amt-container">
                    <asp:Label ID="lblTotalClaimed" runat="server" Text="Total Claimed: " />
                    <asp:TextBox ID="txtTotalClaimedAmount" runat="server" ReadOnly="true" CssClass="form-control total-amount" />

                    <asp:Label ID="lblTotalNonClaimed" runat="server" Text="Total Non-Claimed: " />
                    <asp:TextBox ID="txtTotalNonClaimedAmount" runat="server" ReadOnly="true" CssClass="form-control total-amount" />
                </div>

                <div class="button-container">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn btn-primary px-4 py-2" Style="background-color: green;" />
                    <asp:Button ID="Button1" runat="server" Text="Export to Excel" OnClick="btnExportToExcel_Click" CssClass="btn btn-success px-4 py-2" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btncancel_Click" CssClass="btn btn-secondary px-4 py-2" Style="background-color: #e03b00;" />
                </div>

                <div>
                    <asp:Label ID="Label1" runat="server" ForeColor="Green" Visible="false"></asp:Label>
                </div>

                <asp:Label ID="lblMessage" runat="server" Style="display: none;"></asp:Label>

                <div class="containerback">
                    <button id="btnBack" runat="server" onserverclick="btnBack_Click" class="back-button">
                        <i class="fas fa-arrow-left"></i>
                    </button>
                </div>

            </section>
        </div>
    </main>
</asp:Content>