using AutoMapper;
using Investment.Core.DTOs;
using Investment.Core.Entities;
using Investment.Core.Enums;
using Investment.Core.Interfaces;

namespace Investment.BLL.Services;

/// <summary>
/// Provides core business logic for managing investments, including calculating potential sale outcomes
/// (profit, cost basis), and recording confirmed sales transactions. This service interacts with stock lots and 
/// sale records, utilizing various cost-basis strategies such as FIFO and LIFO.
/// </summary>
public class InvestmentService : IInvestmentService
{
    private readonly IMapper _mapper;
    private readonly IStockLotRepository _stockLotRepository;
    private readonly ISaleRecordRepository _saleRecordRepository;

    public InvestmentService(IStockLotRepository stockLotRepository, ISaleRecordRepository saleRecordRepository, IMapper mapper)
    {
        _stockLotRepository = stockLotRepository;
        _mapper = mapper;
        _saleRecordRepository = saleRecordRepository;
    }

    public async Task<List<StockLotDto>> GetAllStockLotsAsync(bool ascending = true)
    {
        var stockLots = await _stockLotRepository.GetAllOrderedAsync(i => i.PurchaseDate, ascending, i => i.StockLotSales);
        return _mapper.Map<List<StockLotDto>>(stockLots);
    }

    /// <summary>
    /// Calculates the result of a potential stock sale based on the specified number of shares, sale price per share, and cost-basis strategy (e.g., FIFO or LIFO).
    /// This calculation determines the total profit, cost basis of sold shares, remaining cost basis, and updates the remaining shares after a hypothetical sale.
    /// </summary>
    /// <param name="calculateSellData">Data required for the calculation, including the strategy, number of shares to sell, and sale price per share.</param>
    /// <returns>A <see cref="SellSharesResultDto"/> containing the calculated profit, cost basis details, and remaining shares.</returns>
    public async Task<SellSharesResultDto> CalculateSellResultsAsync(CalculateSellDto calculateSellData)
    {
        var isFIFO = calculateSellData.StrategyType == StrategyType.FIFO;
        var stockLots = await _stockLotRepository.GetAllStockLotsAsync(i => i.PurchaseDate, isFIFO, true);

        int totalAvailableShares = stockLots.Sum(l => l.Shares);

        if (calculateSellData.SharesToSell > totalAvailableShares)
            throw new InvalidOperationException("The number of shares requested exceeds the total available for sale.");

        var result = CalculateCostBasis(stockLots, calculateSellData.SharesToSell, calculateSellData.SalePricePerShare);

        return result;
    }

    /// <summary>
    /// Confirms and records a stock sale transaction based on the specified data.
    /// This method updates the stock lots by deducting the sold shares, calculates the total profit from the sale, and records the transaction in the database.
    /// The operation includes creating a sale record with the associated stock lot details.
    /// </summary>
    /// <param name="confirmData">Data required to confirm the sale, including the list of stock lots and shares sold.</param>
    /// <returns>A boolean value indicating whether the sale was successfully confirmed and recorded.</returns>
    public async Task<bool> ConfirmSaleAsync(ConfirmSaleDto confirmData)
    {
        decimal totalCostBasis = 0;
        int totalSharesSold = 0;
        List<StockLotSale> stockLotSales = new List<StockLotSale>();

        foreach (var item in confirmData.StockLotItems)
        {
            var lot = await _stockLotRepository.GetByIdAsync(item.Id);
            if (lot == null || lot.Shares < item.Shares)
                throw new InvalidOperationException("StockLot does not have enough shares.");

            decimal costBasisForThisLot = item.Shares * lot.PricePerShare;
            totalCostBasis += costBasisForThisLot;
            totalSharesSold += item.Shares;

            lot.Shares -= item.Shares;
            _stockLotRepository.Update(lot);

            stockLotSales.Add(new StockLotSale
            {
                StockLotId = lot.Id,
                SharesSold = item.Shares,
                CostBasis = costBasisForThisLot
            });
        }

        var totalSaleValue = totalSharesSold * confirmData.SalePricePerShare;
        decimal profit = totalSaleValue - totalCostBasis;
        decimal costBasisPerSoldShare = totalSharesSold > 0 ? totalCostBasis / totalSharesSold : 0;

        var saleRecord = new SaleRecord
        {
            SaleDate = DateTime.UtcNow,
            TotalCost = totalCostBasis,
            TotalSaleValue = totalSaleValue,
            TotalProfit = profit,
            StockLotSales = stockLotSales,
            CostBasisPerSoldShare = costBasisPerSoldShare
        };

        await _saleRecordRepository.AddAsync(saleRecord);
        await _saleRecordRepository.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Retrieves a list of all recorded stock sale transactions. 
    /// </summary>
    /// <returns>A list of <see cref="SaleRecordDto"/> objects representing each sale record in the system.</returns>
    public async Task<List<SaleRecordDto>> GetAllSaleRecordsAsync()
    {
        var sales = await _saleRecordRepository.GetAllAsync(i => i.StockLotSales);
        return _mapper.Map<List<SaleRecordDto>>(sales);
    }

    /// <summary>
    /// Retrieves detailed information for a specific stock sale transaction based on its unique identifier. 
    /// The details include information such as the total profit, cost basis, sale date, and breakdown of each stock lot involved in the sale.
    /// </summary>
    /// <param name="id">The unique identifier of the sale record to retrieve.</param>
    /// <returns>A <see cref="SaleRecordDetailsDto"/> containing the detailed information for the specified sale record.</returns>
    public async Task<SaleRecordDetailsDto> GetSaleRecordDetailsByIdAsync(int id)
    {
        var sale = await _saleRecordRepository.GetSaleRecordById(id);
        return _mapper.Map<SaleRecordDetailsDto>(sale);
    }

    /// <summary>
    /// Creates a new StockLot, saves it to the database, and returns the created StockLot as a DTO.
    /// </summary>
    /// <param name="stockLotData">Data for the new StockLot.</param>
    /// <returns>The newly created StockLot as a DTO.</returns>
    public async Task<StockLotDto> CreateStockLot(StockLotDto stockLotData)
    {
        var stockLot = _mapper.Map<StockLot>(stockLotData);
        stockLot.OriginalShares = stockLotData.Shares;

        await _stockLotRepository.AddAsync(stockLot);
        await _stockLotRepository.SaveChangesAsync();

        return _mapper.Map<StockLotDto>(stockLot);
    }
    
    #region Private methods

    /// <summary>
    /// Calculates the cost basis and profit for a potential stock sale based on the specified number of shares to sell, 
    /// sale price per share, and the ordered list of stock lots (using a chosen strategy such as FIFO or LIFO). 
    /// This calculation considers the cost basis per sold share, total profit, and remaining shares and cost basis after the sale.
    /// </summary>
    /// <param name="orderedLots">The ordered collection of stock lots from which shares will be sold according to the selected strategy.</param>
    /// <param name="sharesToSell">The total number of shares to sell in the transaction.</param>
    /// <param name="salePricePerShare">The price at which each share is being sold.</param>
    /// <returns>A <see cref="SellSharesResultDto"/> containing details of the cost basis per sold share,
    /// total profit, remaining shares, and remaining cost basis per share.</returns>
    private SellSharesResultDto CalculateCostBasis(IEnumerable<StockLot> orderedLots, int sharesToSell, decimal salePricePerShare)
    {
        int remainingSharesToSell = sharesToSell;
        decimal totalCostBasis = 0;
        int totalSharesSold = 0;
        var stockLotItems = new List<StockLotItemDto>();

        foreach (var lot in orderedLots)
        {
            if (remainingSharesToSell <= 0) break;

            int sharesFromThisLot = lot.SellShares(remainingSharesToSell);
            totalCostBasis += sharesFromThisLot * lot.PricePerShare;
            totalSharesSold += sharesFromThisLot;

            stockLotItems.Add(new StockLotItemDto
            {
                Id = lot.Id,
                Shares = sharesFromThisLot
            });

            remainingSharesToSell -= sharesFromThisLot;
        }

        decimal costBasisPerSoldShare = totalSharesSold > 0 ? totalCostBasis / totalSharesSold : 0;
        decimal profit = totalSharesSold * salePricePerShare - totalCostBasis;

        decimal totalShares = orderedLots.Where(l => l.Shares > 0).Sum(l => l.Shares);
        decimal remainingCostBasisPerShare = totalShares > 0 ? orderedLots.Where(l => l.Shares > 0).Sum(l => l.Shares * l.PricePerShare) / totalShares : 0;

        int totalRemainingShares = orderedLots.Sum(l => l.Shares);

        return new SellSharesResultDto
        {
            SalePricePerShare = salePricePerShare,
            RemainingShares = totalRemainingShares,
            SoldSharesCostBasis = costBasisPerSoldShare,
            RemainingSharesCostBasis = remainingCostBasisPerShare,
            Profit = profit,
            StockLotItems = stockLotItems
        };
    }

    #endregion
}
