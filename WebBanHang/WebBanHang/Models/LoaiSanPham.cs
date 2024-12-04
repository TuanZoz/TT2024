using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class LoaiSanPham
{
    public string Id { get; set; } = null!;

    public string TenLoai { get; set; } = null!;

    public string? MauSac { get; set; }

    public string? Size { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
