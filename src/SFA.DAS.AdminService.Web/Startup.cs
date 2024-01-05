using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Azure;
using SFA.DAS.AdminService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.AutoMapperProfiles;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ModelBinders;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using CheckSessionFilter = SFA.DAS.AdminService.Web.Infrastructure.CheckSessionFilter;
using FeatureToggleFilter = SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles.FeatureToggleFilter;
using ISessionService = SFA.DAS.AdminService.Web.Infrastructure.ISessionService;

namespace SFA.DAS.AdminService.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string ServiceName = "SFA.DAS.AdminService";
        private const string Version = "1.0";
        public IConfiguration Configuration { get; }
        public IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            if (!configuration.IsDev())
            {
                config.AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif

            config.AddEnvironmentVariables();
            if (!configuration.IsDev())
            {
                // read the DfeSignIn Configurations from Application settings.
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }
            Configuration = config.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; // Default is true, make it false
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ApplicationConfiguration = ConfigurationService.GetConfig<WebConfiguration>(
                Configuration["EnvironmentName"], 
                Configuration["ConfigurationStorageConnectionString"], 
                Version, 
                ServiceName).Result;

            services
                .AddHttpClient<IApplicationApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<ICertificateApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IContactsApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<ILearnerDetailsApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IMergeOrganisationsApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IOrganisationsApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IRegisterApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IRegisterValidationApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IScheduleApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IStaffReportsApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IStaffSearchApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IStandardVersionApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<IQnaApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(ApplicationConfiguration.QnaApiAuthentication.ApiBaseUrl);
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            AddAuthentication(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services
                .AddMvc(options =>
                {
                    options.Filters.Add<CheckSessionFilter>();
                    options.Filters.Add<FeatureToggleFilter>();
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.ModelBinderProviders.Insert(0, new SuppressBindingErrorsModelBinderProvider());
                    options.ModelBinderProviders.Insert(0, new StringTrimmingModelBinderProvider());
                    options.EnableEndpointRouting = false;
                })
                .AddMvcOptions(m => m.ModelMetadataDetailsProviders.Add(new HumanizerMetadataProvider()))
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.Configure<RazorViewEngineOptions>(o =>
                {
                    o.ViewLocationFormats.Add("/Views/Application/{1}/{0}" + RazorViewEngine.ViewExtension);
                    o.ViewLocationFormats.Add("/Views/Application/{0}" + RazorViewEngine.ViewExtension);
                });

            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromHours(1);
            });

            services.AddDistributedCache(ApplicationConfiguration.RedisCacheSettings, _env);

            services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".Assessors.Staff.AntiForgery", HttpOnly = false });
            services.AddHealthChecks();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddAutoMapper(config =>
            {
                config.AddProfile<AutoMapperMappings>();
            });
            services.AddApplicationInsightsTelemetry();
            ConfigureDependencyInjection(services);
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.Scan(x => x.FromCallingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IQnaTokenService, QnaTokenService>();

            services.AddTransient(x => ApplicationConfiguration);

            services.AddTransient<ISessionService>(x =>
                new SessionService(x.GetService<IHttpContextAccessor>(), Configuration["EnvironmentName"]));

            services.AddTransient<CertificateDateViewModelValidator>();

            services.AddTransient<IAssessorTokenService>(x =>
                    new AssessorTokenService(ApplicationConfiguration.EpaoApiAuthentication,
                        x.GetService<ILogger<AssessorTokenService>>()));

            services.AddTransient<IQnaTokenService>(x =>
                    new QnaTokenService(ApplicationConfiguration.QnaApiAuthentication,
                        x.GetService<ILogger<QnaTokenService>>()));

            services.AddTransient<ICompaniesHouseApiClient>(x => new CompaniesHouseApiClient(
                ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<CompaniesHouseApiClient>>(),
                x.GetService<IRoatpApplyTokenService>()));

            services.AddTransient<IRoatpApplyTokenService, RoatpApplyTokenService>();

            services.AddTransient<IRoatpApplicationApiClient>(x => new RoatpApplicationApiClient(
                ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<RoatpApplicationApiClient>>(),
                x.GetService<IRoatpApplyTokenService>()));

            services.AddTransient<IRoatpOrganisationApiClient>(x => new RoatpOrganisationApiClient(
                ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<RoatpOrganisationApiClient>>(),
                x.GetService<IRoatpApplyTokenService>()));

            services.AddTransient<IRoatpOrganisationSummaryApiClient>(x => new RoatpOrganisationSummaryApiClient(
                ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<RoatpOrganisationSummaryApiClient>>(),
                x.GetService<IRoatpApplyTokenService>()));

            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAssessorValidationService, AssessorValidationService>();
            services.AddTransient<ISpecialCharacterCleanserService, SpecialCharacterCleanserService>();

            services.AddTransient<IAzureApiClientConfiguration>(x => 
                ApplicationConfiguration.AzureApiAuthentication);

            services.AddTransient<IAzureTokenService>(x =>
                    new AzureTokenService(ApplicationConfiguration.AzureApiAuthentication));

            services.AddTransient<IAzureApiClient>(x => new AzureApiClient(
                ApplicationConfiguration.AzureApiAuthentication.ApiBaseAddress,
                x.GetService<IAzureTokenService>(),
                x.GetService<ILogger<AzureApiClientBase>>(),
                x.GetService<IAzureApiClientConfiguration>(),
                x.GetService<IOrganisationsApiClient>(),
                x.GetService<IContactsApiClient>()));
            
            services.AddTransient<ICsvExportService, CsvExportService>();

            Common.DependencyInjection.ConfigureDependencyInjection(services);
            services.AddTransient<IFeatureToggles>(x =>
            {
                var config = x.GetService<IWebConfiguration>();
                return config.FeatureToggles;
            });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            if (ApplicationConfiguration.UseDfESignIn)
                UseDfeSignInAuthentication(services);
            else 
                UseWsFederationAuthentication(services);
        }

        /// <summary>
        /// Method to register the DfeSignIn Authentication services with AspNetCore Authentication Options.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        private void UseDfeSignInAuthentication(IServiceCollection services)
        {
            services.AddAndConfigureDfESignInAuthentication(
                Configuration, 
                $"{typeof(Extensions.ServiceCollectionExtensions).Assembly.GetName().Name}.Auth",
                typeof(CustomServiceRole),
                DfESignIn.Auth.Enums.ClientName.ServiceAdmin,
                "/SignOut",
                "");
        }

        /// <summary>
        /// Method to register the WsFederation Authentication services with AspNetCore Authentication Options.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        private void UseWsFederationAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
            }).AddWsFederation(options =>
            {
                options.Wtrealm = ApplicationConfiguration.StaffAuthentication.WtRealm;
                options.MetadataAddress = ApplicationConfiguration.StaffAuthentication.MetadataAddress;
                options.TokenValidationParameters.RoleClaimType = Domain.Roles.RoleClaimType;
            }).AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");
            app.UseSecurityHeaders();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHealthChecks("/health");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}