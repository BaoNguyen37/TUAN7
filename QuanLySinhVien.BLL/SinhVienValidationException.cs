namespace QuanLySinhVien.BLL;

/// <summary>Lỗi vi phạm quy tắc nghiệp vụ (validate dữ liệu).</summary>
public class SinhVienValidationException : Exception
{
    public SinhVienValidationException(string message) : base(message) { }
}
