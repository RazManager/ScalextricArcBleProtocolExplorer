export class GattCharacteristicDto {
    public uuid!: string;
    public name!: string | null;
    public value!: string | null;
    public length!: number | null;
    public flags!: GattCharacteristicFlagDto[];
}


export class GattCharacteristicFlagDto {
    public flag!: string;
}