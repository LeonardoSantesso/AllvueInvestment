namespace Investment.Core.DTOs;

public class ConfirmSaleDto
{
    public decimal SalePricePerShare { get; set; }
    public List<StockLotItemDto> StockLotItems { get; set; } = new List<StockLotItemDto>();
}

