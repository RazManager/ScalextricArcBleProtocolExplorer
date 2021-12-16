using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.CpuInfo;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/system-information")]
    public class SystemInformationController : ControllerBase
    {
        private readonly ICpuInfoService _cpuInfoService;


        public SystemInformationController(ICpuInfoService cpuInfoService)
        {
            _cpuInfoService = cpuInfoService;
        }


        [HttpGet]
        public SystemInformationDto Get()
        {
            return new SystemInformationDto
            {
                HardwareModel = _cpuInfoService.CpuInfo.Model,
                SoftwareAssemblyVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(),
                SoftwareSnapVersion = Environment.GetEnvironmentVariable("SNAP_VERSION"),
                SoftwareDotNetVersion = Environment.Version.ToString(),
                SoftwareOsVersion = $"{Environment.OSVersion} ({(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")})",
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

        public string? SoftwareAssemblyVersion { get; set; } = null!;

        public string? SoftwareSnapVersion { get; set; }

        [Required]
        public string SoftwareDotNetVersion { get; set; } = null!;

        [Required]
        public string SoftwareOsVersion { get; set; } = null!;

        [Required]
        public string NetworkIpAddresses { get; set; } = null!;
    }
}
