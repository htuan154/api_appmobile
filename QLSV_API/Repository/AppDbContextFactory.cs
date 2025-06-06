using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
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

				// Tạo configuration builder
				var configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile("appsettings.Development.json", optional: true)
					.Build();

				var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
				var connectionString = configuration.GetConnectionString("DefaultConnection");

				// Fallback nếu không tìm thấy chuỗi kết nối
				if (string.IsNullOrEmpty(connectionString))
				{
					Console.WriteLine("⚠️ Using fallback connection string");
					connectionString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=6543;Username=postgres.ptjwvhtebbrdsqkelcrl;Password=htuan15424;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Pooling=false;Application Name=QLSV-API;";
				}

				Console.WriteLine($"🔗 Design-time connection: {MaskPassword(connectionString)}");

				// Cấu hình Npgsql
				optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
				{
					npgsqlOptions.EnableRetryOnFailure(
						maxRetryCount: 5,
						maxRetryDelay: TimeSpan.FromSeconds(30),
						errorCodesToAdd: null
					);
					npgsqlOptions.CommandTimeout(180);
				});

				// Ghi log chi tiết nếu đang dev
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
