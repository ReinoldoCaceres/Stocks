using System;
using Microsoft.AspNetCore.Mvc;
using ApiDemo2.Services;

namespace ApiDemo2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockService _stockService;

        public StockController(StockService stockService)
        {
            _stockService = stockService;
        }


        [HttpGet("GetStockData")]
        public async Task<IActionResult> GetDataAsync(string symbol)
        {
            try
            {
                var data = await _stockService.GetDataAsync(symbol);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetStockDataByDate")]
        public async Task<IActionResult> GetStockDataByDateAsync(string symbol, DateTime date)
        {
            try
            {
                var data = await _stockService.GetStockDataByDateAsync(symbol, date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetLastTwoDaysData")]
        public async Task<IActionResult> GetLastTwoDaysDataAsync(string symbol)
        {
            try
            {
                var data = await _stockService.GetLastTwoDaysDataAsync(symbol);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetClosePricesLastTwoDays")]
        public async Task<IActionResult> GetClosePricesLastTwoDaysAsync(string symbol)
        {
            try
            {
                var data = await _stockService.GetLastTwoDaysVarianceAsync(symbol);
                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
