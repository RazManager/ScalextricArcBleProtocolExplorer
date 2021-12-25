using Microsoft.Extensions.Logging;


namespace ScalextricArcBleProtocolExplorer.Services.OsRelease
{
    public class OsReleaseService : IOsReleaseService
    {
        public OsRelease OsRelease { get; private set; }


        public OsReleaseService(ILogger<OsReleaseService> logger)
        {
            OsRelease = new OsRelease();

            try
            {
                var osReleaseLines = System.IO.File.ReadAllLines("/etc/os-release");

                foreach (var osReleaseLine in osReleaseLines)
                {
                    var pos = osReleaseLine.IndexOf("=");
                    if (pos > 0)
                    {
                        var key = osReleaseLine.Substring(0, pos);
                        var value = osReleaseLine.Substring(pos + 1).Replace("\"", "");

                        System.Console.WriteLine($"{key } {value}");

                        switch (key)
                        {
                            case "PRETTY_NAME":
                                OsRelease.PrettyName = value;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                logger.LogError(exception, $"Could not read /etc/os-release: {exception.Message}");
            }
        }
    }


    public class OsRelease
    {
        public string? PrettyName { get; set; }
    }
}
