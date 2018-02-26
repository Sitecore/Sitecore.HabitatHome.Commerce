// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.AdventureWorks
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// Defines the configure sitecore class for the AdventureWorks plugin.
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Configuration.IConfigureSitecore" />
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
             .ConfigurePipeline<IInitializeEnvironmentPipeline>(d =>
                {
                    d.Add<InitializeCatalogBlock>()
                        .Add<InitializeInventoryBlock>()
                        .Add<InitializeEnvironmentRegionsBlock>()
                        .Add<InitializeEnvironmentShopsBlock>()
                        .Add<InitializeEnvironmentPricingBlock>()
                        .Add<InitializeEnvironmentPromotionsBlock>()
                        .Add<InitializeEnvironmentGiftCardsBlock>();
                })
             .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); }));

            services.ConfigureCartPipelines();
            services.ConfigureOrdersPipelines();
        }
    }
}