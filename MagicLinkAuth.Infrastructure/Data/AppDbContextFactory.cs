using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MagicLinkAuth.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "MagicLinkAuth.Api");
            Console.WriteLine($"BasePath configurado para: {basePath}");

            if (!Directory.Exists(basePath))
            {
                throw new DirectoryNotFoundException($"Diretório não encontrado: {basePath}");
            }

            var jsonPath = Path.Combine(basePath, "appsettings.json");
            Console.WriteLine($"Caminho do appsettings.json: {jsonPath}");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"Arquivo appsettings.json não encontrado no caminho: {jsonPath}");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = config.GetConnectionString("DefaultConnection1");

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
