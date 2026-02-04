<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="TravelExpensePage.aspx.cs" Inherits="Vivify.TravelExpensePage" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <style>
        /* Your existing CSS remains the same */
        body {
            font-size: 14px;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
        
        .HeaderStyle {
            width: 100%;
        }

        .grid-container {
            max-height: 400px;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: white;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            font-size: 13px;
            margin-top: 15px;
        }

        .gridview {
            width: 100%;
            border-spacing: 0;
            margin: 0;
            font-size: 13px;
            border-collapse: collapse;
        }

        .gridview th, .gridview td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }

        .gridview th {
            background-color: #3f418d;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            font-size: 13px;
            padding: 8px;
            font-weight: 600;
        }

        .gridview tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        .gridview tr:hover {
            background-color: #e9ecef;
        }

        main {
            background-color: #cadcfc;
        }

        #header {
            background-color: #3f418d;
        }

        .filter-container {
            background-color: white;
            padding: 15px;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            margin-bottom: 15px;
        }

        .form-row {
            display: flex;
            align-items: end;
            flex-wrap: nowrap;
            gap: 10px;
            margin-bottom: 10px;
        }

        .filter-group {
            display: flex;
            flex-direction: column;
            flex: 1;
            min-width: 120px;
        }

        .filter-label {
            font-size: 12px;
            font-weight: 600;
            color: #333;
            margin-bottom: 4px;
            white-space: nowrap;
        }

        .filter-control {
            width: 100%;
            padding: 6px 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 13px;
            background-color: white;
            height: 34px;
            box-sizing: border-box;
        }

        .filter-control:focus {
            outline: none;
            border-color: #3f418d;
            box-shadow: 0 0 0 2px rgba(63, 65, 141, 0.2);
        }

        .search-btn-container {
            display: flex;
            align-items: end;
            margin-top: 0;
        }

        #ContentPlaceHolder1_btnFilter {
            background-color: #3f418d !important;
            color: white !important;
            padding: 8px 20px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            min-width: 120px;
            height: 34px;
            box-sizing: border-box;
        }

        #ContentPlaceHolder1_btnFilter:hover {
            background-color: #2a2c6b !important;
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        }

        @media (max-width: 768px) {
            .form-row {
                flex-wrap: wrap;
                gap: 10px;
            }
            
            .filter-group {
                min-width: calc(50% - 10px);
            }
        }

        .content {
            margin-left: 10px;
            padding: 15px;
            background-color: #cadcfc;
            min-height: calc(100vh - 40px);
            padding-bottom: 50px;
            box-sizing: border-box;
        }

        .btn-primary {
            background-color: #3f418d;
            border-color: #3f418d;
            font-size: 12px;
            padding: 6px 12px;
            border-radius: 4px;
            color: white;
            border: none;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn-primary:hover {
            background-color: #2a2c6b;
            border-color: #2a2c6b;
            color: white;
        }

        .verified-btn {
            background-color: #28a745 !important;
            border-color: #28a745 !important;
            cursor: not-allowed;
        }

        .verified-btn:hover {
            background-color: #218838 !important;
            border-color: #1e7e34 !important;
            transform: none !important;
            box-shadow: none !important;
        }

        .card-title {
            text-align: center;
            background-color: #3f418d;
            color: white;
            padding: 12px;
            margin: 0;
            font-size: 16px;
            font-weight: 600;
            border-radius: 8px 8px 0 0;
        }
        
        .card {
            margin-bottom: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            border: none;
        }
        
        .custom-button {
            font-size: 12px;
            padding: 6px 12px;
            border-radius: 4px;
            font-weight: 500;
            min-width: 120px;
        }

        .main {
            padding: 0;
            margin: 0;
        }

        .section.dashboard {
            padding: 0;
            margin: 0;
        }

        .row {
            margin: 0;
        }

        .col {
            padding: 0;
        }

        #ContentPlaceHolder1_verificationForm {
            background-color: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            margin-top: 20px;
        }

        .footer {
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
            background-color: #f9f3f3;
            text-align: center;
            padding: 10px;
            color: #333;
            z-index: 1000;
            border-top: 1px solid #ddd;
        }

        .footer a {
            color: #3f418d;
            text-decoration: none;
            font-weight: 500;
        }

        .footer a:hover {
            text-decoration: underline;
        }

        .filter-container .form-row {
            display: flex;
            align-items: end;
        }

        .filter-container .filter-group {
            flex: 1;
            margin-bottom: 0;
        }
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
    </style>

      <aside id="sidebar" class="sidebar" style="box-shadow: 0 2px 10px rgba(63, 65, 141, 0.3);">
        <ul class="sidebar-nav" id="sidebar-nav">
            <li class="nav-item">
                <a class="nav-link" href="AdminPage.aspx">
                    <i class="bi bi-pc-display"></i>
                    <span>Expense Page</span>
                </a>
            </li>
           <li class="nav-item">
  <a class="nav-link" href="TravelExpensePage.aspx">
    <i class="bi bi-geo-alt"></i>
    <span>Travel Expense Page</span>
  </a>
</li>

<li class="nav-item">
  <a class="nav-link" href="TravelReport.aspx">
    <i class="bi bi-map"></i>
    <span>Travel Report</span>
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
    <a class="nav-link" href="RefreshmentReport.aspx">
        <i class="bi-file-earmark-text"></i> 
        <span>Refreshment Report</span>
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
    <i class="bi bi-journal-text"></i>
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

   <div class="content">
    <main id="main" class="main">
        <section class="section dashboard">
            <div class="row">
                <div class="col">
                    <div class="card">
                        <h5 class="card-title">Travel Expense Verification</h5>
                        <asp:Label ID="Label1" runat="server" ForeColor="Red" Visible="false"></asp:Label>

                        <div class="filter-container">
                            <div class="form-row">
                                <div class="filter-group">
                                    <label class="filter-label" for="txtFromDate">From Date</label>
                                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="filter-control"></asp:TextBox>
                                </div>

                                <div class="filter-group">
                                    <label class="filter-label" for="txtToDate">To Date</label>
                                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="filter-control"></asp:TextBox>
                                </div>

                                <div class="filter-group">
                                    <label class="filter-label" for="ddlRegion">Region</label>
                                    <asp:DropDownList ID="ddlRegion" runat="server" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged" 
                                        AutoPostBack="true" CssClass="filter-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="filter-group">
                                    <label class="filter-label" for="ddlBranch">Branch Name</label>
                                    <asp:DropDownList ID="ddlBranch" runat="server" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" 
                                        AutoPostBack="true" CssClass="filter-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="filter-group">
                                    <label class="filter-label" for="ddlEmployee">Employee Name</label>
                                    <asp:DropDownList ID="ddlEmployee" runat="server" AutoPostBack="True" CssClass="filter-control">
                                    </asp:DropDownList>
                                </div>
                                
                                <div class="search-btn-container">
                                    <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" CssClass="btn-primary" />
                                </div>
                            </div>
                        </div>

                        <section class="scrollable-container">
                            <div class="grid-container">
                            
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false"
    CssClass="gridview" OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound">
    <HeaderStyle BackColor="#3f418d" ForeColor="White" />
    <RowStyle CssClass="grid-row" />
    <AlternatingRowStyle CssClass="grid-alt-row" />

    <Columns>
        <asp:TemplateField HeaderText="S.No">
            <ItemTemplate>
                <%# Container.DataItemIndex + 1 %>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="60px" />
        </asp:TemplateField>
        
        <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" ItemStyle-HorizontalAlign="Center" />
        
        <asp:BoundField DataField="BranchName" HeaderText="Branch" ItemStyle-HorizontalAlign="Center" />
        
        <asp:BoundField DataField="TravelDate" HeaderText="Travel Date" ItemStyle-HorizontalAlign="Center" />
        
   
        
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:Button ID="btnVerify" runat="server"
    CommandName="Verify" 
    CommandArgument='<%# Eval("Id") + "|" + Eval("EmployeeName") + "|" + Eval("TravelDate") %>'
    CssClass="btn btn-primary custom-button center-button" />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="180px" />
        </asp:TemplateField>
    </Columns>
    
    <EmptyDataTemplate>
        <div style="text-align: center; padding: 20px;">
            No records found for the selected criteria.
        </div>
    </EmptyDataTemplate>
</asp:GridView>    <asp:Panel ID="verificationForm" runat="server" Visible="false">
                                    <h3>Verify and Update Expenses</h3>
                                    <div style="margin-bottom: 15px;">
                                        <label>Conveyance Total:</label>
                                        <asp:TextBox ID="txtConveyanceTotal" runat="server" ReadOnly="true" />
                                        <asp:CheckBox ID="chkConveyanceClaimable" runat="server" Text="Claimable" />
                                        <asp:TextBox ID="txtConveyanceTotalEditable" runat="server" />
                                    </div>

                                    <div style="margin-bottom: 15px;">
                                        <label>Food Total:</label>
                                        <asp:TextBox ID="txtFoodTotal" runat="server" ReadOnly="true" />
                                        <asp:CheckBox ID="chkFoodClaimable" runat="server" Text="Claimable" />
                                        <asp:TextBox ID="txtFoodTotalEditable" runat="server" />
                                    </div>

                                    <div style="margin-bottom: 15px;">
                                        <label>Miscellaneous Total:</label>
                                        <asp:TextBox ID="txtMiscellaneousTotal" runat="server" ReadOnly="true" />
                                        <asp:CheckBox ID="chkMiscellaneousClaimable" runat="server" Text="Claimable" />
                                        <asp:TextBox ID="txtMiscellaneousTotalEditable" runat="server" />
                                    </div>

                                    <div style="margin-bottom: 15px;">
                                        <label>Others Total:</label>
                                        <asp:TextBox ID="txtOthersTotal" runat="server" ReadOnly="true" />
                                        <asp:CheckBox ID="chkOthersClaimable" runat="server" Text="Claimable" />
                                        <asp:TextBox ID="txtOthersTotalEditable" runat="server" />
                                    </div>

                                    <div style="margin-bottom: 15px;">
                                        <label>Lodging Total:</label>
                                        <asp:TextBox ID="txtLodgingTotal" runat="server" ReadOnly="true" />
                                        <asp:CheckBox ID="chkLodgingClaimable" runat="server" Text="Claimable" />
                                        <asp:TextBox ID="txtLodgingTotalEditable" runat="server" />
                                    </div>
                                    
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn btn-primary" />
                                    <br />
                                    <asp:Label ID="lblError" runat="server" ForeColor="Red" />
                                </asp:Panel>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </section>
    </main>
</div>
</asp:Content>