// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.XmlEncryption;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.OData.Builder;
    using Microsoft.AspNetCore.OData.Extensions;
    using Microsoft.AspNetCore.OData.Routing;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Serilog;
    using Serilog.Events;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Logging;
    using Sitecore.Commerce.Plugin.Rules;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Commerce.Provider.FileSystem;
    using Sitecore.Framework.Common;
    using Sitecore.Framework.Diagnostics;
    using Sitecore.Framework.Rules;
    using Sitecore.Framework.Rules.Serialization;

    /// <summary>
    /// Defines the commerce engine startup.
    /// </summary>
    public class Startup
    {

        private readonly string _nodeInstanceId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _hostEnv;
        private readonly TelemetryClient _telemetryClient;
        private volatile CommerceEnvironment _environment;
        private volatile NodeContext _nodeContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="hostEnv">The hosting environment.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(
            IServiceProvider serviceProvider,
            IHostingEnvironment hostEnv,
            IConfiguration configuration)
        {
            _hostEnv = hostEnv;
            _serviceProvider = serviceProvider;

            Configuration = configuration;

            var appInsightsInstrumentationKey = Configuration.GetSection("ApplicationInsights:InstrumentationKey").Value;
            _telemetryClient = !string.IsNullOrWhiteSpace(appInsightsInstrumentationKey) ? new TelemetryClient { InstrumentationKey = appInsightsInstrumentationKey } : new TelemetryClient();

            if (bool.TryParse(Configuration.GetSection("Logging:SerilogLoggingEnabled")?.Value, out var serilogEnabled))
            {
                if (serilogEnabled)
                {
                    if (!long.TryParse(Configuration.GetSection("Serilog:FileSizeLimitBytes").Value, out var fileSize))
                    {
                        fileSize = 100000000;
                    }

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.With(new ScLogEnricher())
                        .WriteTo.Async(a => a.File(
                            $@"{Path.Combine(_hostEnv.WebRootPath, "logs")}\SCF.{DateTimeOffset.UtcNow:yyyyMMdd}.log.{_nodeInstanceId}.txt",
                            GetSerilogLogLevel(),
                            "{ThreadId:D5} {Timestamp:HH:mm:ss} {ScLevel} {Message}{NewLine}{Exception}",
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
            get => _environment ?? (_environment = new CommerceEnvironment { Name = "Bootstrap" });
            set => _environment = value;
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
            _nodeContext = new NodeContext(logger, _telemetryClient)
            {
                CorrelationId = _nodeInstanceId,
                ConnectionId = "Node_Global",
                ContactId = "Node_Global",
                GlobalEnvironment = StartupEnvironment,
                Environment = StartupEnvironment,
                WebRootPath = _hostEnv.WebRootPath,
                LoggingPath = _hostEnv.WebRootPath + @"\logs\"
            };

            SetupDataProtection(services);

            var serializer = new CommerceCommander();
            StartupEnvironment = GetGlobalEnvironment(serializer);
            _nodeContext.Environment = StartupEnvironment;
            _nodeContext.GlobalEnvironment = StartupEnvironment;
            services.AddSingleton(StartupEnvironment);
            services.AddSingleton(_nodeContext);

            services.Configure<LoggingSettings>(options => Configuration.GetSection("Logging").Bind(options));
            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<ApplicationInsightsSettings>(options => Configuration.GetSection("ApplicationInsights").Bind(options));
            services.Configure<CertificatesSettings>(Configuration.GetSection("Certificates"));
            services.Configure<List<string>>(Configuration.GetSection("AppSettings:AllowedOrigins"));

            services.AddSingleton(_telemetryClient);

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CommerceContractResolver());
            services.AddOData();
            services.AddCors();
            services.AddMvcCore(options => options.InputFormatters.Add(new ODataFormInputFormatter())).AddJsonFormatters();
            services.AddHttpContextAccessor();
            services.AddWebEncoders();
            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddIdentityServerAuthentication(
                    options =>
                    {
                        options.Authority = Configuration.GetSection("AppSettings:SitecoreIdentityServerUrl").Value;
                        options.RequireHttpsMetadata = false;
                        options.EnableCaching = false;
                        options.ApiName = "EngineAPI";
                        options.ApiSecret = "secret";
                    });
            services.AddAuthorization(
                options => { options.AddPolicy("RoleRequirement", policy => policy.Requirements.Add(new RoleAuthorizationRequirement(_nodeContext.CertificateHeaderName))); });

            _nodeContext.CertificateHeaderName = Configuration.GetSection("Certificates:CertificateHeaderName").Value;

            var antiForgeryEnabledSetting = Configuration.GetSection("AppSettings:AntiForgeryEnabled").Value;
            _nodeContext.AntiForgeryEnabled = !string.IsNullOrWhiteSpace(antiForgeryEnabledSetting) && System.Convert.ToBoolean(antiForgeryEnabledSetting, CultureInfo.InvariantCulture);
            _nodeContext.CommerceServicesHostPostfix = Configuration.GetSection("AppSettings:CommerceServicesHostPostfix").Value;
            if (string.IsNullOrEmpty(_nodeContext.CommerceServicesHostPostfix))
            {
                if (_nodeContext.AntiForgeryEnabled)
                {
                    services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
                }
            }
            else
            {
                if (_nodeContext.AntiForgeryEnabled)
                {
                    services.AddAntiforgery(
                        options =>
                        {
                            options.HeaderName = "X-XSRF-TOKEN";
                            options.Cookie.SameSite = SameSiteMode.Lax;
                            options.Cookie.Domain = string.Concat(".", _nodeContext.CommerceServicesHostPostfix);
                            options.Cookie.HttpOnly = false;
                        });
                }
            }

            Log.Information("Bootstrapping Application ...");

            services.Sitecore()
                .Eventing()
                .Rules()
                .BootstrapProduction(_serviceProvider)
                .ConfigureCommercePipelines();
            services.Add(new ServiceDescriptor(typeof(IRuleBuilderInit), typeof(RuleBuilder), ServiceLifetime.Transient));
            services.Replace(new ServiceDescriptor(typeof(IRuleMetadataMapper), typeof(CommerceRuleMetadataMapper), ServiceLifetime.Singleton));

            var cachingSettings = new CachingSettings();
            Configuration.GetSection("Caching").Bind(cachingSettings);
            var memoryCacheSettings = cachingSettings.Memory;
            var redisCacheSettings = cachingSettings.Redis;
            if (memoryCacheSettings.Enabled && redisCacheSettings.Enabled)
            {
                Log.Error("Only one cache provider can be enable at the same time, please choose Memory or Redis.");
                return;
            }

            if (!memoryCacheSettings.Enabled && !redisCacheSettings.Enabled)
            {
                Log.Warning("There is not cache provider configured, a default memory cache will be use.");
                return;
            }

            services.Sitecore()
                .Caching(
                    config =>
                    {
                        if (memoryCacheSettings.Enabled)
                        {
                            config
                                .AddMemoryStore(memoryCacheSettings.CacheStoreName, options => options = memoryCacheSettings.Options)
                                .ConfigureCaches(WildcardMatch.All(), memoryCacheSettings.CacheStoreName);
                        }

                        if (redisCacheSettings.Enabled)
                        {
                            _nodeContext.IsRedisCachingEnabled = true;
                            config
                                .AddRedisStore(redisCacheSettings.CacheStoreName, redisCacheSettings.Options.Configuration, redisCacheSettings.Options.InstanceName)
                                .ConfigureCaches(WildcardMatch.All(), redisCacheSettings.CacheStoreName);
                        }

                        config.SetDefaultSerializer<CommerceCacheStoreSerializer>();
                    });
            if (Configuration.GetSection("Compression:Enabled").Get<bool>())
            {
                var responseCompressionOptions = Configuration.GetSection("Compression:ResponseCompressionOptions")
                    .Get<ResponseCompressionOptions>();

                services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = responseCompressionOptions?.EnableForHttps ?? true;
                    options.MimeTypes = responseCompressionOptions?.MimeTypes ?? ResponseCompressionDefaults.MimeTypes;
                    options.Providers.Add<GzipCompressionProvider>();
                });

                var gzipCompressionProviderOptions = Configuration.GetSection("Compression:GzipCompressionProviderOptions")
                    .Get<GzipCompressionProviderOptions>();

                services.Configure<GzipCompressionProviderOptions>(options => { options.Level = gzipCompressionProviderOptions?.Level ?? CompressionLevel.Fastest; });
            }

            _nodeContext.AddObject(services);
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configureServiceApiPipeline">The context pipeline.</param>
        /// <param name="startNodePipeline">The start node pipeline.</param>
        /// <param name="configureOpsServiceApiPipeline">The context ops service API pipeline.</param>
        /// <param name="startEnvironmentPipeline">The start environment pipeline.</param>
        /// <param name="loggingSettings">The logging settings.</param>
        /// <param name="certificatesSettings">The certificates settings.</param>
        /// <param name="allowedOriginsOptions"></param>
        /// <param name="getDatabaseVersionCommand">Command to get DB version</param>
        public void Configure(
            IApplicationBuilder app,
            IConfigureServiceApiPipeline configureServiceApiPipeline,
            IStartNodePipeline startNodePipeline,
            IConfigureOpsServiceApiPipeline configureOpsServiceApiPipeline,
            IStartEnvironmentPipeline startEnvironmentPipeline,
            IOptions<LoggingSettings> loggingSettings,
            IOptions<CertificatesSettings> certificatesSettings,
            IOptions<List<string>> allowedOriginsOptions,
            GetDatabaseVersionCommand getDatabaseVersionCommand)
        {
            // TODO: Check if we can move this code to a better place, this code checks Database version against Core required version
            // Get the core required database version from config policy
            var coreRequiredDbVersion = string.Empty;
            if (StartupEnvironment.HasPolicy<EntityStoreSqlPolicy>())
            {
                coreRequiredDbVersion = StartupEnvironment.GetPolicy<EntityStoreSqlPolicy>().Version;
            }

            // Get the db version
            var dbVersion = Task.Run(() => getDatabaseVersionCommand.Process(_nodeContext)).Result;

            // Check versions
            if (string.IsNullOrEmpty(dbVersion) || string.IsNullOrEmpty(coreRequiredDbVersion) || !string.Equals(coreRequiredDbVersion, dbVersion, StringComparison.Ordinal))
            {
                throw new CommerceException($"Core required DB Version [{coreRequiredDbVersion}] and DB Version [{dbVersion}]");
            }

            Log.Information($"Core required DB Version [{coreRequiredDbVersion}] and DB Version [{dbVersion}]");

            if (Configuration.GetSection("Compression:Enabled").Get<bool>())
            {
                app.UseResponseCompression();
            }

            app.UseDiagnostics();

            // Set the error page
            if (_hostEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
            }

            app.UseClientCertificateValidationMiddleware(certificatesSettings);

            app.UseCors(builder =>
#pragma warning disable CA1308 // Normalize strings to uppercase
                builder.WithOrigins(allowedOriginsOptions.Value.ConvertAll(d => d.ToLower(CultureInfo.InvariantCulture)).ToArray())
#pragma warning restore CA1308 // Normalize strings to uppercase
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseAuthentication();

            Task.Run(() => startNodePipeline.Run(_nodeContext, _nodeContext.PipelineContextOptions)).Wait();

            var environmentName = Configuration.GetSection("AppSettings:EnvironmentName").Value;
            if (!string.IsNullOrEmpty(environmentName))
            {
                _nodeContext.AddDataMessage("EnvironmentStartup", $"StartEnvironment={environmentName}");
                Task.Run(() => startEnvironmentPipeline.Run(environmentName, _nodeContext.PipelineContextOptions)).Wait();
            }

            // Initialize plugins OData contexts
            app.InitializeODataBuilder();

            // Run the pipeline to configure the plugins OData context
            var contextResult = Task.Run(() => configureServiceApiPipeline.Run(new ODataConventionModelBuilder(), _nodeContext.PipelineContextOptions)).Result;
            contextResult.Namespace = "Sitecore.Commerce.Engine";

            // Get the model and register the ODataRoute
            var apiModel = contextResult.GetEdmModel();
            app.UseRouter(new ODataRoute("Api", apiModel));

            // Register the bootstrap context for the engine
            var contextOpsResult = Task.Run(() => configureOpsServiceApiPipeline.Run(new ODataConventionModelBuilder(), _nodeContext.PipelineContextOptions)).Result;
            contextOpsResult.Namespace = "Sitecore.Commerce.Engine";

            // Get the model and register the ODataRoute
            var opsModel = contextOpsResult.GetEdmModel();
            app.UseRouter(new ODataRoute("CommerceOps", opsModel));

            _nodeContext.PipelineTraceLoggingEnabled = loggingSettings.Value.PipelineTraceLoggingEnabled;
        }

        /// <summary>
        /// Gets the serilog log level.
        /// </summary>
        /// <returns>A <see cref="LogEventLevel"/></returns>
        private LogEventLevel GetSerilogLogLevel()
        {
            var level = LogEventLevel.Verbose;
            var configuredLevel = Configuration.GetSection("Serilog:MinimumLevel:Default").Value;
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
            var pathToKeyStorage = Configuration.GetSection("AppSettings:EncryptionKeyStorageLocation").Value;

            // Persist keys to a specific directory (should be a network location in distributed application)
            builder.PersistKeysToFileSystem(new DirectoryInfo(pathToKeyStorage));

            var protectionType = Configuration.GetSection("AppSettings:EncryptionProtectionType").Value.ToUpperInvariant();

            switch (protectionType)
            {
                case "DPAPI-SID":
                    var storageSid = Configuration.GetSection("AppSettings:EncryptionSID").Value.ToUpperInvariant();
                    //// Uses the descriptor rule "SID=S-1-5-21-..." to encrypt with domain joined user
                    builder.ProtectKeysWithDpapiNG($"SID={storageSid}", flags: DpapiNGProtectionDescriptorFlags.None);
                    break;
                case "DPAPI-CERT":
                    var storageCertificateHash = Configuration.GetSection("AppSettings:EncryptionCertificateHash").Value.ToUpperInvariant();
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
        private CommerceEnvironment GetGlobalEnvironment(CommerceCommander serializer)
        {
            CommerceEnvironment environment;
            var bootstrapProviderFolderPath = string.Concat(Path.Combine(_hostEnv.WebRootPath, "Bootstrap"), Path.DirectorySeparatorChar);

            Log.Information($"Loading Global Environment using Filesystem Provider from: {bootstrapProviderFolderPath}");

            // Use the default File System provider to setup the environment
            _nodeContext.BootstrapProviderPath = bootstrapProviderFolderPath;
            var bootstrapProvider = new FileSystemEntityProvider(_nodeContext.BootstrapProviderPath, serializer);

            var bootstrapFile = Configuration.GetSection("AppSettings:BootStrapFile").Value;

            if (!string.IsNullOrEmpty(bootstrapFile))
            {
                _nodeContext.BootstrapEnvironmentPath = bootstrapFile;

                _nodeContext.AddDataMessage("NodeStartup", $"GlobalEnvironmentFrom='Configuration: {bootstrapFile}'");
                environment = bootstrapProvider.Find<CommerceEnvironment>(_nodeContext, bootstrapFile, false).Result;
            }
            else
            {
                // Load the _nodeContext default
                bootstrapFile = "Global";
                _nodeContext.BootstrapEnvironmentPath = bootstrapFile;
                _nodeContext.AddDataMessage("NodeStartup", $"GlobalEnvironmentFrom='{bootstrapFile}.json'");
                environment = bootstrapProvider.Find<CommerceEnvironment>(_nodeContext, bootstrapFile, false).Result;
            }

            _nodeContext.GlobalEnvironmentName = environment.Name;
            _nodeContext.AddDataMessage("NodeStartup", $"Status='Started',GlobalEnvironmentName='{_nodeContext.GlobalEnvironmentName}'");

            if (Configuration.GetSection("AppSettings:BootStrapFile").Value != null)
            {
                _nodeContext.ContactId = Configuration.GetSection("AppSettings:NodeId").Value;
            }

            if (!string.IsNullOrEmpty(environment.GetPolicy<DeploymentPolicy>().DeploymentId))
            {
                _nodeContext.ContactId = $"{environment.GetPolicy<DeploymentPolicy>().DeploymentId}_{_nodeInstanceId}";
            }

            return environment;
        }

    }
}
