using Investment.Core.Entities.Base;

namespace Investment.Core.Entities;

public class StockLotSale : BaseEntity
{
    public int SaleRecordId { get; set; }
    public int StockLotId { get; set; }
    public int SharesSold { get; set; }
    public decimal CostBasis { get; set; }

    public SaleRecord SaleRecord { get; set; }
    public StockLot StockLot { get; set; }
}
