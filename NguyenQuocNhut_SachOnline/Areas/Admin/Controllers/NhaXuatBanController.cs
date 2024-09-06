using NguyenQuocNhut_SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class NhaXuatBanController : Controller
    {
        SachOnline1DataContext db = new SachOnline1DataContext();

        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("Login", "Home");
            }

            int iPageNum = page ?? 1;
            int iPageSize = 5;
            var nxbList = db.NHAXUATBANs.OrderBy(n => n.MaNXB).ToList();
            return View(nxbList.ToPagedList(iPageNum, iPageSize));
        }

        [HttpGet]
        public JsonResult DsNXB()
        {
            var nxbList = db.NHAXUATBANs.OrderBy(n => n.MaNXB).Select(n => new {
                n.MaNXB,
                n.TenNXB,
                n.DiaChi,
                n.DienThoai
            }).ToList();
            return Json(nxbList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Detail(int maNXB)
        {
            var nxb = db.NHAXUATBANs.FirstOrDefault(n => n.MaNXB == maNXB);

            if (nxb != null)
            {
                var nxbData = new
                {
                    nxb.MaNXB,
                    nxb.TenNXB,
                    nxb.DiaChi,
                    nxb.DienThoai
                };
                return Json(nxbData, JsonRequestBehavior.AllowGet);
            }

            return Json(null); // Or handle the case when the record is not found
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddNXB(string strTenNXB, string strDiaChi, string strDienThoai)
        {
            try
            {
                NHAXUATBAN nxb = new NHAXUATBAN
                {
                    TenNXB = strTenNXB,
                    DiaChi = strDiaChi,
                    DienThoai = strDienThoai
                };

                db.NHAXUATBANs.InsertOnSubmit(nxb);
                db.SubmitChanges();

                return Json(new { success = true, message = "Thêm nhà xuất bản thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(int maNXB, string strTenNXB, string strDiaChi, string strDienThoai)
        {
            try
            {
                var nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == maNXB);

                if (nxb != null)
                {
                    nxb.TenNXB = strTenNXB;
                    nxb.DiaChi = strDiaChi;
                    nxb.DienThoai = strDienThoai;

                    db.SubmitChanges();
                    return Json(new { success = true, message = "Cập nhật nhà xuất bản thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà xuất bản" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int maNXB)
        {
            try
            {
                var nxbToDelete = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == maNXB);

                if (nxbToDelete != null)
                {
                    db.NHAXUATBANs.DeleteOnSubmit(nxbToDelete);
                    db.SubmitChanges();
                    return Json(new { success = true, message = "Xóa nhà xuất bản thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà xuất bản" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }

}