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
			try
			{
				// Cấu hình timezone cho Npgsql
				AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

				// Tạo configuration builder với error handling
				var configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile("appsettings.Development.json", optional: true)
					.Build();

				var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
				var connectionString = configuration.GetConnectionString("DefaultConnection");

				// Fallback connection string nếu không đọc được
				if (string.IsNullOrEmpty(connectionString))
				{
					Console.WriteLine("⚠️ Using fallback connection string");
					connectionString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Database=postgres;Username=postgres.lpfczxkyuvntotmxgbxy;Password=htuan15424@;Port=6543;SSL Mode=Require;Trust Server Certificate=true;Pooling=false;No Reset On Close=true;Enlist=false;Connection Idle Lifetime=300;Include Error Detail=true;Timeout=60;Command Timeout=180;Application Name=QLSV-API;";
				}

				// URL decode password nếu cần thiết
				if (connectionString.Contains("htuan15424%40"))
				{
					connectionString = connectionString.Replace("htuan15424%40", "htuan15424@");
				}

				Console.WriteLine($"🔗 Design-time connection: {MaskPassword(connectionString)}");

				// Cấu hình Npgsql với retry và timeout
				optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
				{
					npgsqlOptions.EnableRetryOnFailure(
						maxRetryCount: 5,
						maxRetryDelay: TimeSpan.FromSeconds(30),
						errorCodesToAdd: null
					);
					npgsqlOptions.CommandTimeout(180); // 3 minutes timeout
				});

				// Enable sensitive data logging cho development
				optionsBuilder.EnableSensitiveDataLogging();
				optionsBuilder.EnableDetailedErrors();

				return new AppDbContext(optionsBuilder.Options);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error creating DbContext: {ex.Message}");
				Console.WriteLine($"📝 Stack trace: {ex.StackTrace}");
				throw;
			}
		}

		private static string MaskPassword(string connectionString)
		{
			if (connectionString.Contains("Password="))
			{
				var start = connectionString.IndexOf("Password=") + "Password=".Length;
				var end = connectionString.IndexOf(";", start);
				if (end == -1) end = connectionString.Length;
				return connectionString.Substring(0, start) + "***" + connectionString.Substring(end);
			}
			return connectionString;
		}
	}
}