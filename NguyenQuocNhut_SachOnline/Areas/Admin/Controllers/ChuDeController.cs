using NguyenQuocNhut_SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList.Mvc;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class ChuDeController : Controller
    {
        // GET: Admin/ChuDe1
        SachOnline1DataContext db = new SachOnline1DataContext();
        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                // In giá trị Session để kiểm tra
                return RedirectToAction("Login", "Home");
            }
            int iPageNum = page ?? 1;
            int iPageSize = 10;
            var chudeList = db.CHUDEs.OrderBy(n => n.MaCD).ToList();
            return View(chudeList.ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public JsonResult DsChuDe()
        {
            try
            {
                var dsCD = (from cd in db.CHUDEs
                            select new
                            {
                                MaCD = cd.MaCD,
                                TenCD = cd.TenChuDe
                            }).ToList();
                return Json(new { code = 200, dsCD = dsCD, msg = "Lấy danh sách chủ đề sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách chủ đề sách thất bại" + ex.Message },
                 JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult Detail(int maCD)
        {
            try
            {
                var cd = (from s in db.CHUDEs
                          where (s.MaCD == maCD)
                          select new
                          {
                              MaCD = s.MaCD,
                              TenChuDe = s.TenChuDe,
                          }).SingleOrDefault(c => c.MaCD == maCD);
                return Json(new { code = 200, cd = cd, msg = "Lấy thông tin chủ đề thành công." },
                    JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin chủ đề thất bại." + ex.Message }, JsonRequestBehavior.AllowGet);

            }


        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddChuDe(int maCD, string strTenCD)
        {
            try
            {
                var cd = new CHUDE();
                cd.TenChuDe = strTenCD;
                db.CHUDEs.InsertOnSubmit(cd);
                db.SubmitChanges();
                return Json(new { code = 200, msg = "Thêm chủ đề thành công." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thêm chủ đề thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]

        public JsonResult Update(int maCD, string strTenCD)
        {
            try
            {
                var cd = db.CHUDEs.SingleOrDefault(c => c.MaCD == maCD);
                cd.TenChuDe = strTenCD;
                db.SubmitChanges();
                return Json(new { code = 200, msg = "Sửa chủ đề thành công." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "sửa chủ đề thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]

        public JsonResult Delete(int maCD, string strTenCD)
        {
            try
            {
                var cd = db.CHUDEs.SingleOrDefault(c => c.MaCD == maCD);
                db.CHUDEs.DeleteOnSubmit(cd);
                db.SubmitChanges();
                return Json(new { code = 200, msg = "Xoá chủ đề thành công." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xoá chủ đề thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }


    }
}