<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_ErrorMessage.aspx.cs" 
    Inherits="Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages.Stcl_ErrorMessage" MasterPageFile="~/WebPages/EpicorErp.Master" %>

<%--VirtualPath to access master page control from child page--%>
<%@ MasterType VirtualPath="~/WebPages/EpicorErp.Master" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .msgTxt
        {
            background: url(../images/s-save.gif) no-repeat 0 -1px;
            padding: 4px 0 8px 28px;
            height: 40px;
            font-weight: bold !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container">
        <table cellpadding="0" cellspacing="0" border="0" width="100%" class="formtbl">
            <tr>
                <td valign="middle" align="center" height="35">
                    <asp:Label ID="lblMessage" runat="server" CssClass="msgTxt"></asp:Label>
                </td>
            </tr>
            <tr>
                <td valign="middle" align="center">
                    <asp:Button ID="btnOk" runat="server" OnClick="btnOk_Click" Text="OK" CssClass="formbtnfield" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
