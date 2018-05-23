// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatchEnvironmentJsonBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the patch environment Json block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.ConditionalPipelineBlock{Sitecore.Commerce.Core.FindEntityArgument, Sitecore.Commerce.Core.FindEntityArgument,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>   
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.PatchEnvironmentJsonBlock)]
    public class PatchEnvironmentJsonBlock : PipelineBlock<Sitecore.Commerce.Core.FindEntityArgument, Sitecore.Commerce.Core.FindEntityArgument, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A string</returns>
        public override async Task<FindEntityArgument> Run(FindEntityArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The FindEntityArgument argument cannot be null or empty.");
            if (arg.EntityType != typeof(CommerceEnvironment))
            {
                return arg;
            }

            try
            {
                if (arg.SerializedEntity != null)
                {
                    arg.SerializedEntity = await this.MigrateEntity(arg.SerializedEntity, context.CommerceContext);
                }
                
                return arg;
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException(this.Name, ex);
                throw;
            }
        }

        /// <summary>
        /// Migrates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>Upgraded entity json</returns>
        private async Task<string> MigrateEntity(string entity, CommerceContext context)
        {
            try
            {
                JObject jObject = null;
                using (var reader = new StringReader(entity))
                {
                    using (var jsonReader = new JsonTextReader(reader) { DateParseHandling = DateParseHandling.DateTimeOffset })
                    {
                        while (jsonReader.Read())
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject)
                            {
                                jObject = JObject.Load(jsonReader);
                                UpgradeObsoleteTokens(jObject, context);
                            }
                        }
                    }
                }

                return JsonConvert.SerializeObject(jObject);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                await context.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "FailedToMigrateEnvironment",
                        new object[] { entity, ex },
                        $"Failed to migrate environment.");
                return entity;
            }          
        }

        /// <summary>
        /// Upgrades the obsolete tokens.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="context">The context.</param>
        private static void UpgradeObsoleteTokens(JToken token, CommerceContext context)
        {
            JContainer container = token as JContainer;
            if (container == null) return;

            FindTokens(container, context);
        }

        /// <summary>
        /// Finds the obsolete tokens.
        /// </summary>
        /// <param name="containerToken">The container token.</param>
        /// <param name="context">The context.</param>
        private static void FindTokens(JToken containerToken, CommerceContext context)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    FindTokens(child, context);
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, context);
                }
            }
            else if (containerToken.Type == JTokenType.Property)
            {
                var property = containerToken as JProperty;
                if (property.Name.Equals("$type", StringComparison.OrdinalIgnoreCase) &&
                    property.Value.ToString().Equals("Sitecore.Commerce.Plugin.Customers.Cs.ProfilesSqlPolicy, Sitecore.Commerce.Plugin.Customers.Cs", StringComparison.OrdinalIgnoreCase))
                {
                    property.Value = "Plugin.Sample.Customers.CsMigration.ProfilesSqlPolicy, Plugin.Sample.Customers.CsMigration";
                }
                else if (property.Value is JContainer)
                {
                    foreach (JToken child in (JContainer)property.Value)
                    {
                        FindTokens(child, context);
                    }
                }
            }
        }
    }
}
