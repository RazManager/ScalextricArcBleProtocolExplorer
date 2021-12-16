dotnet publish dotnet -c Debug --self-contained true -r linux-x64
dotnet publish dotnet -c Debug --self-contained true -r linux-arm64
dotnet publish dotnet -c Debug --self-contained true -r linux-arm
tar -czvf scalextric-arc-ble-protocol-explorer-linux-x64.tar.gz --directory=dotnet/bin/Debug/net6.0/linux-x64/publish .
tar -czvf scalextric-arc-ble-protocol-explorer-linux-arm64.tar.gz --directory=dotnet/bin/Debug/net6.0/linux-arm64/publish .
tar -czvf scalextric-arc-ble-protocol-explorer-linux-arm.tar.gz --directory=dotnet/bin/Debug/net6.0/linux-arm/publish .
snapcraft clean --use-lxd
snapcraft --use-lxd
