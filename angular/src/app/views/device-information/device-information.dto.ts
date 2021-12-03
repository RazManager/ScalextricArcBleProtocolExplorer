export class DeviceInformationDto {
    public manufacturerName!: string | null;
    public modelNumber!: string | null;
    public hardwareRevision!: string | null;
    public firmwareRevision!: string | null;
    public softwareRevision!: string | null;
    public gattCharacteristics!: GattCharacteristic[];
}


export class GattCharacteristic {
    public uuid!: string;
    public flags!: GattCharacteristicFlag[];
}


export class GattCharacteristicFlag {
    public flag!: string;
}