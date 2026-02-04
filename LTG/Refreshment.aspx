<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Refreshment.aspx.cs" Inherits="Vivify.Refreshment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main">
        <style>
            /* Base Styles */
            .main {
                margin: 0;
                padding: 10px;
                background-color: #cadcfc;
                min-height: 100vh;
            }

            .form-label {
                font-weight: bold;
                color: #333;
                margin-bottom: 0.5rem;
                font-size: 14px;
            }

            .form-control, .form-select {
                width: 100%;
                padding: 8px;
                margin-bottom: 12px;
                border-radius: 4px;
                border: 2px solid darkblue;
                font-size: 14px;
            }

            .button-container {
                display: flex;
                gap: 8px;
                justify-content: flex-start;
                flex-wrap: wrap;
            }

            .btn-primary {
                width: 80px;
                padding: 6px 8px;
                border-radius: 6px;
                cursor: pointer;
                background-color: green;
                color: white;
                border: none;
                font-size: 12px;
                font-weight: bold;
            }

            .btn-primary:hover {
                background-color: darkgreen;
            }

            .btn-back {
                width: 80px;
                padding: 6px 8px;
                border-radius: 6px;
                cursor: pointer;
                background-color: #3f418d;
                color: white;
                border: none;
                font-size: 12px;
                font-weight: bold;
            }

            .btn-back:hover {
                background-color: #2a2c6b;
            }

            .card {
                background: white;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                margin-bottom: 20px;
                overflow: hidden;
            }

            .card-title {
                text-align: center;
                background-color: #3f418d;
                color: white;
                padding: 12px;
                margin: 0;
                font-size: 16px;
            }

            .form-container {
                padding: 15px;
            }

            .custom-grid {
                border-collapse: collapse;
                width: 100%;
                font-family: Arial, sans-serif;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                border: 1px solid black;
                background-color: white;
                font-size: 12px;
            }

            .custom-grid th,
            .custom-grid td {
                padding: 6px;
                text-align: left;
                border: 1px solid black;
                word-wrap: break-word;
            }

         .btn-view-attachment {
    background-color: #2a2c6b;
    color: white;
    border: none;
    padding: 4px 8px;
    border-radius: 4px;
    font-size: 12px;
    font-weight: bold;
    cursor: pointer;
    min-width: auto;
    width: auto;
    display: inline-block;
    margin-top: 4px;
    margin-left: 0;
}

            .btn-view-attachment:hover {
                background-color: #1e2050;
            }

            .custom-grid th {
                background-color: #3f418d;
                color: white;
                font-weight: bold;
                text-align: center;
                padding: 8px;
                font-size: 11px;
                position: sticky;
                top: 0;
                z-index: 10;
                border-bottom: 2px solid #fff;
            }

            .custom-grid thead {
                position: sticky;
                top: 0;
                z-index: 10;
            }

            .grid-container {
                overflow-x: auto;
                overflow-y: auto;
                max-height: 300px;
                border: 1px solid #ddd;
                border-radius: 4px;
                margin-top: 15px;
                position: relative;
            }

            .custom-grid tr {
                background-color: white;
            }

            .custom-grid tr:hover {
                background-color: #f8f9fa;
            }

            .btn-edit {
                background-color: #3f418d;
                color: white;
                border: none;
                padding: 4px 6px;
                font-size: 10px;
                border-radius: 4px;
                cursor: pointer;
                margin-right: 3px;
                min-width: 40px;
            }

            .btn-delete {
                background-color: #dc3545;
                color: white;
                border: none;
                padding: 4px 6px;
                font-size: 10px;
                border-radius: 4px;
                cursor: pointer;
                min-width: 40px;
            }

            .btn-edit:hover, .btn-delete:hover {
                opacity: 0.9;
            }

            @media screen and (max-width: 768px) {
                .main { padding: 5px; }
                .card-title { font-size: 14px; padding: 10px; }
                .form-container { padding: 10px; }
                .form-label { font-size: 13px; }
                .form-control, .form-select { padding: 6px; font-size: 13px; margin-bottom: 10px; }
                .btn-primary, .btn-back { width: 70px; padding: 5px 6px; font-size: 11px; }
                .button-container { gap: 6px; }
                .grid-container { overflow-x: auto; border: 1px solid #ddd; border-radius: 4px; margin-top: 15px; }
                .custom-grid { min-width: 600px; font-size: 11px; }
                .custom-grid th, .custom-grid td { padding: 4px; font-size: 10px; }
                .custom-grid th { padding: 6px 4px; font-size: 10px; }
                .btn-edit, .btn-delete { padding: 3px 5px; font-size: 9px; min-width: 35px; margin-right: 2px; }
            }

            @media screen and (max-width: 480px) {
                .main { padding: 3px; }
                .card-title { font-size: 13px; padding: 8px; }
                .form-container { padding: 8px; }
                .btn-primary, .btn-back { width: 65px; padding: 4px 5px; font-size: 10px; }
                .custom-grid { min-width: 500px; font-size: 10px; }
                .custom-grid th, .custom-grid td { padding: 3px; font-size: 9px; }
                .btn-edit, .btn-delete { padding: 2px 4px; font-size: 8px; min-width: 30px; }
            }

            @media screen and (max-width: 360px) {
                .btn-primary, .btn-back { width: 60px; padding: 3px 4px; font-size: 9px; }
                .custom-grid { min-width: 450px; }
            }

            .alert-info {
                background-color: #d1ecf1;
                border: 1px solid #bee5eb;
                border-radius: 4px;
                padding: 8px;
                margin-top: 8px;
                font-size: 12px;
            }

            #lblValidationMessage, #lblSuccessMessage {
                font-size: 12px;
                margin: 8px 0;
                display: block;
            }

            @media print {
                .btn-primary, .btn-back, .btn-edit, .btn-delete { display: none; }
                .custom-grid { box-shadow: none; }
            }
            .custom-file-input-container {
        position: relative;
        display: flex;
        align-items: center;
    }

    .custom-file-input-container input[type="text"] {
    flex-grow: 1;
    padding-right: 30px; /* ✅ Make space for × icon inside input */
    border: 2px solid darkblue;
    border-radius: 4px;
    font-size: 14px;
    cursor: pointer;
    position: relative;
}

    .file-clear-icon {
        position: absolute;
        right: 8px;
        top: 50%;
        transform: translateY(-50%);
        font-size: 25px;
        color: red;
        cursor: pointer;
        background: none;
        border: none;
        padding: 0;
        margin: 0;
        z-index: 10;
        line-height: 1;
        margin-bottom:18px;
    }

    .file-clear-icon:hover {
        color: darkred;
    }

  .btn-view-attachment {
    background-color: #2a2c6b;
    color: white;
    border: none;
    padding: 4px 8px;
    border-radius: 4px;
    font-size: 12px;
    font-weight: bold;
    cursor: pointer;
    min-width: auto;
    width: auto;
    display: inline-block;
    margin-top: 4px;
    margin-right: 700px;
}
        </style>

        <div class="formarea">
            <section class="section dashboard">
                <div class="row">
                    <div class="col">
                        <div class="card">
                            <h5 class="card-title">Assign Refreshment</h5>
                            <section class="form-container section error-404 d-flex flex-column" style="box-shadow: 0 2px 10px #1f2b60;">
                                <label for="txtEmployeeName" class="form-label">Employee Name:</label>
                                <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>

                                <label for="txtServiceType" class="form-label">Service Type:</label>
                                <asp:TextBox ID="txtServiceType" runat="server" Text="Refresh" CssClass="form-control" ReadOnly="true"></asp:TextBox>

                                <label for="txtDepartment" class="form-label">Department:</label>
                                <asp:TextBox ID="txtDepartment" runat="server" Text="Refresh" CssClass="form-control" ReadOnly="true"></asp:TextBox>

                                <label for="txtLocalRefreshmentFromDate" class="form-label">From Date:</label>
                                <asp:TextBox ID="txtLocalRefreshmentFromDate" runat="server" CssClass="form-control" placeholder="MM/DD/YYYY" TextMode="Date"></asp:TextBox>

                                <label for="txtLocalRefreshmentToDate" class="form-label">To Date:</label>
                                <asp:TextBox ID="txtLocalRefreshmentToDate" runat="server" CssClass="form-control" placeholder="MM/DD/YYYY" TextMode="Date"></asp:TextBox>

                                <label for="txtLocalRefreshmentAmount" class="form-label">Refreshment Amount:</label>
                                <asp:TextBox ID="txtLocalRefreshmentAmount" runat="server" CssClass="form-control" placeholder="Enter amount"></asp:TextBox>

                             <!-- Hidden File Input -->
<asp:FileUpload ID="fileUploadRefBill" runat="server" CssClass="form-control" onchange="handleFileSelection(this);" style="display:none;" />

<!-- Custom Styled File Input -->
<!-- Custom Styled File Input -->
<label for="customFileInput" class="form-label">Upload Approval:
    <asp:Button ID="btnViewAttachment" runat="server" 
    Text="👁 View" 
    CssClass="btn-view-attachment" 
    OnClick="btnViewAttachment_Click" 
    Visible="false" />
</label>
<div class="custom-file-input-container">
    <input type="text" id="customFileInput" class="form-control" placeholder="Choose File" readonly />
    <span id="fileClearIcon" class="file-clear-icon" style="display:none;">×</span>
</div>



                                <asp:Label ID="lblValidationMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                                <asp:Label ID="lblSuccessMessage" runat="server" ForeColor="Green" Visible="false"></asp:Label>

                                <div class="button-container">
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-primary" OnClick="btnSave_Click" />
                                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn-back" OnClick="btnBack_Click" />
                                </div>
                                
                                <br />
                                
                                <div class="grid-container">
                                  <asp:GridView ID="gvAssignedRefreshments" runat="server"
    AutoGenerateColumns="false"
    CssClass="custom-grid"
    EmptyDataText="No refreshments assigned yet."
    OnRowCommand="gvAssignedRefreshments_RowCommand"
    OnRowDataBound="gvAssignedRefreshments_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="S.No" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%# Container.DataItemIndex + 1 %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField DataField="FromDate" HeaderText="From Date" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="ToDate" HeaderText="To Date" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="RefreshAmount" HeaderText="Amount" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="ServiceType" HeaderText="Service Type" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="Department" HeaderText="Department" ItemStyle-HorizontalAlign="Center" />
        <asp:TemplateField HeaderText="Actions" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="btnEdit" runat="server" Text="Edit" 
                    CssClass="btn-edit"
                    CommandName="EditRecord" 
                    CommandArgument='<%# Eval("Id") %>' />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" 
                    CssClass="btn-delete"
                    CommandName="DeleteRecord" 
                    CommandArgument='<%# Eval("Id") %>'
                    OnClientClick="return confirm('Are you sure you want to delete this record?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            </section>
        </div>

      <script type="text/javascript">
          function handleFileSelection(fileInput) {
              var fileName = fileInput.files.length > 0 ? fileInput.files[0].name : '';
              var customInput = document.getElementById('customFileInput');
              var clearIcon = document.getElementById('fileClearIcon');
              var viewButton = document.getElementById('<%= btnViewAttachment.ClientID %>');
        var hasExistingFile = <%= (btnViewAttachment.Visible ? "true" : "false") %>;

              if (fileName) {
                  // If replacing existing file, confirm
                  if (hasExistingFile) {
                      if (!confirm("An existing approval file will be replaced.?")) {
                          fileInput.value = ''; // Clear selection
                          return;
                      }
                      // Hide View button for old file
                      if (viewButton) viewButton.style.display = 'none';
                  }

                  customInput.value = fileName;
                  clearIcon.style.display = 'inline-block';

                  // Show View button for new file (client-side preview)
                  if (viewButton) {
                      viewButton.style.display = 'inline-block';
                      viewButton.onclick = function (e) {
                          e.preventDefault();
                          var reader = new FileReader();
                          reader.onload = function (e) {
                              var blob = new Blob([e.target.result], { type: fileInput.files[0].type });
                              var url = URL.createObjectURL(blob);
                              window.open(url, '_blank');
                          };
                          reader.readAsArrayBuffer(fileInput.files[0]);
                      };
                  }
              } else {
                  customInput.value = '';
                  clearIcon.style.display = 'none';
                  if (viewButton) viewButton.style.display = 'none';
              }
          }

          function removeSelectedFile() {
              var fileInput = document.getElementById('<%= fileUploadRefBill.ClientID %>');
        var customInput = document.getElementById('customFileInput');
        var clearIcon = document.getElementById('fileClearIcon');
        var viewButton = document.getElementById('<%= btnViewAttachment.ClientID %>');

        fileInput.value = '';
        customInput.value = '';
        clearIcon.style.display = 'none';
        if (viewButton) viewButton.style.display = 'none';
    }

    // Make custom input clickable to trigger file dialog
    document.getElementById('customFileInput').addEventListener('click', function() {
        document.getElementById('<%= fileUploadRefBill.ClientID %>').click();
    });

          // Handle clear icon click
          document.getElementById('fileClearIcon').addEventListener('click', function (e) {
              e.stopPropagation();
              removeSelectedFile();
          });

          function adjustMobileLayout() {
              var screenWidth = window.innerWidth;
              var gridContainer = document.querySelector('.grid-container');
              if (screenWidth < 768 && gridContainer) {
                  gridContainer.style.overflowX = 'auto';
              }
          }

          window.addEventListener('load', adjustMobileLayout);
          window.addEventListener('resize', adjustMobileLayout);
</script>
    </main>
</asp:Content>