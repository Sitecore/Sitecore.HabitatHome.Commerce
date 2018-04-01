// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreResourceOwnerValidator.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Contrib.Membership.Helpers;
using IdentityServer4.Contrib.Membership.Interfaces;
using IdentityServer4.Validation;

namespace Sitecore.IdentityServer.Validators
{
    /// <summary>
    /// Implement IResourceOwnerPasswordValidator to provide the way to validate access in Silet mode (Without using UI)
    /// </summary>
    public class SitecoreResourceOwnerValidator : IResourceOwnerPasswordValidator
    {
        private readonly IMembershipService _membershipService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="membershipService"></param>
        public SitecoreResourceOwnerValidator(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <summary>
        /// Validate user credentials
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (await _membershipService.ValidateUser(context.UserName, context.Password))
            {
                var user = await _membershipService.GetUserAsync(context.UserName);
                context.Result = new GrantValidationResult(user.GetSubjectId(), OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}
