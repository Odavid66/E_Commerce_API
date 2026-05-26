using E_commerce_API.DTOs;

namespace E_commerce_API.Services

{
    public interface IPaymentService
    {
        Task<PaymentResponseDtos> InitializePayment(IntializePaymentDtos dto);
        // Intialize payment and provide a url for payment
        Task<VerifyPaymentDto> VerifyPayment(string reference);
        // verified payment and provide a reference string to know if they have paid
    }
}
