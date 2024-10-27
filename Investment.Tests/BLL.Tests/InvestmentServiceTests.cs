using Investment.BLL.Services;
using Investment.Core.DTOs;
using Investment.Core.Enums;
using Investment.DAL.Repositories;
using Investment.Tests.Base;

namespace Investment.Tests.BLL.Tests
{
    public class InvestmentServiceTests : TestBase
    {
        private readonly InvestmentService _investmentService;

        public InvestmentServiceTests()
        {
            var dbContext = CreateDbContext();
            var stockLotRepository = new StockLotRepository(dbContext);
            var saleRecordRepository = new SaleRecordRepository(dbContext);

            _investmentService = new InvestmentService(stockLotRepository, saleRecordRepository, CreateMapper());
        }

        /// <summary>
        /// Verifies that the CalculateSellResultsAsync method correctly calculates profit using the FIFO strategy.
        /// </summary>
        [Fact]
        public async Task CalculateSellResults_FifoStrategy_CorrectProfit()
        {
            var dto = new CalculateSellDto
            {
                StrategyType = StrategyType.FIFO,
                SharesToSell = 150,
                SalePricePerShare = 40
            };

            var result = await _investmentService.CalculateSellResultsAsync(dto);

            Assert.Equal(220, result.RemainingShares);
            Assert.Equal(23.33m, result.SoldSharesCostBasis, 2);
            Assert.Equal(19.09m, result.RemainingSharesCostBasis, 2);
            Assert.Equal(2500m, result.Profit);
        }

        /// <summary>
        /// Verifies that the CalculateSellResultsAsync method correctly calculates profit using the LIFO strategy.
        /// </summary>
        [Fact]
        public async Task CalculateSellResults_LifoStrategy_CorrectProfit()
        {
            var dto = new CalculateSellDto
            {
                StrategyType = StrategyType.LIFO,
                SharesToSell = 150,
                SalePricePerShare = 40
            };
            var result = await _investmentService.CalculateSellResultsAsync(dto);

            Assert.Equal(220, result.RemainingShares);
            Assert.Equal(14m, result.SoldSharesCostBasis, 2);
            Assert.Equal(25.45m, result.RemainingSharesCostBasis, 2);
            Assert.Equal(3900m, result.Profit);
        }

        /// <summary>
        /// Verifies that the GetAllStockLotsAsync method returns all stock lots from the database.
        /// </summary>
        [Fact]
        public async Task GetAllStockLotsAsync_ReturnsAllStockLots()
        {
            var stockLots = await _investmentService.GetAllStockLotsAsync();

            Assert.Equal(3, stockLots.Count);
            Assert.Equal(100, stockLots[0].Shares);
            Assert.Equal(150, stockLots[1].Shares);
            Assert.Equal(120, stockLots[2].Shares);
        }

        /// <summary>
        /// Verifies that the ConfirmSaleAsync method successfully saves a sale record when provided with valid data.
        /// </summary>
        [Fact]
        public async Task ConfirmSale_WithValidData_SavesSuccessfully()
        {
            var confirmDto = new ConfirmSaleDto
            {
                SalePricePerShare = 40,
                StockLotItems =
                [
                    new StockLotItemDto { Id = 1, Shares = 100 },
                    new StockLotItemDto { Id = 2, Shares = 50 }
                ]
            };

            var result = await _investmentService.ConfirmSaleAsync(confirmDto);

            Assert.True(result);

            var dbContext = CreateDbContext();
            var saleRecords = dbContext.SaleRecords.ToList();
            Assert.Single(saleRecords);
            Assert.Equal(2500m, saleRecords.First().TotalProfit);
        }

        /// <summary>
        /// Verifies that the GetAllSaleRecordsAsync method returns all sale records from the database.
        /// </summary>
        [Fact]
        public async Task GetAllSaleRecordsAsync_ReturnsSaleRecords()
        {
            await ConfirmSale_WithValidData_SavesSuccessfully();

            var saleRecords = await _investmentService.GetAllSaleRecordsAsync();

            Assert.Single(saleRecords);
            Assert.Equal(2500m, saleRecords.First().TotalProfit);
        }

        /// <summary>
        /// Verifies that the GetSaleRecordDetailsByIdAsync method returns the correct sale record details for a given ID.
        /// </summary>
        [Fact]
        public async Task GetSaleRecordDetailsByIdAsync_ReturnsCorrectDetails()
        {
            await ConfirmSale_WithValidData_SavesSuccessfully();

            var dbContext = CreateDbContext();
            var saleRecordId = dbContext.SaleRecords.First().Id;

            var saleDetails = await _investmentService.GetSaleRecordDetailsByIdAsync(saleRecordId);

            Assert.NotNull(saleDetails);
            Assert.Equal(2500m, saleDetails.TotalProfit);
            Assert.Equal(2, saleDetails.SaleRecordDetailItems.Count);
        }
    }
}
