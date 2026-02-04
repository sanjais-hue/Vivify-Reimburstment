<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="Employeecreation.aspx.cs" Inherits="Vivify.Employeecreation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <head>
       <%-- <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />--%>
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
      
    <style>
    /* Existing styles remain the same */
    .form-label {
        font-weight: bold;
        color: #333;
        margin-bottom: 0.25rem;
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
    .edit-button, .delete-button {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 32px;
        height: 32px;
        padding: 0;
        border-radius: 4px;
        text-decoration: none;
        font-weight: bold;
        cursor: pointer;
        transition: background 0.3s, border 0.3s;
        margin-right: 4px;
    }

    .edit-button:hover, .delete-button:hover {
        opacity: 0.9;
    }

    .edit-button {
        background-color: #003366;
        border: 2px solid #003366;
        color: white;
    }

    .delete-button {
        background-color: #dc3545;
        border: 2px solid #dc3545;
        color: white;
    }

    .form-select{
        width: 100%;
        padding: 5px;
        border-radius: 4px;
        border: 2px solid darkblue;
    }

    .form-control {
        width: 100%;
        padding: 5px;
        border-radius: 4px;
        border: 2px solid darkblue;
    }

    .custom-button {
        background-color: blue;
        color: white;
        border: 2px solid black;
        padding: 5px 10px;
        border-radius: 5px;
        cursor: pointer;
        text-decoration: none;
        display: inline-block;
    }

    .custom-button:hover {
        background-color: darkblue;
    }
    
    .form-control:focus {
        border-color: #80bdff;
        box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
    }
    
    .required-field {
        color: OrangeRed;
        margin-top: 0.25rem;
        font-size: 0.875rem;
    }

    .btn-primary {
        background-color: #3f418d;
        border-color: #3f418d;
    }
    
    .btn-primary:hover {
        background-color: #0056b3;
        border-color: #0056b3;
    }
    
    .form-container {
        padding: 15px;
        background-color: #f8f9fa;
        border-radius: 5px;
        box-shadow: 0 2px 10px darkblue;
    }

    /* Updated Grid Styles */
    .table-container {
        display: block;
        max-height: 400px;
        overflow-y: auto;
        overflow-x: auto; /* Allow horizontal scrolling only when necessary */
        box-shadow: 0 4px 15px darkblue;
        background-color: white;
        border-radius: 5px;
        margin-bottom: 50px;
        width: 100%;
    }

    .mydatagrid {
        width: 100%;
        border-collapse: collapse;
        font-size: 0.75rem; /* Reduced font size */
    }

    .mydatagrid th {
        position: sticky;
        top: 0;
        background-color: #3f418d;
        color: ghostwhite;
        z-index: 10;
        text-align: center;
        padding: 6px 4px; /* Reduced padding */
        border: 1px solid darkblue;
        font-size: 0.7rem; /* Smaller font for headers */
        font-weight: bold;
        white-space: nowrap;
    }

    .mydatagrid td {
        padding: 5px 3px; /* Reduced padding */
        border: 1px solid darkblue;
        text-align: center;
        font-size: 0.7rem; /* Smaller font for cells */
        word-wrap: break-word;
        white-space: normal; /* Allow text to wrap */
    }

    /* Mobile Responsive Styles */
    @media (max-width: 768px) {
        .mydatagrid {
            font-size: 0.65rem; /* Even smaller on mobile */
        }
        
        .mydatagrid th,
        .mydatagrid td {
            padding: 4px 2px;
            font-size: 0.65rem;
        }
        
        .table-container {
            max-height: 350px;
            margin-bottom: 30px;
        }
        
        .edit-button, .delete-button {
            width: 28px;
            height: 28px;
            margin-right: 2px;
        }
        
        .edit-button i, 
        .delete-button i {
            font-size: 0.8rem;
        }
    }

    @media (max-width: 576px) {
        .mydatagrid {
            font-size: 0.6rem;
        }
        
        .mydatagrid th,
        .mydatagrid td {
            padding: 3px 1px;
            font-size: 0.6rem;
        }
        
        /* Hide less important columns on very small screens */
        .mydatagrid td:nth-child(4), /* Official Mail */
        .mydatagrid th:nth-child(4) {
            display: none;
        }
    }

    /* Ensure the grid is responsive */
    .table-responsive {
        width: 100%;
        overflow-x: auto;
        -webkit-overflow-scrolling: touch;
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

</li>
<%--                            <li class="nav-item">
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
</li>--%>     
   </ul>
        </aside>

        <meta name="viewport" content="width=device-width, initial-scale=1">
    </head>
    
    <main id="main" class="main" style="background-color: #cadcfc;">
        <div class="container">
            <div class="formarea">
                <section class="section dashboard">
                    <div class="row">
                        <div class="col">
                            <div class="card">
                                 <h5 class="card-title" style="text-align:center;background-color:#3f418d;color:white">Employee Creation</h5>
                                <section class="form-container section error-404 d-flex flex-column align-items-center justify-content-center" >
                                    <div class="row g-1 needs-validation"> <!-- Changed g-3 to g-1 for tighter spacing -->
                                        <div class="col-12 ">
                                            <label for="ddlBranch" class="form-label">Branch</label>
                                            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="form-select"  EnableViewState="true">
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-12 ">
                                            <label for="txtcode" class="form-label">Employee Code</label>
                                            <asp:TextBox ID="txtcode" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="rfvEmpCode" runat="server" 
        ControlToValidate="txtcode" ErrorMessage="Employee Code is required!" ForeColor="Red"
        ValidationGroup="EmployeeCreation" />
                                        </div>

                                        <div class="col-12 ">
                                            <label for="txtName" class="form-label">First Name</label>
                                            <asp:TextBox ID="txtName" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                             <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
        ControlToValidate="txtName" ErrorMessage="First Name is required!" ForeColor="Red"
        ValidationGroup="EmployeeCreation" />
                                        </div>

                                        <div class="col-12 ">
                                            <label for="txtLname" class="form-label">Last Name</label>
                                            <asp:TextBox ID="txtLname" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                             <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
        ControlToValidate="txtLname" ErrorMessage="Last Name is required!" ForeColor="Red"
        ValidationGroup="EmployeeCreation" />
                                        </div>
                                        <div class="col-12">
    <label for="ddlRole" class="form-label">Role</label>
    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select">
        <asp:ListItem Text="Select" Value="0"></asp:ListItem>
        <asp:ListItem Text="Employee" Value="Employee"></asp:ListItem>
        <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
    </asp:DropDownList>
   
</div>

                                        <div class="col-12 ">
                                        <label for="txtMobno" class="form-label">Mobile Number</label>
                                        <asp:TextBox ID="txtMobno" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                      
                                       <asp:RequiredFieldValidator ID="rfvMobile" runat="server" 
        ControlToValidate="txtMobno" ErrorMessage="Mobile Number is required!" ForeColor="Red"
        ValidationGroup="EmployeeCreation" />
                                        <asp:RegularExpressionValidator 
                                            runat="server" 
                                            ControlToValidate="txtMobno" 
                                            CssClass="required-field" 
                                            ErrorMessage="Please enter a valid 10-digit mobile number!" 
                                            ValidationExpression="^\d{10}$" />
                                    </div>
<div class="col-12">
    <label for="txtAltMobno" class="form-label">Alternative Mobile Number</label>
    <asp:TextBox ID="txtAltMobno" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
    
    <asp:RegularExpressionValidator 
        runat="server" 
        ControlToValidate="txtAltMobno" 
        ValidationExpression="^\d{10}$" 
        ErrorMessage="Please enter a valid 10-digit mobile number (if provided)" 
        ForeColor="Red" />
</div>


<div class="col-12">
        <label for="txtOfcemail" class="form-label">Official Email</label>
       <asp:TextBox ID="txtOfcemail" runat="server" TextMode="Email" ClientIDMode="Static" CssClass="form-control" autocomplete="off"></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
    ControlToValidate="txtOfcemail" ErrorMessage="Official Email is required!" ForeColor="Red"
    ValidationGroup="EmployeeCreation" />
    </div>
                                           <%-- <asp:RegularExpressionValidator 
                    runat="server" 
                    ControlToValidate="txtOfcemail" 
                    CssClass="required-field" 
                    ErrorMessage="Invalid email format!" 
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">
                </asp:RegularExpressionValidator> auto
                                       --%>

                                        <div class="col-12">
    <label for="txtTempPassword" class="form-label">Temporary Password</label>
    <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
     <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
        ControlToValidate="txtPassword" ErrorMessage="Password is required!" ForeColor="Red"
        ValidationGroup="EmployeeCreation" />
</div>



                                        <%--<div class="col-12 ">
                                            <label for="ddldesignation" class="form-label">Designation</label>
                                            <asp:DropDownList ID="ddldesignation" runat="server" CssClass="form-select">
                                                  <asp:ListItem Text="Select" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="FSE" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="FST" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>--%>
                                        <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
<div class="col-12 d-flex gap-2">
    <asp:Button ID="btn1" OnClick="btn1_Click" CssClass="btn btn-primary w-50" Text="Submit" 
        runat="server" ClientIDMode="Static" style="background-color:#3f418d; margin-top:10px;" />
    <asp:Button ID="btnClear" OnClick="btnClear_Click" CssClass="btn btn-warning w-50" Text="Cancel" 
        runat="server" ClientIDMode="Static" style="background-color:#cc4c4c; margin-top:10px; color: white;" />
</div>
                                    </div>
                                </section>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
            
          <section class="scrollable-container mt-4">
    <div class="table-responsive">
        <div class="table-container">
  <asp:GridView ID="GridView1" runat="server"
    AutoGenerateColumns="false"
    CssClass="mydatagrid table table-bordered"
    OnRowDeleting="GridView1_RowDeleting"
    DataKeyNames="EmployeeId"
    Width="100%"
    CellSpacing="0"
    CellPadding="3">
    <Columns>
        <asp:TemplateField HeaderText="Emp Code" ItemStyle-Width="12%">
            <ItemTemplate>
                <asp:Label ID="lblEmployeeCode" runat="server" Text='<%# Eval("EmployeeCode") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="First Name" ItemStyle-Width="15%">
            <ItemTemplate>
                <asp:Label ID="lblFirstName" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Mobile" ItemStyle-Width="12%">
            <ItemTemplate>
                <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("MobileNumber") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email" ItemStyle-Width="20%">
            <ItemTemplate>
                <asp:Label ID="lblMail" runat="server" Text='<%# Eval("OfficialMail") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Branch" ItemStyle-Width="15%">
            <ItemTemplate>
                <asp:Label ID="lblBranch" runat="server" Text='<%# Eval("BranchName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="12%">
            <ItemTemplate>
                <asp:LinkButton ID="btnEdit" runat="server" 
                    OnClick="btnEdit_Click" 
                    CssClass="edit-button" 
                    CausesValidation="false"
                    CommandArgument='<%# Eval("EmployeeId") %>'
                    ToolTip="Edit">
                    <i class="bi bi-pencil"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="btnDelete" runat="server" 
                    CommandName="Delete" 
                    CssClass="delete-button" 
                    OnClientClick="return confirm('Are you sure you want to delete this employee?');" 
                    CausesValidation="false"
                    ToolTip="Delete">
                    <i class="bi bi-trash"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
        </div>
    </div>
</section>


                    </div>
               
    </main>
    

</asp:Content>