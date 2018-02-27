// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetClientTokenBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Payments.Braintree
{
    using System.Threading.Tasks;
    using global::Braintree;
    using global::Braintree.Exceptions;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    ///  Defines a block which gets a payment service client tokent.
    /// </summary>
    /// <seealso>
    /// <cref>
    ///  Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///  </cref>
    ///  </seealso>
    [PipelineDisplayName(PaymentsBraintreeConstants.Pipelines.Blocks.GetClientTokenBlock)]
    public class GetClientTokenBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A client token string</returns>
        public async override Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var braintreeClientPolicy = context.GetPolicy<BraintreeClientPolicy>();
            if (braintreeClientPolicy == null)
            {
                await context.CommerceContext.AddMessage(
                     context.GetPolicy<KnownResultCodes>().Error,
                     "InvalidOrMissingPropertyValue",
                     new object[] { "BraintreeClientPolicy" },
                      $"{this.Name}. Missing BraintreeClientPolicy");
                return arg;
            }

            if (string.IsNullOrEmpty(braintreeClientPolicy?.Environment) || string.IsNullOrEmpty(braintreeClientPolicy?.MerchantId)
                || string.IsNullOrEmpty(braintreeClientPolicy?.PublicKey) || string.IsNullOrEmpty(braintreeClientPolicy?.PrivateKey))
            {
                return string.Empty;
            }

            try
            {
                var gateway = new BraintreeGateway(braintreeClientPolicy?.Environment, braintreeClientPolicy?.MerchantId, braintreeClientPolicy?.PublicKey, braintreeClientPolicy?.PrivateKey);
                var clientToken = gateway.ClientToken.generate();
                return clientToken;
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
    }
}
