using Investment.BLL.Services;
using Investment.Core.DTOs;
using Investment.Core.Enums;
using Investment.DAL.Repositories;
using Investment.Tests.Base;
using Investment.Web.Controllers;
using Investment.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Investment.Tests.Web.Tests
{
    public class HomeControllerTests : TestBase
    {
        private readonly HomeController _controller;
        private readonly InvestmentService _investmentService;

        public HomeControllerTests()
        {
            var dbContext = CreateDbContext();
            var stockLotRepository = new StockLotRepository(dbContext);
            var saleRecordRepository = new SaleRecordRepository(dbContext);

            _investmentService = new InvestmentService(stockLotRepository, saleRecordRepository, CreateMapper());
            _controller = new HomeController(_investmentService);
        }

        /// <summary>
        /// Verifies that the CalculateSellResults action in HomeController returns the expected result
        /// when using the FIFO strategy for calculating a stock sale.
        /// </summary>
        [Fact]
        public async Task CalculateSellResults_FifoStrategy_ReturnsExpectedResult()
        {
            var dto = new CalculateSellDto
            {
                StrategyType = StrategyType.FIFO,
                SharesToSell = 150,
                SalePricePerShare = 40
            };

            var result = await _controller.CalculateSellResults(dto) as JsonResult;

            Assert.NotNull(result);
            var jsonData = result.Value as JsonDataResult;
            var resultObject = jsonData.DataObject as SellSharesResultDto;
            Assert.Equal(220, resultObject.RemainingShares);
            Assert.Equal(23.33m, resultObject.SoldSharesCostBasis, 2);
            Assert.Equal(19.09m, resultObject.RemainingSharesCostBasis, 2);
            Assert.Equal(2500m, resultObject.Profit);
        }

        /// <summary>
        /// Verifies that the CalculateSellResults action in HomeController returns the correct profit
        /// when using the LIFO strategy for calculating a stock sale.
        /// </summary>
        [Fact]
        public async Task CalculateSellResults_LifoStrategy_ReturnsCorrectProfit()
        {
            var dto = new CalculateSellDto
            {
                StrategyType = StrategyType.LIFO,
                SharesToSell = 150,
                SalePricePerShare = 40
            };

            var result = await _controller.CalculateSellResults(dto) as JsonResult;

            Assert.NotNull(result);
            var jsonData = result.Value as JsonDataResult;
            var resultObject = jsonData.DataObject as SellSharesResultDto;
            Assert.Equal(220, resultObject.RemainingShares);
            Assert.Equal(14m, resultObject.SoldSharesCostBasis, 2);
            Assert.Equal(25.45m, resultObject.RemainingSharesCostBasis, 2);
            Assert.Equal(3900m, resultObject.Profit);
        }

        /// <summary>
        /// Verifies that the ConfirmSale action in HomeController successfully saves a sale record
        /// when provided with valid data.
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

            var result = await _controller.ConfirmSale(confirmDto) as JsonResult;

            Assert.NotNull(result);
            var jsonData = result.Value as JsonDataResult;
            Assert.True(jsonData.IsSuccess);
            
            var dbContext = CreateDbContext();
            var saleRecords = dbContext.SaleRecords.ToList();
            Assert.Single(saleRecords);
            Assert.Equal(2500, saleRecords.First().TotalProfit);
        }
    }
}
