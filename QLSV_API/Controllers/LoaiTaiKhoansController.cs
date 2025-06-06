using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiTaiKhoansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoaiTaiKhoansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LoaiTaiKhoans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiTaiKhoan>>> GetLoaiTaiKhoans()
        {
            return await _context.LoaiTaiKhoans.ToListAsync();
        }

        // GET: api/LoaiTaiKhoans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiTaiKhoan>> GetLoaiTaiKhoan(string id)
        {
            var loai = await _context.LoaiTaiKhoans.FindAsync(id);

            if (loai == null)
                return NotFound();

            return loai;
        }

        // POST: api/LoaiTaiKhoans
        [HttpPost]
        public async Task<ActionResult<LoaiTaiKhoan>> PostLoaiTaiKhoan(LoaiTaiKhoan loai)
        {
            _context.LoaiTaiKhoans.Add(loai);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoaiTaiKhoan), new { id = loai.Ma_Loai }, loai);
        }

        // PUT: api/LoaiTaiKhoans/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoaiTaiKhoan(string id, LoaiTaiKhoan loai)
        {
            if (id != loai.Ma_Loai)
                return BadRequest();

            _context.Entry(loai).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.LoaiTaiKhoans.Any(e => e.Ma_Loai == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/LoaiTaiKhoans/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoaiTaiKhoan(string id)
        {
            var loai = await _context.LoaiTaiKhoans.FindAsync(id);
            if (loai == null)
                return NotFound();

            _context.LoaiTaiKhoans.Remove(loai);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
