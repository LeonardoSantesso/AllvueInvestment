namespace Investment.Core.DTOs;

public class SellSharesResultDto
{
    public decimal SalePricePerShare { get; set; }
    public int RemainingShares { get; set; }
    public decimal SoldSharesCostBasis { get; set; }
    public decimal RemainingSharesCostBasis { get; set; }
    public decimal Profit { get; set; }
    public List<StockLotItemDto> StockLotItems { get; set; } = new List<StockLotItemDto>();
}

