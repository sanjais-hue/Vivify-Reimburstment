
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fileupload.aspx.cs" Inherits="Vivify.fileupload" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>File Upload</title>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="UploadButton" Text="Upload File" OnClick="UploadButton_Click" runat="server" />
            <asp:Label ID="StatusLabel" runat="server" Text="" />
        </div>
    </form>
</body>
</html>
