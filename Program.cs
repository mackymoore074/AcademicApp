using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using StackExchange;
using System;
using AcademicApp.Data; // Adjust to your namespace

namespace AcademicApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddControllers();

                        // Configure Entity Framework Core
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseSqlServer("YourConnectionStringHere")); // Replace with your actual connection string

                        // Configure Redis caching
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = "localhost"; // Replace with your Redis server configuration
                            options.InstanceName = "SampleInstance";
                        });

                       

                        services.AddMemoryCache();
                    });

                    webBuilder.Configure(app =>
                    {
                        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        app.UseHttpsRedirection();
                        app.UseRouting();
                        app.UseAuthorization();
                        app.UseResponseCaching();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}
