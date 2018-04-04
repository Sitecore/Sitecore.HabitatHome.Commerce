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
using Newtonsoft.Json.Linq;


namespace Sitecore.Feature.InventoryUpload.HabitatUtility
{
    public partial class InventoryUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        // It is important to have currency populated at all places.
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var inventory = txtInventoryJson.Text;
            string startSpanRed = "<span style= \"color:red\">";
            string startSpanGreen = "<span style= \"color:green\">";
            string endSpan = "</span>";

            if (string.IsNullOrEmpty(inventory))
            {
                Response.Write("Input is empty");
                return;
            }
            var inputJson = JsonConvert.DeserializeObject<ImportInventoryModel>(inventory);
            Sitecore.Context.SetActiveSite("Storefront");            

           var inventoryDetails= inputJson;
            {
                

                try
                {
                    var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
                    var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);

                    Response.Write($" <br /> {startSpanGreen} Submitting inventory upload to authoring {endSpan}");

                    var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(inventoryDetails));
                    content.Headers.Remove("Content-Type");
                    content.Headers.Add("Content-Type", "application/json");
                   
                    var client = this.GetClient(ceConfig);
                    
                    var result = client.PostAsync("CreateStoreInventory", content).Result;
                                        
                    if (result.IsSuccessStatusCode)
                    {
                        Response.Write($" <br /> {startSpanGreen} SUCCESS submitting inventory to authoring {endSpan}");
                        var response = result.Content.ReadAsStringAsync().Result;
                        var responseModel = JsonConvert.DeserializeObject<InventoryResponse>(response);

                    }
                    else
                    {
                        Response.Write($" <br /> {startSpanRed}Failed to submit Inventory to authoring  {endSpan}");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write($" <br /> {startSpanRed} ERROR: {ex.Message} {DateTime.Now} {endSpan}");
                }

                Tracker.Current.EndVisit(true);
                Response.Write($" <br /> {startSpanGreen} Processing Completed {endSpan}");
                Response.Write($" <br /> {startSpanRed}-------------------------- {DateTime.Now} --------------------------------- {endSpan}");
            }

            Response.Write($" <br /> {startSpanRed}Execution Completed at {DateTime.Now} {endSpan}");

        }

   
        public HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(config.ShopsServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Add("ShopName", config.DefaultShopName);
            httpClient.DefaultRequestHeaders.Add("Language", Sitecore.Context.Language.Name);
            httpClient.DefaultRequestHeaders.Add("Currency", config.DefaultShopCurrency);
            httpClient.DefaultRequestHeaders.Add("Environment", config.DefaultEnvironment);

            var certificate = config.GetCertificate();
            if (certificate != null)
                httpClient.DefaultRequestHeaders.Add(config.CertificateHeaderName, certificate);
            httpClient.Timeout = new System.TimeSpan(0, 0, 1500);
            return httpClient;

        }
    }

    
    public class ImportInventoryModel
    {        
        public List<string> ProductToAssociate { get; set; }
        public string Catalog { get; set; }
        
        public List<Store> Stores { get; set; }
    }

    public class Store
    {
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Abbreviation { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class InventoryResponse
    {
        public string value { get; set; }
    }

}