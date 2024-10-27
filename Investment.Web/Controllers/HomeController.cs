using Investment.BLL.Services;
using Investment.Core.DTOs;
using Investment.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Investment.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInvestmentService _investmentService;

        public HomeController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }

        /// <summary>
        /// Renders the index page displaying a list of all stock lots.
        /// </summary>
        /// <returns>The index view with a list of stock lots.</returns>
        public async Task<IActionResult> Index()
        {
            var stockLots = await _investmentService.GetAllStockLotsAsync();
            return View(stockLots);
        }

        /// <summary>
        /// Renders a partial view displaying a grid of all stock lots.
        /// </summary>
        /// <returns>The partial view "_StockGrid" with a list of stock lots.</returns>
        public async Task<IActionResult> GetStocks()
        {
            var stockLots = await _investmentService.GetAllStockLotsAsync();
            return PartialView("_StockGrid", stockLots);
        }

        /// <summary>
        /// Renders a partial view for simulating a sale in a modal.
        /// </summary>
        /// <returns>The partial view "_ModalSimulateSale".</returns>
        public IActionResult ModalSimulateSale()
        {
            return PartialView("_ModalSimulateSale");
        }

        /// <summary>
        /// Renders a partial view for create a stock lot in a modal.
        /// </summary>
        /// <returns>The partial view "_ModalSimulateSale".</returns>
        public IActionResult ModalCreateStockLot()
        {
            return PartialView("_ModalCreateStockLot");
        }

        /// <summary>
        /// Creates a new StockLot entry and returns the result of the operation.
        /// </summary>
        /// <param name="postData">The StockLot data submitted for creation.</param>
        /// <returns>A JSON response indicating success or failure, with relevant messages and data.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateStockLot([FromBody] StockLotDto postData)
        {
            try
            {
                var stockLot = await _investmentService.CreateStockLotAsync(postData);

                var result = new JsonDataResult
                {
                    DataObject = stockLot,
                    IsSuccess = true,
                    Message = new JsonResultMessage { Message = "Stock Lot created successfully." }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new JsonDataResult
                {
                    IsSuccess = false,
                    Error = new JsonResultError { Message = ex.Message, Title = "An error occurred while creating a Stock lot." }
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Calculates the cost basis and profit for a potential stock sale based on the specified sale data.
        /// Accepts a <see cref="CalculateSellDto"/> containing the number of shares to sell, sale price per share, and selected strategy (FIFO or LIFO).
        /// </summary>
        /// <param name="postData">The <see cref="CalculateSellDto"/> containing the shares to sell, sale price, and cost-basis strategy.</param>
        /// <returns>An <see cref="IActionResult"/> with the calculated sale results, including profit, cost basis per share, and remaining shares.</returns>
        [HttpPost]
        public async Task<IActionResult> CalculateSellResults([FromBody] CalculateSellDto postData)
        {
            try
            {
                var simulationResult = await _investmentService.CalculateSellResultsAsync(postData);

                var result = new JsonDataResult
                {
                    DataObject = simulationResult,
                    IsSuccess = true,
                    Message = new JsonResultMessage { Message = "Calculation performed successfully." }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new JsonDataResult
                {
                    IsSuccess = false,
                    Error = new JsonResultError {Message = ex.Message, Title = "An error occurred while calculating the sale" }
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Processes a stock sale confirmation request, saving the sale details to the database and updating stock lot quantities. 
        /// Accepts a <see cref="ConfirmSaleDto"/> with details of the shares to be sold, including quantities and prices.
        /// </summary>
        /// <param name="confirmData">The <see cref="ConfirmSaleDto"/> containing sale details such as stock lot items, share quantities, and sale price per share.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation, typically returning success or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmSale([FromBody] ConfirmSaleDto confirmData)
        {
            try
            {
                var isSuccess = await _investmentService.ConfirmSaleAsync(confirmData);
                var message = isSuccess ? "Sale confirmed successfully!" : "An error occurred while confirming the sale.";
                var result = new JsonDataResult { IsSuccess = isSuccess };

                if (isSuccess)
                    result.Message = new JsonResultMessage { Message = message };
                else
                    result.Error = new JsonResultError { Message = message, Title = "An error occurred while confirming the sale." };

                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new JsonDataResult
                {
                    IsSuccess = false,
                    Error = new JsonResultError { Message = ex.Message, Title = "An error occurred while confirming the sale." }
                };

                return Json(result);
            }
        }
    }
}
