<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stcl_GeneratePayment.aspx.cs" MasterPageFile="~/WebPages/EpicorErp.Master"
    Inherits="Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages.Stcl_GeneratePayment" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <title>Consolidated generate payment information</title>
    <meta http-equiv="Cache-control" content="no-cache" />
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
            top: 10%;
            left: 35%;
            font-weight: 900;
            border: 2px solid #6AB3DB;
            color: #006621;
        }

        table.gridtable {
            font-family: verdana,arial,sans-serif;
            font-size: 11px;
            color: #333333;
            border-width: 1px;
            border-color: #666666;
            border-collapse: collapse;
            width: 1180px;
            margin-left: 10px;
            overflow-y:scroll; 
            max-height:400px;
        }

        table.gridtable th {
            border-width: 1px;
            padding: 8px;
            border-style: solid;
            border-color: #666666;
            background-color: #dedede;
        }

        table.gridtable td {
            border-width: 1px;
            padding: 8px;
            border-style: solid;
            border-color: #666666;
            background-color: #ffffff;
        }
    </style>
    <script type="text/javascript">

        function DisplayProgress() {
            document.getElementById('pnlProgress').style.visibility = 'visible';
        }
        function HideProgressPanel() {
            document.getElementById('pnlProgress').style.visibility = "hidden";
        }
        function DisplayStatusPopup() {
            document.getElementById('pnlStatusPopup').style.visibility = 'visible';
        }
        function HideModalPopup() {            
            var mpu = $find('modalBehavior');
            mpu.hide();
            document.getElementById('<%= tdStatusInfo.ClientID %>').innerHTML = "";

            var isFirefox = typeof InstallTrigger !== 'undefined';
            var isChrome = !!window.chrome && !!window.chrome.webstore;
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            document.getElementById('<%= hfGPValue.ClientID %>').value = "1";
            //uncheck all checkboxes after popup closed
            var GridVwHeaderChckbox = document.getElementById("<%=grvGeneratePayment.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (msie > 0)
                {
                    GridVwHeaderChckbox.rows[i].cells[0].children[0].checked = false;
                }
                else
                {
                    GridVwHeaderChckbox.rows[i].cells[0].children[0].checked = CheckboxHeader.checked;
                }
            }
            
            if (msie > 0) {
                document.getElementById('<%= btnGetData.ClientID %>').click();
                window.location.href = window.location.href;
                //window.location.replace(location.href);
            }

           
            if (isChrome == true) {
                history.go(-1);
            }
            if (isFirefox == true) {
                document.getElementById('<%= btnGetData.ClientID %>').click();
            }

            //return false;
        }
        function PrintElem() {
            var dataSrc = document.getElementById('<%= tdStatusInfo.ClientID %>').innerHTML;
            PopupPrint(dataSrc);
        }

        function PopupPrint(data) {
            var mywindow = window.open('', '', 'height=400,width=600');
            mywindow.document.write('<html><head><title>Generate Payment Status Report</title><style>table.gridtable { font-family: verdana,arial,sans-serif; font-size:11px; color:#333333; border-width: 1px; border-color: #666666; border-collapse: collapse; width: 1200px; } table.gridtable th { border-width: 1px; padding: 8px; border-style: solid; border-color: #666666; background-color: #dedede; } table.gridtable td { border-width: 1px; padding: 8px; border-style: solid; border-color: #666666; background-color: #ffffff; } </style>');
            /*optional stylesheet*/ //mywindow.document.write('<link rel="stylesheet" href="main.css" type="text/css" />');
            mywindow.document.write('</head><body >');
            mywindow.document.write(data);
            mywindow.document.write('</body></html>');
            mywindow.document.close(); // necessary for IE >= 10
            mywindow.focus(); // necessary for IE >= 10
            mywindow.print();
            mywindow.close();
            return true;
        }

        function CheckSum() {
            var sum = 0.00;
            var itemsum = 0.00;
            var IsSelected = false;
            //debugger;

            if (document.getElementsByTagName) {
                var Table = document.getElementById("<%= grvGeneratePayment.ClientID%>");
                var Rows = Table.getElementsByTagName("tr");

                for (i = 1; i < Rows.length; i++) {
                    if (Rows[i].cells[1].children[0] != null) {
                        var VarCheck = Rows[i].cells[1].children[0].id;
                        var InvoiceAmt = Rows[i].cells[7].children[1].id;

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
                temparray.unshift(integer.substr(integer.length - 3, 3));
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
            else {
                DisplayProgress();
            }
            return IsSelected;
        }

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" AsyncPostBackTimeout="36000"></cc1:ToolkitScriptManager>
    <div class="container">
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
        <ajaxToolkit:ModalPopupExtender ID="modalPopup" runat="server" TargetControlID="btnShowPopup" BehaviorID="modalBehavior"
            PopupControlID="pnlStatusPopup" BackgroundCssClass="modalProgressRedBackground" />
        <asp:Button ID="btnShowPopup" runat="server" Text="" Style="display: none;" />
        <div id="pnlStatusPopup" runat="server" style=" display:none; width: 1200px; height: 550px; left: 100px; border: 2px solid #007EC5; margin: 5px 5px 5px 5px; background-color: #ffffff;" class="scrollinghm">
            <table>
                <tr>
                    <td>
                        <asp:Button ID="btnHide" runat="server" Text="X" OnClientClick="return HideModalPopup();"/>
                        <input type="button" value="Print" onclick="PrintElem()" />
                        <asp:HiddenField ID="hfGPValue" runat="server" Value="0" />
                    </td>
                </tr>
                <tr>
                    <td runat="server" id="tdStatusInfo"></td>
                </tr>
            </table>
        </div>
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
                        <asp:Button ID="btnGetData" runat="server" Text="Search AP Invoices" CssClass="formbtnfield" AutoPostBack="true" BackColor="#ACDCF7"
                            OnClientClick="DisplayProgress();" OnClick="btnGetData_Click" />
                        &nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="formbtnfield" AutoPostBack="true" BackColor="#ACDCF7" OnClick="btnBack_Click" />
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

<ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Generate Payment" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="98px" HeaderStyle-Width="98px">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkAction" runat="server" onclick="javascript:CheckSum()" />
                                    </ItemTemplate>

<HeaderStyle Width="98px"></HeaderStyle>
<ItemStyle HorizontalAlign="Center" Width="98px"></ItemStyle>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Legal Number" DataField="LegalNumber" ItemStyle-HorizontalAlign="left" ItemStyle-Width="144px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="144px">
<HeaderStyle VerticalAlign="Middle" Width="144px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Supplier Name" DataField="VendorName" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="212px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="212px">
<HeaderStyle VerticalAlign="Middle" Width="212px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Invoice Number" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInvoiceNum" runat="server" Text='<%# Eval("InvoiceNum") %>'></asp:Label>
                                    </ItemTemplate>

<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center" Width="150px"></ItemStyle>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Voucher List Number" DataField="SrcGroupID" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CPO Invoice Group" DataField="GroupID" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Invoice Amount in Base Currency" ItemStyle-HorizontalAlign="left" ItemStyle-Width="150px" HeaderStyle-Width="150px" HeaderStyle-VerticalAlign="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInvCurrency" runat="server" Text='<%# Eval("BaseCurrency") %>'></asp:Label>
                                        <asp:Label ID="txtInvoiceAmount" runat="server" Text='<%# Eval("InvoiceAmt") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="left" Width="150px"></ItemStyle>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Invoice Amount in Invoice Currency" DataField="BankInvcAmt" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Invoice Apply Date" DataField="ApplyDate" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Vendor Num" DataField="VendorNum" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Vendor Id" DataField="VendorID" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Sub Budget Cls" DataField="SubBudgetCls" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="PMUID" DataField="PMUID" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="hiddencol" ItemStyle-Width="0%" HeaderStyle-CssClass="hiddencol" HeaderStyle-Width="0px">
                                    <HeaderStyle CssClass="hiddencol" />
                                    <ItemStyle CssClass="hiddencol" HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Payment Method" DataField="PayMethodName" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Width="150px">
<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Source Company" DataField="SrcCompany" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="hiddencol" ItemStyle-Width="0%" HeaderStyle-CssClass="hiddencol" HeaderStyle-Width="0px">
                                    <HeaderStyle CssClass="hiddencol" />
                                    <ItemStyle CssClass="hiddencol" HorizontalAlign="Center" />
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="Payment Ref No" DataField="PayRefNo" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="hiddencol" ItemStyle-Width="0%" HeaderStyle-CssClass="hiddencol" HeaderStyle-Width="0px">
                                    <HeaderStyle CssClass="hiddencol" />
                                    <ItemStyle CssClass="hiddencol" HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Setup Validation" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px" HeaderStyle-Width="150px" HeaderStyle-VerticalAlign="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSetupMissing" runat="server" Style="color: red;" Text='<%# Eval("SetupError") %>'></asp:Label>
                                    </ItemTemplate>

<HeaderStyle VerticalAlign="Middle" Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
                                </asp:TemplateField>
                                <%--<asp:TemplateField HeaderText="Comment" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px" HeaderStyle-Width="150px">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtComment" TextMode="MultiLine"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                <%--if required will remove above ode to show comment textbox after confirmation--%>
                            </Columns>
                            <%--<HeaderStyle HorizontalAlign="Right" />
                                    <PagerSettings Visible="False" />--%>

<HeaderStyle HorizontalAlign="Center" Height="50px"></HeaderStyle>
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
                        <asp:Button ID="btnSave" runat="server" Text="Generate Payment" CssClass="formbtnfield" AutoPostBack="true" BackColor="#ACDCF7"
                            OnClientClick="return VerifyAnyRecordSelected()" OnClick="btnSave_Click" />
                        &nbsp;
                                    <asp:Button ID="btnHidden" runat="server" Style="display: none;" OnClick="btnHidden_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
