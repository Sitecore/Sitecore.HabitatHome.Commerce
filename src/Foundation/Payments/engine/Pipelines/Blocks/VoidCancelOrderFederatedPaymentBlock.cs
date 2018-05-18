// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoidCancelOrderFederatedPaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Foundation.Payments.Engine.Pipelines.Blocks
{
    /// <summary>
    /// Defines a void canceled order federated paymentBlock.
    /// </summary>
    /// <seealso>
    /// <cref>
    ///   Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Payments.Order, 
    ///   Sitecore.Commerce.Plugin.Payments.Order, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    /// </cref>
    /// </seealso>
    [PipelineDisplayName(PaymentsMoneybagsConstants.Pipelines.Blocks.VoidCancelOrderFederatedPaymentBlock)]
    public class VoidCancelOrderFederatedPaymentBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoidCancelOrderFederatedPaymentBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public VoidCancelOrderFederatedPaymentBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            this._persistPipeline = persistEntityPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>An order with Federated payment info</returns>
        public override async Task<Order> Run(Order arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull("The arg can not be null");

            var order = arg;

            if (!order.HasComponent<OnHoldOrderComponent>() &&
                !order.Status.Equals(context.GetPolicy<KnownOrderStatusPolicy>().Pending, StringComparison.OrdinalIgnoreCase) &&
                !order.Status.Equals(context.GetPolicy<KnownOrderStatusPolicy>().Problem, StringComparison.OrdinalIgnoreCase))
            {
                var expectedStatuses = $"{context.GetPolicy<KnownOrderStatusPolicy>().Pending}, { context.GetPolicy<KnownOrderStatusPolicy>().Problem}, { context.GetPolicy<KnownOrderStatusPolicy>().OnHold}";
                var invalidOrderStateMessage = $"{this.Name}: Expected order in '{expectedStatuses}' statuses but order was in '{order.Status}' status";
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().ValidationError,
                        "InvalidOrderState",
                        new object[] { expectedStatuses, order.Status },
                        invalidOrderStateMessage),
                    context);
                return arg;
            }

            var components = order.Components.OfType<FederatedPaymentComponent>().ToList();
            if (!components.Any())
            {
                return arg;
            }                                        

            var existingPayment = order.GetComponent<FederatedPaymentComponent>();
            if (existingPayment == null)
            {
                return arg;
            }

            await this.GenerateSalesActivity(order, existingPayment, context);


            return arg;
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

            await this._persistPipeline.Run(new PersistEntityArgument(salesActivity), context);
        }
    }
}
