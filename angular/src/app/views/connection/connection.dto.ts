export enum ConnectionStateType {
    Disabled = "Disabled",
    Enabled = "Enabled",
    Discovering = "Discovering",
    Connected = "Connected",
    Initialized = "Initialized"
}


export class ConnectionDto {
    public state!: ConnectionStateType;
    public gattCharacteristicFlags!: GattCharacteristicFlag[];
}


export class GattCharacteristicFlag {
    public uuid!: string;
    public flag!: string;
}
