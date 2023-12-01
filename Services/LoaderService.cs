
using System.Data.Odbc;

public class LoaderService
{
    private readonly IConfiguration _configuration;

    public LoaderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public void InsertDataIntoDatabaseTable(string outputDirectory)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnectionString");
        string createSQL, copySQL, createAggHourly;// createAggDaily, insertHourly, insertDaily;
        string tableName = "RF_Input";
        string tableName2 = "Radio_Power";
        string rfInputFields = "NeId Float,     Object varchar(255), Time Timestamp,     \"Interval\" varchar(255),     Direction varchar(255),     NeAlias varchar(255),     NeType varchar(255),     RFInputPower float,     TID varchar(255),         FarEndTID varchar(255),         FailureDescription varchar(255),   SLOT varchar(255),     PORT varchar(255),     DATETIME_KEY Timestamp";
        string radioPowerFields = "NeId Float, Object varchar(255), Time Timestamp,\"Interval\" varchar(255), Direction varchar(255),NeAlias varchar(255), NeType varchar(255), RxLevelBelowTS1 varchar(255),  RxLevelBelowTS2 float, MinRxLevel float, MaxRxLevel float, TxLevelAboveTS1 float, MinTxLevel float, MaxTxLevel float, FailureDescription varchar(255), LINK varchar(255) , TID varchar(255), FarEndTID varchar(255), SLOT varchar(255), PORT varchar(255) , DATETIME_KEY Timestamp";
        try
        {
            var csvFiles = Directory.GetFiles(outputDirectory, "*.csv");
            
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();


                foreach (var csvFile in csvFiles)
                {
                     if (csvFile == @"C:\Users\User\Desktop\IMS\Output\SOEM1_TN_RFInputPower.csv")
                    {
                        createSQL = $"Create table if not exists {tableName} ({rfInputFields}) SEGMENTED BY hash(NeId) ALL NODES KSAFE 1; ";
                        Console.WriteLine("RF Table created");
                        copySQL = $"COPY {tableName} FROM LOCAL '{csvFile.Replace("\\", "/")}' DELIMITER ',' direct EXCEPTIONS  '{csvFile}_exceptions.txt'";
                        Console.WriteLine("RF Table Copied");
                        createAggHourly = $"Create table if not exists '{tableName} + Hourly' ";
                    }
                    else
                    {
                        createSQL = $"Create table if not exists {tableName2} ({radioPowerFields}) SEGMENTED BY hash(NeId) ALL NODES KSAFE 1; ";
                        Console.WriteLine("Power Table created");
                        copySQL = $"COPY {tableName2} FROM LOCAL '{csvFile.Replace("\\", "/")}' DELIMITER ',' skip 1 EXCEPTIONS  '{csvFile}_exceptions.txt'";
                        Console.WriteLine("Power Table copied");
                    }
                    Console.WriteLine($"{csvFile.Replace("\\", "/")}");
                    using (OdbcCommand createCommand = new OdbcCommand(createSQL, connection))
                    {
                        createCommand.ExecuteNonQuery();
                        Console.WriteLine($"Table created successfully: {tableName}");
                    }
                    using (OdbcCommand copyCommand = new OdbcCommand(copySQL, connection))
                    {
                        copyCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Data copied successfully: {csvFile}");

                    File.Delete(csvFile);
                    Console.WriteLine($"File Deleted; {csvFile}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting data into the database: {ex.Message}");
        }
    }
}



