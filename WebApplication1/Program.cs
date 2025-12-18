using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
public class Program
{
    public static string servername;
    public static string dbname;
    public static string username;
    public static string password;
    public static void Main(string[] args)
    {
        Console.Write("SERVER:");
        servername = Console.ReadLine();
        Console.Write("DATABASE NAME:");
        dbname = Console.ReadLine();
        Console.Write("User name:");
        username = Console.ReadLine();
        Console.Write("password name:");
        password = Console.ReadLine();
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((context, services) =>
                {
                    webBuilder.UseUrls("http://*:5000");

                    string connectionString = $"Server={servername};Database={dbname};User Id={username};Password={password};MultipleActiveResultSets=True;TrustServerCertificate=True";

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    services.AddControllers();
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            });
}