using Application.Commands;
using Domain;
using Spectre.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        var ATM = InitATM();
        RunATMLoop(ATM);
    }

    private static void RunATMLoop(ATM ATM)
    {
        while (true)
        {
            AuthorizeLoop();
            UserLoop(ATM);
        }
    }

    private static ATM InitATM()
    {
        var ATM = new ATM(capacity:
            new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_50, 100 },
                { BanknoteType.Banknote_100, 100 },
                { BanknoteType.Banknote_500, 100 },
                { BanknoteType.Banknote_1000, 100 },
                { BanknoteType.Banknote_5000, 100 }
            },
            balance: 100000);

        ATM.AddBanknotes(new BanknotesSet()
                        .Add(BanknoteType.Banknote_50, 50)
                        .Add(BanknoteType.Banknote_100, 50)
                        .Add(BanknoteType.Banknote_500, 50)
                        .Add(BanknoteType.Banknote_1000, 50));

        return ATM;
    }

    private static void AuthorizeLoop()
    {
        bool authorized = false;

        while (!authorized)
        {
            RenderLogo();
            var pin = AskPin();

            if (pin == 1234)
            {
                authorized = true;
            }
            else
            {
                AnsiConsole.Clear();
                RenderLogo();
                AnsiConsole.MarkupLine($"[red]Неверный PIN[/]");
                AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("Попробовать снова"));
            }
            AnsiConsole.Clear();
        }
    }

    private static void UserLoop(ATM ATM)
    {
        bool keepProcessingUser = true;
        var exit = new Exit();
        while (keepProcessingUser)
        {
            RenderLogo();

            var command = AnsiConsole.Prompt(new SelectionPrompt<ICommand>() { Converter = (ICommand command) => command.Name }
                                    .AddChoices(new CheckBalance(), new PutMoney(), new TakeMoney(), new ShowBanknotes(), exit)
                                    .Title("Выберете пункт меню"));

            if (command == exit)
                keepProcessingUser = false;
            else
            {
                command.Execute(ATM);

                AnsiConsole.WriteLine($"");
                AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("Назад"));
            }

            AnsiConsole.Clear();
        }
    }
    public static void RenderLogo()
    {
        AnsiConsole.Write(new FigletText("Penkoff Bank").Color(Color.Yellow));
    }

    public static int AskPin()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>("Введите 4-значный [green]PIN[/]?")
                .PromptStyle("red")
                .ValidationErrorMessage("Неверный формат")
                .Secret());
    }
}