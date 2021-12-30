# Scalextric ARC BLE Protocol Explorer

Users of the [Scalextric ARC powerbases](https://uk.scalextric.com/community/advice/app-race-control) can use the official app from Scalextric for managing a slot car race. But Scalextric has also made the documentation of the Scalextric ARC BLE protocol available to anyone that themselves want to develop a similar application. The documentation can be received by sending an e-mail to <customerservice@scalextric.com>.

This little unofficial utility, the Scalextric ARC BLE Protocol Explorer, helps developers understand how the different parameters in the protocol actually works, so that it becomes easier to develop an application for the Scalextric ARC powerbases.


## Hardware and software requirements

- At least one of the Scalextric ARC powerbases; ARC ONE, ARC AIR and/or ARC PRO.
- A computer or device capable of running Linux. There are versions of “Scalextric ARC BLE Protocol Explorer” for x64/amd64, arm64 and arm/armhf. This means you can use a PC, or a Raspberry Pi 3/4/400. Using a Raspberry Pi 2 or the new Raspberry Pi Zero 2 should also work, but has not yet been verified. The first generation of Raspberry Pis and the first Raspberry Pi Zero will not work as they use the older ARMv6 processor (which is not supported by .NET).
- Your computer or device needs to support BLE (Bluetooth Low Energy). The supported Raspberry Pis mentioned above have built-in BLE support. If you have an older PC without built-in BLE support, you can get an USB BLE adapter; the ASUS BT-400 and ASUS BT-500 adapters have been found to work with Scalextric ARC BLE Protocol Explorer.
- A desktop or a server version of Linux. Recent versions of Ubuntu, Fedora and Raspberry Pi OS have been verified to work with Scalextric ARC BLE Protocol Explorer.
- Your Linux version needs to have recent version of BlueZ installed for BLE to work, as well as drivers for your BLE hardware.
- Any modern web browser on any tablet or desktop sized device on any operating system.


## Installation

“Scalextric ARC BLE Protocol Explorer” can be installed in three different ways; as a snap, manually downloading and running a release, or by downloading and running the source code.
- The easiest way to install “Scalextric ARC BLE Protocol Explorer” is by installing [scalextric-ble-protocol-explorer from the Snap Store](https://snapcraft.io/scalextric-arc-ble-protocol-explorer). If your Linux version doesn’t have snap support installed by default, the installation instructions is at the bottom of the Snap Store page (in the link above). The application will auto-update itself and start automatically each time you start up your computer/device.
- You can also manually download and unpack a file from a [GitHub release]( https://github.com/RazManager/ScalextricArcBleProtocolExplorer/releases), and then run the <code>ScalextricArcBleExplorer</code> command. You need to be comfortable with using Linux to able to do this, and you will need to handle any updates by yourself.
- If you’re a skilled .NET developer, you can download the source, install a .NET 6 SDK and run the application that way. But you’re on your own, there’s no support given for this.


## Usage

“Scalextric ARC BLE Protocol Explorer” runs as a web server on port 3301, and also connects through BLE to your Scalextric ARC powerbase.

You can use a modern web browser to access the functionality. Enter http://localhost:3301 in the web browser’s address bar if you’re accessing “Scalextric ARC BLE Protocol Explorer” from the same computer/device as where you have installed it, otherwise replace localhost with the server's IP address.


## References

- [Tmds.DBus](https://github.com/tmds/Tmds.DBus): This package provides the necessary functionality for accessing D-Bus on Linux from a .NET program. It’s then through D-Bus that BlueZ can be accessed.
- [DotNet-BlueZ](https://github.com/hashtagchris/DotNet-BlueZ): This package isn’t used by Scalextric ARC BLE Explorer, but it’s very helpful for anyone that wants to learn how-to use BLE on Linux from a .NET program.