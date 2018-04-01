// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Models
{
    /// <summary>
    /// Login View Model
    /// </summary>
    public class LoginViewModel : LoginInputModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether [enable local login].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable local login]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLocalLogin { get; set; }
    }
}
