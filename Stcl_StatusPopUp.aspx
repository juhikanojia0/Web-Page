<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_StatusPopUp.aspx.cs" Inherits="Stcl.Epicor905.GeneratePayment.WebPages.Stcl_StatusPopUp"  %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payment generation staus</title>
    <link href="../CSS/StyleSheet1.css" rel="Stylesheet" />
    <base target="_self" />
    <script type="text/javascript">
        window.onunload = refreshParent;
        function refreshParent() {
            alert('closing');
            window.opener.location.reload(true);
        }

        function CloseWindow() {
            //window.opener.document.getElementById('btnHidden').click();  
            //return false;            
            window.close();
            //window.opener.location.reload(true);          
        }

        function PrintDiv() {
            var contents = document.getElementById("dvContents").innerHTML;

            var windowUrl = 'about:blank';
            var uniqueName = new Date();
            var windowName = 'Print' + uniqueName.getTime();
            var printWindow = window.open(windowUrl, windowName, 'left=50,top=50,width=400,height=400');

            printWindow.document.write(contents);
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
            printWindow.close();          
            window.close();
            //window.opener.location.reload(true);            
        }
    </script>   
</head>
<body>
    <form id="form1" runat="server">      
        <div class="scrollinghm" style="text-align: center; margin: 0 auto;" id="dvContents">
            <center>
                <asp:PlaceHolder ID="LocalPlaceHolder" runat="server"></asp:PlaceHolder>
            </center>
        </div>
        <div style="text-align: center;">
            <asp:Button ID="btnPrint" runat="server" Text="Print" BackColor="#ACDCF7" Width="90px" OnClientClick="javascript:PrintDiv();" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnClose" runat="server" Text="Close" BackColor="#ACDCF7" Width="90px" OnClientClick="javascript:CloseWindow();" />
            <%--OnClick="btnClose_Click" --%>
        </div>    
    </form>
</body>
</html>
