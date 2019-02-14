# ------------------------------------------------------------------------------------------------
# <copyright file="Common.ps1" company="Microsoft Corporation">
# Copyright (c) Microsoft Corporation. All rights reserved.
# </copyright>
# ------------------------------------------------------------------------------------------------

<#
.SYNOPSIS
Gets valid resource group name
#>
function Get-ResourceGroupName
{
    return getAssetName
}

<#
.SYNOPSIS
Gets valid resource name
#>
function Get-ResourceName
{
    return getAssetName
}

<#
.SYNOPSIS
Gets valid virtual network name
#>
function Get-VirtualNetworkName
{
    return getAssetName
}

<#
.SYNOPSIS
Gets the default location for a provider
#>
function Get-ProviderLocation($provider)
{
	if ([Microsoft.Azure.Test.HttpRecorder.HttpMockServer]::Mode -ne [Microsoft.Azure.Test.HttpRecorder.HttpRecorderMode]::Playback)
	{
		$namespace = $provider.Split("/")[0]
		if($provider.Contains("/"))
		{
			$type = $provider.Substring($namespace.Length + 1)
			$location = Get-AzResourceProvider -ProviderNamespace $namespace | where {$_.ResourceTypes[0].ResourceTypeName -eq $type}

			if ($location -eq $null)
			{
				return "West US"
			} else
			{
				return $location.Locations[0]
			}
		}

		return "West US"
	}

	return "WestUS"
}

<#
.SYNOPSIS
Creates a resource group to use in tests
#>
function TestSetup-CreateResourceGroup
{
    $resourceGroupName = Get-ResourceGroupName
	$rglocation = Get-ProviderLocation "microsoft.compute"
    $resourceGroup = New-AzResourceGroup -Name $resourceGroupName -location $rglocation
	return $resourceGroup
}

<#
.SYNOPSIS
Creates a virtual network to use in tests
#>
function TestSetup-CreateVirtualNetwork($resourceGroup)
{
    $virtualNetworkName = Get-VirtualNetworkName
	$location = Get-ProviderLocation "microsoft.network/virtualNetworks"
    $virtualNetwork = New-AzVirtualNetwork -Name $virtualNetworkName -ResourceGroupName $resourceGroup.ResourceGroupName -Location $location -AddressPrefix "10.0.0.0/8"
	return $virtualNetwork
}

function Get-RandomZoneName
{
	$prefix = getAssetName;
	return $prefix + ".pstest.test" ;
}

function Get-RandomLinkName
{
	$prefix = getAssetName;
	return $prefix + ".testlink" ;
}

function Get-TxtOfSpecifiedLength([int] $length)
{
	$returnValue = "";
	for ($i = 0; $i -lt $length ; $i++)
	{
		$returnValue += "a";
	}
	return $returnValue;
}