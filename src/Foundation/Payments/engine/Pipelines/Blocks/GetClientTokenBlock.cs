// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetClientTokenBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Foundation.Payments.Engine.Pipelines.Blocks
{
    /// <summary>
    ///  Defines a block which gets a payment service client tokent.
    /// </summary>
    /// <seealso>
    /// <cref>
    ///  Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///  </cref>
    ///  </seealso>
    [PipelineDisplayName(PaymentsMoneybagsConstants.Pipelines.Blocks.GetClientTokenBlock)]
    public class GetClientTokenBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A client token string</returns>        
        public override Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            //returning a fake client token so we can reuse the Braintree-dependent client-side code
            return Task.FromResult("eyJ2ZXJzaW9uIjoyLCJhdXRob3JpemF0aW9uRmluZ2VycHJpbnQiOiI2MzM1M2U0YTcxMDVjNWE4ZTIwOWQ1NGUxZWQ1NTI1MjQwMWVkZjM5MWY0MDYyNTU3NDZlMTZhYzMyNzZiOTk5fGNyZWF0ZWRfYXQ9MjAxOC0wMi0yMlQyMjo0NDoxOC40ODU1MjU2MzUrMDAwMFx1MDAyNm1lcmNoYW50X2lkPXQ0emZieXFuOWZxN3FtYzlcdTAwMjZwdWJsaWNfa2V5PTR0Z253bTRmanpka2J4NHMiLCJjb25maWdVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvdDR6ZmJ5cW45ZnE3cW1jOS9jbGllbnRfYXBpL3YxL2NvbmZpZ3VyYXRpb24iLCJjaGFsbGVuZ2VzIjpbXSwiZW52aXJvbm1lbnQiOiJzYW5kYm94IiwiY2xpZW50QXBpVXJsIjoiaHR0cHM6Ly9hcGkuc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbTo0NDMvbWVyY2hhbnRzL3Q0emZieXFuOWZxN3FtYzkvY2xpZW50X2FwaSIsImFzc2V0c1VybCI6Imh0dHBzOi8vYXNzZXRzLmJyYWludHJlZWdhdGV3YXkuY29tIiwiYXV0aFVybCI6Imh0dHBzOi8vYXV0aC52ZW5tby5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tIiwiYW5hbHl0aWNzIjp7InVybCI6Imh0dHBzOi8vY2xpZW50LWFuYWx5dGljcy5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tL3Q0emZieXFuOWZxN3FtYzkifSwidGhyZWVEU2VjdXJlRW5hYmxlZCI6dHJ1ZSwicGF5cGFsRW5hYmxlZCI6dHJ1ZSwicGF5cGFsIjp7ImRpc3BsYXlOYW1lIjoiSGVkZ2Vob2ciLCJjbGllbnRJZCI6bnVsbCwicHJpdmFjeVVybCI6Imh0dHA6Ly9leGFtcGxlLmNvbS9wcCIsInVzZXJBZ3JlZW1lbnRVcmwiOiJodHRwOi8vZXhhbXBsZS5jb20vdG9zIiwiYmFzZVVybCI6Imh0dHBzOi8vYXNzZXRzLmJyYWludHJlZWdhdGV3YXkuY29tIiwiYXNzZXRzVXJsIjoiaHR0cHM6Ly9jaGVja291dC5wYXlwYWwuY29tIiwiZGlyZWN0QmFzZVVybCI6bnVsbCwiYWxsb3dIdHRwIjp0cnVlLCJlbnZpcm9ubWVudE5vTmV0d29yayI6dHJ1ZSwiZW52aXJvbm1lbnQiOiJvZmZsaW5lIiwidW52ZXR0ZWRNZXJjaGFudCI6ZmFsc2UsImJyYWludHJlZUNsaWVudElkIjoibWFzdGVyY2xpZW50MyIsImJpbGxpbmdBZ3JlZW1lbnRzRW5hYmxlZCI6dHJ1ZSwibWVyY2hhbnRBY2NvdW50SWQiOiJoZWRnZWhvZyIsImN1cnJlbmN5SXNvQ29kZSI6IlVTRCJ9LCJtZXJjaGFudElkIjoidDR6ZmJ5cW45ZnE3cW1jOSIsInZlbm1vIjoib2ZmIn0=");
        }
    }
}
