using Microsoft.EntityFrameworkCore;
using QLSV_API.Model;
namespace QLSV_API.Repository
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<SinhVien> SinhViens { get; set; }
		public DbSet<Lop> Lops { get; set; }
		public DbSet<TaiKhoanSinhVien> TaiKhoanSinhViens { get; set; }
		public DbSet<NhanVien> NhanViens { get; set; }
		public DbSet<LoaiTaiKhoan> LoaiTaiKhoans { get; set; }
		public DbSet<TaiKhoan> TaiKhoans { get; set; }
		public DbSet<LoaiYeuCau> LoaiYeuCaus { get; set; }
		public DbSet<YeuCau> YeuCaus { get; set; }
		public DbSet<XuLyYeuCau> XuLyYeuCaus { get; set; }
		public DbSet<TinTuc> TinTucs { get; set; }
		public DbSet<DoanChat> DoanChats { get; set; }
		public DbSet<LichSuYeuCau> LichSuYeuCaus { get; set; }
		public DbSet<ThongBaoYeuCau> ThongBaoYeuCaus { get; set; }
		public DbSet<ThongBao> ThongBaos { get; set; }
		public DbSet<ThongBaoChatYeuCau> ThongBaoChatYeuCaus { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Cấu hình tránh multiple cascade paths cho ThongBaoYeuCau
			modelBuilder.Entity<ThongBaoYeuCau>()
				.HasOne(tbyc => tbyc.YeuCau)
				.WithMany() // hoặc WithMany(yc => yc.ThongBaoYeuCaus) nếu có navigation
				.HasForeignKey(tbyc => tbyc.Ma_YC)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<ThongBaoYeuCau>()
				.HasOne(tbyc => tbyc.TaiKhoanSinhVien)
				.WithMany()
				.HasForeignKey(tbyc => tbyc.Ma_TKSV)
				.OnDelete(DeleteBehavior.Restrict);

			// Khóa chính tổng hợp
			modelBuilder.Entity<XuLyYeuCau>().HasKey(x => new { x.Ma_YC, x.Ma_TK, x.NgayXuLy });
			modelBuilder.Entity<ThongBao>().HasKey(x => new { x.Ma_TT, x.Ma_TKSV });

			// Định nghĩa mối quan hệ giữa SinhVien và TaiKhoanSinhVien
			modelBuilder.Entity<TaiKhoanSinhVien>()
				.HasOne(tsv => tsv.SinhVien)
				.WithMany(sv => sv.TaiKhoanSinhViens)
				.HasForeignKey(tsv => tsv.Ma_SV);

			// Định nghĩa mối quan hệ giữa NhanVien và TaiKhoan
			modelBuilder.Entity<TaiKhoan>()
				.HasOne(tk => tk.NhanVien)
				.WithMany(nv => nv.TaiKhoans)
				.HasForeignKey(tk => tk.Ma_NV);

			// Cấu hình timezone cho PostgreSQL
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

			modelBuilder.Entity<LoaiTaiKhoan>().HasData(
				 new LoaiTaiKhoan { Ma_Loai = "ADMIN", Ten_Loai = "Administrator" },
				 new LoaiTaiKhoan { Ma_Loai = "NEWS", Ten_Loai = "News Publisher" },
				 new LoaiTaiKhoan { Ma_Loai = "REQUEST", Ten_Loai = "Request Handler" },
				 new LoaiTaiKhoan { Ma_Loai = "STUDENT", Ten_Loai = "Student Manager" }
			 );

			modelBuilder.Entity<NhanVien>().HasData(
				new NhanVien
				{
					Ma_NV = "NV0001",
					Ten_NV = "Lê Đăng Hoàng Tuấn",
					DiaChi = "Bạc Liêu",
					NgaySinh = DateTime.SpecifyKind(new DateTime(2004, 4, 15), DateTimeKind.Utc), // Fix timezone
					NamVaoLam = 2015,
					ChucVu = "Admin",
					Email = "htuan15424@gmail.com",
					Gioitinh = "Nam",
					SDT = "0987654321"
				}
			);

			// Thêm tài khoản admin vào hệ thống
			modelBuilder.Entity<TaiKhoan>().HasData(
				new TaiKhoan
				{
					Ma_TK = "A0001",
					TenDangNhap = "admin1",
					MatKhau = "123456789",
					Ma_NV = "NV0001",
					Ma_Loai = "ADMIN"
				}
			);

			base.OnModelCreating(modelBuilder);
		}
	}
}