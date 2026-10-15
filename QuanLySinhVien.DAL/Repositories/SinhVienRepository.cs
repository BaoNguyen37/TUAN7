using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.DAL.Entities;

namespace QuanLySinhVien.DAL.Repositories;

public class SinhVienRepository
{
    static SinhVienRepository()
    {
        // Tự tạo database + bảng SinhViens nếu chưa có.
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
    }

    public async Task<List<SinhVien>> LayDanhSachAsync()
    {
        using var db = new AppDbContext();
        return await db.SinhViens.AsNoTracking().OrderBy(s => s.Id).ToListAsync();
    }

    public async Task<SinhVien?> TimTheoIdAsync(int id)
    {
        using var db = new AppDbContext();
        return await db.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SinhVien> ThemAsync(SinhVien sv)
    {
        using var db = new AppDbContext();
        db.SinhViens.Add(sv);
        await db.SaveChangesAsync();
        return sv;
    }

    public async Task<bool> SuaAsync(SinhVien sv)
    {
        using var db = new AppDbContext();
        var existing = await db.SinhViens.FindAsync(sv.Id);
        if (existing == null) return false;

        existing.HoTen = sv.HoTen;
        existing.Lop = sv.Lop;
        existing.DiemTB = sv.DiemTB;
        existing.NamSinh = sv.NamSinh;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> XoaAsync(int id)
    {
        using var db = new AppDbContext();
        var existing = await db.SinhViens.FindAsync(id);
        if (existing == null) return false;

        db.SinhViens.Remove(existing);
        await db.SaveChangesAsync();
        return true;
    }
}
