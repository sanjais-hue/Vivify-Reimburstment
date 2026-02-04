<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="RefreshmentReport.aspx.cs" Inherits="Vivify.RefreshmentReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            display: flex;
            flex-direction: column;
            min-height: 100vh;
        }

        .content {
            margin-left: 10px;
            padding: 20px;
            background-color: #cadcfc;
            min-height: calc(100vh - 40px);
            padding-bottom: 50px;
            box-sizing: border-box;
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
        .card-title {
            text-align: center;
            background-color: #3f418d;
            color: white;
            padding: 10px;
            margin: 0;
            border-radius: 4px 4px 0 0;
        }

        .footer {
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
            background-color: rgb(249, 243, 243);
            text-align: center;
            padding: 10px;
            color: ghostwhite;
            z-index: 1000;
        }

        .footer a {
            color: midnightblue;
            text-decoration: none;
        }

        .footer a:hover {
            text-decoration: underline;
        }

     
        .sidebar-nav .nav-link:hover {
            color: red;
            border: 2px solid #222b65;
            box-shadow: 0 2px 10px #1f2b60;
        }

        main {
            flex-grow: 1;
        }

        /* GRID */
        .grid-container {
            max-height: 500px;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: white;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-top: 10px;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
        }

        .gridview th, .gridview td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
            font-size: 13px;
        }

        .gridview th {
            background-color: darkblue;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
        }

        #btnGenerate {
            display: block;
            margin: 20px auto;
            padding: 8px 20px;
            font-size: 14px;
            background-color: darkblue;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }

        /* ✅ FILTER CARD - SINGLE LINE, SMALL FONT, CLEAN */
        .filter-card {
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
            padding: 12px 15px;
            margin: 15px 0;
        }

        .filter-row {
            display: flex;
            align-items: end;
            gap: 12px;
            flex-wrap: nowrap; /* Keep in one line on desktop */
        }

        .filter-group {
            display: flex;
            flex-direction: column;
            min-width: 130px;
        }

        .filter-group label {
            font-size: 12px;
            font-weight: 600;
            color: #495057;
            margin-bottom: 4px;
        }

        .filter-group .form-control,
        .filter-group .form-select {
            padding: 6px 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 13px;
            width: 100%;
            box-sizing: border-box;
            height: auto;
        }

        .filter-action {
            margin-left: auto;
        }

        .btn-search {
            background-color: #3f418d;
            color: white;
            padding: 6px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            font-weight: bold;
            min-width: 80px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .btn-search::before {
            content: "🔍";
            font-size: 14px;
        }

        .btn-search:hover {
            background-color: #222b65;
        }

        /* MOBILE RESPONSIVE */
        @media (max-width: 768px) {
            .filter-row {
                flex-direction: column;
                gap: 8px;
            }

            .filter-group {
                width: 100%;
            }

            .filter-action {
                margin-left: 0;
                width: 100%;
            }

            .btn-search {
                width: 100%;
                justify-content: center;
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
                            <h5 class="card-title">Refreshment Report</h5>
                            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>

                            <!-- ✅ FILTER CONTAINER - SINGLE LINE -->
                            <div class="filter-card">
                                <div class="filter-row">
                                    <div class="filter-group">
                                        <label>From Date</label>
                                        <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="filter-group">
                                        <label>To Date</label>
                                        <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="filter-group">
                                        <label>Region</label>
                                        <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged" CssClass="form-select">
                                            <asp:ListItem Text="All Regions" Value="" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="filter-group">
                                        <label>Branch Name</label>
                                        <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" CssClass="form-select">
                                            <asp:ListItem Text="All Branches" Value="" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="filter-group">
                                        <label>Employee Name</label>
                                        <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="All Employees" Value="" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="filter-action">
                                        <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" CssClass="btn-search" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <section class="scrollable-container">
                            <div class="grid-container">
                         <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="gridview">
    <HeaderStyle BackColor="darkblue" ForeColor="White" Font-Size="11px" />
    <RowStyle Font-Size="11px" />
    <Columns>
        <asp:BoundField DataField="Eng_Name" HeaderText="Employee Name" />
        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
        <asp:BoundField DataField="FromTime" HeaderText="From Time" />
        <asp:BoundField DataField="ToTime" HeaderText="To Time" />
        <asp:BoundField DataField="Particulars" HeaderText="Particulars" />
        <asp:BoundField DataField="Distance" HeaderText="Distance" />
        <asp:BoundField DataField="Transport" HeaderText="Mode of Transport" />
        <asp:BoundField DataField="Conveyance" HeaderText="Conveyance" />
        <asp:BoundField DataField="Lodging" HeaderText="Lodging" />
        <asp:BoundField DataField="Food" HeaderText="Fooding Exp" />
        <asp:BoundField DataField="TrainBus" HeaderText="Train/Bus Fair" />
        <asp:BoundField DataField="Others" HeaderText="Others" />
        <asp:BoundField DataField="Miscellaneous" HeaderText="Misc."  />
        <asp:BoundField DataField="Total" HeaderText="Total" />
        <asp:BoundField DataField="Department" HeaderText="Department" />
        <asp:BoundField DataField="Nature_of_Work" HeaderText="Nature of Work" />
        <asp:BoundField DataField="SMO" HeaderText="SMO/SO/WBS" />
        <asp:BoundField DataField="Document_Reference" HeaderText="Doc Ref" />
    </Columns>
</asp:GridView>
                            </div>
                            <asp:Button ID="btnGenerate" runat="server" Text="Generate Excel" OnClick="btnGenerate_Click"  style="background-color:darkblue; color:white;" />
                        </section>
                    </div>
                </div>
            </section>
        </main>
    </div>
</asp:Content>