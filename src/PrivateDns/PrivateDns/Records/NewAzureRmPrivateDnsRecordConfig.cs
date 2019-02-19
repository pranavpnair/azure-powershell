// ------------------------------------------------------------------------------------------------
// <copyright file="NewAzurePrivateDnsRecordSetConfig.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Records
{
    using System.Management.Automation;
    using Microsoft.Azure.Commands.PrivateDns.Models;
    using Microsoft.Azure.Management.PrivateDns.Models;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;

    /// <summary>
    /// Constructs an in-memory dns record object
    /// </summary>
    [Cmdlet("New", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "PrivateDnsRecordConfig"), OutputType(typeof(PrivateDnsRecordBase))]
    public class NewAzureRmDnsRecordConfig : PrivateDnsBaseCmdlet
    {
        private const string ParameterSetA = "A";
        private const string ParameterSetAaaa = "Aaaa";
        private const string ParameterSetCName = "CName";
        private const string ParameterSetTxt = "Txt";
        private const string ParameterSetSrv = "Srv";
        private const string ParameterSetPtr = "Ptr" ;
        private const string ParameterSetMx = "Mx";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The IPv4 address for the A record to add.", ParameterSetName = ParameterSetA)]
        [ValidateNotNullOrEmpty]
        public string Ipv4Address { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The IPv6 address for the AAAA record to add.", ParameterSetName = ParameterSetAaaa)]
        [ValidateNotNullOrEmpty]
        public string Ipv6Address { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The mail exchange host for the MX record to add. Must not be relative to the name of the zone. Must not have a terminating dot", ParameterSetName = ParameterSetMx)]
        [ValidateNotNullOrEmpty]
        public string Exchange { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The preference value for the MX record to add.", ParameterSetName = ParameterSetMx)]
        [ValidateNotNullOrEmpty]
        public ushort Preference { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The target host for the PTR record to add. Must not be relative to the name of the zone. Must not have a terminating dot", ParameterSetName = ParameterSetPtr)]
        [ValidateNotNullOrEmpty]
        public string Ptrdname { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The text value for the TXT record to add.", ParameterSetName = ParameterSetTxt)]
        [ValidateNotNullOrEmpty]
        [ValidateLength(PrivateDnsRecordBase.TxtRecordMinLength, PrivateDnsRecordBase.TxtRecordMaxLength)]
        public string Value { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The priority value SRV record to add.", ParameterSetName = ParameterSetSrv)]
        [ValidateNotNullOrEmpty]
        public ushort Priority { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The target host for the SRV record to add. Must not be relative to the name of the zone. Must not have a terminating dot", ParameterSetName = ParameterSetSrv)]
        [ValidateNotNullOrEmpty]
        public string Target { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The port number for the SRV record to add.", ParameterSetName = ParameterSetSrv)]
        [ValidateNotNullOrEmpty]
        public ushort Port { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The weight value for the SRV record to add.", ParameterSetName = ParameterSetSrv)]
        [ValidateNotNullOrEmpty]
        public ushort Weight { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The canonical name for the CNAME record to add. Must not be relative to the name of the zone. Must not have a terminating dot", ParameterSetName = ParameterSetCName)]
        [ValidateNotNullOrEmpty]
        public string Cname { get; set; }

        public override void ExecuteCmdlet()
        {
            PrivateDnsRecordBase result = null;
            switch (this.ParameterSetName)
            {
                case ParameterSetA:
                    {
                        result = new Models.ARecord { Ipv4Address = this.Ipv4Address };
                        break;
                    }

                case ParameterSetAaaa:
                    {
                        result = new Models.AaaaRecord { Ipv6Address = this.Ipv6Address };
                        break;
                    }

                case ParameterSetMx:
                    {
                        result = new Models.MxRecord { Preference = this.Preference, Exchange = this.Exchange };
                        break;
                    }

                case ParameterSetSrv:
                    {
                        result = new Models.SrvRecord { Priority = this.Priority, Port = this.Port, Target = this.Target, Weight = this.Weight };
                        break;
                    }
                case ParameterSetTxt:
                    {
                        result = new Models.TxtRecord { Value = this.Value };
                        break;
                    }
                case ParameterSetCName:
                    {
                        result = new Models.CnameRecord { Cname = this.Cname };
                        break;
                    }
                case ParameterSetPtr:
                    {
                        result = new Models.PtrRecord { Ptrdname = this.Ptrdname};
                        break;
                    }

                default:
                    {
                        throw new PSArgumentException(string.Format(ProjectResources.Error_UnknownParameterSetName, this.ParameterSetName));
                    }
            }

            WriteObject(result);
        }
    }
}
