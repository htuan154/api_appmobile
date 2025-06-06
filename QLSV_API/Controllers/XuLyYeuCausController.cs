using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XuLyYeuCausController : ControllerBase
    {
        private readonly AppDbContext _context;

        public XuLyYeuCausController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/XuLyYeuCaus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<XuLyYeuCau>>> GetXuLyYeuCaus()
        {
            return await _context.XuLyYeuCaus
                .Include(x => x.TaiKhoan)
                .Include(x => x.YeuCau)
                .ToListAsync();
        }

        // GET: api/XuLyYeuCaus/{ma_yc}/{ma_tk}
        [HttpGet("{ma_yc}/{ma_tk}")]
        public async Task<ActionResult<XuLyYeuCau>> GetXuLyYeuCau(string ma_yc, string ma_tk)
        {
            var xuLy = await _context.XuLyYeuCaus
                .Include(x => x.TaiKhoan)
                .Include(x => x.YeuCau)
                .FirstOrDefaultAsync(x => x.Ma_YC == ma_yc && x.Ma_TK == ma_tk);

            if (xuLy == null) return NotFound();

            return xuLy;
        }

		// GET: api/XuLyYeuCaus/YeuCau/{ma_yc}
		[HttpGet("YeuCau/{ma_yc}")]
		public async Task<ActionResult<IEnumerable<XuLyYeuCau>>> GetXuLyYeuCauByMaYC(string ma_yc)
		{
			var list = await _context.XuLyYeuCaus
				.Include(x => x.TaiKhoan)
				.Include(x => x.YeuCau)
				.Where(x => x.Ma_YC == ma_yc)
				.ToListAsync();

			if (list == null || list.Count == 0) return NotFound();

			return list;
		}

		// GET: api/XuLyYeuCaus/TaiKhoan/{ma_tk}
		[HttpGet("TaiKhoan/{ma_tk}")]
		public async Task<ActionResult<IEnumerable<XuLyYeuCau>>> GetXuLyYeuCauByMaTK(string ma_tk)
		{
			var list = await _context.XuLyYeuCaus
				.Include(x => x.TaiKhoan)
				.Include(x => x.YeuCau)
				.Where(x => x.Ma_TK == ma_tk)
				.ToListAsync();

			// Trả về danh sách rỗng nếu không có
			return Ok(list);
		}

		// POST: api/XuLyYeuCaus
		[HttpPost]
        public async Task<ActionResult<XuLyYeuCau>> PostXuLyYeuCau(XuLyYeuCau xly)
        {
            _context.XuLyYeuCaus.Add(xly);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetXuLyYeuCau), new { ma_yc = xly.Ma_YC, ma_tk = xly.Ma_TK }, xly);
        }

        // DELETE: api/XuLyYeuCaus/{ma_yc}/{ma_tk}
        [HttpDelete("{ma_yc}/{ma_tk}")]
        public async Task<IActionResult> DeleteXuLyYeuCau(string ma_yc, string ma_tk)
        {
            var xly = await _context.XuLyYeuCaus.FindAsync(ma_yc, ma_tk);
            if (xly == null) return NotFound();

            _context.XuLyYeuCaus.Remove(xly);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
