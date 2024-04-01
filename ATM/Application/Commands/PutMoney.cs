using Domain;
using Spectre.Console;

namespace Application.Commands
{
    internal class PutMoney : ICommand
    {
        public string Name => "Внести наличные";

        public void Execute(ATM ATM)
        {
            AnsiConsole.Write(new Rule("[yellow]Внесение наличных[/]").RuleStyle("grey").LeftJustified());

            BanknotesSet banknotesSet = new BanknotesSet();
            foreach (BanknoteType type in ATM.GetSupportedBanknoteTypes().OrderByDescending(t => t))
            {
                var count = AskBanknotesCount(type);
                banknotesSet.Add(type, count);
            }
            var result = ATM.PutMoneyToAccount(banknotesSet);

            if (result.IsFailed)
            {
                AnsiConsole.MarkupLine($"[red]{result.Errors.First().Message}[/]");
                AnsiConsole.WriteLine($"Операция отменена. Заберите деньги");
            }
            else
            {
                AnsiConsole.MarkupLine($"Деньги внесены");
            }
        }

        private uint AskBanknotesCount(BanknoteType banknoteType)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<uint>($"Введите количество банктон достоинства {(int)banknoteType}:")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Неверный формат[/]"));
        }
    }
}
