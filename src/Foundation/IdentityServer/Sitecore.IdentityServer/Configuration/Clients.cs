// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Clients.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using IdentityServer4.Models;

    /// <summary>
    /// Clients Config for Authentication/Authorization
    /// </summary>
    public class Clients
    {
        /// <summary>
        /// Gets the specified application settings.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <returns>A collection of <see cref="Client"/></returns>
        public static IEnumerable<Client> Get(AppSettings appSettings)
        {
            return appSettings.Clients.Select(client => new Client
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                AccessTokenType = client.AccessTokenType,
                AllowedGrantTypes = client.AllowedGrantTypes,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                RedirectUris = client.RedirectUris,
                PostLogoutRedirectUris = client.PostLogoutRedirectUris,
                AllowedCorsOrigins = client.AllowedCorsOrigins,
                AllowedScopes = client.AllowedScopes,
                RequireConsent = client.RequireConsent,
                RequireClientSecret = client.RequireClientSecret,
                IdentityTokenLifetime = client.IdentityTokenLifetimeInSeconds,
                AccessTokenLifetime = client.AccessTokenLifetimeInSeconds
            }).ToList();
        }
    }
}
