using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReadingBuses
{
	class BusService
	{
		public string ServiceId { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
        public string Description { get; set; }
        private string Boperator;
        public string Operator
		{
			get { return Boperator; }
			set
			{
				switch (value)
				{
					case "RB":
						Boperator = "Reading Buses";
						break;
					case "CTNY":
						Boperator = "Courtney Buses";
						break;
					case "TH":
						Boperator = "Thames Travel";
						break;
					case "CB":
						Boperator = "Carousel Buses";
						break;
					case "SYRK":
						Boperator = "SYRK";
						break;
					default:
						Boperator = value;
						break;
				}
			}
		}

        public BusService(string id)
        {
            ServiceId = id;
        }

        public void UpdateDetails(string desc, string opr)
        {
            Description = desc;
            Operator = opr;
        }
	}

	class Location
	{
		public string Id { get; set; }
		public string Direction { get; set; }
        public string DisplayOrder { get; set; }

        public static string GetLocationName(string id, string APIKEY)
        {
            //The only way of getting the common name on this API is to make an individual call to every single stop
            //and then extract the stop name from the header.
            XDocument LiveTimes = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/calls/" + id + "?key=" + APIKEY);
            return LiveTimes.Root.Element("Name").Value.ToString();
        }
    }


	class LiveRecord
	{
		public string Service { get; set; }
        public string Destination { get; set; }
        public DateTime SchArrival { get; set; }
        public DateTime? ExptArrival { get; set; }


        public static List<LiveRecord> GetLiveData(string actoCode, string APIKEY)
        {
            XDocument LiveTimes = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/calls/" + actoCode + "?key=" + APIKEY);
            XNamespace ns = LiveTimes.Root.GetDefaultNamespace();

            List<LiveRecord> Arrivals = LiveTimes.Descendants("Call").Select(x => new LiveRecord()
            {
                Service = (string)x.Element(ns + "Service"),
                Destination = (string)x.Element(ns + "Destination"),
                SchArrival = (DateTime)x.Element(ns + "ScheduledArrival"),
                ExptArrival = (DateTime?)x.Descendants(ns + "ExpectedArrival").FirstOrDefault(),
            }).ToList();

            return Arrivals;
        }
    }
}
