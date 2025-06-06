using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace QLSV_API.Model
{
    public class Lop
    {
        [Key]
        public string MaLop { get; set; }
        public string TenLop { get; set; }

        // Danh sách sinh viên thuộc lớp này
        public ICollection<SinhVien> SinhViens { get; set; }

        public Lop()
        {
            SinhViens = new HashSet<SinhVien>();
        }
    }
}
