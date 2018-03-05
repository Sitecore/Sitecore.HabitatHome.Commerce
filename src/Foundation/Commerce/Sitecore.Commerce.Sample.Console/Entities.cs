namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using Contexts;
    using FluentAssertions;
    using Microsoft.OData.Client;

    using Sitecore.Commerce.ServiceProxy;

    public static class Entities
    {

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Entities");

            GetRawEntity();
            GetEntityView();

            watch.Stop();

            Console.WriteLine($"End Entities: {watch.ElapsedMilliseconds} ms");
        }

        private static void GetRawEntity()
        {
            Console.WriteLine("Begin GetRawEntity");

            var devOps = new DevOpAndre();
            var container = devOps.Context.OpsContainer();
            Action action = null;
            try
            {
                action = () => Proxy.GetValue(container.GetRawEntity("invalidEntityId", "HabitatShops"));
            }
            catch (DataServiceClientException)
            {
                action?.ShouldThrow<DataServiceClientException>();
            }

            var result = Proxy.GetValue(container.GetRawEntity("Entity-SellableItem-AW007 08", "AdventureWorksShops"));
            result.Should().NotBeNull();

            try
            {
                action = () => Proxy.GetValue(container.GetRawEntity($"Environments/{devOps.Context.Environment}", "HabitatShops"));
            }
            catch (DataServiceClientException)
            {
                action?.ShouldThrow<DataServiceClientException>();
            }
        }

        private static void GetEntityView()
        {
            Console.WriteLine("Begin GetEntityView");

            var csrSheila = new CsrSheila();
            var container = csrSheila.Context.ShopsContainer();

            Action action = null;
            try
            {
                action = () => Proxy.GetValue(container.GetEntityView("fakeentityid", "Master", string.Empty, string.Empty));
            }
            catch (DataServiceQueryException)
            {
                action?.ShouldThrow<DataServiceQueryException>();
            }

            try
            {
                action = () => Proxy.GetValue(container.GetEntityView(null, "Master", string.Empty, string.Empty));
            }
            catch (DataServiceQueryException)
            {
                action?.ShouldThrow<DataServiceQueryException>();
            }
        }
    }
}
