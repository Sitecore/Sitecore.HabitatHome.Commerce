// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettleFederatedPaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Payments.Braintree
{
    using System;
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
    ///  Defines the settle federated payment block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Orders.SalesActivity,
    ///         Sitecore.Commerce.Plugin.Orders.SalesActivity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(PaymentsBraintreeConstants.Pipelines.Blocks.SettleFederatedPaymentBlock)]
    public class SettleFederatedPaymentBlock : PipelineBlock<SalesActivity, SalesActivity, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A cart with Federated payment info</returns>
        public override async Task<SalesActivity> Run(SalesActivity arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The sales activity cannot be null.");

            var salesActivity = arg;
            var knownSalesActivityStatuses = context.GetPolicy<KnownSalesActivityStatusesPolicy>();
            if (!salesActivity.HasComponent<FederatedPaymentComponent>() 
                || !salesActivity.PaymentStatus.Equals(knownSalesActivityStatuses.Pending, StringComparison.OrdinalIgnoreCase))
            {
                return salesActivity;
            }

            var payment = salesActivity.GetComponent<FederatedPaymentComponent>();   
            if (string.IsNullOrEmpty(payment.TransactionId))
            {
                salesActivity.PaymentStatus = knownSalesActivityStatuses.Problem;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "TransactionId" },
                    "Invalid or missing value for property 'TransactionId'.");
                return salesActivity;
            }

            var braintreeClientPolicy = context.GetPolicy<BraintreeClientPolicy>();
            if (string.IsNullOrEmpty(braintreeClientPolicy.Environment) || string.IsNullOrEmpty(braintreeClientPolicy.MerchantId)
                || string.IsNullOrEmpty(braintreeClientPolicy.PublicKey) || string.IsNullOrEmpty(braintreeClientPolicy.PrivateKey))
            {
                salesActivity.PaymentStatus = knownSalesActivityStatuses.Problem;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "InvalidClientPolicy",
                    new object[] { "BraintreeClientPolicy" },
                    $"{this.Name}. Invalid BraintreeClientPolicy");
                return salesActivity;
            }

            try
            {
                var gateway = new BraintreeGateway(
                    braintreeClientPolicy.Environment, 
                    braintreeClientPolicy.MerchantId, 
                    braintreeClientPolicy.PublicKey, 
                    braintreeClientPolicy.PrivateKey);

                var transaction = gateway.Transaction.Find(payment.TransactionId);
                if (transaction.Status.ToString().Equals("authorized", StringComparison.OrdinalIgnoreCase))
                {
                    var result = gateway.Transaction.SubmitForSettlement(payment.TransactionId, payment.Amount.Amount);

                    if (result.IsSuccess())
                    {
                        transaction = result.Target;
                        payment.TransactionStatus = transaction.Status.ToString();

                        // Force settlement for testing
                        if (braintreeClientPolicy.Environment.Equals("sandbox", StringComparison.OrdinalIgnoreCase))
                        {
                            gateway.TestTransaction.Settle(payment.TransactionId);
                            transaction = gateway.Transaction.Find(payment.TransactionId);
                            payment.TransactionStatus = transaction.Status.ToString();
                        }

                        switch (transaction.Status.ToString())
                        {
                            case "settled":
                                salesActivity.PaymentStatus = knownSalesActivityStatuses.Settled;
                                break;
                            case "submitted_for_settlement":
                            case "settling":
                                salesActivity.PaymentStatus = knownSalesActivityStatuses.Pending;
                                break;
                            default:
                                salesActivity.PaymentStatus = knownSalesActivityStatuses.Problem;
                                await context.CommerceContext.AddMessage(
                                     context.GetPolicy<KnownResultCodes>().Error,
                                     "SettlePaymentFailed",
                                     new object[] { payment.TransactionId },
                                   $"{this.Name}. Settle payment failed for { payment.TransactionId }: { transaction.ProcessorResponseText }");
                                break;
                        }
                    }
                    else
                    {
                        salesActivity.PaymentStatus = knownSalesActivityStatuses.Problem;
                        var errorMessages = result.Errors.DeepAll().Aggregate(string.Empty, (current, error) => current + ("Error: " + (int)error.Code + " - " + error.Message + "\n"));
                        await context.CommerceContext.AddMessage(
                            context.GetPolicy<KnownResultCodes>().Error,
                            "SettlePaymentFailed",
                            new object[] { payment.TransactionId },
                            $"{this.Name}. Settle payment failed for {payment.TransactionId}: {errorMessages}");
                    }                    
                }
            }
            catch (BraintreeException ex)
            {
                salesActivity.PaymentStatus = knownSalesActivityStatuses.Problem;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "SettlePaymentFailed",
                    new object[] { payment.TransactionId, ex },
                    $"{this.Name}. Settle payment failed for {payment.TransactionId}");
            }

            return salesActivity; 
        }
    }
}
