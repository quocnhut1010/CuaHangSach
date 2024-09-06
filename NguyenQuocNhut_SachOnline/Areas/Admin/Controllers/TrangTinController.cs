using NguyenQuocNhut_SachOnline.Models;
using SachOnline.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenQuocNhut_SachOnline.Areas.Admin.Controllers
{
    public class TrangTinController : Controller
    {
        SachOnline1DataContext db = new SachOnline1DataContext();
        // GET: Admin/TrangTin
        public ActionResult Index()
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                // In giá trị Session để kiểm tra
                return RedirectToAction("Login", "Home");
            }
            return View(db.TRANGTINs.ToList());
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TRANGTIN tt)
        {
            if(ModelState.IsValid)
            {
                tt.MetaTitle = tt.TenTrang.RemoveDiacritics().Replace(" ", "-");
                tt.NgayTao = DateTime.Now;
                db.TRANGTINs.InsertOnSubmit(tt);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var tt = db.TRANGTINs.Where(t => t.MaTT == id);
            return View(tt.SingleOrDefault());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            if (ModelState.IsValid)
            {
                var tt = db.TRANGTINs.Where(t => t.MaTT == int.Parse(f["MaTT"])).SingleOrDefault();
                tt.TenTrang = f["TenTrang"];
                tt.NoiDung = f["NoiDung"];
                tt.NgayTao = Convert.ToDateTime(f["NgayTao"]);
                tt.MetaTitle = f["TenTrang"].RemoveDiacritics().Replace(" ", "-");
               
                
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
       
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var tt = from t in db.TRANGTINs where t.MaTT == id select t;
            return View(tt.SingleOrDefault());
        }
        [HttpPost, ActionName("Delete")] 
        public ActionResult Delete(int id, FormCollection f)
        {
        
                var tt = (from t in db.TRANGTINs where t.MaTT == id select t).SingleOrDefault();
                db.TRANGTINs.DeleteOnSubmit(tt);
                db.SubmitChanges();
                return RedirectToAction("Index");

          }
    }
}