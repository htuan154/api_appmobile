using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SinhVienController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SinhVienController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SinhVien
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SinhVien>>> GetSinhViens()
        {
            try
            {
                var students = await _context.SinhViens.ToListAsync();
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu sinh viên: {ex.Message}");
            }
        }

        // GET: api/SinhVien/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SinhVien>> GetSinhVien(string id)
        {
            try
            {
                var sinhVien = await _context.SinhViens.FindAsync(id);
                if (sinhVien == null)
                {
                    return NotFound();
                }
                return Ok(sinhVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy thông tin sinh viên: {ex.Message}");
            }
        }

        // POST: api/SinhVien
        [HttpPost]
        public async Task<ActionResult<SinhVien>> CreateSinhVien(SinhVien sinhVien)
        {
            try
            {
                _context.SinhViens.Add(sinhVien);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetSinhVien", new { id = sinhVien.Ma_SV }, sinhVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo sinh viên mới: {ex.Message}");
            }
        }

        // PUT: api/SinhVien/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSinhVien(string id, SinhVien sinhVien)
        {
            if (id != sinhVien.Ma_SV)
            {
                return BadRequest("ID sinh viên không khớp.");
            }

            try
            {
                _context.Entry(sinhVien).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SinhVienExists(id))
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
                return StatusCode(500, $"Lỗi khi cập nhật sinh viên: {ex.Message}");
            }
        }

        // DELETE: api/SinhVien/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinhVien(string id)
        {
            try
            {
                var sinhVien = await _context.SinhViens.FindAsync(id);
                if (sinhVien == null)
                {
                    return NotFound();
                }

                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa sinh viên: {ex.Message}");
            }
        }

        // Kiểm tra nếu sinh viên tồn tại
        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.Ma_SV == id);
        }

        // Thêm API tìm kiếm sinh viên theo tên
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SinhVien>>> SearchSinhVien(string name)
        {
            try
            {
                var sinhViens = await _context.SinhViens
                    .Where(sv => sv.Ten_SV.Contains(name))
                    .ToListAsync();

                if (!sinhViens.Any())
                {
                    return NotFound("Không tìm thấy sinh viên nào.");
                }

                return Ok(sinhViens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tìm kiếm sinh viên: {ex.Message}");
            }
        }
    }
}
