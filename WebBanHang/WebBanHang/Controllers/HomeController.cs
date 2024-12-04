using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [ServiceFilter(typeof(SessionActionFilter))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuanLyBanHangOnline4Context db;
        public string idKh = null;
        public HomeController(ILogger<HomeController> logger, QuanLyBanHangOnline4Context context)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            GetSession();
            return View();
        }
        public IActionResult GioHang()
        {
            GetSession();
            if (idKh == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }
            ViewBag.idKh = idKh;
            return View();
        }
        public async Task<IActionResult> LichsuDonMua()
        {
            GetSession();
            if (idKh == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }
            ViewBag.idKh = idKh;
   
            var orders = await db.HoaDon
                .Where(hd => hd.Idkh == idKh)
                .Include(hd => hd.IdkhNavigation) // L?c theo m� kh�ch h�ng
                .Include(hd => hd.HoaDonChiTiets) // L?y chi ti?t h�a ??n
                .ThenInclude(hdc => hdc.IdspNavigation) // L?y th�ng tin s?n ph?m t? chi ti?t h�a ??n
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                TempData["Message"] = "Kh�ng c� ??n h�ng n�o c?a kh�ch h�ng.";
                return RedirectToAction("Index", "Home");
            }

            // T?o danh s�ch c�c ??n h�ng v� chi ti?t
            var orderViewModels = orders.Select(hd => new OrderViewModel
            {
                OrderId = hd.Id,
                CustomerName = hd.IdkhNavigation?.HoTen ?? "Ch?a c� t�n", // Tr�nh l?i null n?u kh�ng c� kh�ch h�ng
                OrderDate = hd.NgayDat,
                ShippingAddress = hd.DiaChi,
                TotalAmount = hd.ThanhTien,
                Status = hd.TrangThai,
                OrderDetails = hd.HoaDonChiTiets.Select(hdc => new OrderDetailViewModel
                {
                    ProductName = hdc.IdspNavigation?.TenSp ?? "S?n ph?m kh�ng x�c ??nh", // ??m b?o kh�ng b? null
                    Quantity = hdc.Soluong,
                    Price = hdc.GiaBan,
                    Total = hdc.Soluong * hdc.GiaBan
                }).ToList()
            }).ToList();

            return View(orderViewModels);
        }
        public void GetSession()
        {
            string userId = HttpContext.Session.GetString("D");
            string email = HttpContext.Session.GetString("Email");
            string userRole = HttpContext.Session.GetString("Role");

            // L?u th�ng tin v�o TempData
            TempData["D"] = userId;
            TempData["Email"] = email;  // L?u email v�o TempData["Email"]
            TempData["Role"] = userRole; // L?u userRole v�o TempData["Role"]
            idKh = userId;
        }
        public IActionResult Hanghoa(string loai)
        {
           
            var hangHoas = db.SanPham.AsQueryable();
            if (!string.IsNullOrEmpty(loai))
            {
                hangHoas = hangHoas.Where(p => p.Idloai == loai);
            }

            var result = hangHoas.Select(p => new SanPham
            {
                Id = p.Id,
                TenSp = p.TenSp,
                Idloai = p.Idloai,
                Soluong = p.Soluong,
                Gia = p.Gia,
                MoTa = p.MoTa,
                HinhAnh = p.HinhAnh,
            });
            GetSession();
            return View(result);
        }
        public IActionResult Search(string? query)
        {
            var hangHoasQuery = db.SanPham.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                hangHoasQuery = hangHoasQuery.Where(p => p.TenSp.Contains(query));
            }

            // L?y k?t qu? sau khi ?� truy v?n
            var result = hangHoasQuery.Select(p => new SanPham
            {
                Id = p.Id,
                TenSp = p.TenSp,
                Idloai = p.Idloai,
                Soluong = p.Soluong,
                Gia = p.Gia,
                MoTa = p.MoTa,
                HinhAnh = p.HinhAnh,
            }).ToList(); 

            GetSession();

            return View(result);
        }


    }
}
