using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.CpuInfo;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/system-information")]
    public class SystemInformationController : ControllerBase
    {
        private readonly ICpuInfoService _cpuInfoService;
        private readonly HttpClient _httpClient;

        public SystemInformationController(ICpuInfoService cpuInfoService,
                                           HttpClient httpClient)
        {
            _cpuInfoService = cpuInfoService;
            _httpClient = httpClient;
        }


        [HttpGet]
        public async Task<SystemInformationDto> GetAsync()
        {
            var result = new SystemInformationDto
            {
                HardwareModel = _cpuInfoService.CpuInfo.Model,

                SoftwareVersion = $"{Environment.Version} {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}",

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

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SNAP")))
            {
                var httpResponseMessage = await _httpClient.GetAsync("http://localhost/v2/system-info");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    result.Snap = JsonSerializer.Deserialize<SnapDto>(content);
                }
                else
                {
                    Console.WriteLine(httpResponseMessage.StatusCode);
                }

                //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                //assembly.
                //FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
                //return fileVersion.FileVersion;

            }

            return result;
        }
    }


    public class SystemInformationDto
    {
        public string? HardwareModel { get; set; }

        [Required]
        public string SoftwareVersion { get; set; } = null!;

        [Required]
        public string SoftwareOsVersion { get; set; } = null!;

        public string? SoftwareSnapVersion { get; set; }

        [Required]
        public string NetworkIpAddresses { get; set; } = null!;

        public SnapDto? Snap { get; set; }
    }


    public class SnapDto
    {
        [Required]
        public string series { get; set; } = null!;

        [Required]
        public string version { get; set; } = null!;

        [Required]
        [JsonPropertyName("os-release")]
        public SnapOsReleaseDto osRelease { get; set; } = new();

        [Required]
        [JsonPropertyName("kernel-version")]
        public string kernelVersion { get; set; } = null!;

        [Required]
        public SnapReleaseDto release { get; set; } = new();

    }


    public class SnapOsReleaseDto
    {
        [Required]
        public string id { get; set; } = null!;

        [Required]
        [JsonPropertyName("version-id")]
        public string versionId { get; set; } = null!;

    }


    public class SnapReleaseDto
    {
        [Required]
        public DateTimeOffset last { get; set; }

        [Required]
        public DateTimeOffset next { get; set; }

        [Required]
        public string timer { get; set; } = null!;
    }
}
