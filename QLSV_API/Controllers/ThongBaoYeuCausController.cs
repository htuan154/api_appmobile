using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongBaoYeuCausController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThongBaoYeuCausController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ThongBaoYeuCaus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThongBaoYeuCau>>> GetThongBaoYeuCaus()
        {
            return await _context.ThongBaoYeuCaus
                .Include(tb => tb.YeuCau)
                .Include(tb => tb.TaiKhoanSinhVien)
                .ToListAsync();
        }

        // GET: api/ThongBaoYeuCaus/byyeucau/{maYC}
        [HttpGet("byyeucau/{maYC}")]
        public async Task<ActionResult<IEnumerable<ThongBaoYeuCau>>> GetByYeuCau(string maYC)
        {
            return await _context.ThongBaoYeuCaus
                .Where(tb => tb.Ma_YC == maYC)
                .OrderByDescending(tb => tb.NgayThongBao)
                .ToListAsync();
        }

		// GET: api/ThongBaoYeuCaus/bytaikhoan/{maTKSV}
		[HttpGet("bytaikhoan/{maTKSV}")]
		public async Task<ActionResult<IEnumerable<ThongBaoYeuCau>>> GetByTaiKhoanSinhVien(string maTKSV)
		{
			return await _context.ThongBaoYeuCaus
				.Where(tb => tb.Ma_TKSV == maTKSV)
				.OrderByDescending(tb => tb.NgayThongBao)
				.Include(tb => tb.YeuCau)
				.Include(tb => tb.TaiKhoanSinhVien)
				.ToListAsync();
		}

		// POST: api/ThongBaoYeuCaus
		[HttpPost]
        public async Task<ActionResult<ThongBaoYeuCau>> PostThongBaoYeuCau(ThongBaoYeuCau thongBao)
        {
            thongBao.NgayThongBao = DateTime.Now;  // Gán ngày thông báo
            _context.ThongBaoYeuCaus.Add(thongBao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByYeuCau), new { maYC = thongBao.Ma_YC }, thongBao);
        }

		// PUT: api/ThongBaoYeuCaus/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> PutThongBaoYeuCau(string id, ThongBaoYeuCau thongBaoUpdate)
		{
			if (id != thongBaoUpdate.Ma_TBYC)
			{
				return BadRequest("Mã thông báo không khớp.");
			}

			var existingThongBao = await _context.ThongBaoYeuCaus.FindAsync(id);
			if (existingThongBao == null)
			{
				return NotFound();
			}

			// Cập nhật các thuộc tính cần thiết
			existingThongBao.TrangThai = thongBaoUpdate.TrangThai;
			existingThongBao.NoiDung = thongBaoUpdate.NoiDung;
			existingThongBao.NgayThongBao = thongBaoUpdate.NgayThongBao;

			_context.Entry(existingThongBao).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// DELETE: api/ThongBaoYeuCaus/{id}
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThongBaoYeuCau(string id)
        {
            var thongBao = await _context.ThongBaoYeuCaus.FindAsync(id);
            if (thongBao == null) return NotFound();

            _context.ThongBaoYeuCaus.Remove(thongBao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
