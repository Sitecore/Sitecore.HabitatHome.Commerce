// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.Sample
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    /// <inheritdoc />
    /// <summary>
    /// Defines the SampleCommand command.
    /// </summary>
    public class SampleCommand : CommerceCommand
    {
        private readonly ISamplePipeline _pipeline;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.SampleCommand" /> class.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline.
        /// </param>
        /// <param name="serviceProvider">The service provider</param>
        public SampleCommand(ISamplePipeline pipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._pipeline = pipeline;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="parameter">
        /// The parameter for the command
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<SampleEntity> Process(CommerceContext commerceContext, object parameter)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var arg = new SampleArgument(parameter);
                var result = await this._pipeline.Run(arg, new CommercePipelineExecutionContextOptions(commerceContext));

                return result;
            }
        }
    }
}