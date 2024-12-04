using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class HoaDon
{
    public string Id { get; set; } = null!;

    public string? Idkh { get; set; }

    public double ThanhTien { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public string? DiaChi { get; set; }

    public DateTime NgayDat { get; set; }

    public DateTime? NgayGiao { get; set; }

    public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();

    public virtual KhachHang? IdkhNavigation { get; set; }
}
