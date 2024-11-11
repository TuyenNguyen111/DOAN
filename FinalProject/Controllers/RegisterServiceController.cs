using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class RegisterServiceController : Controller
    {
        private readonly QlptContext _db;
        private readonly RegisterService _registerService;
        private readonly PaypalClient _paypalClient;

        public RegisterServiceController(QlptContext db, RegisterService registerService, PaypalClient paypalClient)
        {
            _db = db;
            _registerService = registerService;
            _paypalClient = paypalClient;
        }
        #region Trang Service tĩnh
        public IActionResult HomePage()
        {
            return View();
        }
        #endregion

        #region RegisterServiceView
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var services = await _registerService.GetServicesAsync();
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(services);
        }
        #endregion

        #region Paypal payment
        [Authorize]
        [HttpPost("/RegisterService/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(string serviceId, CancellationToken cancellationToken)
        {

            var service = await _registerService.GetServiceByIdAsync(Int32.Parse(serviceId));
            if (service == null)
            {
                return BadRequest(new { message = "Service not found." });
            }
            int postId = (int)TempData["PostId"];

            string totalAmount = ConvertToUSD((decimal)service.ServicePrice).ToString("N0");
            var currency = "USD";
            var referenceOrderId = "DH" + DateTime.Now.Ticks.ToString();
            try
            {
                var response = await _paypalClient.CreateOrder(totalAmount, currency, referenceOrderId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.GetBaseException().Message });
            }
        }

        private decimal ConvertToUSD(decimal servicePrice)
        {
            const decimal exchangeRate = 0.00004m;
            return servicePrice * exchangeRate;
        }

        [Authorize]
        [HttpPost("/RegisterService/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                if (response != null && response.status == "COMPLETED")
                {
                    if (TempData["PostId"] == null || TempData["ServiceId"] == null)
                    {
                        throw new Exception("PostId or ServiceId is missing.");
                    }
                    int postId = (int)TempData["PostId"];
                    int serviceId = (int)TempData["ServiceId"];

                    var service = await _registerService.GetServiceByIdAsync(serviceId);
                    decimal totalPrice = (decimal)service.ServicePrice;
                    DateOnly paymentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    int billStatus = 1;
                    DateOnly expirationDate = paymentDate.AddDays((int)service.ServiceTime);


                    await _registerService.CreateBillAsync(postId, serviceId, totalPrice, paymentDate, billStatus, expirationDate);
                    return Ok(response);
                }

                return BadRequest("Payment could not be completed.");
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        public IActionResult Success()
        {
            return View();
        }
        #endregion

        #region Lấy post id và kiểm tra đã đăng ký dịch vụ hay hết hạn chưa
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SetPostId(int postId)
        {
            var existingBill = await _db.Bills
                   .Include(b => b.Service)
                   .Where(b => b.PostId == postId && b.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                   .FirstOrDefaultAsync();

            if (existingBill != null)
            {
                var serviceName = existingBill.Service?.ServiceName ?? "dịch vụ";

                TempData["FailMessage"] = $"Bạn không thể đăng ký thêm dịch vụ cho bài đăng này vì bạn đã đăng ký dịch vụ {serviceName}.";
                return RedirectToAction("ManageRoom", "RoomPost");
              
            }
            TempData["PostId"] = postId;
            return RedirectToAction("Index");
        }
        #endregion

        #region lưu service id
        [Authorize]
        [HttpPost]
        public IActionResult SaveServiceId(int serviceId)
        {
            TempData["ServiceId"] = serviceId;
            return Ok();
        }
        #endregion

        #region Show history payment
        public async Task<IActionResult> Bills()
        {
            var bills = await _registerService.GetBillsAsync();
            return View(bills);
        }
        #endregion

    }
}
