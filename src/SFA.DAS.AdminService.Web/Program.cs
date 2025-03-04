using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;

namespace SFA.DAS.AdminService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager
                .Setup()
                .LoadConfigurationFromFile("nlog.config") 
                .GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseNLog();

                var startup = new Startup(builder.Configuration, builder.Environment);
                startup.ConfigureServices(builder.Services);

                var app = builder.Build();
                startup.Configure(app, app.Environment);

                app.Run();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not start host");
                throw;
            }
        }
    }
}
