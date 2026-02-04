<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificationPage.aspx.cs" Inherits="Vivify.VerificationPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Claim Status</title>
    <style>
        .status-container {
            margin: 20px;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }
        .status-label {
            font-weight: bold;
            margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="status-container">
            <asp:Label ID="lblServiceId" runat="server" CssClass="status-label" Text="Service ID:"></asp:Label>
            <br />
            <asp:CheckBox ID="chkClaimable" runat="server" Text="Claimable" />
            <asp:CheckBox ID="chkNonClaimable" runat="server" Text="Non-Claimable" />
            <br /><br />
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
        </div>
    </form>
</body>
</html>
