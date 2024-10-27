namespace Investment.Core.DTOs;

public class StockLotDto
{
    public int Id { get; set; }
    public int OriginalShares { get; set; }
    public int Shares { get; set; }
    public decimal PricePerShare { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int RemainingPercentage => OriginalShares > 0 ? (int)Math.Round((decimal)Shares / OriginalShares * 100) : 0;
}
