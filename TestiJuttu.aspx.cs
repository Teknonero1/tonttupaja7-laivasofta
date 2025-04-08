using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

namespace Kalaverkko
{
    public partial class TestiJuttu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindShipNames();
            }
        }

        private void BindShipNames()
        {
            string selectedPort = "FIHEL"; // Example port code, you can change this as needed
            List<string> shipNames = GetArrivingShips(selectedPort, 24);

            ddlShips.Items.Clear();
            foreach (string shipName in shipNames)
            {
                ddlShips.Items.Add(new ListItem(shipName, shipName));
            }
        }

        public static List<string> GetArrivingShips(string portCode, int hours)
        {
            string jsonData = FetchPortCallData();
            JObject portCallData = JObject.Parse(jsonData);
            JArray portCalls = (JArray)portCallData["portCalls"];
            List<string> shipNames = new List<string>();

            DateTime now = DateTime.UtcNow;
            DateTime futureTime = now.AddHours(hours);

            foreach (var portCall in portCalls)
            {
                string port = portCall["portCode"]?.ToString();
                DateTime? eta = portCall["eta"]?.ToObject<DateTime?>();

                if (port == portCode && eta.HasValue && eta.Value >= now && eta.Value <= futureTime)
                {
                    string shipName = portCall["vessel"]["name"]?.ToString();
                    if (!string.IsNullOrEmpty(shipName))
                    {
                        shipNames.Add(shipName);
                    }
                }
            }

            return shipNames;
        }

        private static string FetchPortCallData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://meri.digitraffic.fi/api/port-call/v1/port-calls");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (WebResponse response = request.GetResponse())
            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
