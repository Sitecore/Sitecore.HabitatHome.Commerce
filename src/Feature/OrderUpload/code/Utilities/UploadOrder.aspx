<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadOrder.aspx.cs" Inherits="Sitecore.Feature.UploadOrder.HabitatUtility.UploadOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Import</title>
</head>
<body>
    <form id="MyOrderForm" runat="server">
        <div>            
            <asp:TextBox ID="txtOrderJson" runat="server" TextMode="MultiLine" Rows="50" Columns="30"></asp:TextBox>
            <asp:Button ID="btnSubmit" runat="server" Text="Upload" OnClick="btnSubmit_Click" />
        </div>
    </form>
</body>
</html>
