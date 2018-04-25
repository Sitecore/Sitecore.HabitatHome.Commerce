<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryUpload.aspx.cs" Inherits="Sitecore.Feature.InventoryUpload.HabitatUtility.InventoryUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inventory Import</title>
</head>
<body>
    <form id="MyOrderForm" runat="server">
        <div>
            <div class="row">  
                <asp:Button ID="btnSubmit" runat="server" Text="Upload" OnClick="btnSubmit_Click" Width="400" />
            </div>
            <div>
                <label>PASTE THE JSON WITH INVENTORY DETAILS IN THE TEXTBOX BELOW AND HIT UPLOAD</label>
            </div>
            <div>
                <asp:TextBox ID="txtInventoryJson" runat="server" TextMode="MultiLine" Rows="50" Columns="150"></asp:TextBox>            
            </div>
        </div>       
    </form>
</body>
</html>
