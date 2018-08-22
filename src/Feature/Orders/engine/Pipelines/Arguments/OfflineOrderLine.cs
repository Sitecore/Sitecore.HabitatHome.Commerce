using System; 

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments
{
    public class OfflineOrderLine
    {
        public string ItemId { get; set; }

        public decimal Quantity { get; set; }

        public string ProductName { get; set; }

        public decimal UnitListPrice { get; set; }

        public decimal SubTotal { get; set; }
    }
}