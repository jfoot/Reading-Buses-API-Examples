using System.Collections.Generic;

namespace ReadingBuses
{
	class Bus
	{
		public string ServiceId;
		public List<Location> locations = new List<Location>();
		private string Boperator;
		public string Description;

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
						Boperator = "CTNY";
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

	}

	class Location
	{
		public string Id;
		public string direction;
		public string DisplayOrder;
	}


	class BLocation
	{
		public string Service;
		public string Destination;
		public string SchArrival;
		public string ExptArrival;
	}
}
