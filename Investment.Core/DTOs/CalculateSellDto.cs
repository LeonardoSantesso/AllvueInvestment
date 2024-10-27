using Investment.Core.Enums;

namespace Investment.Core.DTOs;

public class CalculateSellDto
{
    public StrategyType StrategyType { get; set; }
    public int SharesToSell { get; set; }
    public decimal SalePricePerShare { get; set; }
}
