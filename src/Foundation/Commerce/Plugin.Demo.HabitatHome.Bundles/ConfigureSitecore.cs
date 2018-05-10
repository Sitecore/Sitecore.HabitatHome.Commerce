// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Plugin.Demo.HabitatHome.Bundles.Pipelines.Blocks;
namespace Sitecore.Commerce.Plugin.Sample
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
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

             //.AddPipeline<ISamplePipeline, SamplePipeline>(
             //       configure =>
             //           {
             //               configure.Add<SampleBlock>();
             //           })

               .ConfigurePipeline<IAddCartLinePipeline>(builder => builder.Add<AddToCartBundlesBlock>().After<AddCartLineBlock>())
               .ConfigurePipeline<IGetCartPipeline>(builder => builder.Add<HideCartLineBundlesBlock>().After<GetCartBlock>())
               .ConfigurePipeline<IRemoveCartLinePipeline>(builder => builder.Add<RemoveCartLineBundlesBlock>().Before<RemoveCartLineBlock>())
               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}