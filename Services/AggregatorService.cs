using NetworkInsight.Model;
using System.Data;
using System.Data.Odbc;

namespace NetworkInsight.Services
{
    public class AggregatorService
    {
        private readonly string? _connectionString;
        public AggregatorService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnectionString");
        }


        public void Aggregate()
        {

            string HourlyAggregateScript = "INSERT INTO Agg_Hourly(Time,NeAlias, NeType, RFInputPower,MaxRxLevel,RSL_Deviation) select  date_trunc('hour',rp.Time) as Time,rf.NeAlias,rf.NeType, Max(rp.MaxRxLevel) as MaxRxLevel, Max(rf.RFInputPower) as RFInputPower, abs(Max(rp.MaxRxLevel)) - abs(Max(rf.RFInputPower)) as RSL_DEVIATION from  RF_Input rf Inner JOIN Radio_Power rp on rf.NeAlias=rp.NeAlias AND rf.NeType=rp.NeType group by date_trunc('hour',rp.Time),rf.NeAlias,rf.NeType;";


            string DailyAggregateScript = "INSERT INTO Agg_Daily(Time,NeAlias,NeType,RFInputPower,MaxRxLevel,RSL_Deviation) select  date_trunc('Day',rp.Time) as Time,rf.NeAlias,    rf.NeType, Max(rp.MaxRxLevel) as MaxRxLevel, Max(rf.RFInputPower) as RFInputPower, abs(Max(rp.MaxRxLevel)) - abs(Max(rf.RFInputPower)) as RSL_DEVIATION from  RF_Input rf Inner JOIN Radio_Power rp on rf.NeAlias=rp.NeAlias AND rf.NeType=rp.NeType group by date_trunc('Day',rp.Time),rf.NeAlias,rf.NeType;";


            OdbcCommand HourlyAggregateData = new OdbcCommand(HourlyAggregateScript);
            OdbcCommand DailyAggregateData = new OdbcCommand(DailyAggregateScript);

            using (OdbcConnection conn = new OdbcConnection(_connectionString))
            {
                HourlyAggregateData.Connection = conn;
                conn.Open();
                HourlyAggregateData.ExecuteNonQuery();
                DailyAggregateData.Connection = conn;
                DailyAggregateData.ExecuteNonQuery();
                conn.Close();
            }


        }

        public List<AggregatedField> GetKpiData(string globalFilterValues, string dateTimeFilterValue)
        {
            try
            {
                if (string.IsNullOrEmpty(globalFilterValues) || (globalFilterValues != "NeType" && globalFilterValues != "NeAlias"))
                {
                    return null;
                }
                string groupByColumn = globalFilterValues == "NeType" ? "NeType" : "NeAlias";
                // string startDate = "2020-03-11 01:00:00";
                //string endDate = "2020-03-11 02:00:00";
                string query = $"SELECT Time, {groupByColumn}, Max(RFInputPower) as RFInputPower, Max(MaxRxLevel) as MaxRxLevel, Max(RSL_Deviation) as RSL_Deviation FROM Agg_{dateTimeFilterValue} group by 1,2; ";


                using OdbcConnection connection = new OdbcConnection(_connectionString);
                connection.Open();

                using OdbcCommand command = new OdbcCommand(query, connection);
                using OdbcDataAdapter adapter = new OdbcDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                var result = new List<AggregatedField>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var aggregatedField = new AggregatedField
                    {
                        Time = (DateTime)row["Time"],
                        NeType = (groupByColumn == "NeType" && row.Table.Columns.Contains("NeType")) ? row["NeType"].ToString() : string.Empty,
                        NeAlias = (groupByColumn == "NeAlias" && row.Table.Columns.Contains("NeAlias")) ? row["NeAlias"].ToString() : string.Empty,
                        RFInputPower = Convert.ToDouble(row["RFInputPower"]),
                        MaxRxLevel = Convert.ToDouble(row["MaxRxLevel"]),
                        RSL_Deviation = Convert.ToDouble(row["RSL_Deviation"])
                    };
                    result.Add(aggregatedField);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
