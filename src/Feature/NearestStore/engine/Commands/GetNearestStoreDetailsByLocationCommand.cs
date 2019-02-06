using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Entities;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Commands
{
    public class GetNearestStoreDetailsByLocationCommand : CommerceCommand
    {

        private readonly IGetNearestStoreDetailsByLocationPipeline _getNearestStoreDetailsByLocationPipeline
;
        public GetNearestStoreDetailsByLocationCommand(IGetNearestStoreDetailsByLocationPipeline getNearestStoreDetailsByLocationPipeline, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            this._getNearestStoreDetailsByLocationPipeline = getNearestStoreDetailsByLocationPipeline;           
        }

        public async Task<List<NearestStoreLocation>> Process(CommerceContext commerceContext, GetNearestStoreDetailsByLocationArgument inputArgumentList)
        {
            GetNearestStoreDetailsByLocationCommand getNearestStoreDetailsByLocationCommand = this;
 
            List<NearestStoreLocation> sets = new List<NearestStoreLocation>();
            using (CommandActivity.Start(commerceContext, getNearestStoreDetailsByLocationCommand))
            {
                sets = await getNearestStoreDetailsByLocationCommand._getNearestStoreDetailsByLocationPipeline.Run(inputArgumentList, commerceContext.PipelineContextOptions).ConfigureAwait(false);                     
            }

            return sets;
        }
    }
}
