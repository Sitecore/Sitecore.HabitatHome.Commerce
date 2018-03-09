// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilePropertiesPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   A ProfilesCs properties policy
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Sitecore.Commerce.Core;

    /// <summary>
    /// A ProfilesCs properties policy
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class ProfilePropertiesPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePropertiesPolicy"/> class.
        /// </summary>
        public ProfilePropertiesPolicy()
        {
            this.AddressType = "Address";
            this.UserObjectType = "UserObject";
            this.UserIdProperty = "user_id";
            this.AccountStatusProperty = "account_status";
            this.PasswordProperty = "user_security_password";
            this.AccountNumber = "AccountNumber";
        }

        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>
        /// The type of the address.
        /// </value>
        public string AddressType { get; set; }

        /// <summary>
        /// Gets or sets the type of the user object.
        /// </summary>
        /// <value>
        /// The type of the user object.
        /// </value>
        public string UserObjectType { get; set; }

        /// <summary>
        /// Gets or sets the user identifier property.
        /// </summary>
        /// <value>
        /// The user identifier property.
        /// </value>
        public string UserIdProperty { get; set; }

        /// <summary>
        /// Gets or sets the account status property.
        /// </summary>
        /// <value>
        /// The account status property.
        /// </value>
        public string AccountStatusProperty { get; set; }

        /// <summary>
        /// Gets or sets the password property.
        /// </summary>
        /// <value>
        /// The password property.
        /// </value>
        public string PasswordProperty { get; set; }

        /// <summary>
        /// Gets or sets the account number.
        /// </summary>
        /// <value>
        /// The account number.
        /// </value>
        public string  AccountNumber { get; set; }
    }
}
