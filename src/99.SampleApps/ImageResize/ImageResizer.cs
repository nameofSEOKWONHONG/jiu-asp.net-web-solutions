using System.Net.Mime;
using SkiaSharp;

namespace ImageResize;

public class ImageResizer
{
    private readonly FileInfo _fileInfo;
    private readonly string _outFileName;
    public ImageResizer(string fileName)
    {
        _fileInfo = new FileInfo(fileName);
        _outFileName = $"{_fileInfo.Directory}/{_fileInfo.Name.Replace(_fileInfo.Extension, "")}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{_fileInfo.Extension}";
    }

    public string Convert(int size, int quality)
    {
        using (var input = File.OpenRead(_fileInfo.FullName))
        {
            using (var inputStream = new SKManagedStream(input))
            {
                using (var src = SKBitmap.Decode(inputStream))
                {
                    int w, h;
                    if (src.Width > src.Height)
                    {
                        w = size;
                        h = src.Height * size / src.Width;
                    }
                    else
                    {
                        w = src.Width * size / src.Height;
                        h = size;
                    }

                    using (var resized = src.Resize(new SKImageInfo(w, h), SKBitmapResizeMethod.Lanczos3))
                    {
                        if (resized is null) return string.Empty;
                        using (var image = SKImage.FromBitmap(resized))
                        {
                            using (var output = File.OpenWrite(_outFileName))
                            {
                                image.Encode(SKEncodedImageFormat.Jpeg, quality)
                                    .SaveTo(output);
                            }
                        }
                    }
                }
            }
        }

        return _outFileName;
    } 
}