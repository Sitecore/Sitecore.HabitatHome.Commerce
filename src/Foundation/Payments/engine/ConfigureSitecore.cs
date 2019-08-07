// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.Plugin.FaultInjection;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using GetClientTokenBlock = Sitecore.HabitatHome.Foundation.Payments.Engine.Pipelines.Blocks.GetClientTokenBlock;
using RegisteredPluginBlock = Sitecore.HabitatHome.Foundation.Payments.Engine.Pipelines.Blocks.RegisteredPluginBlock;
using Sitecore.HabitatHome.Foundation.Payments.Engine.Pipelines.Blocks;

namespace Sitecore.HabitatHome.Foundation.Payments.Engine
{
    /// <summary>
    /// The payments moneybags configure sitecore class
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

                .ConfigurePipeline<IGetClientTokenPipeline>(d =>
                {
                    d.Add<GetClientTokenBlock>().After<Sitecore.Commerce.Plugin.Payments.GetClientTokenBlock>();
                })
                .ConfigurePipeline<ICreateOrderPipeline>(d =>
                {
                    d.Add<CreateFederatedPaymentBlock>().Before<CreateOrderBlock>();
                })
                .ConfigurePipeline<IReleaseOnHoldOrderPipeline>(d =>
                {
                    d.Add<UpdateFederatedPaymentBlock>().After<ValidateOnHoldOrderBlock>();
                })
                .ConfigurePipeline<IRefundPaymentsPipeline>(d =>
                {
                    d.Add<RefundFederatedPaymentBlock>().Before<PersistOrderBlock>();
                })
                .ConfigurePipeline<ICancelOrderPipeline>(d =>
                {
                    d.Add<VoidCancelOrderFederatedPaymentBlock>().After<GetPendingOrderBlock>();
                })
                .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); })
                .ConfigurePipeline<IReleasedOrdersMinionPipeline>(c => { c.Add<SettleOrderSalesActivitiesBlock>().After<SettlePaymentFaultBlock>(); }));

            services.RegisterAllCommands(assembly);
        }
    }
}
