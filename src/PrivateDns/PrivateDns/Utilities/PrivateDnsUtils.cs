// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------


namespace Microsoft.Azure.Commands.PrivateDns.Utilities
{
    using System.Linq;
    using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;

    internal class PrivateDnsUtils
    {
        public static void GetResourceGroupNameAndZoneNameFromZoneId(
            string resourceId,
            out string resourceGroupName,
            out string zoneName)
        {
            var identifier = new ResourceIdentifier(resourceId);
            resourceGroupName = identifier.ResourceGroupName;
            zoneName = identifier.ResourceName;
        }

        public static void GetResourceGroupNameZoneNameAndLinkNameFromLinkId(
            string resourceId,
            out string resourceGroupName,
            out string zoneName,
            out string linkName)
        {
            var identifier = new ResourceIdentifier(resourceId);
            resourceGroupName = identifier.ResourceGroupName;
            linkName = identifier.ResourceName;
            zoneName = identifier.ParentResource.Split('/').Last();
        }

        public static void GetResourceGroupNameFromResourceId(
            string resourceId,
            out string resourceGroupName)
        {
            var identifier = new ResourceIdentifier(resourceId);
            resourceGroupName = identifier.ResourceGroupName;
        }
    }
}
