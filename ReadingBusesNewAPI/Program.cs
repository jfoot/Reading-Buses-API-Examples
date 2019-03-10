using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReadingBusesNewAPI
{
    class Program
    {
        /// <summary>
        /// This program like it's predecessor gets a list of bus services operating in Reading
        /// and then lets you select a stop and monitor the live arrival times at that stop.
        /// This program is however using the new and improved Reading Buses Open Data API!
        /// </summary>

        private const string APIKEY = "[INSERT-API-KEY-HERE]"; //Get your own from http://rtl2.ods-live.co.uk/cms/apiservice

        static void Main(string[] args)
        {
            BusService[] services;  //Used to store a list of Bus services
            string serviceOption;   //Used to store the user choice for which service to view
            string stopOption;      //Used to store the user choice for which bus stop to view.

            //Find a list of all the bus routes operating in Reading.
            services = JsonConvert.DeserializeObject<BusService[]>(new System.Net.WebClient().DownloadString("https://rtl2.ods-live.co.uk/api/services?key=" + APIKEY)).OrderBy(p => Convert.ToInt32(Regex.Replace(p.ServiceId, "[^0-9.]", ""))).ToArray();

            //Print these routes and their brand names to screen.
            foreach (var service in services)
                Console.WriteLine(service.BrandName + " " + service.ServiceId);

            //Ask the user which service they would like to view, while they do not enter a valid service keep asking
            do
            {
                Console.WriteLine("\nWhich bus service do you want to review? Please enter it's service number.");
                serviceOption = Console.ReadLine();
            } while (!services.Any(p => p.ServiceId == serviceOption));
            Console.Clear();

             //Get all the locations/ stops this service stops at
             services.Single(p => p.ServiceId == serviceOption).GetLocations(APIKEY);
             //Shows all the bus stop IDs and common names for every stop on the selected service
             foreach (var locs in services.Single(p => p.ServiceId == serviceOption).Stops)
                Console.WriteLine("ID : " + locs + "	:	"  + Location.Locations[locs].CommonName);

            //Ask the user which stop they would like to monitor live times for, while they do not enter a valid
            //bus stop acto-code/ ID keep asking.
            do
            {
                Console.WriteLine("\nWhich stop do you wish to review? Please enter it's ID value.");
                stopOption = Console.ReadLine();
            } while (!services.Single(p => p.ServiceId == serviceOption).Stops.Contains(stopOption));
            Console.Clear();

            //Forever keep looping every 30s getting the current live data for the bus stop and outputing it.
            while (true)
            {
                List<LiveRecord> Arrivals = LiveRecord.GetLiveData(stopOption, APIKEY);

                Console.WriteLine(DateTime.Now.ToString());
                foreach (LiveRecord Time in Arrivals)
                    Console.WriteLine(Time.ServiceNumber + "   " + Time.Destination + "		" + Time.SchArrival + "		" + Time.ExptArrival);

                Console.WriteLine("");
                System.Threading.Thread.Sleep(30000);
            }
        }
    }
}
