namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Fulfillments
    {
        private static string _cartId;
        private static string _cartLineId;

        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Begin Fulfillments");

            _cartId = Guid.NewGuid().ToString("B");
            _cartLineId = Carts.AddCartLineWithVariant(_cartId);

            GetFulfillmentMethods();
            GetCartFulfillmentOptions();
            GetCartFulfillmentMethods();
            GetCartLineFulfillmentOptions();
            GetCartLineFulfillmentMethods();

            Carts.DeleteCart(_cartId);

            watch.Stop();

            Console.WriteLine($"End Fulfillments :{watch.ElapsedMilliseconds} ms");
        }

        private static void GetCartFulfillmentOptions()
        {
            Console.WriteLine("GetCartFulfillmentOptions");

            var options = Proxy.Execute(ShopsContainer.GetCartFulfillmentOptions(_cartId));
            options.Should().NotBeEmpty();
        }

        private static void GetCartLineFulfillmentOptions()
        {
            Console.WriteLine("GetCartLineFulfillmentOptions");

            var options = Proxy.Execute(ShopsContainer.GetCartLineFulfillmentOptions(_cartId, _cartLineId));
            options.Should().NotBeEmpty();
        }

        private static void GetFulfillmentMethods()
        {
            Console.WriteLine("GetFulfillmentMethods");

            var methods = Proxy.Execute(ShopsContainer.GetFulfillmentMethods());
            methods.Should().NotBeEmpty();
        }

        private static void GetCartFulfillmentMethods()
        {
            Console.WriteLine("GetCartFulfillmentMethods");

            var methods = Proxy.Execute(ShopsContainer.GetCartFulfillmentMethods(
                _cartId,
                new PhysicalFulfillmentComponent
                {
                    ShippingParty = new Party
                    {
                        FirstName = "first name",
                        LastName = "last name",
                        AddressName = "name",
                        Address1 = "line 1",
                        City = "city",
                        State = "Ontario",
                        StateCode = "ON",
                        Country = "Canada",
                        CountryCode = "CA",
                        ZipPostalCode = "postalcode"
                    }
                }));

            methods.Should().NotBeEmpty();
        }

        private static void GetCartLineFulfillmentMethods()
        {
            Console.WriteLine("GetCartLineFulfillmentMethods");

            var methods = Proxy.Execute(ShopsContainer.GetCartLineFulfillmentMethods(
                _cartId,
                new PhysicalFulfillmentComponent
                {
                    ShippingParty = new Party
                    {
                        FirstName = "first name",
                        LastName = "last name",
                        AddressName = "name",
                        Address1 = "line 1",
                        City = "city",
                        State = "Ontario",
                        StateCode = "ON",
                        Country = "Canada",
                        CountryCode = "CA",
                        ZipPostalCode = "postalcode"
                    },
                    LineId = _cartLineId
                }));

            methods.Should().NotBeEmpty();
        }
    }
}
