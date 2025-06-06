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
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

// Cấu hình phân quyền
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});
// Thêm các dịch vụ vào container trước khi gọi builder.Build()
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.WriteIndented = true; // Tuỳ chọn: format JSON đẹp hơn
	});

// Cấu hình Swagger/OpenAPI cho môi trường phát triển
builder.Services.AddEndpointsApiExplorer();  // Cấu hình OpenAPI
builder.Services.AddSwaggerGen();  // Thêm Swagger để tạo tài liệu API

// Cấu hình DbContext để sử dụng SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Cấu hình CORS để cho phép tất cả các origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();  // Gọi Build sau khi cấu hình dịch vụ

// Cấu hình pipeline cho yêu cầu HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Sử dụng Swagger
    app.UseSwaggerUI(); // Hiển thị UI Swagger
}

// Áp dụng CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();  // Redirect HTTP requests to HTTPS

app.UseAuthentication();

app.UseAuthorization();  // Enable authorization middleware

app.MapControllers();  // Định tuyến controller

app.Run();  // Chạy ứng dụng
