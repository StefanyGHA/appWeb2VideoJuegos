using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
	public class VentaViewModel
	{
		public int Id { get; set; }

		public DateTime FechaCompra { get; set; } 
	
		public int UsuarioId { get; set; }
		public int VideoJuegosId { get;set; }
		public string titulo { get; set; }
		public int cantidad {  get; set; }
		public decimal total { get; set; }
		public string estadoCompra { get; set; }
		public DateTime fechaHoraTransacion { get; set; }

		public string codigoTransaccion { get; set; }

	}
}
