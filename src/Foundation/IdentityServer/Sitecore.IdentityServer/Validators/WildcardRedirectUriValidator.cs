// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreResourceOwnerValidator.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sitecore.IdentityServer.Validators
{
    using IdentityServer4.Validation;
    /// <summary>
    /// Overrides identity server's default uri validator to support subPaths
    /// </summary>
    public class WildcardRedirectUriValidator : IRedirectUriValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestedUri"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public Task<bool> IsRedirectUriValidAsync(string requestedUri, IdentityServer4.Models.Client client)
        {
            return MatchUriAsync(requestedUri, client.RedirectUris.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestedUri"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, IdentityServer4.Models.Client client)
        {
            return MatchUriAsync(requestedUri, client.RedirectUris.ToList());
        }

        private static Task<bool> MatchUriAsync(string requestedUri, IEnumerable<string> allowedUris)
        {
            var rules = allowedUris.Select(ConvertToRegex).ToList();
            var res = rules.Any(r => Regex.IsMatch(requestedUri, r, RegexOptions.IgnoreCase));
            return Task.FromResult(res);
        }

        private const string WildcardCharacter = @"[a-zA-Z0-9\-]";

        private static string ConvertToRegex(string rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return Regex.Escape(rule)
                .Replace(@"\*", WildcardCharacter + "*")
                .Replace(@"\?", WildcardCharacter);
        }
    }
}
