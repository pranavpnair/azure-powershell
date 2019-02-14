// ------------------------------------------------------------------------------------------------
// <copyright file="PrivateDnsZone.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Models
{

    using System.Collections;

    /// <summary>
    /// A private DNS zone
    /// </summary>
    public class PrivateDnsZone
    {
        /// <summary>
        /// Gets or sets the name of the DNS zone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource Id of the DNS zone.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource group to which this zone belongs.
        /// </summary>
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the location to which this zone belongs.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Etag of this zone
        /// </summary>
        public string Etag { get; set; }

        /// <summary>
        /// Gets or sets the tags of this resource
        /// </summary>
        public Hashtable Tags { get; set; }

        /// <summary>
        /// Gets or sets the number of records for this zone
        /// </summary>
        public long? NumberOfRecordSets { get; set; }

        /// <summary>
        ///Gets or sets the max number of records for this zone
        /// </summary>
        public long? MaxNumberOfRecordSets { get; set; }
    }
}
