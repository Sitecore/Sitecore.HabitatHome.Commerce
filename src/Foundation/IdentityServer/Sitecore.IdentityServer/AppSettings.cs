// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer
{
    using System.Collections.Generic;
    using IdentityServer4.Models;
    using Sitecore.IdentityServer.Configuration;

    /// <summary>
    /// Represents Settings frm appSettings.json
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the sitecore membership options.
        /// </summary>
        /// <value>
        /// The sitecore membership options.
        /// </value>
        public SitecoreMembershipOptions SitecoreMembershipOptions { get; set; }

        /// <summary>
        /// Gets or sets the identity server cors policy.
        /// </summary>
        /// <value>
        /// The identity server cors policy.
        /// </value>
        public IdentityServerCorsPolicy IdentityServerCorsPolicy { get; set; }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public IEnumerable<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        public IEnumerable<IdentityResource> IdentityResources { get; set; }

        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public IEnumerable<ApiResource> ApiResources { get; set; }

        /// <summary>
        /// Gets or sets the identifier server certificate thumbprint.
        /// </summary>
        /// <value>
        /// The identifier server certificate thumbprint.
        /// </value>
        public string IDServerCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the identifier server certificate store location.
        /// </summary>
        /// <value>
        /// The identifier server certificate store location.
        /// </value>
        public string IDServerCertificateStoreLocation { get; set; }

        /// <summary>
        /// Gets or sets the name of the identifier server certificate store.
        /// </summary>
        /// <value>
        /// The name of the identifier server certificate store.
        /// </value>
        public string IDServerCertificateStoreName { get; set; }

        /// <summary>
        /// Gets or sets the roles in roles.
        /// </summary>
        /// <value>
        /// The roles in roles.
        /// </value>
        public RolesInRoles RolesInRoles { get; set; }
    }

    /// <summary>
    /// Represents Sitecore membership options structure
    /// </summary>
    public class SitecoreMembershipOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use role provider source].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use role provider source]; otherwise, <c>false</c>.
        /// </value>
        public bool UseRoleProviderSource { get; set; }
    }

    /// <summary>
    /// Represents Client Structure
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow access tokens via browser].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow access tokens via browser]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAccessTokensViaBrowser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require consent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require consent]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require client secret].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require client secret]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the access token lifetime in seconds.
        /// </summary>
        /// <value>
        /// The access token lifetime in seconds.
        /// </value>
        public int AccessTokenLifetimeInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the identity token lifetime in seconds.
        /// </summary>
        /// <value>
        /// The identity token lifetime in seconds.
        /// </value>
        public int IdentityTokenLifetimeInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the type of the access token.
        /// </summary>
        /// <value>
        /// The type of the access token.
        /// </value>
        public AccessTokenType AccessTokenType { get; set; }

        /// <summary>
        /// Gets or sets the allowed grant types.
        /// </summary>
        /// <value>
        /// The allowed grant types.
        /// </value>
        public ICollection<string> AllowedGrantTypes { get; set; }

        /// <summary>
        /// Gets or sets the redirect uris.
        /// </summary>
        /// <value>
        /// The redirect uris.
        /// </value>
        public ICollection<string> RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the post logout redirect uris.
        /// </summary>
        /// <value>
        /// The post logout redirect uris.
        /// </value>
        public ICollection<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the allowed cors origins.
        /// </summary>
        /// <value>
        /// The allowed cors origins.
        /// </value>
        public ICollection<string> AllowedCorsOrigins { get; set; }

        /// <summary>
        /// Gets or sets the allowed scopes.
        /// </summary>
        /// <value>
        /// The allowed scopes.
        /// </value>
        public ICollection<string> AllowedScopes { get; set; }
    }

    /// <summary>
    /// Represents Identity Resource Structure
    /// </summary>
    public class IdentityResource
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the claim types.
        /// </summary>
        /// <value>
        /// The claim types.
        /// </value>
        public ICollection<string> ClaimTypes { get; set; }
    }

    /// <summary>
    /// Represents Api Resource Structure 
    /// </summary>
    public class ApiResource
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the API secrets.
        /// </summary>
        /// <value>
        /// The API secrets.
        /// </value>
        public ICollection<string> ApiSecrets { get; set; }

        /// <summary>
        /// Gets or sets the user claims.
        /// </summary>
        /// <value>
        /// The user claims.
        /// </value>
        public ICollection<string> UserClaims { get; set; }
    }

    /// <summary>
    /// Represents Identity Server Cors Policy Structure
    /// </summary>
    public class IdentityServerCorsPolicy
    {
        /// <summary>
        /// Gets or sets the origins.
        /// </summary>
        /// <value>
        /// The origins.
        /// </value>
        public string[] Origins { get; set; }

        /// <summary>
        /// Gets or sets the exposed header.
        /// </summary>
        /// <value>
        /// The exposed header.
        /// </value>
        public string ExposedHeader { get; set; }
    }
}
