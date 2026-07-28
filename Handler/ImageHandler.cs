using ImageMagick;

namespace FileValid.Handler;

public static class ImageHandler
{
    public static bool CheckValid(string file, int minHeightPx, int minWidthPx)
    {
        try
        {
            using var imageFromFile = new MagickImage(file);

            if (imageFromFile.Height < minHeightPx || imageFromFile.Width < minWidthPx)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
