using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.SecurityModel;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Analytics;
using System.Net.Http;
using Sitecore.Commerce.Entities.Orders;
//using Sitecore.XConnect;
//using Sitecore.XConnect.Client;



namespace Sitecore.Commerce.Website.Views.OrderUpload
{
    public partial class UploadOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var order = txtOrderJson.Text;
            OrderInput input = JsonConvert.DeserializeObject<OrderInput>(order);

            // Send order to Authoring
            try
            {
                var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
                var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);


                var content = new System.Net.Http.StringContent(order);
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json");

                var client = this.GetClient(ceConfig);
                var result = client.PostAsync("CreateOfflineOrder", content).Result;
                // Response.Write("client.PostAsync");
                               

                if (result.IsSuccessStatusCode)
                {
                    // AddOutcome();

                    // Call xconnect pipelines
                    var email = input.Order.Email;
                    Tracker.Current.Session.IdentifyAs(email, "username");

                    Sitecore.Commerce.Entities.Orders.Order outComeOrder = new Order();
                    


                    //<processor type="Sitecore.Commerce.Pipelines.Orders.TriggerOrderOutcome, Sitecore.Commerce.Connect.Core">
                    //	<OutcomeId>{9016E456-95CB-42E9-AD58-997D6D77AE83}</OutcomeId>
                    //</processor>
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

           
        }


        public HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(config.ShopsServiceUrl)
            };

            Response.Write(config.ShopsServiceUrl);
            httpClient.DefaultRequestHeaders.Add("ShopName", config.DefaultShopName);
            httpClient.DefaultRequestHeaders.Add("Language", "en-US");
            httpClient.DefaultRequestHeaders.Add("Currency", config.DefaultShopCurrency);
            httpClient.DefaultRequestHeaders.Add("Environment", config.DefaultEnvironment);

            string certificate = config.GetCertificate();
            if (certificate != null)
                httpClient.DefaultRequestHeaders.Add(config.CertificateHeaderName, certificate);
            httpClient.Timeout = new System.TimeSpan(0, 0, 600);
            Response.Write(config.CertificateHeaderName);
            return httpClient;

        }
    }

    public class OrderInput
    {
        public ImportOrderModel Order { get; set; }
    }
    public class ImportOrderModel
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