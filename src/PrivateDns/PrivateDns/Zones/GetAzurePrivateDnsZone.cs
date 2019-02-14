// ------------------------------------------------------------------------------------------------
// <copyright file="GetAzurePrivateDnsZone.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Zones
{
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;

    /// <summary>
    /// Gets one or more existing zones.
    /// </summary>
    [Cmdlet("Get", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsZone", DefaultParameterSetName = "Default"), OutputType(typeof(PrivateDnsZone))]
    public class GetAzurePrivateDnsZone : PrivateDnsBaseCmdlet
    {
        private const string ParameterSetResourceGroup = "ResourceGroup";
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSetResourceGroup, HelpMessage = "The full name of the private zone (without a terminating dot).")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = ParameterSetResourceGroup, HelpMessage = "The resource group in which the private zone exists.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        public override void ExecuteCmdlet()
        {
            if (this.Name != null)
            {
                if (this.Name.EndsWith("."))
                {
                    this.Name = this.Name.TrimEnd('.');
                    this.WriteWarning(
                        $"Modifying private zone name to remove terminating '.'. Private Zone name used is \"{this.Name}\".");
                }

                this.WriteObject(this.PrivateDnsClient.GetPrivateDnsZone(this.Name, this.ResourceGroupName));
            }
            else if (!string.IsNullOrEmpty(this.ResourceGroupName))
            {
                WriteObject(this.PrivateDnsClient.ListPrivateDnsZonesInResourceGroup(this.ResourceGroupName), true);
            }
            else
            {
                WriteObject(this.PrivateDnsClient.ListPrivateDnsZonesInSubscription(), true);
            }
        }
    }
}
