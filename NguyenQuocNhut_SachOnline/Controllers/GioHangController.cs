using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI.MobileControls;
using NguyenQuocNhut_SachOnline.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Configuration;

namespace NguyenQuocNhut_SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        SachOnline1DataContext db = new SachOnline1DataContext();
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;

            }
            return lstGioHang;
        }
        public ActionResult ThemGiohang(int ms, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
            }

            GioHang sp = lstGioHang.Find(n => n.iMaSach == ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);
                Session["GioHang"] = lstGioHang;
                TempData["ThongBao"] = "Thêm sản phẩm vào giỏ hàng thành công!";
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //tong tien
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"].ToString();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return PartialView();
        }
        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaSPKhoiGioHang(int iMaSach)
        {
            //Lấy giỏ hàng

            List<GioHang> lstGioHang = LayGioHang();

            //Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);

            //Xóa sản phẩm khỏi giỏ hàng
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSach);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnline");
                }
            }
            
            //Cập nhật lại giỏ hàng
            return RedirectToAction("GioHang");
        }
        //Cập nhật giỏ hàng
        public ActionResult CapNhatGioHang(int iMaSach, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);

            // Nếu tồn tại thì cho sửa số lượng
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
                return RedirectToAction("GioHang");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            // Kiểm tra đăng nhập chưa
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                // In giá trị Session để kiểm tra
                return RedirectToAction("DangNhap", "GioHang");
            }

            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "SachOnline");
            }

            // Lấy hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
           
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            // Thêm đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List < GioHang > lstGioHang = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangGiaoHang = 1;
            ddh.DaThanhToan = false;
            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
    {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMaSach;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                db.CHITIETDATHANGs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            // Lấy email của khách hàng
            string toEmail = kh.Email;
            SendOrderConfirmationEmail(toEmail, ddh);
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
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

                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatkhau );
                if (kh != null)

                {

                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return RedirectToAction("DatHang", "GioHang");
        }
        private void SendOrderConfirmationEmail(string toEmail, DONDATHANG ddh)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("gacquy62@gmail.com");
                    mail.To.Add(toEmail);
                    mail.Subject = "Xác nhận đặt hàng thành công";

                    StringBuilder body = new StringBuilder();
                    body.AppendLine($"Đơn hàng của bạn (Mã đơn hàng: {ddh.MaDonHang}) đã được đặt thành công. Cảm ơn bạn đã mua hàng.\n");

                    foreach (var chiTiet in ddh.CHITIETDATHANGs)
                    {
                        body.AppendLine($"Sản phẩm: {chiTiet.SACH.TenSach}");
                        body.AppendLine($"Số lượng: {chiTiet.SoLuong}");
                        body.AppendLine($"Đơn giá: {chiTiet.DonGia:C}");
                        body.AppendLine("------------------------");
                    }

                    mail.Body = body.ToString();

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                    {
                        // Replace 587 with the appropriate port if not using SSL
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("gacquy62@gmail.com", "nhut1801");

                        try
                        {
                            smtp.Send(mail);
                            Console.WriteLine($"Email sent successfully to: {toEmail}");
                        }
                        catch (Exception ex)
                        {
                            // Log the exception
                            Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }


    }
}