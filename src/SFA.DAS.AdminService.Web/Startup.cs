using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Validators;
using CheckSessionFilter = SFA.DAS.AdminService.Web.Infrastructure.CheckSessionFilter;
using FeatureToggleFilter = SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles.FeatureToggleFilter;
using ISessionService = SFA.DAS.AdminService.Web.Infrastructure.ISessionService;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using Microsoft.AspNetCore.Mvc.Razor;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Web.ModelBinders;
using SFA.DAS.AdminService.Web.Extensions;

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
            Configuration = configuration;
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

            ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            services
                .AddHttpClient<ApiClient>("ApiClient", config =>
                {
                    config.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    config.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services
                .AddHttpClient<ApplicationApiClient>("ApplicationApiClient", config =>
                {
                    config.BaseAddress = new Uri(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress);
                    config.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
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
                })
                .AddMvcOptions(m => m.ModelMetadataDetailsProviders.Add(new HumanizerMetadataProvider()))
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
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
            MappingStartup.AddMappings();

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

            services.AddTransient<IOrganisationsApiClient>(x =>
                    new OrganisationsApiClient(ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress,
                        x.GetService<ITokenService>(),
                        x.GetService<ILogger<OrganisationsApiClient>>()));

            services.AddTransient<IApiClient>(x => new ApiClient(
                ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress,
                x.GetService<ITokenService>()));

            services.AddTransient<ICertificateApiClient>(x => new CertificateApiClient(
                ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress,
                x.GetService<ITokenService>(),
                x.GetService<ILogger<CertificateApiClient>>()));

            services.AddTransient<IApplicationApiClient>(x => new ApplicationApiClient(
                ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<ApplicationApiClient>>(),
                x.GetService<ITokenService>()));

            services.AddTransient<ICompaniesHouseApiClient>(x => new CompaniesHouseApiClient(
                ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<CompaniesHouseApiClient>>(),
                x.GetService<IRoatpApplyTokenService>()));


            services.AddTransient<ICharityCommissionApiClient>(x => new CharityCommissionApiClient(
                ApplicationConfiguration.ApplyApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<CharityCommissionApiClient>>(),
                x.GetService<IRoatpApplyTokenService>()));

            services.AddTransient<IContactsApiClient>(x => new ContactsApiClient(
                ApplicationConfiguration.EpaoApiAuthentication.ApiBaseAddress,
                x.GetService<ITokenService>(),
                x.GetService<ILogger<ContactsApiClient>>()));

            services.AddTransient<IQnaApiClient>(x => new QnaApiClient(
              ApplicationConfiguration.QnaApiAuthentication.ApiBaseAddress,
              x.GetService<IQnaTokenService>(),
              x.GetService<ILogger<QnaApiClient>>()));

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

            services.AddTransient<IIfaStandardsApiClient>(x =>
                new IfaStandardsApiClient(ApplicationConfiguration.IfaApiClientBaseUrl));

            services.AddTransient<IAzureTokenService, AzureTokenService>();

            services.AddTransient<IAzureApiClient>(x => new AzureApiClient(
                ApplicationConfiguration.AzureApiAuthentication.ApiBaseAddress,
                x.GetService<IAzureTokenService>(),
                x.GetService<ILogger<AzureApiClientBase>>(),
                x.GetService<IWebConfiguration>(),
                x.GetService<IOrganisationsApiClient>(),
                x.GetService<IContactsApiClient>()));

            services.AddTransient<CacheService>();
            services.AddTransient<CertificateLearnerStartDateViewModelValidator>();
            
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
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");
            app.UseSecurityHeaders();
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