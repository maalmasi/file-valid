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
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip };
Config config = JsonSerializer.Deserialize<Config>(json, options) ?? throw new InvalidOperationException("Failed to deserialize configuration.");

Console.WriteLine($"Processing files in: {config.AllFilesDirectory}");
Console.WriteLine($"Moving valid files to: {config.ValidFilesDirectory}");

if (!Directory.Exists(config.ValidFilesDirectory))
{
    Directory.CreateDirectory(config.ValidFilesDirectory);
}

IEnumerable<string> files = Directory.EnumerateFiles(config.AllFilesDirectory);

int totalFiles = files.Count();

int valid = 0, invalid = 0, index = 0;

Console.WriteLine("___________________________________________________________________");
Console.WriteLine($"Checking validity of {totalFiles} files.");
Console.WriteLine("___________________________________________________________________");

foreach (string file in files)
{
    if (index % 100 == 0)
    {
        Console.WriteLine($"Progress: {index} / {totalFiles} ---- Valid: {valid} ---- Invalid: {invalid}");
        Console.WriteLine("___________________________________________________________________");
    }

    index++;
    var fileInfo = new FileInfo(file);

    if (fileInfo.Length < config.MinByteFileSize)
    {
        invalid++;
        continue;
    }

    bool isValid;
    switch (fileInfo.Extension)
    {
        case ".jpg":
        case ".jpeg":
            isValid = JpegHandler.CheckValid(file, config.MinHeightPx, config.MinWidthPx);
            break;
        default:
            isValid = false;
            break;
    }

    if (isValid)
    {
        valid++;
        Directory.Move(file, $"{config.ValidFilesDirectory}\\{fileInfo.Name}");
    }
    else
    {
        invalid++;
    }
}

Console.WriteLine($"Processing complete. Valid: {valid} of {totalFiles}");