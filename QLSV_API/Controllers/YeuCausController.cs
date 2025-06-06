using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YeuCausController : ControllerBase
    {
        private readonly AppDbContext  _context;

        public YeuCausController(AppDbContext  context)
        {
            _context = context;
        }

        // GET: api/YeuCaus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<YeuCau>>> GetYeuCaus()
        {
            return await _context.YeuCaus
                .Include(y => y.TaiKhoanSinhVien)
                .Include(y => y.LoaiYeuCau)
                .ToListAsync();
        }

        // GET: api/YeuCaus/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<YeuCau>> GetYeuCau(string id)
        {
            var yeuCau = await _context.YeuCaus
                .Include(y => y.TaiKhoanSinhVien)
                .Include(y => y.LoaiYeuCau)
                .FirstOrDefaultAsync(y => y.Ma_YC == id);

            if (yeuCau == null)
                return NotFound();

            return yeuCau;
        }

        // GET: api/YeuCaus/ByTaiKhoanSinhVien/{maTKSV}
        [HttpGet("ByTaiKhoanSinhVien/{maTKSV}")]
        public async Task<ActionResult<IEnumerable<YeuCau>>> GetYeuCausByTaiKhoanSinhVien(string maTKSV)
        {
            var yeuCaus = await _context.YeuCaus
                .Include(y => y.TaiKhoanSinhVien)
                .Include(y => y.LoaiYeuCau)
                .Where(y => y.Ma_TKSV == maTKSV)  // Giả sử trường khoá ngoại tên MaTKSV
                .ToListAsync();

            if (yeuCaus == null || yeuCaus.Count == 0)
                return NotFound();

            return yeuCaus;
        }

        // POST: api/YeuCaus
        [HttpPost]
        public async Task<ActionResult<YeuCau>> PostYeuCau(YeuCau yc)
        {
            _context.YeuCaus.Add(yc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetYeuCau), new { id = yc.Ma_YC }, yc);
        }

        // PUT: api/YeuCaus/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutYeuCau(string id, YeuCau yc)
        {
            if (id != yc.Ma_YC)
                return BadRequest();

            _context.Entry(yc).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.YeuCaus.Any(e => e.Ma_YC == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/YeuCaus/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteYeuCau(string id)
        {
            var yc = await _context.YeuCaus.FindAsync(id);
            if (yc == null) return NotFound();

            _context.YeuCaus.Remove(yc);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
