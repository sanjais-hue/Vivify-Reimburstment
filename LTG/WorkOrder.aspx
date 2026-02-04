<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="WorkOrder.aspx.cs" Inherits="Vivify.WorkOrder" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main">
        <style>
            /* Your existing styles here */
            .form-group {
                margin-bottom: 15px;
                width: 100%; /* Full width for form groups */
            }
            .form-label {
                display: block;
                margin-bottom: 5px;
                font-weight: bold;
                color: #333;
                width: 100%; /* Ensures label takes full width */
                text-align: left; /* Aligns text to the left */
                margin-left:0;
            }
            .form-control {
                width: 100%; /* Full width for inputs */
                padding: 5px;
                border-radius: 4px;
                border: 2px solid darkblue;
                margin:0;
            }
            .form-control:focus {
                border-color: #80bdff;
                box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
            }
            .btn-save {
                background-color: #007bff;
                color: white;
                padding: 10px 15px;
                border: none;
                border-radius: 5px;
                cursor: pointer;
            }
            .btn-save:hover {
                background-color: #0056b3;
            }
            .alert {
                margin-top: 10px;
                padding: 10px;
                border-radius: 5px;
            }
            .header {
                background-color: #3f418d;
                font-weight: bold;
                color: ghostwhite;
                text-align: center;
            }
             .btn-primary {
                 color:white;
     background-color:#3f418d;
     border-color: #3f418d;
 }

 .btn-primary:hover {
     background-color: #3f418d;
     border-color: #3f418d;
 }
        </style>

        <div class="card custom-card">
            <h5 class="card-title text-center" style="background-color: #3f418d; color: ghostwhite;">Work Order Form</h5>
            <section class="section error-404 " style="box-shadow: 0 2px 10px #1f2b60; background-color: #f9eded;">
                <div>
                    <div class="form-group">
                        <label class="form-label" for="txtEmployeeName">Employee Name:</label>
                        <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hiddenEmployeeId" runat="server" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="txtServiceId">Service ID:</label>
                        <asp:TextBox ID="txtServiceId" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hiddenServiceId" runat="server" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="txtExpenseType">Expense Type:</label>
                        <asp:TextBox ID="txtExpenseType" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hiddenExpenseType" runat="server" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="txtServiceType">Service Type:</label>
                        <asp:TextBox ID="txtServiceType" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hiddenServiceType" runat="server" />
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="txtSmoNo">SMO Number:</label>
                        <asp:TextBox ID="txtSmoNo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hiddenSmoNo" runat="server" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Upload Attachment:</label>
                        <asp:FileUpload ID="fileUploadAttachment" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-primary " OnClick="btnSave_Click" />
                    </div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
                </div>
            </section>
        </div>
    </main>
</asp:Content>
