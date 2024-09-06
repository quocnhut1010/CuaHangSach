using NguyenQuocNhut_SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class SachOnline1Controller : Controller
    {
        // GET: Admin/SachOnline1
        SachOnline1DataContext db = new SachOnline1DataContext();
        public ActionResult Index(int? page)
        {
            int pageNum = (page ?? 1);
            int pageSize = 7;
            return View(db.SACHes.ToList().OrderByDescending(n => n.NgayCapNhat).ToPagedList(pageNum, pageSize));
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create(string strTenSach, string moTa, HttpPostedFileBase fileAnh, int soLuong, decimal giaBan, string tenCD, string tenNXB)
        {
            try
            {
                var s = new SACH();
                s.TenSach = strTenSach;
                s.MoTa = moTa;

                // Thêm mã chủ đề
                var cd = db.CHUDEs.SingleOrDefault(c => c.TenChuDe == tenCD);
                if (cd != null)
                {
                    s.MaCD = cd.MaCD;
                }

                // Thêm mã nhà xuất bản
                var nxb = db.NHAXUATBANs.SingleOrDefault(n => n.TenNXB == tenNXB);
                if (nxb != null)
                {
                    s.MaNXB = nxb.MaNXB;
                }
                s.SoLuongBan = soLuong;
                s.GiaBan = giaBan;
                if (fileAnh != null && fileAnh.ContentLength > 0)
                {
                    // Save the file and update the 'AnhBia' property.
                    string fileName = Path.GetFileName(fileAnh.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    fileAnh.SaveAs(filePath);
                    s.AnhBia = fileName;
                }
                db.SACHes.InsertOnSubmit(s);
                db.SubmitChanges();
                return Json(new { code = 200, msg = "Thêm sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Thêm sách thất bại" + e.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Edit(int maSach, string strTenSach, string moTa, int soLuong, decimal giaBan, string tenCD, string tenNXB, HttpPostedFileBase fileAnh)
        {
            try
            {
                var s = db.SACHes.SingleOrDefault(model => model.MaSach == maSach);
                s.TenSach = strTenSach;
                s.MoTa = moTa;
                s.SoLuongBan = soLuong;
                s.GiaBan = giaBan;

                // Sửa thông tin chủ đề
                var cd = db.CHUDEs.SingleOrDefault(c => c.TenChuDe == tenCD);
                if (cd != null)
                {
                    s.MaCD = cd.MaCD;
                }

                // Sửa thông tin nhà xuất bản
                var nxb = db.NHAXUATBANs.SingleOrDefault(n => n.TenNXB == tenNXB);
                if (nxb != null)
                {
                    s.MaNXB = nxb.MaNXB;
                }

                if (fileAnh != null && fileAnh.ContentLength > 0)
                {
                    // Save the file and update the 'AnhBia' property.
                    string fileName = Path.GetFileName(fileAnh.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    fileAnh.SaveAs(filePath);
                    s.AnhBia = fileName;
                }
                db.SubmitChanges();
                return Json(new { code = 200, msg = "Sửa sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sửa sách thất bại" + e.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int maSach)
        {
            try
            {
                var s = db.SACHes.SingleOrDefault(model => model.MaSach == maSach);
                db.SACHes.DeleteOnSubmit(s);
                db.SubmitChanges();
                return Json(new { code = 200, msg = "Xoá sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Xoá sách thất bại" + e.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult DsSach()
        {
            try
            {

                var dsSach = (from s in db.SACHes
                            select new
                            {
                                MaSach = s.MaSach,
                                MoTa = s.MoTa,
                                TenSach = s.TenSach,
                                Anh = s.AnhBia,
                                NgayCapNhat = s.NgayCapNhat,
                                SoLuong = s.SoLuongBan,
                                GiaBan = s.GiaBan,
                                TenCD = from cd in db.CHUDEs where cd.MaCD == s.MaCD select cd.TenChuDe,
                                TenNXB = from nxb in db.NHAXUATBANs where nxb.MaNXB == s.MaNXB select nxb.TenNXB
                            }).ToList();
                return Json(new { code = 200, dsSach = dsSach, msg = "Lấy danh sách sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lấy danh sách sách thất bại" + e.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Details(int maSach)
        {
            try
            {
                var sach = (from s in db.SACHes
                            where (s.MaSach == maSach)
                            select new
                            {
                                MaSach = s.MaSach,
                                TenSach = s.TenSach,
                                MoTa = s.MoTa,
                                SoLuong = s.SoLuongBan,
                                GiaBan = s.GiaBan,
                                TenCD = from cd in db.CHUDEs where cd.MaCD == s.MaCD select cd.TenChuDe,
                                TenNXB = from nxb in db.NHAXUATBANs where nxb.MaNXB == s.MaNXB select nxb.TenNXB
                            }).SingleOrDefault();

                return Json(new { code = 200, sach = sach, msg = "Lấy thông tin sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lấy thông tin sách thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

      
    }
}