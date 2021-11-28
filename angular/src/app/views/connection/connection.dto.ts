export enum ConnectionStateType {
    Disabled = "Disabled",
    Enabled = "Enabled",
    Discovering = "Discovering",
    Connected = "Connected",
    Initialized = "Initialized"
}


export class ConnectionDto {
    public state!: ConnectionStateType;
}