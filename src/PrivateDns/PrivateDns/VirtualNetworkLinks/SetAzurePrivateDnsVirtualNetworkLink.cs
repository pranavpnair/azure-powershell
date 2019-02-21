// ------------------------------------------------------------------------------------------------
// <copyright file="SetAzurePrivateDnsVirtualNetworkLink.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.VirtualNetworkLinks
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
    [Cmdlet("Set", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsVirtualNetworkLink", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium), OutputType(typeof(PrivateDnsZone))]
    public class SetAzurePrivateDnsVirtualNetworkLink : PrivateDnsBaseCmdlet
    {
        private const string FieldsParameterSetName = "Fields";
        private const string ObjectParameterSetName = "Object";
        private const string ResourceParameterSetName = "ResourceId";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group in which the zone exists.", ParameterSetName = FieldsParameterSetName)]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the zone (without a terminating dot).", ParameterSetName = FieldsParameterSetName)]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the virtual network link.", ParameterSetName = FieldsParameterSetName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Boolean that represents if registration is enabled on the link.", ParameterSetName = FieldsParameterSetName)]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Boolean that represents if registration is enabled on the link.", ParameterSetName = ResourceParameterSetName)]
        [ValidateNotNullOrEmpty]
        public bool IsRegistrationEnabled { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.", ParameterSetName = FieldsParameterSetName)]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.", ParameterSetName = ResourceParameterSetName)]
        public Hashtable Tags { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The virtual network link object to set.", ParameterSetName = ObjectParameterSetName)]
        [ValidateNotNullOrEmpty]
        public PrivateDnsLink Link { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the RecordSet parameter for optimistic concurrency checks.", ParameterSetName = ObjectParameterSetName)]
        public SwitchParameter Overwrite { get; set; }

        [Parameter( ParameterSetName = ResourceParameterSetName, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Private DNS Zone ResourceID.")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        public override void ExecuteCmdlet()
        {

            PrivateDnsLink result = null;
            PrivateDnsLink linkToUpdate = null;

            if (this.ParameterSetName == FieldsParameterSetName || this.ParameterSetName == ResourceParameterSetName)
            {
                if (!string.IsNullOrEmpty(this.ResourceId))
                {
                    PrivateDnsUtils.GetResourceGroupNameZoneNameAndLinkNameFromLinkId(this.ResourceId, out var resourceGroupName, out var zoneName, out var linkName);
                    this.ResourceGroupName = resourceGroupName;
                    this.ZoneName = zoneName;
                    this.Name = linkName;
                }

                if (this.ZoneName.EndsWith("."))
                {
                    this.ZoneName = this.ZoneName.TrimEnd('.');
                    this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{this.ZoneName}\".");
                }

                linkToUpdate = this.PrivateDnsClient.GetPrivateDnsLink(this.Name, this.ResourceGroupName, this.ZoneName);
                linkToUpdate.Etag = "*";
                linkToUpdate.Tags = this.Tags;
                linkToUpdate.ZoneName = this.ZoneName;
                linkToUpdate.RegistrationEnabled = this.IsRegistrationEnabled;
            }
            else if (this.ParameterSetName == ObjectParameterSetName)
            {
                if ((string.IsNullOrWhiteSpace(this.Link.Etag) || this.Link.Etag == "*") && !this.Overwrite.IsPresent)
                {
                    throw new PSArgumentException(string.Format(ProjectResources.Error_EtagNotSpecified, typeof(PrivateDnsLink).Name));
                }

                linkToUpdate = this.Link;
            }

            ConfirmAction(
                ProjectResources.Progress_Modifying,
                linkToUpdate?.Name,
                () =>
                {
                    var overwrite = this.Overwrite.IsPresent || this.ParameterSetName != ObjectParameterSetName;
                    result = this.PrivateDnsClient.UpdatePrivateDnsLink(linkToUpdate, overwrite);

                    WriteVerbose(ProjectResources.Success);
                    WriteObject(result);
                });
        }
    }
}
