using System.Runtime.Serialization;
using BlazorAspire.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorAspire.Database.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
    }

    public DbSet<ProductModel> Products { get; set; }

    public DbSet<UserModel> Users { get; set; }

    public DbSet<RoleModel> Roles { get; set; }

    public DbSet<UserRoleModel> UserRoles { get; set; }

    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
}