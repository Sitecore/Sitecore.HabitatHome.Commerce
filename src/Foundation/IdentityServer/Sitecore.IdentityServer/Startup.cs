// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using Configuration;

    using IdentityServer4.Contrib.Membership;
    using IdentityServer4.Validation;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Serilog;
    using Serilog.Events;

    using Validators;

    /// <summary>
    /// Defines the application startup.
    /// </summary>
    public class Startup
    {
        private readonly IHostingEnvironment _hostEnv;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="hostEnv">The host env.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(
            IHostingEnvironment hostEnv,
            IConfiguration configuration)
        {
            this._hostEnv = hostEnv;
            this.Configuration = configuration;

            if (!long.TryParse(this.Configuration.GetSection("Serilog:FileSizeLimitBytes").Value, out var fileSize))
            {
                fileSize = 100000000;
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(this.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    $@"{Path.Combine(hostEnv.WebRootPath, "logs")}\SIS.{DateTimeOffset.UtcNow:yyyyMMdd}.log.{Guid.NewGuid():N}.txt",
                    this.GetSerilogLogLevel(),
                    "{ThreadId} {Timestamp:HH:mm:ss} {ScLevel} {Message}{NewLine}{Exception}",
                    fileSizeLimitBytes: fileSize,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var appSettings = new AppSettings();
            this.Configuration.GetSection("AppSettings").Bind(appSettings);

            var idServices = services
                .AddIdentityServer()
                .AddInMemoryClients(Clients.Get(appSettings))
                .AddInMemoryIdentityResources(Resources.GetIdentityResources(appSettings))
                .AddInMemoryApiResources(Resources.GetApiResources(appSettings))
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                .AddMembershipService(new MembershipOptions
                                          {
                                              ConnectionString = appSettings.SitecoreMembershipOptions.ConnectionString,
                                              ApplicationName = appSettings.SitecoreMembershipOptions.ApplicationName,
                                              UseRoleProviderSource = appSettings.SitecoreMembershipOptions.UseRoleProviderSource
                                          })
                .AddResourceOwnerValidator<SitecoreResourceOwnerValidator>();

            services.AddTransient<IRedirectUriValidator, WildcardRedirectUriValidator>();

            if (this._hostEnv.IsDevelopment())
            {
                idServices.AddDeveloperSigningCredential();
            }
            else if (this._hostEnv.IsProduction())
            {
                var certificate = this.GetCertificate(
                    appSettings.IDServerCertificateStoreName,
                    appSettings.IDServerCertificateStoreLocation,
                    appSettings.IDServerCertificateThumbprint);
                if (certificate == null)
                {
                    Log.Logger.Fatal($"Certificate with thumbprint {appSettings.IDServerCertificateThumbprint} was not found in store {appSettings.IDServerCertificateStoreName}-{appSettings.IDServerCertificateStoreLocation}.");
                    return;
                }

                idServices.AddSigningCredential(certificate);
            }
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer()
                .UseCors(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        /// <summary>
        /// Gets the serilog log level.
        /// </summary>
        /// <returns>A <see cref="LogEventLevel"/></returns>
        private LogEventLevel GetSerilogLogLevel()
        {
            var level = LogEventLevel.Verbose;
            var configuredLevel = this.Configuration.GetSection("Serilog:MinimumLevel:Default").Value;
            if (string.IsNullOrEmpty(configuredLevel))
            {
                return level;
            }

            if (configuredLevel.Equals(LogEventLevel.Debug.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                level = LogEventLevel.Debug;
            }
            else if (configuredLevel.Equals(LogEventLevel.Information.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                level = LogEventLevel.Information;
            }
            else if (configuredLevel.Equals(LogEventLevel.Warning.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                level = LogEventLevel.Warning;
            }
            else if (configuredLevel.Equals(LogEventLevel.Error.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                level = LogEventLevel.Error;
            }
            else if (configuredLevel.Equals(LogEventLevel.Fatal.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                level = LogEventLevel.Fatal;
            }

            return level;
        }

        /// <summary>
        /// Gets the certificate
        /// </summary>
        /// <param name="storeName">the store name.</param>
        /// <param name="storeLocation">the store location.</param>
        /// <param name="thumbprint">the thumbprint</param>
        /// <returns>A <see cref="X509Certificate2"/></returns>
        private X509Certificate2 GetCertificate(string storeName, string storeLocation, string thumbprint)
        {
            var store = new X509Store(this.GetCerfStoreName(storeName), this.GetCerfStoreLocation(storeLocation));
            store.Open(OpenFlags.ReadOnly);

            var certificatesInStore = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            if (certificatesInStore.Count == 0)
            {
                return null;
            }

            var certificate = certificatesInStore[0];
            return certificate;
        }

        /// <summary>
        /// Gets the cerfiticate location
        /// </summary>
        /// <param name="location">the location</param>
        /// <returns>A <see cref="StoreLocation"/></returns>
        private StoreLocation GetCerfStoreLocation(string location)
        {
            var storeLocation = StoreLocation.LocalMachine;

            if (location.Equals("CurrentUser", StringComparison.OrdinalIgnoreCase))
            {
                storeLocation = StoreLocation.CurrentUser;
            }

            return storeLocation;
        }

        /// <summary>
        /// Gets the store name
        /// </summary>
        /// <param name="name">the name</param>
        /// <returns>A <see cref="StoreName"/></returns>
        private StoreName GetCerfStoreName(string name)
        {
            var storeName = StoreName.My;

            if (name.Equals("AddressBook", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.AddressBook;
            }

            if (name.Equals("AuthRoot", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.AuthRoot;
            }

            if (name.Equals("CertificateAuthority", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.CertificateAuthority;
            }

            if (name.Equals("Disallowed", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.Disallowed;
            }

            if (name.Equals("Root", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.Root;
            }

            if (name.Equals("TrustedPeople", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.TrustedPeople;
            }

            if (name.Equals("TrustedPublisher", StringComparison.OrdinalIgnoreCase))
            {
                storeName = StoreName.TrustedPublisher;
            }

            return storeName;
        }
    }
}
