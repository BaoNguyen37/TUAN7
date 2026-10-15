using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.DAL.Entities;

public class SinhVien
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string HoTen { get; set; } = string.Empty;

    [Required]
    public string Lop { get; set; } = string.Empty;

    public double DiemTB { get; set; }

    public int NamSinh { get; set; }
}
