

<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="RefreshmentVerify.aspx.cs" Inherits="Vivify.RefreshmentVerify" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
        <style>
            
                  /*body {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
                display: flex;
                flex-direction: column;
                min-height: 100vh;*/ /* Ensures the body takes full height */
            /*}*/

/*            h1 {
                color: #333;
                text-align: center;
                margin-bottom: 20px;
            }*/

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

    .gridview tr:hover {
        background-color: #f1f1f1;
    }

    .border-right {
        border-right: 4px solid #ddd;
    }

            .label {
                display: block;
                color: black;
                font-weight: bold;
            }

            .date-column {
                white-space: nowrap;
                width: 800px;
            }

            .particulars-column {
                white-space: nowrap;
                width: 250px;
            }

            main {
                background-color: #cadcfc;
            }

            #header {
                background-color: #3f418d;
            }

            .form-row {
                display: flex;
                align-items: center;
            }

            .form-row .label {
                margin-left: 20px;
            }

            .form-row select, .form-row input {
                width: 150px;
            }

            .search-btn-container {
                position: relative;
                margin-top: 20px;
            }

            #btnFilter {
                position: absolute;
                right: 0;
                top: 0;
                background-color: darkblue;
                color: white;
                padding: 5px 15px;
                width: 100px;
                z-index: 10;
                transition: background-color 0.3s;
            }

            #btnFilter:hover {
                background-color: #222b65;
            }

            @media (max-width: 768px) {
                .form-row {
                    justify-content: center;
                    flex-wrap: wrap;
                }

                .form-row .label, .form-row select, .form-row input {
                    width: 100%;
                    margin-bottom: 10px;
                }

                #btnFilter {
                    position: fixed;
                    right: 10px;
                    bottom: 20px;
                    z-index: 999;
                    width: auto;
                }
            }

           
                .content {
                    margin-left: 10px;
                    padding: 20px;
                    background-color: #cadcfc;
                    min-height: calc(100vh - 40px);
                    padding-bottom: 50px;
                    box-sizing: border-box;
                }

                .form-row .col {
                    flex: 1;
                    min-width: 100px;
                }

                .form-row .form-label {
                    font-size: 0.875rem;
                    margin-bottom: 2px;
                }

                .form-row .form-control, .form-row .form-select {
                    width: 100%;
                    padding: 4px;
                    font-size: 0.875rem;
                    height: 28px;
                }

                .form-row .btn {
                    padding: 5px 10px;
                    font-size: 0.875rem;
                    height: 30px;
                    min-width: 70px;
                }

                #verificationForm label {
                    font-size: 0.875rem;
                    margin-right: 5px;
                }

                #verificationForm input[type="text"],
                #verificationForm .btn {
                    width: 120px;
                    height: 28px;
                    font-size: 0.875rem;
                    padding: 4px;
                }

                #verificationForm .form-row {
                    display: flex;
                    flex-wrap: nowrap;
                    gap: 5px;
                    align-items: center;
                }

                .form-row .col-4 {
                    flex: 0 0 22%;
                }

                .btn-container {
                    display: flex;
                    gap: 10px;
                    justify-content: space-between;
                    align-items: center;
                }

                .required-field {
                    color: OrangeRed;
                    margin-top: 0.25rem;
                    font-size: 0.875rem;
                }

              .btn-primary {
    background-color: #3f418d;
    border-color: #3f418d;
    color: white;
    padding: 4px 12px;
    border-radius: 4px;
    font-size: 0.85rem;
    transition: 0.3s ease;
}

.btn-primary:hover {
    background-color: #2d3270;
    border-color: #2d3270;
}

                .form-container {
                    padding: 15px;
                    background-color: #f8f9fa;
                    border-radius: 5px;
                    box-shadow: 0 2px 10px darkblue;
                }

                .table-container {
                    display: block;
                    max-height: 400px;
                    overflow-y: auto;
                    box-shadow: 0 4px 15px darkblue;
                    background-color: white;
                    border-radius: 5px;
                    margin-bottom: 50px;
                }

                .table th {
                    position: sticky;
                    top: 0;
                    background-color: #3f418d;
                    color: ghostwhite;
                    z-index: 10;
                    text-align: center;
                }

                .table th, .table td {
                    padding: 8px;
                    border: 2px solid darkblue;
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
                .footer {
                    position: fixed;
                    bottom: 0;
                    left: 0;
                    right: 0;
                    background-color: rgb(249, 243, 243);
                    text-align: center;
                    padding: 10px;
                    color: ghostwhite;
                    z-index: 1000;
                }

                .footer a {
                    color: midnightblue;
                    text-decoration: none;
                }

                .footer a:hover {
                    text-decoration: underline;
               
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
            <section class="section dashboard" >
                <div class="row">
                    <div class="col">
                        <div class="card">
                            <h5 class="card-title" style="text-align:center;background-color:#3f418d;color:white">Refreshment Verification </h5>
                            <asp:Label ID="Label1" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                           
                              <div class="form-row">
    <div class="label">
        <asp:Label ID="lblFromDate" runat="server" Text="From Date:" ></asp:Label>
        <asp:TextBox ID="txtFromDate" runat="server" TextMode="date"></asp:TextBox>
    </div>

    <div class="label">
        <asp:Label ID="lblToDate" runat="server" Text="To Date:"></asp:Label>
        <asp:TextBox ID="txtToDate" runat="server" TextMode="date"></asp:TextBox>
    </div>

    <div class="label">
        <asp:Label ID="BranchName" runat="server" Text="Branch Name:"></asp:Label>
        <asp:DropDownList ID="ddlBranch" runat="server" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" AutoPostBack="true">
        </asp:DropDownList>
    </div>

    <div class="label">
        <asp:Label ID="lblEmployeeName" runat="server" Text="Employee Name:"></asp:Label>
        <asp:DropDownList ID="ddlEmployee" runat="server" AutoPostBack="True">
        </asp:DropDownList>
    </div>
</div>
                                    <asp:Button ID="btnFilter" runat="server" Text="Search" OnClick="btnFilter_Click"  style="background-color:darkblue; color:white;  padding: 5px 15px; width:80px;margin:8px;"/>
                                </div>

   <div class="grid-container">
    <asp:GridView ID="GridView1" runat="server" CssClass="gridview"
        AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand"
        OnRowDataBound="GridView1_RowDataBound">
        <Columns>
            <asp:BoundField DataField="FirstName" HeaderText="Employee Name" />
            <asp:BoundField DataField="BranchName" HeaderText="Branch" />
            <asp:BoundField DataField="FromDate" HeaderText="From Date" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="ToDate" HeaderText="To Date" DataFormatString="{0:yyyy-MM-dd}" />
           <asp:BoundField DataField="RefreshAmount" HeaderText="Amount" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="btnVerify" runat="server" Text="Verify" CommandName="VerifyRow" CommandArgument='<%# Eval("Id") %>'
                        CssClass="btn btn-sm btn-primary" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
                    <asp:Label ID="lblError" runat="server" CssClass="error-label" Visible="false"></asp:Label>

        </div>
    
</asp:Content>
