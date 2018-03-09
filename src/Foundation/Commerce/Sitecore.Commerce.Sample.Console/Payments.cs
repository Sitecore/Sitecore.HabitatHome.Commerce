namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using FluentAssertions;

    using Sitecore.Commerce.Sample.Contexts;

    public static class Payments
    {
        private static string _cartId;

        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

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
            options.Should().NotBeEmpty();
        }

        private static void GetCartPaymentMethods()
        {
            Console.WriteLine("GetCartPaymentMethods");

            ShopsContainer.GetCartPaymentMethods(_cartId, "Federated").Execute();
        }
    }
}
