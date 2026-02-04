<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="WorkOrderdash.aspx.cs" Inherits="Vivify.WorkOrderdash" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main">
        <style>
            .mydatagrid {
                border-collapse: collapse;
                width: 100%;
            }

            .mydatagrid th, .mydatagrid td {
                border: 1.5px solid midnightblue;
                padding: 12px;
                /*transition: background-color 0.3s;*/
            }
            .mydatagrid td {
                background-color:white;
            }
            .mydatagrid th {
                background-color: #3f418d;
                color: white;
                text-align: center;
            }

            .scrollable-container {
                max-height: 390px;
                overflow-y: auto;
                border: 1px solid #1f2b60;
                box-shadow: 0 2px 10px darkblue;
                margin: 0 auto;
                width: 90%;
            }

            .fixed-header {
                position: sticky;
                top: 0;
                z-index: 10;
            }
            .headerText {
    background-color: white; /* Color for the GridView header */
    font-weight: bold;
    color: ghostwhite; /* Text color for the header */
    position: sticky; /* Make header sticky */
    top: 0; /* Stick to the top */
    z-index: 10; /* Ensure it is above other content */
    text-align:center;
}

            .custom-button {
                background-color: #3f418d;
                color: white;
                border: none;
                padding: 8px 16px;
                cursor: pointer;
                border-radius: 4px;
                transition: background-color 0.3s, transform 0.2s;
                margin: 0 auto;
                display: block;
            }

            .custom-button:hover {
                background-color: #2e2f6a;
                transform: scale(1.05);
            }

            .main {
                margin: 0;
                padding: 0;
                background-color: #cadcfc;
                height: 85vh;
                display: flex;
                justify-content: center;
                align-items: center;
                overflow: hidden;
            }

            .custom-grid {
                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
                border-radius: 8px;
                overflow: hidden;
            }
        </style>

        <section class="scrollable-container">
            <div class="custom-grid">
                
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand" CssClass="mydatagrid">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" />
                        <asp:BoundField DataField="EmployeeId" HeaderText="Employee ID" />
                        <asp:TemplateField HeaderText="First Name">
                            <ItemTemplate>
                                <asp:Label ID="lblFirstName" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="FromDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="Advance" HeaderText="Advance" />
                        <asp:BoundField DataField="ServiceType" HeaderText="Service Type" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:TemplateField HeaderText="Actions" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:HiddenField ID="hdnServiceId" runat="server" Value='<%# Eval("ServiceId") %>' />
                                <asp:HiddenField ID="hdnServiceType" runat="server" Value='<%# Eval("ServiceType") %>' />
                                <asp:HiddenField ID="hdnExpenseType" runat="server" Value='<%# Eval("ExpenseType") %>' />
                                <asp:HiddenField ID="hdnSmoNo" runat="server" Value='<%# Eval("SmoNo") %>' />
                                <asp:Button ID="btnReimburse" runat="server" CommandName="Reimburse" 
                                            CommandArgument='<%# Container.DataItemIndex %>' 
                                            Text="Work Order" 
                                            CssClass="custom-button" 
                                            OnClientClick="return confirm('Are you sure you want to proceed?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </section>
    </main>
</asp:Content>
