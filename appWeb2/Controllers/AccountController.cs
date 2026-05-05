using System.Security.Cryptography;
using System.Text;
using appWeb2.Data;
using appWeb2.Filtros;
using appWeb2.Models;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Linq;
using static appWeb2.Filtros.SessionAuthorize;


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
			
			var rol = HttpContext.Session.GetInt32("Idrol");
			if (rol != 1)
			{
				return RedirectToAction("Index", "Videojuegos");
			}
			else
			{

				var categorias = _context.categoria
					.Select(c => c.categoria)
					.OrderBy(c => c)
					.ToList();

					ViewBag.Categorias = categorias;
				return View();
			}
		}

//Graficas 	
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

		public IActionResult ObtenerVideojuegosPorEdad()
		{
			var data = _context.VideoJuegos
				.GroupBy(v => v.EdadPermitida)
				.Select(g => new
				{
					edad = "+" + g.Key.ToString(),
					total = g.Count()
				})
				.OrderBy(x => x.edad)
				.ToList();

			return Json(data);
		}

		[Filtros.AdminAuthorize]
		public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, string cliente,string videojuego, int pagina = 1)
		{
			int paginador = 10;

			var query = _context.detalle_compra
				.Include(d => d.Compra)
				.ThenInclude(c => c.Usuario)
				.Include(c=>c.VideoJuegos)
				.AsQueryable();

			if (desde.HasValue)
			{
				query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
			}

			if (hasta.HasValue)
			{
				query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);
			}

			if (!string.IsNullOrWhiteSpace(cliente))
			{
				query = query.Where(d => d.Compra.Usuario.nombre.Contains(cliente));
			}

			if (!string.IsNullOrWhiteSpace(videojuego))
			{
				query = query.Where(d => d.VideoJuegos.titulo.Contains(videojuego));
			}

			var totalregistros = await query.CountAsync();

			var datos = await query
			    .OrderByDescending(d => d.fechaHoraTransaccion)
				.Skip((pagina -1) * paginador)
				.Take(paginador)
				.Select(d=> new VentaViewModel
				{
					NombreCliente = d.Compra.Usuario.nombre,
					NombreVideojuego = d.VideoJuegos.titulo,

					UsuarioId = d.Compra.UsuarioId,
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
			ViewBag.ClienteId = cliente;
			ViewBag.VideojuegoId = videojuego;

			return View(datos);
		}

		[SessionAuthorize.AdminAuthorize]
		public async Task<IActionResult> Usuarios()
		{
			var usuarios = await _context.Usuarios
				.Include(u => u.Rol)
				.ToListAsync();

			return View(usuarios);
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
					byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
					byte[] hashBytes = sha256.ComputeHash(inputBytes);

			  if (hashBytes.SequenceEqual(user.password))
			  {
						HttpContext.Session.SetString("usuario", user.nombre);
						HttpContext.Session.SetInt32("usuarioId", user.Id);
						HttpContext.Session.SetInt32("Idrol", user.IdRol);
						
						if (user.IdRol == 1)
						{
							return RedirectToAction("Dashboard", "Account");
						}

						if (user.IdRol == 2) 
						{
							return RedirectToAction("Index", "Home");

						}
			    }
			  }
			}

			ViewBag.Error = "Credenciales incorrectas";
			return View();
		}

		
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(Usuario model, string password, string confirmPassword)
		{
			if (password != confirmPassword)
			{
				ViewBag.Error = "Las contraseñas no coinciden";
				return View();
			}

			var salt = Guid.NewGuid().ToString();

			using var sha256 = SHA256.Create();
			var passwordBytes = Encoding.Unicode.GetBytes(salt + password);
			var hash = sha256.ComputeHash(passwordBytes);

			model.salt = salt;
			model.password = hash;
			model.FechaRegistro = DateTime.Now;
			model.IdRol = 2; 

			_context.Usuarios.Add(model);
			_context.SaveChanges();

			return RedirectToAction("Login");
		}

		[SessionAuthorize]
		public async Task<IActionResult> MiCuenta()
		{
			var usuarioId = HttpContext.Session.GetInt32("usuarioId");

			if (usuarioId == null)
				return RedirectToAction("Login", "Account");

			var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);

			if (usuario == null)
				return NotFound();


			return View(usuario);
		}

		[HttpPost]
		public async Task<IActionResult> MiCuenta(int id, string nombre, string passwordActual, string nuevapassword, string confirmarpassword)
		{
			var usuarioId = HttpContext.Session.GetInt32("usuarioId");

			if (usuarioId == null)
				return RedirectToAction("Login", "Account");

			var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);

			if (usuario == null)
				return NotFound();

			usuario.nombre = nombre;

			if (!string.IsNullOrWhiteSpace(nuevapassword))
			{
				if (string.IsNullOrWhiteSpace(passwordActual))
				{
					ViewBag.Error = "Debe ingresar la contraseña actual.";
					return View(usuario);
				}

				var passwordActualBytes = Encoding.UTF8.GetBytes(passwordActual);

				if (!usuario.password.SequenceEqual(passwordActualBytes))
				{
					ViewBag.Error = "La contraseña actual no es correcta.";
					return View(usuario);
				}

				if (nuevapassword != confirmarpassword)
				{
					ViewBag.Error = "La nueva contraseña no coincide.";
					return View(usuario);
				}

				usuario.password = Encoding.UTF8.GetBytes(nuevapassword);
			}

			await _context.SaveChangesAsync();

			HttpContext.Session.SetString("usuario", usuario.nombre);

			ViewBag.Mensaje = "Cuenta actualizada correctamente.";
			return View(usuario);
		}


		public async Task<IActionResult> VerUsuarios()
		{
			var idRol = HttpContext.Session.GetInt32("Idrol");

			if (idRol != 1)
			{
				return RedirectToAction("Login", "Account");
			}

			var usuarios = await _context.Usuarios
				.Include(u => u.Rol)
				.ToListAsync();

			return View(usuarios);
		}

		public async Task<IActionResult> ExportarPDF(DateTime? desde, DateTime? hasta, string cliente, string videojuego )
		{
			var query = _context.detalle_compra
				.Include(d => d.Compra)
				.ThenInclude(c => c.Usuario)
				.Include(c => c.VideoJuegos)
				.AsQueryable();

			if (desde.HasValue)
			{
				query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
			}

			if (hasta.HasValue)
			{
				query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);
			}

			if (!string.IsNullOrWhiteSpace(cliente))
			{
				query = query.Where(d => d.Compra.Usuario.nombre.Contains(cliente));
			}

			if (!string.IsNullOrWhiteSpace(videojuego))
			{
				query = query.Where(d => d.VideoJuegos.titulo.Contains(videojuego));
			}

			var datos = await query
				.OrderByDescending(d => d.fechaHoraTransaccion)
				.Select(d => new VentaViewModel
				{
					NombreCliente = d.Compra.Usuario.nombre,
					NombreVideojuego = d.VideoJuegos.titulo,

					titulo = d.VideoJuegos.titulo,
					cantidad = d.cantidad,
					total = d.total,
					estadoCompra = d.estadoCompra,
					fechaHoraTransacion = d.fechaHoraTransaccion,
					codigoTransaccion = d.codigoTransaccion
				}).ToListAsync();

			ViewBag.Desde = desde;
			ViewBag.Hasta = hasta;
			ViewBag.Cliente = cliente;
			ViewBag.Videojuego = videojuego;

			return new ViewAsPdf("PdfVentas", datos)
			{
				FileName = $"Ventas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
				PageSize = Rotativa.AspNetCore.Options.Size.A4,
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
				CustomSwitches = "--footer-center \"Página [page] de [toPage]\" --footer-font-size 9 --footer-spacing 5"
			};
		}

		[SessionAuthorize]
		public async Task<IActionResult> MisCompras(DateTime? desde, DateTime? hasta, string videojuego)
		{
			var usuarioId = HttpContext.Session.GetInt32("usuarioId");

			if (usuarioId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var query = _context.detalle_compra
				.Include(d => d.Compra)
				.Include(d => d.VideoJuegos)
				.Where(d => d.Compra.UsuarioId == usuarioId.Value)
				.AsQueryable();

			if (desde.HasValue)
				query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);

			if (hasta.HasValue)
				query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);

			if (!string.IsNullOrEmpty(videojuego))
				query = query.Where(d => d.VideoJuegos.titulo.Contains(videojuego));

			var compras = await query
				.OrderByDescending(d => d.fechaHoraTransaccion)
				.ToListAsync();

			ViewBag.Desde = desde;
			ViewBag.Hasta = hasta;
			ViewBag.Videojuego = videojuego;

			return View(compras);
		}

		[SessionAuthorize]
		[SessionAuthorize]
		public async Task<IActionResult> ReporteMisComprasPdf(DateTime? desde, DateTime? hasta, string videojuego)
		{
			var usuarioId = HttpContext.Session.GetInt32("usuarioId");

			if (usuarioId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var query = _context.detalle_compra
				.Include(d => d.Compra)
				.Include(d => d.VideoJuegos)
				.Where(d => d.Compra.UsuarioId == usuarioId.Value)
				.AsQueryable();

			if (desde.HasValue)
				query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);

			if (hasta.HasValue)
				query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);

			if (!string.IsNullOrEmpty(videojuego))
				query = query.Where(d => d.VideoJuegos.titulo.Contains(videojuego));

			var datos = await query
				.OrderByDescending(d => d.fechaHoraTransaccion)
				.ToListAsync();

			return new ViewAsPdf("PdfCompras", datos)
			{
				FileName = $"MisCompras_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
				PageSize = Rotativa.AspNetCore.Options.Size.A4,
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
				PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 20, 20, 20)
			};
		}
		//Usuarios
		public async Task<IActionResult> DetalleUsuario(int id)
		{
			if (HttpContext.Session.GetInt32("Idrol") != 1)
			{
				return RedirectToAction("Login", "Account");
			}

			var usuario = await _context.Usuarios
				.Include(u => u.Rol)
				.FirstOrDefaultAsync(u => u.Id == id);

			if (usuario == null)
			{
				return NotFound();
			}

			return View(usuario);
		}

		public async Task<IActionResult> EditarUsuario(int id)
		{
			if (HttpContext.Session.GetInt32("Idrol") != 1)
				return RedirectToAction("Login", "Account");

			var usuario = await _context.Usuarios
				.Include(u => u.Rol)
				.FirstOrDefaultAsync(u => u.Id == id);

			if (usuario == null)
				return NotFound();

			ViewBag.Roles = await _context.Rol.ToListAsync();

			return View(usuario);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditarUsuario(Usuario usuario)
		{
			if (HttpContext.Session.GetInt32("Idrol") != 1)
				return RedirectToAction("Login", "Account");

			var usuarioBD = await _context.Usuarios.FindAsync(usuario.Id);

			if (usuarioBD == null)
				return NotFound();

			usuarioBD.nombre = usuario.nombre;
			usuarioBD.correo = usuario.correo;
			usuarioBD.IdRol = usuario.IdRol;

			await _context.SaveChangesAsync();

			return RedirectToAction("VerUsuarios");
		}

		public async Task<IActionResult> EliminarUsuario(int id)
		{
			if (HttpContext.Session.GetInt32("Idrol") != 1)
				return RedirectToAction("Login", "Account");

			var usuario = await _context.Usuarios
				.Include(u => u.Rol)
				.FirstOrDefaultAsync(u => u.Id == id);

			if (usuario == null)
				return NotFound();

			return View(usuario);
		}

		[HttpPost, ActionName("EliminarUsuario")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmarEliminarUsuario(int id)
		{
			if (HttpContext.Session.GetInt32("Idrol") != 1)
				return RedirectToAction("Login", "Account");

			var usuario = await _context.Usuarios.FindAsync(id);

			if (usuario == null)
				return NotFound();

			_context.Usuarios.Remove(usuario);
			await _context.SaveChangesAsync();

			return RedirectToAction("VerUsuarios");
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index", "Home");
		}
	}
}
