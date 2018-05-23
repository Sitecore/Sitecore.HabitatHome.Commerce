// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews;
using Sitecore.HabitatHome.Feature.EBay.Engine.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Pipelines.Blocks;

namespace Sitecore.HabitatHome.Feature.EBay.Engine
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

             .AddPipeline<IPrepareEbayItemPipeline, PrepareEbayItemPipeline>(
                    configure =>
                        {
                            configure.Add<PrepareItemStartBlock>()
                                     .Add<StartSellingActionBlock>()
                                     .Add<PrepareItemVariationsBlock>();
                        })
              .ConfigurePipeline<IGetEntityViewPipeline>(d =>
              {
                  d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                   .Add<FormStartSelling>().Before<IFormatEntityViewPipeline>()
                   .Add<FormStartSellingAll>().Before<IFormatEntityViewPipeline>()
                   .Add<FormEndItem>().Before<IFormatEntityViewPipeline>()
                   .Add<ItemEbayExtensions>().Before<IFormatEntityViewPipeline>()
                   .Add<FormRegisterToken>().Before<IFormatEntityViewPipeline>()
                   .Add<FormConfigure>().Before<IFormatEntityViewPipeline>()
                   .Add<MerchandisingDashboardSearchFix>();
              })

              .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
              {
                  d.Add<EnsureNavigationView>();
              })

              .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                {
                    d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>()
                     .Add<EnsureActionsMarketplace>().After<PopulateEntityViewActionsBlock>()
                     .Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>();
                })

               .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionStartSelling>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionStartSellingAll>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionPublishPending>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionConfigure>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionFixSyncItem>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionForgetItem>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionEndItem>()
                     .Add<DoActionEndListingAll>()
                     .Add<DoActionRemoveToken>()
                     .Add<DoActionRegisterToken>().Before<ValidateEntityVersionBlock>()
                     .Add<MerchandisingDashboardSearchFix>();
                })

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}