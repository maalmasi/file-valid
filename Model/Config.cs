namespace FileValid.Model;

public class Config
{
    public required string BaseDirectory { get; set; }
    public required string SubDirectory { get; set; }
    public string? AllFilesDirectory { get; set; }
    public string? ValidFilesDirectory { get; set; }
    public int MinHeightPx { get; set; }
    public int MinWidthPx { get; set; }
    public int MinByteFileSize { get; set; }
}
