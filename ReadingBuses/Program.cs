using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace ReadingBuses
{
    class Program
	{
        /// <summary>
        /// This is a very basic program demonstrating some basic usage of the Reading Open Data
        /// API. To use this program simply edit the API Key below. Please note, this API is 
        /// now outdated. I would recommend using the Reading Buses API instead!
        /// </summary>

		private const string APIKEY = "[INSERT API KEY HERE]"; //Get your own from http://opendata.reading-travelinfo.co.uk

        static void Main(string[] args)
		{
			List<Bus> buses = new List<Bus>();

			//Grab the bus routes and stop IDs.
			XDocument Services = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/servicepatterns.xml?key=" + APIKEY);
            foreach (XElement Service in Services.Root.Descendants("ServicePattern"))
			{
				buses.Add(new Bus(Service.Element("ServiceId").Value));  //Creates a new bus based upon its bus route ID
                foreach (XElement Locs in Service.Descendants("Locations").Descendants("Location"))
                {
                    buses[buses.Count - 1].locations.Add(new Location() //Adds a new location to this bus route/ service
                    {
                        Id = Convert.ToString(Locs.Element("Id").Value),
                        direction = Convert.ToString(Locs.Element("Direction").Value),
                        DisplayOrder = Convert.ToString(Locs.Element("DisplayOrder").Value)
                    });
                }
			}

            
			//Grab the operator and description
			XDocument BusRoutes = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/services.xml?key=" + APIKEY);
			foreach (XElement Service in BusRoutes.Root.Descendants("Services").Descendants("Service"))
                if(buses.Any(p => p.ServiceId == Service.Element("Id").Value))
                    buses.Single(p => p.ServiceId == Service.Element("Id").Value).UpdateDetails(Service.Element("Description").Value, Service.Element("Operator").Value);


            //Prints the operator and bus ID					
            foreach (Bus bus in buses)		
				Console.WriteLine(bus.Operator + "  " + bus.ServiceId );
			Console.WriteLine("\nWhich bus service do you want to review?");

			string serviceOption = Console.ReadLine();
			Console.Clear();

            if(buses.Any(p => p.ServiceId == serviceOption))
            {
                //Shows all the bus stop IDs and common names for every stop on the selected service
                foreach (var locs in buses.Single(p => p.ServiceId == serviceOption).locations)
                    Console.WriteLine("ID : " + locs.Id + "	:	" + LocName(locs.Id));
            }
            else
            {
                Console.WriteLine("No Bus Service of that ID can be found.");
                Console.ReadLine();
                Environment.Exit(0);
            }


			Console.WriteLine("\nWhich stop do you wish to review?");
			string stopOption = Console.ReadLine();
			Console.Clear();
            while (true)
            {
                //List<BLocation> Arrivals = new List<BLocation>();
                XDocument LiveTimes = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/calls/" + stopOption + "?key=" + APIKEY);
                XNamespace ns = LiveTimes.Root.GetDefaultNamespace();

                List<BLocation> Arrivals = LiveTimes.Descendants("Call").Select(x => new BLocation()
                {
                    Service = (string)x.Element(ns + "Service"),
                    Destination = (string)x.Element(ns + "Destination"),
                    SchArrival = (DateTime)x.Element(ns + "ScheduledArrival"),
                    ExptArrival = (DateTime?)x.Descendants(ns + "ExpectedArrival").FirstOrDefault(),
                }).ToList();


                foreach (BLocation Time in Arrivals)
                    Console.WriteLine(Time.Service + "   " + Time.Destination + "		" + Time.SchArrival + "		" + Time.ExptArrival);

                Console.WriteLine("");
                System.Threading.Thread.Sleep(30000);
            }
		}

        private static string LocName(string id)	//Used to retrieve the plain text name from a bus stops ID.
		{
            //The only way of getting the common name on this API is to make an individual call to every single stop
            //and then extract the stop name from the header.
			XDocument LiveTimes = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/calls/" + id + "?key="+ APIKEY);
			return LiveTimes.Root.Element("Name").Value.ToString(); 
		}
	}
}
