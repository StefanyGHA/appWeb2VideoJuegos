using System.Security.Cryptography;
using System.Text;
using appWeb2.Data;
using appWeb2.Filtros;
using appWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appWeb2.Controllers
{
	public class AccountController : Controller
	{
		private readonly AppDbContext _context;

		public AccountController(AppDbContext context)
		{
		  _context = context;
		}
		public IActionResult Login()
		{
			return View();
	 	}


		[SessionAuthorize]

		public IActionResult Dashboard()
		{
			//var data = (from v in _context.VideoJuegos
			//join c in _context.categoria
			//on v.idcategoria equals c.idcategoria
			//group v by c.idcategoria into g
			//select new
			//{
			//	Categoria = g.Key,
			//	Total = g.Count()
			//}).ToList();
			// ViewBag.Categorias = data.Select(x => x.Categoria).ToList();
			//ViewBag.Totales = data.Select(x=> x.Total).ToList();

			var categorias = _context.categoria
	            .Select(c => c.categoria)
	            .OrderBy(c => c)
	            .ToList();

			ViewBag.Categorias = categorias;

			return View();
		}

		public IActionResult ObtenerDatos(string categoria)
		{
			var query = from v in _context.VideoJuegos
						join c in _context.categoria
						on v.idcategoria equals c.idcategoria
						select new { c.categoria };

            if(!string.IsNullOrEmpty(categoria))
			{
				query = query.Where(x => x.categoria == categoria);
			}

			var data = query
			    .GroupBy(x => x.categoria)
				.Select(g => new
				{
				  categoria = g.Key,
				  total = g.Count()
				}).ToList();
			return Json(data);

		
		}

		public IActionResult ObtenerDatosPastel()
		{
			var data = (from v in _context.VideoJuegos
						join c in _context.categoria
						on v.idcategoria equals c.idcategoria
						group c by c.categoria into g
						select new
						{
							name = g.Key,
							y = g.Count()
						}).ToList();

			return Json(data);
		}

		public IActionResult ObtenerTopCategorias()
		{
			var data = (from v in _context.VideoJuegos
						join c in _context.categoria
						on v.idcategoria equals c.idcategoria
						group c by c.categoria into g
						orderby g.Count() descending
						select new
						{
							categoria = g.Key,
							total = g.Count()
						})
						.Take(5)
						.ToList();

			return Json(data);
		}

		public IActionResult ObtenerVideojuegosPorMes()
		{
			var data = _context.VideoJuegos
				.GroupBy(v => new { v.FechaRegistro.Year, v.FechaRegistro.Month })
				.Select(g => new
				{
					year = g.Key.Year,
					month = g.Key.Month,
					total = g.Count()
				})
				.OrderBy(x => x.year)
				.ThenBy(x => x.month)
				.ToList()
				.Select(x => new
				{
					periodo = x.month.ToString("00") + "/" + x.year,
					total = x.total
				});

			return Json(data);
		}

		public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, int pagina = 1)
		{
			int paginador = 10;

			var query = _context.detalle_compra
				.Include(d => d.Compra)
				.Include(c=>c.VideoJuegos)
				.AsQueryable();

			if (desde.HasValue)
			{
				query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
			}
			if (hasta.HasValue)
			{
			    query= query.Where(d=> d.fechaHoraTransaccion  <= hasta.Value);
			}

			var totalregistros = await query.CountAsync();

			var datos = await query
			    .OrderByDescending(d => d.fechaHoraTransaccion)
				.Skip((pagina -1) * paginador)
				.Select(d=> new VentaViewModel
				{
					UsuarioId = d.idCompra,
					VideoJuegosId = d.VideoJuegosId,
					titulo=d.VideoJuegos.titulo,
					cantidad = d.cantidad,
					total = d.total,
					estadoCompra = d.estadoCompra,
					fechaHoraTransacion = d.fechaHoraTransaccion,
					codigoTransaccion = d.codigoTransaccion
				}).ToListAsync();

			ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalregistros / paginador);
			ViewBag.PaginaActual = pagina;
			ViewBag.Desde = desde;
			ViewBag.Hasta = hasta;

			return View(datos);
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
			  //byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
					byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
					byte[] hashBytes = sha256.ComputeHash(inputBytes);

					Console.WriteLine("Salt DB: " + user.salt);
					Console.WriteLine("Password input: " + model.password);
					Console.WriteLine("Salted: " + (user.salt + model.password));

					Console.WriteLine("Hash generado: " + Convert.ToBase64String(hashBytes));
					Console.WriteLine("Hash DB: " + Convert.ToBase64String(user.password));

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
