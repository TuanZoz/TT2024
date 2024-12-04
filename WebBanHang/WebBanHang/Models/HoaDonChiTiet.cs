using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class HoaDonChiTiet
{
    public string Id { get; set; } = null!;

    public string? Idsp { get; set; }

    public string? Idhd { get; set; }

    public int Soluong { get; set; }

    public double GiaBan { get; set; }

    public virtual HoaDon? IdhdNavigation { get; set; }

    public virtual SanPham? IdspNavigation { get; set; }
}
