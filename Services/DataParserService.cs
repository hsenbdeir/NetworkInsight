using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class DataParserService
{
    private readonly string outputDirectory = @"C:\Users\User\Desktop\IMS\Output";
    private readonly string archiveDirectory = @"C:\Users\User\Desktop\IMS\Parsed";
    private readonly List<string> headersToSkip = new List<string> { "Position", "IdLogNum", "MeanRxLevel1m", "NodeName" };

    private enum SpecialColumns
    {
        SLOT,
        LINK,
        PORT
    }

    private readonly Dictionary<string, string> fieldsToCheck = new Dictionary<string, string>
{
    { "FailureDescription", @"^(?!-).*$" },
    { "FarEndTID","----" },
    { "Object","Unreachable Bulk FC" },
};
    bool tidExists;

    private readonly LoaderService _LoaderService;
    public DataParserService(LoaderService LoaderService)
    {
        _LoaderService = LoaderService;
    }
    public void ProcessFile(string inputFilePath)
    {
        var (header, records) = ParseCsvFile(inputFilePath/*, headersToSkip, fieldsToCheck*/);
        /*           Console.WriteLine("Bdeeirr >>>" + string.Join(", ", header));
                        foreach (var record in records)
                        {
                            foreach (var fieldName in header)
                            {
                                var value = record[fieldName];
                                Console.WriteLine($"{fieldName}: {value}");
                            }
                            Console.WriteLine();
                        }*/
        if (records.Count > 0)
        {
            WriteToCsvFile(outputDirectory, header, records, inputFilePath);
            ArchiveFile(inputFilePath);
            Console.Write(outputDirectory);
            //if(header.Find(Link))
            /*            _LoaderService.CreateDatabaseTable(header);*/
            _LoaderService.InsertDataIntoDatabaseTable(outputDirectory);

        }
        else
        {
            Console.WriteLine("No Valid Records Found");
        }

    }
    public (List<string> header, List<Dictionary<string, string>> records) ParseCsvFile(string filePath)
    {
        var header = new List<string>();
        var records = new List<Dictionary<string, string>>();

        try
        {
            using (var reader = new StreamReader(filePath))
            {
                var isFirstRow = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (isFirstRow)
                    {
                        header = values.Select(field => field.Trim()).ToList();
                        isFirstRow = false;
                    }
                    else
                    {
                        var record = new Dictionary<string, string>();
                        for (var i = 0; i < header.Count; i++)
                        {
                            if (i < values.Length)
                            {
                                record[header[i]] = values[i].Trim();
                            }
                            else
                            {
                                record[header[i]] = null;
                            }
                        }

                        records.Add(record);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing the file: {ex.Message}");
        }

        return (header, records);
    }
    public void WriteToCsvFile(string outputDirectory, List<string> header, List<Dictionary<string, string>> records, string inputFilePath)
    {
        string inputFileName = Path.GetFileNameWithoutExtension(inputFilePath);
        /*        string substringFromInputFileName = inputFilePath.Substring(6, inputFilePath.IndexOf('_')); // Adjust the substring length as needed
               Console.WriteLine("Input file Name is : " +inputFileName);*/
        string outputFileName = $"{inputFileName.Substring(0, inputFileName.IndexOf("_202"))}.csv";
        Console.WriteLine("Output file Name is : " + outputFileName);
        string filePath = Path.Combine(outputDirectory, outputFileName);
        var existingFields = new List<string>(header);

        var newFields = new Dictionary<string, Func<Dictionary<string, string>, string>>
    {
        { "LINK", record => getLink(record["Object"],SpecialColumns.LINK) },
        { "TID", record => GetTid(record["Object"]) },
        { "FarEndTID", record => GetFarEndTid(record["Object"]) },
        { "SLOT", record => getLink(record["Object"],SpecialColumns.SLOT) },
        { "PORT", record =>  getLink(record["Object"],SpecialColumns.PORT) },
        {"DATETIME_KEY",record=>getFileDate(inputFilePath).ToString("yyyy-MM-dd HH:m:ss") }
    };
        tidExists = header.Contains("TID");

        try
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {

                var filteredHeader = header
                    .Where(fieldName => !headersToSkip.Contains(fieldName))
                    .ToList();
                if (tidExists)
                {
                    newFields.Remove("TID");
                    newFields.Remove("FarEndTID");
                    newFields.Remove("LINK");
                }

                foreach (var (newField, generateFunction) in newFields)
                {
                    if (!existingFields.Contains(newField))
                    {
                        filteredHeader.Add(newField);
                    }
                }


                foreach (var fieldName in filteredHeader)
                {
                    csv.WriteField(fieldName);
                }
                csv.NextRecord();


                foreach (var record in records)
                {
                    bool skipRecord = false;

                    foreach (var (fieldToCheck, valueToFilter) in fieldsToCheck)
                    {
                        if (record.TryGetValue(fieldToCheck, out var fieldValue) && fieldValue == valueToFilter)
                        {
                            skipRecord = true;
                            break;
                        }
                    }

                    if (!skipRecord)
                    {
                        // Generate and add new fields to the record
                        foreach (var (newField, generateFunction) in newFields)
                        {
                            if (!record.ContainsKey(newField))
                            {
                                record[newField] = generateFunction(record);
                            }
                        }

                        // Write the filtered record
                        foreach (var fieldName in filteredHeader)
                        {

                            csv.WriteField(record[fieldName]);
                        }

                        csv.NextRecord();
                        if (!tidExists)
                        {
                            if (record["LINK"].Contains("+"))
                            {
                                foreach (var fieldName in filteredHeader)
                                {
                                    if (fieldName == "SLOT")
                                    {
                                        record[fieldName] = record["LINK"].Split('+')[1].Split('/')[0];
                                    }
                                    csv.WriteField(record[fieldName]);
                                }
                                csv.NextRecord();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to the output file: {ex.Message}");
        }
    }

    private void ArchiveFile(string filePath)
    {
        // Construct the destination path in the archive directory
        string fileName = Path.GetFileName(filePath);
        string archiveFilePath = Path.Combine(archiveDirectory, fileName);

        try
        {
            // Move the file to the archive directory
            File.Move(filePath, archiveFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error archiving the file: {ex.Message}");
        }
    }
    private static string GetTid(string input)
    {
        int startIndex = input.IndexOf('_', input.IndexOf('_') + 1) + 1;
        int endIndex = input.LastIndexOf('_') - 1;

        if (startIndex >= 0 && endIndex >= 0 && startIndex <= endIndex)
        {
            return input.Substring(startIndex, endIndex - startIndex + 1);
        }
        else
        {
            // Handle the case where the input doesn't meet the expected format
            return "N/A"; // or any other appropriate default value
        }
    }
    private static string GetFarEndTid(string input)
    {
        int startIndex = input.LastIndexOf('_') + 1;

        return input.Substring(startIndex);
    }
    private static string getLink(string input, SpecialColumns column)
    {
        string output = input;
        string slot = "";
        string port = "";
        string LinkOutput = "";
        bool containsUnderscore = input.Contains("_");
        if (containsUnderscore)
            output = input.Substring(0, input.IndexOf("_"));
        if (output.Contains(".")) // . exists in the middle
        {
            slot = output.Split(".")[0].Split("/")[1];
            port = output.Split(".")[1].Split("/")[0];

            LinkOutput = $"{slot}/{port}";
        }
        else if (output.Contains("+"))// + 
        {
            slot = output.Split("/")[1].Split('+')[0];
            string slot2 = output.Split("/")[1].Split('+')[1];
            port = output.Split("/")[2];

            LinkOutput = $"{slot}+{slot2}/{port}";
        }
        else
        {
            slot = output.Split("/")[1];
            port = output.Split("/")[2];

            LinkOutput = $"{slot}/{port}";
        }
        switch (column)
        {
            case SpecialColumns.SLOT:
                return slot;
            case SpecialColumns.PORT:
                return port;
            case SpecialColumns.LINK:
            default:
                return LinkOutput;
        }

    }

    private static IList<string> GetSlots(string input)
    {
        string[] values = input.Split('/');

        IList<string> slots = new List<string>();

        if (values.Length >= 4)
        {
            slots.Add(values[1]);

            if (values.Length == 5)
            {
                slots.Add(values[2]);
            }
        }
        else
        {
            // If there is a dot in the middle, extract the value before the dot as the slot
            int indexOfDot = values[1].IndexOf('.');
            if (indexOfDot >= 0)
            {
                slots.Add(values[1].Split('.')[0]);
            }
            else
            {
                // No dot, add the first value as the slot
                slots.Add(values[0]);
            }
        }

        return slots;
    }
    private static string GetPort(string input)
    {
        string[] values = input.Split('/');
        return values.Length >= 4 ? values[2] : "1";
    }
    private DateTime getFileDate(string filepath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filepath);
        string dateOfFile = fileName.Substring(fileName.Length - 15).Replace("_", " ");
        dateOfFile = dateOfFile.Insert(4, "-");
        dateOfFile = dateOfFile.Insert(7, "-");
        dateOfFile = dateOfFile.Insert(13, ":");
        dateOfFile = dateOfFile.Insert(16, ":");
        DateTime timeofFile = DateTime.Parse(dateOfFile);
        return timeofFile;
    }

}
