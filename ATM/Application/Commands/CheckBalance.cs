using Domain;
using Spectre.Console;

namespace Application.Commands
{
    internal class CheckBalance : ICommand
    {
        public string Name => "Проверить баланс";

        public void Execute(ATM ATM)
        {
            AnsiConsole.Write(new Rule("[yellow]Проверка баланса[/]").RuleStyle("grey").LeftJustified());

            AnsiConsole.MarkupLine($"Баланс: {ATM.GetBalance()}");
        }
    }
}
