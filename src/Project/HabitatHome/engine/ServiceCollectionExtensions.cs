using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Coupons;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.Plugin.Promotions;
using Sitecore.Commerce.Plugin.Tax;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Sitecore.Commerce.Engine
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureCartPipelines(this IServiceCollection services)
        {
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<ICalculateCartLinesPipeline>(builder => builder
                    .Add<PopulateCartLineItemsBlock>()
                    .Add<CalculateCartLinesPriceBlock>()
                    .Add<ValidateCartLinesPriceBlock>()
                    .Add<CalculateCartLinesSubTotalsBlock>()
                    .Add<CalculateCartLinesFulfillmentBlock>()
                    .Add<ValidateCartCouponsBlock>()
                    .Add<CalculateCartLinesPromotionsBlock>()
                    .Add<CalculateCartLinesTaxBlock>()
                    .Add<CalculateCartLinesTotalsBlock>())

               .ConfigurePipeline<ICalculateCartPipeline>(builder => builder
                    .Add<CalculateCartSubTotalsBlock>()
                    .Add<CalculateCartFulfillmentBlock>()
                    .Add<CalculateCartPromotionsBlock>()
                    .Add<CalculateCartTaxBlock>()
                    .Add<CalculateCartTotalsBlock>()
                    .Add<CalculateCartPaymentsBlock>())

              .ConfigurePipeline<IAddPaymentsPipeline>(builder => builder.Add<ValidateCartHasFulfillmentBlock>().After<ValidateCartAndPaymentsBlock>()));

            return services;
        }
    }
}
