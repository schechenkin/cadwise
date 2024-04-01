using Domain;
using Spectre.Console;

namespace Application.Commands
{
    internal class TakeMoney : ICommand
    {
        public string Name => "Снять наличные";

        public void Execute(ATM ATM)
        {
            AnsiConsole.Write(new Rule("[yellow]Снятие наличных[/]").RuleStyle("grey").LeftJustified());

            var amount = AnsiConsole.Prompt(
                            new TextPrompt<uint>("Введите сумму:")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Неверный формат[/]")
                                );

            var rest = amount;

            BanknotesSet banknotesSet = new BanknotesSet();
            foreach (BanknoteType type in ATM.GetSupportedBanknoteTypes().Where(t => (int)t < rest).OrderByDescending(t => t))
            {
                if ((uint)type > rest)
                    continue;

                var count = AskBanknotesCount(type);
                rest -= count * (uint)type;

                banknotesSet.Add(type, count);
            }

            if (amount == (uint)banknotesSet.Total())
            {
                TakeBanknotes(ATM, banknotesSet);
            }
            else
            {
                if (AskConfirmation(amount < (uint)banknotesSet.Total() ? "Вы запрашиваете больше чем заявили. Снять наличные?" : "Вы запрашиваете меньше чем заявили. Снять наличные?"))
                    TakeBanknotes(ATM, banknotesSet);
            }
        }

        private void TakeBanknotes(ATM ATM, BanknotesSet banknotesSet)
        {
            var result = ATM.TakeMoney(banknotesSet);
            if (result.IsSuccess)
            {
                AnsiConsole.WriteLine($"");
                AnsiConsole.WriteLine($"Заберите деньги");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{result.Errors.First().Message}[/]");
            }
        }

        public static bool AskConfirmation(string text)
        {
            return AnsiConsole.Confirm(text);
        }

        private uint AskBanknotesCount(BanknoteType banknoteType)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<uint>($"Введите количество банктон достоинства {(int)banknoteType}")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Неверный формат[/]"));
        }
    }
}
