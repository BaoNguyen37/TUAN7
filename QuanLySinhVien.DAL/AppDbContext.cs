using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.DAL.Entities;

namespace QuanLySinhVien.DAL;

public class AppDbContext : DbContext
{
    // Đổi tại đây nếu cần: tên server, tên database, hoặc dùng SQL Login
    // (User Id=sa;Password=...;) thay cho Trusted_Connection.
    private const string ConnectionString =
        @"Server=BAONGUYEN\BAONGUYEN1;Database=QuanLySinhVien;Trusted_Connection=True;TrustServerCertificate=True;";

    public DbSet<SinhVien> SinhViens => Set<SinhVien>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SinhVien>(e =>
        {
            e.Property(s => s.HoTen).IsRequired();
            e.Property(s => s.Lop).IsRequired();
        });
    }
}
