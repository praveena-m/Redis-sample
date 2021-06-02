using Company.Core.DomainEntities;
using Company.Core.DTOs;
using Company.Core.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Company.Core.Constants.CompanyEnums;

namespace Company.Core.Interfaces
{
	public interface ITestService
	{
		Task<MyDataModel> GetMyDataAsync();
	}
}
