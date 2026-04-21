using appWeb2.Models;
using Microsoft.EntityFrameworkCore;

namespace appWeb2.Data
{
	public class AppDbContext : DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext> options) 
			: base(options) { }

		public DbSet<Usuario>Usuarios { get; set; }

		public DbSet<VideoJuegos>VideoJuegos { get; set; }

		public DbSet<Compra> Compras { get; set; }

		public DbSet<Categoria> categoria { get; set; }

		public DbSet<DetalleCompra>detalle_compra { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<VideoJuegos>()
				.Property(v => v.precio)
				.HasPrecision(10, 2);

			modelBuilder.Entity<VideoJuegos>()
				.Property(v => v.PrecioAnterior)
				.HasPrecision(10, 2);

			modelBuilder.Entity<VideoJuegos>()
				.HasOne(v => v.Categoria)
				.WithMany(c => c.VideoJuegos)
				.HasForeignKey(v => v.idcategoria)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}


