using Spectre.Console;

namespace SpectreConsoleSample;

public class PromptSample : ISample
{
    public PromptSample()
    {
        
    }

    public void Run()
    {
        if (!AnsiConsole.Confirm("Run prompt example?"))
        {
            AnsiConsole.MarkupLine("Ok... :(");
            return;
        }

        #region [simple]

        var name = AnsiConsole.Ask<string>("What's your [green]name[/]?");
        AnsiConsole.WriteLine($"name is {name}");

        #endregion

        #region [select]

        var favorites = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Title("What are your [green]favorite fruits[/]?")
                .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                .InstructionsText("[grey](Press [blue][/] to toggle a fruit, [green][/] to accept)[/]")
                .AddChoiceGroup("Berries", new[]
                {
                    "Blackcurrant", "Blueberry", "Cloudberry",
                    "Elderberry", "Honeyberry", "Mulberry"
                })
                .AddChoices(new[]
                {
                    "Apple", "Apricot", "Avocado", "Banana",
                    "Cherry", "Cocunut", "Date", "Dragonfruit", "Durian",
                    "Egg plant",  "Fig", "Grape", "Guava",
                    "Jackfruit", "Jambul", "Kiwano", "Kiwifruit", "Lime", "Lylo",
                    "Lychee", "Melon", "Nectarine", "Orange", "Olive"
                }));

        var fruit = favorites.Count == 1 ? favorites[0] : null;
        if (string.IsNullOrWhiteSpace(fruit))
        {
            fruit = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Ok, but if you could only choose [green]one[/]?")
                    .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                    .AddChoices(favorites));
        }

        AnsiConsole.MarkupLine("Your selected: [yellow]{0}[/]", fruit);
        
        #endregion

        #region [validate]

        var result = AnsiConsole.Prompt(
            new TextPrompt<int>("How [green]old[/] are you?")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]That's not a valid age[/]")
                .Validate(age =>
                {
                    return age switch
                    {
                        <= 0 => ValidationResult.Error("[red]You must at least be 1 years old[/]"),
                        >= 123 => ValidationResult.Error("[red]You must be younger than the oldest person alive[/]"),
                        _ => ValidationResult.Success(),
                    };
                }));
        
        AnsiConsole.WriteLine(result);

        #endregion

        #region [password]

        var password =  AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret());
        
        AnsiConsole.WriteLine(password);

        #endregion

        #region [optional]

        var color = AnsiConsole.Prompt(
            new TextPrompt<string>("[grey][[Optional]][/] What is your [green]favorite color[/]?")
                .AllowEmpty());

        AnsiConsole.WriteLine(color);
        #endregion
    }
}