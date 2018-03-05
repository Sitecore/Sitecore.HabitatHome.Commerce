// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resources.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using IdentityServer4.Models;

    /// <summary>
    /// Resources Config for Authentication/Authorization
    /// </summary>
    public class Resources
    {
        /// <summary>
        /// Gets the identity resources.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <returns>A collection of <see cref="IdentityResource"/></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources(AppSettings appSettings)
        {
            var identityResources = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
            identityResources.AddRange(appSettings.IdentityResources.Select(resource => new IdentityResource(resource.Name, resource.ClaimTypes)));
            return identityResources;
        }

        /// <summary>
        /// Gets the API resources.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <returns>A collection of <see cref="ApiResource"/></returns>
        public static IEnumerable<ApiResource> GetApiResources(AppSettings appSettings)
        {
            return appSettings.ApiResources.Select(resource => new ApiResource(resource.Name, resource.DisplayName)
            {
                ApiSecrets = resource.ApiSecrets.Select(secret => new Secret(secret.Sha256())).ToList(),
                UserClaims = resource.UserClaims
            });
        }
    }
}
