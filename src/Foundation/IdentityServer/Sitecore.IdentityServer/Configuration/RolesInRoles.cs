// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RolesInRoles.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Configuration
{
    /// <summary>
    /// Roles Config for authentication
    /// </summary>
    public class RolesInRoles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RolesInRoles"/> class.
        /// </summary>
        public RolesInRoles() 
        {
        }

        /// <summary>
        /// Gets the specified application settings.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <returns></returns>
        public static RolesInRoles Get(AppSettings appSettings)
        {
            return new RolesInRoles()
            {
                Enabled = appSettings.RolesInRoles.Enabled,
                ApplicationName = appSettings.RolesInRoles.ApplicationName,
                DatabaseTable = appSettings.RolesInRoles.DatabaseTable,
                ConnectionString = appSettings.SitecoreMembershipOptions.ConnectionString
            };
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RolesInRoles"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the database table.
        /// </summary>
        public string DatabaseTable { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
