using NguyenQuocNhut_SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        SachOnline1DataContext db = new SachOnline1DataContext();
        // GET: Admin/KhachHang
        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("Login", "Home");
            }

            int iPageNum = page ?? 1;
            int iPageSize = 5;
            var khList = db.KHACHHANGs.OrderBy(n => n.MaKH).ToList();
            return View(khList.ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public JsonResult DsKH()
        {
            var khachHangList = db.KHACHHANGs.OrderBy(kh => kh.MaKH).Select(kh => new
            {
                kh.MaKH,
                kh.HoTen,
                kh.TaiKhoan,
                kh.MatKhau,
                kh.Email,
                kh.DiaChi,
                kh.DienThoai,
                kh.NgaySinh // Fetch the date without formatting
            }).ToList();

            var formattedData = khachHangList.Select(kh => new
            {
                kh.MaKH,
                kh.HoTen,
                kh.TaiKhoan,
                kh.MatKhau,
                kh.Email,
                kh.DiaChi,
                kh.DienThoai,
                NgaySinh = Convert.ToDateTime(kh.NgaySinh).ToString("yyyy-MM-dd") // Format the date after fetching the data
            }).ToList();

            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Detail(int maKH)
        {
            var kh = db.KHACHHANGs.FirstOrDefault(n => n.MaKH == maKH);

            if (kh != null)
            {
                var khData = new
                {
                    kh.MaKH,
                    kh.HoTen,
                    kh.TaiKhoan,
                    kh.MatKhau,
                    kh.Email,
                    kh.DiaChi,
                    kh.DienThoai,
                    NgaySinh = Convert.ToDateTime(kh.NgaySinh).ToString("yyyy-MM-dd")

                };
                return Json(khData, JsonRequestBehavior.AllowGet);
            }

            return Json(null); // Or handle the case when the record is not found
        }
        // ... (Previous code remains unchanged)

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddKH(string hoTen, string taiKhoan, string matKhau, string email, string diaChi, string dienThoai, DateTime ngaySinh)
        {
            try
            {
                var newKhachHang = new KHACHHANG
                {
                    HoTen = hoTen,
                    TaiKhoan = taiKhoan,
                    MatKhau = matKhau,
                    Email = email,
                    DiaChi = diaChi,
                    DienThoai = dienThoai,
                    NgaySinh = ngaySinh
                };

                db.KHACHHANGs.InsertOnSubmit(newKhachHang);
                db.SubmitChanges();

                return Json(new { success = true, message = "Customer added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding customer: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(int maKH, string hoTen, string taiKhoan, string matKhau, string email, string diaChi, string dienThoai, DateTime ngaySinh)
        {
            try
            {
                var kh = db.KHACHHANGs.FirstOrDefault(n => n.MaKH == maKH);

                if (kh != null)
                {
                    kh.HoTen = hoTen;
                    kh.TaiKhoan = taiKhoan;
                    kh.MatKhau = matKhau;
                    kh.Email = email;
                    kh.DiaChi = diaChi;
                    kh.DienThoai = dienThoai;
                    kh.NgaySinh = ngaySinh;

                    db.SubmitChanges();

                    return Json(new { success = true, message = "Customer updated successfully" });
                }

                return Json(new { success = false, message = "Customer not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating customer: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int maKH)
        {
            try
            {
                var kh = db.KHACHHANGs.FirstOrDefault(n => n.MaKH == maKH);

                if (kh != null)
                {
                    db.KHACHHANGs.DeleteOnSubmit(kh);
                    db.SubmitChanges();

                    return Json(new { success = true, message = "Customer deleted successfully" });
                }

                return Json(new { success = false, message = "Customer not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting customer: " + ex.Message });
            }
        }
    }
}