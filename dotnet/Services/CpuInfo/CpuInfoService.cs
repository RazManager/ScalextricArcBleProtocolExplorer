using Microsoft.Extensions.Logging;


namespace ScalextricArcBleProtocolExplorer.Services.CpuInfo
{
    public class CpuInfoService : ICpuInfoService
    {
        public CpuInfo CpuInfo { get; private set; }


        public CpuInfoService(ILogger<CpuInfoService> logger)
        {
            CpuInfo = new CpuInfo();

            try
            {
                var cpuInfoLines = System.IO.File.ReadAllLines("/proc/cpuinfo");

                foreach (var cpuInfoLine in cpuInfoLines)
                {
                    var pos = cpuInfoLine.IndexOf(": ");
                    if (pos > 0)
                    {
                        var key = cpuInfoLine.Substring(0, pos).Replace("\t", "");
                        var value = cpuInfoLine.Substring(pos + 2).Trim();

                        switch (key)
                        {
                            case "Model":
                                CpuInfo.Model = value;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                logger.LogError($"Could not read /proc/cpuinfo: {exception.Message}");
            }
        }
    }


    public class CpuInfo
    {
        public string? Model { get; set; }
    }
}
