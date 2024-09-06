using NguyenQuocNhut_SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using XAct.Library.Settings;
using System.Text;
using System.Diagnostics;

namespace NguyenQuocNhut_SachOnline.Controllers
{
    public class UserController : Controller
    {

        SachOnline1DataContext data = new SachOnline1DataContext();

       

        // GET: User
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            //Gan cac gia tri nguoi dung nhap du lieu cho cac bien
            var sHoTen = collection["HoTen"];
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["Matkhau"];
            var sMatKhauNhapLai = collection["MatKhauNL"];
            var sDiaChi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
            if (String.IsNullOrEmpty(sHoTen))
            {
                ViewData["err1"] = "Họ và tên không được rỗng";
            }

            else if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "Phải nhập mật khẩu";
            }

            else if (String.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }

            else if (sMatKhau != sMatKhauNhapLai)
            {
                ViewData["err4"] = "Mật khẩu nhập lại không khớp";
            }

            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được rỗng";
            }

            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err6"] = "Số điện thoại không được rỗng";
            }
            else if (data.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }
            else if (data.KHACHHANGs.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewBag.ThongBao = "Email đã được sử dụng";
            }
            else
            {
                //Gần giá trị cho đối tượng được tạo mới (kh)
                kh.HoTen = sHoTen;
                kh.TaiKhoan = sTenDN;
                kh.MatKhau = sMatKhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.DienThoai = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                kh.MatKhau = GetMD5(sMatKhau);

                return RedirectToAction("DangNhap");
            }
            return this.DangKy();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatkhau = collection["MatKhau"];
            var url = collection["url"];
            if (String.IsNullOrEmpty(url))
            {
                url = "~/SachOnline/Index";
            }
            if (String.IsNullOrEmpty(sTenDN))
            {
                
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }

            else if (String.IsNullOrEmpty(sMatkhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {

                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatkhau || n.MatKhau == GetMD5(sMatkhau));
                if (kh != null)

                {

                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;

                    return RedirectToAction("Index", "SachOnline");

                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
              
                
            }
            return View();

        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "SachOnline");
        }
    }
}