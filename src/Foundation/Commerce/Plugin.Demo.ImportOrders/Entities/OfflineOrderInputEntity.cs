using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Plugin.Demo.ImportOrders.Entities
{
    public class OfflineOrderInputEntity : CommerceEntity
    {
        public string ShopName { get; set; }
        public string OrderConfirmationId { get; set; }
        public string OrderPlacedDate { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal Discount { get; set; }
        public string PaymentInstrumentType { get; set; }
        public string CardType { get; set; }
        public string MaskedNumber { get; set; }
        public int ExpiresMonth { get; set; }
        public int ExpiresYear { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionId { get; set; }
        public List<OfflineOrderLine> Lines { get; set; }
        public Store StoreDetails { get; set; }
    }

    public class OfflineOrderLine
    {
        public string ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal UnitListPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class Store
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public uint ZipCode { get; set; }
        public string Country { get; set; }
    }
}
