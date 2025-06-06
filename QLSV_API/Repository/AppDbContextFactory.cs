using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QLSV_API.Repository
{
	public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
	{
		public AppDbContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			// Xử lý URL encoding cho password nếu cần
			if (connectionString.Contains("htuan15424@") && !connectionString.Contains("htuan15424%40"))
			{
				connectionString = connectionString.Replace("htuan15424@", "htuan15424%40");
			}
			// Đổi từ UseSqlServer sang UseNpgsql cho PostgreSQL
			optionsBuilder.UseNpgsql(connectionString);

			return new AppDbContext(optionsBuilder.Options);
		}
	}
}