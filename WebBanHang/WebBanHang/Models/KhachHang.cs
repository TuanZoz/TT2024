using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models;

public partial class KhachHang
{
    public string D { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? GioiTinh { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
