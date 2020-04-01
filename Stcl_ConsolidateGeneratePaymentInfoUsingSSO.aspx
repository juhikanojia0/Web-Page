<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_ConsolidateGeneratePaymentInfoUsingSSO.aspx.cs"
    MasterPageFile="~/WebPages/Epicor905Erp.Master" Inherits="Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages.Stcl_ConsolidateGeneratePaymentInfoUsingSSO" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <title>Consolidated generate payment information</title>    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>   
     <script type="text/javascript">
         var prm = Sys.WebForms.PageRequestManager.getInstance();
         //Raised before processing of an asynchronous postback starts and the postback request is sent to the server.
         prm.add_beginRequest(BeginRequestHandler);
         // Raised after an asynchronous postback is finished and control has been returned to the browser.
         prm.add_endRequest(EndRequestHandler);
         function BeginRequestHandler(sender, args) {
             //Shows the modal popup - the update progress
             var popup = $find('<%= modalPopup.ClientID %>');
             if (popup != null) {
                 popup.show();
             }
         }

         function EndRequestHandler(sender, args) {
             //Hide the modal popup - the update progress
             var popup = $find('<%= modalPopup.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
        }
    </script>
    <asp:UpdateProgress ID="UpdateProgress" runat="server">
        <ProgressTemplate>
            <asp:Image ID="Image1" ImageUrl="../Images/big_indicator.gif" AlternateText="Processing" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>
    <ajaxToolkit:ModalPopupExtender ID="modalPopup" runat="server" TargetControlID="UpdateProgress"
        PopupControlID="UpdateProgress" BackgroundCssClass="modalProgressRedBackground" />
    <div class="container">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div style="width: 650px; margin: 0 auto !important;" runat="server" id="Div1">
                    <table border="1" style="font-family: Arial; font-size: 9pt; width: 650px; color: black; border-collapse: collapse; height: 100%; margin: 5px auto;">
                        <tr>
                            <td style="color: blue; text-align: center;">
                                <h3>Generate Payment Consolidated Data
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: blue; text-align: center;" colspan="6">
                                <table>
                                    <tr>
                                        <td>Select Company:</td>
                                        <td>
                                            <asp:DropDownList ID="ddlCompany" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td>Enter Bank Account Id:</td>
                                        <td>
                                            <asp:TextBox ID="txtBankAccId" runat="server" MaxLength="50"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnGetGPData" runat="server" Text="Get Data" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 650px; margin: 0 auto !important;" runat="server" id="DivGeneratePaymentData" class="scrollinghmnew">
                    <table border="1" style="font-family: Arial; font-size: 9pt; width: 640px; color: black; border-collapse: collapse; height: 100%; margin: 0 auto;">
                        <tr>
                            <td style="vertical-align: central; width: 40%; border: 0;">
                                <asp:GridView ID="grvGeneratePaymentConsolidatedData" runat="server" AutoGenerateColumns="false" ShowHeader="true" CssClass="grdth"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="true" Width="100%" CellSpacing="2" CellPadding="5" border="0"
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
               <%-- <div style="width: 600px; margin: 0 auto !important;" runat="server">
                    <center>
                        <asp:Button ID="btnBack" runat="server" Text="Back"  class="formbtnfield" OnClick="btnBack_Click"/>                         
                    </center>
                </div>--%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>   
</asp:Content>

