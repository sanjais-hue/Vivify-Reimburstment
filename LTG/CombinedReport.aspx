
<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="CombinedReport.aspx.cs" Inherits="Vivify.CombinedReport" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .mydatagrid th, .mydatagrid td {
            border: 1px solid black;
            padding: 8px;
            margin: 0;
            width:200px;
            align-items:center;
             margin-bottom: 20px; 
        }
        .mydatagrid {
    width: 100%;
    border-collapse: collapse;
    margin: 0;
    padding: 0;
}
        .mydatagrid td{
            background-color:white;
        }
        .mydatagrid th {
            background-color: #3f418d;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
        }
        .scrollable-container {
            max-height: 400px;
            overflow-x: auto;
            overflow-y: auto;
            border: 0px solid #ccc;
            margin-bottom: 10px;
            width:400px;
                -webkit-overflow-scrolling: touch; /* Smooth scrolling for mobile devices */

        }
            .toggle-sidebar-btn {
     font-size: 40px;
     color:midnightblue;
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

.sidebar-nav .nav-link i {
    font-size: 14px; /* Smaller icons */
    margin-right: 6px; /* Reduced spacing */
}

.sidebar-nav .nav-link span {
    white-space: nowrap; /* Prevent text wrapping */
    overflow: hidden;
    text-overflow: ellipsis; /* Add ellipsis if text is too long */
}

 .sidebar-nav .nav-link:hover {
        color: red;
        transition: background 0.3s, color 0.3s, border 0.3s; 
        border : 2px solid #222b65;
        box-shadow: 0 2px 10px #1f2b60;
    }

.sidebar-nav .nav-link.active {
    background-color: #222b65;
    color: white;
}

.sidebar-nav .nav-item {
    margin-bottom: 6px; /* Further reduced space */
}

.sidebar-nav {
    padding: 0;
    margin: 0;
    list-style: none;
}
                    .footer {
        background-color:rgb(249, 243, 243);
        text-align: center;
        padding:10px;
        color: ghostwhite;
        margin:0px;
    }
    
    .footer a {
        color: midnightblue;
        text-decoration: none;
    }
    
    .footer a:hover {
        text-decoration: underline;
    }

    .btn-primary{
        background-color:#3f418d;
    }
     .btn-primary:hover{
     background-color:#3f418d;
 }

/* Adjust main content area to accommodate smaller sidebar */
.main {
    margin-left: 200px; /* Match sidebar width */
}

/* For mobile responsiveness */
@media (max-width: 768px) {
    .sidebar {
        width: 180px; /* Even smaller on mobile */
        min-width: 180px;
        max-width: 180px;
    }
    
    .main {
        margin-left: 180px;
    }
    
    .sidebar-nav .nav-link {
        font-size: 11px;
        padding: 6px 10px;
    }
    
    .sidebar-nav .nav-link i {
        font-size: 13px;
    }
}
        .content {
            margin-left: 10px;
            padding: 20px;
            background-color: #cadcfc;
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
  /*.blue-background {
            background-color: #007bff;*/ /* Blue background */
            /*color: white;*/ /* White text for contrast */
            /*border: 3px solid #000000;*/ /* Black border */
        /*}*/
.local-tour-border {
    border: 3px solid #FF5733;  /* Orange border for Local & Tour Total */
    background-color: #007bff;  /* Blue background */
    color: white;  /* Text color to white to make it stand out on the blue background */
}

.expense-total-border {
    border: 3px solid #28a745;  /* Green border for Expense Total */
    background-color: #007bff;  /* Blue background */
    color: white;  /* Text color to white */
}

.department-total-border {
    border: 3px solid #007bff;  /* Blue border for Department Total */
    background-color: #007bff;  /* Blue background */
    color: white;  /* Text color to white */
}

 .label {
    display: block;
    margin-bottom: 5px;
    color: black;
    font-weight: bold;
}

@media (max-width: 767px) {
    .d-flex {
        flex-direction: column;
    }
    .col-12 {
        width: 100%;
    }
}

         .content {
    margin-left: 10px; /* Adjust margin to fit sidebar */
    padding: 20px;
    background-color: #cadcfc;
    min-height: calc(100vh - 40px); /* Ensures content fills available height minus footer */
    padding-bottom: 50px; /* To ensure content doesn't overlap footer */
    box-sizing: border-box; /* Includes padding in height calculation */
    flex: 1;
}
         .grid-container {
    margin-bottom: 20px;
}

/* Mobile responsiveness */
        @media (max-width: 768px) {
            .mobile-card {
                display: flex;
                flex-direction: column;
                background-color: #ffffff;
                border: 1px solid #ccc;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                padding: 15px;
                margin-bottom: 20px;
            }
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
                            <h5 class="card-title" style="text-align:center;background-color:#3f418d;color:white">Combined Report</h5>
                            <section class="form-container section error-404 d-flex flex-column align-items-center justify-content-center" >
                                 <div class="label">
                                       <asp:Label ID="lblRegion" runat="server" Text="Region:"></asp:Label>
    <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true" 
    OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
</asp:DropDownList>
                            <asp:Label ID="BranchName" runat="server" Text="Branch Name:"></asp:Label>
                            <asp:DropDownList ID="ddlBranch" runat="server"></asp:DropDownList>
                             <asp:Label ID="lblFromDate" runat="server" Text="From Date:" ></asp:Label>
                             <asp:TextBox ID="txtFromDate" runat="server" TextMode="date"></asp:TextBox>
                             <asp:Label ID="lblToDate" runat="server" Text="To Date:"></asp:Label>
                             <asp:TextBox ID="txtToDate" runat="server" TextMode="date"></asp:TextBox>
                             
                             <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click" style="background-color:#3f418d;color:white; padding: 5px 15px;"/>
                         </div>
                                <br />

                              
                                <h2  style="margin-top:10px;text-align:center">Summary Report</h2>
                                  <div class="scrollable-container" style="width: 75%; overflow-x: auto;">
                                <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                              
                                    <asp:GridView ID="gvExpenseReport" runat="server" AutoGenerateColumns="False" CssClass="mydatagrid" >
    <Columns>
        <asp:BoundField DataField="EngineerName" HeaderText="Engineer Name" SortExpression="EngineerName" />
        <asp:BoundField DataField="LocalExpenses" HeaderText="LocalExpenses" />
        <asp:BoundField DataField="TourExpenses" HeaderText="TourExpenses" />
                <asp:BoundField DataField="RefreshmentAmount" HeaderText="Refreshment Amount" />

        <asp:BoundField DataField="OverallExpenses" HeaderText=" TotalExpenses" />
    </Columns>
</asp:GridView>

                                </div>
                                  

                               <%-- <h2  style="margin-top:10px;">SMO Report</h2>--%>
                                <%--<h2 style="margin-top:10px;">SMO Summary Report</h2>--%>
<div class="grid-container mobile-card">
    <div class="scrollable-container" style="width: 100%; overflow-x: auto;">
        <asp:GridView ID="gvSmoReport" runat="server" AutoGenerateColumns="False" CssClass="mydatagrid">
            <Columns>
                <asp:BoundField DataField="SmoNo" HeaderText="SMO Number" />
                <asp:BoundField DataField="TotalClaimedAmount" HeaderText="Expenses" DataFormatString="{0:F2}" />
            </Columns>
        </asp:GridView>
    </div>
</div>

<%--<h2 style="margin-top:10px;">SO Summary Report</h2>--%>
<div class="grid-container mobile-card">
    <div class="scrollable-container" style="width: 100%; overflow-x: auto;">
        <asp:GridView ID="gvSoReport" runat="server" AutoGenerateColumns="False" CssClass="mydatagrid">
            <Columns>
                <asp:BoundField DataField="SoNo" HeaderText="SO Number" />
                <asp:BoundField DataField="TotalClaimedAmount" HeaderText="Expenses" DataFormatString="{0:F2}" />
            </Columns>
        </asp:GridView>
    </div>
</div>
                          <%--  <h2  style="margin-top:10px;">Export Report</h2>--%>
                         
<%--<h2  style="margin-top:10px;" >Local & Tour Summary</h2>--%>
                                <div class="grid-container mobile-card">
<asp:GridView ID="gvLocalTour" runat="server" AutoGenerateColumns="false" CssClass="mydatagrid">
    <Columns>
        <asp:BoundField DataField="ExpenseCategory" HeaderText="Expense Type" />
        <asp:BoundField DataField="TotalAmount" HeaderText="Expenses"  />
    </Columns>
</asp:GridView>
</div>

<%--<h2  style="margin-top:10px;">Expense Categories</h2>--%>
                                <div class="grid-container mobile-card">
<asp:GridView ID="gvExpenseCategories" runat="server" AutoGenerateColumns="false" CssClass="mydatagrid ">
    <Columns>
        <asp:BoundField DataField="ExpenseCategory" HeaderText="Expense Type" SortExpression="ExpenseCategory" />
        <asp:BoundField DataField="TotalAmount" HeaderText="Expenses" SortExpression="TotalAmount" />
    </Columns>
</asp:GridView>
                                    </div>


<%--<h2  style="margin-top:10px;">Department Totals</h2>--%>
<asp:GridView ID="gvDepartmentTotals" runat="server" AutoGenerateColumns="false" CssClass="mydatagrid ">
    <Columns>
        
        <asp:BoundField DataField="Department" HeaderText="Department" SortExpression="Department" />
        
        
        <asp:BoundField DataField="TotalAmount" HeaderText="Expenses" SortExpression="TotalAmount" 
            HtmlEncode="false" />
    </Columns>
</asp:GridView>

                        </section>
                    </div>
                </div>
            </div>
        </section>

        <asp:Button ID="btnGenerate" runat="server" Text="Generate Excel" OnClick="btnGenerate_Click" CssClass="btn-primary" style="background-color:#3f418d; color:white;"/>
    </main>
</div>

<%--<footer class="footer">
    <p>&copy; 2024 Your Company. All rights reserved.</p>
</footer>--%>
</asp:Content>