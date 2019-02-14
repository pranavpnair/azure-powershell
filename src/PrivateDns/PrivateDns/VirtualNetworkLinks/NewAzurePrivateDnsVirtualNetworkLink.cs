// ------------------------------------------------------------------------------------------------
// <copyright file="NewAzurePrivateDnsVirtualNetworkLink.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Microsoft.Azure.Commands.PrivateDns.Utilities;

namespace Microsoft.Azure.Commands.PrivateDns.VirtualNetworkLinks
{
    using System.Collections;
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Creates a new zone.
    /// </summary>
    [Cmdlet("New", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsVirtualNetworkLink", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium, DefaultParameterSetName = IdsParameterSetName), OutputType(typeof(PrivateDnsZone))]
    public class NewAzurePrivateDnsVirtualNetworkLink : PrivateDnsBaseCmdlet
    {
        private const string IdsParameterSetName = "Ids";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the link.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group in which to create the zone.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the zone associated with the link (without a terminating dot).")]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the virtual network associated with the link.")]
        [ValidateNotNullOrEmpty]
        public string VirtualNetworkId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Boolean that represents if the link is registration enabled or not.")]
        [ValidateNotNullOrEmpty]
        public bool IsRegistrationEnabled { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.")]
        public Hashtable Tags { get; set; }

        public override void ExecuteCmdlet()
        {
            if (this.ZoneName.EndsWith("."))
            {
                this.ZoneName = this.ZoneName.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{this.ZoneName}\".");
            }

            ConfirmAction(
                ProjectResources.Progress_CreatingNewVirtualNetworkLink,
                this.Name,
                () =>
                {
                    var result = this.PrivateDnsClient.CreatePrivateDnsLink(
                        this.Name,
                        this.ResourceGroupName,
                        this.ZoneName,
                        this.VirtualNetworkId,
                        this.IsRegistrationEnabled,
                        this.Tags);
                    this.WriteVerbose(ProjectResources.Success);
                    this.WriteVerbose(string.Format(ProjectResources.Success_NewVirtualNetworkLink, this.Name, this.ResourceGroupName));
                    this.WriteObject(result);
                });
        }
    }
}
