using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinTucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TinTucController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TinTucs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TinTuc>>> GetTinTucs()
        {
            return await _context.TinTucs
                .Include(t => t.TaiKhoan) // Bao gồm thông tin từ bảng TaiKhoan
                .ToListAsync();
        }

        // GET: api/TinTucs/bytaikhoan/{maTK}
        [HttpGet("bytaikhoan/{maTK}")]
        public async Task<ActionResult<IEnumerable<TinTuc>>> GetByTaiKhoan(string maTK)
        {
            return await _context.TinTucs
                .Where(t => t.Ma_TK == maTK)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

		// GET: api/TinTucs/{maTT}
		[HttpGet("{maTT}")]
		public async Task<ActionResult<TinTuc>> GetTinTucById(string maTT)
		{
			var tinTuc = await _context.TinTucs
				.Include(t => t.TaiKhoan)
				.FirstOrDefaultAsync(t => t.Ma_TT == maTT);

			if (tinTuc == null)
			{
				return NotFound($"Không tìm thấy tin tức với mã: {maTT}");
			}

			return tinTuc;
		}

		// POST: api/TinTucs
		[HttpPost]
		public async Task<ActionResult<TinTuc>> PostTinTuc(TinTuc tinTuc)
		{
			tinTuc.NgayTao = DateTime.Now;  // Gán ngày tạo là thời gian hiện tại

			// Kiểm tra xem Ma_TK của TaiKhoan đã tồn tại trong cơ sở dữ liệu chưa
			var taiKhoanExist = await _context.TaiKhoans
											  .AnyAsync(tk => tk.Ma_TK == tinTuc.Ma_TK);

			if (!taiKhoanExist)
			{
				// Nếu không tồn tại, trả về lỗi
				return BadRequest($"TaiKhoan với Ma_TK {tinTuc.Ma_TK} không tồn tại.");
			}

			// Chỉ thêm TinTuc mà không thay đổi đối tượng TaiKhoan
			_context.TinTucs.Add(tinTuc);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				// In ra lỗi chi tiết để xem tại sao không thể lưu
				Console.WriteLine($"Lỗi khi lưu tin tức: {ex.InnerException?.Message}");
				return StatusCode(500, "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại.");
			}

			return CreatedAtAction(nameof(GetByTaiKhoan), new { maTK = tinTuc.Ma_TK }, tinTuc);
		}

		// DELETE: api/TinTucs/{id}
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTinTuc(string id)
        {
            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc == null) return NotFound();

            _context.TinTucs.Remove(tinTuc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

		// PUT: api/TinTucs/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> PutTinTuc(string id, TinTuc tinTuc)
		{
			if (id != tinTuc.Ma_TT)
			{
				return BadRequest("Mã tin tức không khớp với dữ liệu gửi lên.");
			}

			// Kiểm tra tin tức có tồn tại không
			var existingTinTuc = await _context.TinTucs.FindAsync(id);
			if (existingTinTuc == null)
			{
				return NotFound($"Không tìm thấy tin tức với mã {id}");
			}

			// Cập nhật thủ công các trường (tránh ghi đè đối tượng navigation như TaiKhoan)
			existingTinTuc.NoiDung = tinTuc.NoiDung;
			existingTinTuc.NgayTao = DateTime.Now;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				Console.WriteLine($"Lỗi khi cập nhật tin tức: {ex.InnerException?.Message}");
				return StatusCode(500, "Có lỗi xảy ra khi cập nhật dữ liệu.");
			}

			return NoContent();
		}

	}
}
