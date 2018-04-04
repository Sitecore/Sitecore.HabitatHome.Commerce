// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProfileDefinitionBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;   
    using Sitecore.Framework.Pipelines;
    using Sitecore.Framework.Caching;

    /// <summary>
    /// Defines a block which gets a Profile definition.
    /// </summary>
    /// <seealso>
    ///    <cref>
    ///          Sitecore.Framework.Pipelines.PipelineBlock{System.String, 
    ///          System.Collections.Generic.IEnumerable{Plugin.Sample.Customers.CsMigration.ProfileDefinition}, 
    ///          Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///    </cref>
    /// </seealso> 
    [PipelineDisplayName(CustomersCsConstants.Pipelines.Blocks.GetProfileDefinitionBlock)]
    public class GetProfileDefinitionBlock : PipelineBlock<string, IEnumerable<ProfileDefinition>, CommercePipelineExecutionContext>
    {
        private readonly IGetEnvironmentCachePipeline _cachePipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetProfileDefinitionBlock"/> class.
        /// </summary>
        /// <param name="cachePipeline">The cache pipeline.</param>
        public GetProfileDefinitionBlock(IGetEnvironmentCachePipeline cachePipeline)
        {
            this._cachePipeline = cachePipeline;            
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The collection of profile definitions <see cref="ProfileDefinition" />.
        /// </returns>
        public override async Task<IEnumerable<ProfileDefinition>> Run(string arg, CommercePipelineExecutionContext context)
        {
            var cachePolicy = context.GetPolicy<ProfilesCsCachePolicy>();
            var cacheKey = string.IsNullOrEmpty(arg) ? "ProfileDefinition.All" : $"{arg}";
            ICache cache = null;

            if (cachePolicy.AllowCaching)
            {
                cache = await this._cachePipeline.Run(new EnvironmentCacheArgument { CacheName = cachePolicy.CacheName }, context);
                var item = await cache.Get(cacheKey) as List<ProfileDefinition>;
                if (item != null)
                {
                    return item;
                }
            }

            try
            {
                var schema = new List<ProfileDefinition>();
                var sqlContext = ConnectionHelper.GetProfilesSqlContext(context.CommerceContext);
                if (string.IsNullOrEmpty(arg))
                {
                    schema = await sqlContext.GetAllProfileDefinitions();
                    if (schema != null && schema.Count > 0)
                    {
                        if (cachePolicy.AllowCaching && cache != null)
                        {
                            await cache.Set(cacheKey, new Cachable<List<ProfileDefinition>>(schema, 1), cachePolicy.GetCacheEntryOptions());
                        }

                        context.CommerceContext.AddUniqueObjectByType(schema);
                        return schema;
                    }                    
                }
                else
                {
                    var profileDefinition = await sqlContext.GetProfileDefinition(arg);
                    if (profileDefinition != null)
                    {
                        schema.Add(profileDefinition);
                        if (cachePolicy.AllowCaching && cache != null)
                        {
                            await cache.Set(cacheKey, new Cachable<List<ProfileDefinition>>(schema, 1), cachePolicy.GetCacheEntryOptions());
                        }

                        return schema;
                    }
                }

                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { arg },
                        $"Entity {arg} was not found.");
                return null;
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { arg, ex },
                        $"Entity {arg} was not found.");
                return null;
            } 
        }
    }
}
