// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveAzurePrivateDnsZone.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Zones
{
    using System.Management.Automation;
    using ResourceManager.Common.ArgumentCompleters;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.PrivateDns.Utilities;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Deletes an existing zone.
    /// </summary>
    [Cmdlet("Remove", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsZone", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High),OutputType(typeof(bool))]
    public class RemoveAzurePrivateDnsZone : PrivateDnsBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the zone (without a terminating dot).", ParameterSetName = "Fields")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group in which the zone exists.", ParameterSetName = "Fields")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The zone object to set.", ParameterSetName = "Object")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsZone Zone { get; set; }

        [Parameter(ParameterSetName = "ResourceId", Mandatory = true, ValueFromPipeline = true, HelpMessage = "Private DNS Zone ResourceID.")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the Zone parameter for optimistic concurrency checks.", ParameterSetName = "Object")]
        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the Zone parameter for optimistic concurrency checks.", ParameterSetName = "ResourceId")]
        public SwitchParameter Overwrite { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        public override void ExecuteCmdlet()
        {
            const bool deleted = true;
            var overwrite = this.Overwrite.IsPresent || this.ParameterSetName != "Object";

            if (!string.IsNullOrEmpty(this.ResourceId))
            {
                PrivateDnsUtils.GetResourceGroupNameAndZoneNameFromZoneId(this.ResourceId, out var resourceGroupName, out var zoneName);
                this.ResourceGroupName = resourceGroupName;
                this.Name = zoneName;
            }

            if (!string.IsNullOrEmpty(this.Name) && this.Name.EndsWith("."))
            {
                this.Name = this.Name.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{this.Name}\".");
            }

            var zoneToDelete = (this.ParameterSetName != "Object")
                ? this.PrivateDnsClient.GetDnsZoneHandleNonExistentZone(this.Name, this.ResourceGroupName)
                : this.Zone;

            if (zoneToDelete == null)
            {
                this.WriteWarning("Invalid zone details specified");
                return;
            }

            if ((string.IsNullOrWhiteSpace(zoneToDelete.Etag) || zoneToDelete.Etag == "*") && !overwrite)
            {
                throw new PSArgumentException(string.Format(ProjectResources.Error_EtagNotSpecified, typeof(PrivateDnsZone).Name));
            }

            if (zoneToDelete.Name != null && zoneToDelete.Name.EndsWith("."))
            {
                zoneToDelete.Name = zoneToDelete.Name.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{zoneToDelete.Name}\".");
            }

            ConfirmAction(
                ProjectResources.Progress_RemovingZone,
                zoneToDelete.Name,
                () =>
                {
                    PrivateDnsClient.DeletePrivateDnsZone(zoneToDelete, overwrite);

                    WriteVerbose(ProjectResources.Success);
                    WriteVerbose(string.Format(ProjectResources.Success_RemoveZone, zoneToDelete.Name, zoneToDelete.ResourceGroupName));

                    if (this.PassThru)
                    {
                        WriteObject(deleted);
                    }
                });
        }
    }
}
