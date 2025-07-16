using Microsoft.EntityFrameworkCore;

namespace BackEnd.API.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }
    public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Txsk> Txsks { get; set; }
    public DbSet<TxskStatus> TxskStatuses { get; set; }
    public DbSet<TxskType> TxskTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:DevConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(e => e.Company).HasMaxLength(60);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.FirstName).HasMaxLength(60);
            entity.Property(e => e.LastName).HasMaxLength(60);
            entity.Property(e => e.Phone).HasMaxLength(60);
        });

        modelBuilder.Entity<TxskStatus>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(60);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
