// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.HabitatHome.Feature.EBay.Engine.Models;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Components
{
    /// <inheritdoc />
    /// <summary>
    /// Component stored in a SellableItem to track publishing to Ebay
    /// </summary>
    public class EbayItemComponent : Component
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public EbayItemComponent()
        {
            EbayId = "";
            Fees = new List<AwardedAdjustment>();
            Status = "";
            ReasonEnded = "";
            History = new List<HistoryEntryModel>();
        }

        /// <summary>
        /// Ebay's unique id for the listing
        /// </summary>
        public string EbayId { get; set; }

        /// <summary>
        /// A Status code representing a Sellable Item's publishing status with Ebay
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// If a Listing is ended, this tracks the code selected as the reason
        /// </summary>
        public string ReasonEnded { get; set; }

        /// <summary>
        /// A List of History Events
        /// </summary>
        public List<HistoryEntryModel> History { get; set; }

        /// <summary>
        /// When a listing is made, Ebay returns a collection of Fees that could occur as part of the Listing lifecycle, these are stored and are visible when viewing Sellable Items
        /// </summary>
        public List<AwardedAdjustment> Fees { get; set; }
    }
}