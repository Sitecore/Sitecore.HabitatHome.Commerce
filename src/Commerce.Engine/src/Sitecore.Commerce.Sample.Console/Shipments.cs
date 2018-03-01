namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Shipments
    {
        private static string _shipmentFriendlyId;

        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerSteve().Context.ShopsContainer();

        public  static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Shipments");

            GetShipments();
            GetShipment();

            watch.Stop();

            Console.WriteLine($"End Shipments :{watch.ElapsedMilliseconds} ms");
        }
        
        private static void GetShipments()
        {
            Console.WriteLine("Begin GetShipments");

            var result = Proxy.Execute(ShopsContainer.Shipments);
            var shipments = result as IList<Shipment> ?? result.ToList();
            shipments.Should().NotBeNull();
            shipments.Should().NotBeEmpty();
            _shipmentFriendlyId = shipments.FirstOrDefault()?.FriendlyId;
        }

        private static void GetShipment()
        {
            Console.WriteLine("Begin GetShipment");

            var result = Proxy.GetValue(ShopsContainer.Shipments.ByKey(_shipmentFriendlyId).Expand("Lines($expand=CartLineComponents)"));
            result.Should().NotBeNull();
            result.OrderId.Should().NotBeNullOrEmpty();
            result.Lines.Should().NotBeNullOrEmpty();
            result.ShipParty.Should().NotBeNull();
            result.Charge.Should().NotBeNull();
            result.Charge.Amount.Should().NotBe(0);
        }
    }
}
