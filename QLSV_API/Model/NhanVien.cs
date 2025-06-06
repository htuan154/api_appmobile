using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
    public class NhanVien
    {
        [Key]
        public string Ma_NV { get; set; }
        public string Ten_NV { get; set; }
        public string DiaChi { get; set; }
        public DateTime NgaySinh { get; set; }
        public int NamVaoLam { get; set; }
        public string ChucVu { get; set; }
        public string Email { get; set; }
        public string Gioitinh { get; set; }
		public string SDT { get; set; }

		[JsonIgnore]
		public ICollection<TaiKhoan>? TaiKhoans { get; set; }

	}

}
