using Investment.Core.Entities.Base;

namespace Investment.Core.Entities;

public class SaleRecord : BaseEntity
{
    public DateTime SaleDate { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSaleValue { get; set; }
    public decimal TotalProfit { get; set; } 
    public decimal CostBasisPerSoldShare { get; set; } 
    
    public ICollection<StockLotSale> StockLotSales { get; set; } = new List<StockLotSale>();
}


