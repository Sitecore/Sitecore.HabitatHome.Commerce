// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Habitat
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The Habitat configure class.
    /// </summary>
    /// <seealso cref="IConfigureSitecore" />
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

            services.Sitecore().Pipelines(
                config =>
                    config
                        .ConfigurePipeline<IInitializeEnvironmentPipeline>(
                            d =>
                            {
                                d.Add<InitializeCatalogBlock>()
                                    .Add<InitializeInventoryBlock>()
                                    .Add<InitializeEnvironmentPricingBlock>()
                                    .Add<InitializeEnvironmentPromotionsBlock>();
                            })
                        .ConfigurePipeline<IRunningPluginsPipeline>(c =>
                        {
                            c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>();
                        }));
        }
    }
}