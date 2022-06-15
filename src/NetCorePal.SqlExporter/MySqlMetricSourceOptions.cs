using System;
using System.Collections.Generic;
using System.Text;

namespace NetCorePal.SqlExporter
{
    public class MySqlMetricSourceOptions
    {
        public Dictionary<string, MySqlMetricSourceOptionDatabase> Databases { get; set; }
    }


    public class MySqlMetricSourceOptionDatabase
    {
        public string Name { get; set; }
        public MySqlMetricSourceOptionCommand[] Commands { get; set; }
        public string ConnectionString { get; set; }
        public string Tags { get; set; }

    }

    public class MySqlMetricSourceOptionCommand
    {
        public string MetricName { get; set; }
        public string Command { get; set; }

    }
}
