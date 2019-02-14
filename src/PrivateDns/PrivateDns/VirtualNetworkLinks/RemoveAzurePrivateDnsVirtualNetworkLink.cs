﻿// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveAzurePrivateDnsVirtualNetworkLink.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.VirtualNetworkLinks
{
    using System.Management.Automation;
    using ResourceManager.Common.ArgumentCompleters;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.PrivateDns.Utilities;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Deletes an existing zone.
    /// </summary>
    [Cmdlet("Remove", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsVirtualNetworkLink", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High),OutputType(typeof(bool))]
    public class RemoveAzurePrivateDnsVirtualNetworkLink : PrivateDnsBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the virtual network link.", ParameterSetName = "Fields")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the zone associated with this virtual network link.", ParameterSetName = "Fields")]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group in which the zone exists.", ParameterSetName = "Fields")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The link object to remove.", ParameterSetName = "Object")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsLink Link { get; set; }

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
                PrivateDnsUtils.GetResourceGroupNameZoneNameAndLinkNameFromLinkId(this.ResourceId, out var resourceGroupName, out var zoneName, out var linkName);
                this.ResourceGroupName = resourceGroupName;
                this.ZoneName = zoneName;
                this.Name = linkName;
            }

            if (!string.IsNullOrEmpty(this.ZoneName) && this.ZoneName.EndsWith("."))
            {
                this.ZoneName = this.ZoneName.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{this.ZoneName}\".");
            }

            var linkToDelete = (this.ParameterSetName != "Object")
                ? this.PrivateDnsClient.GetLinkHandleNonExistentLink(this.ZoneName, this.ResourceGroupName, this.Name)
                : this.Link;

            if (linkToDelete == null)
            {
                this.WriteWarning("Invalid link details specified");
                return;
            }

            if ((string.IsNullOrWhiteSpace(linkToDelete.Etag) || linkToDelete.Etag == "*") && !overwrite)
            {
                throw new PSArgumentException(string.Format(ProjectResources.Error_EtagNotSpecified, typeof(PrivateDnsZone).Name));
            }

            if (linkToDelete.ZoneName != null && linkToDelete.ZoneName.EndsWith("."))
            {
                linkToDelete.ZoneName = linkToDelete.ZoneName.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{linkToDelete.ZoneName}\".");
            }

            ConfirmAction(
                ProjectResources.Progress_RemovingLink,
                linkToDelete.Name,
                () =>
                {
                    PrivateDnsClient.DeletePrivateDnsLink(linkToDelete, overwrite);

                    WriteVerbose(ProjectResources.Success);
                    WriteVerbose(string.Format(ProjectResources.Success_RemoveLink, linkToDelete.Name, linkToDelete.ResourceGroupName));

                    if (this.PassThru)
                    {
                        WriteObject(deleted);
                    }
                });
        }
    }
}
