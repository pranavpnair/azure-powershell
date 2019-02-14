// ------------------------------------------------------------------------------------------------
// <copyright file="PrivateDnsBaseCmdlet.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.PrivateDns.Models
{
    using Microsoft.Azure.Commands.ResourceManager.Common;

    public abstract class PrivateDnsBaseCmdlet : AzureRMCmdlet
    {
        private PrivateDnsClient _client;

        public PrivateDnsClient PrivateDnsClient
        {
            get => _client ?? (_client = new PrivateDnsClient(DefaultContext));

            set => _client = value;
        }
    }
}
