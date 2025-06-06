using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
using QLSV_API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLSV_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ThongBaoChatYeuCauController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ThongBaoChatYeuCauController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/ThongBaoChatYeuCau
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ThongBaoChatYeuCau>>> GetThongBaoChatYeuCaus()
		{
			return await _context.ThongBaoChatYeuCaus
				.Include(t => t.TaiKhoan)
				.Include(t => t.YeuCau)
				.ToListAsync();
		}

		// GET: api/ThongBaoChatYeuCau/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<ThongBaoChatYeuCau>> GetThongBaoChatYeuCau(string id)
		{
			var tb = await _context.ThongBaoChatYeuCaus
				.Include(t => t.TaiKhoan)
				.Include(t => t.YeuCau)
				.FirstOrDefaultAsync(t => t.Ma_TBCYC == id);

			if (tb == null)
				return NotFound();

			return tb;
		}

		// GET: api/ThongBaoChatYeuCau/TaiKhoan/{maTK}
		[HttpGet("TaiKhoan/{maTK}")]
		public async Task<ActionResult<IEnumerable<ThongBaoChatYeuCau>>> GetThongBaoByMaTK(string maTK)
		{
			var list = await _context.ThongBaoChatYeuCaus
				.Where(t => t.Ma_TK == maTK)
				.Include(t => t.YeuCau)
				.OrderByDescending(t => t.NgayThongBao)
				.ToListAsync();

			if (!list.Any())
				return NotFound("Không có thông báo nào cho tài khoản này.");

			return list;
		}

		// POST: api/ThongBaoChatYeuCau
		[HttpPost]
		public async Task<ActionResult<ThongBaoChatYeuCau>> PostThongBaoChatYeuCau(ThongBaoChatYeuCau tb)
		{
			if (string.IsNullOrWhiteSpace(tb.Ma_TBCYC))
			{
				tb.Ma_TBCYC = Guid.NewGuid().ToString();
			}
			tb.NgayThongBao = DateTime.Now;
			_context.ThongBaoChatYeuCaus.Add(tb);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetThongBaoChatYeuCau), new { id = tb.Ma_TBCYC }, tb);
		}

		// PUT: api/ThongBaoChatYeuCau/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> PutThongBaoChatYeuCau(string id, ThongBaoChatYeuCau tb)
		{
			if (id != tb.Ma_TBCYC)
				return BadRequest();

			_context.Entry(tb).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.ThongBaoChatYeuCaus.Any(e => e.Ma_TBCYC == id))
					return NotFound();
				else
					throw;
			}

			return NoContent();
		}

		// DELETE: api/ThongBaoChatYeuCau/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteThongBaoChatYeuCau(string id)
		{
			var tb = await _context.ThongBaoChatYeuCaus.FindAsync(id);
			if (tb == null)
				return NotFound();

			_context.ThongBaoChatYeuCaus.Remove(tb);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// POST: api/ThongBaoChatYeuCau/SendToAllXuLy
		[HttpPost("SendToAllXuLy")]
		public async Task<IActionResult> SendToAllXuLy(string ma_YC, string noiDung)
		{
			var xuLyList = await _context.XuLyYeuCaus
				.Where(x => x.Ma_YC == ma_YC)
				.Select(x => x.Ma_TK)
				.Distinct()
				.ToListAsync();

			if (!xuLyList.Any())
				return NotFound("Không tìm thấy ai đã xử lý yêu cầu này.");

			var thongBaos = xuLyList.Select(maTK => new ThongBaoChatYeuCau
			{
				Ma_TBCYC = Guid.NewGuid().ToString(),
				Ma_YC = ma_YC,
				Ma_TK = maTK,
				NoiDung = noiDung,
				NgayThongBao = DateTime.Now,
				TrangThai = "ChuaDoc"
			});

			_context.ThongBaoChatYeuCaus.AddRange(thongBaos);
			await _context.SaveChangesAsync();

			return Ok("Đã gửi thông báo đến tất cả người xử lý.");
		}
	}
}
