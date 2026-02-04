<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Training_Report.aspx.cs" Inherits="Vivify.Training_Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            display: flex;
            flex-direction: column;
            min-height: 100vh; /* Ensures the body takes full height */
        }

        h1 {
            color: #333;
            text-align: center;
            margin-bottom: 20px;
        }

        .HeaderStyle {
            width: 100%;
        }

        .grid-container {
            max-height: 500px;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: white;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        .gridview {
            width: 100%;
            border-spacing: 0;
            margin: 0;
        }

        .gridview th, .gridview td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: center;
        }

        .gridview th {
            background-color: darkblue;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .gridview tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .gridview tr:hover {
            background-color: #f1f1f1;
        }

        .border-right {
            border-right: 4px solid #ddd;
        }

        #btnGenerate {
            display: block;
            margin: 20px auto;
            padding: 10px 20px;
            font-size: 16px;
            background-color: #3f418d;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.3s;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
        }

        #btnGenerate:hover {
            background-color: #3f418d;
        }

        .label {
            display: block;
            margin-bottom: 5px;
            color: black;
            font-weight: bold;
        }

        .content {
            padding: 20px;
            background-color: #cadcfc;
            min-height: calc(100vh - 40px); /* Ensures content fills available height minus footer */
            padding-bottom: 50px; /* To ensure content doesn't overlap footer */
            box-sizing: border-box; /* Includes padding in height calculation */
        }

        .footer {
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
            background-color: rgb(249, 243, 243); /* Footer background color */
            text-align: center; /* Center footer text */
            padding: 10px; /* Padding for footer */
            color: ghostwhite; /* Footer text color */
            z-index: 1000; /* Ensure it's above other content */
        }

        .footer a {
            color: midnightblue; /* Link color in footer */
            text-decoration: none; /* Remove underline from links */
        }

        .footer a:hover {
            text-decoration: underline; /* Underline on hover */
        }

        main {
            flex-grow: 1; /* This ensures the content grows and pushes footer down */
        }

        .label-container {
            display: flex;
            justify-content: space-between;
            gap: 20px;
            flex-wrap: wrap;
        }

        .label-container .label-group {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
        }

        .label-container .label-group label {
            margin-bottom: 5px;
        }

        .label-container .label-group select,
        .label-container .label-group input {
            width: 200px; /* Adjust width for consistency */
        }

        /* Additional styles for grid */
        .date-column {
            white-space: nowrap;
            width: 150px;
        }

        .particulars-column {
            white-space: nowrap;
            width: 250px;
        }
        .label{
            margin:10px;
        }
    </style>

       <aside id="sidebar" class="sidebar" style="box-shadow: 0 2px 10px darkblue;">

   <ul class="sidebar-nav" id="sidebar-nav">
      <%-- <li class="nav-item">
    <a class="nav-link" href="Dashboard.aspx">
        <i class="bi bi-grid"></i>
        <span>Dashboard</span>
    </a>
</li>--%>
            
              <%--<li class="nav-item">
    <a class="nav-link" href="AdminPage.aspx">
       <i class="bi bi-pc-display"></i>
        <span>Expense Page</span>
    </a>
</li>
                    
  
          <li class="nav-item">
  <a class="nav-link " href="Employeecreation.aspx">
      <i class="bi bi-personbi bi-person-circle"></i><span>Employee Creation</span>
  </a>
</li>
                   
        
      <li class="nav-item">
    
            <a class="nav-link " href="AdminCustomer_Creation.aspx">
               <i class="bi bi-person-workspace"></i><span>Customer Creation</span>
            </a>
          </li>

                          <li class="nav-item">
  <a class="nav-link " href="AdminService_Assign.aspx">
      <i class="bi bi-diagram-3"></i><span>Service Assignment</span>
  </a>
</li>
       
                                 <li class="nav-item">
  <a class="nav-link " href="Reportform.aspx">
      <i class="bi bi-filetype-exe"></i><span>Expense Report</span>
  </a>

</li>

                                 <li class="nav-item">
  <a class="nav-link " href="CombinedReport.aspx">
      <i class="bi bi-folder-fill"></i><span>Combined Report</span>
  </a>

</li>
       
                <li class="nav-item">
  <a class="nav-link " href="DocView.aspx">
      <i class="bi bi-file-earmark-pdf-fill"></i><span> Attachment</span>
  </a>

</li>--%>
                            <li class="nav-item">
    <a class="nav-link" href="AdminTraining.aspx">
        <i class="bi bi-person-rolodex"></i>
        <span>Training Page</span>
    </a>
</li>
              <li class="nav-item">
    <a class="nav-link" href="Admin_Training_Assign.aspx">
        <i class="bi bi-person-plus-fill"></i>
        <span>Training Assignment</span>
    </a>
</li>     
                     <li class="nav-item">
    <a class="nav-link" href="Training_Report.aspx">
        <i class="bi bi-file-spreadsheet"></i>
        <span>Training Report</span>
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
                            <h5 class="card-title" style="text-align:center;background-color:#3f418d;color:white">Training Report</h5>
                            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                            <div class="label">
                            <div class="label-container">
                                <!-- Left side container for Date fields -->
                                <div class="label-group">
                                    <asp:Label ID="lblFromDate" runat="server" Text="From Date:"></asp:Label>
                                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="date"></asp:TextBox>
                                </div>

                                <div class="label-group">
                                    <asp:Label ID="lblToDate" runat="server" Text="To Date:"></asp:Label>
                                    <asp:TextBox ID="txtToDate" runat="server" TextMode="date"></asp:TextBox>
                                </div>

                                <!-- Right side container for Branch and Employee -->
                                <div class="label-group">
                                    <asp:Label ID="BranchName" runat="server" Text="Branch Name:"></asp:Label>
                                    <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged"></asp:DropDownList>
                                </div>

                                <div class="label-group">
                                    <asp:Label ID="lblEmployeeName" runat="server" Text="Employee Name:"></asp:Label>
                                    <asp:DropDownList ID="ddlEmployeeName" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" style="background-color:darkblue; color:white; padding: 5px 15px; margin-top:15px;width:76px;" />
                              </div> 
                            
                                <div class="grid-container">
                                    <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="gridview">
                                        <HeaderStyle BackColor="#4CAF50" ForeColor="White" />
                                        <Columns>
                                            <asp:BoundField DataField="EngineerName" HeaderText="Engineer Name" ItemStyle-Width="150px" />
                                            <asp:BoundField DataField="FromDate" HeaderText="From Date" DataFormatString="{0:dd-MMM-yyyy}" SortExpression="FromDate" ItemStyle-CssClass="no-wrap" HeaderStyle-Width="120px" ItemStyle-Width="120px" />
                                            <asp:BoundField DataField="ToDate" HeaderText="To Date" DataFormatString="{0:dd-MMM-yyyy}" SortExpression="ToDate" ItemStyle-CssClass="no-wrap" HeaderStyle-Width="120px" ItemStyle-Width="120px" />
                                            <asp:BoundField DataField="FromTime" HeaderText="From Time" DataFormatString="{0:hh\:mm}" SortExpression="FromTime" ItemStyle-CssClass="no-wrap" HeaderStyle-Width="250px" ItemStyle-Width="250px" />
                                            <asp:BoundField DataField="ToTime" HeaderText="To Time" DataFormatString="{0:hh\:mm}" SortExpression="ToTime" ItemStyle-CssClass="no-wrap" HeaderStyle-Width="250px" ItemStyle-Width="250px" />
                                            <asp:BoundField DataField="Particulars" HeaderText="Particulars" ItemStyle-Width="250px" SortExpression="ConvParticulars" NullDisplayText=" " />
                                            <asp:BoundField DataField="Distance" HeaderText="Distance" ItemStyle-Width="100px" SortExpression="Distance" NullDisplayText=" " />
                                            <asp:BoundField DataField="ModeOfTransport" HeaderText="Mode Of Transport" ItemStyle-Width="150px" />
                                            <asp:BoundField DataField="ConveyanceAmount" HeaderText="Conveyance" ItemStyle-Width="150px" SortExpression="ConvAmount" />
                                            <asp:BoundField DataField="FoodAmount" HeaderText="Food" ItemStyle-Width="150px" SortExpression="FoodAmount" />
                                            <asp:BoundField DataField="" HeaderText="Lodging" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="" HeaderText="Others" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="" HeaderText="Misc" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="Total" HeaderText="Total Amount" ItemStyle-Width="150px" />
                                            <asp:BoundField DataField="" HeaderText="Nature of Work" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="" HeaderText="SAP Contract Number/ SO Number" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="" HeaderText="SMO/SO/WBS" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="" HeaderText="Document Reference" SortExpression="" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="TrainingDetails" HeaderText="Remarks" SortExpression="TrainingDetails" ItemStyle-Width="150px" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <asp:Button ID="btnGenerate" runat="server" Text="Generate Excel" OnClick="btnGenerate_Click" style="background-color:darkblue; color:white;width:130px;margin:10px;" />
                      
                        </div>
                    </div>
                </div>
             
            </section>
        </main>
    </div>

</asp:Content>