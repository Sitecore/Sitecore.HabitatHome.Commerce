namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using CommerceOps.Sitecore.Commerce.Core;
    using CommerceOps.Sitecore.Commerce.Plugin.Availability;
    using FluentAssertions;
    using Newtonsoft.Json;

    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Environments
    {
        private static readonly CommerceOps.Sitecore.Commerce.Engine.Container OpsContainer 
            = new DevOpAndre() { Context = { Environment = "GlobalEnvironment" } }.Context.OpsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            var habitat = Proxy.GetValue(OpsContainer.Environments.ByKey("Entity-CommerceEnvironment-HabitatAuthoring").Expand("Components"));          
            habitat.Should().NotBeNull();

            Console.WriteLine("Begin Clone Environment");

            // Export an Environment to use as a template
            var originalEnvironment = ExportEnvironment("AdventureWorksShops");

            var environmentObject = JsonConvert.DeserializeObject(originalEnvironment);

            // Get a dynamic view of the JSON object
            var dynamicEnvironment = (dynamic)environmentObject;

            // Change the Id of the environment in order to import as a new Environment
            var newEnvironmentId = Guid.NewGuid().ToString("N");
            
            dynamicEnvironment.ArtifactStoreId = newEnvironmentId;
            dynamicEnvironment.Name = "ConsoleSample." + newEnvironmentId;

            var newSerializedEnvironment = JsonConvert.SerializeObject(environmentObject);

            // imports the environment into Sitecore Commerce
            var importedEnvironment = ImportEnvironment(newSerializedEnvironment);
            importedEnvironment.EnvironmentId.Should().Be($"Entity-CommerceEnvironment-ConsoleSample.{newEnvironmentId}");
            importedEnvironment.Name.Should().Be($"ConsoleSample.{newEnvironmentId}");

            // Adds a policy
            var policyResult = Proxy.DoOpsCommand(
              OpsContainer.AddPolicy(
                   importedEnvironment.EnvironmentId,
                   "Sitecore.Commerce.Plugin.Availability.GlobalAvailabilityPolicy, Sitecore.Commerce.Plugin.Availability",
                   new GlobalAvailabilityPolicy
                   {
                       AvailabilityExpires = 0
                   },
                   "GlobalEnvironment"));
            policyResult.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            policyResult.Models.OfType<CommerceOps.Sitecore.Commerce.Core.PolicyAddedModel>().Any().Should().BeTrue();

            // Initialize the Environment with default artifacts
            Bootstrapping.InitializeEnvironment(OpsContainer, $"ConsoleSample.{newEnvironmentId}");

            var shopperInNewEnvironmentContainer = new RegisteredCustomerDana
            {
                Context = { Environment = $"ConsoleSample.{newEnvironmentId}" }
            }.Context.ShopsContainer();

            // Get a SellableItem from the environment to assure that we have set it up correctly
            var result = Proxy.GetValue(shopperInNewEnvironmentContainer.SellableItems.ByKey("Adventure Works Catalog,AW055 01,")
                .Expand("Components($expand=ChildComponents($expand=ChildComponents($expand=ChildComponents)))"));
            result.Should().NotBeNull();
            result.Name.Should().Be("Unisex hiking pants");

            // Get the environment to validate change was made
            var updatedEnvironment = Proxy.GetValue(OpsContainer.Environments.ByKey(importedEnvironment.EnvironmentId) );
            var globalAvailabilityPolicy = updatedEnvironment.Policies.OfType<GlobalAvailabilityPolicy>().FirstOrDefault();
            globalAvailabilityPolicy.Should().NotBeNull();
            globalAvailabilityPolicy.AvailabilityExpires.Should().Be(0);

            watch.Stop();

            Console.WriteLine($"End Clone Environments: {watch.ElapsedMilliseconds} ms");
        }

        private static ImportedEnvironmentModel ImportEnvironment(string environmentAsString)
        {
            Console.WriteLine("Begin ImportEnvironment");
           
            var result = Proxy.DoOpsCommand(OpsContainer.ImportEnvironment(environmentAsString));

            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<CommerceOps.Sitecore.Commerce.Core.ImportedEnvironmentModel>().Any().Should().BeTrue();
            result.Models.OfType<CommerceOps.Sitecore.Commerce.Core.ImportedEnvironmentModel>().FirstOrDefault()?.EnvironmentId.Should().NotBeNullOrEmpty();

            return result.Models.OfType<CommerceOps.Sitecore.Commerce.Core.ImportedEnvironmentModel>().First();
        }

        private static string ExportEnvironment(string environmentName)
        {
            Console.WriteLine($"Begin ExportEnvironment ({environmentName})");

            var result = Proxy.GetValue(OpsContainer.ExportEnvironment(environmentName));
            result.Should().NotBeNull();

            Console.WriteLine($"End ExportEnvironment ({environmentName})");

            return result;
        }
    }
}
