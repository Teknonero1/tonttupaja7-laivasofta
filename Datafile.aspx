<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Datafile.aspx.cs" Inherits="Kalaverkko.Datafile" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>


<!-- JOITAIN LAIVAN HAKEMIS CONDITIONEJA SAAATTA OLLA VÄÄRIN -->





<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Kalaverkko - Alustiedot</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 40px;
        }
        .container {
            max-width: 700px;
            margin: auto;
        }
        .section {
            margin-bottom: 30px;
        }
        label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
        }
        .result, .ship-info {
            margin-top: 15px;
            padding: 10px;
            background-color: #f3f3f3;
            border: 1px solid #ddd;
            border-radius: 6px;
        }
    </style>
</head>
<body>
    <form id="form2" runat="server">
        <div class="container">
            <h2>Kalaverkko - Saapuvat alukset</h2>

            <!-- Sataman valinta -->
            <div class="section satama">
                <label for="ddlSatama">Valitse satama</label>
                <asp:DropDownList ID="ddlSatama" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlSatama_SelectedIndexChanged">
                    <asp:ListItem Text="Valitse satama" Value="Valitse satama" />
                    <asp:ListItem Text="Maarianhaminan Länsisatama" Value="Maarianhaminan Länsisatama" />
                    <asp:ListItem Text="Eckerö" Value="Eckerö" />
                    <asp:ListItem Text="Naantali" Value="Naantali" />
                    <asp:ListItem Text="Turku" Value="Turku" />
                    <asp:ListItem Text="Hanko" Value="Hanko" />
                    <asp:ListItem Text="Helsinki" Value="Helsinki" />
                    <asp:ListItem Text="Kilpilahti" Value="Kilpilahti" />
                    <asp:ListItem Text="Hamina-Kotka" Value="Hamina-Kotka" />
                </asp:DropDownList>
                <div class="result">
                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                </div>
            </div>

            <!-- Aluksen valinta -->
            <div class="section alus">
                <label for="ddlAlus">Valitse alus</label>
                <asp:DropDownList ID="ddlAlus" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlAlus_SelectedIndexChanged">
                    <asp:ListItem Text="Valitse alus" Value="Valitse alus" />
                </asp:DropDownList>
                <div class="ship-info">
                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>
    </form>
</body>
</html>