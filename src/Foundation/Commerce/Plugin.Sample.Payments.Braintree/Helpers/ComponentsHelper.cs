// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentsHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Payments.Braintree
{
    using global::Braintree;
    using Sitecore.Commerce.Core;
    
    /// <summary>
    ///  A Components Helper to translate party for address request
    /// </summary>
    public class ComponentsHelper
    {
        /// <summary>
        /// Translates the party to address request.
        /// </summary>
        /// <param name="party">The party.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        internal static protected AddressRequest TranslatePartyToAddressRequest(Party party, CommercePipelineExecutionContext context)
        {            
            var addressRequest = new AddressRequest();
            addressRequest.CountryCodeAlpha2 = party.CountryCode;
            addressRequest.CountryName = party.Country;
            addressRequest.FirstName = party.FirstName;
            addressRequest.LastName = party.LastName;
            addressRequest.PostalCode = party.ZipPostalCode;
            addressRequest.StreetAddress = string.Concat(party.Address1, ",", party.Address2);

            return addressRequest;
        }
    }
}
