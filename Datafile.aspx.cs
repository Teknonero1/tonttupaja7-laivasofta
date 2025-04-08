using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace Kalaverkko
{
    public partial class Datafile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void ddlSatama_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = ddlSatama.SelectedValue;
            ddlAlus.Items.Clear();
            ddlAlus.Items.Add(new ListItem("Valitse alus", "Valitse alus"));

            if (selectedPort == "Valitse satama")
            {
                lblResult.Text = "";
                return;
            }

            Dictionary<string, string> portCodes = new Dictionary<string, string>
            {
                { "Maarianhaminan Länsisatama", "FIMHQ" },
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

                var filteredShips = portCalls.Where(pc =>
                    pc.PortToVisit == portCode &&
                    pc.PortAreaDetails?.Any(detail =>
                        !detail.Ata.HasValue &&
                        detail.Eta.HasValue &&
                        detail.Eta > now &&
                        detail.Eta <= next24Hours
                    ) == true)
                    .OrderBy(pc => pc.PortAreaDetails?.FirstOrDefault()?.Eta)
                    .ToList();

                if (filteredShips.Any())
                {
                    foreach (var ship in filteredShips)
                    {
                        string etaTime = ship.PortAreaDetails?.FirstOrDefault()?.Eta?.ToString("dd.MM.yyyy HH:mm") ?? "";
                        string originInfo = !string.IsNullOrEmpty(ship.PrevPort) && !ship.PrevPort.StartsWith("FI") ? "" : "";
                        ddlAlus.Items.Add(new ListItem($"{ship.VesselName}{originInfo}", ship.VesselName));
                    }
                    lblResult.Text = $"Löytyi {filteredShips.Count} alusta, joka saapuu seuraavan 24 tunnin sisällä.";
                }
                else
                {
                    lblResult.Text = "Ei löytynyt aluksia, jotka saapuvat seuraavan 24 tunnin sisällä valitulle satamalle.";
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Virhe alusten haussa: {ex.Message}";
            }
        }

        protected void ddlAlus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedShip = ddlAlus.SelectedValue;
            if (selectedShip == "Valitse alus")
            {
                Label1.Text = "";
                return;
            }

            try
            {
                var portCalls = GetPortCallsFromApi();
                var selectedShipData = portCalls.FirstOrDefault(pc => pc.VesselName == selectedShip);

                if (selectedShipData != null)
                {
                    string portName = GetPortName(selectedShipData.PortToVisit);
                    string arrivalTime = selectedShipData.PortAreaDetails?.FirstOrDefault()?.Eta?.ToString("dd.MM.yyyy HH:mm") ?? "ei saatavilla";
                    string departureTime = selectedShipData.PortAreaDetails?.FirstOrDefault()?.Etd?.ToString("dd.MM.yyyy HH:mm") ?? "ei saatavilla";
                    string berth = selectedShipData.PortAreaDetails?.FirstOrDefault()?.BerthName ?? "ei saatavilla";
                    string originType = !string.IsNullOrEmpty(selectedShipData.PrevPort) && !selectedShipData.PrevPort.StartsWith("FI") ? "" : "";

                    Label1.Text = $@"
                        <strong>Alus:</strong> {selectedShipData.VesselName}<br>
                        <strong>Satama:</strong> {portName}<br>
                        <strong>Arvioitu saapumisaika:</strong> {arrivalTime}<br>
                        <strong>Arvioitu lähtöaika:</strong> {departureTime}<br>
                        <strong>Laituri:</strong> {berth}<br>
                        <strong>Edellinen satama:</strong> {selectedShipData.PrevPort}{originType}<br>
                        <strong>Seuraava satama:</strong> {selectedShipData.NextPort}";
                }
                else
                {
                    Label1.Text = "Aluksen tietoja ei löytynyt.";
                }
            }
            catch (Exception ex)
            {
                Label1.Text = $"Virhe alustietojen haussa: {ex.Message}";
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
}