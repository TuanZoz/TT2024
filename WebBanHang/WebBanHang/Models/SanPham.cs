using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class SanPham
{
    public string Id { get; set; } = null!;

    public string? TenSp { get; set; }

    public string? Idloai { get; set; }

    public int? Soluong { get; set; }

    public double? Gia { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();

    public virtual LoaiSanPham? IdloaiNavigation { get; set; }

    public virtual ICollection<NhapKho> NhapKhos { get; set; } = new List<NhapKho>();
}
