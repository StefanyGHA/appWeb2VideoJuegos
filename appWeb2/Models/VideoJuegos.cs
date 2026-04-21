using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		public int idcategoria { get; set; }   

		[ForeignKey("idcategoria")]
		public Categoria? Categoria { get; set; }  

		public string descripcion { get; set; }

		public string? imagen { get; set; }

		public DateTime FechaRegistro { get; set; } = DateTime.Now;

		public bool TienePromocion { get; set; }

		public decimal? PrecioAnterior { get; set; }

		public int EdadPermitida { get; set; }


	}
}
