using Domain;
using FluentResults;
using Spectre.Console;

namespace Application.Commands
{
    internal class TakeMoney : ICommand
    {
        public string Name => "Снять наличные";

        static string SelectBanknotesMyself = "Выберу купюры сам";
        static string SelectBanknotesMinCount = "Выдать минимальным количеством банкнот";

        public void Execute(ATM ATM)
        {
            AnsiConsole.Write(new Rule("[yellow]Снятие наличных[/]").RuleStyle("grey").LeftJustified());

            var amount = AnsiConsole.Prompt(
                            new TextPrompt<uint>("Введите сумму:")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Неверный формат[/]")
                                );

            var moneySelecctionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(SelectBanknotesMyself, SelectBanknotesMinCount)
                    .Title("Как выдать деньги?"));

            if(moneySelecctionChoice == SelectBanknotesMyself)
            {
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
                    TryTakeBanknotes(ATM, banknotesSet);
                }
                else
                {
                    if (AskConfirmation(amount < (uint)banknotesSet.Total() ? "Вы запрашиваете больше чем заявили. Снять наличные?" : "Вы запрашиваете меньше чем заявили. Снять наличные?"))
                        TryTakeBanknotes(ATM, banknotesSet);
                }
            }
            else
            {
                var rest = amount;

                BanknotesSet banknotesSet = new BanknotesSet();
                foreach (BanknoteType banknoteType in ATM.GetSupportedBanknoteTypes().OrderByDescending(t => t))
                {
                    var count = rest / (uint)banknoteType;
                    banknotesSet.Add(banknoteType, count);
                    rest -= count * (uint)banknoteType;
                }

                if(rest > 0)
                {
                    string listOfBanknoteTypes = string.Join(',', ATM.GetSlots().Where(s => s.Count > 0).Select(s => ((int)s.BanknoteType)));
                    AnsiConsole.MarkupLine($"[red]Невозможно выдать требуемую сумму. В банкомате есть только купюры достоинства: {listOfBanknoteTypes}[/]");
                }
                else
                {
                    TryTakeBanknotes(ATM, banknotesSet);
                }
            }
        }

        private void TryTakeBanknotes(ATM ATM, BanknotesSet banknotesSet)
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
