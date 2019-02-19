// ------------------------------------------------------------------------------------------------
// <copyright file="SetAzurePrivateDnsRecordSet.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Records
{
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Updates an existing record set.
    /// </summary>
    [Cmdlet("Set", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsRecordSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium), OutputType(typeof(PrivateDnsRecordSet))]
    public class SetAzurePrivateDnsRecordSet : PrivateDnsBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The record set in which to add the record.")]
        [ValidateNotNullOrEmpty]
        public PrivateDnsRecordSet RecordSet { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not use the ETag field of the RecordSet parameter for optimistic concurrency checks.", ParameterSetName = "Object")]
        public SwitchParameter Overwrite { get; set; }

        public override void ExecuteCmdlet()
        {
            if ((string.IsNullOrWhiteSpace(this.RecordSet.Etag) || this.RecordSet.Etag == "*") && !this.Overwrite.IsPresent)
            {
                throw new PSArgumentException(string.Format(ProjectResources.Error_EtagNotSpecified, typeof(PrivateDnsRecordSet).Name));
            }

            var recordSetToUpdate = (PrivateDnsRecordSet)this.RecordSet.Clone();
                    if (recordSetToUpdate.ZoneName != null && recordSetToUpdate.ZoneName.EndsWith("."))
                    {
                        recordSetToUpdate.ZoneName = recordSetToUpdate.ZoneName.TrimEnd('.');
                        this.WriteWarning($"Modifying zone name to remove terminating '.'.  Zone name used is \"{recordSetToUpdate.ZoneName}\".");
                    }

            ConfirmAction(
                ProjectResources.Progress_Modifying,
                recordSetToUpdate.Name,
            () =>
            {
                var result = this.PrivateDnsClient.UpdatePrivateDnsRecordSet(recordSetToUpdate, this.Overwrite.IsPresent);

                WriteVerbose(ProjectResources.Success);
                WriteObject(result);
            });
            }
    }
    }
