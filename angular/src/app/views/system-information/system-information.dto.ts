export class SystemInformationDto {
    public hardwareModel!: string | null;
    public softwareOsVersion!: string;
    public softwareSnapVersion!: string | null;
    public networkIpAddresses!: string;
}