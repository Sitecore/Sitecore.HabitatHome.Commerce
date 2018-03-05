// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFederatedPaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Payments.Braintree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Braintree;
    using global::Braintree.Exceptions;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which updates transaction Id for an order.
    /// </summary>   
    /// <seealso>
    ///   <cref>
    /// Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Orders.Order, Sitecore.Commerce.Plugin.Orders.Order, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    /// </cref>
    /// </seealso>
    [PipelineDisplayName(PaymentsBraintreeConstants.Pipelines.Blocks.UpdateFederatedPaymentBlock)]
    public class UpdateFederatedPaymentBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFederatedPaymentBlock" /> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public UpdateFederatedPaymentBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;           
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A cart with federate payment component
        /// </returns>
        public override async Task<Order> Run(Order arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: order can not be null.");

            var order = arg;
            if (!order.HasComponent<OnHoldOrderComponent>())
            {
                var invalidOrderStateMessage = $"{this.Name}: Expected order in '{context.GetPolicy<KnownOrderStatusPolicy>().OnHold}' status but order was in '{order.Status}' status";
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().ValidationError,
                        "InvalidOrderState",
                        new object[] { context.GetPolicy<KnownOrderStatusPolicy>().OnHold, order.Status },
                        invalidOrderStateMessage), 
                    context);
            }

            var cart = context.CommerceContext.GetEntity<Cart>(c => c.Id.Equals(order.GetComponent<OnHoldOrderComponent>().TemporaryCart.EntityTarget, StringComparison.OrdinalIgnoreCase));
            if (cart == null || !cart.HasComponent<FederatedPaymentComponent>())
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

            if (!string.IsNullOrEmpty(payment.TransactionId))
            {
                // Federated Payment was not changed
                return arg;
            }           

            var braintreeClientPolicy = context.GetPolicy<BraintreeClientPolicy>();
            if (string.IsNullOrEmpty(braintreeClientPolicy?.Environment) || string.IsNullOrEmpty(braintreeClientPolicy?.MerchantId)
                || string.IsNullOrEmpty(braintreeClientPolicy?.PublicKey) || string.IsNullOrEmpty(braintreeClientPolicy?.PrivateKey))
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
                var gateway = new BraintreeGateway(braintreeClientPolicy?.Environment, braintreeClientPolicy?.MerchantId, braintreeClientPolicy?.PublicKey, braintreeClientPolicy?.PrivateKey);

                // void order payment
                if (order.HasComponent<FederatedPaymentComponent>())
                {
                    var orderPayment = order.GetComponent<FederatedPaymentComponent>();

                    // void order payment  
                    var voidResult = gateway.Transaction.Void(orderPayment.TransactionId);
                    if (voidResult.IsSuccess())
                    {
                        context.Logger.LogInformation($"{this.Name} - Void Payment succeeded:{ orderPayment.Id }");
                        orderPayment.TransactionStatus = voidResult.Target.Status.ToString();
                        await this.GenerateSalesActivity(order, orderPayment, context);
                    }
                    else
                    {
                        var errorMessages = voidResult.Errors.DeepAll().Aggregate(string.Empty, (current, error) => current + ("Error: " + (int)error.Code + " - " + error.Message + "\n"));

                        context.Abort(
                            await context.CommerceContext.AddMessage(
                               context.GetPolicy<KnownResultCodes>().Error,
                               "PaymentVoidFailed",
                               new object[] { orderPayment.TransactionId },
                               $"{this.Name}. Payment void failed for transaction { orderPayment.TransactionId }: { errorMessages }"), 
                            context);

                        return arg;
                    }
                }

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
                    var errorMessages = result.Errors.DeepAll().Aggregate(string.Empty, (current, error) => current + ("Error: " + (int)error.Code + " - " + error.Message + "\n"));

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
                await context.CommerceContext.AddMessage(
                   context.GetPolicy<KnownResultCodes>().Error,
                   "InvalidClientPolicy",
                   new object[] { "BraintreeClientPolicy", ex },
                    $"{this.Name}. Invalid BraintreeClientPolicy");
                return arg;
            }
        }

        /// <summary>
        /// Generates the sales activity.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="payment">The payment.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        protected virtual async Task GenerateSalesActivity(Order order, PaymentComponent payment, CommercePipelineExecutionContext context)
        {
            var salesActivity = new SalesActivity
            {
                Id = CommerceEntity.IdPrefix<SalesActivity>() + Guid.NewGuid().ToString("N"),
                ActivityAmount = new Money(payment.Amount.CurrencyCode, 0),
                Customer = new EntityReference
                {
                    EntityTarget = order.Components.OfType<ContactComponent>().FirstOrDefault()?.CustomerId
                },
                Order = new EntityReference
                {
                    EntityTarget = order.Id
                },
                Name = "Void the Federated payment",
                PaymentStatus = context.GetPolicy<KnownSalesActivityStatusesPolicy>().Void
            };

            salesActivity.SetComponent(new ListMembershipsComponent
            {
                Memberships = new List<string>
                    {
                        CommerceEntity.ListName<SalesActivity>(),
                        string.Format(context.GetPolicy<KnownOrderListsPolicy>().OrderSalesActivities, order.FriendlyId)
                    }
            });

            salesActivity.SetComponent(payment);

            var salesActivities = order.SalesActivity.ToList();
            salesActivities.Add(new EntityReference { EntityTarget = salesActivity.Id });
            order.SalesActivity = salesActivities;

            await this._persistEntityPipeline.Run(new PersistEntityArgument(salesActivity), context);
        }
    }
}