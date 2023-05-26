#!/bin/bash

screen -S Zune.Net.Catalog -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.Catalog && dotnet run --urls="http://0.0.0.0:8001;https://0.0.0.0:8801"'
screen -S Zune.Net.Commerce -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.Commerce && dotnet run --urls="http://0.0.0.0:8002;https://0.0.0.0:8802"'
screen -S Zune.Net.Catalog.Image -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.Catalog.Image && dotnet run --urls="http://0.0.0.0:8003;https://0.0.0.0:8803"'
screen -S Zune.Net.SocialApi -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.SocialApi && dotnet run --urls="http://0.0.0.0:8004;https://0.0.0.0:8804"'
screen -S Zune.Net.Mix -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.Mix && dotnet run --urls="http://0.0.0.0:8005;https://0.0.0.0:8805"'
screen -S Zune.Net.Tiles -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.Tiles && dotnet run --urls="http://0.0.0.0:8006;https://0.0.0.0:8806"'
screen -S Zune.Net.Login -dm bash -c 'cd /opt/ZuneNetApi/Zune.Net.Login && dotnet run --urls="http://0.0.0.0:8007;https://0.0.0.0:8807"'
