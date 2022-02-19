// See https://aka.ms/new-console-template for more information


using eXtensionSharp;
using Spectre.Console.Cli;
using SpectreConsoleSample;

var samples = new List<ISample>();
samples.Add(new MarkupSample());
samples.Add(new WidgetSample());
samples.Add(new PromptSample());

samples.xForEach(sample =>
{
    sample.Run();
});

var app = new CommandApp<FileSizeCommand>();
return app.Run(args);