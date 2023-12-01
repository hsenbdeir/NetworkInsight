public class FileWatcherService : BackgroundService
{
    private readonly string directoryPath = @"C:\Users\User\Desktop\IMS\Input";
    private readonly string fileToWatch = "*.txt";
    private readonly DataParserService _dataParserService;
    private readonly string parsedDirectoryPath = @"C:\Users\User\Desktop\IMS\Parsed";
    //private static readonly TimeSpan FileAgeThreshold = TimeSpan.FromSeconds(5); 


    public FileWatcherService(DataParserService dataParserService)
    {
        _dataParserService = dataParserService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
            string[] files = Directory.GetFiles(directoryPath, fileToWatch);

            foreach (string filePath in files)
            {
                if (IsFileDuplicate(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"Duplicate file deleted: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting the duplicate file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"File found: {filePath}");
                    
                    _dataParserService.ProcessFile(filePath);
                }
            }

            await Task.Delay(2000, stoppingToken);
        }
    }
    private bool IsFileDuplicate(string filePath)
    {

        string parsedFilePath = Path.Combine(parsedDirectoryPath, Path.GetFileName(filePath));
        return File.Exists(parsedFilePath);
    }

}

