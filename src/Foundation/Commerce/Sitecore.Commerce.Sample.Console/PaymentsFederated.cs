namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Sample.Contexts;

    public static class PaymentsFederated
    {
        private static string _cartId;
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerBob().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Payments");

            _cartId = Guid.NewGuid().ToString("B");
            Carts.AddCartLineWithVariant(_cartId);

            GetCartPaymentOptions();
            GetCartPaymentMethods();          

            Carts.DeleteCart(_cartId);

            watch.Stop();

            Console.WriteLine($"End Payments :{watch.ElapsedMilliseconds} ms");
        }

        private static void GetCartPaymentOptions()
        {
            Console.WriteLine("GetCartPaymentOptions");

            var options = ShopsContainer.GetCartPaymentOptions(_cartId).Execute();

            foreach (PaymentOption option in options)
            {
                Console.WriteLine($"Payment option: {option.DisplayName}");
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static void GetCartPaymentMethods()
        {
            Console.WriteLine("GetCartPaymentMethods");

            var methods = ShopsContainer.GetCartPaymentMethods(_cartId, "Federated").Execute();

            foreach (PaymentMethod method in methods)
            {
                Console.WriteLine($"Payment method: {method.DisplayName}");
            }

            Console.WriteLine();
        }
    }
}
