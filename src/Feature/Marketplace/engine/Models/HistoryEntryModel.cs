// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryEntryModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a HistoryEntryModel model
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Model" />
    public class HistoryEntryModel : Model
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.SampleModel" /> class.
        /// </summary>
        public HistoryEntryModel()
        {
            this.Id = Guid.NewGuid().ToString("N");
            EventDate = DateTimeOffset.Now;
            EventMessage = "";
            EventUser = "";
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// The DateTimeOffset of the Event
        /// </summary>
        public DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// A Message about the Event
        /// </summary>
        public string EventMessage { get; set; }

        /// <summary>
        /// A User related to the event
        /// </summary>
        public string EventUser { get; set; }

        /// <summary>
        /// Any Data related to the event can be kept here
        /// </summary>
        public string EventData { get; set; }

    }
}
