export class SystemInformationDto {
    public hardwareModel!: string | null;
    public softwareAssemblyVersion!: string | null;
    public softwareSnapVersion!: string | null;
    public softwareDotNetVersion!: string;
    public softwareOsVersion!: string;
    public networkIpAddresses!: string;
}