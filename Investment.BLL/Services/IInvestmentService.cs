using Investment.Core.DTOs;

namespace Investment.BLL.Services;

public interface IInvestmentService
{
    Task<List<StockLotDto>> GetAllStockLotsAsync(bool ascending = true);
    Task<SellSharesResultDto> CalculateSellResultsAsync(CalculateSellDto calculateSellData);
    Task<bool> ConfirmSaleAsync(ConfirmSaleDto confirmData);
    Task<List<SaleRecordDto>> GetAllSaleRecordsAsync();
    Task<SaleRecordDetailsDto> GetSaleRecordDetailsByIdAsync(int id);
    Task<StockLotDto> CreateStockLot(StockLotDto stockLotData);
}
