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
	}
}