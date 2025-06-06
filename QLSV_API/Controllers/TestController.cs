using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace QLSV_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ConnectionTestController : Controller
	{
		private readonly ILogger<ConnectionTestController> _logger;
		private readonly IConfiguration _configuration;

		public ConnectionTestController(ILogger<ConnectionTestController> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		[HttpGet("test-formats")]
		public async Task<IActionResult> TestConnectionFormats()
		{
			var results = new List<object>();

			// Format 1: Npgsql connection string
			var format1 = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Database=postgres;Username=postgres.lpfczxkyuvntotmxgbxy;Password=htuan15424@;Port=6543;SSL Mode=Require;Trust Server Certificate=true;";
			results.Add(await TestConnection("Format 1 (Npgsql)", format1));

			// Format 2: với Pooling
			var format2 = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Database=postgres;Username=postgres.lpfczxkyuvntotmxgbxy;Password=htuan15424@;Port=6543;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Connection Idle Lifetime=300;";
			results.Add(await TestConnection("Format 2 (With Pooling)", format2));

			// Format 3: URI format (converted)
			var uriString = "postgresql://postgres.lpfczxkyuvntotmxgbxy:htuan15424%40@aws-0-ap-southeast-1.pooler.supabase.com:6543/postgres";
			var convertedUri = ConvertUriToConnectionString(uriString);
			results.Add(await TestConnection("Format 3 (URI Converted)", convertedUri));

			// Format 4: Direct connection (không qua pooler)
			var format4 = "Host=db.lpfczxkyuvntotmxgbxy.supabase.co;Database=postgres;Username=postgres;Password=htuan15424@;Port=5432;SSL Mode=Require;Trust Server Certificate=true;";
			results.Add(await TestConnection("Format 4 (Direct)", format4));

			return Ok(new
			{
				status = "Connection format tests completed",
				results = results,
				timestamp = DateTime.UtcNow
			});
		}

		private async Task<object> TestConnection(string formatName, string connectionString)
		{
			try
			{
				using var connection = new NpgsqlConnection(connectionString);
				await connection.OpenAsync();

				using var command = connection.CreateCommand();
				command.CommandText = "SELECT version()";
				var version = await command.ExecuteScalarAsync();

				await connection.CloseAsync();

				return new
				{
					format = formatName,
					status = "Success",
					version = version?.ToString()?.Substring(0, Math.Min(50, version.ToString().Length)) + "...",
					connectionString = MaskPassword(connectionString)
				};
			}
			catch (Exception ex)
			{
				return new
				{
					format = formatName,
					status = "Failed",
					error = ex.Message,
					innerException = ex.InnerException?.Message,
					connectionString = MaskPassword(connectionString)
				};
			}
		}

		private string ConvertUriToConnectionString(string uri)
		{
			try
			{
				var uriObj = new Uri(uri);
				var password = uriObj.UserInfo.Contains(':') ? Uri.UnescapeDataString(uriObj.UserInfo.Split(':')[1]) : "";
				var username = uriObj.UserInfo.Contains(':') ? uriObj.UserInfo.Split(':')[0] : uriObj.UserInfo;

				return $"Host={uriObj.Host};Port={uriObj.Port};Database={uriObj.AbsolutePath.TrimStart('/')};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;Pooling=true;";
			}
			catch (Exception ex)
			{
				return $"Error converting URI: {ex.Message}";
			}
		}

		private string MaskPassword(string connectionString)
		{
			if (connectionString.Contains("Password="))
			{
				var start = connectionString.IndexOf("Password=");
				var end = connectionString.IndexOf(";", start);
				if (end == -1) end = connectionString.Length;

				return connectionString.Substring(0, start) + "Password=***" + connectionString.Substring(end);
			}
			return connectionString;
		}

		[HttpGet("current-config")]
		public IActionResult GetCurrentConfig()
		{
			var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
				?? _configuration.GetConnectionString("DefaultConnection");

			return Ok(new
			{
				status = "Current configuration",
				hasEnvironmentVar = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")),
				hasAppSettings = !string.IsNullOrEmpty(_configuration.GetConnectionString("DefaultConnection")),
				connectionString = MaskPassword(connectionString ?? "Not configured"),
				timestamp = DateTime.UtcNow
			});
		}
	}
}