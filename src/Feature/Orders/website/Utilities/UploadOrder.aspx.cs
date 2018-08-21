using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Analytics;
using System.Net.Http;
using Sitecore.Commerce.Entities.Orders;
using Newtonsoft.Json.Linq;
using Sitecore.Pipelines;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Orders;
using Sitecore.Commerce.Entities.Carts;

namespace Sitecore.HabitatHome.Feature.Orders.Utilities
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
                        Response.Write($" <br /> {startSpanGreen} Submitting to XConnect {orderDetails.OrderConfirmationId} {endSpan}");
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
                Response.Write($" <br /> {startSpanGreen} Processing Completed {orderDetails.OrderConfirmationId} {endSpan}");
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
           



           

        }

        public HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(config.ShopsServiceUrl)
            };

            //Response.Write(config.ShopsServiceUrl);
            httpClient.DefaultRequestHeaders.Add("ShopName", config.DefaultShopName);
            httpClient.DefaultRequestHeaders.Add("Language", Sitecore.Context.Language.Name);
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
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class OrderResponse
    {
        public string value { get; set; }
    }

}