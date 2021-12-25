export class SystemInformationDto {
    public hardwareModel!: string | null;
    public hardwareProcessor!: string | null;
    public softwareAssemblyVersion!: string | null;
    public softwareSnapVersion!: string | null;
    public softwareDotNetVersion!: string;
    public softwareOsVersion!: string;
    public softwareOsReleaseVersion!: string | null;
    public networkIpAddresses!: string;
}