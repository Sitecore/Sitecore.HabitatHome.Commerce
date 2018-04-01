namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using FluentAssertions;

    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Categories
    {
        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Categories");

            GetCategory();
            GetCategories();

            watch.Stop();

            Console.WriteLine($"End Categories :{watch.ElapsedMilliseconds} ms");
        }
        
        private static void GetCategory()
        {
            Console.WriteLine("Begin GetCategory");

            var result = Proxy.GetValue(ShopsContainer.Categories.ByKey("Adventure Works Catalog-Backpacks"));
            result.Should().NotBeNull();
            result.Name.Should().Be("Backpacks");
            result.Description.Should().Be("Catalog Section for Backpack");
        }
        
        private static void GetCategories()
        {
            Console.WriteLine("Begin GetCategories");

            var result = Proxy.Execute(ShopsContainer.Categories);
            result.Should().NotBeNull();
        }
    }
}
