using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;

namespace QLSV_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoaiYeuCausController : ControllerBase
	{
		private readonly AppDbContext _context;

		public LoaiYeuCausController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/LoaiYeuCaus
		[HttpGet]
		public async Task<ActionResult<IEnumerable<LoaiYeuCau>>> GetLoaiYeuCaus()
		{
			return await _context.LoaiYeuCaus.ToListAsync();
		}

		// GET: api/LoaiYeuCaus/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<LoaiYeuCau>> GetLoaiYeuCau(string id)
		{
			var loai = await _context.LoaiYeuCaus.FindAsync(id);

			if (loai == null)
				return NotFound();

			return loai;
		}

		// POST: api/LoaiYeuCaus
		[HttpPost]
		public async Task<ActionResult<LoaiYeuCau>> PostLoaiYeuCau(LoaiYeuCau loai)
		{
			_context.LoaiYeuCaus.Add(loai);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetLoaiYeuCau), new { id = loai.Ma_loaiYC }, loai);
		}

		// PUT: api/LoaiYeuCaus/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> PutLoaiYeuCau(string id, LoaiYeuCau loai)
		{
			if (id != loai.Ma_loaiYC)
				return BadRequest();

			_context.Entry(loai).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.LoaiYeuCaus.Any(e => e.Ma_loaiYC == id))
					return NotFound();
				else
					throw;
			}

			return NoContent();
		}

		// DELETE: api/LoaiYeuCaus/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteLoaiYeuCau(string id)
		{
			var loai = await _context.LoaiYeuCaus.FindAsync(id);
			if (loai == null)
				return NotFound();

			var danhSachYeuCau = await _context.YeuCaus
				.Where(y => y.Ma_loaiYC == id)
				.ToListAsync();

			foreach (var yc in danhSachYeuCau)
			{
				var thongBaoYC = await _context.ThongBaoYeuCaus
					.Where(tb => tb.Ma_YC == yc.Ma_YC)
					.ToListAsync();

				_context.ThongBaoYeuCaus.RemoveRange(thongBaoYC);
			}

			_context.YeuCaus.RemoveRange(danhSachYeuCau);

			_context.LoaiYeuCaus.Remove(loai);

			await _context.SaveChangesAsync();

			return NoContent();
		}

	}
}
