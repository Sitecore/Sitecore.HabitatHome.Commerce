// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BraintreeClientPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Payments.Braintree
{
    using Sitecore.Commerce.Core;

    /// <summary>
    /// Defines the BraintreeClientPolicy for Payments.
    /// </summary>
    public class BraintreeClientPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeClientPolicy" /> class.
        /// </summary>
        public BraintreeClientPolicy()
        {
            this.Environment = string.Empty;
            this.MerchantId = string.Empty;
            this.PublicKey = string.Empty;
            this.PrivateKey = string.Empty;
        }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the merchant identifier.
        /// </summary>
        /// <value>
        /// The merchant identifier.
        /// </value>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        /// <value>
        /// The public key.
        /// </value>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the private key.
        /// </summary>
        /// <value>
        /// The private key.
        /// </value>
        public string PrivateKey { get; set; }
    }
}
