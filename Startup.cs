using Company.Core.Helpers;
using Company.Core.Interfaces;
using Company.Core.Services;
using Company.Web.Formatters;
using Company.Web.HealthChecks;
using Company.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Utf8Json.Resolvers;

namespace Company.Web
{
	public class Startup
	{
		private readonly string[] allowedOrigins;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add functionality to inject IOptions<T>
			services.AddOptions();
			
			services.AddSingleton<IRedisService, RedisService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRedisService redisService)
		{
			// Connect to redis.
			redisService.Connect();
		}
	}
}