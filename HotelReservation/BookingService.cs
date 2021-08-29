using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelReservation
{
	public class BookingService
	{
		private readonly IDataService<Booking> _tableDataService;

		public BookingService(IDataService<Booking> tableDataService)
		{
			_tableDataService = tableDataService;
		}

		public async Task<IEnumerable<Booking>> GetBookingsByDate(DateTime start)
		{
			// Just trying to demo integration tests, in real life, DI will dictate
			// the DataService to use.
			if (_tableDataService is StorageTableDataService<Booking>)
			{
				return await _tableDataService.QueryAsync($"Start eq {ConvertToOData(start)}");
			}
			return await _tableDataService.QueryAsync($"SELECT * FROM c WHERE c.Start = '{ConvertToDateString(start)}'");
		}

		private static string ConvertToOData(DateTime dateTime)
		{
			return $"datetime'{dateTime:yyyy-MM-ddTHH:mm:ss.000Z}'";
		}

		private static string ConvertToDateString(DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
		}
	}
}
