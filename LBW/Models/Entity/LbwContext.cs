using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LBW.Models.Entity
{
    public partial class LbwContext : DbContext
    {
        public LbwContext()
        {
        }

        public LbwContext(DbContextOptions<LbwContext> options)
           : base(options)
        {
        }

        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<SubProducto> SubProductos { get; set; }
        public virtual DbSet<Estado> Estados { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //    optionsBuilder.UseSqlServer("Server=LAPTOP-5GJJMNSE;Database=USAEU2GIG-DEV-SQL;Trusted_Connection=True;");
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.CategoriaID);
                entity.ToTable("Categoria");

                entity.Property(e => e.Nombre).IsRequired();
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.ProductoID);
                entity.ToTable("Producto");

                entity.Property(e => e.Nombre).IsRequired();

                entity.HasOne(d => d.IdCatNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.CategoriaID)
                    .HasConstraintName("FK_Producto_Categoria");
            });

            modelBuilder.Entity<SubProducto>(entity =>
            {
                entity.HasKey(e => e.SubProductoID);
                entity.ToTable("SubProducto");

                entity.Property(e => e.Nombre).IsRequired();

                entity.HasOne(d => d.IdProNavigation)
                    .WithMany(p => p.SubProductos)
                    .HasForeignKey(d => d.ProductoID)
                    .HasConstraintName("FK_SubProducto_Producto");

                entity.HasOne(d => d.IdEstNavigation)
                    .WithMany(p => p.SubProductos)
                    .HasForeignKey(d => d.EstadoID)
                    .HasConstraintName("FK_SubProducto_Estado");
            });

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasKey(e => e.EstadoID);
                entity.ToTable("Estado");

                entity.Property(e => e.Nombre).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}