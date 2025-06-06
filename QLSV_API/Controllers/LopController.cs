using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LopController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LopController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lop
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lop>>> GetLops()
        {
            try
            {
                var lops = await _context.Lops.ToListAsync();
                return Ok(lops);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu lớp: {ex.Message}");
            }
        }

        // GET: api/Lop/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Lop>> GetLop(string id)
        {
            try
            {
                var lop = await _context.Lops.FindAsync(id);
                if (lop == null)
                {
                    return NotFound();
                }
                return Ok(lop);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy thông tin lớp: {ex.Message}");
            }
        }

		// GET: api/Lop/{maLop}/sinhviens
		[HttpGet("{maLop}/sinhviens")]
		public async Task<ActionResult<IEnumerable<SinhVien>>> GetSinhViensByLop(string maLop)
		{
			try
			{
				var sinhViens = await _context.SinhViens
					.Where(sv => sv.MaLop == maLop)
					.ToListAsync();

				if (!sinhViens.Any())
				{
					return NotFound($"Không tìm thấy sinh viên nào trong lớp có mã {maLop}.");
				}

				return Ok(sinhViens);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi khi lấy danh sách sinh viên của lớp {maLop}: {ex.Message}");
			}
		}


		// POST: api/Lop
		[HttpPost]
        public async Task<ActionResult<Lop>> CreateLop(Lop lop)
        {
            try
            {
                _context.Lops.Add(lop);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetLop", new { id = lop.MaLop }, lop);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo lớp mới: {ex.Message}");
            }
        }

        // PUT: api/Lop/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLop(string id, Lop lop)
        {
            if (id != lop.MaLop)
            {
                return BadRequest("ID lớp không khớp.");
            }

            try
            {
                _context.Entry(lop).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LopExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật lớp: {ex.Message}");
            }
        }

        // DELETE: api/Lop/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLop(string id)
        {
            try
            {
                var lop = await _context.Lops.FindAsync(id);
                if (lop == null)
                {
                    return NotFound();
                }

                _context.Lops.Remove(lop);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa lớp: {ex.Message}");
            }
        }

        // Kiểm tra nếu lớp tồn tại
        private bool LopExists(string id)
        {
            return _context.Lops.Any(e => e.MaLop == id);
        }
    }
}
