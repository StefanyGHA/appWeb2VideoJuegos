using System.Diagnostics;
using appWeb2.Data;
using appWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appWeb2.Controllers
{
	public class HomeController : Controller
	{

		private readonly AppDbContext _context;
		

		public HomeController(AppDbContext context)
		{
			_context = context; 
		}

		public async Task<IActionResult> Index(int pagina = 1)
		{
			int cantidad = 10;

			var totalRegistros = await _context.VideoJuegos.CountAsync();

			var juegos = await _context.VideoJuegos
				.Include(v => v.Categoria)
				.OrderBy(v => v.titulo)
				.Skip((pagina - 1) * cantidad)
				.Take(cantidad)
				.ToListAsync();

			ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidad);
			ViewBag.PaginaActual = pagina;

			return View(juegos);
		}

		public IActionResult Privacy()
		{
			return View();
		}

	}
}
