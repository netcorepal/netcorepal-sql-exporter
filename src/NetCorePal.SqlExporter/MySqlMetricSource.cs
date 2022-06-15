using Prometheus;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Dapper;
using MySqlConnector;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Linq;
using System.Collections.Generic;
namespace NetCorePal.SqlExporter
{
    public class MySqlMetricSource : IMetricSource
    {
        IOptions<MySqlMetricSourceOptions> options;
        ILogger logger;

        public MySqlMetricSource(IOptionsSnapshot<MySqlMetricSourceOptions> options, ILogger<MySqlMetricSource> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        public void Load(MetricFactory metricFactory)
        {
            foreach (var database in options.Value.Databases)
            {
                foreach (var cmd in database.Value.Commands)
                {
                    DataTable table;
                    try
                    {
                        table = LoadData(database.Value.ConnectionString, cmd.Command);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "从{name}加载数据出错,Sql: {sql}", database.Value.Name, cmd.Command);
                        continue;

                    }
                    if (table.Rows.Count == 0)
                    {
                        logger.LogWarning("从{name}加载到0条数据,Sql: {sql}", database.Value.Name, cmd.Command);
                        continue;
                    }
                    var labelNames = GetlabelNames(table.Columns);
                    var gauge = metricFactory.CreateGauge(cmd.MetricName, cmd.MetricName, labelNames);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        try
                        {
                            var row = table.Rows[i];
                            var labelValues = new string[labelNames.Length];
                            for (int j = 0; j < labelNames.Length; j++)
                            {
                                labelValues[j] = row[labelNames[j]].ToStringOrEmpty();

                            }
                            gauge.WithLabels(labelValues).Set(Convert.ToDouble(row["Value"]));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "转换指标数据时出错");
                        }
                    }
                }
            }
        }

        string[] GetlabelNames(DataColumnCollection columns)
        {
            var list = new List<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                if (!"value".Equals(columns[i].ColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(columns[i].ColumnName);
                }
            }
            return list.ToArray();
        }


        DataTable LoadData(string connectionString, string commandText)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                DataTable table = new DataTable();
                var reader = connection.ExecuteReader(commandText);
                table.Load(reader);
                return table;
            }
        }
    }
}
