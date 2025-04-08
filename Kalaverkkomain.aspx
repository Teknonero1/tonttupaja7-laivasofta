<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kalaverkkomain.aspx.cs" Inherits="Kalaverkko.Kalaverkkomain" %>


<!DOCTYPE html>
            <!-- https://miro.com/app/board/uXjVIeA5MfQ=/ -->

<html lang="fi" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link rel="stylesheet" href="StyleSheet1.css"/>
    <link href='https://fonts.googleapis.com/css?family=Graduate' rel='stylesheet'/>   <!-- Lisää graduate fontin -->
    <link href='https://fonts.googleapis.com/css?family=Open+Sans' rel='stylesheet'/>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
    <meta charset="utf-8" />
    <title>Suklaatehdas</title>
</head>
<body>
    
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />


                
    <div class="main">

        <div id="map"></div>

        <!-- <img src="kartta/pngtree-police-officer-holding-stop-sign-clipart-illustration-png-image_16269640.png" alt="Italian Trulli">-->

        <asp:HiddenField ID="hfSelectedMarker" runat="server" />
        <asp:Button ID="btnPostBack" runat="server" Style="display:none;" OnClick="MarkerSelected" />


        <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
        <script>
            // Initialize the map and set its view
            var map = L.map('map').setView([60.084, 26.923], 7); // [Latitude, Longitude], Zoom Level

            // Add the OpenStreetMap tile layer
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; OpenStreetMap contributors'
            }).addTo(map);



            var nykyinenlaiva = null;

            function laivaliikkuu(blatitude, blongitude) {
                if (nykyinenlaiva) {
                    map.removeLayer(nykyinenlaiva);
                }
               nykyinenlaiva = L.marker([blatitude, blongitude]).addTo(map).update();
                //alert("Ei vittu Make :D");
            }

            function laivaliikkuutesti() {
                var nykyinenlaivani = L.marker([1, 2])
                    .addTo(map).update();
                //alert("Ei vittu Make :D");
            }

            //L.marker([59.956, 23.6577]).addTo(map)
            //    .bindPopup("Centered here!")
            //    .openPopup();

            // Function to handle marker clicks (except Helsinki)
            function harborInfo(name) {
                document.getElementById('<%= hfSelectedMarker.ClientID %>').value = name;
                document.getElementById('<%= btnPostBack.ClientID %>').click();
            }

            function selectedtag(name) {

            }

            // Function to change the background color when clicking Helsinki
            function changeBackgroundColor() {
                document.body.style.backgroundColor = "lightblue"; // Change to any color you like
            }

            function changeBackgroundColor2() {
                document.body.style.backgroundColor = "red"; // Change to any color you like
            }


            // Add a marker
            var maarianhamina = L.marker([60.1006, 19.9511]).addTo(map)
                .bindPopup("Maarianhamina")
                .on('click', function () { harborInfo("Maarianhamina"); });

            var eckero = L.marker([60.2164, 19.6121]).addTo(map)
                .bindPopup("Eckerö")
                .on('click', function () { harborInfo("Eckerö"); });

            var naantali = L.marker([60.4671, 22.0246]).addTo(map)
                .bindPopup("Naantali")
                .on('click', function () { harborInfo("Naantali"); });

            var turku = L.marker([60.4352, 22.2266]).addTo(map)
                .bindPopup("Turku")
                .on('click', function () { harborInfo("Turku"); });

            var hanko = L.marker([59.8235, 22.9682]).addTo(map)
                .bindPopup("Hanko")
                .on('click', function () { harborInfo("Hanko"); });

            var helsinki = L.marker([60.1695, 24.9499]).addTo(map)
                .bindPopup("Helsinki")
                .on('click', function () { harborInfo("Helsinki"); changeBackgroundColor(); });

            var kilpilahti = L.marker([60.3364, 25.4712]).addTo(map)
                .bindPopup("Kilpilahti")
                .on('click', function () { harborInfo("Kilpilahti"); });

            var haminaKotka = L.marker([60.4649, 26.9456]).addTo(map)
                .bindPopup("Hamina-Kotka")
                .on('click', function () { harborInfo("Hamina-Kotka"); });


            var redIcon = L.icon({
                iconUrl: 'path/to/red-icon.png', // Replace with the actual path to your red icon
                iconSize: [25, 41], // size of the icon
                iconAnchor: [12, 41], // point of the icon which will correspond to marker's location
                popupAnchor: [1, -34], // point from which the popup should open relative to the iconAnchor
                shadowUrl: 'path/to/marker-shadow.png', // Replace with the actual path to your shadow icon
                shadowSize: [41, 41] // size of the shadow
            });

        </script>

        <div class="valikko">
            <div class="päivä">
                <asp:Label ID="lblDateTime" runat="server" Text=""></asp:Label>
            </div>
            <div class="nappi">
                <!-- <button> PÄIVITÄ HAKU </button> -->
            </div>
            <div class="satama">
                <label>SATAMA</label>
                <asp:DropDownList ID="ddlSatama" runat="server" AutoPostBack="true" 
                    OnSelectedIndexChanged="ddlSatama_SelectedIndexChanged">
                    <asp:ListItem Text="Maarianhaminan Länsisatama" Value="Maarianhamina" />
                    <asp:ListItem Text="Eckerö" Value="Eckerö" />
                    <asp:ListItem Text="Naantali" Value="Naantali" />
                    <asp:ListItem Text="Turku" Value="Turku" />
                    <asp:ListItem Text="Hanko" Value="Hanko" />
                    <asp:ListItem Text="Helsinki" Value="Helsinki" />
                    <asp:ListItem Text="Kilpilahti" Value="Kilpilahti" />
                    <asp:ListItem Text="Hamina-Kotka" Value="Hamina-Kotka" />
                </asp:DropDownList>
                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
            </div>

            <div class="alus">
                <label>ALUS</label>
                <asp:DropDownList ID="ddlAlus" runat="server" AutoPostBack="true" 
                    OnSelectedIndexChanged="ddlAlus_SelectedIndexChanged">
                    <asp:ListItem Text="Valitse alus" Value="Valitse alus" />
                </asp:DropDownList>
                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

            </div>

        </div>
        <div class="alatiedot">

        <div class="tiedot">
    <h1>Laivan nimi: <asp:Label ID="lblShipName" runat="server" Text="Juusto"></asp:Label></h1>
    <p>Sijainti: <asp:Label ID="lblLocation" runat="server" Text="Paikka"></asp:Label></p>
    <p>Todellinen lähtöaika: <asp:Label ID="lblActualDeparture" runat="server" Text="Aika"></asp:Label></p>
    <p>Arvioitu saapumisaika: <asp:Label ID="lblEstimatedArrival" runat="server" Text="Aika"></asp:Label></p>
</div>

<div class="lisätiedot">
    <p>Viime satama: <asp:Label ID="lblLastPort" runat="server" Text="SESTO - Tukholma, Ruotsi"></asp:Label></p>
    <ul><asp:Label ID="lblLastPortLink" runat="server" Text="- linkki"></asp:Label></ul>
    <p>Seuraava satama: <asp:Label ID="lblNextPort" runat="server" Text="FITKU - Turku, Suomi"></asp:Label></p>
    <ul><asp:Label ID="lblNextPortLink" runat="server" Text="- linkki"></asp:Label></ul>
</div>


            </div>
        </div>
        </form>
    </body>
        </html>
