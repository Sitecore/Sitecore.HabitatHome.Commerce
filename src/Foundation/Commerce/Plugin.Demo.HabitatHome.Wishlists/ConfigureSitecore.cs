// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Demo.HabitatHome.Wishlists
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Plugin.Demo.HabitatHome.Wishlists.Pipelines;
    using Plugin.Demo.HabitatHome.Wishlists.Pipelines.Blocks.AddWishlistLine;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Carts;
    using Plugin.Demo.HabitatHome.Wishlists.Pipelines.Blocks.RemoveWishlistLine;

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
