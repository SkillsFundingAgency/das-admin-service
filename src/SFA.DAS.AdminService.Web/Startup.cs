using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Azure;
using SFA.DAS.AdminService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.AutoMapperProfiles;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ModelBinders;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;
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
        private readonly IHostEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public IConfiguration Configuration { get; }
        public IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IHostEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            if (!configuration.IsDev())
            {
                config.AddJsonFile("appsettings.Development.json", true);
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
            ApplicationConfiguration = Configuration.Get<WebConfiguration>();
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

            AddAuthentication(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssemblyContaining<Startup>();

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

            services.Scan(scan =>
                scan.FromCallingAssembly()
                    .AddClasses()
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            services.AddTransient(x => ApplicationConfiguration);
            services.AddTransient<QnaApiClientConfiguration>(x => ApplicationConfiguration.QnaApiAuthentication);
            services.AddTransient<AssessorApiClientConfiguration>(x => ApplicationConfiguration.EpaoApiAuthentication);
            services.AddTransient<RoatpApiClientConfiguration>(x => ApplicationConfiguration.RoatpApiAuthentication);
            services.AddTransient<RoatpApplicationApiClientConfiguration>(x => ApplicationConfiguration.ApplyApiAuthentication);

            // in addition to the classes which Scrutor adds, classes which exists in other SFA.DAS assemblies are being
            // added manually as attempting to do so via a global SFA.DAS Scrutor scan instantiates some classes which
            // cannot be safely instantiated so falling back on the safe method of adding some classes manually note 
            // that not all classes need to be added here so check if Scrutor has corrected added them first before
            // adding them manually
            services.AddTransient<IQnaApiClientFactory, QnaApiClientFactory>();
            services.AddTransient<IAssessorApiClientFactory, AssessorApiClientFactory>();
            services.AddTransient<IRoatpApiClientFactory, RoatpApiClientFactory>();
            services.AddTransient<IRoatpApplicationApiClientFactory, RoatpApplicationApiClientFactory>();

            services.AddTransient<IApplicationApiClient, ApplicationApiClient>();
            services.AddTransient<ICertificateApiClient, CertificateApiClient>();
            services.AddTransient<IContactsApiClient, ContactsApiClient>();
            services.AddTransient<ILearnerDetailsApiClient, LearnerDetailsApiClient>();
            services.AddTransient<IMergeOrganisationsApiClient, MergeOrganisationsApiClient>();
            services.AddTransient<IOrganisationsApiClient, OrganisationsApiClient>();
            services.AddTransient<IRegisterApiClient, RegisterApiClient>();
            services.AddTransient<IRegisterValidationApiClient, RegisterValidationApiClient>();
            services.AddTransient<IScheduleApiClient, ScheduleApiClient>();
            services.AddTransient<IStaffReportsApiClient, StaffReportsApiClient>();
            services.AddTransient<IStaffSearchApiClient, StaffSearchApiClient>();
            services.AddTransient<IStandardVersionApiClient, StandardVersionApiClient>();
            services.AddTransient<IStaffReportsApiClient, StaffReportsApiClient>();
            services.AddTransient<IQnaApiClient, QnaApiClient>();
            services.AddTransient<IRoatpApiClient, RoatpApiClient>();
            services.AddTransient<IRoatpApplicationApiClient, RoatpApplicationApiClient>();

            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAssessorValidationService, AssessorValidationService>();
            services.AddTransient<ISpecialCharacterCleanserService, SpecialCharacterCleanserService>();

            services.AddTransient<IAzureApiClientConfiguration>(x => ApplicationConfiguration.AzureApiAuthentication);
            services.AddTransient<IAzureTokenService>(x => new AzureTokenService(ApplicationConfiguration.AzureApiAuthentication));

            services.AddTransient<IAzureApiClient>(x => new AzureApiClient(
                ApplicationConfiguration.AzureApiAuthentication.ApiBaseAddress,
                x.GetService<IAzureTokenService>(),
                x.GetService<ILogger<AzureApiClientBase>>(),
                x.GetService<IAzureApiClientConfiguration>(),
                x.GetService<IOrganisationsApiClient>(),
                x.GetService<IContactsApiClient>()));

            services.AddTransient<ISessionService>(x =>
                new SessionService(x.GetService<IHttpContextAccessor>(), Configuration["EnvironmentName"]));

            services.AddTransient<CertificateDateViewModelValidator>();

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
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
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