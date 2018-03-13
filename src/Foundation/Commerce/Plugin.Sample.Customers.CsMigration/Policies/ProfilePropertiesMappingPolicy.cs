// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilePropertiesMappingPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Core;

    /// <summary>
    /// Defines a Profile properties mapping policy
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class ProfilePropertiesMappingPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePropertiesMappingPolicy"/> class.
        /// </summary>
        public ProfilePropertiesMappingPolicy()
        {
            this.UserProperties = new Dictionary<string, string>()
            {
                { "GeneralInfo.user_id", "AccountNumber" },
                { "AccountInfo.account_status", "AccountStatus" },
                { "GeneralInfo.email_address", "Email" },
                { "GeneralInfo.first_name", "FirstName" },
                { "GeneralInfo.last_name", "LastName" }
            };

            this.AddressProperties = new Dictionary<string, string>()
            {
                { "GeneralInfo.address_name", "AddressName" },
                { "GeneralInfo.first_name", "FirstName" },
                { "GeneralInfo.last_name", "LastName" },
                { "GeneralInfo.country_name", "Country" },
                { "GeneralInfo.country_code", "CountryCode" },
                { "GeneralInfo.region_name", "State" },
                { "GeneralInfo.region_code", "StateCode" },
                { "GeneralInfo.city", "City" },
                { "GeneralInfo.address_line1", "Address1" },
                { "GeneralInfo.address_line2", "Address2" },
                { "GeneralInfo.postal_code", "ZipPostalCode" },
                { "GeneralInfo.tel_number", "PhoneNumber" }
            };
        }

        /// <summary>
        /// Gets or sets the user properties.
        /// </summary>
        /// <value>
        /// The user properties.
        /// </value>
        public Dictionary<string, string> UserProperties { get; set; }

        /// <summary>
        /// Gets or sets the address properties.
        /// </summary>
        /// <value>
        /// The address properties.
        /// </value>
        public Dictionary<string, string> AddressProperties { get; set; }
    }       
}
