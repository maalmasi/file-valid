using FileValid.Handler;
using FileValid.Model;
using System.Text.Json;

string configFile = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
if (!File.Exists(configFile))
{
    Console.Error.WriteLine($"Configuration file not found: {configFile}");
    return;
}

string json = await File.ReadAllTextAsync(configFile);
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip
};

Config config = JsonSerializer.Deserialize<Config>(json, options)
    ?? throw new InvalidOperationException("Failed to deserialize configuration.");

config.AllFilesDirectory = $"{config.BaseDirectory}\\{config.SubDirectory}";
config.ValidFilesDirectory = $"{config.BaseDirectory}\\{config.SubDirectory}valid";

Console.WriteLine($"Processing files in: {config.AllFilesDirectory}");
Console.WriteLine($"Moving valid files to: {config.ValidFilesDirectory}");

if (!Directory.Exists(config.ValidFilesDirectory))
{
    Directory.CreateDirectory(config.ValidFilesDirectory);
}

var files = Directory.EnumerateFiles(config.AllFilesDirectory).ToList();

int totalFiles = files.Count;

int valid = 0;
int invalid = 0;
int index = 0;

Console.WriteLine("___________________________________________________________________");
Console.WriteLine($"Checking validity of {totalFiles} files.");
Console.WriteLine("___________________________________________________________________");

var parallelOptions = new ParallelOptions
{
    MaxDegreeOfParallelism = Environment.ProcessorCount
};

Parallel.ForEach(files, parallelOptions, file =>
{
    int currentIndex = Interlocked.Increment(ref index);

    if (currentIndex % 100 == 0)
    {
        Console.WriteLine(
            $"Progress: {currentIndex} / {totalFiles} ---- Valid: {valid} ---- Invalid: {invalid}");
    }

    var fileInfo = new FileInfo(file);

    if (fileInfo.Length < config.MinByteFileSize)
    {
        Interlocked.Increment(ref invalid);
        return;
    }

    bool isValid = false;

    switch (fileInfo.Extension.ToLowerInvariant())
    {
        case ".jpg":
        case ".jpeg":
        case ".png":
            isValid = JpegHandler.CheckValid(
                file,
                config.MinHeightPx,
                config.MinWidthPx);
            break;
    }

    if (isValid)
    {
        Interlocked.Increment(ref valid);

        try
        {
            Directory.Move(
                file,
                Path.Combine(config.ValidFilesDirectory, fileInfo.Name));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed moving {file}: {ex.Message}");
        }
    }
    else
    {
        Interlocked.Increment(ref invalid);
    }
});

Console.WriteLine($"Processing complete. Valid: {valid} of {totalFiles}");