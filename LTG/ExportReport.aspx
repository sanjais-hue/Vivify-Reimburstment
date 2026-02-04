<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportReport.aspx.cs" Inherits="Vivify.ExportReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Expense Report</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }
        h1 {
            text-align: center;
            color: #333;
        }
        #form1 {
            max-width: 800px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }
        .grid-view {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        .grid-view th, .grid-view td {
            padding: 10px;
            text-align: center;
            border-bottom: 1px solid #ddd;
        }
        .grid-view th {
            background-color: #4CAF50;
            color: white;
        }
        .grid-view tr:hover {
            background-color: #f1f1f1;
        }
        .grid-view .total-row {
            font-weight: bold;
            background-color: #e0ffe0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Expense Report</h1>
        <div>
            <asp:GridView ID="gvExpenseReport" runat="server" AutoGenerateColumns="False" CssClass="grid-view" ShowFooter="True" OnRowDataBound="gvExpenseReport_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Expense Type">
                        <ItemTemplate>
                            <%# Eval("ExpenseType") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TotalConveyance" HeaderText="Total Conveyance" />
                    <asp:BoundField DataField="TotalFood" HeaderText="Total Food" />
                    <asp:BoundField DataField="TotalOthers" HeaderText="Total Others" />
                    <asp:BoundField DataField="TotalMiscellaneous" HeaderText="Total Miscellaneous" />
                    <asp:BoundField DataField="TotalLodging" HeaderText="Total Lodging" />
                    <asp:BoundField DataField="TotalRefreshment" HeaderText="Total Refreshment" />
                    <asp:BoundField DataField="OverallLocalAmount" HeaderText="Overall Amount" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
