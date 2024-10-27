using Investment.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investment.Web.Controllers;

public class SaleRecordsController : Controller
{
    private readonly IInvestmentService _investmentService;

    public SaleRecordsController(IInvestmentService investmentService)
    {
        _investmentService = investmentService;
    }

    /// <summary>
    /// Renders the index page displaying a list of all sale records.
    /// </summary>
    /// <returns>The index view with a list of sale records.</returns>
    public async Task<IActionResult> Index()
    {
        var sales = await _investmentService.GetAllSaleRecordsAsync();
        return View(sales);
    }

    /// <summary>
    /// Renders a modal with details for a specific sale record.
    /// </summary>
    /// <param name="id">The ID of the sale record.</param>
    /// <returns>A partial view with the sale record details.</returns>
    public async Task<IActionResult> ModalSaleRecordDetails(int id)
    {
        var sale = await _investmentService.GetSaleRecordDetailsByIdAsync(id);
        return PartialView("_ModalSaleRecordDetails", sale);
    }
}

