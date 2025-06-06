using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ThongBaoController : ControllerBase
	{
		private readonly AppDbContext _context;
		public ThongBaoController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/ThongBao
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ThongBao>>> GetThongBaos()
		{
			return await _context.ThongBaos
				.Include(t => t.TinTuc)  // Bao gồm thông tin từ bảng TinTuc
				.Include(t => t.TaiKhoanSinhVien)  // Bao gồm thông tin từ bảng TaiKhoanSinhVien
				.ToListAsync();
		}
		
		// GET: api/ThongBao/byTaiKhoan/{maTKSV}
		[HttpGet("byTaiKhoan/{maTKSV}")]
		public async Task<ActionResult<IEnumerable<ThongBao>>> GetThongBaosByTaiKhoan(string maTKSV)
		{
			var thongBaos = await _context.ThongBaos
				.Where(t => t.Ma_TKSV == maTKSV)
				.Include(t => t.TinTuc)
				.Include(t => t.TaiKhoanSinhVien)
				.ToListAsync();

			if (thongBaos == null || thongBaos.Count == 0)
			{
				return NotFound($"Không tìm thấy thông báo nào cho mã tài khoản: {maTKSV}");
			}

			return thongBaos;
		}

		// POST: api/ThongBao
		[HttpPost]
		public async Task<ActionResult<ThongBao>> PostThongBao(ThongBao thongBao)
		{
			thongBao.NgayTao = DateTime.Now;
			thongBao.TrangThai = "Chưa Xem";
			_context.ThongBaos.Add(thongBao);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetThongBaos), new { id = thongBao.Ma_TT }, thongBao);
		}

		// DELETE: api/ThongBao/{maTT}/{maTKSV}
		[HttpDelete("{maTT}/{maTKSV}")]
		public async Task<ActionResult> DeleteThongBao(string maTT, string maTKSV)
		{
			var thongBao = await _context.ThongBaos
				.FirstOrDefaultAsync(t => t.Ma_TT == maTT && t.Ma_TKSV == maTKSV);
			if (thongBao == null) return NotFound();
			_context.ThongBaos.Remove(thongBao);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		// POST: api/ThongBao/update
		[HttpPost("update")]
		public async Task<ActionResult<ThongBao>> UpdateThongBao(ThongBao thongBao)
		{
			var existingThongBao = await _context.ThongBaos
				.FirstOrDefaultAsync(t => t.Ma_TT == thongBao.Ma_TT && t.Ma_TKSV == thongBao.Ma_TKSV);

			if (existingThongBao == null)
			{
				return NotFound();
			}

			// Cập nhật các trường cần thiết
			existingThongBao.TrangThai = thongBao.TrangThai;
			// Cập nhật các trường khác nếu cần

			try
			{
				_context.Entry(existingThongBao).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ThongBaoExists(thongBao.Ma_TT, thongBao.Ma_TKSV))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return Ok(existingThongBao);
		}

		private bool ThongBaoExists(string maTT, string maTKSV)
		{
			return _context.ThongBaos.Any(e => e.Ma_TT == maTT && e.Ma_TKSV == maTKSV);
		}
	}
}