using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    public class TuQuanLyDonHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: TuQuanLyDonHang
        public ActionResult Index()
        {
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            int MaKH = 0;
            var khachang = db.KhachHangs.Where(n => n.MaThanhVien == tv.MaThanhVien);
            //lấy ds đơn hàng chưa duyệt
            List<DonDatHang> abc = new List<DonDatHang>();
            try
            {
                foreach (var temp in khachang)
                {
                    MaKH = temp.MaKH;
                    DonDatHang ddhUpdate = db.DonDatHangs.Single(n => n.MaKH == MaKH);

                    abc.Add(ddhUpdate);
                }
                var lst = db.DonDatHangs.Where(n => n.MaKH == MaKH).OrderBy(n => n.NgayDat);
            }
            catch
            {

            }

            return View(abc);
        }

        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            ViewBag.thongbao = "";
            //ktra id hợp lệ
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            DonDatHang model = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);
            //ktra đơn hàng có tồn tại ko
            if (model == null)
            {
                return HttpNotFound();
            }
            //hiển thị ds chitietdonhang 
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            ViewBag.TenKH = model.KhachHang.TenKH;
            return View(model);
        }

        [HttpPost]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            ViewBag.thongbao = "";

            DonDatHang ddhUpdate = db.DonDatHangs.Single(n => n.MaDDH == ddh.MaDDH);    //lấy dl của đơn hàng trên
            
            TimeSpan t = DateTime.Now - (DateTime)ddhUpdate.NgayDat;
            if (t.Days <= 3)
            {
                ddhUpdate.DaHuy = true;
                ViewBag.thongbao = "Hủy đơn hàng thành công";
            }
            else
            {
                ViewBag.thongbao = "Đơn hàng của bạn đã đặt quá 3 ngày nên không thể hủy";
            }
            db.SaveChanges();

            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH);
            ViewBag.ListChiTietDH = lstChiTietDH;

            return View(ddhUpdate);
        }

        //Giải phóng dung lượng biến db, để ở cuối controller
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}