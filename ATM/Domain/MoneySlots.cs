namespace Domain
{
    public partial class ATM
    {
        internal class MoneySlots
        {
            public MoneySlots(IList<MoneySlot> slots)
            {
                Slots = slots;
            }

            public IList<MoneySlot> Slots { get; }

            public MoneySlot? GetMoneySlot(BanknoteType type) => Slots.FirstOrDefault(s => s.BanknoteType == type);

        }
    }
}
