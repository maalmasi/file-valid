namespace FileValid.Model;

public class Config
{
    public required string AllFilesDirectory { get; set; }
    public required string ValidFilesDirectory { get; set; }
    public int MinHeightPx { get; set; }
    public int MinWidthPx { get; set; }
    public int MinByteFileSize { get; set; }
}
