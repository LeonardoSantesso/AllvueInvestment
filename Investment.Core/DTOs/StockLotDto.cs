namespace Investment.Core.DTOs;

public class StockLotDto
{
    public int Id { get; set; }
    public int Shares { get; set; }
    public decimal PricePerShare { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool HasSales { get; set; }
}
