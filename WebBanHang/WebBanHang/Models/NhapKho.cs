using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class NhapKho
{
    public string Id { get; set; } = null!;

    public string? Idsp { get; set; }

    public int Soluong { get; set; }

    public double GiaNhap { get; set; }

    public double ThanhTien { get; set; }

    public DateTime NgayNhap { get; set; }

    public virtual SanPham? IdspNavigation { get; set; }
}
