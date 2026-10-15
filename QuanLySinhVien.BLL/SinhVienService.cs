using QuanLySinhVien.DAL.Entities;
using QuanLySinhVien.DAL.Repositories;

namespace QuanLySinhVien.BLL;

public class SinhVienService
{
    private readonly SinhVienRepository _repo = new();

    // ---------- CRUD ----------
    public Task<List<SinhVien>> LayDanhSachAsync() => _repo.LayDanhSachAsync();

    public Task<SinhVien?> TimTheoIdAsync(int id) => _repo.TimTheoIdAsync(id);

    public async Task<SinhVien> ThemAsync(SinhVien sv)
    {
        Validate(sv);
        sv.HoTen = sv.HoTen.Trim();
        sv.Lop = sv.Lop.Trim();
        return await _repo.ThemAsync(sv);
    }

    public async Task SuaAsync(SinhVien sv)
    {
        Validate(sv);
        sv.HoTen = sv.HoTen.Trim();
        sv.Lop = sv.Lop.Trim();
        if (!await _repo.SuaAsync(sv))
            throw new SinhVienValidationException("Không tìm thấy sinh viên cần sửa.");
    }

    public async Task XoaAsync(int id)
    {
        if (!await _repo.XoaAsync(id))
            throw new SinhVienValidationException("Không tìm thấy sinh viên cần xóa.");
    }

    // ---------- Validate nghiệp vụ ----------
    public static void Validate(SinhVien sv)
    {
        if (string.IsNullOrWhiteSpace(sv.HoTen))
            throw new SinhVienValidationException("Họ tên không được để trống.");

        if (string.IsNullOrWhiteSpace(sv.Lop))
            throw new SinhVienValidationException("Lớp không được để trống.");

        if (sv.DiemTB < 0 || sv.DiemTB > 10)
            throw new SinhVienValidationException("Điểm trung bình phải từ 0 đến 10.");

        if (sv.NamSinh < 1900 || sv.NamSinh > DateTime.Now.Year)
            throw new SinhVienValidationException($"Năm sinh phải từ 1900 đến {DateTime.Now.Year}.");
    }

    // ---------- Thống kê ----------
    public double TinhDiemTrungBinh(List<SinhVien> ds)
        => ds.Count == 0 ? 0 : ds.Average(s => s.DiemTB);

    public double? DiemCaoNhat(List<SinhVien> ds)
        => ds.Count == 0 ? null : ds.Max(s => s.DiemTB);

    public double? DiemThapNhat(List<SinhVien> ds)
        => ds.Count == 0 ? null : ds.Min(s => s.DiemTB);

    public async Task<List<SinhVien>> LocSinhVienTheoNamSinh(int namBatDau, int namKetThuc)
    {
        if (namBatDau > namKetThuc)
            throw new SinhVienValidationException("Năm bắt đầu không được lớn hơn năm kết thúc.");

        var all = await _repo.LayDanhSachAsync();
        return all.Where(s => s.NamSinh >= namBatDau && s.NamSinh <= namKetThuc).ToList();
    }
}
