﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine
{
    /// <summary>
    /// Defines a block which configures the OData model
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("SamplePluginConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            // Add the entities
            //modelBuilder.AddEntityType(typeof(SampleEntity));

            // Add the entity sets
            //modelBuilder.EntitySet<SampleEntity>("Sample");

            // Add complex types

            // Add unbound functions

            // Add unbound actions
            //var configuration = modelBuilder.Action("SampleCommand");
            //configuration.Parameter<string>("Id");
            //configuration.ReturnsFromEntitySet<CommerceCommand>("Commands");


            ActionConfiguration actionConfiguration2 = modelBuilder.Action("AddWishListLineItem");            
            actionConfiguration2.Parameter<string>("wishlistId");            
            actionConfiguration2.Parameter<string>("itemId");            
            actionConfiguration2.Parameter<System.Decimal>("quantity");
            //string entitySetName2 = "Commands";
            actionConfiguration2.ReturnsFromEntitySet<CommerceCommand>("Commands");

            ActionConfiguration actionConfiguration3 = modelBuilder.Action("RemoveWishListLineItem");            
            actionConfiguration3.Parameter<string>("wishlistId");            
            actionConfiguration3.Parameter<string>("cartLineId");            
            actionConfiguration3.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}
