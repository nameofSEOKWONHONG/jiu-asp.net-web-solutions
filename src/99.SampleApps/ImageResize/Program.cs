// See https://aka.ms/new-console-template for more information

using ImageResize;

string rxtFileName = string.Empty;

#if DEBUG
rxtFileName = "D:/a.rxt";
#else 
rxtFileName = Environment.GetCommandLineArgs()[1]; 
#endif

if(rxtFileName is null) goto EXAMPLE;
goto IMPL;

EXAMPLE: {
    Console.WriteLine("example: ImageResize [rxt file name]");
    return;
}

IMPL:
Console.WriteLine("Convert Start");

var rxtReader = new RxtFileReader(rxtFileName);
var rxtSets = rxtReader.Read();
foreach (var rxtSet in rxtSets)
{
    var resizer = new ImageResizer(rxtSet.FileName);
    var convertedFilename = resizer.Convert(rxtSet.Size, rxtSet.Quality);
    Console.WriteLine($"Convert complete : {convertedFilename}");
}
