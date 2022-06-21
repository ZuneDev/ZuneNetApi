param([bool] $Debug = $true)

$projInfos = @(
    New-Object "tuple[String,String]" "Zune.Net.MetaServices", "1"
    New-Object "tuple[String,String]" "Zune.Net.Catalog", "2"
    # New-Object "tuple[String,String]" "Zune.Net.Commerce", "3"
    # New-Object "tuple[String,String]" "Zune.Net.ImageCatalog", "4"
    # New-Object "tuple[String,String]" "Zune.Net.SocialApi", "5"
    # New-Object "tuple[String,String]" "Zune.Net.Comments", "6"
    # New-Object "tuple[String,String]" "Zune.Net.Inbox", "7"
    New-Object "tuple[String,String]" "Zune.Net.Mix", "8"
    # New-Object "tuple[String,String]" "Zune.Net.Stats", "9"
    # New-Object "tuple[String,String]" "Zune.Net.Cache", "10"
    # New-Object "tuple[String,String]" "Zune.Net.Tuners", "11"
    # New-Object "tuple[String,String]" "Zune.Net.Tiles", "12"
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

    $command = '$env:ASPNETCORE_URLS=\"http://127.0.0.'+ $hostNum + ':80;https://127.0.0.'+ $hostNum + ':443\"; ' + $exePath
    echo $command
    
    Start-Process powershell.exe -ArgumentList "-noexit", ("-command " + $command)
}
