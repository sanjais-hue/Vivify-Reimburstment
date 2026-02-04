<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="Vivify.AdminPage" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <style>
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
        }

        .verified-btn:hover {
            background-color: #218838 !important;
            border-color: #1e7e34 !important;
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

        /* Searchable Dropdown Styles */
        .custom-dropdown {
            position: relative;
            width: 100%;
        }

        .custom-dropdown.open {
            z-index: 10001;
        }

        .dropdown-input {
            width: 100%;
            padding: 6px 30px 6px 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 13px;
            background-color: white;
            height: 34px;
            box-sizing: border-box;
            cursor: text;
            outline: none;
        }

        .dropdown-input:focus {
            border-color: #3f418d;
            box-shadow: 0 0 0 2px rgba(63, 65, 141, 0.2);
        }

        .dropdown-arrow {
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            font-size: 12px;
            color: #666;
            pointer-events: none;
            transition: transform 0.3s ease;
        }

        .dropdown-arrow.open {
            transform: translateY(-50%) rotate(180deg);
        }

        .dropdown-options {
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border: 1px solid #ddd;
            border-top: none;
            border-radius: 0 0 4px 4px;
            max-height: 250px;
            overflow-y: auto;
            z-index: 10000;
            display: none;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        .dropdown-options.show {
            display: block;
        }

        .dropdown-option {
            padding: 8px 12px;
            cursor: pointer;
            font-size: 13px;
            border-bottom: 1px solid #f0f0f0;
            transition: background-color 0.2s ease;
        }

        .dropdown-option:hover {
            background-color: #f8f9fa;
        }

        .dropdown-option.selected {
            background-color: #3f418d;
            color: white;
        }

        .dropdown-option:last-child {
            border-bottom: none;
        }

        .no-results {
            padding: 12px;
            text-align: center;
            color: #999;
            font-style: italic;
        }

        /* Hide original dropdowns */
        .hidden-dropdown {
            display: none !important;
        }

        /* Anti-shake stabilization */
        .filter-group {
            min-height: 60px;
            position: relative;
        }

        .custom-dropdown {
            position: relative;
            z-index: 1;
        }

        /* Enhanced freeze during postback to prevent all shaking */
        .filter-container.postback-freeze {
            pointer-events: none;
            position: relative;
        }

        .filter-container.postback-freeze * {
            transition: none !important;
            animation: none !important;
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
        }

        .filter-container.postback-freeze .filter-control,
        .filter-container.postback-freeze .dropdown-input {
            background-color: white !important;
            border-color: #ddd !important;
            opacity: 1 !important;
            position: relative !important;
        }

        .custom-dropdown.frozen {
            pointer-events: none;
            opacity: 0.95;
            position: relative;
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
        }

        .custom-dropdown.frozen .dropdown-input {
            transition: none !important;
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            position: relative !important;
            width: 100% !important;
            height: 34px !important;
        }

        .custom-dropdown.frozen .dropdown-arrow {
            transition: none !important;
            transform: translateY(-50%) translateZ(0) !important;
            backface-visibility: hidden !important;
        }

        /* Stabilize all form elements during postback */
        .form-element-stable {
            position: relative !important;
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            transition: none !important;
            will-change: auto !important;
        }

        /* Lock filter groups during postback */
        .filter-group.locked {
            position: relative !important;
            overflow: hidden !important;
        }

        .filter-group.locked * {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
        }

        .dropdown-arrow.loading {
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% { transform: translateY(-50%) rotate(0deg); }
            100% { transform: translateY(-50%) rotate(360deg); }
        }

        /* Additional anti-shake classes for maximum stability */
        .position-locked {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            will-change: auto !important;
            transition: none !important;
        }

        .input-locked {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            will-change: auto !important;
            transition: none !important;
            position: relative !important;
        }

        .dropdown-locked {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            will-change: auto !important;
            transition: none !important;
        }

        .container-locked {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            will-change: auto !important;
        }

        /* Prevent any movement during postback */
        .filter-container.postback-freeze .filter-group {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            will-change: auto !important;
        }

        .filter-container.postback-freeze .custom-dropdown {
            transform: translateZ(0) !important;
            backface-visibility: hidden !important;
            will-change: auto !important;
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
                        <h5 class="card-title">Expense Verification</h5>
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
                                    <div class="custom-dropdown" data-dropdown="region">
                                        <input type="text" class="dropdown-input" placeholder="Search region..." readonly />
                                        <span class="dropdown-arrow">&#9660;</span>
                                        <div class="dropdown-options"></div>
                                    </div>
                                    <asp:DropDownList ID="ddlRegion" runat="server" CssClass="hidden-dropdown">
                                    </asp:DropDownList>
                                </div>

                                <div class="filter-group">
                                    <label class="filter-label" for="ddlBranch">Branch Name</label>
                                    <div class="custom-dropdown" data-dropdown="branch">
                                        <input type="text" class="dropdown-input" placeholder="Search branch..." readonly />
                                        <span class="dropdown-arrow">&#9660;</span>
                                        <div class="dropdown-options"></div>
                                    </div>
                                    <asp:DropDownList ID="ddlBranch" runat="server" CssClass="hidden-dropdown">
                                    </asp:DropDownList>
                                </div>

                                <div class="filter-group">
                                    <label class="filter-label" for="ddlEmployee">Employee Name</label>
                                    <div class="custom-dropdown" data-dropdown="employee">
                                        <input type="text" class="dropdown-input" placeholder="Search employee..." readonly />
                                        <span class="dropdown-arrow">&#9660;</span>
                                        <div class="dropdown-options"></div>
                                    </div>
                                    <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="hidden-dropdown">
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
                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                <asp:TemplateField HeaderText="Customer Name(s)">
                    <ItemTemplate>
                        <asp:Label ID="lblCustomerNames" runat="server" 
                            Text='<%# GetCustomerNamesFromIds(Convert.ToString(Eval("CustomerId"))) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Advance" HeaderText="Advance" />
                <asp:BoundField DataField="ClaimedAmount" HeaderText="Total" />
                <asp:BoundField DataField="FormattedFromDate" HeaderText="Service Date" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnVerify" runat="server"
                            Text='<%# Eval("ExpenseStatus") != null && Eval("ExpenseStatus").ToString() == "3" ? "View Details" : "Click here to Verify" %>'
                            CommandName="Verify" 
                            CommandArgument='<%# Eval("ServiceId") %>'
                            CssClass='<%# "btn btn-primary custom-button center-button" + (Eval("ExpenseStatus") != null && Eval("ExpenseStatus").ToString() == "3" ? " verified-btn" : "") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

                                <asp:Panel ID="verificationForm" runat="server" Visible="false">
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

<!-- jQuery -->
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

<script type="text/javascript">
    $(document).ready(function () {
        // Initialize custom dropdowns
        initializeCustomDropdowns();
    });

    function initializeCustomDropdowns() {
        // Sync custom dropdowns with ASP.NET dropdowns
        syncDropdownData('region', '<%= ddlRegion.ClientID %>');
        syncDropdownData('branch', '<%= ddlBranch.ClientID %>');
        syncDropdownData('employee', '<%= ddlEmployee.ClientID %>');
        
        // Bind events only once
        if (!window.dropdownEventsInitialized) {
            bindDropdownEvents();
            window.dropdownEventsInitialized = true;
        }
    }

    function syncDropdownData(dropdownType, aspDropdownId) {
        var $aspDropdown = $('#' + aspDropdownId);
        var $customDropdown = $('[data-dropdown="' + dropdownType + '"]');
        var $dropdownOptions = $customDropdown.find('.dropdown-options');
        var $input = $customDropdown.find('.dropdown-input');

        // Build options from ASP.NET dropdown
        var options = [];
        $aspDropdown.find('option').each(function () {
            options.push({
                value: $(this).val(),
                text: $(this).text()
            });
        });

        // Update custom dropdown options
        $dropdownOptions.hide().empty();
        var selectedValue = $aspDropdown.val();

        options.forEach(function (option) {
            var $option = $('<div class="dropdown-option"></div>')
                .text(option.text)
                .attr('data-value', option.value);
            if (option.value === selectedValue) {
                $option.addClass('selected');
            }
            $dropdownOptions.append($option);
        });

        // Update input display
        var selectedText = $aspDropdown.find('option:selected').text();
        $input.val(selectedText).attr('data-value', selectedValue);
        $customDropdown.data('all-options', options);
    }

    function bindDropdownEvents() {
        // Click on input to open dropdown
        $(document).off('click', '.dropdown-input').on('click', '.dropdown-input', function(e) {
            e.stopPropagation();
            
            var $dropdown = $(this).closest('.custom-dropdown');
            var $options = $dropdown.find('.dropdown-options');
            var $arrow = $dropdown.find('.dropdown-arrow');
            
            // Close other dropdowns
            $('.dropdown-options').not($options).hide();
            $('.dropdown-arrow').not($arrow).removeClass('open');
            $('.dropdown-input').not($(this)).attr('readonly', true);
            $('.custom-dropdown').not($dropdown).removeClass('open');
            
            // Toggle current dropdown
            if ($options.is(':visible')) {
                $options.hide();
                $arrow.removeClass('open');
                $dropdown.removeClass('open');
                $(this).attr('readonly', true);
            } else {
                $options.show();
                $arrow.addClass('open');
                $dropdown.addClass('open');
                $(this).removeAttr('readonly').focus();
                showAllOptions($dropdown);
            }
        });

        // Search as user types
        $(document).off('input', '.dropdown-input').on('input', '.dropdown-input', function() {
            var $dropdown = $(this).closest('.custom-dropdown');
            var searchTerm = $(this).val().toLowerCase();
            
            if (searchTerm === '') {
                showAllOptions($dropdown);
            } else {
                filterOptions($dropdown, searchTerm);
            }
        });

        // Select option
        $(document).off('click', '.dropdown-option').on('click', '.dropdown-option', function(e) {
            e.stopPropagation();
            var $option = $(this);
            var $dropdown = $option.closest('.custom-dropdown');
            var dropdownType = $dropdown.attr('data-dropdown');
            var value = $option.attr('data-value');
            var text = $option.text();
            
            // Update custom dropdown
            var $input = $dropdown.find('.dropdown-input');
            $input.val(text).attr('data-value', value);
            $dropdown.find('.dropdown-option').removeClass('selected');
            $option.addClass('selected');
            
            // Update ASP.NET dropdown
            var aspDropdownId = getAspDropdownId(dropdownType);
            $('#' + aspDropdownId).val(value);
            
            // Close dropdown
            $dropdown.find('.dropdown-options').hide();
            $dropdown.find('.dropdown-arrow').removeClass('open');
            $dropdown.removeClass('open');
            $input.attr('readonly', true);
            
            // Handle cascading without postback
            if (dropdownType === 'region') {
                loadBranchesAjax(value);
            } else if (dropdownType === 'branch') {
                var selectedRegion = $('[data-dropdown="region"] .dropdown-input').attr('data-value') || 'All';
                loadEmployeesAjax(value, selectedRegion);
            }
        });

        // Close dropdown when clicking outside
        $(document).off('click.dropdown').on('click.dropdown', function(e) {
            if (!$(e.target).closest('.custom-dropdown').length) {
                $('.custom-dropdown').each(function() {
                    $(this).find('.dropdown-options').hide();
                    $(this).find('.dropdown-arrow').removeClass('open');
                    $(this).removeClass('open');
                    $(this).find('.dropdown-input').attr('readonly', true);
                });
            }
        });
    }

    function loadBranchesAjax(regionValue) {
        // Show loading indicator
        var $branchDropdown = $('[data-dropdown="branch"]');
        var $branchArrow = $branchDropdown.find('.dropdown-arrow');
        $branchArrow.html('⟳').css('animation', 'spin 1s linear infinite');

        $.ajax({
            type: "POST",
            url: "AdminPage.aspx/GetBranches",
            data: JSON.stringify({ regionName: regionValue }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(response) {
                // Update branch dropdown
                updateDropdownOptions('branch', response.d);
                
                // Reset employee dropdown to "All"
                updateDropdownOptions('employee', [{ value: 'All', text: 'All' }]);
                
                // Remove loading indicator
                $branchArrow.html('&#9660;').css('animation', '');
            },
            error: function() {
                console.error('Error loading branches');
                $branchArrow.html('&#9660;').css('animation', '');
            }
        });
    }

    function loadEmployeesAjax(branchValue, regionValue) {
        // Show loading indicator
        var $employeeDropdown = $('[data-dropdown="employee"]');
        var $employeeArrow = $employeeDropdown.find('.dropdown-arrow');
        $employeeArrow.html('⟳').css('animation', 'spin 1s linear infinite');

        $.ajax({
            type: "POST",
            url: "AdminPage.aspx/GetEmployees",
            data: JSON.stringify({ branchName: branchValue, regionName: regionValue }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(response) {
                // Update employee dropdown
                updateDropdownOptions('employee', response.d);
                
                // Remove loading indicator
                $employeeArrow.html('&#9660;').css('animation', '');
            },
            error: function() {
                console.error('Error loading employees');
                $employeeArrow.html('&#9660;').css('animation', '');
            }
        });
    }

    function updateDropdownOptions(dropdownType, options) {
        var $customDropdown = $('[data-dropdown="' + dropdownType + '"]');
        var $aspDropdown = $('#' + getAspDropdownId(dropdownType));
        var $dropdownOptions = $customDropdown.find('.dropdown-options');
        var $input = $customDropdown.find('.dropdown-input');

        // Clear existing options
        $aspDropdown.empty();
        $dropdownOptions.empty();

        // Add new options
        options.forEach(function(option) {
            // Add to ASP.NET dropdown
            $aspDropdown.append(new Option(option.text, option.value));
            
            // Add to custom dropdown
            var $option = $('<div class="dropdown-option"></div>')
                .text(option.text)
                .attr('data-value', option.value);
            $dropdownOptions.append($option);
        });

        // Set default selection to "All"
        $aspDropdown.val('All');
        $input.val('All').attr('data-value', 'All');
        $dropdownOptions.find('[data-value="All"]').addClass('selected');
        
        // Store options for search functionality
        $customDropdown.data('all-options', options);
    }

    function showAllOptions($dropdown) {
        var allOptions = $dropdown.data('all-options') || [];
        var $options = $dropdown.find('.dropdown-options');
        var selectedValue = $dropdown.find('.dropdown-input').attr('data-value');
        
        $options.empty();
        
        allOptions.forEach(function(option) {
            var $optionDiv = $('<div class="dropdown-option"></div>')
                .text(option.text)
                .attr('data-value', option.value);
                
            if (option.value === selectedValue) {
                $optionDiv.addClass('selected');
            }
            
            $options.append($optionDiv);
        });
    }

    function filterOptions($dropdown, searchTerm) {
        var allOptions = $dropdown.data('all-options') || [];
        var $options = $dropdown.find('.dropdown-options');
        var selectedValue = $dropdown.find('.dropdown-input').attr('data-value');
        var hasResults = false;
        
        $options.empty();
        
        allOptions.forEach(function(option) {
            if (option.text.toLowerCase().includes(searchTerm)) {
                var $optionDiv = $('<div class="dropdown-option"></div>')
                    .text(option.text)
                    .attr('data-value', option.value);
                    
                if (option.value === selectedValue) {
                    $optionDiv.addClass('selected');
                }
                
                $options.append($optionDiv);
                hasResults = true;
            }
        });
        
        if (!hasResults) {
            $options.append('<div class="no-results">No results found</div>');
        }
    }

    function getAspDropdownId(dropdownType) {
        switch(dropdownType) {
            case 'region':
                return '<%= ddlRegion.ClientID %>';
            case 'branch':
                return '<%= ddlBranch.ClientID %>';
            case 'employee':
                return '<%= ddlEmployee.ClientID %>';
            default:
                return '';
        }
    }
</script>

</asp:Content>