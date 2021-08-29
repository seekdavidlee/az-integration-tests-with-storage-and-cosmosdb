using FastMember;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservation
{
	public class StorageTableDataService<T> : IDataService<T>
	{
		private readonly CloudTable _cloudTable;
		private readonly string _resourceType;
		public StorageTableDataService(IConfiguration configuration)
		{
			var storageAccount = CloudStorageAccount.Parse(configuration["TableStorageConnection"]);
			var cloudTableClient = storageAccount.CreateCloudTableClient();
			var prefix = configuration["Prefix"];

			if (prefix == null) prefix = "";

			_resourceType = $"{prefix}{typeof(T).Name}";

			_cloudTable = cloudTableClient.GetTableReference(_resourceType);
		}

		public async Task AddAsync(T item)
		{
			await _cloudTable.ExecuteAsync(TableOperation.Insert(Convert(item)));
		}

		private DynamicTableEntity Convert(T item)
		{
			var ta = TypeAccessor.Create(typeof(T));
			var hasId = ta.GetMembers().SingleOrDefault(x => x.Name == "Id");

			if (hasId == null) throw new NotImplementedException("Model does not contain an Id property.");

			string key = ta[item, "Id"].ToString();

			var partitionKey = string.Concat(ta.GetMembers().Where(x => x.GetAttribute(typeof(PartitionKeyAttribute), false) != null)
				.Select(x => ta[item, x.Name].ToString()));

			var dynamicTableEntity = new DynamicTableEntity(partitionKey, key)
			{
				Properties = EntityPropertyConverter.Flatten(item, new OperationContext())
			};
			dynamicTableEntity.ETag = "*";
			return dynamicTableEntity;
		}

		public async Task<IEnumerable<T>> QueryAsync(string query)
		{
			return await Query(query);
		}

		private async Task<IEnumerable<T>> Query(string queryString)
		{
			var list = new List<T>();

			var continuationToken = default(TableContinuationToken);

			do
			{
				var results = await _cloudTable.ExecuteQuerySegmentedAsync(new TableQuery { FilterString = queryString }, continuationToken);

				list.AddRange(results.Results.Select(x => EntityPropertyConverter.ConvertBack<T>(x.Properties, new OperationContext())).ToList());

				continuationToken = results.ContinuationToken;
			}
			while (continuationToken != null);

			return list;
		}

		public async Task CreateIfNotExistsAsync()
		{
			await _cloudTable.CreateIfNotExistsAsync();
		}

		public async Task DeleteAsync()
		{
			await _cloudTable.DeleteAsync();
		}
	}
}
