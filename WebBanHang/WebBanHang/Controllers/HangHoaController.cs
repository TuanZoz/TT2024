using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{

    public class HangHoaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuanLyBanHangOnline4Context db;
        public  HangHoaController(ILogger<HomeController> logger, QuanLyBanHangOnline4Context context)
        {
            _logger = logger;
            db = context;
        }
        public void GetSession()
        {
            string userId = HttpContext.Session.GetString("D");
            string email = HttpContext.Session.GetString("Email");
            string userRole = HttpContext.Session.GetString("Role");

            // L?u thông tin vào TempData
            TempData["D"] = userId;
            TempData["Email"] = email;  // L?u email vào TempData["Email"]
            TempData["Role"] = userRole; // L?u userRole vào TempData["Role"]
        }
        public IActionResult Details(string id)
        {
            GetSession();
            var data = db.SanPham.Include(p => p.IdloaiNavigation)
                                 .SingleOrDefault(p => p.Id == id);

            if (data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return RedirectToAction("Index", "Home");
            }

            var result = new SanPhamViewModel
            {
                Id = data.Id,
                Tensp = data.TenSp,
                Gia = data.Gia ?? 0,
                Hinh = data.HinhAnh ?? string.Empty,
                ChiTiet = data.MoTa ?? string.Empty,
                MoTaNgan = data.MoTa ?? string.Empty,
                Idloai = data.IdloaiNavigation.TenLoai,
                SoLuongTon = 10, // check sau
                DiemDanhGia = 5, // check sau
            };

            return View("~/Views/HangHoa/Details.cshtml", result);
        }
        public async Task<IActionResult> SubmitOrder(string Idkh, double ThanhTien)
        {
            // Lấy các giá trị sản phẩm và số lượng từ form
            var productIds = Request.Form["productIds"].ToString().Split(',').Select(int.Parse).ToList();
            var productQuantities = Request.Form["productQuantities"].ToString().Split(',').Select(int.Parse).ToList();

            // Kiểm tra dữ liệu
            if (productIds.Count != productQuantities.Count)
            {
                return BadRequest("Số lượng sản phẩm và số lượng không khớp.");
            }

            // Tiếp tục xử lý đơn hàng...
            var kh = await db.KhachHang.FirstOrDefaultAsync(kh => kh.D == Idkh);

            if (kh == null)
            {
                return BadRequest("Khách hàng không tồn tại.");
            }

            // Thêm hóa đơn
            var isHoaDonAdded = await AddHoaDonAsync(Idkh, ThanhTien, "Đã Xác Nhận", null, kh.DiaChi);

            if (!isHoaDonAdded)
            {
                return BadRequest("Không thể thêm hóa đơn.");
            }

            // Lấy Id của hóa đơn vừa thêm
            var hoaDon = await db.HoaDon
                .Where(hd => hd.Idkh == Idkh && hd.ThanhTien == ThanhTien && hd.TrangThai == "Đã Xác Nhận")
                .OrderByDescending(hd => hd.NgayDat)
                .FirstOrDefaultAsync();

            if (hoaDon == null)
            {
                return BadRequest("Không tìm thấy hóa đơn.");
            }

            // Thêm chi tiết hóa đơn cho từng sản phẩm
            for (int i = 0; i < productIds.Count; i++)
            {
                var productId = productIds[i];
                var quantity = productQuantities[i];

                // Lấy sản phẩm từ cơ sở dữ liệu
                var product = await db.SanPham.FirstOrDefaultAsync(pd => pd.Id == productId.ToString());

                if (product == null)
                {
                    return BadRequest($"Sản phẩm với ID {productId} không tồn tại.");
                }

                // Lấy giá bán của sản phẩm
                var giaBan = product.Gia ?? 0;

                // Thêm chi tiết hóa đơn
                var isHoaDonChiTietAdded = await AddHoaDonChiTietAsync(hoaDon.Id, productId.ToString(), quantity, giaBan);

                if (!isHoaDonChiTietAdded)
                {
                    return BadRequest("Không thể thêm chi tiết hóa đơn.");
                }
            }

            return RedirectToAction("LichsuDonMua", "Home");
        }


        public async Task<bool> AddHoaDonChiTietAsync(string idhd, string idsp, int soLuong, double giaBan)
        {
            try
            {
                // Kiểm tra giá trị đầu vào
                if (string.IsNullOrEmpty(idhd) || string.IsNullOrEmpty(idsp) || soLuong <= 0 || giaBan <= 0)
                {
                    Console.WriteLine("Lỗi: Dữ liệu đầu vào không hợp lệ.");
                    return false;
                }

                // Tìm max Id trực tiếp trên cơ sở dữ liệu
                var HoaDons = await db.HoaDonChiTiet.ToListAsync();
                var validHoaDons = HoaDons.Where(kh => int.TryParse(kh.Id, out _)).ToList();
                var maxId = validHoaDons.Any() ? validHoaDons.Max(kh => int.Parse(kh.Id)) : 0;

                var newId = maxId + 1;

                // Kiểm tra Id có bị trùng không
                var exists = await db.HoaDonChiTiet.AnyAsync(kh => kh.Id == newId.ToString());
                while (exists)
                {
                    newId++;
                    exists = await db.HoaDonChiTiet.AnyAsync(kh => kh.Id == newId.ToString());
                }

                // Tạo chi tiết hóa đơn mới và đảm bảo rằng Id được gán trước khi thêm vào cơ sở dữ liệu
                var hoaDonChiTiet = new HoaDonChiTiet
                {
                    Id = newId.ToString(),  // Đảm bảo rằng Id được gán giá trị trước khi thêm vào cơ sở dữ liệu
                    Idhd = idhd,
                    Idsp = idsp,
                    Soluong = soLuong,
                    GiaBan = giaBan
                };

                db.HoaDonChiTiet.Add(hoaDonChiTiet);
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi
                Console.WriteLine($"Lỗi khi thêm chi tiết hóa đơn: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> AddHoaDonAsync(string idkh, double thanhTien, string trangThai, string? ghiChu, string? diaChi)
        {
            try
            {
                // Tìm max Id trực tiếp trên cơ sở dữ liệu
                var HoaDons = await db.HoaDon.ToListAsync();
                var validHoaDons = HoaDons.Where(kh => int.TryParse(kh.Id, out _)).ToList();
                var maxId = validHoaDons.Any() ? validHoaDons.Max(kh => int.Parse(kh.Id)) : 0;

                var newId = maxId + 1;

                // Kiểm tra Id có bị trùng không
                var exists = await db.HoaDon.AnyAsync(kh => kh.Id == newId.ToString());
                while (exists)
                {
                    newId++;
                    exists = await db.HoaDon.AnyAsync(kh => kh.Id == newId.ToString());
                }

                // Tạo hóa đơn mới
                var hoaDon = new HoaDon
                {
                    Id = newId.ToString(),
                    Idkh = idkh,
                    ThanhTien = thanhTien,
                    TrangThai = trangThai,
                    GhiChu = ghiChu,
                    DiaChi = diaChi,
                    NgayDat = DateTime.Now,
                    NgayGiao = null
                };

                // Lưu hóa đơn vào cơ sở dữ liệu
                db.HoaDon.Add(hoaDon);
                await db.SaveChangesAsync();

                Console.WriteLine($"Thêm hóa đơn thành công. Id: {hoaDon.Id}");
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Lỗi khi thêm hóa đơn: {ex.Message}");
                return false;
            }
        }


        






    }
}

