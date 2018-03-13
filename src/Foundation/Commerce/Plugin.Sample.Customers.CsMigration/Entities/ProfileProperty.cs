// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileProperty.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   A ProfileProperty Commerce entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Sitecore.Commerce.Core;   

    /// <summary>
    /// A ProfileProperty Commerce entity
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CommerceEntity" />
    public class ProfileProperty : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileProperty"/> class.
        /// </summary>
        public ProfileProperty()
        {
            this.IsHidden = false;
            this.IsReadOnly = false;
        }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the raw value of a property.
        /// </summary>
        /// <value>
        /// The raw value.
        /// </value>
        public object RawValue { get; set; }

        /// <summary>
        /// Gets or sets the value of a property.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets whether the property is hidden in the UX.
        /// </summary>
        /// <value>
        /// The IsHidden value.
        /// </value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the original type of the RawValue.
        /// </summary>
        /// <value>
        /// The OriginalType value.
        /// </value>
        public string OriginalType { get; set; }

        /// <summary>
        /// Gets or sets the profile reference.
        /// </summary>
        /// <value>
        /// The profile reference.
        /// </value>
        public string ProfileReference { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the minimum length.
        /// </summary>
        /// <value>
        /// The minimum length.
        /// </value>
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi valued.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multi valued; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiValued { get; set; }
    }
}
