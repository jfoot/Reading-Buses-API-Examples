using ReadingBusesAPI;
using System;
using System.Collections.Generic;

namespace ReadingBusesNewAPIWithLibrary
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args) {
            // Optional - Default is true, Caches Bus Stops and Bus Services Data (by default for 7 days) as you can assume this to not change regularly.
            ReadingBuses.SetCache(true);
            //Initializes the controller, enter in your Reading Buses API - Get your own from http://rtl2.ods-live.co.uk/cms/apiservice
            //Once Instantiated you can also use, "ReadingBuses.GetInstance();" to get future instances.
            ReadingBuses Controller =  await ReadingBuses.Initialise("");
            
            string serviceOption;   //Used to store the user choice for which service to view
            string stopOption;      //Used to store the user choice for which bus stop to view.

            //Prints off all services running in Reading.
            Controller.PrintServices();

            do
            {
                Console.WriteLine("\nWhich bus service do you want to review? Please enter it's service number.");
                serviceOption = Console.ReadLine();
            } while (!Controller.IsService(serviceOption)); //Checks the value entered is a ReadingBuses Service
            Console.Clear();

            //Gets that service and stores it in a BusServices Object.
            BusService service = Controller.GetService(serviceOption);

            //Prints off the name and code of every bus stop visited by that service.
            foreach (var location in service.GetLocations())
                Console.WriteLine(location.CommonName + " : " + location.ActoCode);

            do
            {
                Console.WriteLine("\nWhich stop do you wish to review? Please enter it's ID value.");
                stopOption = Console.ReadLine();
            } while (!Controller.IsLocation(stopOption)); //Checks the value entered is a Acto-Code for a bus stop.
            Console.Clear();

            //Gets that bus stop and stores it in a BusStop Object.
            BusStop stop = Controller.GetLocation(stopOption);

            //Loops continuously.
            while (true)
            {
                //Gets Live Data from the bus stop selected.
                List<LiveRecord> Arrivals = stop.GetLiveData();

                //Prints off the data for each service due to arrive at the stop.
                Console.WriteLine(DateTime.Now.ToString());
                foreach (LiveRecord bus in Arrivals)
                    Console.WriteLine(bus.ServiceNumber +" - " + bus.Service().BrandName + "   " + bus.Destination + "		" + bus.SchArrival + "		" + bus.ExptArrival);

                Console.WriteLine("");
                System.Threading.Thread.Sleep(30000);
            }
        }
    }
}
