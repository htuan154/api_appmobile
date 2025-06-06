using QLSV_API.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace QLSV_API.Service
{
    public class AuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            // Kiểm tra với bảng TaiKhoan (admin)
            var admin = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == username && t.MatKhau == password);
            if (admin != null)
            {
                // Tạo JWT cho admin
                return GenerateJwtToken(admin.Ma_TK, "Admin");
            }

            // Kiểm tra với bảng TaiKhoanSinhVien (sinh viên)
            var sinhVien = await _context.TaiKhoanSinhViens.FirstOrDefaultAsync(t => t.TenDangNhap == username && t.MatKhau == password);
            if (sinhVien != null)
            {
                // Tạo JWT cho sinh viên
                return GenerateJwtToken(sinhVien.Ma_TKSV, "User");
            }

            return null;  // Nếu không tìm thấy tài khoản
        }

        private string GenerateJwtToken(string userId, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
