<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Training_Verify.aspx.cs" Inherits="Vivify.Training_Verify" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main container d-flex flex-column align-items-center">
        <style>
            .scrollable-container {
                width: 100%;
                overflow-x: auto;
            }
            .table-responsive {
                display: block;
                max-width: 100%;
                overflow-x: auto;
                white-space: nowrap;
            }
            .mydatagrid {
                width: 100%;
                border-collapse: collapse;
            }
            .mydatagrid th, .mydatagrid td {
                padding: 8px;
                border: 1px solid black;
                text-align: center;
            }
            .mydatagrid th {
                background-color: #3f418d;
                color: white;
                position: sticky;
                top: 0;
            }
            .amount-container {
                display: flex;
                align-items: center;
                justify-content: center;
                gap: 10px;
            }
            .button-container {
                margin-top: 20px;
                display: flex;
                justify-content: center;
                gap: 15px;
            }
            .form-control {
                width: 120px;
                text-align: center;
                padding: 5px;
            }
            .form-container {
                padding: 20px;
                background-color: #f8f9fa;
                border-radius: 5px;
                box-shadow: 0 2px 10px darkblue;
            }
            .custom-card {
                width: 100%;
                max-width: 1200px;
                margin-top: 20px;
            }
            .btn-warning, .btn-success {
                padding: 5px 10px;
            }
            .text-success, .text-error {
                margin-top: 15px;
                font-size: 1rem;
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
            
<%--              <li class="nav-item">
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
        <div class="card custom-card">
            <h5 class="card-title text-center" style="background-color: #3f418d; color: ghostwhite;">Training Verification</h5>
            <section class="section error-404 d-flex flex-column align-items-center justify-content-center">
                <div class="scrollable-container">
                   <asp:GridView ID="TrainingGridView" runat="server" AutoGenerateColumns="False" 
    DataKeyNames="TrainingId" CssClass="mydatagrid" AllowPaging="True" PageSize="5"
    OnRowDataBound="TrainingGridView_RowDataBound">

                        <Columns>
                            <asp:TemplateField HeaderText="Source">
                                <ItemTemplate>
                                    <asp:Label ID="lblSource" runat="server" Text='<%# Eval("Source") %>' Visible="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Date" HeaderText="Date" />
                            <asp:BoundField DataField="Particulars" HeaderText="Particulars" />
                            <asp:BoundField DataField="Distance" HeaderText="Distance" />
                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <div class="amount-container">
                                        <asp:TextBox ID="txtAmount" runat="server" Text='<%# Bind("Amount") %>' ReadOnly="true" CssClass="form-control" />
                                        <asp:Label ID="lblTrainingId" runat="server" Text='<%# Eval("TrainingId") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandArgument='<%# Eval("TrainingId") %>' CssClass="btn btn-warning btn-sm" OnClick="btnEdit_Click" />
                                        <asp:Button ID="btnSave" runat="server" Text="Save" CommandArgument='<%# Eval("TrainingId") %>' Visible="false" CssClass="btn btn-success btn-sm" OnClick="btnSave_Click" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="button-container">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn btn-primary px-4 py-2" />
                </div>
                <div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
                </div>
            </section>
        </div>
    </main>
</asp:Content>