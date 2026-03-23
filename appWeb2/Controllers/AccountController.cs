using appWeb2.Data;
using appWeb2.Models;
using Microsoft.AspNetCore.Mvc;

namespace appWeb2.Controllers
{
	public class AccountController : Controller
	{
		private readonly AppDbContext _context;

		public AccountController(AppDbContext context)
		{
		  _context = context;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]

		public IActionResult Login(Login model)
		{
		    var user = _context.Usuarios
			.FirstOrDefault(u => u.correo == model.correo && u.password == model.password);

			if (user == null) 
			{
			  HttpContext.Session.SetString("usuario", user.nombre);
			  Console.WriteLine("Usuario logueado: " + user.nombre);
				return RedirectToAction("Index", "Home");
			}

			ViewBag.Error = "Credenciales incorrectas";
			return View();
		}

		public IActionResult Logout()
		{
		 HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}
}
