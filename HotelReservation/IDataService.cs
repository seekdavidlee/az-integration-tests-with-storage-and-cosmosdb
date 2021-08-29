using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelReservation
{
	public interface IDataService<T>
	{
		Task AddAsync(T item);
		Task<IEnumerable<T>> QueryAsync(string query);
		Task CreateIfNotExistsAsync();
		Task DeleteAsync();
	}
}
