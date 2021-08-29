using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelReservation
{
	public class SqlDataService<T> : IDataService<T>
	{
		private readonly CosmosClient _client;
		private readonly string _resourceType;
		public SqlDataService(IConfiguration configuration)
		{
			var uri = configuration["EndpointUri"];
			var key = configuration["PrimaryKey"];

			_client = new CosmosClient(uri, key);
			var prefix = configuration["Prefix"];

			if (prefix == null) prefix = "";

			_resourceType = $"{prefix}{typeof(T).Name}";
		}

		public async Task AddAsync(T item)
		{
			await _container.CreateItemAsync(item);
		}

		private Database _database;
		private Container _container;
		public async Task CreateIfNotExistsAsync()
		{
			var response = await _client.CreateDatabaseIfNotExistsAsync("db" + _resourceType,
				ThroughputProperties.CreateManualThroughput(400));

			_database = response.Database;

			_container = (await _database.CreateContainerIfNotExistsAsync(new ContainerProperties
			{
				Id = _resourceType,
				PartitionKeyPath = "/HotelName"
			})).Container;
		}

		public async Task DeleteAsync()
		{
			await _database.DeleteAsync();
		}

		public async Task<IEnumerable<T>> QueryAsync(string query)
		{
			QueryDefinition queryDefinition = new QueryDefinition(query);
			FeedIterator<T> queryResultSetIterator = _container.GetItemQueryIterator<T>(queryDefinition);
			List<T> items = new List<T>();

			while (queryResultSetIterator.HasMoreResults)
			{
				FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
				foreach (T item in currentResultSet)
				{
					items.Add(item);
				}
			}

			return items;
		}
	}
}
