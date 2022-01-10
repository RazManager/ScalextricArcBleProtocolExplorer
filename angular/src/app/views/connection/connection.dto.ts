export enum BluetoothConnectionStateType {
    Disabled = "Disabled",
    Enabled = "Enabled",
    Discovering = "Discovering",
    Connected = "Connected",
    Initialized = "Initialized"
}


export class ConnectionDto {
    public connect!: boolean;
    public modelNumber!: string | null;
    public bluetoothConnectionState!: BluetoothConnectionStateType;
    public bluetoothProperties!: BluetoothPropertyDto[];
}


export class BluetoothPropertyDto {
    public key!: string;
    public value!: string | null;
}