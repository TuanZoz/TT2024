using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Versioning;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [ServiceFilter(typeof(SessionActionFilter))]
    public class NhanVienController : Controller
    {
        private readonly ILogger<NhanVienController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly QuanLyBanHangOnline4Context db;
        public NhanVienController(ILogger<NhanVienController> logger, QuanLyBanHangOnline4Context context, IWebHostEnvironment environment)
        {
            _logger = logger;
            db = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> QLDonHang()
        {
       
            var hoaDons = await db.HoaDon
                .Include(hd => hd.HoaDonChiTiets)
                .ToListAsync();

            // Chuyển hóa đơn sang dạng JSON
            ViewBag.HoaDons = JsonConvert.SerializeObject(hoaDons, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // Truy vấn danh sách khách hàng
            var khachhang = await db.KhachHang.Select(kh => new KhachHang
            {
                D = kh.D,
                HoTen = kh.HoTen,
                DiaChi = kh.DiaChi,
                Sdt = kh.Sdt,
            }).ToListAsync();

            // Chuyển khách hàng sang dạng JSON
            ViewBag.Khachhang = JsonConvert.SerializeObject(khachhang);

            // Truy vấn sản phẩm
            var hangHoas = db.SanPham.AsQueryable();
            var result = hangHoas.Select(p => new SanPham
            {
                Id = p.Id,
                TenSp = p.TenSp,
                Idloai = p.Idloai,
                Soluong = p.Soluong,
                Gia = p.Gia,
                MoTa = p.MoTa,
                HinhAnh = p.HinhAnh,
            }).ToList();

            // Chuyển sản phẩm sang dạng JSON
            ViewBag.HangHoas = JsonConvert.SerializeObject(result);

            return View("~/Views/Home/QLDonHang.cshtml");
        }

        public IActionResult QLKhachHang()
        {
            var KhachHangs = db.KhachHang.ToList();

            ViewBag.KhachHangs = JsonConvert.SerializeObject(KhachHangs, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return View("~/Views/Home/QLKhachHang.cshtml");
        }

        public async Task<IActionResult> AddKhachHang(string hoTen, string sdt, string password, string role, string diaChi, string gioiTinh, DateTime ngaySinh, string email)
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

            // Tạo khách hàng mới
            var khachHang = new KhachHang
            {
                D = newId.ToString(), 
                HoTen = hoTen,
                Sdt = sdt,
                Password = password,
                Role = role,
                DiaChi = diaChi,
                GioiTinh = gioiTinh,
                NgaySinh = ngaySinh,
                Email = email
            };

            // Thêm khách hàng vào cơ sở dữ liệu
            db.KhachHang.Add(khachHang);
            await db.SaveChangesAsync();

            // Quay lại trang quản lý khách hàng
            return RedirectToAction("QLKhachHang");
        }
        public async Task<IActionResult> EditKhachHang(string Id, string hoTen, string sdt, string? password, string role, string diaChi, string gioiTinh, DateTime ngaySinh, string email)
        {
            // Lấy khách hàng theo ID
            var khachHang = db.KhachHang.FirstOrDefault(kh => kh.D == Id);

            // Kiểm tra nếu không tìm thấy khách hàng
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Khách hàng không tồn tại!";
                return RedirectToAction("QLKhachHang");
            }

            // Cập nhật thông tin khách hàng
            khachHang.HoTen = hoTen;
            khachHang.Sdt = sdt;
            khachHang.Role = role;
            khachHang.DiaChi = diaChi;
            khachHang.GioiTinh = gioiTinh;
            khachHang.NgaySinh = ngaySinh;
            khachHang.Email = email;

            // Cập nhật khách hàng trong cơ sở dữ liệu
            db.KhachHang.Update(khachHang);
            await db.SaveChangesAsync();

            // Thông báo thành công và quay lại trang quản lý khách hàng
            TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
            return RedirectToAction("QLKhachHang");
        }




        public IActionResult QLKho()
        {
            var sp = db.SanPham.Select(sp => new SanPham
            {
                Id = sp.Id,
                TenSp = sp.TenSp,
            }).ToList(); 
            ViewBag.SanPham = JsonConvert.SerializeObject(sp, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var nhapKhoList = db.NhapKho
                                 .Include(nk => nk.IdspNavigation)
                                 .ToList();
            ViewBag.NhapKhoJson = JsonConvert.SerializeObject(nhapKhoList, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var loai = db.LoaiSanPham.ToList();
            ViewBag.Loais = loai;

            return View("~/Views/Home/QLKho.cshtml");
        }
        public async Task<IActionResult> AddNhapKho(string idsp, int soluong, double giaNhap, DateTime ngayNhap )
        {
            var nhapKhos = await db.NhapKho.ToListAsync();
            var validNhapKhos = nhapKhos.Where(nk => int.TryParse(nk.Id, out _)).ToList();
            var maxId = validNhapKhos.Any() ? validNhapKhos.Max(nk => int.Parse(nk.Id)) : 0;

            // Tạo ID mới
            var newId = maxId + 1;

            // Kiểm tra trùng lặp
            var exists = await db.NhapKho.AnyAsync(nk => nk.Id == newId.ToString());
            while (exists)
            {
                newId++;
                exists = await db.NhapKho.AnyAsync(nk => nk.Id == newId.ToString());
            }
            var sp = await db.SanPham.FirstOrDefaultAsync(sp => sp.Id == idsp);
            if (sp == null)
            {
                return NotFound("Sản phẩm không tồn tại!");
            }

            // Tính toán thành tiền
            var thanhTien = soluong * giaNhap;

            // Tạo phiếu nhập kho mới
            var nhapKho = new NhapKho
            {
                Id = newId.ToString(),
                Idsp = idsp,
                Soluong = soluong,
                GiaNhap = giaNhap,
                ThanhTien = thanhTien,
                NgayNhap = ngayNhap
            };


            sp.Soluong += soluong;
            sp.Gia += giaNhap;

            db.NhapKho.Add(nhapKho);

            await db.SaveChangesAsync();

          
            return RedirectToAction("QLKho");
        }


        public IActionResult QLSanPham()
        {
            var hangHoas = db.SanPham.AsQueryable();

            var result = hangHoas.Select(p => new SanPham
            {
                Id = p.Id,
                TenSp = p.TenSp,
                Idloai = p.Idloai,
                Soluong = p.Soluong,
                Gia = p.Gia,
                MoTa = p.MoTa,
                HinhAnh = p.HinhAnh,
            }).ToList();
            ViewBag.HangHoas = JsonConvert.SerializeObject(result);

            var loai = db.LoaiSanPham.ToList();
            ViewBag.Loais = loai;


            return View("~/Views/Home/QLSanPham.cshtml");
        }
        public async Task<IActionResult> AddProduct([FromForm] string tenSp, [FromForm] string idloai,
                                               [FromForm] int soluong, [FromForm] double gia,
                                               [FromForm] string moTa, [FromForm] IFormFile hinhAnh, string? kho)
        {
            string fileName = null;

            var sanPhams = await db.SanPham.ToListAsync();
            var validSanPhams = sanPhams.Where(sp => int.TryParse(sp.Id, out _)).ToList();
            var maxId = validSanPhams.Any() ? validSanPhams.Max(sp => int.Parse(sp.Id)) : 0;
            var newId = maxId + 1;

            var exists = await db.SanPham.AnyAsync(sp => sp.Id == newId.ToString());
            while (exists)
            {
                newId++;
                exists = await db.SanPham.AnyAsync(sp => sp.Id == newId.ToString());
            }

            if (hinhAnh != null)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "images");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                fileName = Path.GetRandomFileName() + Path.GetExtension(hinhAnh.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }
            }

            // Tạo đối tượng sản phẩm
            var sanPham = new SanPham
            {
                Id = newId.ToString(),
                TenSp = tenSp,
                Idloai = idloai,
                Soluong = soluong,
                Gia = gia,
                MoTa = moTa,
                HinhAnh = fileName
            };

            // Lưu sản phẩm vào cơ sở dữ liệu
            db.SanPham.Add(sanPham);
            await db.SaveChangesAsync();

            if (kho != null)
            {
                return RedirectToAction("QLKho");
            }
            // Redirect đến trang quản lý sản phẩm
            return RedirectToAction("QLSanPham");
        }
        public async Task<IActionResult> EditProduct(
             [FromForm] string idSp,
             [FromForm] string tenSp,
             [FromForm] string idloai,
             [FromForm] int soluong,
             [FromForm] double gia,
             [FromForm] string moTa,
             [FromForm] IFormFile? hinhAnh)
        {

            var sanPham = await db.SanPham.FirstOrDefaultAsync(sp => sp.Id == idSp);
            if (sanPham == null)
            {
                return RedirectToAction("QLSanPham");  
            }

 
            string fileName = sanPham.HinhAnh;

            if (hinhAnh != null)
            {

                var uploads = Path.Combine(_environment.WebRootPath, "images");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }


                var oldFilePath = Path.Combine(uploads, sanPham.HinhAnh);
                if (!string.IsNullOrEmpty(sanPham.HinhAnh) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                fileName = Path.GetRandomFileName() + Path.GetExtension(hinhAnh.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);  
                }
            }

  
            sanPham.TenSp = tenSp;
            sanPham.Idloai = idloai;
            sanPham.Soluong = soluong;
            sanPham.Gia = gia;
            sanPham.MoTa = moTa;
            sanPham.HinhAnh = fileName;  

            db.SanPham.Update(sanPham);
            await db.SaveChangesAsync();

         
            return RedirectToAction("QLSanPham");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Remove("D");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Email");
            TempData["SuccessMessage"] = "Đăng xuất thành công";

            return RedirectToAction("Index", "Home");
        }
        
    }
}
