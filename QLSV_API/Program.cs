using Microsoft.EntityFrameworkCore;
using QLSV_API.Repository;
using QLSV_API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true, // ✅ Đã sửa lỗi ở đây
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])
			)
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

// ✅ THAY ĐỔI: Cấu hình DbContext cho PostgreSQL (Supabase)
// Ưu tiên Environment Variable từ Render, fallback về appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
	?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString, npgsqlOptions =>
	{
		npgsqlOptions.EnableRetryOnFailure(3); // Retry 3 lần nếu connection fail
		npgsqlOptions.CommandTimeout(30); // Timeout 30 giây
	})
);

// ❌ KHÔNG DÙNG SQL Server nữa (nhưng giữ lại để tham khảo)
// builder.Services.AddDbContext<AppDbContext>(options =>
// 	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
// );

// Cho phép tất cả origin (nên giới hạn sau này nếu cần bảo mật)
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

// ✅ Luôn bật Swagger, kể cả môi trường Production (Render dùng môi trường này)
app.UseSwagger();
app.UseSwaggerUI();

// Middleware pipeline
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();