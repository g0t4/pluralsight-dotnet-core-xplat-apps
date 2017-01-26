
namespace CheckLinksConsole
{
    using Microsoft.EntityFrameworkCore;

	public class LinksDb : DbContext 
	{
        public DbSet<LinkCheckResult> Links {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // MSSQL:
            //var connection = @"Server=localhost;Database=Links;User Id=sa;Password=whatever12!";
            //optionsBuilder.UseSqlServer(connection);

            // MySQL (Pomelo):
            //var connection = "server=localhost;userid=root;pwd=password;database=Links;sslmode=none;";
            //optionsBuilder.UseMySql(connection);

            // PostgreSQL (Npgsql):
            var connection = "Host=localhost;Database=Links;Username=postgres;Password=password";
            optionsBuilder.UseNpgsql(connection);
        }
	}
}
