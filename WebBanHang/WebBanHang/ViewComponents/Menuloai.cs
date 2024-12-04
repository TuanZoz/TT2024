using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
namespace WebBanHang.ViewComponents
{
    public class Menuloai : ViewComponent
    {
        private readonly QuanLyBanHangOnline4Context db;
        public Menuloai(QuanLyBanHangOnline4Context context) => db = context;
        public IViewComponentResult Invoke()
        {
            var data = db.LoaiSanPham.Select(lo => new LoaiSanPham
            {
                Id = lo.Id,
                TenLoai = lo.TenLoai,
                MauSac = lo.MauSac,
                Size = lo.Size,
                MoTa = lo.MoTa
            });
            return View(data); 
        }
    }
}
