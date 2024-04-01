using Domain;
using Spectre.Console;

namespace Application.Commands
{
    internal class ShowBanknotes : ICommand
    {
        public string Name => "Показать содержимое банкомата";

        public void Execute(ATM ATM)
        {
            AnsiConsole.Write(new Rule("[yellow]Содержимое банкомата[/]").RuleStyle("grey").LeftJustified());

            var table = new Table().AddColumns("[grey]Номинал[/]", "[grey]Количество[/]").RoundedBorder().BorderColor(Color.Grey);

            foreach (var slot in ATM.GetSlots().OrderByDescending(s => s.BanknoteType))
            {
                table.AddRow($"[grey]{(int)slot.BanknoteType}[/]", $"{slot.Count} из {slot.MaxBanknoteCount}");
            }

            AnsiConsole.Write(table);
        }
    }
}
