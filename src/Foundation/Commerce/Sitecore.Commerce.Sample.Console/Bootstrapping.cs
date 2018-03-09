
namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Contexts;
    using FluentAssertions;

    using CommerceOps = CommerceOps.Sitecore.Commerce.Engine;
    using CommerceOps.Sitecore.Commerce.Core.Commands;
    
    public static class Bootstrapping
    {
        private const string Environment = "AdventureWorksShops";

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            var devOp = new DevOpAndre();
            var container = devOp.Context.OpsContainer();

            Console.WriteLine("---------------------------------------------------");
            
            CleanEnvironment(container, Environment);
            CleanEnvironment(container, "HabitatShops");
            InitializeEnvironment(container, Environment);
            InitializeEnvironment(container, "HabitatShops");

            Console.WriteLine("---------------------------------------------------");

            GetPipelines(container);

            Console.WriteLine("---------------------------------------------------");

            watch.Stop();

            Console.WriteLine($"End Bootstrapping: {watch.ElapsedMilliseconds} ms");
        }

        private static void CleanEnvironment(CommerceOps.Container container, string environment)
        {
            Console.WriteLine($"Begin>> Clean Environment: {environment}");
            var result = container.CleanEnvironment(environment).GetValue();
            Console.WriteLine($"End>> Clean Environment: {result.ResponseCode}");
            result.ResponseCode.Should().Be("Ok");
        }

        public static void InitializeEnvironment(CommerceOps.Container container, string environmentName)
        {
            Console.WriteLine($"Begin>> Initialize Environment:{environmentName}");
            var result = container.InitializeEnvironment(environmentName).GetValue();
            result.ResponseCode.Should().Be("Ok");

            var longRunningCommand = result;
            longRunningCommand.Should().NotBeNull();
            var waitingTime = new Stopwatch();
            waitingTime.Start();
            while (!longRunningCommand.Status.Equals("RanToCompletion") && waitingTime.Elapsed <= TimeSpan.FromMinutes(10))
            {
                Thread.Sleep(60000);
                longRunningCommand = container.CheckCommandStatus(longRunningCommand.TaskId).GetValue();
                longRunningCommand.Should().NotBeNull();
            }

            waitingTime.Stop();
            waitingTime.Elapsed.Should().BeLessOrEqualTo(TimeSpan.FromMinutes(10));
            longRunningCommand.ResponseCode.Should().Be("Ok");
            Console.WriteLine($"End>> Initialize Environment: {longRunningCommand.ResponseCode}");
        }

        private static void GetPipelines(CommerceOps.Container container)
        {
            var pipelineConfiguration = container.GetPipelines().GetValue();

            string localPath = AppDomain.CurrentDomain.BaseDirectory;

            string pipelineFile = $"{localPath}/logs/ConfiguredPipelines.log";

            if (!System.IO.Directory.Exists($"{localPath}/logs"))
            {
                System.IO.Directory.CreateDirectory($"{localPath}/logs");
            }

            if (System.IO.File.Exists(pipelineFile))
            {
                System.IO.File.Delete(pipelineFile);
            }

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(pipelineFile))
            {
                file.WriteLine("Current Pipeline Configuration");
                file.WriteLine("-----------------------------------------------------------------");
                foreach (var pipeline in pipelineConfiguration.List)
                {
                    file.WriteLine($"{pipeline.Namespace}");
                    file.WriteLine($"{pipeline.Name} ({pipeline.Receives} => {pipeline.Returns})");
                    foreach (var block in pipeline.Blocks)
                    {
                        var computedNamespace = block.Namespace.Replace("Sitecore.Commerce.","");
                        file.WriteLine($"     {computedNamespace}.{block.Name} ({block.Receives} => {block.Returns})");
                    }

                    if (!string.IsNullOrEmpty(pipeline.Comment))
                    {
                        file.WriteLine("     ------------------------------------------------------------");
                        file.WriteLine($"     Comment: {pipeline.Comment}");
                    }

                    file.WriteLine("-----------------------------------------------------------------");
                }
            }
        }
    }
}
