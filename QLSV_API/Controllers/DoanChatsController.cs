using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoanChatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoanChatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/DoanChats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoanChat>>> GetDoanChats()
        {
            return await _context.DoanChats
                .Include(dc => dc.YeuCau)
                .ToListAsync();
        }

        // GET: api/DoanChats/byyeucau/{maYC}
        [HttpGet("byyeucau/{maYC}")]
        public async Task<ActionResult<IEnumerable<DoanChat>>> GetByYeuCau(string maYC)
        {
            return await _context.DoanChats
                .Where(dc => dc.Ma_YC == maYC)
                .OrderBy(dc => dc.NgayTao)
                .ToListAsync();
        }

		// GET: api/DoanChats/bytaikhoan/{maTK}
		[HttpGet("bytaikhoan/{maTK}")]
		public async Task<ActionResult<IEnumerable<DoanChat>>> GetByTaiKhoan(string maTK)
		{
			var chats = await _context.DoanChats
				.Where(dc => dc.MaNguoiGui == maTK)
				.OrderBy(dc => dc.NgayTao)
				.ToListAsync();

			// Đừng return NotFound, cứ return danh sách trống (200 OK)
			return chats;
		}

		// POST: api/DoanChats
		[HttpPost]
        public async Task<ActionResult<DoanChat>> PostDoanChat(DoanChat chat)
        {
            chat.NgayTao = DateTime.Now;
            _context.DoanChats.Add(chat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByYeuCau), new { maYC = chat.Ma_YC }, chat);
        }

        // DELETE: api/DoanChats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoanChat(string id)
        {
            var chat = await _context.DoanChats.FindAsync(id);
            if (chat == null) return NotFound();

            _context.DoanChats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
