using HotelReservation;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationTests
{

	[TestClass]
	public class SqlTests
	{
		private BookingService bookingService;
		private IDataService<Booking> dataService;
		private static int Counter;

		public SqlTests()
		{
			Counter += 1;
			var config = Substitute.For<IConfiguration>();
			config["Prefix"].Returns($"pf{Counter}");
			config["EndpointUri"].Returns("https://localhost:8081");
			config["PrimaryKey"].Returns("C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

			dataService = new SqlDataService<Booking>(config);
			bookingService = new BookingService(dataService);
		}

		[TestInitialize]
		public async Task Init()
		{
			await dataService.CreateIfNotExistsAsync();

			await dataService.AddAsync(new Booking
			{
				Start = DateTime.Parse("2020-06-01"),
				End = DateTime.Parse("2020-06-05"),
				GuestName = "Foo One",
				HotelName = "GrandHotel",
				Id = Guid.NewGuid()
			});

			await dataService.AddAsync(new Booking
			{
				Start = DateTime.Parse("2020-08-11"),
				End = DateTime.Parse("2020-08-15"),
				GuestName = "Foo One",
				HotelName = "GrandHotel",
				Id = Guid.NewGuid()
			});
		}

		[TestCleanup]
		public async Task Cleanup()
		{
			await dataService.DeleteAsync();
		}

		[TestMethod]
		public async Task AbleToFindRecordsWithStartDate()
		{
			var bookings = (await bookingService.GetBookingsByDate(DateTime.Parse("2020-06-01"))).ToList();
			Assert.AreEqual(1, bookings.Count);
		}

		[TestMethod]
		public async Task NotAbleToFindRecordsWithStartDate()
		{
			var bookings = (await bookingService.GetBookingsByDate(DateTime.Parse("2020-06-02"))).ToList();
			Assert.AreEqual(0, bookings.Count);
		}
	}
}
