using Microsoft.AspNetCore.Builder;
using System;

namespace SFA.DAS.AdminService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup(builder.Configuration, builder.Environment);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app, app.Environment);

            app.Run();
        }
    }
}
