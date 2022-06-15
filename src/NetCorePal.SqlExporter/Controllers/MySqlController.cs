using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Metric = Prometheus.Metrics;
namespace NetCorePal.SqlExporter.Controllers;

[ApiController]
[Route("[controller]")]
public class MySqlController : ControllerBase
{
    public async Task Metrics([FromServices] MySqlMetricSource source, [FromServices] ILogger<MySqlController> logger)
    {
        Stopwatch watch = Stopwatch.StartNew();
        var r = Metric.NewCustomRegistry();
        MetricFactory f = Metric.WithCustomRegistry(r);
        r.AddBeforeCollectCallback(() =>
        {
            source.Load(f);
        });
        Response.ContentType = PrometheusConstants.ExporterContentType;
        Response.StatusCode = 200;
        await r.CollectAndExportAsTextAsync(Response.Body, HttpContext.RequestAborted);
        watch.Stop();
        logger.LogInformation("Metrics接口耗时(不包含网络传输时间):{0}ms", watch.ElapsedMilliseconds);
    }
}
