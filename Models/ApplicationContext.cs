using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConsoleApp5.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }
        

        public class SampleContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
        {
            public ApplicationContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

                // получаем конфигурацию из файла appsettings.json
                ConfigurationBuilder builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                IConfigurationRoot config = builder.Build();

                // получаем строку подключения из файла appsettings.json
                string? connectionString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
                return new ApplicationContext(optionsBuilder.Options);
            }
        }
    }
}