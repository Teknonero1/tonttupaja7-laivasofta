using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kalaverkko
{
    public partial class Datafile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlSatama_SelectedIndexChanged(ddlSatama, EventArgs.Empty);
                ddlAlus_SelectedIndexChanged(ddlAlus, EventArgs.Empty);
            }
        }

        protected void ddlSatama_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = ddlSatama.SelectedValue;
            ddlAlus.Items.Clear();
            //ddlAlus.Items.Add(new ListItem("Valitse alus", "Valitse alus"));
            ddlAlus.Items.Remove(ddlAlus.Items.FindByValue("Laivoja ei löytynyt"));

            if (selectedPort == "Valitse satama")
            {
                lblResult.Text = "";
                return;
            }

            Dictionary<string, string> portCodes = new Dictionary<string, string>
            {
                { "Maarianhamina", "FIMHQ" },
                { "Eckerö", "FIECK" },
                { "Naantali", "FINLI" },
                { "Turku", "FITKU" },
                { "Hanko", "FIHKO" },
                { "Helsinki", "FIHEL" },
                { "Kilpilahti", "FISKV" },
                { "Hamina-Kotka", "FIKTK" }
            };

            if (!portCodes.TryGetValue(selectedPort, out string portCode))
            {
                lblResult.Text = "Tuntematon satama.";
                return;
            }

            try
            {
                var portCalls = GetPortCallsFromApi();
                DateTime now = DateTime.UtcNow;
                DateTime next24Hours = now.AddHours(24);

                var filteredShips = portCalls.Where(ship =>
                    ship.PortToVisit == portCode &&
                    !string.IsNullOrEmpty(ship.PrevPort) &&
                    !ship.PrevPort.StartsWith("FI") &&
                    ship.PortAreaDetails?.Any(detail =>
                        !detail.Ata.HasValue &&
                        detail.Eta.HasValue &&
                        detail.Eta > now &&
                        detail.Eta <= next24Hours
                    ) == true)
                    .OrderBy(ship => ship.PortAreaDetails?.FirstOrDefault()?.Eta)
                    .ToList();



                if (filteredShips.Any())
                {
                    foreach (var ship in filteredShips)
                    {
                        string etaTime = ship.PortAreaDetails?.FirstOrDefault()?.Eta?.ToString("dd.MM.yyyy HH:mm") ?? "";
                        string originInfo = !string.IsNullOrEmpty(ship.PrevPort) && !ship.PrevPort.StartsWith("FI") ? "" : "";
                        ddlAlus.Items.Add(new ListItem($"{ship.VesselName}{originInfo}", ship.VesselName));
                        //Haetaan sijainti MMSI:llä uudella logiikalla

                    }
                    lblResult.Text = $"Löytyi {filteredShips.Count} alusta, joka saapuu seuraavan 24 tunnin sisällä.";
                    ddlAlus_SelectedIndexChanged(ddlAlus, EventArgs.Empty);
                }
                else
                {
                    lblResult.Text = "Ei löytynyt aluksia, jotka saapuvat seuraavan 24 tunnin sisällä valitulle satamalle.";
                    ddlAlus.Items.Add(new ListItem("Laivoja ei löytynyt", "Laivoja ei löytynyt"));
                    //ddlAlus_SelectedIndexChanged(ddlAlus, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Virhe alusten haussa: {ex.Message}";
            }
        }

        protected void MarkerSelected(object sender, EventArgs e)
        {
            string selectedMarker = hfSelectedMarker.Value;
            // Call your desired method here, for example:
            ClickHarbor(selectedMarker);
        }

        public void ClickHarbor(string selectedTag)
        {
            ddlSatama.ClearSelection(); //making sure the previous selection has been cleared
            ddlSatama.Items.FindByValue(selectedTag).Selected = true;
            ddlSatama_SelectedIndexChanged(ddlSatama, EventArgs.Empty);
            ddlAlus_SelectedIndexChanged(ddlAlus, EventArgs.Empty);
        }

        protected void ddlAlus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedShipWithInfo = ddlAlus.SelectedValue;
            string selectedShip = selectedShipWithInfo.Split('(')[0].Trim();
            if (selectedShip == "Valitse alus")
            {
                lblShipName.Text = "Valitse alus";
                lblLocation.Text = "???";
                lblActualDeparture.Text = "???";
                lblEstimatedArrival.Text = "???";
                lblLastPort.Text = "???"; ;
                lblLastPortLink.Text = "- linkki"; // Update with actual link if needed
                lblNextPort.Text = "???";
                lblNextPortLink.Text = "- linkki"; // Update with actual link if needed
                return;
            }

            try
            {
                var portCalls = GetPortCallsFromApi();
                var selectedShipData = portCalls.FirstOrDefault(ship =>
                    ship.VesselName == selectedShip &&
                    !ship.PrevPort.StartsWith("FI"));

                if (selectedShipData != null)
                {
                    string portName = GetPortName(selectedShipData.PortToVisit);
                    string arrivalTime = selectedShipData.PortAreaDetails?.FirstOrDefault()?.Eta?.ToString("dd.MM.yyyy HH:mm") ?? "ei saatavilla";
                    string departureTime = selectedShipData.PortAreaDetails?.FirstOrDefault()?.Etd?.ToString("dd.MM.yyyy HH:mm") ?? "ei saatavilla";
                    string berth = selectedShipData.PortAreaDetails?.FirstOrDefault()?.BerthName ?? "ei saatavilla";
                    string originType = !string.IsNullOrEmpty(selectedShipData.PrevPort) ?
                        (selectedShipData.PrevPort.StartsWith("FI") ? "" : "") : "Tuntematon";
                    string theNextPort = selectedShipData.NextPort;
                    string nextPortFirstTwoLetters = !string.IsNullOrEmpty(theNextPort) && theNextPort.Length >= 2
                        ? theNextPort.Substring(0, 2)
                        : "??"; // Default value if NextPort is null or too short
                    string thePrevPort = selectedShipData.PrevPort;
                    string prevPortFirstTwoLetters = !string.IsNullOrEmpty(thePrevPort) && thePrevPort.Length >= 2
                        ? thePrevPort.Substring(0, 2)
                        : "??"; // Default value if NextPort is null or too short


                    var locationRequest = (HttpWebRequest)WebRequest.Create("https://meri.digitraffic.fi/api/ais/v1/locations");
                    locationRequest.AutomaticDecompression = DecompressionMethods.GZip;
                    var locationResponse = locationRequest.GetResponse();

                    string locationJson;
                    using (var stream = locationResponse.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        locationJson = reader.ReadToEnd();
                    }

                    var locationRoot = JObject.Parse(locationJson);
                    var locationFeatures = locationRoot["features"] as JArray;

                    string lat = null, lon = null;
                    if (selectedShipData.mmsi != "0")
                    {
                        var feature = locationFeatures?.FirstOrDefault(f =>
                        f["mmsi"]?.ToString() == selectedShipData.mmsi);
                        if (feature != null)
                        {
                            var coords = feature["geometry"]?["coordinates"] as JArray; if (coords != null && coords.Count == 2)
                            {
                                lon = coords[0]?.ToString();
                                lat = coords[1]?.ToString();
                                string alat = lat.Replace(',', '.');
                                string alon = lon.Replace(',', '.');
                                if (lon == "" || lat == "")
                                {

                                    lblLocation.Text = "Sijaintia ei löytynyt";
                                }
                                else
                                {

                                    lblLocation.Text = $@"{alat}, {alon}";
                                    string scripti = $"laivaliikkuu({alat}, {alon});";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "laivaliikkuu", scripti, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        lblLocation.Text = "Sijaintia ei löytynyt";
                    }

                    string wikipediaUrl1 = "??";
                    string wikipediaUrl2 = "??";

                    lblShipName.Text = selectedShipData.VesselName;
                    //lblLocation.Text = $@"{lon}, {lat}";
                    lblEstimatedArrival.Text = arrivalTime;
                    lblActualDeparture.Text = departureTime;
                    lblLastPort.Text = selectedShipData.PrevPort;
                    if (getCountryData.countryWikipediaUrls.TryGetValue(prevPortFirstTwoLetters, out wikipediaUrl2))
                    {
                        // Use the URL as needed
                        lblLastPortLink.Text = $"<a href=\"{wikipediaUrl2}\" target=\"_blank\">{wikipediaUrl2}</a>";
                    }
                    else
                    {
                        lblLastPortLink.Text = $"No Wikipedia URL found for country code: {prevPortFirstTwoLetters}";
                    }
                    lblNextPort.Text = selectedShipData.NextPort;
                    if (getCountryData.countryWikipediaUrls.TryGetValue(nextPortFirstTwoLetters, out wikipediaUrl1))
                    {
                        // Use the URL as needed
                        lblNextPortLink.Text = $"<a href=\"{wikipediaUrl1}\" target=\"_blank\">{wikipediaUrl1}</a>";
                    }
                    else
                    {
                        lblNextPortLink.Text = $"No Wikipedia URL found for country code: {nextPortFirstTwoLetters}";
                    }

                }
                else
                {
                    lblShipName.Text = $@"Aluksen {selectedShipData.VesselName} tietoja ei löytynyt.";
                    lblLocation.Text = "???";
                    lblEstimatedArrival.Text = "???";
                    lblActualDeparture.Text = "???";
                    lblLastPort.Text = "???"; ;
                    lblLastPortLink.Text = "- linkki"; // Update with actual link if needed
                    lblNextPort.Text = "???";
                    lblNextPortLink.Text = "- linkki"; // Update with actual link if needed
                }
            }
            catch (Exception ex)
            {
                lblShipName.Text = $"Virhe alustietojen haussa: {ex.Message}";
                lblLocation.Text = "???";
                lblActualDeparture.Text = "???";
                lblEstimatedArrival.Text = "???";
                lblLastPort.Text = "???"; ;
                lblLastPortLink.Text = "- linkki"; // Update with actual link if needed
                lblNextPort.Text = "???";
                lblNextPortLink.Text = "- linkki"; // Update with actual link if needed
            }
        }

        private List<PortCall> GetPortCallsFromApi()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://meri.digitraffic.fi/api/port-call/v1/port-calls");
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    var apiResponse = JsonConvert.DeserializeObject<PortCallApiResponse>(responseFromServer);
                    return apiResponse.PortCalls;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"API-yhteyden virhe: {ex.Message}");
            }
        }

        private string GetPortName(string portCode)
        {
            switch (portCode)
            {
                case "FIMHQ": return "Maarianhamina";
                case "FINLI": return "Naantali";
                case "FITKU": return "Turku";
                case "FIHKO": return "Hanko";
                case "FIHEL": return "Helsinki";
                case "FISKV": return "Kilpilahti";
                case "FIECK": return "Eckerö";
                case "FIKTK": return "Kotka-Hamina";
                default: return portCode;
            }
        }
    }

    public class PortCallApiResponse
    {
        public DateTime DataUpdatedTime { get; set; }
        public List<PortCall> PortCalls { get; set; }
    }

    public class PortCall
    {
        public int PortCallId { get; set; }
        public string PortToVisit { get; set; }
        public string PrevPort { get; set; }
        public string NextPort { get; set; }
        public string VesselName { get; set; }
        public string mmsi { get; set; }
        public string Nationality { get; set; }
        public List<PortAreaDetail> PortAreaDetails { get; set; }
    }

    public class PortAreaDetail
    {
        public string PortAreaCode { get; set; }
        public string BerthCode { get; set; }
        public string BerthName { get; set; }
        public DateTime? Eta { get; set; }
        public DateTime? Etd { get; set; }
        public DateTime? Ata { get; set; }
    }
    public class getCountryData
    {
        public static readonly Dictionary<string, string> countryWikipediaUrls = new Dictionary<string, string>
        {
{ "FI", "https://fi.wikipedia.org/wiki/Suomi" },
{ "SE", "https://fi.wikipedia.org/wiki/Ruotsi" },
{ "EE", "https://fi.wikipedia.org/wiki/Viro" },
{ "LV", "https://fi.wikipedia.org/wiki/Latvia" },
{ "LT", "https://fi.wikipedia.org/wiki/Liettua" },
{ "DE", "https://fi.wikipedia.org/wiki/Saksa" },
{ "DK", "https://fi.wikipedia.org/wiki/Tanska" },
{ "NL", "https://fi.wikipedia.org/wiki/Alankomaat" },
{ "BE", "https://fi.wikipedia.org/wiki/Belgia" },
{ "PL", "https://fi.wikipedia.org/wiki/Puola" },
{ "NO", "https://fi.wikipedia.org/wiki/Norja" },
{ "RU", "https://fi.wikipedia.org/wiki/Venäjä" },
{ "FR", "https://fi.wikipedia.org/wiki/Ranska" },
{ "GB", "https://fi.wikipedia.org/wiki/Yhdistynyt_kuningaskunta" },
{ "IE", "https://fi.wikipedia.org/wiki/Irlanti" },
{ "IT", "https://fi.wikipedia.org/wiki/Italia" },
{ "ES", "https://fi.wikipedia.org/wiki/Espanja" },
{ "PT", "https://fi.wikipedia.org/wiki/Portugali" },
{ "GR", "https://fi.wikipedia.org/wiki/Kreikka" },
{ "TR", "https://fi.wikipedia.org/wiki/Turkki" },
{ "CN", "https://fi.wikipedia.org/wiki/Kiina" },
{ "JP", "https://fi.wikipedia.org/wiki/Japani" },
{ "KR", "https://fi.wikipedia.org/wiki/Etelä-Korea" },
{ "IN", "https://fi.wikipedia.org/wiki/Intia" },
{ "US", "https://fi.wikipedia.org/wiki/Yhdysvallat" },
{ "CA", "https://fi.wikipedia.org/wiki/Kanada" },
{ "BR", "https://fi.wikipedia.org/wiki/Brasilia" },
{ "SG", "https://fi.wikipedia.org/wiki/Singapore" },
{ "AE", "https://fi.wikipedia.org/wiki/Yhdistyneet_arabiemiirikunnat" },
{ "EG", "https://fi.wikipedia.org/wiki/Egypti" },
{ "MA", "https://fi.wikipedia.org/wiki/Marokko" },
{ "IL", "https://fi.wikipedia.org/wiki/Israel" },
{ "UA", "https://fi.wikipedia.org/wiki/Ukraina" }
        };
    }
    public class getCountryDataOld
    {
        private string getCountryWikipedia(string portCodeStart)
        {
            switch (portCodeStart)
            {
                case "FI": return "https://fi.wikipedia.org/wiki/Suomi";
                case "PL": return "https://fi.wikipedia.org/wiki/Puola";
                default: return "Ei löydy :(";
            }
        }
    }
}