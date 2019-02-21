---
external help file: Microsoft.Azure.PowerShell.Cmdlets.PrivateDns.dll-Help.xml
Module Name: Az.PrivateDns
ms.assetid: B831ABE6-348C-4DD6-9295-18D23A1FDF63
online version: https://docs.microsoft.com/en-us/powershell/module/az.dns/get-azdnszone
schema: 2.0.0
---

# Get-AzPrivateDnsZone

## SYNOPSIS
Gets a Private DNS zone.

## SYNTAX

### Default (Default)
```
Get-AzPrivateDnsZone [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### ResourceGroup
```
Get-AzPrivateDnsZone [-Name <String>] -ResourceGroupName <String> [-DefaultProfile <IAzureContextContainer>]
 [<CommonParameters>]
```

## DESCRIPTION
The **Get-AzPrivateDnsZone** cmdlet gets a Private Domain Name System (DNS) zone from the specified resource group.
If you specify the *Name* parameter, a single **DnsZone** object is returned.
If you do not specify the *Name* parameter, an array containing all of the zones in the specified resource group is returned.
You can use the **PrivateDnsZone** object to update the zone, for example you can add **RecordSet** objects to it.

## EXAMPLES

### Example 1: Get a zone
```
PS C:\> $Zone = Get-AzPrivateDnsZone -ResourceGroupName "MyResourceGroup" -Name "myzone.com"
```

This example gets the Private DNS zone named myzone.com from the specified resource group, and then stores it in the $Zone variable.

### Example 2: Get all of the zones in a resource group
```
PS C:\> $Zones = Get-AzPrivateDnsZone -ResourceGroupName "MyResourceGroup"
```

This example gets all of the Private DNS zones in the specified resource group, and then stores it in the $Zones variable.

### Example 3: Get all of the zones in a subscription
```
PS C:\> $Zones = Get-AzPrivateDnsZone
```

This example gets all of the Private DNS zones in the current Azure subscription, and then stores them in the $Zones variable.

## PARAMETERS

### -DefaultProfile
The credentials, account, tenant, and subscription used for communication with azure

```yaml
Type: Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer
Parameter Sets: (All)
Aliases: AzContext, AzureRmContext, AzureCredential

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Specifies the name of the Private DNS zone to get.
If you do not specify a value for the *Name* parameter, this cmdlet gets all Private DNS zones in the specified resource group.
If you also omit the *ResourceGroupName* parameter, this cmdlet gets all Private DNS zones in the current Azure subscription.

```yaml
Type: System.String
Parameter Sets: ResourceGroup
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ResourceGroupName
Specifies the name of the resource group that contains the Private DNS zone to get.
If you do not specify the *ResourceGroupName*, then you must also omit the *Name* parameter.
In this case, this cmdlet gets all Private DNS zones in the current Azure subscription.

```yaml
Type: System.String
Parameter Sets: ResourceGroup
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### Microsoft.Azure.Commands.PrivateDns.Models.PrivateDnsZone

## NOTES

## RELATED LINKS

[New-AzPrivateDnsZone](./New-AzPrivateDnsZone.md)

[Remove-AzPrivateDnsZone](./Remove-AzPrivateDnsZone.md)

[Set-AzPrivateDnsZone](./Set-AzPrivateDnsZone.md)
