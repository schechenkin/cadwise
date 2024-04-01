namespace Domain
{
    public class BanknotesSet
    {
        public readonly Dictionary<BanknoteType, uint> Banknotes = new();

        public BanknotesSet Add(BanknoteType banknoteType, uint count)
        {
            if (!Banknotes.ContainsKey(banknoteType))
                Banknotes.Add(banknoteType, count);
            else
                Banknotes[banknoteType] += count;

            return this;
        }

        public decimal Total() 
        {
            decimal sum = 0;

            foreach (var item in Banknotes) 
            {
                sum += (int)item.Key * item.Value;
            }

            return sum;
        }
    }

}
