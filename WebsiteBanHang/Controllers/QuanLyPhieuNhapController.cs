﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "QuanLy,QuanTriWeb")]
    public class QuanLyPhieuNhapController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: QuanLyPhieuNhap
        [HttpGet]
        public ActionResult NhapHang()
        {
            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;
            ViewBag.NgayNhap = DateTime.Today;

            return View();
        }
        [HttpPost]
        public ActionResult NhapHang(PhieuNhap model, IEnumerable<ChiTietPhieuNhap> lstModel)
        {
            if (lstModel != null)
            {
                foreach (var item in lstModel)
                {
                    if (item.SoLuongNhap == null || item.DonGiaNhap == null || item.SoLuongNhap < 1 || item.DonGiaNhap < 100000)
                    {
                        @ViewBag.KetQuaNhap = "Nhập hàng không thành công";
                        return View();
                    }
                }
                @ViewBag.KetQuaNhap = "Nhập hàng thành công";
                ViewBag.MaNCC = db.NhaCungCaps;
                ViewBag.ListSanPham = db.SanPhams;
                ViewBag.NgayNhap = DateTime.Today;

                model.NgayNhap = ViewBag.NgayNhap;
                model.DaXoa = false;
                //Sau khi đã ktra hết dl đầu vào

                db.PhieuNhaps.Add(model);
                db.SaveChanges();   //save để lấy MaPN gán cho lst chitietpn
                SanPham sp;


                foreach (var item in lstModel)
                {
                    sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                    sp.SoLuongTon += item.SoLuongNhap;  //update solg tồn

                    item.MaPN = model.MaPN; //gán MaPN cho all chitietpn
                }
                db.ChiTietPhieuNhaps.AddRange(lstModel);
                db.SaveChanges();

                return View();
            }
            else
            {
                @ViewBag.KetQuaNhap = "Vui lòng thêm sản phẩm nhập";
                return View();
            }
        }

        [HttpGet]
        public ActionResult DSSPHetHang()
        {
            //ds sp gần hết hàng với số lượng tồn bé hơn hoặc bằng 5
            var lstSP = db.SanPhams.Where(n => n.DaXoa == false && n.SoLuongTon <= 5);
            return View(lstSP);
        }

        [HttpGet]
        public ActionResult NhapHangDon(int? id)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");

            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }

        [HttpPost]
        public ActionResult NhapHangDon(PhieuNhap model, ChiTietPhieuNhap ctpn)
        {
            SanPham sp = db.SanPhams.Single(n => n.MaSP == ctpn.MaSP);

            if (ctpn.SoLuongNhap == null || ctpn.DonGiaNhap == null || ctpn.SoLuongNhap < 1 || ctpn.DonGiaNhap < 100000)
            {
                @ViewBag.KetQuaNhap = "Nhập hàng không thành công";
                return View(sp);
            }
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", model.MaNCC);

            model.NgayNhap = DateTime.Now;
            model.DaXoa = false;
            db.PhieuNhaps.Add(model);
            db.SaveChanges();   //save để lấy MaPN gán cho lst chitietpn

            ctpn.MaPN = model.MaPN;


            sp.SoLuongTon += ctpn.SoLuongNhap;
            db.ChiTietPhieuNhaps.Add(ctpn);
            db.SaveChanges();

            @ViewBag.KetQuaNhap = "Nhập hàng thành công";
            return View(sp);
        }
        public ActionResult DanhSachPhieuNhap()
        {
            //lấy ds đơn hàng chưa duyệt
            var lst = db.PhieuNhaps;
            return View(lst);
        }

        public ActionResult ChiTietPhieuNhap(int? id)
        {
            //ktra id hợp lệ
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            PhieuNhap model = db.PhieuNhaps.SingleOrDefault(n => n.MaPN == id);
            //ktra đơn hàng có tồn tại ko
            if (model == null)
            {
                return HttpNotFound();
            }
            //hiển thị ds chitietdonhang 
            var lstchiTietPhieuNhap = db.ChiTietPhieuNhaps.Where(n => n.MaPN == id);
            ViewBag.ListChiTietPN = lstchiTietPhieuNhap;
            ViewBag.TenKH = model.NhaCungCap.TenNCC;
            return View(model);
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
