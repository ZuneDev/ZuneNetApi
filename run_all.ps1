param([bool] $Debug = $true)

$projInfoType = "tuple[String,String]"

$projInfos = @(
    # New-Object $projInfoType "Zune.Net.MetaServices", "1"
    New-Object $projInfoType "Zune.Net.Catalog", "2"
    New-Object $projInfoType "Zune.Net.Commerce", "3"
    New-Object $projInfoType "Zune.Net.Catalog.Image", "4"
    New-Object $projInfoType "Zune.Net.SocialApi", "5"
    # New-Object $projInfoType "Zune.Net.Comments", "6"
    # New-Object $projInfoType "Zune.Net.Inbox", "7"
    New-Object $projInfoType "Zune.Net.Mix", "8"
    # New-Object $projInfoType "Zune.Net.Stats", "9"
    # New-Object $projInfoType "Zune.Net.Cache", "10"
    # New-Object $projInfoType "Zune.Net.Tuners", "11"
    New-Object $projInfoType "Zune.Net.Tiles", "12"
    New-Object $projInfoType "Zune.Net.Login", "14"
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
    
    $exePath = ".\" + $projName + "\bin\" + $configuration + "\net6.0\" + $projName + ".exe"

    $command = '$env:ASPNETCORE_URLS=\"http://127.0.0.' + $hostNum + ':80;https://127.0.0.' + $hostNum + ':443\"; echo "Starting ' + $projName + '..."; ' + $exePath
    echo $command
    
    Start-Process powershell.exe -ArgumentList "-noexit", ("-command " + $command)
}
