<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Vivify.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main">
        <style>
            /* Base Styles */
            .main {
                margin: 0;
                padding: 10px;
                background-color: #cadcfc;
                min-height: 85vh;
                overflow-x: hidden;
            }

            /* Card Styling */
            .card {
                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
                border-radius: 10px;
                background-color: #ffffff;
                margin: 15px auto;
                padding: 10px;
                width: 100%;
                max-width: 1200px;
            }

            .card-header {
                text-align: center;
                background-color: #3f418d;
                color: ghostwhite;
                padding: 12px;
                font-size: 18px;
                border-radius: 5px 5px 0 0;
                font-weight: bold;
            }

            /* GridView Styling */
            .mydatagrid {
                width: 100%;
                border-collapse: collapse;
                font-size: 14px;
            }

            .mydatagrid th,
            .mydatagrid td {
                border: 1.5px solid black;
                padding: 8px;
                text-align: left;
                word-wrap: break-word;
            }

            .header {
                background-color: #3f418d;
                font-weight: bold;
                color: ghostwhite;
                text-align: center;
                position: sticky;
                top: 0;
                z-index: 10;
            }

            .rows {
                background-color: #ffffff;
            }

            .rows:nth-child(even) {
                background-color: #f8f9fa;
            }

            /* Scrollable Container with Fixed Max Height */
            .scrollable-container {
                overflow-x: auto;
                overflow-y: auto;
                max-height: 400px; /* ✅ FIXED MAX HEIGHT */
                border: 1px solid #1f2b60;
                box-shadow: 0 2px 10px darkblue;
                margin: 15px 0;
                border-radius: 5px;
                position: relative;
            }

            /* Ensure thead stays fixed when scrolling */
            .mydatagrid thead {
                position: sticky;
                top: 0;
                z-index: 10;
            }

            /* Button Styling */
            .custom-button {
                background-color: #3f418d;
                color: white;
                border: none;
                padding: 6px 12px;
                border-radius: 4px;
                font-size: 12px;
                cursor: pointer;
                white-space: nowrap;
                min-width: 120px;
            }

            .custom-button:hover {
                background-color: #2a2c6b;
                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            }

            .delete-button {
                background-color: #dc3545;
                color: white;
                border: none;
                padding: 6px 12px;
                border-radius: 4px;
                font-size: 12px;
                cursor: pointer;
                white-space: nowrap;
                min-width: 80px;
            }

            .delete-button:hover {
                background-color: #c82333;
                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            }

            /* Mobile Responsive Styles */
            @media screen and (max-width: 768px) {
                .main {
                    padding: 5px;
                }

                .card {
                    margin: 10px 0;
                    padding: 8px;
                    border-radius: 8px;
                }

                .card-header {
                    padding: 10px;
                    font-size: 16px;
                }

                .scrollable-container {
                    margin: 10px 0;
                    border-radius: 4px;
                    max-height: 350px; /* ✅ Adjusted for mobile */
                }

                /* Make table responsive */
                .mydatagrid {
                    font-size: 12px;
                    min-width: 600px; /* Minimum width for horizontal scroll */
                }

                .mydatagrid th,
                .mydatagrid td {
                    padding: 6px 4px;
                    font-size: 11px;
                }

                /* Button adjustments for mobile */
                .custom-button,
                .delete-button {
                    padding: 4px 8px;
                    font-size: 10px;
                    min-width: 100px;
                }

                .custom-button {
                    min-width: 110px;
                }
            }

            @media screen and (max-width: 480px) {
                .card-header {
                    font-size: 14px;
                    padding: 8px;
                }

                .scrollable-container {
                    max-height: 300px; /* ✅ Smaller max height for mobile */
                }

                .mydatagrid {
                    font-size: 11px;
                    min-width: 500px;
                }

                .mydatagrid th,
                .mydatagrid td {
                    padding: 4px 3px;
                    font-size: 10px;
                }

                .custom-button,
                .delete-button {
                    padding: 3px 6px;
                    font-size: 9px;
                    min-width: 90px;
                }

                .custom-button {
                    min-width: 100px;
                }
            }

            /* Small mobile devices */
            @media screen and (max-width: 360px) {
                .main {
                    padding: 3px;
                }

                .card {
                    margin: 5px 0;
                    padding: 5px;
                }

                .scrollable-container {
                    max-height: 250px; /* ✅ Even smaller for very small screens */
                }

                .mydatagrid {
                    min-width: 450px;
                }

                .mydatagrid th,
                .mydatagrid td {
                    padding: 3px 2px;
                    font-size: 9px;
                }

                .custom-button,
                .delete-button {
                    padding: 2px 4px;
                    font-size: 8px;
                    min-width: 80px;
                }
            }

            /* Hide serial number on very small screens */
            @media screen and (max-width: 320px) {
                .mydatagrid th:nth-child(1),
                .mydatagrid td:nth-child(1) {
                    display: none;
                }
            }

            /* Print Styles */
            @media print {
                .card {
                    box-shadow: none;
                    border: 1px solid #000;
                }

                .custom-button,
                .delete-button {
                    display: none;
                }

                .scrollable-container {
                    overflow: visible;
                    box-shadow: none;
                    max-height: none; /* ✅ Remove max height for printing */
                }
            }

            /* High contrast for accessibility */
            @media (prefers-contrast: high) {
                .mydatagrid th,
                .mydatagrid td {
                    border: 2px solid #000;
                }

                .header {
                    background-color: #000;
                    color: #fff;
                }
            }

            /* Reduced motion for accessibility */
            @media (prefers-reduced-motion: reduce) {
                .custom-button:hover,
                .delete-button:hover {
                    transition: none;
                    box-shadow: none;
                }
            }
            .verified-button {
    background-color: #28a745; /* Green color for verified */
    color: white;
    border: none;
    padding: 6px 12px;
    border-radius: 4px;
    font-size: 12px;
    cursor: default; /* Optional: no hover effect if it's final state */
    white-space: nowrap;
    min-width: 120px;
}

.verified-button:hover {
    background-color: #218838; /* Slightly darker green on hover */
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}
        </style>

        <!-- First GridView Section - Expense Reimbursement -->
        <div class="card">
            <div class="card-header">
                Expense Reimbursement
            </div>
            <div class="card-body">
                <section class="scrollable-container">
                    <asp:GridView ID="GridView1" runat="server" 
                        AutoGenerateColumns="false" 
                        CellPadding="4" 
                        CellSpacing="0" 
                        GridLines="None"
                        Width="100%" 
                        CssClass="mydatagrid" 
                        PagerStyle-CssClass="pager"
                        RowStyle-CssClass="rows" 
                        HeaderStyle-CssClass="header"
                        OnRowCommand="GridView1_RowCommand" 
                        OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
<asp:BoundField DataField="CustomerNames" HeaderText="Customer Names">
    <ItemStyle HorizontalAlign="Center" />
</asp:BoundField>                            <asp:BoundField DataField="Advance" HeaderText="Advance" />
                            <asp:BoundField DataField="FromDate" HeaderText="Service Date"
                                DataFormatString="{0:dd-MMM-yyyy}" SortExpression="FromDate" />
                            <asp:BoundField DataField="ServiceType" HeaderText="Service Type" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
               <asp:TemplateField HeaderText="Action" ItemStyle-Width="150px">
    <ItemTemplate>
        <asp:HiddenField ID="hdnServiceId" runat="server" Value='<%# Eval("ServiceId") %>' />
        <asp:Button ID="btnReimburse" runat="server" 
            CommandName="Reimburse" 
            Text='<%# 
                Eval("Status").ToString() == "Service Assigned" ? "Proceed To Reimbursement" :
                Eval("Status").ToString() == "Reimbursement Submitted" ? "In Progress" :
                "Verified"
            %>'
            CommandArgument='<%# Container.DataItemIndex %>'
            CssClass='<%# 
                Eval("Status").ToString() == "Verified" ? "verified-button" : "custom-button"
            %>'
            Visible='<%# new[] { "Service Assigned", "Reimbursement Submitted", "Verified" }.Contains(Eval("Status").ToString()) %>' />
    </ItemTemplate>
</asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <asp:Button ID="btnDelete" runat="server" CommandName="DeleteRow" 
                                        Text="Delete" CommandArgument='<%# Eval("ServiceId") %>'
                                        CssClass="delete-button" 
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </section>
            </div>
        </div>

        <!-- Second GridView Section - Employee Refreshment -->
        <div class="card">
            <div class="card-header">
                Employee Refreshment 
            </div>
            <div class="card-body">
                <section class="scrollable-container">
                    <asp:GridView ID="GridView2" runat="server" 
                        AutoGenerateColumns="false" 
                        CellPadding="4" 
                        CellSpacing="0" 
                        GridLines="None"
                        Width="100%" 
                        CssClass="mydatagrid" 
                        PagerStyle-CssClass="pager"
                        RowStyle-CssClass="rows" 
                        HeaderStyle-CssClass="header"
                        OnRowCommand="GridView2_RowCommand" 
                        OnSelectedIndexChanged="GridView2_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField HeaderText="Employee ID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmployeeId" runat="server" Text='<%# Eval("EmployeeId") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="First Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblFirstName" runat="server" Text='<%# Eval("FirstName") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:Button ID="btnReimburse" runat="server" 
                                        CommandName="Reimburse" 
                                        Text="Assign Refreshment"
                                        CommandArgument='<%# Container.DataItemIndex %>' 
                                        CssClass="custom-button" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </section>
            </div>
        </div>

    </main>

    <!-- Add viewport meta tag for mobile responsiveness -->
    <script type="text/javascript">
        // Add viewport meta tag dynamically if not present
        if (!document.querySelector('meta[name="viewport"]')) {
            var meta = document.createElement('meta');
            meta.name = 'viewport';
            meta.content = 'width=device-width, initial-scale=1.0';
            document.getElementsByTagName('head')[0].appendChild(meta);
        }

        // Function to handle responsive behavior
        function adjustLayout() {
            var screenWidth = window.innerWidth;
            var scrollContainers = document.querySelectorAll('.scrollable-container');

            scrollContainers.forEach(function (container) {
                if (screenWidth < 768) {
                    container.style.overflowX = 'auto';
                    container.style.overflowY = 'auto';
                } else {
                    container.style.overflowX = 'auto';
                    container.style.overflowY = 'auto';
                }
            });
        }

        // Call on load and resize
        window.addEventListener('load', adjustLayout);
        window.addEventListener('resize', adjustLayout);
    </script>
</asp:Content>