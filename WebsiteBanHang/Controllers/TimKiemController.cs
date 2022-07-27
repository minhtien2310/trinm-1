using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
using PagedList;

namespace WebsiteBanHang.Controllers
{
    public class TimKiemController : Controller
    {

        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: TimKiem
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa, int? page)
        {
            ViewBag.KetQuaTimKiem = "";
            //Phân trang
            if (Request.HttpMethod != "GET")
                page = 1;
            //tạo biến số sản phẩm trên trang
            int PageSize = 6;
            //tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            //tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa) && n.DaXoa == false && n.SoLuongTon.Value > 0);
            
            ViewBag.TuKhoa = sTuKhoa;
            var lstSPTemp = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa));

            if (lstSPTemp.Count() > 0)
            {
                ViewBag.KetQuaTimKiem = "Sản phẩm hết hàng";
            }
            else
            {
                ViewBag.KetQuaTimKiem = "Không tìm thấy sản phẩm";
            }
            return View(lstSP.OrderBy(n=>n.TenSP).ToPagedList(PageNumber,PageSize));
        }
        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string sTuKhoa)
        {
            //gọi về hàm get tìm kiếm

            return RedirectToAction("KQTimKiem", new {@sTuKhoa = sTuKhoa });
        }

        protected void SetAlert(string message, int type)
        {
            TempData["AlertMessage"] = message;
            if (type == 1)
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == 2)
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == 3)
            {
                TempData["AlertType"] = "alert-danger";
            }
            else
            {
                TempData["AlertType"] = "alert-info";
            }
        }
    }
}