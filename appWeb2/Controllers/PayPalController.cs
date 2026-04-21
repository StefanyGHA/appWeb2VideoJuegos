using appWeb2.Data;
using appWeb2.Models;
using appWeb2.Services;
using Microsoft.AspNetCore.Mvc;

namespace appWeb2.Controllers
{
	public class PayPalController : Controller
	{
		private readonly PayPalService _payPalService;
		private readonly AppDbContext _context;

		public PayPalController(PayPalService payPalService, AppDbContext context)
		{
			_payPalService = payPalService;
			_context = context;
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
		public async Task<IActionResult> CaptureOrder(string orderId, int videojuegoId, int cantidad, decimal total)
		{
			var result = await _payPalService.CaptureOrderAsync(orderId);

			// 1. Crear compra
			var compra = new Compra
			{
				FechaCompra = DateTime.Now,
				UsuarioId = 1
			};

			_context.Compras.Add(compra);
			await _context.SaveChangesAsync();

			// 2. Crear detalle
			var detalle = new DetalleCompra
			{
				VideoJuegosId = videojuegoId,
				cantidad = cantidad,
				total = total,
				estadoCompra = "COMPLETADO",
				fechaHoraTransaccion = DateTime.Now,
				codigoTransaccion = orderId,
				idCompra = compra.Id
			};

			_context.detalle_compra.Add(detalle);
			await _context.SaveChangesAsync();

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