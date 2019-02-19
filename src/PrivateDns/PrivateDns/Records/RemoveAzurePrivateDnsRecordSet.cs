// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveAzurePrivateDnsRecordSet.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Records
{
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Management.PrivateDns.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Deletes an existing record set.
    /// </summary>
    [Cmdlet("Remove", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsRecordSet", SupportsShouldProcess = true),OutputType(typeof(bool))]
    public class RemoveAzureDnsRecordSet : PrivateDnsBaseCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "Fields", HelpMessage = "The name of the records in the record set (relative to the name of the zone and without a terminating dot).")]
        [Parameter(Mandatory = true, ParameterSetName = "Mixed")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The type of Private DNS records in the record set.", ParameterSetName = "Fields")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The type of Private DNS records in the record set.", ParameterSetName = "Mixed")]
        [ValidateNotNullOrEmpty]
        public RecordType RecordType { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The zone in which the record set exists (without a terminating dot).", ParameterSetName = "Fields")]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group to which the zone belongs.", ParameterSetName = "Fields")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The PrivateDnsZone object representing the zone in which to create the record set.", ParameterSetName = "Mixed")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsZone Zone { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The record set in which to add the record.", ParameterSetName = "Object")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsRecordSet RecordSet { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the RecordSet parameter for optimistic concurrency checks.", ParameterSetName = "Object")]
        public SwitchParameter Overwrite { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        public override void ExecuteCmdlet()
        {
            bool deleted = false;
            PrivateDnsRecordSet recordSetToDelete = null;

            if (this.ParameterSetName == "Fields")
            {
                if (this.Name.EndsWith("."))
                {
                    this.Name = this.Name.TrimEnd('.');
                    this.WriteWarning(
                        $"Modifying recordset name to remove terminating '.'.  Recordset name used is \"{this.Name}\".");
                }

                recordSetToDelete = new PrivateDnsRecordSet
                {
                    Name = this.Name,
                    Etag = null,
                    RecordType = this.RecordType,
                    ResourceGroupName = this.ResourceGroupName,
                    ZoneName = this.ZoneName,
                };
            }
            else if (this.ParameterSetName == "Mixed")
            {
                if (this.Name.EndsWith("."))
                {
                    this.Name = this.Name.TrimEnd('.');
                    this.WriteWarning(
                        $"Modifying recordset name to remove terminating '.'.  Recordset name used is \"{this.Name}\".");
                }

                recordSetToDelete = new PrivateDnsRecordSet
                {
                    Name = this.Name,
                    Etag = null,
                    RecordType = this.RecordType,
                    ResourceGroupName = this.Zone.ResourceGroupName,
                    ZoneName = this.Zone.Name,
                };
            }
            else if (this.ParameterSetName == "Object")
            {
                if ((string.IsNullOrWhiteSpace(this.RecordSet.Etag) || this.RecordSet.Etag == "*") && !this.Overwrite.IsPresent)
                {
                    throw new PSArgumentException(string.Format(ProjectResources.Error_EtagNotSpecified, typeof(PrivateDnsRecordSet).Name));
                }

                recordSetToDelete = this.RecordSet;
            }

            if (recordSetToDelete?.ZoneName != null && recordSetToDelete.ZoneName.EndsWith("."))
            {
                recordSetToDelete.ZoneName = recordSetToDelete.ZoneName.TrimEnd('.');
                this.WriteWarning(
                    $"Modifying zone name to remove terminating '.'.  Zone name used is \"{recordSetToDelete.ZoneName}\".");
            }

            var overwrite = this.Overwrite.IsPresent || this.ParameterSetName != "Object";

            ConfirmAction(
                ProjectResources.Progress_RemovingRecordSet,
                this.Name,
                () =>
                {
                    deleted = PrivateDnsClient.DeletePrivateDnsRecordSet(recordSetToDelete, overwrite);
                    if (deleted)
                    {
                        WriteVerbose(ProjectResources.Success);
                        WriteVerbose(ProjectResources.Success_RemoveRecordSet);
                    }

                    if (this.PassThru)
                    {
                        WriteObject(deleted);
                    }
                });
        }
    }
}
