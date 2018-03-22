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
using Newtonsoft.Json.Linq;
using Sitecore.Pipelines;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Orders;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;


namespace Sitecore.Feature.UploadOrder.HabitatUtility
{
    public partial class UploadOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        // It is important to have currency populated at all places.
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var order = txtOrderJson.Text;
            string startSpanRed = "<span style= \"color:red\">";
            string startSpanGreen = "<span style= \"color:green\">";
            string endSpan = "</span>";

            if (string.IsNullOrEmpty(order))
            {
                Response.Write("Input is empty");
                return;
            }
            OrderInput inputJson = JsonConvert.DeserializeObject<OrderInput>(order);
            Sitecore.Context.SetActiveSite("Storefront");

            //var response ="{\"ShopName\":\"SacramentoStore\",\"OrderPlacedDate\":\"2018-02-14T14:32:41.5332063-08:00\",\"OrderConfirmationId\":\"670003123232\",\"Status\":\"StoreOrder\",\"SalesActivity\":[],\"Lines\":[{\"ItemId\":\"Habitat_Master|7042121|57042121\",\"ParentId\":null,\"Quantity\":1.0,\"UnitListPrice\":{\"CurrencyCode\":\"USD\",\"Amount\":429.99},\"Totals\":{\"SubTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":429.99},\"AdjustmentsTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":0.0},\"GrandTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":429.99},\"PaymentsTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":0.0},\"Name\":\"\",\"Policies\":[]},\"Adjustments\":[],\"Id\":\"05dfb3baf98543ccb27dffbf29c39263\",\"Name\":\"\",\"Comments\":\"\",\"Policies\":[{\"SellPrice\":{\"CurrencyCode\":\"USD\",\"Amount\":429.99},\"Expires\":\"2018-03-21T16:30:39.4487539+00:00\",\"PolicyId\":\"c24f0ed4f2f1449b8a488403b6bf368a\",\"Models\":[]}],\"ChildComponents\":[{\"ProductName\":\"\",\"ItemTemplate\":null,\"DisplayName\":\"Optix 12.8MP 4K Ultra HD Action Camera (White)\",\"ExternalId\":null,\"ProductUrl\":\"\",\"ItemType\":\"\",\"Comment\":\"\",\"Size\":\"\",\"Color\":\"\",\"Style\":\"\",\"Image\":{\"ImageName\":\"\",\"Url\":\"\",\"SitecoreId\":\"\",\"AltText\":\"\",\"Width\":0.0,\"Height\":0.0},\"Tags\":[],\"Catalog\":\"\",\"Id\":\"5d29c4184ff24cdcad330334de219d60\",\"Name\":\"\",\"Comments\":\"\",\"Policies\":[],\"ChildComponents\":[]},{\"VariationId\":\"57042121\",\"Id\":\"3615d6d7e627406cbbfc31fe44260a54\",\"Name\":\"\",\"Comments\":\"\",\"Policies\":[],\"ChildComponents\":[]}]}],\"Totals\":{\"SubTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":429.99},\"AdjustmentsTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":3.0},\"GrandTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":460.99},\"PaymentsTotal\":{\"CurrencyCode\":\"USD\",\"Amount\":460.99},\"Name\":\"\",\"Policies\":[]},\"Adjustments\":[{\"AdjustmentType\":\"Discount\",\"Adjustment\":{\"CurrencyCode\":\"USD\",\"Amount\":-3.0},\"AwardingBlock\":\"In Store Discount\",\"IsTaxable\":false,\"IncludeInGrandTotal\":true,\"Name\":\"Discount\",\"DisplayName\":\"\"},{\"AdjustmentType\":\"Tax\",\"Adjustment\":{\"CurrencyCode\":\"USD\",\"Amount\":34.0},\"AwardingBlock\":\"Tax:block:calculatecarttax\",\"IsTaxable\":false,\"IncludeInGrandTotal\":true,\"Name\":\"TaxFee\",\"DisplayName\":\"\"}],\"CompositeKey\":null,\"Components\":[{\"MaskedNumber\":\"3321\",\"ExpiresMonth\":10,\"ExpiresYear\":2020,\"CardType\":\"Visa\",\"PaymentMethodNonce\":\"\",\"TransactionStatus\":\"authorized\",\"TransactionId\":\"ejcg6vky\",\"PaymentInstrumentType\":\"credit_card\",\"BillingParty\":{\"ExternalId\":null,\"AddressName\":null,\"City\":null,\"PhoneNumber\":null,\"Email\":null,\"State\":null,\"StateCode\":null,\"Organization\":null,\"FirstName\":null,\"LastName\":null,\"Country\":null,\"CountryCode\":null,\"Address1\":null,\"Address2\":null,\"ZipPostalCode\":null,\"IsPrimary\":false,\"Name\":\"\",\"Policies\":[]},\"Amount\":{\"CurrencyCode\":\"USD\",\"Amount\":460.99},\"PaymentMethod\":{\"Name\":\"00000000-0000-0000-0000-000000000000\",\"EntityTarget\":\"Card\",\"Policies\":[]},\"Id\":\"71d7ed6db197430c96f9b0a80ae9b81d\",\"Name\":\"Store Payment\",\"Comments\":\"Store Payment\",\"Policies\":[],\"ChildComponents\":[]},{\"ShippingParty\":{\"ExternalId\":\"0\",\"AddressName\":\"Sacramento Store\",\"City\":\"Sacramento\",\"PhoneNumber\":null,\"Email\":null,\"State\":\"CA\",\"StateCode\":\"CA\",\"Organization\":null,\"FirstName\":null,\"LastName\":null,\"Country\":\"US\",\"CountryCode\":\"US\",\"Address1\":\"926 Marshall Dr.\",\"Address2\":null,\"ZipPostalCode\":\"95814\",\"IsPrimary\":false,\"Name\":\"\",\"Policies\":[]},\"Shipments\":[],\"FulfillmentMethod\":{\"Name\":\"Offline Store Order By Customer\",\"EntityTarget\":\"b146622d-dc86-48a3-b72a-05ee8ffd187a\",\"Policies\":[]},\"LineId\":\"\",\"Status\":\"\",\"Id\":\"650b400c62f74b66a7e9e334eb83d350\",\"Name\":\"\",\"Comments\":\"\",\"Policies\":[],\"ChildComponents\":[]},{\"ShopperId\":\"Entity-Customer-7ee8cf19db2b4a32b3a281f048f24fe2\",\"CustomerId\":\"Entity-Customer-7ee8cf19db2b4a32b3a281f048f24fe2\",\"IpAddress\":\"\",\"Latitude\":\"\",\"Longitude\":\"\",\"Currency\":\"\",\"Language\":\"\",\"Email\":\"prasa@gmail.com\",\"IsRegistered\":true,\"Id\":\"5760fdade70e4c34920f16ee794f8af5\",\"Name\":\"\",\"Comments\":\"\",\"Policies\":[],\"ChildComponents\":[]},{\"Memberships\":[\"Orders\",\"AuthenticatedOrders\",\"Orders-ByCustomer-Entity-Customer-7ee8cf19db2b4a32b3a281f048f24fe2\"],\"Id\":\"edb603afc25f4ce28281000525e00ea0\",\"Name\":\"\",\"Comments\":\"\",\"Policies\":[],\"ChildComponents\":[]}],\"DateCreated\":\"2018-03-21T16:25:39.4467398+00:00\",\"DateUpdated\":\"2018-03-21T16:25:39.7038446+00:00\",\"DisplayName\":\"\",\"FriendlyId\":\"Entity-Order-d6d071c1a541456eb8c1d5d28cb135f2\",\"Id\":\"Entity-Order-d6d071c1a541456eb8c1d5d28cb135f2\",\"Version\":1,\"IsPersisted\":true,\"Name\":\"InStoreOrder\",\"Policies\":[]}";

            foreach(var orderDetails in inputJson.Order)
            {
                var email = orderDetails.Email;
                Tracker.Current.Session.IdentifyAs("username", email);

                Order outComeOrder = new Order();
                OrderInputForAuthoring authOrderInput = new OrderInputForAuthoring() { Order = orderDetails };

                try
                {
                    var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
                    var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);

                    Response.Write($" <br /> {startSpanGreen} Submitting to authoring {orderDetails.OrderConfirmationId} {endSpan}");

                    var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(authOrderInput));
                    content.Headers.Remove("Content-Type");
                    content.Headers.Add("Content-Type", "application/json");

                    var client = this.GetClient(ceConfig);
                    var result = client.PostAsync("CreateOfflineOrder", content).Result;
                                        
                    if (result.IsSuccessStatusCode)
                    {
                        Response.Write($" <br /> {startSpanGreen} SUCCESS submitting to authoring {orderDetails.OrderConfirmationId} {endSpan}");
                        var response = result.Content.ReadAsStringAsync().Result;
                        var responseModel = JsonConvert.DeserializeObject<OrderResponse>(response);
                        MapResponseFromCommerceToConnectOrder(responseModel.value, outComeOrder);

                        ServicePipelineArgs args = new ServicePipelineArgs(new SubmitVisitorOrderRequest(new Commerce.Entities.Carts.Cart()), new SubmitVisitorOrderResult() { Order = outComeOrder, Success = true });
                        CorePipeline.Run("commerce.orders.submitOfflineOrder", args);
                    }
                    else
                    {
                        Response.Write($" <br /> {startSpanRed}Failed to submit order to authoring {orderDetails.OrderConfirmationId} {endSpan}");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write($" <br /> {startSpanRed} ERROR: {ex.Message} {DateTime.Now} {endSpan}");
                }

                Tracker.Current.EndVisit(true);
                Response.Write($" <br /> {startSpanGreen} Submitted order to authoring  and XConnect {orderDetails.OrderConfirmationId} {endSpan}");
                Response.Write($" <br /> {startSpanRed}-------------------------- {DateTime.Now} --------------------------------- {endSpan}");
            }

            Response.Write($" <br /> {startSpanRed}Execution Completed at {DateTime.Now} {endSpan}");

        }

        private void MapResponseFromCommerceToConnectOrder(string response, Order outComeOrder)
        {
            var jo = JObject.Parse(response);

            outComeOrder.ShopName = jo["ShopName"]?.ToString();
            outComeOrder.ExternalId = jo["FriendlyId"]?.ToString();
            outComeOrder.UserId = jo["Components"][2]?["CustomerId"]?.ToString();
            outComeOrder.CustomerId = jo["Components"][2]?["CustomerId"]?.ToString();
            outComeOrder.Name = jo["Name"]?.ToString();
            outComeOrder.CurrencyCode = jo["Totals"]?["GrandTotal"]["CurrencyCode"].ToString();// jo["Components"][2]?["Currency"]?.ToString(); // todo Populate it from auth
            outComeOrder.Status = jo["Status"]?.ToString();
            outComeOrder.Email = jo["Components"][2]?["Email"]?.ToString();
            outComeOrder.OrderID = jo["FriendlyId"]?.ToString();
            outComeOrder.OrderDate = System.Convert.ToDateTime(jo["OrderPlacedDate"].ToString());
            outComeOrder.TrackingNumber = jo["OrderConfirmationId"].ToString();
            outComeOrder.IsOfflineOrder = true;

            outComeOrder.Adjustments = new List<CartAdjustment>();
            JArray authAdjustments = (JArray)jo["Adjustments"];

            foreach (var item in authAdjustments)
            {
                var adj = new CartAdjustment();
                adj.Amount = System.Convert.ToDecimal(item["Adjustment"]?["Amount"]?.ToString());
                adj.Description = item["AdjustmentType"]?.ToString();
                outComeOrder.Adjustments.Add(adj);
            }

            outComeOrder.Lines = new List<CartLine>();

            JArray lines = (JArray)jo["Lines"];

            foreach (var lineItem in lines)
            {
                var line = new CartLine();

                line.ExternalCartLineId = lineItem["Id"]?.ToString();
                line.Total = new Sitecore.Commerce.Entities.Prices.Total();
                line.Total.Amount = System.Convert.ToDecimal(lineItem["Totals"]?["GrandTotal"]?["Amount"].ToString());
                line.Total.CurrencyCode = lineItem["Totals"]?["GrandTotal"]?["CurrencyCode"].ToString();

                line.Product = new Sitecore.Commerce.Entities.Carts.CartProduct();
                line.Product.ProductId = lineItem["ItemId"].ToString();
                line.Product.Price = new Sitecore.Commerce.Entities.Prices.Price();
                line.Product.Price.Amount = System.Convert.ToDecimal(lineItem["UnitListPrice"]?["Amount"]?.ToString());
                line.Product.Price.CurrencyCode = lineItem["UnitListPrice"]?["CurrencyCode"]?.ToString();
                line.Product.ProductName = lineItem["ChildComponents"][0]["DisplayName"].ToString();
                line.Quantity = System.Convert.ToDecimal(lineItem["Quantity"]?.ToString());

                outComeOrder.Lines.Add(line);
            }

            outComeOrder.Total = new Sitecore.Commerce.Entities.Prices.Total();
            outComeOrder.Total.Amount = System.Convert.ToDecimal(jo["Totals"]?["GrandTotal"]["Amount"].ToString());
            outComeOrder.Total.CurrencyCode = jo["Totals"]?["GrandTotal"]["CurrencyCode"].ToString();
            outComeOrder.Total.TaxTotal = new Sitecore.Commerce.Entities.Prices.TaxTotal();
            //outComeOrder.Total.TaxTotal.Amount = System.Convert.ToDecimal(jo["Totals"]?["GrandTotal"]["Amount"].ToString());



            var data = jo["Lines"][0]["Totals"]["SubTotal"]["Amount"];
            var data2 = jo["Lines"][0]["ChildComponents"][0]["DisplayName"];
            var data3 = jo["Adjustments"][0];
            var data4 = jo["Components"][2];

        }

        public HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(config.ShopsServiceUrl)
            };

            //Response.Write(config.ShopsServiceUrl);
            httpClient.DefaultRequestHeaders.Add("ShopName", config.DefaultShopName);
            httpClient.DefaultRequestHeaders.Add("Language", "en-US");
            httpClient.DefaultRequestHeaders.Add("Currency", config.DefaultShopCurrency);
            httpClient.DefaultRequestHeaders.Add("Environment", config.DefaultEnvironment);

            string certificate = config.GetCertificate();
            if (certificate != null)
                httpClient.DefaultRequestHeaders.Add(config.CertificateHeaderName, certificate);
            httpClient.Timeout = new System.TimeSpan(0, 0, 600);
            //Response.Write(config.CertificateHeaderName);
            return httpClient;

        }
    }

    public class OrderInput
    {
        public List<ImportOrderModel> Order { get; set; }
    }

    public class OrderInputForAuthoring
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

    public class OrderResponse
    {
        public string value { get; set; }
    }

}