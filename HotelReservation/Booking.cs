using Newtonsoft.Json;
using System;

namespace HotelReservation
{
	public class Booking
	{
		[JsonProperty(PropertyName = "id")]
		public Guid Id { get; set; }
		public string GuestName { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }

		[PartitionKey]
		public string HotelName { get; set; }
	}
}
