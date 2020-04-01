<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_GeneratePaymentUsingSSO.aspx.cs" MasterPageFile="~/WebPages/EpicorErp.Master"
    Inherits="Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages.Stcl_GeneratePaymentUsingSSO" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <title>Consolidated generate payment information</title>
    <script type="text/javascript">
        function CheckSum() {
            var sum = 0.00;
            var itemsum = 0.00;
            var IsSelected = false;
            //debugger;

            if (document.getElementsByTagName) {
                var Table = document.getElementById("<%= grvGeneratePayment.ClientID%>");
                var Rows = Table.getElementsByTagName("tr");

                for (i = 1; i < Rows.length; i++) {
                    var VarCheck = Rows[i].cells[1].children[0].id;
                    var InvoiceAmt = Rows[i].cells[7].children[0].id;

                    if (!isNaN((document.getElementById(InvoiceAmt).innerHTML).replace(/,/g, ""))) {
                        if (document.getElementById(VarCheck).checked) {
                            itemsum = parseFloat((document.getElementById(InvoiceAmt).innerHTML).replace(/,/g, ""));
                            sum += itemsum;

                            if (!IsSelected) {
                                IsSelected = true;
                            }
                        }
                    }
                }

                var totallbl = document.getElementById("<%=lblTotalAmountSelectedValue.ClientID %>");
                totallbl.innerHTML = CurrencyFormat((Math.round(sum * 100) / 100).toFixed(2)).toString();

                if (!IsSelected) {
                    document.getElementById("<%= btnSave.ClientID%>").style.display = 'none';
                }
                else {
                    document.getElementById("<%= btnSave.ClientID%>").style.display = 'block';
                }
            }
        }


        function CurrencyFormat(number) {
            var decimalplaces = 2;
            var decimalcharacter = ".";
            var thousandseparater = ",";
            number = parseFloat(number);
            var sign = number < 0 ? "-" : "";
            var formatted = new String(number.toFixed(decimalplaces));
            if (decimalcharacter.length && decimalcharacter != ".") { formatted = formatted.replace(/\./, decimalcharacter); }
            var integer = "";
            var fraction = "";
            var strnumber = new String(formatted);
            var dotpos = decimalcharacter.length ? strnumber.indexOf(decimalcharacter) : -1;
            if (dotpos > -1) {
                if (dotpos) { integer = strnumber.substr(0, dotpos); }
                fraction = strnumber.substr(dotpos + 1);
            }
            else { integer = strnumber; }
            if (integer) { integer = String(Math.abs(integer)); }
            while (fraction.length < decimalplaces) { fraction += "0"; }
            temparray = new Array();
            while (integer.length > 3) {
                temparray.unshift(integer.substr(-3));
                integer = integer.substr(0, integer.length - 3);
            }
            temparray.unshift(integer);
            integer = temparray.join(thousandseparater);
            return sign + integer + decimalcharacter + fraction;
        }

        function VerifyAnyRecordSelected() {
            var IsSelected = false;
            if (document.getElementsByTagName) {
                var Table = document.getElementById("<%= grvGeneratePayment.ClientID%>");
                var Rows = Table.getElementsByTagName("tr");
                //debugger;
                for (i = 1; i < Rows.length ; i++) {
                    var VarCheck = Rows[i].cells[1].children[0].id;
                    if (Rows[i].cells[1].children[0].type == 'checkbox' && document.getElementById(VarCheck).checked) {
                        if (document.getElementById(VarCheck).checked) {
                            IsSelected = true;
                        }
                    }
                }
            }
            if (!IsSelected) {
                alert('Select checkbox from generate payment column, \n\r for transaction which you want to generate payment entry.')
            }
            return IsSelected;
        }

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" AsyncPostBackTimeout="36000"></cc1:ToolkitScriptManager>
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
                <div style="width: 100%;">
                    <table style="width: 100%;">
                        <tr>
                            <td colspan="6">
                                <h3 align="center">Generate Payment</h3>
                            </td>
                        </tr>
                        <tr style="color: blue;">
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblSourceCompany" runat="server" Text="Source Company"></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblGenerator" runat="server" Text="Generator"></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblBanckAccountId" runat="server" Text="Bank Account Id"></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblSourceGroupId" runat="server" Text="Voucher List Number"></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblVendorId" runat="server" Text="Vendor Id"></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblLegalNumber" runat="server" Text="LegalNumber"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblTxtSourceCompany" runat="server" Text=""></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblTxtGenerator" runat="server" Text=""></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblTxtBankAccountId" runat="server" Text=""></asp:Label>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:Label ID="lblTxtSourceGroupId" runat="server" Text=""></asp:Label>
                                <%--<asp:TextBox ID="txtSourceGroupId" runat="server"></asp:TextBox>--%>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:TextBox ID="txtVendorId" runat="server"></asp:TextBox>
                            </td>
                            <td align="center" style="vertical-align: central;">
                                <asp:TextBox ID="txtLegalNumber" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" align="center" style="vertical-align: central">
                                <asp:Button ID="btnGetData" runat="server" Text="Get Data" CssClass="formbtnfield" AutoPostBack="true" BackColor="#ACDCF7" Width="90px" OnClick="btnGetData_Click" />
                                &nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="formbtnfield" AutoPostBack="true" BackColor="#ACDCF7" Width="90px" OnClick="btnBack_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 100%;" runat="server" id="DivGeneratePaymentData" class="scrollinghm">
                    <table>
                        <tr>
                            <td style="vertical-align: top; width: 100%; border: 0;">
                                <asp:GridView ID="grvGeneratePayment" runat="server" AutoGenerateColumns="false" ShowHeader="true"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="true" Width="100%" CellSpacing="1" CellPadding="3" border="1"
                                    HeaderStyle-Height="50px" OnRowDataBound="grvGeneratePayment_RowDataBound" ShowFooter="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr.No." ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSrNo" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Generate Payment" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="98px" HeaderStyle-Width="98px">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkAction" runat="server" onclick="javascript:CheckSum()" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Legal Number" DataField="LegalNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="144px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="144px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Supplier Name" DataField="VendorName" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="212px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="212px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Invoice Number" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInvoiceNum" runat="server" Text='<%# Eval("InvoiceNum") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Voucher List Number" DataField="SrcGroupID" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="CPO Invoice Group" DataField="GroupID" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Invoice Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="150px" HeaderStyle-Width="150px" HeaderStyle-VerticalAlign="Middle">
                                            <ItemTemplate>
                                                <asp:Label ID="txtInvoiceAmount" runat="server" Text='<%# Eval("InvoiceAmt") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Currency Code" DataField="CurrencyCode" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Invoice Apply Date" DataField="ApplyDate" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Vendor Num" DataField="VendorNum" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Vendor Id" DataField="VendorID" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Sub Budget Cls" DataField="SubBudgetCls" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="PMUID" DataField="PMUID" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="hiddencol" ItemStyle-Width="0%" HeaderStyle-CssClass="hiddencol" HeaderStyle-Width="0px">
                                            <HeaderStyle CssClass="hiddencol" />
                                            <ItemStyle CssClass="hiddencol" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Payment Method" DataField="PayMethodName" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Source Company" DataField="SrcCompany" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="hiddencol" ItemStyle-Width="0%" HeaderStyle-CssClass="hiddencol" HeaderStyle-Width="0px">
                                            <HeaderStyle CssClass="hiddencol" />
                                            <ItemStyle CssClass="hiddencol" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Setup Validation" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px" HeaderStyle-Width="150px" HeaderStyle-VerticalAlign="Middle">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSetupMissing" runat="server" Style="color: red;" Text='<%# Eval("SetupError") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="Comment" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-Width="150px">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtComment" TextMode="MultiLine"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                    </Columns>
                                    <%--<HeaderStyle HorizontalAlign="Right" />
                                    <PagerSettings Visible="False" />--%>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 30%; margin: 5px auto;" runat="server" id="DivAction">
                    <table style="width: 100%;" border="1">
                        <tr>
                            <td style="width: 50%;" align="right">
                                <asp:Label ID="lblTotalAmount" Font-Bold="true" runat="server" Text="Total Invoice Amount" ForeColor="Blue"></asp:Label>
                            </td>
                            <td style="width: 50%; padding: 2px;" align="right">
                                <asp:Label ID="lblTotalAmountValue" Font-Bold="true" runat="server" Text="" ForeColor="Blue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="lblTotalSelectedAmount" Font-Bold="true" runat="server" Text="Total Selected Invoice Amount" ForeColor="Blue"></asp:Label>
                            </td>
                            <td align="right" style="padding: 2px">
                                <asp:Label ID="lblTotalAmountSelectedValue" Font-Bold="true" runat="server" Text="0.00" ForeColor="Blue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="vertical-align: central" colspan="2">
                                <asp:Button ID="btnSave" runat="server" Text="Generate Payment" CssClass="formbtnfield" AutoPostBack="true" BackColor="#ACDCF7" OnClientClick="return VerifyAnyRecordSelected()" OnClick="btnSave_Click" />
                                &nbsp;
                                    <asp:Button ID="btnHidden" runat="server" Style="display: none;" OnClick="btnHidden_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
