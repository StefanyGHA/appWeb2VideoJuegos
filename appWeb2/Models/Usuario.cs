using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
	public class Usuario
	{
		[Key]

		public int Id { get; set; }

		[Required]
		public string nombre { get; set; }

		[Required]

		public string correo { get; set; }

		[Required]

		public byte[] password { get; set; }

		public string salt { get; set; }

		[Required]

		public DateTime FechaRegistro { get; set; } = DateTime.Now;


	}
}
