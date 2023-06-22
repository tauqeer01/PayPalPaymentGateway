using Microsoft.AspNetCore.Mvc;
using paypalGateway.Models;
using paypalGateway.Services;
using System.Diagnostics;

namespace paypalGateway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Success(string paymentId, string token, string PayerID)
        {
            ViewData["PaymentId"] = paymentId;
            ViewData["token"] = token;
            ViewData["payerid"] = PayerID;

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> PayUsingCard (PaymentViewModel model)
        {
            try
            {
                if(model.Amount == 0)
                {
                    TempData["error"] = "please enter amount";
                    return RedirectToAction(nameof(Index));
                }

                // Generate PayPal payment details
                decimal amount = model.Amount; // Assuming Price is the membership amount
                string returnUrl = "https://localhost:7182/Home/Success"; // Specify your return URL
                string cancelUrl = "https://localhost:7182/Home/Cencel"; // Specify your cancel URL

                // Create a PayPal order
                var createdPayment = await _unitOfWork.PaypalServices.CreateOrderAsync(amount, returnUrl, cancelUrl);

                // Get the PayPal approval URL
                string approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href;

                // Redirect the user to the PayPal approval URL
                if (!string.IsNullOrEmpty(approvalUrl))
                {
                    return Redirect(approvalUrl);
                }
                else
                {
                    TempData["error"] = "Failed to initiate PayPal payment.";
                }

            }
            catch (Exception ex)
            {

                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}