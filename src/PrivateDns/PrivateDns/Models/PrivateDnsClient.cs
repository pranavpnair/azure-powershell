// ------------------------------------------------------------------------------------------------
// <copyright file="DnsClient.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Azure.Commands.Common.Authentication;
    using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
    using Microsoft.Azure.Commands.ResourceManager.Common.Tags;
    using Microsoft.Azure.Management.PrivateDns;
    using Microsoft.Azure.Management.PrivateDns.Models;
    using Microsoft.Rest.Azure;
    using ProjectResources = Microsoft.Azure.Commands.PrivateDns.Properties.Resources;
    using Sdk = Microsoft.Azure.Management.PrivateDns.Models;

    public class PrivateDnsClient
    {
        public const string DnsResourceLocation = "global";
        public const int TxtRecordMaxLength = 1024;
        public const int TxtRecordMinLength = 0;

        private readonly Dictionary<RecordType, Type> recordTypeValidationEntries = new Dictionary<RecordType, Type>()
        {
            {RecordType.A, typeof (ARecord)},
            {RecordType.AAAA, typeof (AaaaRecord)},
            {RecordType.CNAME, typeof (CnameRecord)},
            {RecordType.MX, typeof (MxRecord)},
            {RecordType.SOA, typeof (SoaRecord)},
            {RecordType.PTR, typeof (PtrRecord)},
            {RecordType.SRV, typeof (SrvRecord)},
            {RecordType.TXT, typeof (TxtRecord)}
        };

        public PrivateDnsClient(IAzureContext context)
            : this(AzureSession.Instance.ClientFactory.CreateArmClient<PrivateDnsManagementClient>(context, AzureEnvironment.Endpoint.ResourceManager))
        {
        }

        public PrivateDnsClient(IPrivateDnsManagementClient managementClient)
        {
            this.PrivateDnsManagementClient = managementClient;
        }

        public IPrivateDnsManagementClient PrivateDnsManagementClient { get; set; }

        public PrivateDnsZone CreatePrivateDnsZone(
            string name,
            string resourceGroupName,
            Hashtable tags)
        {
            var response = this.PrivateDnsManagementClient.PrivateZones.CreateOrUpdate(
                resourceGroupName,
                name,
                new PrivateZone
                {
                    Location = DnsResourceLocation,
                    Tags = TagsConversionHelper.CreateTagDictionary(tags, validate: true)

                },
                ifMatch: null,
                ifNoneMatch: "*");

            return ToPrivateDnsZone(response);
        }

        public PrivateDnsZone UpdatePrivateDnsZone(PrivateDnsZone zone, bool overwrite)
        {
            var response = this.PrivateDnsManagementClient.PrivateZones.CreateOrUpdate(
                zone.ResourceGroupName,
                zone.Name,
                new PrivateZone
                {
                    Location = DnsResourceLocation,
                    Tags = TagsConversionHelper.CreateTagDictionary(zone.Tags, validate: true),
                },
                ifMatch: overwrite ? null : zone.Etag,
                ifNoneMatch: null);

            return ToPrivateDnsZone(response);
        }

        public void DeletePrivateDnsZone(
            PrivateDnsZone zone,
            bool overwrite)
        {
            this.PrivateDnsManagementClient.PrivateZones.Delete(
                zone.ResourceGroupName,
                zone.Name,
                ifMatch: overwrite ? "*" : zone.Etag);
        }

        public PrivateDnsZone GetPrivateDnsZone(string name, string resourceGroupName)
        {
            return ToPrivateDnsZone(this.PrivateDnsManagementClient.PrivateZones.Get(resourceGroupName, name));
        }

        public List<PrivateDnsZone> ListPrivateDnsZonesInResourceGroup(string resourceGroupName)
        {
            List<PrivateDnsZone> results = new List<PrivateDnsZone>();
            IPage<PrivateZone> getResponse = null;
            do
            {
                if (getResponse != null && getResponse.NextPageLink != null)
                {
                    getResponse = this.PrivateDnsManagementClient.PrivateZones.ListByResourceGroupNext(getResponse.NextPageLink);
                }
                else
                {
                    getResponse = this.PrivateDnsManagementClient.PrivateZones.ListByResourceGroup(resourceGroupName);
                }

                results.AddRange(getResponse.Select(ToPrivateDnsZone));
            } while (getResponse != null && getResponse.NextPageLink != null);

            return results;
        }

        public List<PrivateDnsZone> ListPrivateDnsZonesInSubscription()
        {
            List<PrivateDnsZone> results = new List<PrivateDnsZone>();
            IPage<PrivateZone> getResponse = null;
            do
            {
                if (getResponse != null && getResponse.NextPageLink != null)
                {
                    getResponse = this.PrivateDnsManagementClient.PrivateZones.ListNext(getResponse.NextPageLink);
                }
                else
                {
                    getResponse = this.PrivateDnsManagementClient.PrivateZones.List();
                }

                results.AddRange(getResponse.Select(ToPrivateDnsZone));
            } while (getResponse != null && getResponse.NextPageLink != null);

            return results;
        }

        public PrivateDnsRecordSet CreatePrivateDnsRecordSet(
            string zoneName,
            string resourceGroupName,
            string relativeRecordSetName,
            uint? ttl,
            RecordType recordType,
            Hashtable tags,
            bool overwrite,
            PrivateDnsRecordBase[] resourceRecords)
        {
            var recordSet = ConstructRecordSetProperties(recordType, ttl, tags, resourceRecords);

            var response = this.PrivateDnsManagementClient.RecordSets.CreateOrUpdate(
                resourceGroupName,
                zoneName,
                recordType,
                relativeRecordSetName,
                recordSet,
                null,
                overwrite ? null : "*");

            return GetPowerShellRecordSet(zoneName, resourceGroupName, response);
        }

        private RecordSet ConstructRecordSetProperties(
            RecordType recordType,
            uint? ttl,
            Hashtable tags,
            PrivateDnsRecordBase[] resourceRecords)
        {

            var properties = new RecordSet
            {
                Metadata = TagsConversionHelper.CreateTagDictionary(tags, validate: true),
                Ttl = ttl
            };

            if (resourceRecords != null && resourceRecords.Length != 0)
            {
                var expectedTypeOfRecords = this.recordTypeValidationEntries[recordType];
                var mismatchedRecord = resourceRecords.FirstOrDefault(x => x.GetType() != expectedTypeOfRecords);
                if (mismatchedRecord != null)
                {
                    throw new ArgumentException(string.Format(ProjectResources.Error_AddRecordTypeMismatch, mismatchedRecord.GetType(), recordType));
                }

                FillRecordsForType(properties, recordType, resourceRecords);
            }
            else
            {
                FillEmptyRecordsForType(properties, recordType);
            }

            return properties;
        }

        private static void FillRecordsForType(RecordSet properties, RecordType recordType, IReadOnlyList<PrivateDnsRecordBase> resourceRecords)
        {
            switch (recordType)
            {
                case RecordType.A:
                    properties.ARecords = resourceRecords.Select(x => (Sdk.ARecord)(x as ARecord)?.ToMamlRecord()).ToList();
                    break;
                case RecordType.AAAA:
                    properties.AaaaRecords = resourceRecords.Select(x => (Sdk.AaaaRecord)(x as AaaaRecord)?.ToMamlRecord()).ToList();
                    break;
                case RecordType.CNAME:
                    if (resourceRecords.Count > 1)
                    {
                        throw new ArgumentException(ProjectResources.Error_AddRecordMultipleCnames);
                    }

                    properties.CnameRecord = (Sdk.CnameRecord)resourceRecords[0].ToMamlRecord();
                    break;
                case RecordType.MX:
                    properties.MxRecords = resourceRecords.Select(x => (Sdk.MxRecord)(x as MxRecord)?.ToMamlRecord()).ToList();
                    break;
                case RecordType.PTR:
                    properties.PtrRecords = resourceRecords.Select(x => (Sdk.PtrRecord)(x as PtrRecord)?.ToMamlRecord()).ToList();
                    break;
                case RecordType.SRV:
                    properties.SrvRecords = resourceRecords.Select(x => (Sdk.SrvRecord)(x as SrvRecord)?.ToMamlRecord()).ToList();
                    break;
                case RecordType.TXT:
                    properties.TxtRecords = resourceRecords.Select(x => (Sdk.TxtRecord)(x as TxtRecord)?.ToMamlRecord()).ToList();
                    break;
                case RecordType.SOA:
                    properties.SoaRecord = (Sdk.SoaRecord)resourceRecords[0].ToMamlRecord();
                    break;
            }
        }

        private static void FillEmptyRecordsForType(RecordSet properties, RecordType recordType)
        {
            properties.AaaaRecords = recordType == RecordType.AAAA ? new List<Sdk.AaaaRecord>() : null;
            properties.ARecords = recordType == RecordType.A ? new List<Sdk.ARecord>() : null;
            properties.CnameRecord = recordType == RecordType.CNAME ? new Sdk.CnameRecord(String.Empty) : null;
            properties.MxRecords = recordType == RecordType.MX ? new List<Sdk.MxRecord>() : null;
            properties.PtrRecords = recordType == RecordType.PTR ? new List<Sdk.PtrRecord>() : null;
            properties.SoaRecord = null;
            properties.SrvRecords = recordType == RecordType.SRV ? new List<Sdk.SrvRecord>() : null;
            properties.TxtRecords = recordType == RecordType.TXT ? new List<Sdk.TxtRecord>() : null;
        }

        public PrivateDnsRecordSet UpdatePrivateDnsRecordSet(PrivateDnsRecordSet recordSet, bool overwrite)
        {
            var response = this.PrivateDnsManagementClient.RecordSets.CreateOrUpdate(
                recordSet.ResourceGroupName,
                recordSet.ZoneName,
                recordSet.RecordType,
                recordSet.Name,
                new RecordSet
                {
                    Ttl = recordSet.Ttl,
                    Metadata = TagsConversionHelper.CreateTagDictionary(recordSet.Metadata, true),
                    AaaaRecords =
                        recordSet.RecordType == RecordType.AAAA
                            ? GetMamlRecords<AaaaRecord, Sdk.AaaaRecord>(recordSet.Records)
                            : null,
                    ARecords =
                        recordSet.RecordType == RecordType.A
                            ? GetMamlRecords<ARecord, Sdk.ARecord>(recordSet.Records)
                            : null,
                    MxRecords =
                        recordSet.RecordType == RecordType.MX
                            ? GetMamlRecords<MxRecord, Sdk.MxRecord>(recordSet.Records)
                            : null,
                    SrvRecords =
                        recordSet.RecordType == RecordType.SRV
                            ? GetMamlRecords<SrvRecord, Sdk.SrvRecord>(recordSet.Records)
                            : null,
                    TxtRecords =
                        recordSet.RecordType == RecordType.TXT
                            ? GetMamlRecords<TxtRecord, Sdk.TxtRecord>(recordSet.Records)
                            : null,
                    PtrRecords =
                        recordSet.RecordType == RecordType.PTR
                            ? GetMamlRecords<PtrRecord, Sdk.PtrRecord>(recordSet.Records)
                            : null,
                    SoaRecord =
                        recordSet.RecordType == RecordType.SOA
                            ? GetMamlRecords<SoaRecord, Sdk.SoaRecord>(recordSet.Records).SingleOrDefault()
                            : null,
                    CnameRecord =
                        recordSet.RecordType == RecordType.CNAME
                            ? GetMamlRecords<CnameRecord, Sdk.CnameRecord>(recordSet.Records).SingleOrDefault()
                            : null,
                },
                ifMatch: overwrite ? "*" : recordSet.Etag,
                ifNoneMatch: null);

            return GetPowerShellRecordSet(recordSet.ZoneName, recordSet.ResourceGroupName, response);
        }

        public bool DeletePrivateDnsRecordSet(PrivateDnsRecordSet recordSet, bool overwrite)
        {
            this.PrivateDnsManagementClient.RecordSets.Delete(
                recordSet.ResourceGroupName,
                recordSet.ZoneName,
                recordSet.RecordType,
                recordSet.Name,
                ifMatch: overwrite ? "*" : recordSet.Etag);
            return true;
        }

        public PrivateDnsRecordSet GetPrivateDnsRecordSet(string name, string zoneName, string resourceGroupName, RecordType recordType)
        {
            var getResponse = this.PrivateDnsManagementClient.RecordSets.Get(resourceGroupName, zoneName, recordType, name);
            return GetPowerShellRecordSet(zoneName, resourceGroupName, getResponse);
        }

        public List<PrivateDnsRecordSet> ListRecordSets(string zoneName, string resourceGroupName, RecordType recordType)
        {
            var results = new List<PrivateDnsRecordSet>();

            IPage<RecordSet> listResponse = null;
            do
            {
                if (listResponse?.NextPageLink != null)
                {
                    listResponse = this.PrivateDnsManagementClient.RecordSets.ListByTypeNext(listResponse.NextPageLink);
                }
                else
                {
                    listResponse = this.PrivateDnsManagementClient.RecordSets.ListByType(
                                    resourceGroupName,
                                    zoneName,
                                    recordType);
                }

                results.AddRange(listResponse.Select(recordSet => GetPowerShellRecordSet(zoneName, resourceGroupName, recordSet)));

            } while (listResponse?.NextPageLink != null);

            return results;
        }

        public List<PrivateDnsRecordSet> ListRecordSets(string zoneName, string resourceGroupName)
        {
            var results = new List<PrivateDnsRecordSet>();

            IPage<RecordSet> listResponse = null;
            do
            {
                if (listResponse != null && listResponse.NextPageLink != null)
                {
                    listResponse = this.PrivateDnsManagementClient.RecordSets.ListNext(listResponse.NextPageLink);
                }
                else
                {
                    listResponse = this.PrivateDnsManagementClient.RecordSets.List(
                                    resourceGroupName,
                                    zoneName);
                }

                results.AddRange(listResponse.Select(recordSet => GetPowerShellRecordSet(zoneName, resourceGroupName, recordSet)));
            } while (listResponse?.NextPageLink != null);

            return results;
        }

        public PrivateDnsZone GetDnsZoneHandleNonExistentZone(string zoneName, string resourceGroupName)
        {
            PrivateDnsZone retrievedZone = null;
            try
            {
                retrievedZone = this.GetPrivateDnsZone(zoneName, resourceGroupName);
            }
            catch (CloudException exception)
            {
                if (exception.Body.Code != "ResourceNotFound")
                {
                    throw;
                }
            }

            return retrievedZone;
        }

        private static PrivateDnsRecordSet GetPowerShellRecordSet(string zoneName, string resourceGroupName, RecordSet mamlRecordSet)
        {
            // e.g. "/subscriptions/<guid>/resourceGroups/<rg>/providers/microsoft.dns/privatednszones/<zone>/A/<recordset>"
            string recordTypeAsString = mamlRecordSet.Id.Split('/').Reverse().Skip(1).First();

            var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeAsString, true);

            return new PrivateDnsRecordSet
            {
                Etag = mamlRecordSet.Etag,
                Id = mamlRecordSet.Id,
                Name = mamlRecordSet.Name,
                RecordType = recordType,
                Records = GetPowerShellRecords(mamlRecordSet),
                Metadata = TagsConversionHelper.CreateTagHashtable(mamlRecordSet.Metadata),
                ResourceGroupName = resourceGroupName,
                Ttl = (uint)mamlRecordSet.Ttl.GetValueOrDefault(),
                ZoneName = zoneName,
            };
        }


        private static List<PrivateDnsRecordBase> GetPowerShellRecords(RecordSet recordSet)
        {
            var result = new List<PrivateDnsRecordBase>();
            result.AddRange(GetPowerShellRecords(recordSet.AaaaRecords));
            result.AddRange(GetPowerShellRecords(recordSet.ARecords));
            result.AddRange(GetPowerShellRecords(recordSet.MxRecords));
            result.AddRange(GetPowerShellRecords(recordSet.SrvRecords));
            result.AddRange(GetPowerShellRecords(recordSet.TxtRecords));
            result.AddRange(GetPowerShellRecords(recordSet.PtrRecords));
            if (recordSet.CnameRecord != null)
            {
                result.Add(PrivateDnsRecordBase.FromMamlRecord(recordSet.CnameRecord));
            }
            if (recordSet.SoaRecord != null)
            {
                result.Add(PrivateDnsRecordBase.FromMamlRecord(recordSet.SoaRecord));
            }

            return result;
        }

        private static IEnumerable<PrivateDnsRecordBase> GetPowerShellRecords<T>(ICollection<T> mamlObjects) where T : class
        {
            var result = new List<PrivateDnsRecordBase>();
            if (mamlObjects == null || mamlObjects.Count == 0)
            {
                return result;
            }

            return mamlObjects.Select(PrivateDnsRecordBase.FromMamlRecord).ToList();
        }

        private List<MamlRecordType> GetMamlRecords<PSRecordType, MamlRecordType>(IEnumerable<PrivateDnsRecordBase> powerShellRecords)
            where PSRecordType : PrivateDnsRecordBase
        {
            return powerShellRecords
                .Where(record => record is PSRecordType)
                .Cast<PSRecordType>()
                .Select(record => record.ToMamlRecord())
                .Cast<MamlRecordType>()
                .ToList();
        }

        private static PrivateDnsZone ToPrivateDnsZone(PrivateZone zone)
        {
            return new PrivateDnsZone()
            {
                Name = zone.Name,
                ResourceGroupName = ExtractResourceGroupNameFromId(zone.Id),
                Etag = zone.Etag,
                Tags = TagsConversionHelper.CreateTagHashtable(zone.Tags),
                NumberOfRecordSets = zone.NumberOfRecordSets,
                MaxNumberOfRecordSets = zone.MaxNumberOfRecordSets,
            };
        }

        private static string ExtractResourceGroupNameFromId(string id)
        {
            var parts = id.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int rgIndex = -1;
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Equals("resourceGroups", StringComparison.OrdinalIgnoreCase))
                {
                    rgIndex = i;
                    break;
                }
            }

            if (rgIndex != -1 && rgIndex + 1 < parts.Length)
            {
                return parts[rgIndex + 1];
            }

            throw new FormatException($"Unable to extract resource group name from {id} ");
        }
    }
}
