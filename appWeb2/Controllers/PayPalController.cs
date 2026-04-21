using appWeb2.Services;
using Microsoft.AspNetCore.Mvc;

namespace appWeb2.Controllers
{
	public class PayPalController : Controller
	{
		private readonly PayPalService _payPalService;

		public PayPalController(PayPalService payPalService)
		{
			_payPalService = payPalService;
		}

		public async Task<IActionResult> TestToken()
		{
			var token = await _payPalService.GetAccessTokenAsync();

			if (string.IsNullOrEmpty(token))
			{
				return Content("No se pudo obtener el token.");
			}

			var preview = token.Length > 20 ? token.Substring(0, 20) : token;

			return Content($"Token obtenido correctamente: {preview}...");
		}

		[HttpPost]
		public async Task<IActionResult> CreateOrder(decimal amount)
		{
			var orderId = await _payPalService.CreateOrderAsync(amount);
			return Json(new { id = orderId });
		}

		public async Task<IActionResult> TestOrder()
		{
			var orderId = await _payPalService.CreateOrderAsync(10.00m);
			return Content($"Orden creada: {orderId}");
		}

		[HttpPost]
		public async Task<IActionResult> CaptureOrder(string orderId)
		{
			var result = await _payPalService.CaptureOrderAsync(orderId);
			return Content(result, "application/json");
		}

		public async Task<IActionResult> TestCapture()
		{
			var orderId = await _payPalService.CreateOrderAsync(10.00m);

			if (string.IsNullOrWhiteSpace(orderId))
			{
				return Content("No se pudo crear la orden.");
			}

			var result = await _payPalService.CaptureOrderAsync(orderId);
			return Content(result, "application/json");
		}
	}
}