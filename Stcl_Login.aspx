<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_Login.aspx.cs" Inherits="Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages.Stcl_Login" %>

<%@ Register Assembly="AjaxControls" Namespace="AjaxControls" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Login screen</title>
    <link rel="stylesheet" href="../css/style.css">
</head>
<body>
    <div style="height: 50px"></div>
    <div style="background-image: url(../images/GPLogo.PNG); height: 50px; margin: 10px; background-repeat: no-repeat; vertical-align: bottom">
    </div>
    <div class="wrapper">

        <div class="container">
            <h1>Welcome</h1>
            <form id="form1" runat="server" autocomplete="Off" class="form">
                <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server"></ajaxToolkit:ToolkitScriptManager>
                <asp:Label ID="lblErrMsg" runat="server" Text="" ForeColor="#cc3300"></asp:Label>
                <asp:TextBox ID="txtLoginUserId" runat="server" Width="200px" CssClass="EpiTextBox" autocomplete="Off" Font-Bold="false" AutoCompleteType="Disabled" placeholder="Username"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVUId" runat="server" ErrorMessage="Please, Specify User Name." ControlToValidate="txtLoginUserId" Display="Dynamic" ForeColor="#CC3300" SetFocusOnError="True"></asp:RequiredFieldValidator>
                <asp:TextBox ID="txtPassword" runat="server" Width="200px" autocomplete="off" Font-Bold="false" TextMode="Password" CssClass="EpiTextBox" placeholder="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVPswd" runat="server" ErrorMessage="Please, Specify Password." ControlToValidate="txtPassword" Display="Dynamic" ForeColor="#CC3300" SetFocusOnError="True"></asp:RequiredFieldValidator>
                <asp:Button ID="btnProceed" runat="server" Width="200px" Text="Login" OnClick="btnProceed_Click" CssClass="formbtnfield" />
            </form>
        </div>
        <ul class="bg-bubbles">
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
        </ul>
    </div>
    <script src="../jquery/2.1.3/jquery.min.js"></script>
    <script src="../js/index.js"></script>
</body>
</html>
