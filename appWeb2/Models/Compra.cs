using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb2.Models
{
	public class Compra
	{
		[Key]

		public int Id { get; set; }

		public DateTime FechaCompra {  get; set; } = DateTime.Now;
		[Required]

		public int UsuarioId { get; set; }
		[ForeignKey("UsuarioId")]

		public Usuario Usuario { get; set; }

		//public int VideoJuegosId { get; set; }
		//[ForeignKey("VideoJuegosId")]

		//public VideoJuegos VideoJuegos { get; set; }
	}
}
