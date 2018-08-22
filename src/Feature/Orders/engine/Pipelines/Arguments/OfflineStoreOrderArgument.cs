using System.Collections.Generic;
using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments
{
    public class OfflineStoreOrderArgument : PipelineArgument
    {
        public OfflineStoreOrderArgument()
        {
            Lines = new List<OfflineOrderLine>();
            StoreDetails = new Store();
        }

        public string ShopName { get; set; }

        public string OrderConfirmationId { get; set; }

        public string OrderPlacedDate { get; set; }

        public string Email { get; set; }

        public string Domain { get; set; }

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
}

