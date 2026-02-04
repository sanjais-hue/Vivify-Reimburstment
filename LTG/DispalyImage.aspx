<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayImage.aspx.cs" Inherits="Vivify.DisplayImage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <title>Claimable Expenses</title>
    <link rel="stylesheet" type="text/css" href="styles.css" />
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }
        .container {
            max-width: 800px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        h2 {
            text-align: center;
            color: #333;
        }
        .image-gallery {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
        }
        .image-container {
            margin: 10px;
            text-align: center;
        }
        img {
            max-width: 150px;
            height: auto;
            border-radius: 5px;
            transition: transform 0.2s;
        }
        img:hover {
            transform: scale(1.05);
        }
        .no-claimable {
            text-align: center;
            color: #888;
            font-style: italic;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Claimable Expenses</h2>
            <div id="ImagePlaceholder" class="image-gallery" runat="server">
                <!-- Images will be dynamically added here -->
            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="no-claimable" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>
