using appWeb2.Data;
using appWeb2.Filtros;
using appWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace appWeb2.Controllers
{
	public class VideoJuegosController : Controller
	{
		private readonly AppDbContext _context;

		public VideoJuegosController(AppDbContext context)
		{
			_context = context;
		}

		//public async Task<IActionResult> Index()
		//{
		//	var juegos = await _context.VideoJuegos
		//		.Include(v => v.Categoria)
		//		.ToListAsync();

		//	return View(juegos);
		//}

		public IActionResult Create()
		{
			ViewBag.Categorias = new SelectList(_context.categoria.ToList(), "idcategoria", "categoria");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(VideoJuegos juego, IFormFile archivoImagen)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Categorias = new SelectList(_context.categoria.ToList(), "idcategoria", "categoria", juego.idcategoria);
				return View(juego);
			}

			if (archivoImagen != null && archivoImagen.Length > 0)
			{
				var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

				var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", nombreArchivo);

				using (var stream = new FileStream(ruta, FileMode.Create))
				{
					await archivoImagen.CopyToAsync(stream);
				}

				juego.imagen = "/imagenes/" + nombreArchivo;
			}

			juego.FechaRegistro = DateTime.Now;

			_context.VideoJuegos.Add(juego);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Categorias()
		{
			var categorias = _context.categoria.ToList();
			return View(categorias);
		}

		public IActionResult PorCategoria(int id)
		{
			var juegos = _context.VideoJuegos
				.Include(v => v.Categoria)
				.Where(v => v.idcategoria == id)
				.ToList();

			var categoria = _context.categoria.FirstOrDefault(c => c.idcategoria == id);
			ViewBag.Categoria = categoria?.categoria;

			return View(juegos);
		}

		public IActionResult Nuevos()
		{
			var nuevosJuegos = _context.VideoJuegos
				.Include(v => v.Categoria)
				.OrderByDescending(v => v.FechaRegistro)
				.Take(15)
				.ToList();

			return View(nuevosJuegos);
		}

		public IActionResult Promociones()
		{
			var juegosEnPromocion = _context.VideoJuegos
				.Include(v => v.Categoria)
				.Where(v => v.TienePromocion)
				.OrderByDescending(v => v.FechaRegistro)
				.ToList();

			return View(juegosEnPromocion);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var juego = await _context.VideoJuegos.FindAsync(id);
			if (juego == null) return NotFound();

			ViewBag.Categorias = new SelectList(_context.categoria.ToList(), "idcategoria", "categoria", juego.idcategoria);
			return View(juego);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, VideoJuegos juego, IFormFile? archivoImagen)
		{
			if (id != juego.Id)
				return NotFound();

			var juegoDB = await _context.VideoJuegos.FindAsync(juego.Id);

			if (juegoDB == null)
				return NotFound();

			if (!ModelState.IsValid)
			{
				ViewBag.Categorias = new SelectList(_context.categoria.ToList(), "idcategoria", "categoria", juego.idcategoria);
				return View(juego);
			}

			juegoDB.titulo = juego.titulo;
			juegoDB.precio = juego.precio;
			juegoDB.idcategoria = juego.idcategoria;
			juegoDB.descripcion = juego.descripcion;
			juegoDB.EdadPermitida = juego.EdadPermitida;
			juegoDB.TienePromocion = juego.TienePromocion;
			juegoDB.PrecioAnterior = juego.PrecioAnterior;

			if (archivoImagen != null && archivoImagen.Length > 0)
			{
				if (!string.IsNullOrEmpty(juegoDB.imagen))
				{
					var rutaAnterior = Path.Combine(
						Directory.GetCurrentDirectory(),
						"wwwroot",
						juegoDB.imagen.TrimStart('/')
					);

					if (System.IO.File.Exists(rutaAnterior))
						System.IO.File.Delete(rutaAnterior);
				}

				var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

				var rutaNueva = Path.Combine(
					Directory.GetCurrentDirectory(),
					"wwwroot",
					"imagenes",
					nombreArchivo
				);

				using (var stream = new FileStream(rutaNueva, FileMode.Create))
				{
					await archivoImagen.CopyToAsync(stream);
				}

				juegoDB.imagen = "/imagenes/" + nombreArchivo;
			}

			_context.Update(juegoDB);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var juego = await _context.VideoJuegos
				.Include(v => v.Categoria)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (juego == null) return NotFound();

			return View(juego);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var juego = await _context.VideoJuegos.FindAsync(id);

			if (juego != null)
			{
				_context.VideoJuegos.Remove(juego);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
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

		[SessionAuthorize]
		public async Task<IActionResult> MisCompras(DateTime? desde, DateTime? hasta, string videojuego)
		{
			var usuarioId = HttpContext.Session.GetInt32("usuarioId");

			var query = _context.detalle_compra
				.Include(d => d.Compra)
				.Include(d => d.VideoJuegos)
				.Where(d => d.Compra.UsuarioId == usuarioId)
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

			return View(compras);
		}
	}
}
