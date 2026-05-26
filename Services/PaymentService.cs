using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using E_commerce_API.DTOs;
using E_Commerce_API.Data;
using System.Text.Json.Nodes;
using E_Commerce_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        //HttpClient is used to make HTTP requests to Paystack API
        private readonly IConfiguration _config;
        //reads values from appsettings.json
        private readonly DataContext _context;
        //gives access to database

        public PaymentService(HttpClient httpClient, IConfiguration config, DataContext context)
        {
            _httpClient = httpClient;
            _config = config;
            _context = context;
        }

        public async Task<PaymentResponseDtos> InitializePayment(IntializePaymentDtos dto)
            // data contains emai; amount and orderId
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);
            // check if order exists in database
            if (order == null)
            {
                return new PaymentResponseDtos
                {
                    Status = false,
                    Message = "Order not found"
                };
            }

            if (order.Status == "Paid")
            {
                return new PaymentResponseDtos
                {
                    Status = false,
                    Message = "Order already paid"
                };
            }
            //prevent from payingorder twice

            var secretKey = _config["Paystack:SecretKey"];
            // reads secret key from appsettings.json

            // set the authorization header
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", secretKey);
            // Paystack requires: Authorization: Bearer sk_test_xxx on every request

            var reference = $"ORDER_{dto.OrderId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            var payload = new
            // form/receipts to send to paystack
            {
                email = dto.Email,
                // ↑ customer email — Paystack sends receipt here
                amount = (int)(dto.Amount * 100),
                //convert naira to kobo
                reference = reference,
                // unique reference for this transaction
                // we build it from the order id and current timestamp
                metadata = new
                {
                    order_id = dto.OrderId,
                    // pass the order id to Paystack as metadata
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");
            // ↑ converts the payload object to a JSON string

            var response = await _httpClient.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                content);
            // and sends the payload as POST request to Paystack API

            var responseString = await response.Content.ReadAsStringAsync();
            // Paystack returns JSON 

            var jsonResponse = JsonNode.Parse(responseString);
            // parse data response, convert json to object

            var status = jsonResponse?["status"]?.GetValue<bool>() ?? false;
            var message = jsonResponse?["message"]?.GetValue<string>() ?? string.Empty;
            var data = jsonResponse?["data"];
            // navigate into the "data" section of the response
            // ?. is the null safe operator — does not crash if null
            // ?? provides a default value if null

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                // ↑ links this payment to the order

                Reference = reference,
                // ↑ our unique reference — used later to verify

                Amount = dto.Amount,
                Status = "Pending"
                // ↑
                // Pending means customer has not paid yet
                // just the checkout URL was created
                // Status changes to "Success" after VerifyPayment
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            // ↑
            // saves the payment record to the database
            // now we have a record that a payment was started
            // if customer never pays it stays as Pending

            return new PaymentResponseDtos
            {
                Status = status,
                Message = message,
                AuthorizationUrl = data?["authorization_url"]?.GetValue<string>() ?? string.Empty,
                Reference = data?["reference"]?.GetValue<string>() ?? string.Empty
            };

        }

        public async Task<VerifyPaymentDto> VerifyPayment(string reference)
        {
            // verify payment exist in database
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Reference == reference);

            if (payment == null)
            {
                return new VerifyPaymentDto
                {
                    Status = false,
                    Message = "Payment not found"
                };
            }

            var response = await _httpClient.GetAsync(
                $"https://api.paystack.co/transaction/verify/{reference}");
            // Paystack looks up their records for this reference

            var responseString = await response.Content.ReadAsStringAsync();
            // reads Paystack's response as a raw string
            // Paystack returns JSON 
            var jsonResponse = JsonNode.Parse(responseString);
            // parse the JSON response into an object we can navigate

            var status = jsonResponse?["status"]?.GetValue<bool>() ?? false;
            var message = jsonResponse?["message"]?.GetValue<string>() ?? string.Empty;
            var data = jsonResponse?["data"];

            // reads the required data from the Paystack object

            var paystackStatus = data?["status"]?.GetValue<string>() ?? string.Empty;
            // reads the payment status from INSIDE the data section

            if (paystackStatus.Equals("success", StringComparison.OrdinalIgnoreCase))
            // .Equals() with StringComparison.OrdinalIgnoreCase means "success", "Success", "SUCCESS" all match
            // safer than == because it ignores letter casing
            {
                payment.Status = "Success";
                payment.PaidAt = DateTime.UtcNow;
                payment.PaystackReference = data?["reference"]?.GetValue<string>() ?? payment.PaystackReference;

                if (payment.Order is not null)
                {
                    payment.Order.Status = "Paid";
                }

                await _context.SaveChangesAsync();
            }

            return new VerifyPaymentDto
            {
                Status = paystackStatus.Equals("success", StringComparison.OrdinalIgnoreCase),
                Message = message
            };
        }
    }
}


