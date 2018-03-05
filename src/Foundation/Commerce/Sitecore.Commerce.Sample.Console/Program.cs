using Sitecore.Commerce.Sample.Console.Authentication;

namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    public class Program
    {
        // Should the environment be bootstrapped when this program runs?
        private static bool ShouldBootstrapOnLoad = true;
        private static bool ShouldRunPricingScenarios = true;
        private static bool DemoStops = true;
        
        public static string CurrentEnvironment = "AdventureWorksShops";
        public static string DefaultStorefront = "CommerceEngineDefaultStorefront";

        public static string OpsServiceUri = "https://localhost:5000/CommerceOps/";
        public static string ShopsServiceUri = "https://localhost:5000/api/";
        public static string MinionsServiceUri = "https://localhost:5000/CommerceOps/";
        public static string AuthoringServiceUri = "https://localhost:5000/api/";
        public static string SitecoreIdServerUri = "https://localhost:5050/";

        public static string UserName = @"sitecore\admin";
        public static string Password = "b";

        public static string SitecoreToken;

        static void Main(string[] args)
        {

            OpsServiceUri = Properties.Settings.Default.OpsServiceUri;
            ShopsServiceUri = Properties.Settings.Default.ShopsServiceUri;
            MinionsServiceUri = Properties.Settings.Default.MinionsServiceUri;
            AuthoringServiceUri = Properties.Settings.Default.AuthoringServiceUri;
            SitecoreIdServerUri = Properties.Settings.Default.SitecoreIdServerUri;

            UserName = Properties.Settings.Default.UserName;
            Password = Properties.Settings.Default.Password;

            SitecoreToken = SitecoreIdServerAuth.GetToken();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.ForegroundColor = ConsoleColor.Green;

            if (ShouldBootstrapOnLoad)
            {
                //// Bootstrap the environment
                Bootstrapping.RunScenarios();
            }

            //// Environment 
            Environments.RunScenarios();

            //// Plugins
            Plugins.RunScenarios();

            Catalogs.RunScenarios();
            CatalogsUX.RunScenarios();

            Categories.RunScenarios();
            CategoriesUX.RunScenarios();

            if (ShouldRunPricingScenarios)
            {
                //// Pricing 
                Pricing.RunScenarios();
                PricingUX.RunScenarios();
            }

            //// Promotions
            Promotions.RunScenarios();
            PromotionsUX.RunScenarios();
            PromotionsRuntime.RunScenarios();

            //// Rules
            Rules.RunScenarios();

            //// Sellable items
            SellableItems.RunScenarios();

            //// Inventory
            Inventory.RunScenarios();
            InventoryUX.RunScenarios();

            //// Entities
            Entities.RunScenarios();

            //// Coupons
            Coupons.RunScenarios();
            CouponsUX.RunScenarios();

            //// Policies
            Policies.RunScenarios();

            //// Fulfillment
            Fulfillments.RunScenarios();

            //// Payments
            Payments.RunScenarios();
            PaymentsFederated.RunScenarios();

            //// Carts
            Carts.RunScenarios();

            //// Returns
            Returns.RunScenarios();

            //// Orders
            OrdersUX.RunScenarios();
            Orders.RunScenarios();

            //// Shipments
            Shipments.RunScenarios(); // ORDERS HAVE TO BE RELEASED FOR SHIPMENTS TO GET GENERATED

            //// Customers
            CustomersUX.RunScenarios();

            //// Entitlements
            Entitlements.RunScenarios();

            //// Caching
            Caching.RunScenarios();

            //Search.RunScenarios();

            stopwatch.Stop();

            Console.WriteLine($"Test Runs Complete - {stopwatch.ElapsedMilliseconds} ms -  (Hit any key to continue)");

            if (DemoStops)
            {
                Console.ReadKey();
            }
        }
    }
}