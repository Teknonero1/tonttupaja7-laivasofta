<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFromOwnPage.aspx.cs" Inherits="Kalaverkko.AddFromOwnPage" %>

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta charset="utf-8" />
    <title></title>    
</head>
<body>
    <form id="form1" runat="server">   
                      <%
Response.Write("<br>");
Server.Execute("Datafile.aspx");
Response.Write("<br>");
Response.Write("Hello"); %>

    </form>
</body>
</html>
