using DexWallet.Identity.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DexWallet.Identity.Helpers;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}