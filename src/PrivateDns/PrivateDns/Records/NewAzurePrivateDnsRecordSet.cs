// ------------------------------------------------------------------------------------------------
// <copyright file="NewAzurePrivateDnsRecordSet.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Records
{
    using System.Collections;
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Management.PrivateDns.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Creates a new record set.
    /// </summary>
    [Cmdlet("New", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsRecordSet", DefaultParameterSetName = "Fields", SupportsShouldProcess = true),OutputType(typeof(PrivateDnsRecordSet))]
    public class NewAzurePrivateDnsRecordSet : PrivateDnsBaseCmdlet
    {
        private uint? _ttlValue;

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the records in this record set (relative to the name of the zone and without a terminating dot).")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The zone in which to create the record set (without a terminating dot).", ParameterSetName = "Fields")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The zone in which to create the record set (without a terminating dot).", ParameterSetName = "AliasFields")]
        [ValidateNotNullOrEmpty]
        public string ZoneName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group to which the zone belongs.", ParameterSetName = "Fields")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group to which the zone belongs.", ParameterSetName = "AliasFields")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The PrivateDnsZone object representing the zone in which to create the record set.", ParameterSetName = "Object")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The PrivateDnsZone object representing the zone in which to create the record set.", ParameterSetName = "AliasObject")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsZone Zone { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The TTL value of all the records in this record set.", ParameterSetName = "Fields")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The TTL value of all the records in this record set.", ParameterSetName = "Object")]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The TTL value of all the records in this record set.", ParameterSetName = "AliasObject")]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The TTL value of all the records in this record set.", ParameterSetName = "AliasFields")]
        [ValidateNotNullOrEmpty]
        public uint Ttl
        {
            get => _ttlValue ?? 0;
            set => _ttlValue = value;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The type of Private DNS records in this record set.")]
        [ValidateNotNullOrEmpty]
        public RecordType RecordType { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.")]
        public Hashtable Metadata { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "The private dns records that are part of this record set.")]
        [ValidateNotNull]
        public PrivateDnsRecordBase[] PrivateDnsRecords { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not fail if the record set already exists.")]
        public SwitchParameter Overwrite { get; set; }

        public override void ExecuteCmdlet()
        {
            string zoneName = null;
            string resourceGroupname = null;
            PrivateDnsRecordSet result = null;

            if (RecordType == RecordType.SOA)
            {
                throw new System.ArgumentException(ProjectResources.Error_AddRecordSOA);
            }

            if (ParameterSetName == "Fields" || ParameterSetName == "AliasFields")
            {
                zoneName = this.ZoneName;
                resourceGroupname = this.ResourceGroupName;
            }
            else if (ParameterSetName == "Object" || ParameterSetName == "AliasObject")
            {
                zoneName = this.Zone.Name;
                resourceGroupname = this.Zone.ResourceGroupName;
            }
            if(zoneName != null && this.Name.EndsWith(zoneName))
            {
                this.WriteWarning(string.Format(ProjectResources.Error_RecordSetNameEndsWithZoneName, this.Name, zoneName));
            }

            if (zoneName != null && zoneName.EndsWith("."))
            {
                zoneName = zoneName.TrimEnd('.');
                this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{zoneName}\".");
            }

            if (this.PrivateDnsRecords == null)
            {
                this.WriteWarning(ProjectResources.Warning_DnsRecordsParamNeedsToBeSpecified);
            }

            ConfirmAction(
                ProjectResources.Progress_CreatingRecordSet,
                this.Name,
                () =>
                {
                    result = this.PrivateDnsClient.CreatePrivateDnsRecordSet(
                        zoneName,
                        resourceGroupname,
                        this.Name, 
                        this._ttlValue,
                        this.RecordType,
                        this.Metadata,
                        this.Overwrite,
                        this.PrivateDnsRecords);

                    if (result != null)
                    {
                        WriteVerbose(ProjectResources.Success);
                        WriteVerbose(string.Format(ProjectResources.Success_NewRecordSet, this.Name, zoneName, this.RecordType));
                        WriteVerbose(string.Format(ProjectResources.Success_RecordSetFqdn, this.Name, zoneName, this.RecordType));
                    }

                    WriteObject(result);
                });
        }
    }
}
