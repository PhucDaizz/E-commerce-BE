namespace Ecommerce.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,    // Order is pending
        Error = 1,      // Order encountered an error
        Completed = 2,  // Order is completed
        Cancelled = 3,  // Order is cancelled
        Confirmed = 4   // Order is confirmed
    }
}
