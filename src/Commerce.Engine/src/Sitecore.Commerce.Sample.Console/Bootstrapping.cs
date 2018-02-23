namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;

    using Contexts;
    using FluentAssertions;

    using CommerceOps = CommerceOps.Sitecore.Commerce.Engine;

    public static class Bootstrapping
    {
        private const string Environment = "AdventureWorksShops";

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            var devOp = new DevOpAndre();
            var container = devOp.Context.OpsContainer();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------------------------------------------------");

            // CleanGlobalEnvironment(container);
            // BootstrapEnvironment(container);
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
            Console.WriteLine($"End  >> Clean Environment: {result.ResponseCode}");
            result.ResponseCode.Should().Be("Ok");
        }

        public static void InitializeEnvironment(CommerceOps.Container container, string environmentName)
        {
            Console.WriteLine($"Begin>> Initialize Environment:{environmentName}");
            var result = container.InitializeEnvironment(environmentName).GetValue();
            Console.WriteLine($"End  >> Initialize Environment: {result.ResponseCode}");
            result.ResponseCode.Should().Be("Ok");
        }

        private static void GetPipelines(CommerceOps.Container container)
        {
            var pipelineConfiguration = container.GetPipelines().GetValue();

            string localPath = AppDomain.CurrentDomain.BaseDirectory;

            string pipelineFile =  $"{localPath}/logs/ConfiguredPipelines.log";

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
                    //System.Console.WriteLine($"{pipeline.Namespace}");
                    //System.Console.WriteLine($"{pipeline.Name}({pipeline.Receives}=>{pipeline.Returns})");
                    file.WriteLine($"{pipeline.Namespace}");
                    file.WriteLine($"{pipeline.Name} ({pipeline.Receives} => {pipeline.Returns})");
                    foreach (var block in pipeline.Blocks)
                    {
                        var computedNamespace = block.Namespace.Replace("Sitecore.Commerce.","");
                        //System.Console.WriteLine($"     {block.Name}({block.Receives}=>{block.Returns})");
                        file.WriteLine($"     {computedNamespace}.{block.Name} ({block.Receives} => {block.Returns})");
                    }
                    if (!string.IsNullOrEmpty(pipeline.Comment))
                    {
                        file.WriteLine("     ------------------------------------------------------------");
                        file.WriteLine($"     Comment: {pipeline.Comment}");
                    }
                    //System.Console.WriteLine("-----------------------------------------------------------------");
                    file.WriteLine("-----------------------------------------------------------------");
                }
            }
        }
    }
}
