namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Caching
    {
        private static CommerceOps.Sitecore.Commerce.Engine.Container Container = new DevOpAndre().Context.OpsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Caching");
            
            GetStoreCaches();
            GetStoreCache();
            ClearCacheStoreCache();
            ClearCacheStore();
            RequestCacheReset();

            watch.Stop();

            Console.WriteLine($"End Caching:{watch.ElapsedMilliseconds} ms");
        }

        private static void ClearCacheStore()
        {
            Console.WriteLine("Begin ClearCacheStore");
            
            var result = Proxy.GetValue(Container.ClearCacheStore("AdventureWorksShops", "AdventureWorksShops"));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void ClearCacheStoreCache()
        {
            Console.WriteLine("Begin ClearCacheStoreCache");
            
            var result = Proxy.GetValue(Container.ClearCacheStoreCache("AdventureWorksShops", "AdventureWorksShops.Items", "AdventureWorksShops"));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void RequestCacheReset()
        {
            Console.WriteLine("Begin RequestCacheReset");

            var result = Proxy.GetValue(Container.RequestCacheReset("AdventureWorksShops", "AdventureWorksShops.Items", null));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void GetStoreCaches()
        {
            Console.WriteLine("Begin GetStoreCaches");

            var result = Proxy.Execute(Container.GetCacheStores()).ToList();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
        
        private static void GetStoreCache()
        {
            Console.WriteLine("Begin GetStoreCache");

            var result = Proxy.GetValue(Container.GetCacheStore(Program.CurrentEnvironment));
            result.Should().NotBeNull();
            result.Caches.Should().NotBeEmpty();
        }
    }
}
