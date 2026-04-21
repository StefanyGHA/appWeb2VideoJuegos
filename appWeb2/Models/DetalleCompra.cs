using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb2.Models
{
	public class DetalleCompra
	{

	 [Key]
	public int Id { get; set; }

	public int VideoJuegosId { get; set; }
	[ForeignKey("VideoJuegosId")]

	public VideoJuegos VideoJuegos { get; set; }

	public int cantidad {  get; set; }

	 public decimal total { get; set; }

	 public string estadoCompra { get; set; }

	 public DateTime fechaHoraTransaccion { get; set; }

	 public string codigoTransaccion {  get; set; }

	 public int idCompra { get; set; }

	 [ForeignKey("idCompra")]

	 public Compra Compra { get; set; }
	}
}
