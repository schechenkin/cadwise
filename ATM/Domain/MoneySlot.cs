namespace Domain
{
    public partial class ATM
    {
        public class MoneySlot
        {
            public MoneySlot(BanknoteType banknoteType, uint maxBanknoteCount, uint count = 0)
            {
                BanknoteType = banknoteType;
                MaxBanknoteCount = maxBanknoteCount;
                Count = count;
            }

            public BanknoteType BanknoteType { get; init; }
            public uint MaxBanknoteCount { get; init; }
            public uint Count { get; internal set; }
        }
    }
}
