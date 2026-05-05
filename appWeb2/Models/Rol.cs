using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace appWeb2.Models
{
	public class Rol
	{
	[Key]
	public int IdRol { get; set; }

	public string rol{ get; set; }
	}
}
