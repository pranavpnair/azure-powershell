// ------------------------------------------------------------------------------------------------
// <copyright file="SetAzurePrivateDnsZone.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Zones
{
    using System.Collections;
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.PrivateDns.Utilities;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Updates an existing zone.
    /// </summary>
    [Cmdlet("Set", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsZone", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium), OutputType(typeof(PrivateDnsZone))]
    public class SetAzurePrivateDnsZone : PrivateDnsBaseCmdlet
    {
        private const string FieldsParameterSetName = "Fields";
        private const string ObjectParameterSetName = "Object";
        private const string ResourceParameterSetName = "ResourceId";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the zone (without a terminating dot).", ParameterSetName = FieldsParameterSetName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group in which the zone exists.", ParameterSetName = FieldsParameterSetName)]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.", ParameterSetName = FieldsParameterSetName)]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.", ParameterSetName = ResourceParameterSetName)]
        public Hashtable Tags { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The zone object to set.", ParameterSetName = ObjectParameterSetName)]
        [ValidateNotNullOrEmpty]
        public PrivateDnsZone Zone { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the RecordSet parameter for optimistic concurrency checks.", ParameterSetName = ObjectParameterSetName)]
        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the RecordSet parameter for optimistic concurrency checks.", ParameterSetName = ResourceParameterSetName)]
        public SwitchParameter Overwrite { get; set; }

        [Parameter( ParameterSetName = ResourceParameterSetName, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Private DNS Zone ResourceID.")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        public override void ExecuteCmdlet()
        {

            PrivateDnsZone result = null;
            PrivateDnsZone zoneToUpdate = null;

            if (this.ParameterSetName == FieldsParameterSetName || this.ParameterSetName == ResourceParameterSetName)
            {
                if (!string.IsNullOrEmpty(this.ResourceId))
                {
                    PrivateDnsUtils.GetResourceGroupNameAndZoneNameFromZoneId(this.ResourceId, out var resourceGroupName, out var zoneName);
                    this.ResourceGroupName = resourceGroupName;
                    this.Name = zoneName;
                }

                if (this.Name.EndsWith("."))
                {
                    this.Name = this.Name.TrimEnd('.');
                    this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{this.Name}\".");
                }

                zoneToUpdate = this.PrivateDnsClient.GetPrivateDnsZone(this.Name, this.ResourceGroupName);
                zoneToUpdate.Etag = "*";
                zoneToUpdate.Tags = this.Tags;
            }
            else if (this.ParameterSetName == ObjectParameterSetName)
            {
                if ((string.IsNullOrWhiteSpace(this.Zone.Etag) || this.Zone.Etag == "*") && !this.Overwrite.IsPresent)
                {
                    throw new PSArgumentException(string.Format(ProjectResources.Error_EtagNotSpecified, typeof(PrivateDnsZone).Name));
                }

                zoneToUpdate = this.Zone;
            }

            if (zoneToUpdate?.Name != null && zoneToUpdate.Name.EndsWith("."))
            {
                zoneToUpdate.Name = zoneToUpdate.Name.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{zoneToUpdate.Name}\".");
            }
            ConfirmAction(
                ProjectResources.Progress_Modifying,
                zoneToUpdate?.Name,
                () =>
                {
                    bool overwrite = this.Overwrite.IsPresent || this.ParameterSetName != ObjectParameterSetName;
                    result = this.PrivateDnsClient.UpdatePrivateDnsZone(zoneToUpdate, overwrite);

                    WriteVerbose(ProjectResources.Success);
                    WriteObject(result);
                });
        }
    }
}
