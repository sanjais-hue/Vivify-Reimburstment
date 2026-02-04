<%@ Page Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="AdminTraining.aspx.cs" Inherits="Vivify.AdminTraining" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <style>
        /* Data Grid Styles */
        .mydatagrid th, .mydatagrid td {
            border: 1.5px solid black;
            padding: 12px;
            box-shadow: #1f2b60;
        }

        .header {
            background-color: #3f418d;
            font-weight: bold;
            color: ghostwhite;
            position: sticky;
            top: 0;
            z-index: 10;
            text-align: center;
        }

        .rows {
            background-color: #ffffff;
        }

        /* Pagination and scrolling */
        .pager {
            text-align: right;
        }

        .scrollable-container, .scrollable-container1 {
            max-height: 300px;
            overflow: auto;
            border: 1px solid #1f2b60;
            box-shadow: 0 2px 10px darkblue;
            margin: 20px auto;
            width: 90%;
        }.sidebar {
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

        /* Button Styles */
         .custom-button {
             background-color: #3f418d;
             color: white;
             margin-left: 30px; 
             box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
             border: none;
             padding: 10px 20px;
             cursor: pointer;
             text-align: center;
             font-size: 16px;
             border-radius: 5px;
         }
         .custom-button:hover {
             background-color:#3f418d;
         }

        /* Main container styling */
        .main {
            margin: 0;
            padding: 0;
            background-color: #cadcfc;
            height: 85vh;
            justify-content: center;
            align-items: center;
        }

        /* Custom GridView box-shadow */
        .custom-grid {
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
        }
        .card{
             height:400px;
        }
        /* Card styling */
        .card-header {
            text-align: center;
            background-color: #3f418d;
            color: ghostwhite;
            padding: 15px;
            font-size: 20px;
            border-radius: 5px 5px 0 0;
           
        }

        .form-label {
            font-weight: bold;
            color: #333;
        }

        /* Grid View header */
        .header th {
            background-color: #3f418d;
            color: ghostwhite;
        }

        /* Center the dropdown */
        .centered-dropdown {
            display: flex;             
            justify-content: center;   
            align-items: center;       
            margin: 20px auto;         
        }

        .custom-dropdown {
            background-color: white;  
            border: 1.5px solid #3f418d;  
            color: #3f418d;  
            padding: 5px 10px;  
            border-radius: 5px;  
            margin: 10px;
        }

        /* Align items center for the GridView td */
        .mydatagrid td {
            text-align: center;  
            vertical-align: middle; 
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
            
             <%-- <li class="nav-item">
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
                <li class="nav-item">
    <a class="nav-link" href="AdminPage.aspx">
       <i class="bi bi-pc-display"></i>
        <span>ExpensePage</span>
    </a>
</li>
   </ul>
        </aside>


    <main id="main" class="main">
        
        <div class="centered-dropdown">
            <label class="form-label" for="ddlBranch">Select a Branch:</label>
            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" CssClass="custom-dropdown">
            </asp:DropDownList>
        </div>

        <section class="section dashboard">
            <div class="row">
                <div class="col">
                    <div class="card">
                        <div class="card-header">
                             Training Page
                        </div>
                        <div>
                            <section class="scrollable-container">
                                <asp:GridView ID="gvTraining" runat="server" AutoGenerateColumns="false" 
                                              CellPadding="4" CellSpacing="0" GridLines="None"
                                              Width="100%" CssClass="mydatagrid" PagerStyle-CssClass="pager"
                                              RowStyle-CssClass="rows" HeaderStyle-CssClass="header"
                                              style="border: 1.5px solid midnightblue; border-collapse: collapse; font-size:14px; line-height:20px; box-shadow:0 4px 15px rgba(0, 0, 0, 0.2);" 
                                              OnRowCommand="gvTraining_RowCommand">
                                    <Columns>
                                        
                                           <asp:BoundField DataField="FirstName" HeaderText="First Name" />
<asp:BoundField DataField="FromDate" HeaderText="From Date" DataFormatString="{0:dd-MMM-yyyy}" />                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnVerify" runat="server" Text="Click Here to Verify" CommandName="Verify" CommandArgument='<%# Eval("Training_Id") %>' CssClass="btn btn-primary custom-button center-button" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </section>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </main>

</asp:Content>