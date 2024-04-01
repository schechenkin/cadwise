using Domain;

namespace Application.Commands
{
    internal interface ICommand
    {
        public string Name { get; }
        public void Execute(ATM ATM);
    }
}
