using Microsoft.EntityFrameworkCore;
using QLSV_API.Repository;
using QLSV_API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;

// ✅ QUAN TRỌNG: Cấu hình timezone cho Npgsql ngay từ đầu
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Đọc JWT configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey))
{
	throw new InvalidOperationException("JWT SecretKey is not configured.");
}

// Cấu hình JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = issuer,
			ValidAudience = audience,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(secretKey)
			),
			ClockSkew = TimeSpan.Zero
		};
	});

// Cấu hình phân quyền
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
	options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Thêm các dịch vụ vào container
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.WriteIndented = true;
		// ✅ Cấu hình JSON serialization cho DateTime
		options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
	});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình DbContext cho PostgreSQL (Supabase) với connection string được tối ưu
var connectionString = await GetOptimalConnectionString(builder.Configuration);

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("No working database connection string found.");
}

Console.WriteLine($"🔗 Using connection string: {MaskPasswordInConnectionString(connectionString)}");

try
{
	builder.Services.AddDbContext<AppDbContext>(options =>
	{
		options.UseNpgsql(connectionString, npgsqlOptions =>
		{
			npgsqlOptions.EnableRetryOnFailure(
				maxRetryCount: 5,
				maxRetryDelay: TimeSpan.FromSeconds(30),
				errorCodesToAdd: null
			);
			npgsqlOptions.CommandTimeout(180); // Tăng timeout lên 3 phút
		});

		// ✅ Enable detailed logging cho development
		if (builder.Environment.IsDevelopment())
		{
			options.EnableSensitiveDataLogging();
			options.EnableDetailedErrors();
			options.LogTo(Console.WriteLine, LogLevel.Information);
		}
	});
}
catch (Exception ex)
{
	Console.WriteLine($"❌ Error configuring DbContext: {ex.Message}");
	throw;
}

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Test database connection và migration
await InitializeDatabase(app);

// Luôn bật Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "QLSV API V1");
	c.RoutePrefix = string.Empty; // Swagger UI ở root
});

// Middleware pipeline
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint với thông tin chi tiết
app.MapGet("/health", async (AppDbContext context) =>
{
	try
	{
		var startTime = DateTime.UtcNow;
		var canConnect = await context.Database.CanConnectAsync();
		var responseTime = DateTime.UtcNow - startTime;

		if (canConnect)
		{
			return Results.Ok(new
			{
				status = "Healthy",
				timestamp = DateTime.UtcNow,
				responseTime = responseTime.TotalMilliseconds + "ms",
				database = "Connected",
				timezone = TimeZoneInfo.Local.Id,
				utcNow = DateTime.UtcNow,
				localNow = DateTime.Now
			});
		}
		else
		{
			return Results.Problem("Database connection failed");
		}
	}
	catch (Exception ex)
	{
		return Results.Problem($"Database connection failed: {ex.Message}");
	}
});

// Debug endpoint để test connection strings
app.MapGet("/debug/connection", async () =>
{
	var config = app.Services.GetRequiredService<IConfiguration>();
	var connections = GetAlternativeConnectionStrings(config);
	var results = new List<object>();

	foreach (var conn in connections)
	{
		var result = await TestDatabaseConnection(conn);
		results.Add(new
		{
			connectionString = MaskPasswordInConnectionString(conn),
			isSuccess = result.IsSuccess,
			error = result.ErrorMessage,
			responseTime = result.ResponseTime
		});
	}

	return Results.Ok(results);
});

// ✅ Debug endpoint để test timezone
app.MapGet("/debug/timezone", async (AppDbContext context) =>
{
	try
	{
		// Test timezone configuration
		var testDateTime = new DateTime(2024, 1, 1, 12, 0, 0);
		var utcDateTime = DateTime.SpecifyKind(testDateTime, DateTimeKind.Utc);
		var localDateTime = DateTime.SpecifyKind(testDateTime, DateTimeKind.Local);

		return Results.Ok(new
		{
			serverTimeZone = TimeZoneInfo.Local.Id,
			utcNow = DateTime.UtcNow,
			localNow = DateTime.Now,
			testDateTime = testDateTime,
			utcDateTime = utcDateTime,
			localDateTime = localDateTime,
			npgsqlLegacyBehavior = AppContext.TryGetSwitch("Npgsql.EnableLegacyTimestampBehavior", out bool isEnabled) ? isEnabled : false
		});
	}
	catch (Exception ex)
	{
		return Results.Problem($"Timezone test failed: {ex.Message}");
	}
});

Console.WriteLine("🚀 Application starting...");
Console.WriteLine($"🌍 Server timezone: {TimeZoneInfo.Local.Id}");
Console.WriteLine($"🕐 UTC time: {DateTime.UtcNow}");
Console.WriteLine($"🕐 Local time: {DateTime.Now}");
app.Run();

// Helper Methods - Cải thiện để xử lý Supabase tốt hơn
static async Task<string> GetOptimalConnectionString(IConfiguration configuration)
{
	var alternatives = GetAlternativeConnectionStrings(configuration);

	Console.WriteLine("🔍 Testing connection strings to find the best one...");

	foreach (var conn in alternatives)
	{
		Console.WriteLine($"🧪 Testing: {MaskPasswordInConnectionString(conn)}");
		var result = await TestDatabaseConnection(conn);

		if (result.IsSuccess)
		{
			Console.WriteLine($"✅ Connection successful! Response time: {result.ResponseTime}ms");
			return conn;
		}
		else
		{
			Console.WriteLine($"❌ Connection failed: {result.ErrorMessage}");
		}
	}

	throw new InvalidOperationException("No working database connection found.");
}

static List<string> GetAlternativeConnectionStrings(IConfiguration configuration)
{
	var baseConnectionString = configuration.GetConnectionString("DefaultConnection");
	var alternatives = new List<string>();

	if (!string.IsNullOrEmpty(baseConnectionString))
	{
		// 1. PRIORITY: Supavisor Session Mode (IPv4 compatible) - Port 6543
		var supavisorSession = baseConnectionString
			.Replace(":5432", ":6543")
			.Replace("db.lpfczxkyuvntotmxgbxy.supabase.co:5432", "db.lpfczxkyuvntotmxgbxy.supabase.co:6543");
		if (!supavisorSession.Contains("Pooling=false"))
		{
			supavisorSession += "Pooling=false;"; // Disable connection pooling for session mode
		}
		if (!supavisorSession.Contains("No Reset On Close"))
		{
			supavisorSession += "No Reset On Close=true;";
		}
		// ✅ Thêm timezone parameters
		if (!supavisorSession.Contains("Timezone"))
		{
			supavisorSession += "Timezone=UTC;";
		}
		alternatives.Add(supavisorSession);

		// 2. Supavisor Session Mode với IPv4 forcing parameters
		var supavisorIPv4 = supavisorSession;
		if (!supavisorIPv4.Contains("IP Version"))
		{
			supavisorIPv4 += "IP Version=IPv4;";
		}
		alternatives.Add(supavisorIPv4);

		// 3. Transaction pooling mode (port 6543) 
		var transactionPooling = baseConnectionString
			.Replace(":5432", ":6543")
			.Replace("db.lpfczxkyuvntotmxgbxy.supabase.co:5432", "db.lpfczxkyuvntotmxgbxy.supabase.co:6543");
		if (!transactionPooling.Contains("Enlist"))
		{
			transactionPooling += "Enlist=false;";
		}
		if (!transactionPooling.Contains("No Reset On Close"))
		{
			transactionPooling += "No Reset On Close=true;";
		}
		if (!transactionPooling.Contains("Timezone"))
		{
			transactionPooling += "Timezone=UTC;";
		}
		alternatives.Add(transactionPooling);

		// 4. Legacy PgBouncer format nếu có thể
		try
		{
			var pgbouncerUri = "postgresql://postgres.lpfczxkyuvntotmxgbxy:htuan15424%40@db.lpfczxkyuvntotmxgbxy.supabase.co:6543/postgres?sslmode=require&pgbouncer=true&pooling=false";
			alternatives.Add(ConvertPostgresUriToConnectionString(pgbouncerUri));
		}
		catch
		{
			// Ignore URI conversion errors
		}

		// 5. Direct connection với IPv4 parameters (fallback)
		var directIPv4 = baseConnectionString;
		if (!directIPv4.Contains("IP Version"))
		{
			directIPv4 += "IP Version=IPv4;";
		}
		if (!directIPv4.Contains("Application Name"))
		{
			directIPv4 += "Application Name=QLSV-API;";
		}
		if (!directIPv4.Contains("Timezone"))
		{
			directIPv4 += "Timezone=UTC;";
		}
		alternatives.Add(directIPv4);

		// 6. Connection string gốc (cuối cùng thử)
		alternatives.Add(baseConnectionString);
	}

	return alternatives.Distinct().ToList();
}

static string ConvertPostgresUriToConnectionString(string uri)
{
	try
	{
		var parsedUri = new Uri(uri);
		var password = parsedUri.UserInfo.Contains(':') ? Uri.UnescapeDataString(parsedUri.UserInfo.Split(':')[1]) : "";
		var username = parsedUri.UserInfo.Contains(':') ? parsedUri.UserInfo.Split(':')[0] : parsedUri.UserInfo;

		var connectionString = $"Host={parsedUri.Host};Port={parsedUri.Port};Database={parsedUri.AbsolutePath.TrimStart('/')};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Connection Idle Lifetime=300;Include Error Detail=true;Timeout=60;Command Timeout=180;No Reset On Close=true;Timezone=UTC;";

		// Thêm IPv4 và session mode parameters cho Supavisor
		if (parsedUri.Port == 6543)
		{
			connectionString += "Pooling=false;Enlist=false;"; // Session mode parameters
		}

		// Thêm pgbouncer parameters nếu có trong query string
		if (parsedUri.Query.Contains("pgbouncer=true") || parsedUri.Query.Contains("pooling=false"))
		{
			connectionString += "Enlist=false;";
		}

		return connectionString;
	}
	catch (Exception ex)
	{
		Console.WriteLine($"❌ Error converting URI: {ex.Message}");
		return uri;
	}
}

static string MaskPasswordInConnectionString(string connectionString)
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

static async Task<(bool IsSuccess, string ErrorMessage, double ResponseTime)> TestDatabaseConnection(string connectionString)
{
	var stopwatch = System.Diagnostics.Stopwatch.StartNew();

	try
	{
		using var connection = new NpgsqlConnection(connectionString);

		await connection.OpenAsync();

		// Test với query đơn giản và test timezone
		using var command = new NpgsqlCommand("SELECT 1, NOW(), timezone('UTC', NOW())", connection);
		command.CommandTimeout = 30;
		var result = await command.ExecuteScalarAsync();

		await connection.CloseAsync();
		stopwatch.Stop();

		return (true, "", stopwatch.Elapsed.TotalMilliseconds);
	}
	catch (Exception ex)
	{
		stopwatch.Stop();
		return (false, ex.Message, stopwatch.Elapsed.TotalMilliseconds);
	}
}

static async Task InitializeDatabase(WebApplication app)
{
	try
	{
		using var scope = app.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		Console.WriteLine("🔍 Testing database connection...");
		var canConnect = await context.Database.CanConnectAsync();

		if (canConnect)
		{
			Console.WriteLine("✅ Database connection successful!");

			// ✅ Test timezone configuration
			try
			{
				using var command = context.Database.GetDbConnection().CreateCommand();
				command.CommandText = "SELECT timezone('UTC', NOW()), NOW()";
				await context.Database.OpenConnectionAsync();
				var result = await command.ExecuteScalarAsync();
				await context.Database.CloseConnectionAsync();
				Console.WriteLine($"✅ Database timezone test successful! Result: {result}");
			}
			catch (Exception tzEx)
			{
				Console.WriteLine($"⚠️ Database timezone test failed: {tzEx.Message}");
			}

			// Apply migrations với retry
			Console.WriteLine("🔄 Applying database migrations...");
			var retryCount = 0;
			var maxRetries = 3;

			while (retryCount < maxRetries)
			{
				try
				{
					await context.Database.MigrateAsync();
					Console.WriteLine("✅ Database migrations applied successfully!");
					break;
				}
				catch (Exception migrationEx)
				{
					retryCount++;
					Console.WriteLine($"⚠️ Migration attempt {retryCount} failed: {migrationEx.Message}");

					// ✅ Log chi tiết lỗi timezone nếu có
					if (migrationEx.Message.Contains("timestamp with time zone"))
					{
						Console.WriteLine("🔍 Timezone error detected. Check DateTime configuration in models.");
						Console.WriteLine($"🔧 Legacy timestamp behavior enabled: {AppContext.TryGetSwitch("Npgsql.EnableLegacyTimestampBehavior", out bool isEnabled) && isEnabled}");
					}

					if (retryCount >= maxRetries)
					{
						Console.WriteLine("❌ All migration attempts failed!");
						throw;
					}

					await Task.Delay(TimeSpan.FromSeconds(5 * retryCount)); // Exponential backoff
				}
			}

			// Test query
			try
			{
				using var command = context.Database.GetDbConnection().CreateCommand();
				command.CommandText = "SELECT version()";
				await context.Database.OpenConnectionAsync();
				var version = await command.ExecuteScalarAsync();
				await context.Database.CloseConnectionAsync();
				Console.WriteLine($"✅ Database query test successful! Version: {version?.ToString()?.Substring(0, Math.Min(50, version.ToString().Length))}...");
			}
			catch (Exception queryEx)
			{
				Console.WriteLine($"⚠️ Database query test failed: {queryEx.Message}");
			}
		}
		else
		{
			Console.WriteLine("❌ Database connection failed!");
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"❌ Database initialization error: {ex.Message}");
		Console.WriteLine($"📝 Stack trace: {ex.StackTrace}");

		// Log inner exception nếu có
		if (ex.InnerException != null)
		{
			Console.WriteLine($"🔍 Inner exception: {ex.InnerException.Message}");
		}

		// ✅ Kiểm tra lỗi timezone cụ thể
		if (ex.Message.Contains("timestamp with time zone") || ex.Message.Contains("DateTime"))
		{
			Console.WriteLine("🔧 TIMEZONE FIX SUGGESTIONS:");
			Console.WriteLine("1. Ensure AppContext.SetSwitch('Npgsql.EnableLegacyTimestampBehavior', true) is set");
			Console.WriteLine("2. Use DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) for seed data");
			Console.WriteLine("3. Consider using DateTimeOffset instead of DateTime");
			Console.WriteLine($"4. Current legacy behavior setting: {AppContext.TryGetSwitch("Npgsql.EnableLegacyTimestampBehavior", out bool isEnabled) && isEnabled}");
		}

		// Không throw exception để app vẫn có thể chạy và debug
		Console.WriteLine("⚠️ Application will continue running for debugging purposes...");
	}
}