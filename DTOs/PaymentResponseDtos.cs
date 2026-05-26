namespace E_commerce_API.DTOs
{
    public class PaymentResponseDtos
        //what we send back to the client
    {
        public bool Status { get; set; }
        // true means payment successful
        public string Message { get; set; } = string.Empty;
        public string AuthorizationUrl { get; set; } = string.Empty;
        // the URL the client opens to complete payment
        public string Reference { get; set; } = string.Empty;
        // unique reference for this transaction
    }
}
