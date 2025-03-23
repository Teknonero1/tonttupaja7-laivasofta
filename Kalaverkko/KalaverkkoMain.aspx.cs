using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;

namespace Kalaverkko
{
    public partial class Kalaverkkomain : System.Web.UI.Page
    {
        protected string alongitude;
        protected string alatitude;

        int testnumber;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                ddlSatama_SelectedIndexChanged(ddlSatama, EventArgs.Empty);
                ddlAlus_SelectedIndexChanged(ddlAlus, EventArgs.Empty);
            }


            // Set the current date and time
            lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy<br>HH:mm");
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


        protected void ddlSatama_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected value from the dropdown
            string selectedPort = ddlSatama.SelectedValue;

            // Perform an action, such as displaying the selected option
            //lblResult.Text = "Selected port: " + selectedPort;

            ddlAlus.Items.Clear();

            // You can also execute other logic here, like database operations
            switch (selectedPort)
            {
                case "Maarianhamina":
                    ddlAlus.Items.Add(new ListItem("M/S Gabriella", "M/S Gabriella"));
                    ddlAlus.Items.Add(new ListItem("FGS Seeadler", "FGS Seeadler"));
                    ddlAlus.Items.Add(new ListItem("M/S Umpolumpo", "M/S Umpolumpo"));
                    ddlAlus.Items.Add(new ListItem("HMS Nordstjärnä", "HMS Nordstjärnä"));
                    ddlAlus.Items.Add(new ListItem("ORP Biay Orzei", "ORP Biay Orzei"));
                    ddlAlus.Items.Add(new ListItem("MS Zilvestorm", "MS Zilvestorm"));
                    break;
                case "Eckerö":
                    ddlAlus.Items.Add(new ListItem("M/S Rosella", "M/S Rosella"));
                    ddlAlus.Items.Add(new ListItem("M/S Eckero", "M/S Eckero"));
                    break;
                case "Naantali":
                    ddlAlus.Items.Add(new ListItem("M/S Finnlines", "M/S Finnlines"));
                    ddlAlus.Items.Add(new ListItem("M/S Baltic Queen", "M/S Baltic Queen"));
                    break;
                case "Turku":
                    ddlAlus.Items.Add(new ListItem("M/S Gabriella", "M/S Gabriella"));
                    ddlAlus.Items.Add(new ListItem("M/S Viking Grace", "M/S Viking Grace"));
                    ddlAlus.Items.Add(new ListItem("M/S Silja Symphony", "M/S Silja Symphony"));
                    ddlAlus.Items.Add(new ListItem("M/S Amorella", "M/S Amorella"));
                    break;
                case "Hanko":
                    ddlAlus.Items.Add(new ListItem("M/S Bore", "M/S Bore"));
                    ddlAlus.Items.Add(new ListItem("M/S Regal Star", "M/S Regal Star"));
                    break;
                case "Helsinki":
                    ddlAlus.Items.Add(new ListItem("M/S Silja Serenade", "M/S Silja Serenade"));
                    ddlAlus.Items.Add(new ListItem("M/S Viking XPRS", "M/S Viking XPRS"));
                    ddlAlus.Items.Add(new ListItem("M/S Star", "M/S Star"));
                    break;
                case "Kilpilahti":
                    ddlAlus.Items.Add(new ListItem("M/S Öljypohatta", "M/S Öljypohatta"));
                    ddlAlus.Items.Add(new ListItem("M/S Transfennica", "M/S Transfennica"));
                    break;
                case "Hamina-Kotka":
                    ddlAlus.Items.Add(new ListItem("M/S Finnmaid", "M/S Finnmaid"));
                    ddlAlus.Items.Add(new ListItem("M/S Transfennica", "M/S Transfennica"));
                    break;
                default:
                    ddlAlus.Items.Add(new ListItem("Select a port first", ""));
                    break;
            }
        }
        protected void ddlAlus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedShip = ddlAlus.SelectedValue;

            // Update the labels with information based on the selected ship
            lblShipName.Text = selectedShip;
            lblLocation.Text = GetLocation(selectedShip);
            lblActualDeparture.Text = GetActualDeparture(selectedShip);
            lblEstimatedArrival.Text = GetEstimatedArrival(selectedShip);
            lblLastPort.Text = GetLastPort(selectedShip);
            lblLastPortLink.Text = "- linkki"; // Update with actual link if needed
            lblNextPort.Text = GetNextPort(selectedShip);
            lblNextPortLink.Text = "- linkki"; // Update with actual link if needed

        // Example coordinates for the selected ship
        //alatitude = "54.4649"; // Replace with actual latitude
        //alongitude = "28.9456"; // Replace with actual longitude
            if (!IsPostBack)
            {
                alongitude = "28.9456";
                alatitude = "60.4649";
            }
            else
            {
                alongitude = "0";
                alatitude = "60.4649";
            }

            // Call the laivaliikkuu JavaScript function with the coordinates
            //string skripti = $"laivaliikkuu({alatitude}, {alongitude})";
            // ScriptManager.RegisterStartupScript(this, this.GetType(), "laivaliikkuu", "laivaliikkuu()", true);
            string scripti = $"laivaliikkuu({alatitude}, {alongitude});";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "laivaliikkuu", scripti, true);
        }

        // Example methods to get information based on the selected ship
        private string GetLocation(string ship)
        {
            // Return location based on the ship
            return "Example Location";
        }

        private string GetActualDeparture(string ship)
        {
            // Return actual departure time based on the ship
            return "Example Departure Time";
        }

        private string GetEstimatedArrival(string ship)
        {
            // Return estimated arrival time based on the ship
            return "Example Arrival Time";
        }

        private string GetLastPort(string ship)
        {
            // Return last port based on the ship
            return "Example Last Port";
        }

        private string GetNextPort(string ship)
        {
            // Return next port based on the ship
            return "Example Next Port";
        }

    }
}