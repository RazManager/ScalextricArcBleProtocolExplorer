using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.CpuInfo;
using ScalextricArcBleProtocolExplorer.Services.OsRelease;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/system-information")]
    public class SystemInformationController : ControllerBase
    {
        private readonly ICpuInfoService _cpuInfoService;
        private readonly IOsReleaseService _osReleaseService;


        public SystemInformationController(ICpuInfoService cpuInfoService,
                                           IOsReleaseService osReleaseService)
        {
            _cpuInfoService = cpuInfoService;
            _osReleaseService = osReleaseService;
        }


        [HttpGet]
        public SystemInformationDto Get()
        {
            return new SystemInformationDto
            {
                HardwareModel = _cpuInfoService.CpuInfo.Model,
                HardwareProcessor = _cpuInfoService.CpuInfo.ModelName,
                SoftwareAssemblyVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(),
                SoftwareSnapVersion = Environment.GetEnvironmentVariable("SNAP_VERSION"),
                SoftwareDotNetVersion = Environment.Version.ToString(),
                SoftwareOsVersion = $"{Environment.OSVersion} ({(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")})",
                SoftwareOsReleaseVersion = _osReleaseService.OsRelease.PrettyName,
                NetworkIpAddresses = string.Join
                (
                    ", ",
                    Dns.GetHostAddresses(Dns.GetHostName())
                        .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(x => x.ToString())
                        .OrderBy(x => x)
                )
            };
        }
    }


    public class SystemInformationDto
    {
        public string? HardwareModel { get; set; }
        
        public string? HardwareProcessor { get; set; }

        public string? SoftwareAssemblyVersion { get; set; } = null!;

        public string? SoftwareSnapVersion { get; set; }

        [Required]
        public string SoftwareDotNetVersion { get; set; } = null!;

        [Required]
        public string SoftwareOsVersion { get; set; } = null!;

        public string? SoftwareOsReleaseVersion { get; set; }

        [Required]
        public string NetworkIpAddresses { get; set; } = null!;
    }
}
