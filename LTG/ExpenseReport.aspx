<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpenseReport.aspx.cs" Inherits="ExpenseReport" %>

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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Expense Report</h1>
        <div>
            <asp:GridView ID="gvExpenseReport" runat="server" AutoGenerateColumns="False" CssClass="grid-view">
                <Columns>
                    <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                    <asp:BoundField DataField="TotalLocal" HeaderText="Total Local" />
                    <asp:BoundField DataField="TotalTour" HeaderText="Total Tour" />
                    <asp:BoundField DataField="OverallAmount" HeaderText="Overall Amount" />
                </Columns>
                 <FooterStyle BackColor="#CCCCCC" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
