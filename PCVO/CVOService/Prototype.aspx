<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Prototype.aspx.cs" Inherits="CVOService.Prototype" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
     <form id="form1" runat="server">

        [start]
        <asp:TextBox runat="server" id="begin_time">

        </asp:TextBox>
        <asp:TextBox runat="server" id="begin_date">
        </asp:TextBox>
        <br />
        [end]
        <asp:TextBox runat="server" id="end_time">

        </asp:TextBox>
        <asp:TextBox runat="server" id="end_date">

        </asp:TextBox>
         <asp:Button runat="server" id="submit" OnClick="submit_Click"/>
        <br />
         <asp:GridView ID="gvTimeline" runat="server">
         </asp:GridView>
         <asp:GridView ID="gvEvent" runat="server">
         </asp:GridView>
    </form>
</body>
</html>
