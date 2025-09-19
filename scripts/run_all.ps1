#!/usr/bin/env pwsh

# Runs all microservices in their own PowerShell window

param([bool] $Debug = $true, [bool] $Build = $true, [String[]] $ExcludedServices = @())

$projInfoType = "tuple[String,String]"

$projInfos = @(
    New-Object $projInfoType "MetaServices", "1"
    New-Object $projInfoType "Catalog", "2"
    New-Object $projInfoType "Commerce", "3"
    New-Object $projInfoType "Catalog.Image", "4"
    New-Object $projInfoType "SocialApi", "5"
    #New-Object $projInfoType "Comments", "6"
    New-Object $projInfoType "Inbox", "7"
    New-Object $projInfoType "Mix", "8"
    #New-Object $projInfoType "Stats", "9"
    #New-Object $projInfoType "Cache", "10"
    New-Object $projInfoType "Tuners", "11"
    New-Object $projInfoType "Tiles", "12"
    New-Object $projInfoType "Login", "14"
)

if ($Debug -eq $true)
{
    $configuration = "Debug"
}
else
{
    $configuration = "Release"
}

foreach ($projInfo in $projInfos)
{
    $projName = $projInfo.Item1
    $hostNum = $projInfo.Item2

    if ($ExcludedServices -Contains $projName)
    {
        continue
    }

    $command = "dotnet run -c " + $configuration + " --project Zune.Net." + $projName

    if ($Build -eq $false)
    {
        $command = $command + " --no-build"
    }
    
    echo $command
    
    Start-Process powershell.exe -ArgumentList "-noexit", ("-command " + $command)
}
