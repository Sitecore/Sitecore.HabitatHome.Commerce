// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbayGlobalConfigComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Components
{
    /// <inheritdoc />
    /// <summary>
    /// The EbayGlobalConfigComponent.
    /// </summary>
    public class EbayGlobalConfigComponent : Component
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public EbayGlobalConfigComponent()
        {
            ReturnsPolicy = "ReturnsAccepted";
            InventorySet = "";
        }

        /// <summary>
        /// Code reprenting an Ebay Return Policy (ReturnsAccepted)
        /// </summary>
        public string ReturnsPolicy { get; set; }

        /// <summary>
        /// An InventorySet that is used to track inventory allocated for sale on Ebay.  If no InventorySet is identified then allocation is not available.
        /// </summary>
        public string InventorySet { get; set; }


    }
}