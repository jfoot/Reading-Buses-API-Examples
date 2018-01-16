using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;


namespace ReadingBuses
{
	class Program
	{
		public static string APIKEY = "[INSERT YOUR API KEY HERE]"; //Get your own from http://opendata.reading-travelinfo.co.uk
		static void Main(string[] args)
		{
			List<Bus> buses = new List<Bus>();
			

			//Grab the bus routes and stop IDs.
			StringBuilder ServicesDoc = new StringBuilder();
			ServicesDoc.Append("http://opendata.reading-travelinfo.co.uk/api/1/bus/servicepatterns.xml");
			ServicesDoc.Append(APIKEY);
		
			XDocument Services = XDocument.Load(ServicesDoc.ToString());
			foreach (XElement Service in Services.Root.Descendants("ServicePattern"))
			{
					buses.Add(new Bus { ServiceId = Service.Element("ServiceId").Value });	//Creates a new bus based upon its bus route ID
					foreach (XElement Locs in Service.Descendants("Locations").Descendants("Location"))
					{
						buses[buses.Count - 1].locations.Add(new Location()	//Adds a new location to this bus route/ service
						{
							Id = Convert.ToString(Locs.Element("Id").Value),
							direction = Convert.ToString(Locs.Element("Direction").Value),
							DisplayOrder = Convert.ToString(Locs.Element("DisplayOrder").Value)
						});
					}
			}



			//Grab the operator and description
			StringBuilder BusRoutesDoc = new StringBuilder();
			BusRoutesDoc.Append("http://opendata.reading-travelinfo.co.uk/api/1/bus/services.xml");
			BusRoutesDoc.Append(APIKEY);
			XDocument BusRoutes = XDocument.Load(BusRoutesDoc.ToString());
			foreach (XElement Service in BusRoutes.Root.Descendants("Services").Descendants("Service"))
			{
				foreach (Bus BusObj in buses)
				{
					if (Convert.ToString(Service.Element("Id").Value) == Convert.ToString(BusObj.ServiceId))	//Matches the bus stop ID retrieved above with its plain text name
					{
						BusObj.Description = Service.Element("Description").Value;
						BusObj.Operator = Service.Element("Operator").Value;
					}
				}
			}

						
			foreach (Bus Cart in buses)		//Prints the operator and bus ID
			{
				Console.WriteLine(Cart.Operator + "  " + Cart.ServiceId );
			}

			Console.WriteLine("");
			Console.WriteLine("Which bus service do you want to review?");

			string option = Console.ReadLine();
			Console.Clear();

			foreach (Bus Cart in buses)
			{
				if(Cart.ServiceId == option)
				{
					foreach (var locs in Cart.locations)	//Shows all the bus stop IDs and plain text names for every stop on the selected service
					{
						Console.WriteLine("ID : " + locs.Id +"	:	" + LocName(locs.Id));
					}
				}
			}

			Console.WriteLine("");
			Console.WriteLine("Which stop do you wish to review?");
			string stop = Console.ReadLine();
			Console.Clear();

			List<BLocation> Arrivals = new List<BLocation>();
			StringBuilder LiveTimesDoc = new StringBuilder();
			LiveTimesDoc.Append("http://opendata.reading-travelinfo.co.uk/api/1/bus/calls/");
			LiveTimesDoc.Append(stop);
			LiveTimesDoc.Append(APIKEY);

			Console.WriteLine(	);

			XDocument LiveTimes = XDocument.Load(LiveTimesDoc.ToString());
			foreach (XElement Call in LiveTimes.Root.Descendants("Call"))
			{
				Arrivals.Add(new BLocation	//Creates live times for each bus service, its final destination and arrival times
				{
					Service = Call.Element("Service").Value.ToString(),
					Destination = Call.Element("Destination").Value.ToString(),
					SchArrival = DateTime.ParseExact(Call.Element("ScheduledArrival").Value.ToString(), "yyyy-MM-ddTHH:mm:ss+ff:ff", System.Globalization.CultureInfo.InvariantCulture)

				});

				foreach (XElement Expct in Call.Descendants("MonitoredCall"))
					Arrivals[Arrivals.Count - 1].ExptArrival = DateTime.ParseExact(Expct.Element("ExpectedArrival").Value.ToString(), "yyyy-MM-ddTHH:mm:ss+ff:ff", System.Globalization.CultureInfo.InvariantCulture);
			}

			foreach (BLocation Time in Arrivals)	//If Expt Arrival is 0001, then a time has not yet been published.
			{
				Console.WriteLine(Time.Service + "   " + Time.Destination + "		" + Time.SchArrival + "		" + Time.ExptArrival);
			}

			Console.ReadKey();
		}
		public static string LocName(string id)	//Used to retrieve the plain text name from a bus stops ID.
		{
			string name = "";

			StringBuilder builder = new StringBuilder();
			builder.Append("http://opendata.reading-travelinfo.co.uk/api/1/bus/calls/");
			builder.Append(id);
			builder.Append(APIKEY);

			XDocument LiveTimes = XDocument.Load(builder.ToString());
			name = LiveTimes.Root.Element("Name").Value.ToString();

			return name;
		}
	}
}
