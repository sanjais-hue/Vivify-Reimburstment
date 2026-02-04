<%@ Page Language="C#" AutoEventWireup="true" Codefile="Dash.aspx.cs" Inherits="Vivify.Dash" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .mydatagrid th, .mydatagrid td {
     border: 1px solid black;
     padding: 8px;
 }
 .header {
     background-color: #f2f2f2;
     font-weight: bold;
 }
 .rows {
     background-color: #ffffff;
 }
 .pager {
     text-align: right;
 }
 .scrollable-container {
     max-height: 300px; /* Set a maximum height */
     overflow-x: auto;  /* Enable horizontal scrolling if needed */
     overflow-y: auto;  /* Enable vertical scrolling if needed */
     border: 1px solid #ccc;
 }
 .main{
     background-color:rgb(179, 241, 248)
 }
    </style>
</head>
<body>


      <section class="section dashboard">
      <div class="row">
          <!-- Right side columns -->
          <div class="col">
              <!-- Recent Activity -->
              <div class="card">
                  <section class="section error-404 d-flex flex-column align-items-center justify-content-center">
                      <h2>WELCOME TO LTG WAREHOUSE MANAGEMENT SYSTEM!</h2>
                      <h2 style="color:#3e92c9;">Let's Get Started?</h2>
                      <img src="assets/img/logo1.jpg" alt="Page Not Found">
                  </section>
              </div><!-- End Recent Activity -->
          </div>
      </div>
  </section>
    <form id="form1" runat="server">
        <div class="container">
           
              <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CellPadding="4" CellSpacing="0" GridLines="None"
      Width="100%" CssClass="mydatagrid" PagerStyle-CssClass="pager"
      RowStyle-CssClass="rows" HeaderStyle-CssClass="header"
      style="border: 1px solid black; border-collapse: collapse; font-size:14px; line-height:12px;"
      OnRowCommand="GridView1_RowCommand" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="ServiceType" HeaderText="Service Type" />
                    <asp:BoundField DataField="ServiceId" HeaderText="Service ID" />
                    <asp:BoundField DataField="ExpenseId" HeaderText="Expense ID" />
                    <asp:BoundField DataField="CustomerId" HeaderText="Customer ID" />
                    <asp:BoundField DataField="EmployeeId" HeaderText="Employee ID" />
                    <asp:BoundField DataField="LocalAmount" HeaderText="LocalAmount" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
