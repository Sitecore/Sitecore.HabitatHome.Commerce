namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Rules
    {
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Begin Rules");

            GetConditions();
            GetRuntimeSessionConditions();
            GetDateConditions();
            GetActions();
            GetOperators();

            watch.Stop();

            Console.WriteLine($"End Rules :{watch.ElapsedMilliseconds} ms");
        }

        private static void GetConditions()
        {
            Console.WriteLine("Begin GetConditions");

            var result = Proxy.Execute(ShopsContainer.GetConditions(string.Empty));
            result.Should().NotBeNull();
            result.Any().Should().BeTrue();
        }

        private static void GetRuntimeSessionConditions()
        {
            Console.WriteLine("Begin GetRuntimeSessionConditions");

            var result = Proxy.Execute(ShopsContainer.GetConditions("Sitecore.Commerce.Plugin.Rules.IRuntimeSessionCondition, Sitecore.Commerce.Plugin.Rules"));
            result.Should().NotBeNull();
            result.Any().Should().BeTrue();
        }

        private static void GetDateConditions()
        {
            Console.WriteLine("Begin GetDateConditions");

            var result = Proxy.Execute(ShopsContainer.GetConditions("Sitecore.Commerce.Plugin.Rules.IDateCondition, Sitecore.Commerce.Plugin.Rules"));
            result.Should().NotBeNull();
            result.Any().Should().BeTrue();
        }

        private static void GetActions()
        {
            Console.WriteLine("Begin GetActions");

            var result = Proxy.Execute(ShopsContainer.GetActions(string.Empty));
            result.Should().NotBeNull();
            result.Any().Should().BeTrue();
        }

        private static void GetOperators()
        {
            Console.WriteLine("Begin GetOperators");

            var result = Proxy.Execute(ShopsContainer.GetOperators(string.Empty));
            result.Should().NotBeNull();
            result.Any().Should().BeTrue();
        }
    }
}
