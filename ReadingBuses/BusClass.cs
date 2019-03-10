using System;
using System.Collections.Generic;

namespace ReadingBuses
{
	class Bus
	{
		public string ServiceId { get; set; }
        public List<Location> locations { get; set; } = new List<Location>();
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

        public Bus(string id)
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
		public string direction { get; set; }
        public string DisplayOrder { get; set; }
    }


	class BLocation
	{
		public string Service { get; set; }
        public string Destination { get; set; }
        public DateTime SchArrival { get; set; }
        public DateTime? ExptArrival { get; set; }
    }
}
