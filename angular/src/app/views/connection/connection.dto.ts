export enum BluetoothConnectionStateType {
    Disabled = "Disabled",
    Enabled = "Enabled",
    Discovering = "Discovering",
    Connected = "Connected",
    Initialized = "Initialized"
}


export class ConnectionDto {
    public connect!: boolean;
    public bluetoothConnectionState!: BluetoothConnectionStateType;
    public rssi!: number | null;
}