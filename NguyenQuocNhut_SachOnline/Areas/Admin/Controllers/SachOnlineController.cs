using NguyenQuocNhut_SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using XAct.Library.Settings;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class SachOnlineController : Controller
    {
        // GET: Admin/SachOnline
        private SachOnline1DataContext db = new SachOnline1DataContext();
        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                // In giá trị Session để kiểm tra
                return RedirectToAction("Login", "Home");
            }

            
            int iPageNum = page ?? 1;
            int iPageSize = 10;
            return View(db.SACHes.ToList().OrderBy(n => n.MaSach).ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SACH sach, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            //Đưa dữ liệu vào DropDown
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            if (fFileUpload == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.ThongBao = "Hãy chọn ảnh bìa.";
                //Lưu thông tin để khi load lại trang do yếu cầu chọn ảnh bìa sẽ hiển thị các thông tin này lên trang
                ViewBag.TenSach = f["sTenSach"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
                ViewBag.GiaBan = decimal.Parse(f["mGiaBan"]);
                ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", int.Parse(f["MaCD"]));
                ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", int.Parse(f["MaNXB"]));
                return View();
            }


            else
            {
                if (ModelState.IsValid)
                {
                    // Lấy tên file (Khai báo thư viện: System.IO)
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    //Lấy đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    //Kiểm tra ảnh bìa đã tồn tại chưa để lưu lên thư mục 
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    //Lưu Sạch vào CSDL
                    sach.TenSach = f["sTenSach"];
                    sach.MoTa = f["sMoTa"];
                    sach.AnhBia = sFileName;
                    sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                    sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                    sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                    sach.MaCD = int.Parse(f["MaCD"]);

                    sach.MaNXB = int.Parse(f["MaNXB"]);
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                    //trở lại trang Quản lý sách
                    return RedirectToAction("Index");
                }
                return View();

            }
        }


public ActionResult Details(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(int id, FormCollection f)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaSach == id);
            if (ctdh.Count() > 0)
            {
                ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đơn hàng <br>" +
                    "Nếu muốn xoá  thì phải xoá hết mã sách này trong bảng Chi tiết đặt hàng";
                return View(sach);
            }

            var vietsach = db.VIETSACHes.Where(vs => vs.MaSach == id).ToList();
            if (vietsach != null)
            {
                db.VIETSACHes.DeleteAllOnSubmit(vietsach);
                db.SubmitChanges();
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe",sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == int.Parse(f["iMaSach"])); 
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe),"MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            if (ModelState.IsValid)
            {
                if (fFileUpload != null) // Kiểm tra để xác nhận cho thay đổi ảnh bìa
                {
                    //Lấy tên file (Khai báo thư viện: System.IO)
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    //Lấy đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName); // Kiếm tra file đã tồn tại chưa
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);

                    }
                    sach.AnhBia = sFileName;
                }
                sach.TenSach = f["sTenSach"];
                sach.MoTa = f["sMoTa"];
                sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                sach.MaCD = int.Parse(f["MaCD"]);
                sach.MaNXB = int.Parse(f["MaNXB"]);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(sach);
        }
    }
}