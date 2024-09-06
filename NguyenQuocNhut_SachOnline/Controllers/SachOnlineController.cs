using NguyenQuocNhut_SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;

namespace NguyenQuocNhut_SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        SachOnline1DataContext data = new SachOnline1DataContext();
        // GET: SachOnline
        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        private List<SACH> LaySachBanNhieu(int count)
        {
            return data.SACHes.OrderByDescending(a => a.SoLuongBan).Take(count).ToList();
        }


        
        public ActionResult Index(int ? page)
        {
            int iSize = 6;
            int iPageNumber = (page ?? 1);
            var kq = from s in data.SACHes  select s;
            return View(kq.ToPagedList(iPageNumber, iSize));
        }
        public ActionResult ChuDePartial()
        {
            var listChuDe = from cd in data.CHUDEs select cd;
            return PartialView(listChuDe);
       
        }
        public ActionResult SachTheoChuDe(int id)
        {
            //var sach = from s in data.SACHes where s.MaCD == id select s;
            //return View(sach);
            var sachList = data.SACHes.Where(s => s.MaCD == id).ToList();

            return View(sachList);
        }
        public ActionResult SachTheoNhaXuatBan(int id)
        {
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach);
        }   
        public ActionResult ChiTietSach(int id )
        {
            //var sach = from s in data.SACHes where s.MaSach == id select s;       
            //return View(sach.Single());
            var sach = data.SACHes.SingleOrDefault(s => s.MaSach == id);

            if (sach == null)
            {
                // Handle the case where the book is not found (e.g., show an error view or redirect to a 404 page)
                return HttpNotFound();
            }

            return View(sach);
        }
        public ActionResult BookByTopic(int? page)
        {
            var maCD = Request.QueryString["MaCD"] ?? "1";
            ViewBag.MaCD = maCD;
            int iSize = 3;
            int iPageNumber = (page ?? 1);
            var kq = from s in data.SACHes where s.MaCD == int.Parse(maCD) select s;
            return View(kq.ToPagedList(iPageNumber, iSize));
        }
            public ActionResult NhaXuatBanPartial()
        {
            var listNhaXuatBan = from nxb in data.NHAXUATBANs select nxb;
            return PartialView(listNhaXuatBan);
        }

        public ActionResult SachBanNhieuPartial()
        {
            //     var listSachMoi = LaySachMoi(6); // Lấy 6 sản phẩm mới nhất
            //   return PartialView(listSachMoi);
            var listSachBanChay = LaySachBanNhieu(6);
            return PartialView(listSachBanChay);
        }
        [ChildActionOnly]
        public ActionResult NavPartial()
        {
            List<MENU> lst = new List<MENU>();
            lst = data.MENUs.Where(m => m.ParentId == null).OrderBy(m => m.OrderNumber).ToList();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                var l = data.MENUs.Where(m => m.ParentId == lst[i].Id);
                a[i] = l.Count();
            }
            ViewBag.lst = a;
            return PartialView(lst);
        }
        [ChildActionOnly]
        public ActionResult LoadChildMenu(int parentId)
        {
            List<MENU> lst = new List<MENU>();
            lst = data.MENUs.Where(m => m.ParentId == parentId).OrderBy(m => m.OrderNumber).ToList();
           
            if (lst.Any())
            {
                // Tiếp tục gọi đệ quy nếu có dữ liệu
                ViewBag.Count = lst.Count();
                int[] a = new int[lst.Count()];
                for (int i = 0; i < lst.Count; i++)
                {
                    var l = data.MENUs.Where(m => m.ParentId == lst[i].Id);
                    a[i] = l.Count();
                }
                ViewBag.lst = a;
                return PartialView("LoadChildMenu", lst);
            }
            else
            {
                // Trả về một kết quả không gọi đệ quy nữa (ví dụ: Partial View trống)
                return PartialView("EmptyView");
            }
        }

        public ActionResult TrangTin(string metatitle)
        {
            var tt = data.TRANGTINs.FirstOrDefault(t => t.MetaTitle == metatitle);
            if (tt == null)
            {
                // Handle the case when the record is not found
                return RedirectToAction("Index"); // Or return a specific view or error page
            }

            return View(tt);
        }


    }
}