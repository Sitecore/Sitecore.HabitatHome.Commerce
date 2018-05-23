// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbayBusinessUserComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Components
{
    /// <inheritdoc />
    /// <summary>
    /// The EbayBusinessUserComponent.
    /// </summary>
    public class EbayBusinessUserComponent : Component
    {
        /// <summary>
        /// Component stored in an EbayConfigEntity that tracks individual information/policies about a specific Business User
        /// </summary>
        public EbayBusinessUserComponent()
        {
            EbayToken = "";
            Status = "Pending";
            TokenDate = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// A Bearer Token that was granted to the Business User by Ebay which allows the software to interact, on the companies behalf, with Ebay
        /// </summary>
        public string EbayToken { get; set; }

        /// <summary>
        /// The date the Token was added.  Since Token's do expire, it is important to track how long it has been
        /// </summary>
        public DateTimeOffset TokenDate { get; set; }

        /// <summary>
        /// Status code reprenting this Business User's Status
        /// </summary>
        public string Status { get; set; }


    }
}