// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Blocks;
namespace Sitecore.Commerce.Plugin.Sample
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
   

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
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

            .AddPipeline<ICreateStoreInventorySetPipeline, CreateStoreInventorySetPipeline>(
                    configure =>
                        {
                            configure.Add<CreateStoreInventorySetBlock>();
                        })

            .AddPipeline<IAssociateStoreInventoryToSellableItem, AssociateStoreInventoryToSellableItem>(
                configure =>
                {
                    configure.Add<AssociateStoreInventoryToSellablteItemBlock>();
                })

            .AddPipeline<IGetNearestStoreDetailsByLocationPipeline, GetNearestStoreDetailsByLocationPipeline>(
            configure =>
            {
                configure.Add<GetNearestStoreDetailsByLocationBlock>();
            })

            .AddPipeline<IGetProductsToUpdateInventoryPipeline, GetProductsToUpdateInventoryPipeline>(
                configure =>
                {
                    configure.Add<GetProductsToUpdateInventoryBlock>();
                })

            

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}