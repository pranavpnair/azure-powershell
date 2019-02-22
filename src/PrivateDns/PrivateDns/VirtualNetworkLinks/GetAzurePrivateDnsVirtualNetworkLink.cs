﻿// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------


namespace Microsoft.Azure.Commands.PrivateDns.VirtualNetworkLinks
{
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;

    /// <summary>
    /// Gets one or more existing zones.
    /// </summary>
    [Cmdlet("Get", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsVirtualNetworkLink", DefaultParameterSetName = "Default"), OutputType(typeof(PrivateDnsZone))]
    public class GetAzurePrivateDnsVirtualNetworkLink : PrivateDnsBaseCmdlet
    {
        private const string ParameterSetResourceGroup = "ResourceGroup";

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetResourceGroup, HelpMessage = "The resource group in which the private zone exists.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetResourceGroup, HelpMessage = "The full name of the private zone (without a terminating dot).")]
        [ResourceNameCompleter("Microsoft.Network/privateDnsZones", "ResourceGroupName")]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = ParameterSetResourceGroup, HelpMessage = "The full name of the virtual network link.")]
        [ResourceNameCompleter("Microsoft.Network/privateDnsZones/virtualNetworkLinks", "ResourceGroupName","ZoneName")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            if (this.Name != null)
            {
                if (this.ZoneName.EndsWith("."))
                {
                    this.ZoneName = this.ZoneName.TrimEnd('.');
                    this.WriteWarning(
                        $"Modifying private zone name to remove terminating '.'. Private Zone name used is \"{this.ZoneName}\".");
                }

                this.WriteObject(this.PrivateDnsClient.GetPrivateDnsLink(this.Name, this.ResourceGroupName, this.ZoneName));
            }
            else if (!string.IsNullOrEmpty(this.ZoneName) && !string.IsNullOrEmpty(this.ResourceGroupName))
            {
                WriteObject(this.PrivateDnsClient.ListPrivateDnsLinksInZone(this.ResourceGroupName, this.ZoneName), true);
            }
            else
            {
                this.WriteWarning("Invalid parameters specified. Please check the values entered for Zone name and ResourceGroup name.");
                return;
            }
        }
    }
}
