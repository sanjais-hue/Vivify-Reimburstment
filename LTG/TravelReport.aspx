<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="TravelReport.aspx.cs" Inherits="Vivify.TravelReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
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
        /* Nuclear option to remove any employee headers */
tr.employee-header-row,
td.employee-header,
.employee-header-row,
.employee-header {
    display: none !important;
    visibility: hidden !important;
    height: 0 !important;
    padding: 0 !important;
    margin: 0 !important;
    border: none !important;
    font-size: 0 !important;
    line-height: 0 !important;
}
        .card-title {
            text-align: center;
            background-color: #3f418d;
            color: white;
            padding: 10px;
            margin: 0;
            border-radius: 4px 4px 0 0;
        }

      .grid-container {
    max-height: 300px; /* Changed from 600px to 300px */
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
            font-size: 12px;
        }

        .gridview th, .gridview td {
            border: 1px solid #ddd;
            padding: 6px;
            text-align: center;
        }

        .gridview th {
            background-color: darkblue;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
            font-weight: bold;
        }

/*        .gridview tr.total-row {
            background-color: #e8f4ff;
            font-weight: bold;
        }

        .gridview tr.total-row td {
            background-color: #d4edda;
            color: #155724;
            font-weight: bold;
        }*/

      .employee-header-row {
    background-color: transparent !important; /* Remove yellow background */
}

.employee-header {
    background-color: transparent !important; /* Remove yellow background */
    border: none !important; /* Remove borders if any */
}

        .non-claimable-row {
            background-color: #fff3cd !important;
        }

        /* FILTER CARD - UPDATED with searchable dropdown styles */
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

        .btn-primary {
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

        .btn-primary:hover {
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

        /* ADDED: Searchable dropdown specific styles */
        .datalist-container {
            position: relative;
        }
        
        .datalist-dropdown {
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            max-height: 200px;
            overflow-y: auto;
            background-color: white;
             border: 1px solid #000;
    border-radius: 6px;
     box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
 box-shadow: 0 4px 12px rgba(63, 65, 141, 0.25);
            z-index: 1000;
            display: none;
            margin-top: 2px;
        }
        
        .datalist-option {
            padding: 8px 12px;
            cursor: pointer;
            font-size: 13px;
            border-bottom: none !important; 
        }
        
        .datalist-option:hover {
            background-color: #1E73D8; 
            color: white;
        }
        
        .datalist-option:last-child {
            border-bottom: none;
        }
        
        .searchable-dropdown {
            width: 100%;
            padding: 6px 32px 6px 8px;
            border: 1px solid #ddd;
            border-radius: 6px;
            font-size: 13px;
            background-color: white;
            height: 34px;
            box-sizing: border-box;
            cursor: pointer;
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' fill='%23000000' viewBox='0 0 16 16'%3E%3Cpath d='M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z'/%3E%3C/svg%3E");
            background-repeat: no-repeat;
            background-position: right 10px center;
            background-size: 12px;
        }
        
        .searchable-dropdown:focus {
            outline: none;
border-color: #3f418d;
box-shadow: 0 0 0 2px rgba(63, 65, 141, 0.2);
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' fill='%23000000' viewBox='0 0 16 16'%3E%3Cpath d='M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z'/%3E%3C/svg%3E");
        }

        .btn-excel {
            background-color: #217346;
            color: white;
            padding: 8px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            font-weight: bold;
            margin: 10px auto;
            display: block;
        }

        .btn-excel:hover {
            background-color: #1e663e;
        }

        /* SIDEBAR */
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

        /* MOBILE RESPONSIVE */
        @media (max-width: 768px) {
            .form-row {
                flex-direction: column;
                gap: 8px;
            }

            .filter-group {
                width: 100%;
            }

            .search-btn-container {
                margin-left: 0;
                width: 100%;
            }

            .btn-primary {
                width: 100%;
                justify-content: center;
            }
        }
        
        /* ADDED: UpdatePanel smooth transitions */
      
    </style>

 <script type="text/javascript">

     /* ===============================
        CREATE SEARCHABLE DROPDOWN
        =============================== */
     function createSearchableDropdown(dropdownId) {

         var dropdown = document.getElementById(dropdownId);
         if (!dropdown) return;

         /* 🔒 DO NOT recreate if already exists */
         var parent = dropdown.parentNode;
         var existingContainer = parent.querySelector('.datalist-container');

         if (existingContainer) {
             var input = existingContainer.querySelector('.searchable-dropdown');
             if (dropdown.selectedIndex >= 0) {
                 input.value = dropdown.options[dropdown.selectedIndex].text;
             }
             return;
         }

         /* Input */
         var input = document.createElement('input');
         input.type = 'text';
         input.className = 'searchable-dropdown form-control';
         input.autocomplete = 'off';

         if (dropdownId.includes('ddlRegion')) input.placeholder = 'Type to search regions...';
         else if (dropdownId.includes('ddlBranch')) input.placeholder = 'Type to search branches...';
         else if (dropdownId.includes('ddlEmployee')) input.placeholder = 'Type to search employees...';
         else input.placeholder = 'Type to search...';

         /* Containers */
         var container = document.createElement('div');
         container.className = 'datalist-container';

         var list = document.createElement('div');
         list.className = 'datalist-dropdown';
         list.style.display = 'none';

         /* Hide select */
         dropdown.style.display = 'none';

         /* Initial value */
         if (dropdown.selectedIndex >= 0) {
             input.value = dropdown.options[dropdown.selectedIndex].text;
         }

         /* Populate options */
         function populate(text) {
             list.innerHTML = '';
             text = (text || '').toLowerCase();
             var found = false;

             for (var i = 0; i < dropdown.options.length; i++) {
                 var opt = dropdown.options[i];

                 if (!text || opt.text.toLowerCase().includes(text)) {
                     var div = document.createElement('div');
                     div.className = 'datalist-option';
                     div.textContent = opt.text;
                     div.dataset.value = opt.value;

                     div.onclick = function () {
                         input.value = this.textContent;
                         dropdown.value = this.dataset.value;
                         list.style.display = 'none';

                         dropdown.dispatchEvent(new Event('change'));

                         if (dropdown.id.includes('ddlRegion') || dropdown.id.includes('ddlBranch')) {
                             __doPostBack(dropdown.id, '');
                         }
                     };

                     list.appendChild(div);
                     found = true;
                 }
             }

             if (!found) {
                 var empty = document.createElement('div');
                 empty.className = 'datalist-option';
                 empty.textContent = 'No results found';
                 empty.style.color = '#999';
                 list.appendChild(empty);
             }
         }

         /* Events */
         input.onclick = input.onfocus = function () {
             populate('');
             list.style.display = 'block';
         };

         input.oninput = function () {
             populate(this.value);
             list.style.display = 'block';
         };

         document.addEventListener('click', function (e) {
             if (!container.contains(e.target)) {
                 list.style.display = 'none';
             }
         });

         input.onblur = function () {
             setTimeout(function () {
                 list.style.display = 'none';
             }, 150);
         };

         /* Append */
         container.appendChild(input);
         container.appendChild(list);
         parent.insertBefore(container, dropdown.nextSibling);

         populate('');
     }

     /* ===============================
        INIT DROPDOWNS
        =============================== */
     function initializeSearchableDropdowns() {
         createSearchableDropdown('<%= ddlRegion.ClientID %>');
        createSearchableDropdown('<%= ddlBranch.ClientID %>');
        createSearchableDropdown('<%= ddlEmployee.ClientID %>');
     }

     document.addEventListener('DOMContentLoaded', initializeSearchableDropdowns);

     /* ===============================
        UPDATEPANEL SAFE HANDLING
        =============================== */
     if (typeof Sys !== 'undefined') {

         var prm = Sys.WebForms.PageRequestManager.getInstance();

         prm.add_beginRequest(function () {
             var up = document.getElementById('<%= UpdatePanel1.ClientID %>');
            if (up) up.style.opacity = '0.85';
        });

        prm.add_endRequest(function () {

            var up = document.getElementById('<%= UpdatePanel1.ClientID %>');
            if (up) up.style.opacity = '1';

            var dropdowns = [
                '<%= ddlRegion.ClientID %>',
                '<%= ddlBranch.ClientID %>',
                '<%= ddlEmployee.ClientID %>'
            ];

            dropdowns.forEach(function (id) {
                var ddl = document.getElementById(id);
                if (ddl && !ddl.parentNode.querySelector('.datalist-container')) {
                    createSearchableDropdown(id);
                }
            });
        });
     }

 </script>


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
                            <h5 class="card-title">Travel Expense Report</h5>
                            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>

                            <!-- UpdatePanel for filter and GridView only -->
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <!-- FILTER CONTAINER -->
                                    <div class="filter-container">
                                        <div class="form-row">
                                            <div class="filter-group">
                                                <label class="filter-label" for="<%= txtFromDate.ClientID %>">From Date</label>
                                                <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="filter-control"></asp:TextBox>
                                            </div>

                                            <div class="filter-group">
                                                <label class="filter-label" for="<%= txtToDate.ClientID %>">To Date</label>
                                                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="filter-control"></asp:TextBox>
                                            </div>

                                            <div class="filter-group">
                                                <label class="filter-label" for="<%= ddlRegion.ClientID %>">Region</label>
                                                <asp:DropDownList ID="ddlRegion" runat="server" 
                                                    OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged" 
                                                    AutoPostBack="True" CssClass="filter-control">
                                                    <asp:ListItem Text="All Regions" Value="" Selected="True"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div class="filter-group">
                                                <label class="filter-label" for="<%= ddlBranch.ClientID %>">Branch Name</label>
                                                <asp:DropDownList ID="ddlBranch" runat="server"  
                                                    OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" 
                                                    AutoPostBack="True" CssClass="filter-control">
                                                    <asp:ListItem Text="All Branches" Value="" Selected="True"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div class="filter-group">
                                                <label class="filter-label" for="<%= ddlEmployee.ClientID %>">Employee Name</label>
                                                <asp:DropDownList ID="ddlEmployee" runat="server" 
                                                    AutoPostBack="False" CssClass="filter-control">
                                                    <asp:ListItem Text="All Employees" Value="" Selected="True"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div class="search-btn-container">
                                                <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" CssClass="btn-primary" />
                                            </div>
                                        </div>
                                    </div>

                                    <!-- GRIDVIEW ONLY (inside UpdatePanel) -->
                                    <section class="scrollable-container">
                                        <div class="grid-container">
                                            <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="gridview" 
                                                OnRowDataBound="gvReport_RowDataBound" ShowHeader="true">
                                                <HeaderStyle BackColor="darkblue" ForeColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" ItemStyle-Width="150px" />
                                                    <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-Width="100px" />
                                                    <asp:BoundField DataField="FromPlace" HeaderText="From Place" ItemStyle-Width="120px" />
                                                    <asp:BoundField DataField="ToPlace" HeaderText="To Place" ItemStyle-Width="120px" />
                                                    <asp:BoundField DataField="TransportType" HeaderText="Travel Mode" ItemStyle-Width="120px" />
                                                    <asp:BoundField DataField="ClaimedAmount" HeaderText="Amount" DataFormatString="{0:N2}" ItemStyle-Width="100px" />
                                                    <asp:BoundField DataField="RefreshAmnt" HeaderText="Refreshments" DataFormatString="{0:N2}" ItemStyle-Width="100px" />
                                                    <asp:BoundField DataField="Particulars" HeaderText="Purpose of Travel" ItemStyle-Width="200px" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </section>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlRegion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlBranch" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <!-- EXCEL BUTTON OUTSIDE UpdatePanel -->
                            
                                
                            
                        </div>
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate Excel Report" OnClick="btnGenerate_Click" CssClass="btn-excel" />
                    </div>
                </div>
            </section>
        </main>
    </div>
</asp:Content>