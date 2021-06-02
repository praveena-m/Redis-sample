using Company.Core.Constants;
using Company.Core.Do;mainEntities;
using Company.Core.DTOs
using Company.Core.Helpers;
using Company.Core.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Core.Services
{
	public class TestService : ITestService
	{
		private readonly IRedisService _redisService;
		

		public TestService(IRedisService redisService)
		{
			_redisService = redisService;
		}
		
		public async Task<MyDataModel> GetMyDataAsync()
		{
			// Check if mydata is present in Redis.
			var myData = await _redisService.GetAsync<MyDataModel>(Rediskeys.MyData).ConfigureAwait(false);
			if (myData == null)
			{
				

				// Get my data from database.
				myData = await _databaseRepository.GetSingleAsync<MyDataModel>().ConfigureAwait(false);


				// Set my data in Redis.
				await _redisService.SetAsync(Rediskeys.MyData, myData).ConfigureAwait(false);
			}
			return myData;
		}
	}
}
