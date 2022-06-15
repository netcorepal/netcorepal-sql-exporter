using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCorePal.SqlExporter
{
    public interface IMetricSource
    {
        void Load(MetricFactory metricFactory);
    }
}
