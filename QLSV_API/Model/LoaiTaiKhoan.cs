using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
    public class LoaiTaiKhoan
    {
        [Key]
        public string Ma_Loai { get; set; }
        public string Ten_Loai { get; set; }

		[JsonIgnore]
		public ICollection<TaiKhoan>? TaiKhoans { get; set; }

	}
}
