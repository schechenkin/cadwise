using Domain;

namespace Tests
{
    public class ATMTests
    {
        [Fact]
        public void Add_Money()
        {
            //Given
            var sut = new ATM(capacity: new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_50, 100 },
                { BanknoteType.Banknote_100, 100 },
                { BanknoteType.Banknote_500, 100 },
                { BanknoteType.Banknote_1000, 100 },
            }, balance: 0);

            //When
            sut.PutMoneyToAccount(new BanknotesSet()
                .Add(BanknoteType.Banknote_100, count: 10)
                .Add(BanknoteType.Banknote_1000, count: 10)
            )
            .Should().BeEquivalentTo(Result.Ok());

            //Then
            sut.GetBalance().Should().Be(11000);
        }

        [Fact]
        public void Add_Money_Fails_When_Not_Enought_Space_For_Banknotes()
        {
            //Given
            var sut = new ATM(capacity: new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_100, 10 },
                { BanknoteType.Banknote_500, 10 },
                { BanknoteType.Banknote_1000, 10 },
            }, balance: 10000);

            //When
            var result = sut.AddBanknotes(new BanknotesSet()
                .Add(BanknoteType.Banknote_100, count: 20)
                .Add(BanknoteType.Banknote_500, count: 20)
            );

            //Then
            result.Should().BeEquivalentTo(Result.Fail("Недостаточно места для банктон достоинства 100"));
        }

        [Fact]
        public void Take_Money_Fails_When_Not_Enought_Banknoter()
        {
            //Given
            var sut = new ATM(capacity: new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_1000, 1000 },
            }, balance: 100000);

            sut.AddBanknotes(new BanknotesSet()
                .Add(BanknoteType.Banknote_1000, count: 5)
            );

            //When
            var result = sut.TakeMoney(new BanknotesSet().Add(BanknoteType.Banknote_1000, count: 6));

            //Then
            result.Should().BeEquivalentTo(Result.Fail("В банкомате не хватает банкнот достоинства 1000"));
        }

        [Fact]
        public void Take_Money_Success()
        {
            //Given
            var sut = new ATM(capacity: new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_1000, 100 },
                { BanknoteType.Banknote_500, 100 },
                { BanknoteType.Banknote_100, 100 },
            }, balance: 10000);

            sut.AddBanknotes(new BanknotesSet()
                .Add(BanknoteType.Banknote_1000, count: 5)
                .Add(BanknoteType.Banknote_500, count: 5)
                .Add(BanknoteType.Banknote_100, count: 5)
            );
 
            //When
            var result = sut.TakeMoney(new BanknotesSet()
                             .Add(BanknoteType.Banknote_1000, 5)
                             .Add(BanknoteType.Banknote_500, 1)
                             .Add(BanknoteType.Banknote_100, 2)
                         );

            //Then
            result.Should().BeEquivalentTo(Result.Ok());
            sut.GetBalance().Should().Be(4300);
        }

        [Fact]
        public void Take_Money_Fails_When_unable_to_collect_banknotes()
        {
            //Given
            var sut = new ATM(capacity: new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_1000, 1000 },
                { BanknoteType.Banknote_500, 1000 },
                { BanknoteType.Banknote_100, 1000 },
            }, balance: 100000);

            sut.AddBanknotes(new BanknotesSet()
                .Add(BanknoteType.Banknote_1000, count: 5)
                .Add(BanknoteType.Banknote_500, count: 5)
                .Add(BanknoteType.Banknote_100, count: 1)
            );

            //When
            var result = sut.TakeMoney(new BanknotesSet()
                .Add(BanknoteType.Banknote_1000, count: 6)
                .Add(BanknoteType.Banknote_500, count: 5));

            //Then
            result.Should().BeEquivalentTo(Result.Fail("В банкомате не хватает банкнот достоинства 1000"));
        }

        [Fact]
        public void Take_Money_Fails_When_balance_is_low()
        {
            //Given
            var sut = new ATM(capacity: new Dictionary<BanknoteType, uint>() {
                { BanknoteType.Banknote_1000, 1000 },
                { BanknoteType.Banknote_500, 1000 },
                { BanknoteType.Banknote_100, 1000 },
            }, balance: 5000);

            sut.AddBanknotes(new BanknotesSet()
                .Add(BanknoteType.Banknote_1000, count: 5)
                .Add(BanknoteType.Banknote_500, count: 5)
                .Add(BanknoteType.Banknote_100, count: 1)
            );

            //When
            var result = sut.TakeMoney(new BanknotesSet().Add(BanknoteType.Banknote_1000, count: 6));

            //Then
            result.Should().BeEquivalentTo(Result.Fail("Недостаточно средств на счете"));
        }
    }
}
