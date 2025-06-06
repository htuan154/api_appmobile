using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
	public class SinhVien
	{
		[Key]
		public string Ma_SV { get; set; }
		public string Ten_SV { get; set; }
		public string Gioi_Tinh { get; set; }
		public string DiaChi { get; set; }
		public DateTime NgaySinh { get; set; }
		public int KhoaHoc { get; set; }
		public string BacDaoTao { get; set; }
		public string LoaiHinhDaoTao { get; set; }
		public string Nganh { get; set; }
		public string LopHoc { get; set; }
		public string Email { get; set; }

		[ForeignKey("Lop")]
		public string MaLop { get; set; }

		[JsonIgnore] // Ngăn serialize/deserialize navigation property
		public virtual Lop? Lop { get; set; }

		[JsonIgnore] // Ngăn serialize/deserialize navigation property
		public virtual ICollection<TaiKhoanSinhVien>? TaiKhoanSinhViens { get; set; } = new List<TaiKhoanSinhVien>();

		private static int currentMaSV = 0;

		public SinhVien()
		{
			
		}

		private string GenerateNextMaSV()
		{
			currentMaSV++;
			return currentMaSV.ToString("D10");
		}
	}
}