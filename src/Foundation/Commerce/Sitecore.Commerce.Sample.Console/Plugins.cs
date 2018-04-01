namespace Sitecore.Commerce.Sample.Console
{
    using System.Diagnostics;

    using FluentAssertions;

    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Plugins
    {
        private static CommerceOps.Sitecore.Commerce.Engine.Container OpsContainer = new DevOpAndre().Context.OpsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Begin Entities");

            RunningPlugins();

            watch.Stop();

            System.Console.WriteLine($"End Entities: {watch.ElapsedMilliseconds} ms");
        }

        private static void RunningPlugins()
        {
            System.Console.WriteLine("Begin GetRawEntity");

            var result = Proxy.Execute(OpsContainer.RunningPlugins());
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
    }
}
