namespace Investment.Core.DTOs;

public class SaleRecordDetailsDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSaleValue { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalSold { get; set; }
    public decimal CostBasisPerSoldShare { get; set; }
    public List<SaleRecordDetailItemDto> SaleRecordDetailItems { get; set; } = new List<SaleRecordDetailItemDto>();
}

public class SaleRecordDetailItemDto
{
    public int SharesSold { get; set; }
    public decimal CostBasis { get; set; }
    public decimal PricePerShare { get; set; }
    public DateTime PurchaseDate { get; set; }
}

