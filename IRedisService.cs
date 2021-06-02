using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Company.Core.Interfaces
{
	public interface IRedisService
	{
		bool Connect(bool healthCheck = false);

		Task<T> GetAsync<T>(string key);

		Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null);

		Task<bool> DeleteAsync(string key);

		Task<bool> FlushRedisAsync();

		IEnumerable<string> GetAllKeys();
	}
}
