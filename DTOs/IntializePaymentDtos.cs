namespace E_commerce_API.DTOs
{
    public class IntializePaymentDtos
    {
        public string Email { get; set; } = string.Empty;
        //email of paing customer
        public decimal Amount { get; set; }
        // amount in naira, we willc onvert to kobo as paystack works in smallest currency
        public int OrderId { get; set; }
        // which order this payment is for
    }
}
