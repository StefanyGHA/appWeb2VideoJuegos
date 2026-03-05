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

		public async Task<IActionResult> Index()
		{
			var juegos = await _context.VideoJuegos.ToListAsync();
			return View(juegos);
		}

		public IActionResult Privacy()
		{
			return View();
		}

	}
}
