<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="TravelExpense.aspx.cs" Inherits="Vivify.TravelExpense" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <!-- Bootstrap 5 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <!-- Bootstrap 5 JS (requires Popper) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <script type="text/javascript">
        function toggleTransportFields() {
            var transportType = document.getElementById('ddlTransportType').value;
            var detailsDiv = document.getElementById('transportDetails');
            detailsDiv.style.display = transportType ? 'block' : 'none';
        }

        function confirmSubmission() {
            return confirm('Are you sure you want to save this expense?');
        }

        function clearFields() {
            document.getElementById('ddlTransportType').selectedIndex = 0;
            document.getElementById('txtFromDate').value = '';
            document.getElementById('txtFromPlace').value = '';
            document.getElementById('txtToPlace').value = '';
            document.getElementById('txtAmountConveyance').value = '';
            document.getElementById('txtFromTime').value = '';
            document.getElementById('txtToTime').value = '';
            document.getElementById('txtParticularsConveyance').value = '';
            document.getElementById('txtWBSNo').value = '';
            document.getElementById('txtSAPNo').value = '';
            document.getElementById('ddlReportingManager').selectedIndex = 0;
            document.getElementById('transportDetails').style.display = 'none';
            return false;
        }

        function openRefreshModal() {
            var modal = new bootstrap.Modal(document.getElementById('refreshAmountModal'));
            modal.show();
        }

        function showAlert(message, type) {
            alert((type === 'error' ? 'Error: ' : 'Success: ') + message);
        }
        function openRefreshModal() {
            var currentRefresh = document.getElementById('<%= hdnCurrentRefreshAmount.ClientID %>').value;
            document.getElementById('txtRefreshAmnt').value = currentRefresh || '';
            var modal = new bootstrap.Modal(document.getElementById('refreshAmountModal'));
            modal.show();
        }
        function submitWithRefreshAmount() {
            var refreshAmount = document.getElementById('txtRefreshAmnt').value;
            if (!refreshAmount || parseFloat(refreshAmount) <= 0) {
                alert('Please enter a valid refreshment amount');
                return;
            }
            document.getElementById('<%= hdnRefreshAmount.ClientID %>').value = refreshAmount;
            var modal = bootstrap.Modal.getInstance(document.getElementById('refreshAmountModal'));
            if (modal) {
                modal.hide();
                document.getElementById('refreshAmountModal').addEventListener('hidden.bs.modal', function () {
                    __doPostBack('<%= btnFinalSubmit.UniqueID %>', '');
                });
            } else {
                __doPostBack('<%= btnFinalSubmit.UniqueID %>', '');
            }
        }
        document.getElementById('txtFromDate').addEventListener('change', function () {
            document.getElementById('<%= hdnRefreshSubmitDate.ClientID %>').value = this.value;
        });
        window.onload = function () {
            toggleTransportFields();
        };
    </script>
<style>
    .form-group { 
        margin-bottom: 20px; 
        width: 100%; /* Ensure full width within container */
    }
    .form-label { 
        font-weight: bold; 
        color: midnightblue;
    }
    .form-control { 
        width: 100%; 
        padding: 10px; 
        margin-bottom: 15px; 
        border-radius: 6px; 
        border: 2px solid darkblue; 
        font-size: 15px;
    }
    .form-control[readonly] { 
        background-color: #e9ecef; 
        cursor: not-allowed; 
    }

    .card-title {
        text-align: center;
        background-color: #3f418d;
        color: white;
        margin-bottom: 0;
        font-size: 1.5rem;
        font-weight: bold;
        padding: 15px 0;
        border-top-left-radius: 8px;
        border-top-right-radius: 8px;
    }
.form-container {
    padding: 30px; /* Adds space on all sides */
    background-color: white;
    border-radius: 0 0 8px 8px;
    box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

    .button-container {
        display: flex;
        justify-content: center;
        gap: 10px;
        margin-top: 20px;
    }

    .btn-primary, .btnCancel, .btn-submit {
        width: 120px;
        padding: 10px;
        border-radius: 10px;
        cursor: pointer;
        font-size: 14px;
    }

    .btn-primary, .btn-submit { 
        background-color: #3f418d; 
        color: white; 
    }
    .btn-primary:hover, .btn-submit:hover { 
        background-color: #2a2c6e; 
    }
    .btn-submit { 
        background-color: #28a745; 
    }
    .btn-submit:hover { 
        background-color: #218838; 
    }
    .btnCancel { 
        background-color: #df4e4e; 
        color: white; 
    }

    .custom-gridview {
    width: 100%; /* or 90% if you want it narrower */
    border-collapse: collapse;
    margin-top: 20px;
    font-size: 13px; /* Smaller text */
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.custom-gridview th,
.custom-gridview td {
    padding: 6px 8px; /* Reduced padding */
    border: 1px solid #ccc;
    text-align: center;
    vertical-align: middle;
}

.custom-gridview th {
    background-color: #3f418d;
    color: white;
    font-weight: bold;
    font-size: 13px;
}

    .btn-icon-edit, .btn-icon-delete {
        width: 32px;
        height: 32px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        border-radius: 4px;
        border: none;
        color: white;
        text-decoration: none;
        cursor: pointer;
        font-size: 16px;
    }
    .btn-icon-edit { background-color: #3f418d; }
    .btn-icon-delete { background-color: #d32f2f; }
    .btn-icon-edit:hover { background-color: #2a2c6e; transform: scale(1.05); }
    .btn-icon-delete:hover { background-color: #b71c1c; transform: scale(1.05); }

    /* Optional: Center the card */
  .centered-card {
    width: 98%;           /* Increased from 95% */
    max-width: 1000px;    /* Increased from 800px → now wider */
    margin: 0 auto;
    border: none;
    box-shadow: 0 4px 15px rgba(0,0,0,0.1);
    border-radius: 8px;
    overflow: hidden;
} 
</style>

    <main id="main" class="main">
        <div class="formarea">
            <section class="section dashboard">
                <div class="row">
                    <div class="col">
                        <div class="card centered-card">
                            <h5 class="card-title">Travel Expense</h5>
                            <section class="form-container">
                                <!-- Transport Type -->
                                <div class="form-group">
                                    <label class="form-label" for="ddlTransportType">Transport Type</label>
                                    <asp:DropDownList ID="ddlTransportType" runat="server" ClientIDMode="Static"
                                        CssClass="form-control" onchange="toggleTransportFields();">
                                        <asp:ListItem Value="">-- Select Transport --</asp:ListItem>
                                        <asp:ListItem Value="Bus">Bus</asp:ListItem>
                                        <asp:ListItem Value="Bike">Bike</asp:ListItem>
                                        <asp:ListItem Value="Own Vehicle">Own Vehicle</asp:ListItem>
                                        <asp:ListItem Value="Taxi">Taxi</asp:ListItem>
                                        <asp:ListItem Value="Others">Others</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <!-- WBS, SAP, Reporting Manager (ALWAYS VISIBLE) -->
                              

                                <!-- Transport Details (conditionally shown) -->
                                <div id="transportDetails" style="display:none;">
                                    <div class="form-group">
                                        <label class="form-label" for="txtFromDate">Date</label>
                                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label" for="txtFromPlace">From Place</label>
                                        <asp:TextBox ID="txtFromPlace" runat="server" CssClass="form-control" placeholder="Enter starting location"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label" for="txtToPlace">To Place</label>
                                        <asp:TextBox ID="txtToPlace" runat="server" CssClass="form-control" placeholder="Enter destination"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label" for="txtAmountConveyance">Amount</label>
                                        <asp:TextBox ID="txtAmountConveyance" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
    <div class="row">
        <div class="col-md-6">
            <label class="form-label" for="txtFromTime">From Time</label>
            <asp:TextBox ID="txtFromTime" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
        </div>
        <div class="col-md-6">
            <label class="form-label" for="txtToTime">To Time</label>
            <asp:TextBox ID="txtToTime" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
        </div>
    </div>
</div>
                                      <div class="form-group">
      <label class="form-label" for="txtWBSNo">WBS No</label>
      <asp:TextBox ID="txtWBSNo" runat="server" CssClass="form-control" placeholder="Enter WBS Number"></asp:TextBox>
  </div>

  <div class="form-group">
      <label class="form-label" for="txtSAPNo">SAP No</label>
      <asp:TextBox ID="txtSAPNo" runat="server" CssClass="form-control" placeholder="Enter SAP Number"></asp:TextBox>
  </div>

 <div class="form-group">
    <label class="form-label" for="txtReportingManager">Reporting Manager</label>
    <asp:TextBox ID="txtReportingManager" runat="server" CssClass="form-control" placeholder="Enter Reporting Manager Name"></asp:TextBox>
</div>
                                    <div class="form-group">
                                        <label class="form-label" for="txtParticularsConveyance">Particulars</label>
                                        <asp:TextBox ID="txtParticularsConveyance" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <asp:HiddenField ID="hdnExpenseId" runat="server" />
                                <asp:HiddenField ID="hdnRefreshAmount" runat="server" />
                                <asp:HiddenField ID="hdnRefreshSubmitDate" runat="server" />
                                <asp:HiddenField ID="hdnCurrentRefreshAmount" runat="server" />  <!-- 👈 ADD THIS -->
                                <!-- Buttons -->
                                <div class="form-group button-container">
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn-primary" Text="Save" OnClick="btnSubmit_Click"
                                        OnClientClick="return confirmSubmission();" />
                                    <asp:Button ID="btnSave" runat="server" CssClass="btn-submit" Text="Submit"
                                        OnClientClick="openRefreshModal(); return false;" />
                                    <asp:Button ID="btnFinalSubmit" runat="server" Style="display: none;" OnClick="btnFinalSubmit_Click" />
                                    <asp:Button ID="btnCancel" runat="server" CssClass="btnCancel" Text="Cancel" OnClientClick="clearFields();" />
                                </div>

                                <!-- GridView (NO WBS/SAP/MANAGER COLUMNS) -->
                                <div class="form-group">
                                    <asp:GridView ID="GridViewTravelExpenses" runat="server"
                                        AutoGenerateColumns="false"
                                        CssClass="custom-gridview"
                                        HeaderStyle-BackColor="#3f418d"
                                        HeaderStyle-ForeColor="White"
                                        EmptyDataText="No travel expenses found."
                                        DataKeyNames="Id"
                                        OnRowCommand="GridViewTravelExpenses_RowCommand">
                                        <Columns>
<asp:TemplateField HeaderText="Date" ItemStyle-Width="100px">
    <ItemTemplate>
        <%# Eval("Date") == DBNull.Value ? "" : Convert.ToDateTime(Eval("Date")).ToString("dd-MMM-yyyy") %>
    </ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Transport Type" ItemStyle-Width="110px">
    <ItemTemplate><%# Eval("TransportType") %></ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="From" ItemStyle-Width="90px">
    <ItemTemplate><%# Eval("FromPlace") %></ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="To" ItemStyle-Width="90px">
    <ItemTemplate><%# Eval("ToPlace") %></ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Amount" ItemStyle-Width="80px">
    <ItemTemplate><%# Eval("Amount", "{0:F2}") %></ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Particulars" ItemStyle-Width="150px">
    <ItemTemplate><%# Eval("Particulars") %></ItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Actions" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center">
    <ItemTemplate>
        <div style="display: flex; justify-content: center; gap: 4px;">
            <asp:LinkButton ID="btnEditRow" runat="server"
                ToolTip="Edit"
                CssClass="btn-icon-edit"
                CommandName="EditRecord"
                CommandArgument='<%# Eval("Id") %>'>
                <i class="bi bi-pencil"></i>
            </asp:LinkButton>
            <asp:LinkButton ID="btnDeleteRow" runat="server"
                ToolTip="Delete"
                CssClass="btn-icon-delete"
                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                CommandName="DeleteRecord"
                CommandArgument='<%# Eval("Id") %>'>
                <i class="bi bi-trash"></i>
            </asp:LinkButton>
        </div>
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

        <!-- Refresh Amount Modal -->
        <div class="modal fade" id="refreshAmountModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Enter Refreshment Amount</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label>Refreshment Amount</label>
                            <input type="number" step="0.01" id="txtRefreshAmnt" class="form-control" placeholder="Enter amount" min="0" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" onclick="submitWithRefreshAmount()">Submit</button>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>