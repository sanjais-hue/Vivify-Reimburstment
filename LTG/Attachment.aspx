<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeFile="Attachment.aspx.cs" Inherits="Vivify.Attachment" %>
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
            width: 400px;
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
            overflow: auto;
            border: 1px solid #ccc;
            margin-bottom: 10px;
            width: 100%;
        }
        .sidebar {
            width: 100%;
            max-width: 300px;
            float: left;
            background: #3f418d;
            padding: 20px;
            box-shadow: 2px 0 5px rgba(0, 0, 0, 0.1);
        }
        .content {
            margin-left: 10px;
            padding: 20px;
            background-color: #cadcfc;
            flex: 1;
        }
        .footer {
            background-color: rgb(249, 243, 243);
            text-align: center;
            padding: 10px;
            color: #333;
        }
        .footer a {
            color: midnightblue;
            text-decoration: none;
        }
        .footer a:hover {
            text-decoration: underline;
        }
        .form-container {
           
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-top:10px;
        }
     /*   .input{
        width: 100%;
    padding: 5px;
    margin-bottom: 15px;
    border-radius: 4px;
    border: 2px solid darkblue;
}*/

          .input {
    width: 100%;
    padding: 5px;
    margin-bottom: 15px;
    border-radius: 4px;
    border: 2px solid darkblue;
}

        .label {
            display: block;
            margin-bottom: 5px;
        }
        
        .button {
            width: 100%;
            padding: 10px;
            background-color:#3f418d ;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: background-color 0.3s;
        }
        .button:hover {
            background-color: #3f418d;
        }
        .card {
            border-radius: 5px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
            background-color: #fff;
        }
        .card-title {
            text-align: center;
            background-color: #3f418d;
            color: white;
            padding: 15px;
            margin: 0;
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
  <a class="nav-link " href="ServiceAssignment.aspx">
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
    <a class="nav-link" href="SmithReport.aspx">
     <i class="bi bi-file-earmark-medical"></i>
        <span>Smith Report </span>
    </a>
</li>
       
                                
       <li class="nav-item">
    <a class="nav-link" href="DocView.aspx">
        <i class="bi bi-file-earmark-pdf-fill"></i>
        <span>Attachment</span>
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
                            <h5 class="card-title">Attachments</h5>
                            <section class="form-container">
                                <asp:Label ID="lblFromDate" runat="server" Text="From Date:" CssClass="label"></asp:Label>
                                <asp:TextBox ID="txtFromDate" runat="server" TextMode="date" CssClass="input"></asp:TextBox>
                                <asp:Label ID="lblToDate" runat="server" Text="To Date:" CssClass="label"></asp:Label>
                                <asp:TextBox ID="txtToDate" runat="server" TextMode="date" CssClass="input"></asp:TextBox>
                                <asp:Label ID="lblBranchName" runat="server" Text="Branch Name:" CssClass="label"></asp:Label>
                                <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input"></asp:DropDownList>
                                <asp:Label ID="lblExpenseType" runat="server" Text="Expense Type:" CssClass="label"></asp:Label>
                                <asp:DropDownList ID="ddlExpenseType" runat="server" CssClass="input">
                                    <asp:ListItem Value="">--Select Expense Type--</asp:ListItem>
                                    <asp:ListItem Value="Tour">Tour</asp:ListItem>
                                    <asp:ListItem Value="Local">Local</asp:ListItem>
                                    
                                </asp:DropDownList>
                                <asp:Button ID="btnFilter" runat="server" Text="Generate Report" OnClick="btnFilter_Click" CssClass="button" />
                            </section>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    </div>

   
</asp:Content>
