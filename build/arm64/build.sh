dotnet publish ../../dotnet -c Release --self-contained true -r linux-arm64
tar -czvf scalextric-arc-ble-protocol-explorer-linux-arm64.tar.gz --directory=../../dotnet/bin/Release/net6.0/linux-arm64/publish .
snapcraft clean --use-lxd
snapcraft --use-lxd
