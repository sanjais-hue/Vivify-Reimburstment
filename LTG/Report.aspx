<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="Vivify.Report" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Expense Report</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
           
            <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="Gridview">
                <Columns>
                    <asp:BoundField DataField="ExpenseType" HeaderText="Expense Type" />
                    <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                    <asp:BoundField DataField="OverallLocalAmount" HeaderText="Overall Local Amount" />
                    <asp:BoundField DataField="OverallTourAmount" HeaderText="Overall Tour Amount" />
                    <asp:BoundField DataField="TotalDepartmentExpenses" HeaderText="Total Department Expenses" />
                    <asp:BoundField DataField="TotalRefreshmentAmount" HeaderText="Total Refreshment Amount" />
                </Columns>
            </asp:GridView>
        </div>
         <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" OnClick="btnGenerateReport_Click" />
 <asp:Button ID="btnExportToExcel" runat="server" Text="Export to Excel" OnClick="btnExportToExcel_Click" />
    </form>
</body>
</html>
