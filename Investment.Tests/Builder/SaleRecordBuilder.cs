using Investment.Core.Entities;

namespace Investment.Tests.Builder;

public static class SaleRecordBuilder
{
    /// <summary>
    /// Builds a new instance of <see cref="SaleRecord"/> based on the provided stock lot data. 
    /// </summary>
    /// <param name="stockLot"></param>
    /// <returns></returns>
    public static SaleRecord BuildSaleRecord(StockLot stockLot)
    {
        return new SaleRecord
        {
            CostBasisPerSoldShare = Faker.RandomNumber.Next(10,50),
            SaleDate = DateTime.Now,
            TotalProfit = Faker.RandomNumber.Next(100, 1000),
            TotalCost = Faker.RandomNumber.Next(1000, 4000),
            TotalSaleValue = Faker.RandomNumber.Next(100, 1000),
            StockLotSales = new List<StockLotSale>
            {
                new()
                {
                    CostBasis = Faker.RandomNumber.Next(10, 40),
                    SharesSold = Faker.RandomNumber.Next(10, 40),
                    StockLotId = stockLot.Id
                }
            }
        };
    }
}

