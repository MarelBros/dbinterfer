using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

Console.Write("SERVER: ");
string servername = Console.ReadLine();

Console.Write("DATABASE NAME: ");
string dbname = Console.ReadLine();

Console.Write("User name: ");
string username = Console.ReadLine();

Console.Write("Password: ");
string password = Console.ReadLine();


string connectionString =
    $"Server={servername};Database={dbname};User Id={username};Password={password};" +
    $"MultipleActiveResultSets=True;TrustServerCertificate=True";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();


builder.WebHost.UseUrls("http://*:5000");

var app = builder.Build();


app.UseRouting();
app.MapControllers();

app.Run();



public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierID);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Warehouse)
            .WithMany(w => w.Products)
            .HasForeignKey(p => p.WarehouseID);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Product)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.ProductID);
    }
}
