namespace FileValid.Model;

public class Config
{
    public string? AllFilesDirectory { get; set; }
    public string? ValidFilesDirectory { get; set; }
    public int MinHeightPx { get; set; }
    public int MinWidthPx { get; set; }
    public int MinByteFileSize { get; set; }
}
