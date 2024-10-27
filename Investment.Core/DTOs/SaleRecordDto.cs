namespace Investment.Core.DTOs;

public class SaleRecordDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public int TotalSharesSold { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSaleValue { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal CostBasisPerSoldShare { get; set; }
}
