<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormTest.aspx.cs" Inherits="Playground.Mvc.WebForms.WebFormTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                This is a web-form!
            </p>
            <asp:GridView ID="gridViewPerson" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="EmpName" HeaderText="Name" />
                    <asp:BoundField DataField="EmpEmail" HeaderText="Email" />
                    <asp:BoundField DataField="EmpPhone" HeaderText="Phone" />
                    <asp:BoundField DataField="HireDate" HeaderText="Hire Date" />
                </Columns>
            </asp:GridView>
        </div>
        <p>
            <a runat="server" href="~/Home/Index">Home</a>
        </p>
    </form>
</body>
</html>