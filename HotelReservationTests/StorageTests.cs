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
	public class StorageTests
	{
		private BookingService bookingService;
		private IDataService<Booking> dataService;
		private static int Counter;
		public StorageTests()
		{
			Counter += 1;
			var config = Substitute.For<IConfiguration>();
			config["Prefix"].Returns($"pf{Counter}");
			config["TableStorageConnection"].Returns("UseDevelopmentStorage=true");

			dataService = new StorageTableDataService<Booking>(config);
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
			var bookings = (await bookingService.GetBookingsByDate(DateTime.Parse("2020-06-01").ToUniversalTime())).ToList();
			Assert.AreEqual(1, bookings.Count);
		}

		[TestMethod]
		public async Task NotAbleToFindRecordsWithStartDate()
		{
			var bookings = (await bookingService.GetBookingsByDate(DateTime.Parse("2020-06-02").ToUniversalTime())).ToList();
			Assert.AreEqual(0, bookings.Count);
		}
	}
}
