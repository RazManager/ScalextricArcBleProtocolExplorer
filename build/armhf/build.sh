dotnet publish ../../dotnet -c Release --self-contained true -r linux-arm
tar -czvf scalextric-arc-ble-protocol-explorer-linux-arm.tar.gz --directory=../../dotnet/bin/Release/net6.0/linux-arm/publish .
snapcraft clean
snapcraft