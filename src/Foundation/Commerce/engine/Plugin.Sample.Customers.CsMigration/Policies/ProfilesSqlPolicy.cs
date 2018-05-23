// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilesSqlPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   A class used to establish the connection policy to the Commerce Profiles and tables
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;

    /// <summary>
    /// A class used to establish the connection policy to the Commerce Profiles and tables
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class ProfilesSqlPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesSqlPolicy"/> class.
        /// </summary>
        public ProfilesSqlPolicy()
        {
            this.Server = ".";
            this.Database = "";
            this.TrustedConnection = true;
            this.UserName = "";
            this.Password = "";
            this.AdditionalParameters = "";
            this.ConnectTimeout = 120000;
        }

        /// <summary>
        /// The _connection.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Gets or sets a value indicating to use a trusted connection
        /// </summary>
        public bool TrustedConnection { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Server name.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the Database name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Additional ConnectionString Parameters
        /// </summary>
        public string AdditionalParameters { get; set; }

        /// <summary>
        /// Connection Timeout (1000)
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// Generates a SQL Connection String based on the policy.
        /// </summary>
        /// <param name="context">
        /// The calling context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ConnectionString(CommerceContext context)
        {

            if (!string.IsNullOrEmpty(_connectionString))
            {
                return _connectionString;
            }

            var additionalParameters = string.IsNullOrEmpty(AdditionalParameters) ? "" : ";" + AdditionalParameters;          

            _connectionString = TrustedConnection ?
                    $"Server={Server};Database={Database};Trusted_Connection=True;Connect Timeout={ConnectTimeout}{additionalParameters}" :
                    $"Server={Server};Database={Database};Connect Timeout={ConnectTimeout};User={UserName};Password={Password}{additionalParameters}";

            return _connectionString;
        }
    }
}
