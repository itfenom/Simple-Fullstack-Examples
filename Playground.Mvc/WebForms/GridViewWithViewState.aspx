<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridViewWithViewState.aspx.cs" Inherits="Playground.Mvc.WebForms.GridViewWithViewState" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            <div id="gridViewArea">
                <asp:GridView ID="dgvIssuenceLogEntry" runat="server"
                    ShowFooter="True" AutoGenerateColumns="False"
                    CellPadding="4" ForeColor="#333333"
                    GridLines="None" OnRowDeleting="dgvIssuenceLogEntry_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="RowNumber" HeaderText="" />
                        <asp:TemplateField HeaderText="User">
                            <ItemTemplate>
                                <asp:TextBox ID="txtUser" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txtUser" ForeColor="Red" ErrorMessage="*" SetFocusOnError="True">
                                </asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Part Number">
                            <ItemTemplate>
                                <asp:TextBox ID="txtPartNumber" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                    ControlToValidate="txtPartNumber" ForeColor="Red" ErrorMessage="*" SetFocusOnError="True">
                                </asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Lot ID">
                            <ItemTemplate>
                                <asp:TextBox ID="txtLotID" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    ControlToValidate="txtLotID" ForeColor="Red" ErrorMessage="*" SetFocusOnError="True">
                                </asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Expiration Date">
                            <ItemTemplate>
                                <asp:TextBox ID="txtExpirationDate" runat="server" CausesValidation="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                    ControlToValidate="txtExpirationDate" ForeColor="Red" ErrorMessage="*" SetFocusOnError="True">
                                </asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantity">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                    ControlToValidate="txtQuantity" ForeColor="Red" ErrorMessage="*" SetFocusOnError="True">
                                </asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comments">
                            <ItemTemplate>
                                <asp:TextBox ID="txtComments" runat="server" Height="50px" TextMode="MultiLine"></asp:TextBox>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Right" />
                            <FooterTemplate>
                                <asp:Button ID="ButtonAdd" runat="server" Text="Add New Row" OnClick="ButtonAdd_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowDeleteButton="True" />
                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                    <EditRowStyle BackColor="#2461BF" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </div>
            <br />
            <div id="errorMsgArea">
                <table>
                    <tr runat="server" id="MessagesRow" visible="False">
                        <td colspan="2">
                            <asp:Label runat="server" ID="MessageValue" ForeColor="Red" />
                        </td>
                        <td>
                            <asp:Button runat="server" ID="OkButton" Text="OK" OnClick="OkButton_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <div id="submitArea">
                <asp:Button runat="server" ID="SubmitButton" Text="Submit" OnClick="SubmitButton_Click" />
            </div>
        </div>

        <br />
        <div id="exportSectionDiv" visible="false">
            <h4>Select From and To date to Export data to Excel:</h4>
            <table width="40%">
                <tr>
                    <td width="20%">
                        <table class="style5">
                            <tr>
                                <td width="25%">From :
                                </td>
                                <td width="25%">
                                    <asp:Label ID="lblFromDate" runat="server" Text="mm/dd/yyyy"></asp:Label>
                                </td>
                                <td width="50%">
                                    <asp:RadioButton ID="rbFromDate" runat="server" GroupName="DateDiff" Checked="true" />
                                </td>
                            </tr>
                            <tr>
                                <td width="20%">To :
                                </td>
                                <td width="25%">
                                    <asp:Label ID="lblToDate" runat="server" Text="mm/dd/yyyy"></asp:Label>
                                </td>
                                <td width="55%">
                                    <asp:RadioButton ID="rbToDate" runat="server" GroupName="DateDiff" />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" width="20%">
                        <table class="style5">
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Calendar ID="Calendar1" runat="server" Height="137px" Width="288px" OnSelectionChanged="Calandar1_SelectionChanged"></asp:Calendar>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Button runat="server" ID="btnExportToExcel" Text="Export to Excel" OnClick="ExportToExcelButton_Click" />
            <br />
        </div>
    </form>
</body>
</html>