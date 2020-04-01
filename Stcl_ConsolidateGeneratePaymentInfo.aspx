<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_ConsolidateGeneratePaymentInfo.aspx.cs"
    MasterPageFile="~/WebPages/EpicorErp.Master" Inherits="Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages.Stcl_ConsolidateGeneratePaymentInfo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <title>Consolidated generate payment information</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .PopupBG {
            position: absolute;
            top: 0%;
            left: 0%;
            width: 1600px;
            height: 650px;
            background-color: Black;
            opacity: 0.6;
        }

        .Popup {
            position: absolute;
            top: 50%;
            left: 35%;
            font-weight: 900;
            border: 2px solid #6AB3DB;
            color: #006621;
        }
    </style>
    <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
    <script src="../js/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function ValidateDDLCompany() {          
            var ddlComp = document.getElementById('<%= ddlCompany.ClientID %>');
            var chkSalaryPay = document.getElementById('<%= chkSalaryPay.ClientID %>');
            if ((chkSalaryPay != null) && (chkSalaryPay.checked == 0)) {
                if (ddlComp.selectedIndex == 0) {
                    alert('Please, Select company to search Voucher List Number.');
                    return false;
                }
            }
            else if (chkSalaryPay == null)
            {
                if (ddlComp.selectedIndex == 0) {
                    alert('Please, Select company to search Voucher List Number.');
                    return false;
                }
            }
            DisplayProgress();
        }

        function DisplayProgress() {
            document.getElementById('pnlProgress').style.visibility = 'visible';
        }
        function HideProgressPanel() {
            document.getElementById('pnlProgress').style.visibility = "hidden";
        }

        function EnableDiableControl() {
            var ddlComp = document.getElementById('<%= ddlCompany.ClientID %>');
            var btnGetData = document.getElementById('<%= btnGetData.ClientID %>');
            var chkSalaryPay = document.getElementById('<%= chkSalaryPay.ClientID %>');
            if (chkSalaryPay.checked == 1) {
                ddlComp.disabled = 'disabled';
                btnGetData.innerText = 'Search Payment Ref Number';
            }
            else {
                ddlComp.disabled = '';
                btnGetData.innerText = 'Search Voucher List Number';
            }
        }
    </script>
    <div id="pnlProgress" class="PopupBG" style="visibility: hidden; align-items: center;">
        <table class="Popup">
            <tr style="height: 40px; width: 200px;">
                <td align="center" bgcolor="#FFFFFF" style="font-size: larger">Processing...</td>
            </tr>
            <tr>
                <td align="center" bgcolor="#FFFFFF" style="height: 40px; width: 200px;">
                    <asp:Image ID="Image1" ImageUrl="../Images/ProgressBar.gif" AlternateText="Processing" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div class="container">
        <div style="width: 100%; margin: 0 auto !important;" runat="server" id="Div1">
            <table border="1" style="font-family: Arial; font-size: 9pt; width: 100%; color: black; border-collapse: collapse; height: 100%; margin: 5px auto;">
                <tr>
                    <td style="text-align: center;">
                        <h3>Generate Payment Consolidated Data</h3>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;">&nbsp;</td>
                </tr>
                <tr>
                    <td style="text-align: center;">
                        <center>
                        <table style="width: auto">
                            <tr>
                                <td>
                                    <asp:Label ID="lblSalaryPayment" runat="server" Text="Salary Payment:"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox runat="server" ID="chkSalaryPay" onclick="javascript:EnableDiableControl()"></asp:CheckBox> 
                                </td>
                                <td>Select Company:</td>
                                <td>
                                    <asp:DropDownList ID="ddlCompany" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>Enter Bank Account Id:</td>
                                <td>
                                    <asp:TextBox ID="txtBankAccId" runat="server" CssClass="EpiTextBox" MaxLength="50"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="btnGetData" runat="server" OnClientClick="return ValidateDDLCompany();" OnClick="btnGetData_Click" Text="Search Voucher List Number" CssClass="formbtnfield" Width="225px" />
                                </td>
                            </tr>
                        </table>
                        </center>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="grvGeneratePaymentConsolidatedData" runat="server" AutoGenerateColumns="false" ShowHeader="true" CssClass="grdth"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="true" Width="100%" CellSpacing="2" CellPadding="5" border="1px"
                            HeaderStyle-Height="50px" Style="height: 100%; overflow: auto" OnRowCommand="grvGeneratePaymentConsolidatedData_RowCommand"
                            OnRowDataBound="grvGeneratePaymentConsolidatedData_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Sr.No." ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSrNo" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vote" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSrcCompany" runat="server" Text='<%# Eval("SrcCompany") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Bank Account Id" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBankAcctID" runat="server" Text='<%# Eval("BankAcctID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Voucher List Number" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVoucherListNum" runat="server" Text='<%# Eval("VoucherListNum") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total Records To Generate Payment" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalRecord" runat="server" Text='<%# Eval("TotalRecord") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Generate Payment" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkGeneratePay" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                            CommandName="Proceed" Text="Generate Payment" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
