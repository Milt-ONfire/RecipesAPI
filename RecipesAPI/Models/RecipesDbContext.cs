using Microsoft.EntityFrameworkCore;

namespace RecipesAPI.Models;

public partial class RecipesDbContext : DbContext
{
    public RecipesDbContext()
    {
    }

    public RecipesDbContext(DbContextOptions<RecipesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<RecetaIngrediente> RecetaIngredientes { get; set; }

    public virtual DbSet<Receta> Receta { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<RecetaGuardadaUsuario> RecetaGuardadaUsuarios { get; set; }

    public virtual DbSet<Calificacion> Calificacion { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente);

            entity.ToTable("INGREDIENTE");

            entity.Property(e => e.IdIngrediente)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .HasColumnName("id_ingrediente");
            entity.Property(e => e.NombreIngrediente)
                .HasMaxLength(20)
                .HasColumnName("nombre_ingrediente");
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(5)
                .HasColumnName("unidad_medida");
        });

        modelBuilder.Entity<RecetaIngrediente>(entity =>
        {
            entity.HasKey(e => e.IdRecetaIngrediente);

            entity.ToTable("RECETA_INGREDIENTE");

            entity.Property(e => e.IdRecetaIngrediente)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .HasColumnName("id_receta_ingrediente");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdIngrediente).HasColumnName("id_ingrediente");
            entity.Property(e => e.IdReceta).HasColumnName("id_receta");

            entity.HasOne(d => d.IdIngredienteNavigation).WithMany(p => p.RecetaIngredientes)
                .HasForeignKey(d => d.IdIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_INGREDIENTE_RECETA_INGREDIENTE");

            entity.HasOne(d => d.IdRecetaNavigation).WithMany(p => p.RecetaIngredientes)
                .HasForeignKey(d => d.IdReceta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RECETA_RECETA_INGREDIENTE");
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.HasKey(e => e.IdReceta);

            entity.ToTable("RECETA");

            entity.Property(e => e.IdReceta)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .HasColumnName("id_receta");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .HasColumnName("categoria");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Imagen)
                .HasMaxLength(50)
                .HasColumnName("imagen");
            entity.Property(e => e.NombreReceta)
                .HasMaxLength(200)
                .HasColumnName("nombre_receta");
            entity.Property(e => e.IdCategoria)
                .HasColumnName("id_categoria");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Receta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RECETA_USUARIO");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Receta)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RECETA_CATEGORIA");
        });

        modelBuilder.Entity<RecetaGuardadaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRecetaGuardada);

            entity.ToTable("RECETAS_GUARDADAS_USUARIO");

            entity.Property(e => e.IdRecetaGuardada)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .HasColumnName("id_receta_guardada");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdReceta).HasColumnName("id_receta");

            entity.HasOne(d => d.IdRecetaNavigation).WithMany(p => p.RecetaGuardada)
                .HasForeignKey(d => d.IdReceta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RECETA_RECETA_GUARDADA");
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RecetaGuardada)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USUARIO_RECETAS_GUARDADAS");

        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("USUARIO");

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_usuario");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("contraseña");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .HasColumnName("email");
            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .HasColumnName("imagen");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .HasColumnName("nombre_usuario");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoría);

            entity.ToTable("CATEGORIAS");

            entity.Property(e => e.IdCategoría)
                  .ValueGeneratedOnAdd()
                  .HasColumnName("id_categoria");
            entity.Property(e => e.NombreCategoria)
                  .HasMaxLength(30)
                  .HasColumnName("nombreCategoria");
            entity.Property(e => e.ImagenCategoria)
                  .HasMaxLength(100)
                  .HasColumnName("imagenCategoria");
        });

        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.HasKey(e => e.IdCalificacion);

            entity.ToTable("CALIFICACION");

            entity.Property(e => e.IdCalificacion).ValueGeneratedOnAdd().HasColumnName("id_calificacion").UseIdentityColumn();
            entity.Property(e => e.IdReceta).HasColumnName("id_receta");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.FechaCalificacion)
                  .HasColumnName("fecha")
                  .HasColumnType("datetime2")
                  .HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Comentarios).HasMaxLength(50).HasColumnName("comentario").HasDefaultValue("");

            entity.HasOne(e => e.Receta).WithMany(p => p.Calificaciones)
                  .HasForeignKey(r => r.IdReceta)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_CALIFICACION_RECETA");

            entity.HasOne(e => e.Usuario).WithMany(p => p.Calificaciones)
                  .HasForeignKey(u => u.IdUsuario)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_CALIFICACION_USUARIO");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
