using Domain;

namespace Application.Commands
{

    internal class Exit : ICommand
    {
        public string Name => "Завершить обслуживание";

        public void Execute(ATM ATM)
        {

        }
    }
}
