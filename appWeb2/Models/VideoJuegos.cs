using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
	public class VideoJuegos
	{
		[Key]

		public int Id { get; set; }
		[Required]

		public string titulo { get; set; }
		[Required]

		public decimal precio { get; set; }
		[Required]

		public string categoria { get; set; }
		[Required]

		public string descripcion { get; set; }

		public string? imagen { get; set; }

		public DateTime FechaRegistro { get; set; } = DateTime.Now;

		public bool TienePromocion { get; set; }

		public decimal? PrecioAnterior { get; set; }

		public int EdadPermitida { get; set; }

		//public ICollection<Compra> Compras { get; set; }

	}
}
