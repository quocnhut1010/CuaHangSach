using NguyenQuocNhut_SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        // GET: Admin/DonHang
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
            var dhList = db.DONDATHANGs.OrderBy(n => n.MaDonHang).ToList();
            return View(dhList.ToPagedList(iPageNum, iPageSize));
        }
    }
}