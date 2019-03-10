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

        private const string APIKEY = "[INSERT-API-KEY-HERE]"; //Get your own from http://opendata.reading-travelinfo.co.uk

        static void Main(string[] args)
        {
            List<BusService> services = new List<BusService>();   //Used to store a list of Bus services
            string serviceOption;   //Used to store the user choice for which service to view
            string stopOption;      //Used to store the user choice for which bus stop to view.

            //Find a list of all the bus routes operating in Reading and then a list of all the stops each service stops at.
            XDocument Services = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/servicepatterns.xml?key=" + APIKEY);
            foreach (XElement Service in Services.Root.Descendants("ServicePattern"))
            {
                services.Add(new BusService(Service.Element("ServiceId").Value));  //Creates a new bus based upon its bus route ID
                foreach (XElement Locs in Service.Descendants("Locations").Descendants("Location"))
                {
                    services[services.Count - 1].Locations.Add(new Location() //Adds a new location to this bus route/ service
                    {
                        Id = Convert.ToString(Locs.Element("Id").Value),
                        Direction = Convert.ToString(Locs.Element("Direction").Value),
                        DisplayOrder = Convert.ToString(Locs.Element("DisplayOrder").Value)
                    });
                }
            }


            //Grab the operator and description
            XDocument BusRoutes = XDocument.Load("http://opendata.reading-travelinfo.co.uk/api/1/bus/services.xml?key=" + APIKEY);
            foreach (XElement Service in BusRoutes.Root.Descendants("Services").Descendants("Service"))
                if (services.Any(p => p.ServiceId == Service.Element("Id").Value))
                    services.Single(p => p.ServiceId == Service.Element("Id").Value).UpdateDetails(Service.Element("Description").Value, Service.Element("Operator").Value);


            //Prints the operator and bus ID					
            foreach (BusService bus in services)
                Console.WriteLine(bus.Operator + "  " + bus.ServiceId);

            //Ask the user which service they would like to view, while they do not enter a valid service keep asking
            do
            {
                Console.WriteLine("\nWhich bus service do you want to review? Please enter it's service number.");
                serviceOption = Console.ReadLine();
            } while (!services.Any(p => p.ServiceId == serviceOption));
            Console.Clear();

        
            //Shows all the bus stop IDs and common names for every stop on the selected service
            foreach (var locs in services.Single(p => p.ServiceId == serviceOption).Locations)
                Console.WriteLine("ID : " + locs.Id + "	:	" + Location.GetLocationName(locs.Id, APIKEY));
        
            //Ask the user which stop they would like to monitor live times for, while they do not enter a valid
            //bus stop acto-code/ ID keep asking.
            do
            {
                Console.WriteLine("\nWhich stop do you wish to review? Please enter it's ID value.");
                stopOption = Console.ReadLine();
            } while (!services.Single(p => p.ServiceId == serviceOption).Locations.Any(x => x.Id == stopOption));
            Console.Clear();

            //Forever keep looping every 30s getting the current live data for the bus stop and outputing it.
            while (true)
            {
                List<LiveRecord> Arrivals = LiveRecord.GetLiveData(stopOption, APIKEY);
                
                Console.WriteLine(DateTime.Now.ToString());
                foreach (LiveRecord Time in Arrivals)
                    Console.WriteLine(Time.Service + "   " + Time.Destination + "		" + Time.SchArrival + "		" + Time.ExptArrival);

                Console.WriteLine("");
                System.Threading.Thread.Sleep(30000);
            }
        }
    }
}
