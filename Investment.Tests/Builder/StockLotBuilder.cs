using Investment.Core.Entities;

namespace Investment.Tests.Builder;

public static class StockLotBuilder
{
    /// <summary>
    /// Builds and returns a single <see cref="StockLot"/> instance with randomized data for testing purposes. 
    /// The stock lot includes random values for shares, price per share, and purchase date within specified ranges.
    /// </summary>
    /// <returns>A <see cref="StockLot"/> object with randomly generated data.</returns>

    public static StockLot BuildStockLot()
    {
        return new StockLot
        {
            Id = 1, 
            Shares = Faker.RandomNumber.Next(20, 90), 
            PricePerShare = Faker.RandomNumber.Next(10, 50), 
            PurchaseDate = new DateTime(2024, Faker.RandomNumber.Next(1, 12), Faker.RandomNumber.Next(1, 28))
        };
    }

    /// <summary>
    /// Builds and returns a predefined list of <see cref="StockLot"/> instances. 
    /// Each stock lot includes sample data such as shares, price per share, and purchase date, 
    /// simulating stock purchases for testing or demonstration purposes.
    /// </summary>
    /// <returns>A list of <see cref="StockLot"/> objects with predefined values.</returns>
    public static List<StockLot> BuildStockLots()
    {
        return
        [
            new StockLot { Id = 1, OriginalShares = 100, Shares = 100, PricePerShare = 20, PurchaseDate = new DateTime(2024, 01, 01) },
            new StockLot { Id = 2, OriginalShares = 150, Shares = 150, PricePerShare = 30, PurchaseDate = new DateTime(2024, 02, 01) },
            new StockLot { Id = 3, OriginalShares = 120, Shares = 120, PricePerShare = 10, PurchaseDate = new DateTime(2024, 03, 01) }
        ];
    }
}

