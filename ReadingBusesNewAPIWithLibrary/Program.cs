using ReadingBusesAPI;
using System;
using System.Globalization;
using System.Threading.Tasks;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.JourneyDetails;
using ReadingBusesAPI.Common;

namespace ReadingBusesNewAPIWithLibrary
{
	/// <summary>
	/// A simple program which lets you select a bus service, then a bus stop and get live data from that bus stop.
	/// </summary>
    class Program
    {
        static async Task Main(string[] args) {
            // Optional - Default is true, Caches Bus Stops and Bus Services Data (by default for 7 days) as you can assume this to not change regularly.
            ReadingBuses.SetCache(true);
            //Initializes the controller, enter in your Reading Buses API - Get your own from http://rtl2.ods-live.co.uk/cms/apiservice
            //Once Instantiated you can also use, "ReadingBuses.GetInstance();" to get future instances.
            ReadingBuses controller =  await ReadingBuses.Initialise("[API KEY]");

	        const Company busCompany = Company.ReadingBuses; //Used to store which operator to get data from.
			string serviceOption;   //Used to store the user choice for which service to view
            string stopOption;      //Used to store the user choice for which bus stop to view.

            //Prints off all services running in Reading.
            foreach (var busService in controller.GetServices(busCompany))
	            Console.WriteLine(busService.ServiceId + " " + busService.BrandName);
			
	            

			do
            {
                Console.WriteLine("\nWhich bus service do you want to review? Please enter it's service number.");
                serviceOption = Console.ReadLine();
            } while (!controller.IsService(serviceOption, busCompany)); //Checks the value entered is a ReadingBuses Service
            Console.Clear();

            //Gets that service and stores it in a BusServices Object.
            BusService service = controller.GetService(serviceOption, busCompany);

            ////Prints off the name and code of every bus stop visited by that service.
            foreach (var location in await service.GetLocations())
                Console.WriteLine(location.CommonName + " : " + location.ActoCode);

			do
			{
				Console.WriteLine("\nWhich stop do you wish to review? Please enter it's ID value.");
				stopOption = Console.ReadLine();
			} while (!controller.IsLocation(stopOption)); //Checks the value entered is a Acto-Code for a bus stop.
			Console.Clear();

			//Gets that bus stop and stores it in a BusStop Object.
			BusStop stop = controller.GetLocation(stopOption);

			//Loops continuously.
			while (true)
			{
				//Gets Live Data from the bus stop selected.
				LiveRecord[] arrivals = await stop.GetLiveData();

				//Prints off the data for each service due to arrive at the stop.
				Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));
				foreach (LiveRecord bus in arrivals)
					Console.WriteLine(bus.ServiceNumber + " - " + bus.Service().BrandName + "   " + bus.Destination + "		" + bus.SchArrival + "		" + bus.ExptArrival);

				Console.WriteLine("");
				System.Threading.Thread.Sleep(30000);
			}
		}
    }
}
