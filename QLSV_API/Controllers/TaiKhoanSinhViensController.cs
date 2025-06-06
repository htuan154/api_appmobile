using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLSV_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TaiKhoanSinhViensController : ControllerBase
	{
		private readonly AppDbContext _context;

		public TaiKhoanSinhViensController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/TaiKhoanSinhViens
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaiKhoanSinhVien>>> GetTaiKhoanSinhViens()
		{
			return await _context.TaiKhoanSinhViens
								 .Include(tk => tk.SinhVien)
								 .Include(tk => tk.YeuCaus)
								 .ToListAsync();
		}

		// GET: api/TaiKhoanSinhViens/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<TaiKhoanSinhVien>> GetTaiKhoanSinhVien(string id)
		{
			var taiKhoan = await _context.TaiKhoanSinhViens
										 .Include(tk => tk.SinhVien)
										 .Include(tk => tk.YeuCaus)
										 .FirstOrDefaultAsync(tk => tk.Ma_TKSV == id);

			if (taiKhoan == null)
				return NotFound();

			return taiKhoan;
		}

		// POST: api/TaiKhoanSinhViens
		[HttpPost]
		public async Task<ActionResult<TaiKhoanSinhVien>> PostTaiKhoanSinhVien(TaiKhoanSinhVien taiKhoan)
		{
			// Kiểm tra SV có tồn tại không
			var sinhVien = await _context.SinhViens.FindAsync(taiKhoan.Ma_SV);
			if (sinhVien == null)
			{
				return BadRequest("Sinh viên không tồn tại");
			}

			_context.TaiKhoanSinhViens.Add(taiKhoan);
			await _context.SaveChangesAsync();

			// Lấy tài khoản đã tạo (kèm theo navigation properties)
			var createdAccount = await _context.TaiKhoanSinhViens
				.Include(tk => tk.SinhVien)
				.FirstOrDefaultAsync(tk => tk.Ma_TKSV == taiKhoan.Ma_TKSV);

			return CreatedAtAction(nameof(GetTaiKhoanSinhVien), new { id = taiKhoan.Ma_TKSV }, createdAccount);
		}

		// PUT: api/TaiKhoanSinhViens/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> PutTaiKhoanSinhVien(string id, TaiKhoanSinhVien taiKhoan)
		{
			if (id != taiKhoan.Ma_TKSV)
				return BadRequest();

			// Tìm tài khoản hiện có
			var existingAccount = await _context.TaiKhoanSinhViens.FindAsync(id);
			if (existingAccount == null)
				return NotFound();

			// Cập nhật thông tin
			existingAccount.TenDangNhap = taiKhoan.TenDangNhap;
			existingAccount.MatKhau = taiKhoan.MatKhau;
			existingAccount.Ma_SV = taiKhoan.Ma_SV;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.TaiKhoanSinhViens.Any(tk => tk.Ma_TKSV == id))
					return NotFound();
				else
					throw;
			}

			return NoContent();
		}

		// DELETE: api/TaiKhoanSinhViens/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTaiKhoanSinhVien(string id)
		{
			var taiKhoan = await _context.TaiKhoanSinhViens.FindAsync(id);
			if (taiKhoan == null)
				return NotFound();

			_context.TaiKhoanSinhViens.Remove(taiKhoan);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPost("TaoTatCaTaiKhoanSinhVien")]
		public async Task<IActionResult> TaoTatCaTaiKhoanSinhVien()
		{
			var danhSachSinhVien = await _context.SinhViens.ToListAsync();
			int soLuongTao = 0;

			foreach (var sv in danhSachSinhVien)
			{
				// Kiểm tra nếu sinh viên đã có tài khoản
				var daCoTaiKhoan = await _context.TaiKhoanSinhViens.AnyAsync(tk => tk.Ma_SV == sv.Ma_SV);
				if (daCoTaiKhoan)
					continue; // Bỏ qua nếu đã có

				// Tạo tài khoản mới
				var taiKhoanMoi = new TaiKhoanSinhVien
				{
					Ma_TKSV = "TK" + sv.Ma_SV.Substring(2),
					TenDangNhap = sv.Email,
					MatKhau = sv.NgaySinh.ToString("ddMMyyyy"),
					Ma_SV = sv.Ma_SV
					// Không set Ma_TKSV → database tự sinh
				};

				_context.TaiKhoanSinhViens.Add(taiKhoanMoi);
				soLuongTao++;
			}

			await _context.SaveChangesAsync();

			return Ok($"Đã tạo thành công {soLuongTao} tài khoản sinh viên.");
		}

		// PUT: api/TaiKhoanSinhViens/ResetMatKhau/{maSV}
		[HttpPut("ResetMatKhau/{maSV}")]
		public async Task<IActionResult> ResetMatKhauSinhVien(string maSV)
		{
			// Tìm tài khoản theo Ma_SV
			var taiKhoan = await _context.TaiKhoanSinhViens
										 .FirstOrDefaultAsync(tk => tk.Ma_SV == maSV);

			if (taiKhoan == null)
				return NotFound("Không tìm thấy tài khoản sinh viên.");

			// Tìm trực tiếp thông tin sinh viên
			var sinhVien = await _context.SinhViens.FindAsync(maSV);
			if (sinhVien == null)
				return BadRequest("Không tìm thấy thông tin sinh viên.");

			// Reset mật khẩu
			taiKhoan.MatKhau = sinhVien.NgaySinh.ToString("ddMMyyyy");

			await _context.SaveChangesAsync();

			return Ok("Đã reset mật khẩu thành công.");
		}
	}
}