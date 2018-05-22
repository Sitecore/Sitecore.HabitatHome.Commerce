// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Blocks.AddWishlistLine;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Blocks.RemoveWishlistLine;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine
{
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

             .AddPipeline<IAddWishListLineItemPipeline, AddWishListLineItemPipeline>(
                    configure =>
                        {
                            configure.Add<ValidateSellableItemBlock>();
                            configure.Add<AddWishListLineBlock>();
                            configure.Add<AddContactBlock>()
                           .Add<ICalculateCartLinesPipeline>()
                           .Add<ICalculateCartPipeline>();
                            configure.Add<PersistCartBlock>();
                        })

                .AddPipeline<IRemoveWishListLinePipeline, RemoveWishListLinePipeline>(
                    configure =>
                    {
                        configure.Add<RemoveWishlistLineBlock>()
                       .Add<ICalculateCartLinesPipeline>()
                       .Add<ICalculateCartPipeline>()
                       .Add<PersistCartBlock>();
                    })

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}
