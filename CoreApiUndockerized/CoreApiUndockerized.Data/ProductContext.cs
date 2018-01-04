using CoreApiUndockerized.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreApiUndockerized.Data
{
  public class ProductContext : IdentityDbContext
  {
    private readonly IConfiguration _config;

    public ProductContext(DbContextOptions options, IConfiguration config)
      : base(options)
    {
      _config = config;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      //builder.Entity<Product>()
      //  .Property(c => c.Id)
      //  .IsRequired();
      //builder.Entity<Product>()
      //  .Property(c => c.Price)
      //  .ValueGeneratedOnAddOrUpdate()
      //  .IsConcurrencyToken();
      //builder.Entity<Speaker>()
      //  .Property(c => c.RowVersion)
      //  .ValueGeneratedOnAddOrUpdate()
      //  .IsConcurrencyToken();
      //builder.Entity<Talk>()
      //  .Property(c => c.RowVersion)
      //  .ValueGeneratedOnAddOrUpdate()
      //  .IsConcurrencyToken();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);

      optionsBuilder.UseSqlServer(_config["ConnectionStrings:DefaultConnection"]);
    }
  }
}
