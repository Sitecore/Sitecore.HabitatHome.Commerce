namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using FluentAssertions;

    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Catalogs
    {
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Catalogs");

            GetCatalog();
            GetCatalogs();

            watch.Stop();

            Console.WriteLine($"End Catalogs :{watch.ElapsedMilliseconds} ms");
        }
        
        private static void GetCatalog()
        {
            Console.WriteLine("Begin GetCatalog");

            var result = Proxy.GetValue(ShopsContainer.Catalogs.ByKey("Adventure Works Catalog"));
            result.Should().NotBeNull();
        }
        
        private static void GetCatalogs()
        {
            Console.WriteLine("Begin GetCatalogs");

            var result = Proxy.Execute(ShopsContainer.Catalogs);
            result.Should().NotBeNull();
        }
    }
}
