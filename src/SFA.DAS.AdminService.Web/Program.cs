﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using System;

namespace SFA.DAS.AdminService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting up host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not start host");
                throw;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("https://localhost:44348")
                .UseNLog();
    }
}
