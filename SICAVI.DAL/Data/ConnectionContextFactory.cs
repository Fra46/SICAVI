using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace SICAVI.DAL.Data
{
    public class ConnectionContextFactory : IDesignTimeDbContextFactory<ConnectionContext>
    {
        public ConnectionContext CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "SICAVI", "sicavi.db"
            );

            var optionsBuilder = new DbContextOptionsBuilder<ConnectionContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new ConnectionContext(optionsBuilder.Options);
        }
    }
}