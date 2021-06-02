using Company.Core.Helpers;
using Company.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utf8Json;

namespace Company.Infrastructure.Cache
{
	public class RedisService : IRedisService
	{
		private bool _redisCachingEnabled;
		private readonly string _connectionString;
		private readonly string _redisKeyPrefix;
		private readonly TimeSpan _defaultExpiry;
		private ConnectionMultiplexer _redisConnection = null;
		private IDatabase _db;

		public RedisService(IConfiguration config, IOptions<AppSettings> settings)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			_redisCachingEnabled = settings.Value.Redis.CachingEnabled;
			_redisKeyPrefix = settings.Value.Redis.KeyPrefix;
			_defaultExpiry = TimeSpan.Parse(settings.Value.Redis.KeyExpiration);
			_connectionString = settings.Value.Redis.ConnectionString;
		}

		public bool Connect(bool healthCheck = false)
		{
			try
			{
				if (!_redisCachingEnabled && !healthCheck)
					return true;

				_redisConnection = ConnectionMultiplexer.Connect(_connectionString + ",allowAdmin=true");
				_db = _redisConnection.GetDatabase();
			}
			catch (Exception ex)
			{
				Log.Warning("Redis connection was not possible at this point, hence disabling the Redis Caching. There will be performance degradation.");
				Log.Error(ex, "Unable to connect to Redis.");
				_redisCachingEnabled = false;
				if (healthCheck)
				{
					throw;
				}
			}
			return true;
		}

		public async Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null)
		{
			try
			{
				if (!_redisCachingEnabled)
					return true;

				var data = JsonSerializer.Serialize(value);
				return await _db.StringSetAsync($"{_redisKeyPrefix}{key}", data, expiry ?? _defaultExpiry).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Exception while Redis SetAsync.");
			}
			return false;
		}

		public async Task<T> GetAsync<T>(string key)
		{
			try
			{
				if (!_redisCachingEnabled)
					return default;

				string value = await _db.StringGetAsync($"{_redisKeyPrefix}{key}").ConfigureAwait(false);
				if (!string.IsNullOrEmpty(value))
					return JsonSerializer.Deserialize<T>(value);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Exception while Redis GetAsync.");
			}
			return default;
		}

		public async Task<bool> DeleteAsync(string key)
		{
			try
			{
				if (!_redisCachingEnabled)
					return default;

				return await _db.KeyDeleteAsync($"{_redisKeyPrefix}{key}").ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Exception while Redis DeleteAsync.");
			}
			return default;
		}

		public async Task<bool> FlushRedisAsync()
		{
			try
			{
				if (!_redisCachingEnabled)
					return true;

				var server = _redisConnection.GetServer(_connectionString);
				await server.FlushAllDatabasesAsync().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Exception while Redis FlushRedisAsync.");
				return false;
			}
			return true;
		}

		public IEnumerable<string> GetAllKeys()
		{
			try
			{
				if (!_redisCachingEnabled)
					return default;

				var server = _redisConnection.GetServer(_connectionString);
				return server.Keys(pattern: "*").Select(x => x.ToString());
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Exception while Redis GetAllKeys.");
			}
			return default;
		}
	}
}
