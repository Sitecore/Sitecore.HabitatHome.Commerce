<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadOrder.aspx.cs" Inherits="Sitecore.Feature.UploadOrder.HabitatUtility.UploadOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Import</title>
</head>
<body>
    <form id="MyOrderForm" runat="server">
        <div>
            <div class="row">  
                <asp:Button ID="btnSubmit" runat="server" Text="Upload" OnClick="btnSubmit_Click" Width="400" />
            </div>
            <div>
                <asp:TextBox ID="txtOrderJson" runat="server" TextMode="MultiLine" Rows="50" Columns="150"></asp:TextBox>            
            </div>
        </div>       
    </form>
</body>
</html>
