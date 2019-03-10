using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReadingBusesNewAPI
{
    public class BusService 
    {
        [JsonProperty("route_code")]
        public string ServiceId { get; set; }   

        [JsonProperty("group_name")]
        public string BrandName { get; set; }   

        public List<string> Stops { get; set; } = new List<string>();


        public void GetLocations(string APIKEY)
        {
            //First get a list of all the locations/ bus stops in Reading.
            var locations = JsonConvert.DeserializeObject<List<Location>>(new System.Net.WebClient().DownloadString("https://rtl2.ods-live.co.uk/api/busstops?key=" + APIKEY));
            foreach (var location in locations)
                if (!Location.Locations.ContainsKey(location.ActoCode))
                    Location.Locations.Add(location.ActoCode, location);

            //Then get a list of all the stops this bus/service stops at, only storing the ID values which act as lookup/key values.
            Stops = JsonConvert.DeserializeObject<List<Location>>(new System.Net.WebClient().DownloadString("https://rtl2.ods-live.co.uk/api/linePatterns?key=" + APIKEY + "&service=" + ServiceId)).Select(p => p.ActoCode).ToList();
        }
    }

    public class Location
    {
        //If you were to expand this program and need to query multiple services they will share common stops.
        //Ie: multiple services will stop at the same stop. So instead of storing data about locations multiple times
        //have one public static variable storing stop detials which you can look-up using acto-codes/ IDs.
        public static Dictionary<string, Location> Locations = new Dictionary<string, Location>();

        [JsonProperty("location_code")]
        public string ActoCode { get; set; }

        [JsonProperty("description")]
        public string CommonName { get; set; }
    }


    public class LiveRecord
    {
        public string ServiceNumber { get; set; }
        public string Destination { get; set; }
        public DateTime SchArrival { get; set; }
        public DateTime? ExptArrival { get; set; }

        public static List<LiveRecord> GetLiveData(string actoCode, string APIKEY)
        {
            XDocument doc = XDocument.Load("https://rtl2.ods-live.co.uk/api/siri/sm?key=" + APIKEY + "&location=" + actoCode);
            XNamespace ns = doc.Root.GetDefaultNamespace();
            List<LiveRecord> Arrivals = new List<LiveRecord>();
            Arrivals = doc.Descendants(ns + "MonitoredStopVisit").Select(x => new LiveRecord()
            {
                ServiceNumber = (string)x.Descendants(ns + "LineRef").FirstOrDefault(),
                Destination = (string)x.Descendants(ns + "DestinationName").FirstOrDefault(),
                SchArrival = (DateTime)x.Descendants(ns + "AimedArrivalTime").FirstOrDefault(),
                ExptArrival = (DateTime?)x.Descendants(ns + "ExpectedArrivalTime").FirstOrDefault(),
            }).ToList();

            return Arrivals;
        }
    }
}
