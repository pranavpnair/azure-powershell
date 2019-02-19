// ------------------------------------------------------------------------------------------------
// <copyright file="GetAzurePrivateDnsRecordSet.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Records
{
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
    using Microsoft.Azure.Management.PrivateDns.Models;
    using System.Collections.Generic;
    using System.Management.Automation;

    /// <summary>
    /// Gets one or more existing record sets.
    /// </summary>
    [Cmdlet("Get", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsRecordSet"), OutputType(typeof(PrivateDnsRecordSet))]
    public class GetAzurePrivateDnsRecordSet : PrivateDnsBaseCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "Fields", ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the records inthis record set (relative to the name of the zone and without a terminating dot).")]
        [Parameter(Mandatory = false, ParameterSetName = "Object")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The zone in which to create the record set (without a terminating dot).", ParameterSetName = "Fields")]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group to which the zone belongs.", ParameterSetName = "Fields")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The DnsZone object representing the zone in which to create the record set.", ParameterSetName = "Object")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsZone Zone { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The type of DNS records in this record set.")]
        [ValidateNotNullOrEmpty]
        public RecordType? RecordType { get; set; }

        public override void ExecuteCmdlet()
        {
            string zoneName = null;
            string resourceGroupName = null;

            if (this.ParameterSetName == "Fields")
            {
                zoneName = this.ZoneName;
                resourceGroupName = this.ResourceGroupName;
            }
            else
            {
                zoneName = this.Zone.Name;
                resourceGroupName = this.Zone.ResourceGroupName;
            }

            if (zoneName != null && zoneName.EndsWith("."))
            {
                zoneName = zoneName.TrimEnd('.');
                this.WriteWarning(string.Format("Modifying zone name to remove terminating '.'.  Zone name used is \"{0}\".", zoneName));
            }

            if (this.Name != null)
            {
                if (this.RecordType == null)
                {
                    throw new PSArgumentException("If you specify the Name parameter you must also specify the RecordType parameter.");
                }

                PrivateDnsRecordSet result = this.PrivateDnsClient.GetPrivateDnsRecordSet(this.Name, zoneName, resourceGroupName, this.RecordType.Value);
                this.WriteObject(result);
            }
            else
            {
                List<PrivateDnsRecordSet> result = null;
                if (this.RecordType == null)
                {
                    result = this.PrivateDnsClient.ListRecordSets(zoneName, resourceGroupName);
                }
                else
                {
                    result = this.PrivateDnsClient.ListRecordSets(zoneName, resourceGroupName, this.RecordType.Value);
                }

                foreach (var r in result)
                {
                    this.WriteObject(r);
                }
            }

        }
    }
}
