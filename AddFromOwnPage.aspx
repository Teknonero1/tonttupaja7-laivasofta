<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFromOwnPage.aspx.cs" Inherits="Kalaverkko.AddFromOwnPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
              <%
Response.Write("<br>");
Server.Execute("Datafile.aspx");
Response.Write("<br>");
Response.Write("Hello"); %>

        </div>
    </form>
</body>
</html>
