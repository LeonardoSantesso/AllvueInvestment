using Investment.Core.Entities.Base;

namespace Investment.Core.Entities;

public class StockLot : BaseEntity
{
    public int Shares { get; set; }
    public decimal PricePerShare { get; set; }
    public DateTime PurchaseDate { get; set; }
    public ICollection<StockLotSale> StockLotSales { get; set; } = new List<StockLotSale>();

    public StockLot()
    {
        PurchaseDate = DateTime.Now;
    }
    
    public int SellShares(int quantity)
    {
        int sharesSold = Math.Min(Shares, quantity);
        Shares -= sharesSold;
        return sharesSold;
    }
}
