using Microsoft.EntityFrameworkCore;
using QLSV_API.Repository;
using QLSV_API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
	});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình DbContext cho PostgreSQL (Supabase)
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
	?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("Database connection string is not configured.");
}

// Chuyển đổi từ PostgreSQL URI sang Npgsql connection string nếu cần
if (connectionString.StartsWith("postgresql://") || connectionString.StartsWith("postgres://"))
{
	var uri = new Uri(connectionString);
	var password = uri.UserInfo.Contains(':') ? uri.UserInfo.Split(':')[1] : "";
	var username = uri.UserInfo.Contains(':') ? uri.UserInfo.Split(':')[0] : uri.UserInfo;

	connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Connection Idle Lifetime=300;";
}

// Log connection string (ẩn password)
var logConnectionString = connectionString.Contains("Password=")
	? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***;" + connectionString.Substring(connectionString.IndexOf(";", connectionString.IndexOf("Password=")))
	: connectionString;
Console.WriteLine($"Using connection string: {logConnectionString}");

try
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseNpgsql(connectionString, npgsqlOptions =>
		{
			npgsqlOptions.EnableRetryOnFailure(
				maxRetryCount: 3,
				maxRetryDelay: TimeSpan.FromSeconds(5),
				errorCodesToAdd: null
			);
			npgsqlOptions.CommandTimeout(60); // Tăng timeout lên 60 giây
		})
	);
}
catch (Exception ex)
{
	Console.WriteLine($"Error configuring DbContext: {ex.Message}");
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

// Test database connection
try
{
	using var scope = app.Services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	Console.WriteLine("Testing database connection...");
	await context.Database.CanConnectAsync();
	Console.WriteLine("Database connection successful!");

	// Apply migrations
	Console.WriteLine("Applying database migrations...");
	await context.Database.MigrateAsync();
	Console.WriteLine("Database migrations applied successfully!");
}
catch (Exception ex)
{
	Console.WriteLine($"Database connection/migration error: {ex.Message}");
	Console.WriteLine($"Stack trace: {ex.StackTrace}");

	// Không throw exception ở đây để app vẫn có thể chạy
	// throw;
}

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

// Health check endpoint
app.MapGet("/health", async (AppDbContext context) =>
{
	try
	{
		await context.Database.CanConnectAsync();
		return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
	}
	catch (Exception ex)
	{
		return Results.Problem($"Database connection failed: {ex.Message}");
	}
});

Console.WriteLine("Application starting...");
app.Run();