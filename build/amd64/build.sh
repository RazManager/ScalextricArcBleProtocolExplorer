dotnet publish dotnet -c Release --self-contained true -r linux-x64
tar -czvf scalextric-arc-ble-protocol-explorer-linux-x64.tar.gz --directory=../dotnet/bin/Release/net6.0/linux-x64/publish .
snapcraft clean --use-lxd
snapcraft --use-lxd
