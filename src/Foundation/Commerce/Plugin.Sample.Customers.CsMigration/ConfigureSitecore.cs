// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Reflection;   
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// Defines the Customer.Cs plugin configure sitecore.
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
                .AddPipeline<IGetProfileDefinitionPipeline, GetProfileDefinitionPipeline>(d =>
                {
                    d.Add<GetProfileDefinitionBlock>();
                })

                .AddPipeline<IMigrateCsCustomerPipeline, MigrateCsCustomerPipeline>(d =>
                {
                    d.Add<ValidateCustomerBlock>()
                     .Add<MapCustomerDetailsBlock>()
                     .Add<MapAddressesBlock>()                   
                     .Add<PersistCustomerBlock>()
                     .Add<PersistCustomerIdIndexBlock>();
                })               

                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>())

                .ConfigurePipeline<IRunningPluginsPipeline>(d => { d.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); }));               

            services.RegisterAllCommands(assembly);
        }
    }
}