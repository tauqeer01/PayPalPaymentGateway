using PayPal.Api;

namespace paypalGateway.Services
{
    public interface IPaypalServices
    {
        Task<Payment> CreateOrderAsync(decimal amount, string returnUrl, string cancelUrl);
    }
}
