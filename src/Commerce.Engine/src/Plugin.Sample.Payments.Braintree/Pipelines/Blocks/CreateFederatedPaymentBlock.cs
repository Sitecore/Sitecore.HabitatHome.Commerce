// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateFederatedPaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Payments.Braintree
{
    using System.Linq;
    using System.Threading.Tasks;
    using global::Braintree;
    using global::Braintree.Exceptions;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    ///  Defines a block which creates a payment service transaction.
    /// </summary>  
    /// <seealso>
    ///   <cref>
    /// Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Orders.CartEmailArgument, Sitecore.Commerce.Plugin.Orders.CartEmailArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    /// </cref>
    /// </seealso>
    [PipelineDisplayName(PaymentsBraintreeConstants.Pipelines.Blocks.CreateFederatedPaymentBlock)]
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

            var braintreeClientPolicy = context.GetPolicy<BraintreeClientPolicy>();
            if (string.IsNullOrEmpty(braintreeClientPolicy.Environment) || string.IsNullOrEmpty(braintreeClientPolicy.MerchantId)
                || string.IsNullOrEmpty(braintreeClientPolicy.PublicKey) || string.IsNullOrEmpty(braintreeClientPolicy.PrivateKey))
            {
                await context.CommerceContext.AddMessage(
                   context.GetPolicy<KnownResultCodes>().Error,
                   "InvalidClientPolicy",
                   new object[] { "BraintreeClientPolicy" },
                    $"{this.Name}. Invalid BraintreeClientPolicy");
                return arg;
            }

            try
            {
                var gateway = new BraintreeGateway(braintreeClientPolicy.Environment, braintreeClientPolicy.MerchantId, braintreeClientPolicy.PublicKey, braintreeClientPolicy.PrivateKey);

                var request = new TransactionRequest
                {
                    Amount = payment.Amount.Amount,
                    PaymentMethodNonce = payment.PaymentMethodNonce,
                    BillingAddress = ComponentsHelper.TranslatePartyToAddressRequest(payment.BillingParty, context),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = false
                    }
                };

                var result = gateway.Transaction.Sale(request);
                if (result.IsSuccess())
                {
                    var transaction = result.Target;
                    payment.TransactionId = transaction?.Id;
                    payment.TransactionStatus = transaction?.Status?.ToString();
                    payment.PaymentInstrumentType = transaction?.PaymentInstrumentType?.ToString();

                    var cc = transaction?.CreditCard;
                    payment.MaskedNumber = cc?.MaskedNumber;
                    payment.CardType = cc?.CardType?.ToString();
                    if (cc?.ExpirationMonth != null)
                    {
                        payment.ExpiresMonth = int.Parse(cc.ExpirationMonth);
                    }

                    if (cc?.ExpirationYear != null)
                    {
                        payment.ExpiresYear = int.Parse(cc.ExpirationYear);
                    }
                }
                else 
                {
                    var errorMessages = string.Concat(result.Message, " ", result.Errors.DeepAll().Aggregate(string.Empty, (current, error) => current + ("Error: " + (int)error.Code + " - " + error.Message + "\n")));
                    context.Abort(
                        await context.CommerceContext.AddMessage(
                           context.GetPolicy<KnownResultCodes>().Error,
                           "CreatePaymentFailed",
                           new object[] { "PaymentMethodNonce" },
                           $"{this.Name}. Create payment failed :{ errorMessages }"), 
                        context);                    
                }

                return arg;               
            }
            catch (BraintreeException ex)
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                       context.GetPolicy<KnownResultCodes>().Error,
                       "CreatePaymentFailed",
                       new object[] { "PaymentMethodNonce", ex },
                       $"{this.Name}. Create payment failed."), 
                    context);
                return arg;
            }
        }
    }
}