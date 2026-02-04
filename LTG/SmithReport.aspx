<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SmithReport.aspx.cs" Inherits="Vivify.SmithReport" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        /* Grid Styling */
        .mydatagrid th, .mydatagrid td {
            border: 1px solid black;
            padding: 8px;
            margin: 0;
            text-align: center;
        }
        .mydatagrid {
            width: 100%;
            border-collapse: collapse;
            margin: 0;
            padding: 0;
        }
          .mydatagrid11 {
      width: 120%;
      border-collapse: collapse;
      margin: 0;
      padding: 0;
  }
           .mydatagrid1 th {
     background-color: #3f418d;
     color: white;
     position: sticky;
     top: 0;
     z-index: 10;
 }
           .mydatagrid1 th, .mydatagrid1 td {
     border: 1px solid black;
     padding: 8px;
     margin: 0;
     text-align: center;
 }
        .mydatagrid th {
            background-color: #3f418d;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
        }
        .scrollable-container {
            max-height: 400px;
            max-width:800px;
            overflow-x: auto;
            overflow-y: auto;
            border: 1px solid #ccc;
            margin-bottom: 20px;
        }

        /* Button Styling */
        .btn-primary {
            background-color: #3f418d;
            color: white;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
        }
        .btn-primary:hover {
            background-color: #2a2c5d;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .scrollable-container {
                width: 100%;
            }
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
                        <h5 class="card-title" style="text-align:center;background-color:#3f418d;color:white">Smith Report</h5>
                        <section class="form-container section error-404 d-flex flex-column align-items-center justify-content-center" >
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
    <asp:DropDownList ID="ddlRegion" runat="server" 
        OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged" 
        AutoPostBack="true" CssClass="filter-control">
    </asp:DropDownList>
</div>

        <div class="filter-group">
            <label class="filter-label" for="ddlBranch">Branch Name</label>
            <asp:DropDownList ID="ddlBranch" runat="server" 
                OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" 
                AutoPostBack="true" CssClass="filter-control">
            </asp:DropDownList>
        </div>

        <div class="search-btn-container">
            <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" CssClass="btn-primary" />
        </div>
    </div>
</div>
                            <br />
                                                        <!-- Department Totals Report -->
                           <div class="grid-container mobile-card">
                          <!-- <h4 style="margin-top: 10px;">Local & Tour Expenses</h4>-->
                          <div class="scrollable-container">
                              <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                                  <asp:GridView ID="gvDepartmentTotals" runat="server" AutoGenerateColumns="false" CssClass="mydatagrid" OnDataBound="gvDepartmentTotals_DataBound">
                        <Columns>
                            <asp:BoundField DataField="SNo" HeaderText="S.No" HtmlEncode="false" />
                            <asp:BoundField DataField="Month" HeaderText="Month" HtmlEncode="false" />
                            <asp:BoundField DataField="BranchName" HeaderText="Branch Name" HtmlEncode="false" />
                            <asp:BoundField DataField="ServiceTotal" HeaderText="Service/Refresh Total" HtmlEncode="false" />
                            <asp:BoundField DataField="SalesTotal" HeaderText="Sales Total" HtmlEncode="false" />
                           <%-- <asp:BoundField DataField="RefreshTotal" HeaderText="Refresh Total" HtmlEncode="false" />--%>
                             <asp:BoundField DataField="AwardTotal" HeaderText="Award Total" HtmlEncode="false" />
                            <asp:BoundField DataField="OverallTotal" HeaderText="Reimbursement Total" HtmlEncode="false" />
                            <asp:BoundField DataField="HandlingCharge" HeaderText="Handling Charge 5%" HtmlEncode="false" />
                            <asp:BoundField DataField="WithoutGSTAmount" HeaderText="Without GST Amount" HtmlEncode="false" />
                        </Columns>
                    </asp:GridView>
                          </div>
                      </div>
                            <div class="scrollable-container">
                    <asp:Label ID="Label1" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                     <asp:GridView ID="gvOverallTotals" runat="server" AutoGenerateColumns="False" CssClass="mydatagrid"  OnDataBound="gvOverallTotals_DataBound">
                    <Columns>
        
                        <asp:TemplateField HeaderText="SNo">
                            <ItemTemplate>
                                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Department" HeaderText="Department" />
                        <asp:BoundField DataField="ExpenseAmount" HeaderText="Expense Amount" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="HandlingCharge" HeaderText="Handling Charge 5%" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalWithoutGST" HeaderText="Total Without GST" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="GST" HeaderText="GST 18%" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="TotalWithGST" HeaderText="Total With GST" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
                        </div>
                            <!-- SMO Report -->
                              <div class="grid-container mobile-card">
                                
                                <asp:Label ID="lblHeaderMessage" runat="server" CssClass="header-message"></asp:Label>

                                <div class="scrollable-container">
                                    <asp:GridView ID="gvSmoSoReport" runat="server" AutoGenerateColumns="False" CssClass="mydatagrid1">
                                        <Columns>
                                            <asp:BoundField DataField="SerialNo" HeaderText="S.No" />
                                            <asp:BoundField DataField="MonthYear" HeaderText="Month" />
                                            <asp:BoundField DataField="BranchName" HeaderText="Branch Name" />
                                            <asp:BoundField DataField="SmoSo" HeaderText="Smo No - So No" />
                                            <asp:BoundField DataField="Department" HeaderText="Department" />
                                            <asp:BoundField DataField="TotalClaimedAmount" HeaderText="Reimbursement Total" />
                                            <asp:BoundField DataField="HandlingCharge" HeaderText="Handling Charge (5%)" />
                                            <asp:BoundField DataField="TotalWithoutGST" HeaderText="Total Without GST" />
                                            <asp:BoundField DataField="GST" HeaderText="GST (18%)" />
                                            <asp:BoundField DataField="OverallTotal" HeaderText="Overall Total" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            </section>
                        </div>
                    </div>
                </div>
            </section>

            <!-- Generate Excel Button -->
            <asp:Button ID="btnGenerate" runat="server" Text="Generate Excel" OnClick="btnGenerate_Click" CssClass="btn-primary" style="margin-top: 20px;" />
        </main>
    </div>
</asp:Content>