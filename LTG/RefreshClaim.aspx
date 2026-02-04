
<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="RefreshClaim.aspx.cs" Inherits="Vivify.RefreshClaim" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
        }

        .main {
            flex: 1;
            padding: 20px;
            margin-left: 250px;
            display: flex;
            flex-direction: column;
            align-items: center;
            background-color: #cadcfc;
            transition: margin-left 0.3s;
        }

        .grid-container {
            margin-top: 40px;
            width: 90%;
            max-height: 70vh;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: white;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            padding: 10px;
        }

        .gridview {
            width: 100%;
            border-spacing: 0;
        }

        .gridview th, .gridview td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: center;
            word-wrap: break-word;
            white-space: normal;
        }

        .gridview th {
            background-color: #3f418d;
            color: white;
            position: sticky;
            top: 0;
            z-index: 10;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .amount-textbox {
            width: 100px;
            padding: 8px;
            text-align: right;
            border: 1px solid #ddd;
            border-radius: 4px;
            background-color: #e9ecef;
        }

        .btn {
            padding: 8px 12px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            text-align: center;
            display: inline-block; /* Ensure visibility */
        }

        .btn-edit, .btn-submit {
            background-color: #3f418d;
            color: white;
        }

        .btn-edit:hover, .btn-submit:hover {
            background-color: #2c2f6a;
        }

        .btn-cancel {
            background-color: #dc3545;
            color: white;
        }

        .btn-cancel:hover {
            background-color: #b52b37;
        }

        h5 {
            text-align: center;
            background-color: #3f418d;
            color: white !important; /* Force white text */
            padding: 10px;
            border-radius: 5px;
            font-size: 18px;
            width: 100%;
            display: inline-block;
            margin-top: 20px;
        }
    </style>

    <script type="text/javascript">
        function showSuccessMessage() {
            alert("Amount claimed successfully!");
        }
    </script>
       <aside id="sidebar" class="sidebar" style="box-shadow: 0 2px 10px darkblue;">
       <ul class="sidebar-nav" id="sidebar-nav">
           <li class="nav-item">
               <a class="nav-link" href="AdminPage.aspx">
                   <i class="bi bi-pc-display"></i>
                   <span>Expense Page</span>
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
        <span>Refreshment Verify </span>
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

    <div class="main">
        <div class="grid-container">
            <h5 class="card-title text-center">Refreshment Claim</h5>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" DataKeyNames="Id" CssClass="gridview" OnRowCommand="GridView1_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Employee Name">
                        <ItemTemplate>
                            <%# Eval("FirstName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="From Date">
                        <ItemTemplate>
                            <%# Eval("FromDate", "{0:MM/dd/yyyy}") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="To Date">
                        <ItemTemplate>
                            <%# Eval("ToDate", "{0:MM/dd/yyyy}") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtAmount" runat="server" Text='<%# Eval("RefreshAmount") %>' ReadOnly="true" CssClass="amount-textbox" />
                            <asp:Button ID="btnEdit" runat="server" CommandName="EditAmount" Text="Edit" CommandArgument="<%# Container.DataItemIndex %>" CssClass="btn btn-edit" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="form-group">
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn btn-submit" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-cancel" />
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="btn btn-submit" />
        </div>

        <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="False" />
    </div>
</asp:Content>
