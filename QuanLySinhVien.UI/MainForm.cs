using System.Globalization;
using QuanLySinhVien.BLL;                 // chỉ dùng BLL ...
using QuanLySinhVien.DAL.Entities;        // ... và DAL.Entities (không dùng AppDbContext / Repository)

namespace QuanLySinhVien.UI;

public class MainForm : Form
{
    private readonly SinhVienService _service = new();

    // trạng thái lọc hiện tại (null = đang xem tất cả)
    private int? _locTu, _locDen;

    private readonly TextBox txtHoTen = new();
    private readonly TextBox txtLop = new();
    private readonly TextBox txtDiem = new();
    private readonly NumericUpDown nudNamSinh = new();
    private readonly NumericUpDown nudFrom = new();
    private readonly NumericUpDown nudTo = new();
    private readonly Button btnAdd = new();
    private readonly Button btnEdit = new();
    private readonly Button btnDelete = new();
    private readonly Button btnReload = new();
    private readonly Button btnFilter = new();
    private readonly DataGridView dgv = new();
    private readonly Label lblStats = new();

    public MainForm()
    {
        BuildUi();
        Load += async (_, _) => await Run(LoadAllAsync);
    }

    // ================= GIAO DIỆN =================
    private void BuildUi()
    {
        Text = "Quản lý Sinh viên - 3 Project Architecture";
        Font = new Font("Segoe UI", 10F);
        ClientSize = new Size(900, 600);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;

        var lblTitle = new Label
        {
            Text = "QUẢN LÝ SINH VIÊN - KIẾN TRÚC 3 TẦNG (3 PROJECT)",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 12),
            Size = new Size(900, 40),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Hàng 1
        Place(txtHoTen, 90, 68, 240);
        Place(txtLop, 420, 68, 200);
        Place(txtDiem, 730, 68, 110);

        // Hàng 2
        nudNamSinh.Minimum = 1900; nudNamSinh.Maximum = DateTime.Now.Year; nudNamSinh.Value = 2005;
        Place(nudNamSinh, 90, 108, 80);
        SetupButton(btnAdd, "＋ Thêm", 190, 106, BtnAdd_Click);
        SetupButton(btnEdit, "✎ Sửa", 300, 106, BtnEdit_Click);
        SetupButton(btnDelete, "✖ Xóa", 410, 106, BtnDelete_Click);
        SetupButton(btnReload, "⟳ Tải lại", 520, 106, BtnReload_Click);

        // Hàng 3
        nudFrom.Minimum = 1900; nudFrom.Maximum = DateTime.Now.Year; nudFrom.Value = 1900;
        nudTo.Minimum = 1900; nudTo.Maximum = DateTime.Now.Year; nudTo.Value = DateTime.Now.Year;
        Place(nudFrom, 130, 152, 80);
        Place(nudTo, 255, 152, 80);
        SetupButton(btnFilter, "🔍 Lọc", 360, 150, BtnFilter_Click);

        // Lưới dữ liệu
        dgv.Location = new Point(20, 195);
        dgv.Size = new Size(860, 340);
        dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.MultiSelect = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.BackgroundColor = SystemColors.ControlLight;
        dgv.RowHeadersVisible = false;
        dgv.SelectionChanged += Dgv_SelectionChanged;

        lblStats.Location = new Point(20, 550);
        lblStats.Size = new Size(860, 30);
        lblStats.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStats.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblStats.ForeColor = Color.Navy;

        Controls.AddRange(new Control[]
        {
            lblTitle,
            Lbl("Họ tên:", 20, 71), txtHoTen,
            Lbl("Lớp:", 350, 71), txtLop,
            Lbl("Điểm TB:", 650, 71), txtDiem,
            Lbl("Năm sinh:", 20, 111), nudNamSinh,
            btnAdd, btnEdit, btnDelete, btnReload,
            Lbl("Lọc năm sinh:", 20, 155), nudFrom,
            Lbl("đến", 220, 155), nudTo,
            btnFilter,
            dgv, lblStats
        });
    }

    private static Label Lbl(string text, int x, int y)
        => new() { Text = text, Location = new Point(x, y), AutoSize = true };

    private static void Place(Control c, int x, int y, int w)
    {
        c.Location = new Point(x, y);
        c.Width = w;
    }

    private static void SetupButton(Button b, string text, int x, int y, EventHandler onClick)
    {
        b.Text = text;
        b.Location = new Point(x, y);
        b.Size = new Size(100, 32);
        b.Click += onClick;
    }

    // ================= NẠP DỮ LIỆU =================
    private async Task LoadAllAsync()
    {
        _locTu = _locDen = null;
        Bind(await _service.LayDanhSachAsync());
    }

    private async Task RefreshAsync()
    {
        if (_locTu is int a && _locDen is int b)
            Bind(await _service.LocSinhVienTheoNamSinh(a, b));
        else
            Bind(await _service.LayDanhSachAsync());
    }

    private void Bind(List<SinhVien> list)
    {
        dgv.DataSource = list;
        if (dgv.Columns["DiemTB"] is DataGridViewColumn diemCol)
            diemCol.DefaultCellStyle.Format = "0.00";

        var tb = _service.TinhDiemTrungBinh(list);
        var max = _service.DiemCaoNhat(list);
        var min = _service.DiemThapNhat(list);
        lblStats.Text =
            $"Tổng: {list.Count} sinh viên  |  Điểm TB: {tb:0.00}  |  " +
            $"Cao nhất: {(max.HasValue ? max.Value.ToString("0.00") : "—")}  |  " +
            $"Thấp nhất: {(min.HasValue ? min.Value.ToString("0.00") : "—")}";
    }

    // ================= SỰ KIỆN =================
    private async void BtnAdd_Click(object? s, EventArgs e) => await Run(async () =>
    {
        await _service.ThemAsync(ReadInput());
        ClearInput();
        await RefreshAsync();
    });

    private async void BtnEdit_Click(object? s, EventArgs e) => await Run(async () =>
    {
        var selected = GetSelected();
        if (selected == null)
        {
            MessageBox.Show("Hãy chọn một sinh viên trong danh sách để sửa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var sv = ReadInput();
        sv.Id = selected.Id;
        await _service.SuaAsync(sv);
        await RefreshAsync();
    });

    private async void BtnDelete_Click(object? s, EventArgs e) => await Run(async () =>
    {
        var selected = GetSelected();
        if (selected == null)
        {
            MessageBox.Show("Hãy chọn một sinh viên trong danh sách để xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var ok = MessageBox.Show($"Xóa sinh viên \"{selected.HoTen}\"?", "Xác nhận",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (ok != DialogResult.Yes) return;

        await _service.XoaAsync(selected.Id);
        ClearInput();
        await RefreshAsync();
    });

    private async void BtnReload_Click(object? s, EventArgs e) => await Run(async () =>
    {
        ClearInput();
        nudFrom.Value = 1900;
        nudTo.Value = DateTime.Now.Year;
        await LoadAllAsync();
    });

    private async void BtnFilter_Click(object? s, EventArgs e) => await Run(async () =>
    {
        int from = (int)nudFrom.Value, to = (int)nudTo.Value;
        var list = await _service.LocSinhVienTheoNamSinh(from, to);   // ném lỗi nếu from > to
        _locTu = from; _locDen = to;
        Bind(list);
    });

    private void Dgv_SelectionChanged(object? s, EventArgs e)
    {
        if (GetSelected() is not SinhVien sv) return;
        txtHoTen.Text = sv.HoTen;
        txtLop.Text = sv.Lop;
        txtDiem.Text = sv.DiemTB.ToString("0.##", CultureInfo.InvariantCulture);
        nudNamSinh.Value = Math.Clamp(sv.NamSinh, (int)nudNamSinh.Minimum, (int)nudNamSinh.Maximum);
    }

    // ================= TIỆN ÍCH =================
    private SinhVien? GetSelected() => dgv.CurrentRow?.DataBoundItem as SinhVien;

    private SinhVien ReadInput()
    {
        // Cho phép gõ 8.5 hoặc 8,5
        var text = txtDiem.Text.Trim().Replace(',', '.');
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var diem))
            throw new SinhVienValidationException("Điểm TB phải là một số hợp lệ (ví dụ 8.5).");

        return new SinhVien
        {
            HoTen = txtHoTen.Text,
            Lop = txtLop.Text,
            DiemTB = diem,
            NamSinh = (int)nudNamSinh.Value
        };
    }

    private void ClearInput()
    {
        txtHoTen.Clear();
        txtLop.Clear();
        txtDiem.Clear();
        nudNamSinh.Value = 2005;
        txtHoTen.Focus();
    }

    private static async Task Run(Func<Task> action)
    {
        try { await action(); }
        catch (SinhVienValidationException ex)
        {
            MessageBox.Show(ex.Message, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
