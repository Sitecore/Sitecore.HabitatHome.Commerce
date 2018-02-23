// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateFederatedPaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Demo.Payments.Moneybags
{
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;

    /// <summary>
    ///  Defines a block which creates a payment service transaction.
    /// </summary>  
    /// <seealso>
    ///   <cref>
    /// Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Orders.CartEmailArgument, Sitecore.Commerce.Plugin.Orders.CartEmailArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    /// </cref>
    /// </seealso>
    [PipelineDisplayName(PaymentsMoneybagsConstants.Pipelines.Blocks.CreateFederatedPaymentBlock)]
    public class CreateFederatedPaymentBlock : PipelineBlock<CartEmailArgument, CartEmailArgument, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A cart with federate payment component
        /// </returns>
        public override async Task<CartEmailArgument> Run(CartEmailArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The cart can not be null");

            var cart = arg.Cart;
            if (!cart.HasComponent<FederatedPaymentComponent>())
            {
                return arg;
            }

            var payment = cart.GetComponent<FederatedPaymentComponent>();
            if (string.IsNullOrEmpty(payment.PaymentMethodNonce))
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "InvalidOrMissingPropertyValue",
                        new object[] { "PaymentMethodNonce" },
                        "Invalid or missing value for property 'PaymentMethodNonce'."),
                    context);
                return arg;
            }

            //fake payment details
            payment.TransactionId = Guid.NewGuid().ToString();
            payment.TransactionStatus = "Settled";
            payment.PaymentInstrumentType = "Moneybags";
            payment.MaskedNumber = "****-****-****-1111";
            payment.CardType = "Moneybags";
            payment.ExpiresMonth = 12;
            payment.ExpiresYear = 25;   

            return arg;                        
        }
    }
}