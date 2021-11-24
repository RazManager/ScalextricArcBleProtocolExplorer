using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.CpuInfo;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly ICpuInfoService _cpuInfoService;


        public SystemController(ICpuInfoService cpuInfoService)
        {
            _cpuInfoService = cpuInfoService;
        }


        [HttpGet]
        public SystemDto Get()
        {
            var result = new SystemDto
            {
                HardwareModel = _cpuInfoService.CpuInfo.Model,

                SoftwareOsVersion = $"{Environment.OSVersion} ({(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")})",
                SoftwareSnapVersion = Environment.GetEnvironmentVariable("SNAP_VERSION"),

                NetworkIpAddresses = string.Join
                (
                    ", ",
                    Dns.GetHostAddresses(Dns.GetHostName())
                        .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(x => x.ToString())
                        .OrderBy(x => x)
                )
            };

            return result;
        }
    }


    public class SystemDto
    {
        public string? HardwareModel { get; set; }

        [Required]
        public string SoftwareOsVersion { get; set; } = null!;

        public string? SoftwareSnapVersion { get; set; }

        [Required]
        public string NetworkIpAddresses { get; set; } = null!;
    }
}
