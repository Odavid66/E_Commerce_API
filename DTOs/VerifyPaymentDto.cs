namespace E_commerce_API.DTOs
{
    public class VerifyPaymentDto
        //what we send back after verifying payment
    {
        public bool Status { get; set; }
        // true payment was verifiedsuccessfully
        public string Message { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        //amount paid in naira
        public string Reference { get; set; } = string.Empty;
    }
}
