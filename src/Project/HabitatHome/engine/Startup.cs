// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.XmlEncryption;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.OData.Builder;
    using Microsoft.AspNetCore.OData.Extensions;
    using Microsoft.AspNetCore.OData.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json.Serialization;

    using Serilog;
    using Serilog.Events;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Core.Logging;
    using Sitecore.Commerce.Provider.FileSystem;
    using Sitecore.Framework.Diagnostics;
    using Sitecore.Framework.Rules;

    /// <summary>
    /// Defines the commerce engine startup.
    /// </summary>
    public class Startup
    {
        private readonly string _nodeInstanceId = Guid.NewGuid().ToString("N");
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _hostEnv;
        private volatile CommerceEnvironment _environment;
        private volatile NodeContext _nodeContext;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="hostEnv">The host env.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(
            IServiceProvider serviceProvider,
            IHostingEnvironment hostEnv,
            IConfiguration configuration)
        {
            this._hostEnv = hostEnv;
            this._serviceProvider = serviceProvider;

            this.Configuration = configuration;
            
            var appInsightsInstrumentationKey = this.Configuration.GetSection("ApplicationInsights:InstrumentationKey").Value;
            _telemetryClient = !string.IsNullOrWhiteSpace(appInsightsInstrumentationKey) ? new TelemetryClient { InstrumentationKey = appInsightsInstrumentationKey } : new TelemetryClient();

            if (bool.TryParse(this.Configuration.GetSection("Logging:SerilogLoggingEnabled")?.Value, out var serilogEnabled))
            {
                if (serilogEnabled)
                {
                    if (!long.TryParse(this.Configuration.GetSection("Serilog:FileSizeLimitBytes").Value, out var fileSize))
                    {
                        fileSize = 100000000;
                    }

                    Log.Logger = new LoggerConfiguration()
                                 .ReadFrom.Configuration(this.Configuration)
                                 .Enrich.FromLogContext()
                                 .Enrich.With(new ScLogEnricher())
                                 .WriteTo.Async(a => a.File(
                                     $@"{Path.Combine(this._hostEnv.WebRootPath, "logs")}\SCF.{DateTimeOffset.UtcNow:yyyyMMdd}.log.{this._nodeInstanceId}.txt",
                                     this.GetSerilogLogLevel(),
                                     "{ThreadId} {Timestamp:HH:mm:ss} {ScLevel} {Message}{NewLine}{Exception}",
                                     fileSizeLimitBytes: fileSize,
                                     rollOnFileSizeLimit: true), bufferSize: 500)
                                 .CreateLogger();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Initial Startup Environment. This will tell the Node how to behave
        /// This will be overloaded by the Environment stored in configuration.
        /// </summary>
        /// <value>
        /// The startup environment.
        /// </value>
        public CommerceEnvironment StartupEnvironment
        {
            get => this._environment ?? (this._environment = new CommerceEnvironment { Name = "Bootstrap" });
            set => this._environment = value;
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
            var logger = services.BuildServiceProvider().GetService<ILogger<Startup>>();
            this._nodeContext = new NodeContext(logger, _telemetryClient)
            {
                CorrelationId = this._nodeInstanceId,
                ConnectionId = "Node_Global",
                ContactId = "Node_Global",
                GlobalEnvironment = this.StartupEnvironment,
                Environment = this.StartupEnvironment,
                WebRootPath = this._hostEnv.WebRootPath,
                LoggingPath = this._hostEnv.WebRootPath + @"\logs\"
            };

            this.SetupDataProtection(services);

            var serializer = new EntitySerializerCommand(this._serviceProvider);
            this.StartupEnvironment = this.GetGlobalEnvironment(serializer);
            this._nodeContext.Environment = this.StartupEnvironment;

            services.AddSingleton(this.StartupEnvironment);
            services.AddSingleton(this._nodeContext);

            services.Configure<LoggingSettings>(options => this.Configuration.GetSection("Logging").Bind(options));
            services.AddApplicationInsightsTelemetry(this.Configuration);
            services.Configure<ApplicationInsightsSettings>(options => this.Configuration.GetSection("ApplicationInsights").Bind(options));
            services.Configure<CertificatesSettings>(this.Configuration.GetSection("Certificates"));
            services.Configure<List<string>>(Configuration.GetSection("AppSettings:AllowedOrigins"));

            services.AddSingleton(_telemetryClient);

            Log.Information("BootStrapping Application ...");
            services.Sitecore()
                .Eventing()
                .Caching(config => config
                    .AddMemoryStore("GlobalEnvironment")
                    .ConfigureCaches("GlobalEnvironment.*", "GlobalEnvironment"))
                .Rules();
            services.Add(new ServiceDescriptor(typeof(IRuleBuilderInit), typeof(RuleBuilder), ServiceLifetime.Transient));
            services.Sitecore()
                .BootstrapProduction(this._serviceProvider)
                .ConfigureCommercePipelines();

            services.AddOData();
            services.AddCors();
            services.AddMvcCore(options => options.InputFormatters.Add(new ODataFormInputFormatter())).AddJsonFormatters();
            services.AddWebEncoders();
            services.AddDistributedMemoryCache();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = this.Configuration.GetSection("AppSettings:SitecoreIdentityServerUrl").Value;
                    options.RequireHttpsMetadata = false;
                    options.EnableCaching = false;
                    options.ApiName = "EngineAPI";
                    options.ApiSecret = "secret";
                });

            this._nodeContext.CertificateHeaderName = this.Configuration.GetSection("Certificates:CertificateHeaderName").Value;

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RoleRequirement", policy => policy.Requirements.Add(new RoleAuthorizationRequirement(this._nodeContext.CertificateHeaderName)));
            });

            var antiForgeryEnabledSetting = this.Configuration.GetSection("AppSettings:AntiForgeryEnabled").Value;
            this._nodeContext.AntiForgeryEnabled = !string.IsNullOrWhiteSpace(antiForgeryEnabledSetting) && Convert.ToBoolean(antiForgeryEnabledSetting);
            if (this._nodeContext.AntiForgeryEnabled) services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddMvc()
                    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            this._nodeContext.AddObject(services);
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configureServiceApiPipeline">The context pipeline.</param>
        /// <param name="startNodePipeline">The start node pipeline.</param>
        /// <param name="configureOpsServiceApiPipeline">The context ops service API pipeline.</param>
        /// <param name="startEnvironmentPipeline">The start environment pipeline.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="loggingSettings">The logging settings.</param>
        /// <param name="applicationInsightsSettings">The application insights settings.</param>
        /// <param name="certificatesSettings">The certificates settings.</param>
        /// <param name="allowedOriginsOptions"></param>
        public void Configure(
            IApplicationBuilder app,
            IConfigureServiceApiPipeline configureServiceApiPipeline,
            IStartNodePipeline startNodePipeline,
            IConfigureOpsServiceApiPipeline configureOpsServiceApiPipeline,
            IStartEnvironmentPipeline startEnvironmentPipeline,
            ILoggerFactory loggerFactory,
            IOptions<LoggingSettings> loggingSettings,
            IOptions<ApplicationInsightsSettings> applicationInsightsSettings,
            IOptions<CertificatesSettings> certificatesSettings,
            IOptions<List<string>> allowedOriginsOptions)
        {
            app.UseDiagnostics();
            app.UseStaticFiles();

            // Set the error page
            if (this._hostEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
            }

            app.UseClientCertificateValidationMiddleware(certificatesSettings);

            app.UseCors(builder =>
                builder.WithOrigins(allowedOriginsOptions.Value.ToArray())
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseAuthentication();

            Task.Run(() => startNodePipeline.Run(this._nodeContext, this._nodeContext.GetPipelineContextOptions())).Wait();

            var environmentName = this.Configuration.GetSection("AppSettings:EnvironmentName").Value;
            if (!string.IsNullOrEmpty(environmentName))
            {
                this._nodeContext.AddDataMessage("EnvironmentStartup", $"StartEnvironment={environmentName}");
                Task.Run(() => startEnvironmentPipeline.Run(environmentName, this._nodeContext.GetPipelineContextOptions())).Wait();
            }

            // Initialize plugins OData contexts
            app.InitializeODataBuilder();
            var modelBuilder = new ODataConventionModelBuilder();

            // Run the pipeline to configure the plugin's OData context
            var contextResult = Task.Run(() => configureServiceApiPipeline.Run(modelBuilder, this._nodeContext.GetPipelineContextOptions())).Result;
            contextResult.Namespace = "Sitecore.Commerce.Engine";

            // Get the model and register the ODataRoute
            var model = contextResult.GetEdmModel();
            app.UseRouter(new ODataRoute("Api", model));

            // Register the bootstrap context for the engine
            modelBuilder = new ODataConventionModelBuilder();
            var contextOpsResult = Task.Run(() => configureOpsServiceApiPipeline.Run(modelBuilder, this._nodeContext.GetPipelineContextOptions())).Result;
            contextOpsResult.Namespace = "Sitecore.Commerce.Engine";

            // Get the model and register the ODataRoute
            model = contextOpsResult.GetEdmModel();
            app.UseRouter(new ODataRoute("CommerceOps", model));

            var appInsightsSettings = applicationInsightsSettings.Value;
            if (!(appInsightsSettings.TelemetryEnabled &&
                    !string.IsNullOrWhiteSpace(appInsightsSettings.InstrumentationKey)))
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }

            if (loggingSettings.Value != null && loggingSettings.Value.ApplicationInsightsLoggingEnabled)
            {
                loggerFactory.AddApplicationInsights(appInsightsSettings);
            }
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
        /// Setups the data protection storage and encryption protection type
        /// </summary>
        /// <param name="services">The services.</param>
        private void SetupDataProtection(IServiceCollection services)
        {
            var builder = services.AddDataProtection();
            var pathToKeyStorage = this.Configuration.GetSection("AppSettings:EncryptionKeyStorageLocation").Value;

            // Persist keys to a specific directory (should be a network location in distributed application)
            builder.PersistKeysToFileSystem(new DirectoryInfo(pathToKeyStorage));

            var protectionType = this.Configuration.GetSection("AppSettings:EncryptionProtectionType").Value.ToUpperInvariant();

            switch (protectionType)
            {
                case "DPAPI-SID":
                    var storageSid = this.Configuration.GetSection("AppSettings:EncryptionSID").Value.ToUpperInvariant();
                    //// Uses the descriptor rule "SID=S-1-5-21-..." to encrypt with domain joined user
                    builder.ProtectKeysWithDpapiNG($"SID={storageSid}", flags: DpapiNGProtectionDescriptorFlags.None);
                    break;
                case "DPAPI-CERT":
                    var storageCertificateHash = this.Configuration.GetSection("AppSettings:EncryptionCertificateHash").Value.ToUpperInvariant();
                    //// Searches the cert store for the cert with this thumbprint
                    builder.ProtectKeysWithDpapiNG(
                        $"CERTIFICATE=HashId:{storageCertificateHash}",
                        DpapiNGProtectionDescriptorFlags.None);
                    break;
                case "LOCAL":
                    //// Only the local user account can decrypt the keys
                    builder.ProtectKeysWithDpapiNG();
                    break;
                case "MACHINE":
                    //// All user accounts on the machine can decrypt the keys
                    builder.ProtectKeysWithDpapi(true);
                    break;
                default:
                    //// All user accounts on the machine can decrypt the keys
                    builder.ProtectKeysWithDpapi(true);
                    break;
            }
        }

        /// <summary>
        /// Gets the global environment.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <returns>A <see cref="CommerceEnvironment"/></returns>
        private CommerceEnvironment GetGlobalEnvironment(EntitySerializerCommand serializer)
        {
            CommerceEnvironment environment;

            Log.Information($"Loading Global Environment using Filesystem Provider from: {this._hostEnv.WebRootPath} s\\Bootstrap\\");

            // Use the default File System provider to setup the environment
            this._nodeContext.BootstrapProviderPath = this._hostEnv.WebRootPath + @"\Bootstrap\";
            var bootstrapProvider = new FileSystemEntityProvider(this._nodeContext.BootstrapProviderPath, serializer);

            var bootstrapFile = this.Configuration.GetSection("AppSettings:BootStrapFile").Value;

            if (!string.IsNullOrEmpty(bootstrapFile))
            {
                this._nodeContext.BootstrapEnvironmentPath = bootstrapFile;

                this._nodeContext.AddDataMessage("NodeStartup", $"GlobalEnvironmentFrom='Configuration: {bootstrapFile}'");
                environment = Task.Run(() => bootstrapProvider.Find<CommerceEnvironment>(this._nodeContext, bootstrapFile, false)).Result;
            }
            else
            {
                // Load the _nodeContext default
                bootstrapFile = "Global";
                this._nodeContext.BootstrapEnvironmentPath = bootstrapFile;
                this._nodeContext.AddDataMessage("NodeStartup", $"GlobalEnvironmentFrom='{bootstrapFile}.json'");
                environment = Task.Run(() => bootstrapProvider.Find<CommerceEnvironment>(this._nodeContext, bootstrapFile, false)).Result;
            }

            this._nodeContext.BootstrapEnvironmentPath = bootstrapFile;

            this._nodeContext.GlobalEnvironmentName = environment.Name;
            this._nodeContext.AddDataMessage("NodeStartup", $"Status='Started',GlobalEnvironmentName='{_nodeContext.GlobalEnvironmentName}'");

            if (this.Configuration.GetSection("AppSettings:BootStrapFile").Value != null)
            {
                this._nodeContext.ContactId = this.Configuration.GetSection("AppSettings:NodeId").Value;
            }

            if (!string.IsNullOrEmpty(environment.GetPolicy<DeploymentPolicy>().DeploymentId))
            {
                this._nodeContext.ContactId = $"{environment.GetPolicy<DeploymentPolicy>().DeploymentId}_{this._nodeInstanceId}";
            }

            return environment;
        }
    }
}
