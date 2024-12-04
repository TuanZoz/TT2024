using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [ServiceFilter(typeof(SessionActionFilter))]
    public class KhachHangController : Controller
    {
        public readonly ILogger<HomeController> _logger;
        private readonly QuanLyBanHangOnline4Context db;
        public string UserId = null;
        public string Email;
        public string UserRole = null;
        public KhachHangController(ILogger<HomeController> logger, QuanLyBanHangOnline4Context context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet]
        public IActionResult DangKy()
        {   
            return View("~/Views/KhachHang/DangKy.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(string hoTen, string sdt, string password, string diaChi, string gioiTinh, DateTime ngaySinh, string email, string confirmPassword)
        {
            var khachHangs = await db.KhachHang.ToListAsync();
            var validKhachHangs = khachHangs.Where(kh => int.TryParse(kh.D, out _)).ToList();
            var maxId = validKhachHangs.Any() ? validKhachHangs.Max(kh => int.Parse(kh.D)) : 0;

            var newId = maxId + 1;

            var exists = await db.KhachHang.AnyAsync(kh => kh.D == newId.ToString());
            while (exists)
            {
                newId++;
                exists = await db.KhachHang.AnyAsync(kh => kh.D == newId.ToString());
            }

            var newKhachHang = new KhachHang
            {
                D = newId.ToString(), 
                HoTen = hoTen,
                Password = password,
                GioiTinh = gioiTinh,
                NgaySinh = ngaySinh,
                Email = email,
                Sdt = sdt,
                DiaChi = diaChi,
                Role = "Khách hàng"
            };

            if (hoTen.Length < 6)
            {
                ModelState.AddModelError("HoTen", "Họ tên dài hơn 6 ký tự");
            }

            if (db.KhachHang.Any(c => c.Email == email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
            }
            if (password != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu không trùng khớp");
            }

            if (!string.IsNullOrEmpty(sdt) && !Regex.IsMatch(sdt, @"^\d+$"))
            {
                ModelState.AddModelError("Sdt", "Số điện thoại chỉ được chứa chữ số");
            }

            if (!string.IsNullOrEmpty(sdt) && sdt.Length != 10)
            {
                ModelState.AddModelError("Sdt", "Số điện thoại dài quá 10 ký tự");
            }
            if (ngaySinh == null)
            {
                ModelState.AddModelError("NgaySinh", "Vui lòng chọn ngày sinh.");
            }
            else
            {
                // Tính tuổi
                var age = DateTime.Now.Year - ngaySinh.Year;
                if (ngaySinh > DateTime.Now.AddYears(-age))
                {
                    age--; // Nếu chưa đến ngày sinh nhật năm nay, giảm tuổi đi 1.
                }

                if (age < 18)
                {
                    ModelState.AddModelError("NgaySinh", "Bạn phải đủ 18 tuổi để đăng ký.");
                }
            }
            if (db.KhachHang.Any(c => c.Sdt == sdt))
            {
                ModelState.AddModelError("Sdt", "Số điện thoại đã được sử dụng");
            }


            if (!ModelState.IsValid)
            {
                ViewBag.Khachhang = newKhachHang;
                ViewBag.ConfirmPassword = confirmPassword;
                return View("DangKy");
            }

            db.KhachHang.Add(newKhachHang);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đăng ký thành công";
            return RedirectToAction("DangNhap");
        }


        public IActionResult DangNhap()
        {
            GetSession();
            return View();
        }

        // Phương thức Set thông tin vào session
        public void SetSession(string userId, string email, string role)
        {
            HttpContext.Session.SetString("D", userId);
            HttpContext.Session.SetString("Email", email);
            HttpContext.Session.SetString("Role", role);
        }

        // Phương thức Get thông tin từ session
        public void GetSession()
        {
            string userId = HttpContext.Session.GetString("D");
            string email = HttpContext.Session.GetString("Email");
            string userRole = HttpContext.Session.GetString("Role");

            // Lưu thông tin vào TempData
            TempData["D"] = userId;
            TempData["Email"] = email;  // Lưu email vào TempData["Email"]
            TempData["Role"] = userRole; // Lưu userRole vào TempData["Role"]
        }


        // Phương thức đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(string email, string password)
        {
            var khachhang = db.KhachHang.FirstOrDefault(c => c.Email == email && c.Password == password);

            if (khachhang != null)
            {
                // Lưu thông tin vào Session
                SetSession(khachhang.D, email, khachhang.Role);

                // Lưu thông tin vào TempData để thông báo thành công
                TempData["D"] = khachhang.D;
                TempData["Role"] = khachhang.Role;
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                if (khachhang.Role == "Nhân viên" || khachhang.Role == "Admin")
                {
                    return RedirectToAction("QLDonHang", "NhanVien");
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không hợp lệ.");
                return View();
            }
        }

        // Phương thức đăng xuất
        public IActionResult DangXuat()
        {
            // Xóa session khi đăng xuất
            HttpContext.Session.Remove("D");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Email");

            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            GetSession();
            return RedirectToAction("Index", "Home");
        }
    }
}
