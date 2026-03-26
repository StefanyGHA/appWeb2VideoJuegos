using System.Security.Cryptography;
using System.Text;
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
		[ValidateAntiForgeryToken]

		public IActionResult Login(Login model)
		{
			var user = _context.Usuarios
			.FirstOrDefault(u => u.correo == model.correo);

			if (user != null)
			{
			 string saltedPassword = user.salt + model.password;

			 using (SHA256 sha256 = SHA256.Create()) 
			 {
			  byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
			  byte[] hashBytes = sha256.ComputeHash(inputBytes);

			  if (hashBytes.SequenceEqual(user.password))
			  {
						HttpContext.Session.SetString("usuario", user.nombre);
						return RedirectToAction("Index", "Home");
			  }
			 }
			}

			ViewBag.Error = "Credenciales incorrectas";
			return View();
		}

		public IActionResult Logout()
		{
		 HttpContext.Session.Clear();
			return RedirectToAction("Home");
		}
	}
}
