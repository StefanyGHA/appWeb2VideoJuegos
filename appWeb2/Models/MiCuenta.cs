namespace appWeb2.Models
{
	public class MiCuenta
	{
		public int Id { get; set; }

		public string Nombre { get; set; }

		public string? PasswordActual { get; set; }

		public string? NuevaPassword { get; set; }

		public string? ConfirmarPassword { get; set; }
	}
}
