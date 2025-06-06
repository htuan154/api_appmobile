using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichSuYeuCausController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LichSuYeuCausController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LichSuYeuCaus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LichSuYeuCau>>> GetLichSuYeuCaus()
        {
            return await _context.LichSuYeuCaus
                .Include(l => l.YeuCau)
                .ToListAsync();
        }

        // GET: api/LichSuYeuCaus/byyeucau/YC001
        [HttpGet("byyeucau/{maYC}")]
        public async Task<ActionResult<IEnumerable<LichSuYeuCau>>> GetByYeuCau(string maYC)
        {
            return await _context.LichSuYeuCaus
                .Where(l => l.Ma_YC == maYC)
                .OrderByDescending(l => l.Ma_LSYC)
                .ToListAsync();
        }

        // POST: api/LichSuYeuCaus
        [HttpPost]
        public async Task<ActionResult<LichSuYeuCau>> PostLichSuYeuCau(LichSuYeuCau lichSu)
        {
            _context.LichSuYeuCaus.Add(lichSu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByYeuCau), new { maYC = lichSu.Ma_YC }, lichSu);
        }

        // DELETE: api/LichSuYeuCaus/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLichSuYeuCau(string id)
        {
            var lichSu = await _context.LichSuYeuCaus.FindAsync(id);
            if (lichSu == null) return NotFound();

            _context.LichSuYeuCaus.Remove(lichSu);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
