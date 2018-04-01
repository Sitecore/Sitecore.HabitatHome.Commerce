namespace Sitecore.Commerce.Sample.Contexts
{
    using System;
    using System.Collections.Generic;

    public class MinionRunner
    {
        public MinionRunner()
        {
            this.Context = new ShopperContext
            {
                Shop = Sample.Console.Program.DefaultStorefront,
                ShopperId = "MinionRunnerShopperId",
                Language = "en-US",
                Currency = "USD",
                PolicyKeys = "ZeroMinionDelay|xActivityPerf",
                EffectiveDate = DateTimeOffset.Now,
                Components = new List<Core.Component>()
            };
        }

        public ShopperContext Context { get; set; }
    }
}
