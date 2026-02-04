<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Refreshdash.aspx.cs" Inherits="Vivify.Refreshdash" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main">
        <style>
            .mydatagrid {
                table-layout: fixed; /* Enforces consistent column widths */
                width: 100%; /* Ensures full usage of available space */
            }

            .mydatagrid th, .mydatagrid td {
                border: 1.5px solid black;
                padding: 12px;
                white-space: nowrap; /* Prevent wrapping */
                text-align:center;
            }

            .mydatagrid td {
                background-color: white;
            }

            .mydatagrid th {
                background-color: #3f418d;
                color: white;
                position: sticky;
                top: 0;
                z-index: 10;
                text-align: center;
            }

            .rows:hover {
                background-color: #f1f1f1;
            }

            .scrollable-container {
                max-height: 400px; /* Set max height for vertical scrolling */
                overflow-y: auto; /* Enable vertical scroll */
                overflow-x: auto; /* Enable horizontal scroll */
                border: 1px solid #1f2b60;
                box-shadow: 0 2px 10px darkblue;
                margin: 0 auto;
                width: 95%; /* Set container width */
                max-width: 1200px; /* Maximum width for the grid */
            }

            .btnReimburse {
                background-color: #3f418d;
                color: white;
                border: none;
                padding: 8px 16px;
                cursor: pointer;
                border-radius: 4px;
                transition: background-color 0.3s, transform 0.2s;
            }

            .main {
                margin: 0;
                padding: 0;
                background-color: #cadcfc;
                height: 85vh;
                display: flex;
                justify-content: center;
                align-items: center;
            }

            .custom-grid {
                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
                border-radius: 8px;
                overflow: hidden;
                min-width: 800px; /* Minimum width to enforce scrolling */
            }
        </style>

        <section class="scrollable-container">
            <div class="custom-grid">
               <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CellPadding="4" CellSpacing="0" GridLines="None"
              Width="100%" CssClass="mydatagrid" PagerStyle-CssClass="pager"
              RowStyle-CssClass="rows" HeaderStyle-CssClass="header"
              style="border: 1.5px solid red; border-collapse: collapse; font-size:14px; line-height:20px; box-shadow:0 4px 15px rgba(0, 0, 0, 0.2);"
              OnRowCommand="GridView1_RowCommand" >
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

       
        <asp:TemplateField>
            <ItemTemplate>
                <div style="text-align: center;">
                    <asp:Button ID="btnReimburse" runat="server" CommandName="Reimburse" Text="Assign Refreshment"
                                CommandArgument='<%# Container.DataItemIndex %>' 
                                CssClass="btn btn-primary custom-button" />
                </div>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
            </div>
        </section>
    </main>
</asp:Content>