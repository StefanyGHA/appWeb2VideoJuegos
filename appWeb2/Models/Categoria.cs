using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
	public class Categoria
	{
	   [Key]

	 public int idcategoria { get; set;  }

	 [Required]
	 public string categoria { get; set; }

     public ICollection<VideoJuegos> VideoJuegos { get; set; } = new List<VideoJuegos>();
	}
}
