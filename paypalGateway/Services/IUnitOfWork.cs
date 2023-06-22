namespace paypalGateway.Services
{
    public interface IUnitOfWork
    {
        IPaypalServices PaypalServices { get; }
    }
}
