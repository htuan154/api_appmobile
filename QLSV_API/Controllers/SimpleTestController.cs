using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SimpleTestController : Controller
	{
		private readonly AppDbContext _context;
		private readonly ILogger<SimpleTestController> _logger;

		public SimpleTestController(AppDbContext context, ILogger<SimpleTestController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet("ping")]
		public async Task<IActionResult> Ping()
		{
			try
			{
				_logger.LogInformation("Testing basic database connection...");

				// Test connection
				var canConnect = await _context.Database.CanConnectAsync();

				if (!canConnect)
				{
					return BadRequest(new { status = "Cannot connect to database" });
				}

				// Get database info
				var connectionString = _context.Database.GetConnectionString();
				var providerName = _context.Database.ProviderName;

				return Ok(new
				{
					status = "Database connection successful",
					canConnect = canConnect,
					provider = providerName,
					connectionInfo = connectionString?.Substring(0, Math.Min(50, connectionString.Length)) + "...",
					timestamp = DateTime.UtcNow
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Database ping failed");
				return StatusCode(500, new
				{
					status = "Database ping failed",
					error = ex.Message,
					innerException = ex.InnerException?.Message,
					stackTrace = ex.StackTrace
				});
			}
		}

		[HttpGet("migrations")]
		public async Task<IActionResult> CheckMigrations()
		{
			try
			{
				_logger.LogInformation("Checking database migrations...");

				var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
				var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

				return Ok(new
				{
					status = "Migration check successful",
					appliedMigrations = appliedMigrations,
					pendingMigrations = pendingMigrations,
					hasPendingMigrations = pendingMigrations.Any(),
					timestamp = DateTime.UtcNow
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Migration check failed");
				return StatusCode(500, new
				{
					status = "Migration check failed",
					error = ex.Message,
					innerException = ex.InnerException?.Message
				});
			}
		}

		[HttpPost("migrate")]
		public async Task<IActionResult> RunMigrations()
		{
			try
			{
				_logger.LogInformation("Running database migrations...");

				await _context.Database.MigrateAsync();

				return Ok(new
				{
					status = "Migrations applied successfully",
					timestamp = DateTime.UtcNow
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Migration failed");
				return StatusCode(500, new
				{
					status = "Migration failed",
					error = ex.Message,
					innerException = ex.InnerException?.Message
				});
			}
		}

		[HttpGet("ef-test")]
		public async Task<IActionResult> TestEntityFramework()
		{
			try
			{
				_logger.LogInformation("Testing Entity Framework DbContext...");

				// Test 1: Basic connection
				var canConnect = await _context.Database.CanConnectAsync();

				// Test 2: Get connection info
				var connectionString = _context.Database.GetConnectionString();
				var providerName = _context.Database.ProviderName;

				// Test 3: Check if database exists
				var dbExists = await _context.Database.EnsureCreatedAsync();

				return Ok(new
				{
					status = "Entity Framework test successful",
					canConnect = canConnect,
					provider = providerName,
					connectionInfo = MaskPassword(connectionString),
					databaseCreated = dbExists,
					timestamp = DateTime.UtcNow
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Entity Framework test failed");
				return StatusCode(500, new
				{
					status = "Entity Framework test failed",
					error = ex.Message,
					innerException = ex.InnerException?.Message,
					stackTrace = ex.StackTrace?.Split('\n').Take(5).ToArray() // First 5 lines only
				});
			}
		}

		private string MaskPassword(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString)) return "Not configured";

			if (connectionString.Contains("Password="))
			{
				var start = connectionString.IndexOf("Password=");
				var end = connectionString.IndexOf(";", start);
				if (end == -1) end = connectionString.Length;

				return connectionString.Substring(0, start) + "Password=***" + connectionString.Substring(end);
			}
			return connectionString;
		}
	}
}