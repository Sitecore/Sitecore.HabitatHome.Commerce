namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Sitecore.Commerce.Engine;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Inventory
    {
        private static readonly Container AwShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();
        private static readonly Container HabitatShopsContainer = new AnonymousCustomerSteve().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Inventory");

            VerifyInventorySet(AwShopsContainer, "Adventure Works Inventory");
            VerifyInventorySet(HabitatShopsContainer, "Habitat_Inventory");

            watch.Stop();

            Console.WriteLine($"End Inventory :{watch.ElapsedMilliseconds} ms");
        }

        private static void VerifyInventorySet(Container container, string expectedName)
        {
            Console.WriteLine("Begin GetInventorySet");

            var result = Proxy.Execute(container.InventorySets).ToList();

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterOrEqualTo(1);
            result.Any(x => x.Name.Equals(expectedName)).Should().BeTrue();
        }
    }
}
