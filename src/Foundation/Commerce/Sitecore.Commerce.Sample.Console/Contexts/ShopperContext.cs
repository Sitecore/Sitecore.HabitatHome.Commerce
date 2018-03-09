using Sitecore.Commerce.Sample.Console.Authentication;

namespace Sitecore.Commerce.Sample
{
    using System;
    using System.Collections.Generic;

    using Engine;
    using Microsoft.OData.Client;
    using Sitecore.Commerce.Core;

    using CommerceOps = CommerceOps.Sitecore.Commerce.Engine;

    public class ShopperContext
    {
        private Container _shopsContainer;
        private Container _authoringContainer;

        public ShopperContext()
        {
            this.IsRegistered = false;
            this.Shop = Console.Program.DefaultStorefront;
            this.ShopperId = "ConsoleShopper01";
            this.Environment = "AdventureWorksShops";
            this.Language = "en-US";
            this.Currency = "USD";
            this.EffectiveDate = DateTimeOffset.Now;
            this.CustomerId = "DefaultCustomerId";
            this.PolicyKeys = string.Empty;
            this.GiftCards = new List<string>
                                 {
                                     "GC1000000",
                                     "GC100"
                                 };
        }

        public bool IsRegistered { get; set; }

        public string ShopperId { get; set; }

        public string Shop { get; set; }

        public string Language { get; set; }

        public string Currency { get; set; }

        public string Environment { get; set; }

        public string CustomerId { get; set; }

        public DateTimeOffset EffectiveDate { get; set; }

        public List<Component> Components { get; set; }

        public List<string> GiftCards { get; set; }

        public string PolicyKeys { get; set; }

        public Container ShopsContainer()
        {
            if (this._shopsContainer != null)
            {
                return this._shopsContainer;
            }

            this._shopsContainer = new Container(new Uri(Console.Program.ShopsServiceUri))
                                       {
                                           MergeOption = MergeOption.OverwriteChanges,
                                           DisableInstanceAnnotationMaterialization = true
                                       };

            this._shopsContainer.BuildingRequest += (s, e) =>
                {
                    e.Headers.Add("ShopName", this.Shop);
                    e.Headers.Add("ShopperId", this.ShopperId);
                    e.Headers.Add("CustomerId", this.CustomerId);
                    e.Headers.Add("Language", this.Language);
                    e.Headers.Add("Currency", this.Currency);
                    e.Headers.Add("Environment", this.Environment);
                    e.Headers.Add("PolicyKeys", this.PolicyKeys);
                    e.Headers.Add("EffectiveDate", this.EffectiveDate.ToString());
                    e.Headers.Add("IsRegistered", this.IsRegistered.ToString());
                    e.Headers.Add("Authorization", Console.Program.SitecoreToken);
                };
            return this._shopsContainer;
        }

        public Container AuthoringContainer()
        {
            if (this._authoringContainer != null)
            {
                return this._authoringContainer;
            }

            this._authoringContainer = new Container(new Uri(Console.Program.ShopsServiceUri))
            {
                MergeOption = MergeOption.OverwriteChanges,
                DisableInstanceAnnotationMaterialization = true
            };

            this._authoringContainer.BuildingRequest += (s, e) =>
            {
                e.Headers.Add("ShopName", this.Shop);
                e.Headers.Add("Language", this.Language);
                e.Headers.Add("Currency", this.Currency);
                e.Headers.Add("Environment", this.Environment);
                e.Headers.Add("PolicyKeys", this.PolicyKeys);
                e.Headers.Add("Authorization", Console.Program.SitecoreToken);
            };
            return this._authoringContainer;
        }

        public CommerceOps.Container OpsContainer()
        {
            var container = new CommerceOps.Container(new Uri(Console.Program.OpsServiceUri))
            {
                MergeOption = MergeOption.OverwriteChanges,
                DisableInstanceAnnotationMaterialization = true
            };

            container.BuildingRequest += (s, e) =>
            {
                e.Headers.Add("PolicyKeys", this.PolicyKeys);
                e.Headers.Add("Authorization", Console.Program.SitecoreToken);
            };

            return container;
        }

        public CommerceOps.Container MinionsContainer()
        {
            var container = new CommerceOps.Container(new Uri(Console.Program.MinionsServiceUri))
            {
                MergeOption = MergeOption.OverwriteChanges,
                DisableInstanceAnnotationMaterialization = true
            };

            container.BuildingRequest += (s, e) =>
            {
                e.Headers.Add("PolicyKeys", this.PolicyKeys);
                e.Headers.Add("Authorization", Console.Program.SitecoreToken);
            };

            return container;
        }
    }
}
