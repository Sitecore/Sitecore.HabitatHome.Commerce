// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCustomerBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Data;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which gets a customer
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Pipelines.PipelineBlock{DataRow, DataRow, CommercePipelineExecutionContext}" />
    [PipelineDisplayName(CustomersCsConstants.Pipelines.Blocks.ValidateCustomerBlock)]
    public class ValidateCustomerBlock : PipelineBlock<DataRow, DataRow, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateCustomerBlock" /> class.
        /// </summary>
        /// <param name="findEntityPipeline">The find entity pipeline.</param>
        public ValidateCustomerBlock(IFindEntityPipeline findEntityPipeline)
        {
            this._findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The data row for a customer<see cref="DataRow" />.
        /// </returns>
        public override async Task<DataRow> Run(DataRow arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name} The data row can not be null");

            var email = arg["u_email_address"] as string;

            // verify if customer exists
            var result = await this._findEntityPipeline.Run(new FindEntityArgument(typeof(EntityIndex), $"{EntityIndex.IndexPrefix<Customer>("Id")}{email}"), context);
            var customerIndex = result as EntityIndex;
            if (customerIndex != null)
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Warning,
                        "CustomerAlreadyExists",
                        new object[] { email, customerIndex.EntityId },
                        $"Customer { email } already exists."),
                    context);               
                return null;
            }
          
            return arg;           
        }
    }
}
