// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefundFederatedPaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Foundation.Payments.Engine.Pipelines.Blocks
{
    /// <summary>
    /// Defines a refund federated paymentBlock.
    /// </summary>
    /// <seealso>
    /// <cref>
    ///    Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Payments.OrderPaymentsArgument, 
    ///    Sitecore.Commerce.Plugin.Payments.Order, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    /// </cref>
    /// </seealso>
    [PipelineDisplayName(PaymentsMoneybagsConstants.Pipelines.Blocks.RefundFederatedPaymentBlock)]
    public class RefundFederatedPaymentBlock : PipelineBlock<OrderPaymentsArgument, Order, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundFederatedPaymentBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public RefundFederatedPaymentBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            this._persistPipeline = persistEntityPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An OrderPaymentsArgument with order and Federated existingPayment info
        /// </returns>
        public override async Task<Order> Run(OrderPaymentsArgument arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            Condition.Requires(arg).IsNotNull("The arg can not be null");
            Condition.Requires(arg.Order).IsNotNull("The order can not be null");

            var order = arg.Order;

            if (!order.Status.Equals(context.GetPolicy<KnownOrderStatusPolicy>().Completed, StringComparison.OrdinalIgnoreCase))
            {
                var invalidOrderStateMessage = $"{this.Name}: Expected order in '{context.GetPolicy<KnownOrderStatusPolicy>().Completed}' status but order was in '{order.Status}' status";
                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().ValidationError,
                        "InvalidOrderState",
                        new object[] { context.GetPolicy<KnownOrderStatusPolicy>().OnHold, order.Status },
                        invalidOrderStateMessage);
                return null;
            }

            if (!order.HasComponent<FederatedPaymentComponent>())
            {
                return order;
            }

            var existingPayment = order.GetComponent<FederatedPaymentComponent>();
            var paymentToRefund = arg.Payments.FirstOrDefault(p => p.Id.Equals(existingPayment.Id, StringComparison.OrdinalIgnoreCase)) as FederatedPaymentComponent;
            if (paymentToRefund == null)
            {
                return order;
            }

            if (existingPayment.Amount.Amount < paymentToRefund.Amount.Amount)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "IllegalRefundOperation",
                    new object[] { order.Id, existingPayment.Id },
                    "Order Federated Payment amount is less than refund amount");
                return null;
            }


            // force payment refund
            context.Logger.LogInformation($"{this.Name} - Refund Payment succeeded:{paymentToRefund.Id}");
            existingPayment.TransactionStatus = "Success";

            if (existingPayment.Amount.Amount == paymentToRefund.Amount.Amount)
            {
                order.Components.Remove(existingPayment);
            }
            else
            {
                // Reduce existing existingPayment by the amount being refunded
                existingPayment.Amount.Amount -= paymentToRefund.Amount.Amount;
            }

            await this.GenerateSalesActivity(order, existingPayment, paymentToRefund, Guid.NewGuid().ToString(), context);


            return order;
        }

        /// <summary>
        /// Generates the sales activity.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="existingPayment">The existingPayment.</param>
        /// <param name="paymentToRefund">The payment to refund</param>
        /// <param name="refundTransactionId">The refund transaction identifier.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="Task" />
        /// </returns>
        protected virtual async Task GenerateSalesActivity(Order order, PaymentComponent existingPayment, PaymentComponent paymentToRefund, string refundTransactionId, CommercePipelineExecutionContext context)
        {
            var salesActivity = new SalesActivity
            {
                Id = CommerceEntity.IdPrefix<SalesActivity>() + Guid.NewGuid().ToString("N"),
                ActivityAmount = new Money(existingPayment.Amount.CurrencyCode, paymentToRefund.Amount.Amount * -1),
                Customer = new EntityReference
                {
                    EntityTarget = order.Components.OfType<ContactComponent>().FirstOrDefault()?.CustomerId
                },
                Order = new EntityReference
                {
                    EntityTarget = order.Id
                },
                Name = "Refund the Federated Payment",
                PaymentStatus = context.GetPolicy<KnownSalesActivityStatusesPolicy>().Completed
            };

            salesActivity.SetComponent(new ListMembershipsComponent
            {
                Memberships = new List<string>
                    {
                        CommerceEntity.ListName<SalesActivity>(),
                        context.GetPolicy<KnownOrderListsPolicy>().SalesCredits,
                        string.Format(context.GetPolicy<KnownOrderListsPolicy>().OrderSalesActivities, order.FriendlyId)
                    }
            });

            if (existingPayment.Amount.Amount != paymentToRefund.Amount.Amount)
            {
                salesActivity.SetComponent(existingPayment);
            }

            if (!string.IsNullOrEmpty(refundTransactionId))
            {
                salesActivity.SetComponent(new TransactionInformationComponent(refundTransactionId));
            }

            var salesActivities = order.SalesActivity.ToList();
            salesActivities.Add(new EntityReference { EntityTarget = salesActivity.Id });
            order.SalesActivity = salesActivities;

            await this._persistPipeline.Run(new PersistEntityArgument(salesActivity), context);
        }
    }
}
