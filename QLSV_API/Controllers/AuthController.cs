	using QLSV_API.Model;
	using global::QLSV_API.Repository;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.IdentityModel.Tokens;
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Text;

	namespace QLSV_API.Controllers
	{
		[Route("api/[controller]")]
		[ApiController]
		public class AuthController : ControllerBase
		{
			private readonly AppDbContext _context;
			private readonly IConfiguration _configuration;

			public AuthController(AppDbContext context, IConfiguration configuration)
			{
				_context = context;
				_configuration = configuration;
			}

			// Đăng nhập
			[HttpPost("login")]
			public async Task<IActionResult> Login([FromBody] LoginModel model)
			{
				// Kiểm tra tài khoản admin
				var admin = await _context.TaiKhoans
					.FirstOrDefaultAsync(x => x.TenDangNhap == model.Username && x.MatKhau == model.Password);
				if (admin != null)
				{
					var token = GenerateJwtTokenAdmin(admin);
					return Ok(new
					{
						Token = token,
						Role = admin.Ma_Loai,
						// Thêm các thông tin để trả về cùng với token nếu cần
						UserInfo = new
						{
							Ma_TK = admin.Ma_TK,
							TenDangNhap = admin.TenDangNhap,
							MatKhau = admin.MatKhau,
							Ma_NV = admin.Ma_NV,
							Ma_Loai = admin.Ma_Loai
						}
					});
				}
				// Kiểm tra tài khoản sinh viên
				var studentAccount = await _context.TaiKhoanSinhViens
				.FirstOrDefaultAsync(x => x.TenDangNhap == model.Username && x.MatKhau == model.Password);
				if (studentAccount != null)
				{
					var token = GenerateJwtTokenSinhVien(studentAccount);
					return Ok(new
					{
						Token = token,
						Role = "Student",
						UserInfo = new
						{
							Ma_TKSV = studentAccount.Ma_TKSV,
							TenDangNhap = studentAccount.TenDangNhap,
							Ma_SV = studentAccount.Ma_SV,
							MatKhau = studentAccount.MatKhau
						}
					});
				}
			// Nếu không tìm thấy tài khoản hợp lệ
			return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
			}

			// Tạo mã JWT cho người dùng (Admin/Sinh viên)
			private string GenerateJwtTokenAdmin(TaiKhoan admin)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, admin.TenDangNhap),
					new Claim(ClaimTypes.NameIdentifier, admin.Ma_TK),
					new Claim("Ma_TK", admin.Ma_TK),
					new Claim("TenDangNhap", admin.TenDangNhap),
					new Claim("Ma_NV", admin.Ma_NV ?? ""),
					new Claim("Ma_Loai", admin.Ma_Loai ?? "")
				};

				// ✅ Sửa lại key đúng với appsettings.json ("Jwt:SecretKey")
				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
					issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
					claims: claims,
					expires: DateTime.Now.AddDays(1),
					signingCredentials: creds
				);

				return new JwtSecurityTokenHandler().WriteToken(token);
			}

			private string GenerateJwtTokenSinhVien(TaiKhoanSinhVien sinhVien)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, sinhVien.TenDangNhap),
					new Claim(ClaimTypes.NameIdentifier, sinhVien.Ma_TKSV),
					new Claim("Ma_TKSV", sinhVien.Ma_TKSV),
					new Claim("TenDangNhap", sinhVien.TenDangNhap),
					new Claim("Ma_SV", sinhVien.Ma_SV),
					new Claim(ClaimTypes.Role, "Student") // Thêm role để dễ phân quyền
				};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
					issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
					claims: claims,
					expires: DateTime.Now.AddDays(1),
					signingCredentials: creds
				);

				return new JwtSecurityTokenHandler().WriteToken(token);
			}


		private string GenerateJwtToken(string username, string userId)
			{
				var claims = new[]
				{
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.NameIdentifier, userId),
			};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
					issuer: _configuration["JWT:Issuer"],
					audience: _configuration["JWT:Audience"],
					claims: claims,
					expires: DateTime.Now.AddDays(1),
					signingCredentials: creds
				);

				return new JwtSecurityTokenHandler().WriteToken(token);
			}
			// Chỉ Admin mới có thể thêm tài khoản sinh viên vào hệ thống
			[HttpPost("register/student")]
			[Authorize(Roles = "Admin")]  // Chỉ Admin mới có quyền gọi API này
			public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentModel model)
			{
				// Kiểm tra xem sinh viên đã có tài khoản chưa
				var existingUser = await _context.TaiKhoanSinhViens
					.FirstOrDefaultAsync(x => x.TenDangNhap == model.Username);

				if (existingUser != null)
					return BadRequest("Tên đăng nhập đã tồn tại.");

				var newUser = new TaiKhoanSinhVien
				{
					TenDangNhap = model.Username,
					MatKhau = model.Password, // Đảm bảo mã hóa mật khẩu khi lưu trữ
					Ma_SV = model.Ma_SV  // Mã sinh viên
				};

				// Thêm sinh viên vào hệ thống
				_context.TaiKhoanSinhViens.Add(newUser);
				await _context.SaveChangesAsync();

				return Ok("Tạo tài khoản sinh viên thành công.");
			}
			// 
			// Lấy thông tin tài khoản sinh viên theo ID
			[HttpGet("student/{id}")]
			[Authorize] // Yêu cầu xác thực
			public async Task<IActionResult> GetStudentAccount(string id)
			{
				var studentAccount = await _context.TaiKhoanSinhViens
					.FirstOrDefaultAsync(x => x.Ma_TKSV == id);

				if (studentAccount == null)
					return NotFound("Không tìm thấy tài khoản sinh viên.");

				// Có thể thêm SinhVien model để trả về thông tin chi tiết của sinh viên
				return Ok(new
				{
					Ma_TKSV = studentAccount.Ma_TKSV,
					TenDangNhap = studentAccount.TenDangNhap,
					Ma_SV = studentAccount.Ma_SV
					// Không trả về mật khẩu vì lý do bảo mật
				});
			}

			// Lấy danh sách tất cả tài khoản sinh viên
			[HttpGet("students")]
			[Authorize(Roles = "Admin,Staff")] // Chỉ Admin và Staff có quyền xem danh sách
			public async Task<IActionResult> GetAllStudentAccounts()
			{
				var studentAccounts = await _context.TaiKhoanSinhViens
					.Select(s => new
					{
						Ma_TKSV = s.Ma_TKSV,
						TenDangNhap = s.TenDangNhap,
						Ma_SV = s.Ma_SV
						// Không trả về mật khẩu vì lý do bảo mật
					})
					.ToListAsync();

				return Ok(studentAccounts);
			}

			// Lấy thông tin tài khoản admin theo ID
			[HttpGet("admin/{id}")]
			[Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền xem thông tin admin khác
			public async Task<IActionResult> GetAdminAccount(string id)
			{
				var adminAccount = await _context.TaiKhoans
					.FirstOrDefaultAsync(x => x.Ma_TK == id);

				if (adminAccount == null)
					return NotFound("Không tìm thấy tài khoản admin.");

				return Ok(new
				{
					Ma_TK = adminAccount.Ma_TK,
					TenDangNhap = adminAccount.TenDangNhap,
					Ma_NV = adminAccount.Ma_NV,
					Ma_Loai = adminAccount.Ma_Loai
					// Không trả về mật khẩu vì lý do bảo mật
				});
			}

			// Lấy danh sách tất cả tài khoản admin
			[HttpGet("admins")]
			[Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền xem danh sách admin
			public async Task<IActionResult> GetAllAdminAccounts()
			{
				var adminAccounts = await _context.TaiKhoans
					.Select(a => new
					{
						Ma_TK = a.Ma_TK,
						TenDangNhap = a.TenDangNhap,
						Ma_NV = a.Ma_NV,
						Ma_Loai = a.Ma_Loai
						// Không trả về mật khẩu vì lý do bảo mật
					})
					.ToListAsync();

				return Ok(adminAccounts);
			}

			// Lấy thông tin tài khoản hiện tại đang đăng nhập
			[HttpGet("me")]
			[Authorize]
			public async Task<IActionResult> GetCurrentUser()
			{
				// Lấy thông tin người dùng từ token
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var userName = User.FindFirst(ClaimTypes.Name)?.Value;

				// Kiểm tra xem người dùng là admin hay sinh viên
				if (User.IsInRole("Admin") || User.IsInRole("Staff"))
				{
					var admin = await _context.TaiKhoans
						.FirstOrDefaultAsync(x => x.Ma_TK == userId);

					if (admin != null)
					{
						return Ok(new
						{
							Ma_TK = admin.Ma_TK,
							TenDangNhap = admin.TenDangNhap,
							Ma_NV = admin.Ma_NV,
							Ma_Loai = admin.Ma_Loai,
							Role = User.IsInRole("Admin") ? "Admin" : "Staff"
						});
					}
				}
				else // Người dùng là sinh viên
				{
					var student = await _context.TaiKhoanSinhViens
						.FirstOrDefaultAsync(x => x.Ma_TKSV == userId);

					if (student != null)
					{
						return Ok(new
						{
							Ma_TKSV = student.Ma_TKSV,
							TenDangNhap = student.TenDangNhap,
							Ma_SV = student.Ma_SV,
							Role = "Student"
						});
					}
				}

				return NotFound("Không tìm thấy thông tin người dùng.");
			}
		}
	}

