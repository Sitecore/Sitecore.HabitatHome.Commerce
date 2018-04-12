using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Sitecore.Foundation.Commerce.StoreLocator.Models
{
    public class InventoryStore
    {
        public string Id { get; set; }
        public string InventoryStoreId { get; set; }
        public string DisplayName { get; set; }
        public double Distance { get; set; }
        public int InventoryAmount { get; set; }
        public InventoryStore(dynamic result)
        {
            this.Id = result.Id;
            this.InventoryStoreId = result.InventoryStoreId;
            this.DisplayName = result.DisplayName;
            this.Distance = result.Distance;

        }
        public dynamic GetViewModel()
        {
            dynamic model = new ExpandoObject();
            model.Id = this.Id;
            model.InventoryStoreId = this.InventoryStoreId;
            model.DisplayName = GetDisplayName(this.InventoryStoreId);
            model.Distance = GetDistanceInMiles(this.Distance);
            model.InventoryAmount = this.InventoryAmount;
            model.ZeroInventory = this.InventoryAmount == 0;
            model.Limited = this.InventoryAmount < 6 && this.InventoryAmount != 0 ? true : false;
            return model;
        }
        private string GetDisplayName(string StoreId)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return (r.Replace(StoreId, " "));
        }
        private double GetDistanceInMiles(double meters)
        {
            return (Math.Round(meters / 1609.344, 1));
        }
    }
}