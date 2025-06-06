using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaiKhoansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TaiKhoans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaiKhoan>>> GetTaiKhoans()
        {
            return await _context.TaiKhoans
                                 .Include(t => t.NhanVien)
                                 .Include(t => t.LoaiTaiKhoan)
                                 .ToListAsync();
        }

        // GET: api/TaiKhoans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaiKhoan>> GetTaiKhoan(string id)
        {
            var taiKhoan = await _context.TaiKhoans
                                         .Include(t => t.NhanVien)
                                         .Include(t => t.LoaiTaiKhoan)
                                         .FirstOrDefaultAsync(t => t.Ma_TK == id);

            if (taiKhoan == null)
                return NotFound();

            return taiKhoan;
        }

        // POST: api/TaiKhoans
        [HttpPost]
        public async Task<ActionResult<TaiKhoan>> PostTaiKhoan(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaiKhoan), new { id = taiKhoan.Ma_TK }, taiKhoan);
        }

		// POST: api/TaiKhoans/CreateBulk
		[HttpPost("CreateBulk")]
		public async Task<IActionResult> CreateBulkTaiKhoans()
		{
			// Lấy tất cả nhân viên chưa có tài khoản
			var nhanViensWithoutAccount = await _context.NhanViens
				.Where(nv => !_context.TaiKhoans.Any(tk => tk.Ma_NV == nv.Ma_NV))
				.ToListAsync();

			if (nhanViensWithoutAccount.Count == 0)
				return Ok("Tất cả nhân viên đã có tài khoản.");

			foreach (var nv in nhanViensWithoutAccount)
			{
				// Tìm mã loại tài khoản tương ứng với chức vụ
				var loaiTK = await _context.LoaiTaiKhoans
					.FirstOrDefaultAsync(l => l.Ten_Loai == nv.ChucVu);

				if (loaiTK == null)
				{
					// Nếu không tìm thấy loại tài khoản tương ứng, bỏ qua nhân viên này
					continue;
				}

				var maNV = nv.Ma_NV;
				var maTK = maNV.Replace("NV", "TK"); // Chuyển đổi mã

				var newTaiKhoan = new TaiKhoan
				{
					Ma_TK = maTK,
					Ma_NV = maNV,
					Ma_Loai = loaiTK.Ma_Loai, // Sử dụng mã loại tìm được
					TenDangNhap = nv.Email,
					MatKhau = "123456" // TODO: Băm mật khẩu trước khi lưu vào DB trong môi trường thật
				};

				_context.TaiKhoans.Add(newTaiKhoan);
			}

			await _context.SaveChangesAsync();

			return Ok($"{nhanViensWithoutAccount.Count} tài khoản đã được tạo.");
		}


		// PUT: api/TaiKhoans/{id}
		[HttpPut("{id}")]
        public async Task<IActionResult> PutTaiKhoan(string id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.Ma_TK)
                return BadRequest();

            _context.Entry(taiKhoan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TaiKhoans.Any(t => t.Ma_TK == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/TaiKhoans/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaiKhoan(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
                return NotFound();

            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
