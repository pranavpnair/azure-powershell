# ------------------------------------------------------------------------------------------------
# <copyright file="LinkTests.ps1" company="Microsoft Corporation">
# Copyright (c) Microsoft Corporation. All rights reserved.
# </copyright>
# ------------------------------------------------------------------------------------------------

<#
.SYNOPSIS
Full Link CRUD cycle
#>
function Test-LinkCrud
{
	$zoneName = "testzone.com"
	$linkName = Get-RandomLinkName
    $resourceGroup = TestSetup-CreateResourceGroup

	$createdZone = New-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Tags @{tag1="value1"}
	$createdVirtualNetwork = New-AzureRmVirtualNetwork -Name MyVirtualNetwork -ResourceGroupName $resourceGroup.ResourceGroupName -Location $resourceGroup.Location -AddressPrefix "10.0.0.0/27"
	$createdLink = New-AzPrivateDnsVirtualNetworkLink -ZoneName $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Name $linkName -Tags @{tag1="value1"} -VirtualNetworkId $createdVirtualNetwork.Id -IsRegistrationEnabled $false

	Write-Verbose "hello"
	Write-Verbose $createdLink.Name
	Write-Verbose $createdLink.ZoneName
	Write-Verbose $zoneName
	Write-Verbose "World"

	Assert-NotNull $createdLink
	Assert-NotNull $createdLink.Etag
	Assert-AreEqual $linkName $createdLink.Name
	Assert-AreEqual $zoneName $createdLink.ZoneName
	Assert-AreEqual $resourceGroup.ResourceGroupName $createdLink.ResourceGroupName
	Assert-AreEqual 1 $createdLink.Tags.Count
	Assert-AreEqual $false $createdLink.RegistrationEnabled
	Assert-AreNotEqual $createdVirtualNetwork.Id $createdZone.VirtualNetworkId
	Assert-Null $createdLink.Type

	$retrievedLink = Get-AzPrivateDnsVirtualNetworkLink -ZoneName $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Name $linkName
	Write-Verbose "hello"
	Write-Verbose $createdZone.Name
	Write-Verbose $zoneName
	Write-Verbose "World"

	Assert-NotNull $retrievedLink
	Assert-NotNull $retrievedLink.Etag
	Assert-AreEqual $linkName $retrievedLink.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $retrievedLink.ResourceGroupName
	Assert-AreEqual $retrievedLink.Etag $createdLink.Etag
	Assert-AreEqual 1 $retrievedLink.Tags.Count
	Assert-AreEqual $createdLink.VirtualNetworkId $retrievedLink.VirtualNetworkId
	Assert-AreEqual $createdLink.ZoneName $retrievedLink.ZoneName
	Assert-AreEqual $createdLink.RegistrationEnabled $retrievedLink.RegistrationEnabled
	Assert-Null $retrievedLink.Type

	$updatedLink = Set-AzPrivateDnsVirtualNetworkLink -ZoneName $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Name $linkName -Tags @{tag1="value1";tag2="value2"}

	Assert-NotNull $updatedLink
	Assert-NotNull $updatedLink.Etag
	Assert-AreEqual $linkName $updatedLink.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $updatedLink.ResourceGroupName
	Assert-AreNotEqual $updatedLink.Etag $createdLink.Etag
	Assert-AreEqual 2 $updatedLink.Tags.Count
	Assert-Null $updatedLink.Type

	$retrievedLink = Get-AzPrivateDnsVirtualNetworkLink -ZoneName $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Name $linkName

	Assert-NotNull $retrievedLink
	Assert-NotNull $retrievedLink.Etag
	Assert-AreEqual $linkName $retrievedLink.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $retrievedLink.ResourceGroupName
	Assert-AreEqual $retrievedLink.Etag $updatedLink.Etag
	Assert-AreEqual 2 $retrievedLink.Tags.Count
	Assert-Null $retrievedLink.Type

	$removed = Remove-AzPrivateDnsVirtualNetworkLink -ZoneName $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Name $linkName -PassThru -Confirm:$false

	Assert-True { $removed }

	Assert-Throws { Get-AzPrivateDnsVirtualNetworkLink -ZoneName $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Name $linkName }
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force	
}