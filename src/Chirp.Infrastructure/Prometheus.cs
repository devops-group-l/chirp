using System.Diagnostics;
using Hardware.Info;
using Prometheus;

namespace Chirp.Infrastructure;

public static class Prometheus
{
    public static IHardwareInfo HardwareInfo { get; set; } = new HardwareInfo();
    public static Gauge CpuGauge { get; set; } = Metrics.CreateGauge("minitwit_cpu_load_percent", "Current load of the cpu in percent.");
    public static Counter ResponseCounter { get; set; } = Metrics.CreateCounter("minitwit_http_responses_total", "The count of HTTP responses sent.");
    public static Counter PublicAccess { get; set; } = Metrics.CreateCounter("public_access", "The number of times public timeline is accessed");
    public static Histogram ReqDurationSummary { get; set; } =
        Metrics.CreateHistogram("minitwit_request_duration_miliseconds", "Request duration distribution.");
}