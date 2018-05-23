using System;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace Sitecore.HabitatHome.Foundation.StoreLocator.Models
{
    public class InventoryStore
    {
        public string Id { get; set; }
        public string InventoryStoreId { get; set; }
        public string DisplayName { get; set; }
        public double Distance { get; set; }
        public int InventoryAmount { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public InventoryStore(dynamic result)
        {
            this.Id = result.Id;
            this.InventoryStoreId = result.InventoryStoreId;
            this.DisplayName = result.DisplayName;
            this.Distance = result.Distance;
            this.Address = result.Address;
            this.City = result.City;
            this.Country = result.Country;
            this.Zip = result.Zip;
            this.State = result.State;

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
            model.Address = this.Address;
            model.City = this.City;
            model.Zip = this.Zip;
            model.State = this.State;           
            model.Country = this.Country;
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