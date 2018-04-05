<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadOfflineOrder.aspx.cs" Inherits="Sitecore.Commerce.Website.Views.OrderUpload.UploadOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Import</title>

    <script>      
	  function prettyPrint() {
		var ugly = document.getElementById('myTextArea').value;
		var obj = JSON.parse(ugly);
		var pretty = JSON.stringify(obj, undefined, 4);
		document.getElementById('myTextArea').value = pretty;
		document.getElementById('btnSubmit').value = pretty;
		}
  </script>
</head>
<body>
    <form id="MyOrderForm" runat="server">
        <div>
            <button onclick="prettyPrint()">Format JSON</button>				  
            <asp:TextBox ID="txtOrderJson" runat="server" TextMode="MultiLine" Rows="80"></asp:TextBox>
            <asp:Button ID="btnSubmit" runat="server" Text="Upload" OnClick="btnSubmit_Click" />
        </div>
    </form>
</body>
</html>
