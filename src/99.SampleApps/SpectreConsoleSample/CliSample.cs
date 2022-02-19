using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using eXtensionSharp;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;
using ValidationResult = Spectre.Console.ValidationResult;

namespace SpectreConsoleSample;

internal class FileSizeCommand : Command<FileSizeCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Required]
        [Description("찾을 경로입니다. 기본 경로는 현재 경로입니다.")]
        [CommandArgument(0, "[searchPath]")]
        public string? SearchPath { get; init; }
        
        [CommandOption("-p|--pattern")]
        public string? SearchPattern { get; init; }
        [CommandOption("--hidden")]
        [DefaultValue(true)]
        public bool IncludeHidden { get; init; }
        
        public class Validator : AbstractValidator<Settings>
        {
            public Validator()
            {
                RuleFor(m => m.SearchPath).NotEmpty();
            }
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var searchOptions = new EnumerationOptions
        {
            AttributesToSkip = settings.IncludeHidden
                ? FileAttributes.Hidden | FileAttributes.System
                : FileAttributes.System
        };

        var searchPattern = settings.SearchPattern ?? "*.*";
        var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();
        var files = new DirectoryInfo(searchPath).GetFiles(searchPattern, searchOptions);

        var totalFileSize = files.Sum(fileInfo => fileInfo.Length);
        
        AnsiConsole.MarkupLine($"Total file size for [green]{searchPattern}[/] file in [green]{searchPath}[/]: [blue]{totalFileSize:N0}[/] bytes");
        return 0;
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        Settings.Validator validator = new Settings.Validator();
        var result = validator.Validate(settings);
        if (result.IsValid.xIsFalse())
        {
            result.Errors.ToDictionary(failure => failure.PropertyName, failure => failure.ErrorMessage);
            return ValidationResult.Error(result.Errors.xFirst().ErrorMessage);
        }
        // if (settings.SearchPath.xIsEmpty())
        // {
        //     return ValidationResult.Error($"Path is empty");
        // }
        return base.Validate(context, settings);
    }
}