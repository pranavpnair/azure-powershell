﻿# ------------------------------------------------------------------------------------------------
# <copyright file="ZoneTests.ps1" company="Microsoft Corporation">
# Copyright (c) Microsoft Corporation. All rights reserved.
# </copyright>
# ------------------------------------------------------------------------------------------------

<#
.SYNOPSIS
Full Zone CRUD cycle
#>
function Test-ZoneCrud
{
	$zoneName = Get-RandomZoneName
    $resourceGroup = TestSetup-CreateResourceGroup

	Write-Debug "Resource group returned"
	$createdZone = New-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Tags @{tag1="value1"}

	Assert-NotNull $createdZone
	Assert-NotNull $createdZone.Etag
	Assert-AreEqual $zoneName $createdZone.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $createdZone.ResourceGroupName
	Assert-AreEqual 1 $createdZone.Tags.Count
	Assert-AreEqual 1 $createdZone.NumberOfRecordSets
	Assert-AreNotEqual $createdZone.NumberOfRecordSets $createdZone.MaxNumberOfRecordSets
	Assert-Null $createdZone.Type

	Write-Debug "zone created"
	$retrievedZone = Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName

	Assert-NotNull $retrievedZone
	Assert-NotNull $retrievedZone.Etag
	Assert-AreEqual $zoneName $retrievedZone.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $retrievedZone.ResourceGroupName
	Assert-AreEqual $retrievedZone.Etag $createdZone.Etag
	Assert-AreEqual 1 $retrievedZone.Tags.Count
	Assert-AreEqual $createdZone.NumberOfRecordSets $retrievedZone.NumberOfRecordSets
	Assert-Null $retrievedZone.Type

	$updatedZone = Set-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Tags @{tag1="value1";tag2="value2"}

	Assert-NotNull $updatedZone
	Assert-NotNull $updatedZone.Etag
	Assert-AreEqual $zoneName $updatedZone.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $updatedZone.ResourceGroupName
	Assert-AreNotEqual $updatedZone.Etag $createdZone.Etag
	Assert-AreEqual 2 $updatedZone.Tags.Count
	Assert-Null $updatedZone.Type

	$retrievedZone = Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName

	Assert-NotNull $retrievedZone
	Assert-NotNull $retrievedZone.Etag
	Assert-AreEqual $zoneName $retrievedZone.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $retrievedZone.ResourceGroupName
	Assert-AreEqual $retrievedZone.Etag $updatedZone.Etag
	Assert-AreEqual 2 $retrievedZone.Tags.Count
	Assert-Null $retrievedZone.Type

	$removed = Remove-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -PassThru -Confirm:$false

	Assert-True { $removed }

	Assert-Throws { Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName }
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force	
}

<#
.SYNOPSIS
Tests that the zone cmdlets trim the terminating dot from the zone name
#>
function Test-ZoneCrudTrimsDot
{
	$zoneName = Get-RandomZoneName
	$zoneNameWithDot = $zoneName + "."
    $resourceGroup = TestSetup-CreateResourceGroup
	$createdZone = New-AzPrivateDnsZone -Name $zoneNameWithDot -ResourceGroupName $resourceGroup.ResourceGroupName

	Assert-NotNull $createdZone
	Assert-AreEqual $zoneName $createdZone.Name

	$retrievedZone = Get-AzPrivateDnsZone -Name $zoneNameWithDot -ResourceGroupName $resourceGroup.ResourceGroupName

	Assert-NotNull $retrievedZone
	Assert-AreEqual $zoneName $retrievedZone.Name

	$updatedZone = Set-AzPrivateDnsZone -Name $zoneNameWithDot -ResourceGroupName $resourceGroup.ResourceGroupName -Tags @{tag1="value1";tag2="value2"}

	Assert-NotNull $updatedZone
	Assert-AreEqual $zoneName $updatedZone.Name

	$removed = Remove-AzPrivateDnsZone -Name $zoneNameWithDot -ResourceGroupName $resourceGroup.ResourceGroupName -PassThru -Confirm:$false

	Assert-True { $removed }

	Assert-Throws { Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName }
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force
}

<#
.SYNOPSIS
Zone CRUD with piping
#>
function Test-ZoneCrudWithPiping
{
	$zoneName = Get-RandomZoneName
    $createdZone = TestSetup-CreateResourceGroup | New-AzPrivateDnsZone -Name $zoneName -Tags @{tag1="value1"}

	$resourceGroupName = $createdZone.ResourceGroupName

	Assert-NotNull $createdZone
	Assert-NotNull $createdZone.Etag
	Assert-AreEqual $zoneName $createdZone.Name
	Assert-NotNull $createdZone.ResourceGroupName
	Assert-AreEqual 1 $createdZone.Tags.Count

	$updatedZone = Get-AzResourceGroup -Name $resourceGroupName | Get-AzPrivateDnsZone -Name $zoneName | Set-AzPrivateDnsZone -Tags $null

	Assert-NotNull $updatedZone
	Assert-NotNull $updatedZone.Etag
	Assert-AreEqual $zoneName $updatedZone.Name
	Assert-AreEqual $resourceGroupName $updatedZone.ResourceGroupName
	Assert-AreNotEqual $updatedZone.Etag $createdZone.Etag
	Assert-AreEqual 0 $updatedZone.Tags.Count

	$removed = Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroupName | Remove-AzPrivateDnsZone -PassThru -Confirm:$false

	Assert-True { $removed }

	Assert-Throws { Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroupName }
	Remove-AzResourceGroup -Name $ResourceGroupName -Force
}

<#
.SYNOPSIS
Tests that the zone CRUD cmdlets trim the terminating dot from the zone name when piping
#>
function Test-ZoneCrudWithPipingTrimsDot
{
	$zoneName = Get-RandomZoneName
	$zoneNameWithDot = $zoneName + "."
    $createdZone = TestSetup-CreateResourceGroup | New-AzPrivateDnsZone -Name $zoneName

	$resourceGroupName = $createdZone.ResourceGroupName

	$zoneObjectWithDot = New-Object Microsoft.Azure.Commands.PrivateDns.Models.PrivateDnsZone
	$zoneObjectWithDot.Name = $zoneNameWithDot
	$zoneObjectWithDot.ResourceGroupName = $resourceGroupName

	$updatedZone = $zoneObjectWithDot | Set-AzPrivateDnsZone -Overwrite

	Assert-NotNull $updatedZone
	Assert-AreEqual $zoneName $updatedZone.Name

	$removed = $zoneObjectWithDot | Remove-AzPrivateDnsZone -Overwrite -PassThru -Confirm:$false

	Assert-True { $removed }

	Assert-Throws { Get-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroupName }
	Remove-AzResourceGroup -Name $resourceGroupName -Force
}

<#
.SYNOPSIS
Test zone creation when a zone already exists with the same name and resource group.
#>
function Test-ZoneNewAlreadyExists
{
	$zoneName = Get-RandomZoneName
    $createdZone = TestSetup-CreateResourceGroup | New-AzPrivateDnsZone -Name $zoneName
	$resourceGroupName = $createdZone.ResourceGroupName
	Assert-NotNull $createdZone

	$message = [System.String]::Format("*The Zone {0} exists already and hence cannot be created again*", $zoneName);
	Assert-ThrowsLike { New-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroupName } $message

	$createdZone | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	Remove-AzResourceGroup -Name $resourceGroupName -Force
}

<#
.SYNOPSIS
Test zone update when etags mismatch
#>
function Test-ZoneSetEtagMismatch
{
	$zoneName = Get-RandomZoneName
    $createdZone = TestSetup-CreateResourceGroup | New-AzPrivateDnsZone -Name $zoneName
	$originalEtag = $createdZone.Etag
	$createdZone.Etag = "gibberish"

	$resourceGroupName = $createdZone.ResourceGroupName
	$message = [System.String]::Format("*The Zone {0} has been modified (etag mismatch)*", $zoneName);
	Assert-ThrowsLike { $createdZone | Set-AzPrivateDnsZone } $message

	$updatedZone = $createdZone | Set-AzPrivateDnsZone -Overwrite

	Assert-AreNotEqual "gibberish" $updatedZone.Etag
	Assert-AreNotEqual $createdZone.Etag $updatedZone.Etag

	$updatedZone | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	Remove-AzResourceGroup -Name $resourceGroupName -Force
}

<#
.SYNOPSIS
Test zone update using resource id parameter set
#>
function Test-ZoneSetUsingResourceId
{
	$zoneName = Get-RandomZoneName
    $resourceGroup = TestSetup-CreateResourceGroup
	$createdZone = New-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Tags @{tag1="value1"}
	$updatedZone = $createdZone.ResourceId | Set-AzPrivateDnsZone -Tags @{tag1="value1";tag2="value2"}
	
	Assert-NotNull $updatedZone
	Assert-NotNull $updatedZone.Etag
	Assert-AreEqual $zoneName $updatedZone.Name
	Assert-AreEqual $resourceGroup.ResourceGroupName $updatedZone.ResourceGroupName
	Assert-AreNotEqual $updatedZone.Etag $createdZone.Etag
	Assert-AreEqual 2 $updatedZone.Tags.Count
	Assert-Null $updatedZone.Type

	$updatedZone | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	Remove-AzResourceGroup -Name $resourceGroupName -Force
}

<#
.SYNOPSIS
Test zone delete using resource id parameter set
#>
function Test-ZoneRemoveUsingResourceId
{
	$zoneName = Get-RandomZoneName
    $resourceGroup = TestSetup-CreateResourceGroup
	$createdZone = New-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Tags @{tag1="value1"}
	
	$createdZone.ResourceId | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	Remove-AzResourceGroup -Name $resourceGroupName -Force
}

<#
.SYNOPSIS
Test set operation when zone doesn't exist
#>
function Test-ZoneSetNotFound
{
	$zoneName = Get-RandomZoneName
    $resourceGroup = TestSetup-CreateResourceGroup

	Assert-ThrowsLike { Set-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName }  "*was not found*";
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force
}

<#
.SYNOPSIS
Test zone remove when etag doesn't match.
#>
function Test-ZoneRemoveEtagMismatch
{
	$zoneName = Get-RandomZoneName
    $createdZone = TestSetup-CreateResourceGroup | New-AzPrivateDnsZone -Name $zoneName
	$originalEtag = $createdZone.Etag
	$createdZone.Etag = "gibberish"

	$resourceGroupName = $createdZone.ResourceGroupName
	$message = [System.String]::Format("*The Zone {0} has been modified (etag mismatch)*", $zoneName);
	Assert-ThrowsLike { $createdZone | Remove-AzPrivateDnsZone -Confirm:$false } $message

	$removed = $createdZone | Remove-AzPrivateDnsZone -Overwrite -Confirm:$false -PassThru

	Assert-True { $removed }
	Remove-AzResourceGroup -Name $resourceGroupName -Force
}

<#
.SYNOPSIS
Test zone remove when zone doesn't exist
#>
function Test-ZoneRemoveNonExisting
{
	$zoneName = Get-RandomZoneName
    $resourceGroup = TestSetup-CreateResourceGroup

	$removed = Remove-AzPrivateDnsZone -Name $zoneName -ResourceGroupName $resourceGroup.ResourceGroupName -Confirm:$false -PassThru
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force
}

<#
.SYNOPSIS
Zone List
#>
function Test-ZoneList
{
	$zoneName1 = Get-RandomZoneName
	$zoneName2 = $zoneName1 + "A"
	Write-Debug $zoneName1
	Write-Debug $zoneName2
	$resourceGroup = TestSetup-CreateResourceGroup

	$createdZone1 = $resourceGroup | New-AzPrivateDnsZone -Name $zoneName1 -Tags @{tag1="value1"}
	$createdZone2 = $resourceGroup | New-AzPrivateDnsZone -Name $zoneName2

	$result = Get-AzPrivateDnsZone -ResourceGroupName $resourceGroup.ResourceGroupName

	Assert-AreEqual 2 $result.Count

	Assert-AreEqual $createdZone1.Etag $result[0].Etag
	Assert-AreEqual $createdZone1.Name $result[0].Name
	Assert-NotNull $resourceGroup.ResourceGroupName $result[0].ResourceGroupName
	Assert-AreEqual 1 $result[0].Tags.Count

	Assert-AreEqual $createdZone2.Etag $result[1].Etag
	Assert-AreEqual $createdZone2.Name $result[1].Name
	Assert-NotNull $resourceGroup.ResourceGroupName $result[1].ResourceGroupName
	Assert-AreEqual 0 $result[1].Tags.Count

	$result | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force
}

function Test-ZoneListSubscription
{
	$zoneName1 = Get-RandomZoneName
	$zoneName2 = $zoneName1 + "A"
	$resourceGroup = TestSetup-CreateResourceGroup
    $createdZone1 = $resourceGroup | New-AzPrivateDnsZone -Name $zoneName1 -Tags @{tag1="value1"}
	$createdZone2 = $resourceGroup | New-AzPrivateDnsZone -Name $zoneName2

	$result = Get-AzPrivateDnsZone

	Assert-True   { $result.Count -ge 2 }

	$createdZone1 | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	$createdZone2 | Remove-AzPrivateDnsZone -PassThru -Confirm:$false
	Remove-AzResourceGroup -Name $resourceGroup.ResourceGroupName -Force
}

