using FluentResults;
using System.Collections.Immutable;

namespace Domain
{
    public partial class ATM
    {
        private readonly MoneySlots _moneySlots;
        private decimal _balance;

        public ATM(Dictionary<BanknoteType, uint> capacity, uint balance)
        {
            List<MoneySlot> slots = new();

            foreach (var slotConfiguration in capacity)
            {
                slots.Add(new MoneySlot(slotConfiguration.Key, slotConfiguration.Value));
            }

            _moneySlots = new MoneySlots(slots);

            _balance = balance;
        }

        public Result AddBanknotes(BanknotesSet set)
        {
            foreach (var kvp in set.Banknotes)
            {
                if (kvp.Value == 0)
                    continue;

                MoneySlot? slot = _moneySlots.GetMoneySlot(kvp.Key);
                if (slot is null)
                    return Result.Fail($"Банкноты достоинством {(int)kvp.Key} не поддерживаются");
                if (slot.Count + kvp.Value > slot.MaxBanknoteCount)
                    return Result.Fail($"Недостаточно места для банктон достоинства {(int)kvp.Key}"); ;
            }

            foreach (var kvp in set.Banknotes)
            {
                if (kvp.Value == 0)
                    continue;

                MoneySlot? slot = _moneySlots.GetMoneySlot(kvp.Key);

                slot!.Count += kvp.Value;
            }

            return Result.Ok();
        }

        public Result PutMoneyToAccount(BanknotesSet set)
        {
            var result = AddBanknotes(set);
            if (result.IsFailed)
                return result;

            _balance += set.Total();
            return Result.Ok();
        }

        public decimal GetBalance()
        {
            return _balance;
        }
        public Result TakeMoney(BanknotesSet set)
        {
            if(_balance < set.Total())
                return Result.Fail($"Недостаточно средств на счете");

            foreach (var kvp in set.Banknotes)
            {
                if (kvp.Value == 0)
                    continue;

                MoneySlot? slot = _moneySlots.GetMoneySlot(kvp.Key);
                if (slot is null)
                    return Result.Fail($"Банкноты достоинством {(int)kvp.Key} не поддерживаются");
                if ((int)slot.Count - kvp.Value < 0)
                    return Result.Fail($"В банкомате не хватает банкнот достоинства {(int)kvp.Key}");
            }

            foreach (var kvp in set.Banknotes)
            {
                if (kvp.Value == 0)
                    continue;

                MoneySlot? slot = _moneySlots.GetMoneySlot(kvp.Key);

                slot!.Count -= kvp.Value;
            }

            _balance -= set.Total();

            return Result.Ok();
        }

        public ImmutableList<MoneySlot> GetSlots()
        {
            return _moneySlots.Slots.ToImmutableList();
        }

        public IEnumerable<BanknoteType> GetSupportedBanknoteTypes()
        {
            return _moneySlots.Slots.Select(s => s.BanknoteType);
        }
    }
}
