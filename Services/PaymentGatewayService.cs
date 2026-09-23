using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Payments;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public interface IPaymentGatewayService
{
    PayFastInitiateResponseDto InitiatePayFast(string userId, string userEmail, InitiatePayFastRequest request, string baseUrl);
    Task<PaymentResultDto> ProcessDirectPaymentAsync(string userId, DirectPaymentRequest request);
    Task<bool> HandlePayFastNotifyAsync(IFormCollection form);
}

public sealed class PaymentGatewayService(
    IOptions<PayFastOptions> options,
    IActivationService activationService,
    ICommerceService commerceService,
    IPortalRepository portalRepository,
    IEmailSenderService emailService,
    ILogger<PaymentGatewayService> logger) : IPaymentGatewayService
{
    private readonly PayFastOptions _options = options.Value;

    public PayFastInitiateResponseDto InitiatePayFast(string userId, string userEmail, InitiatePayFastRequest request, string baseUrl)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchant_id"] = _options.MerchantId,
            ["merchant_key"] = _options.MerchantKey,
            ["return_url"] = request.ReturnUrl ?? $"{baseUrl}/dashboard.html?payment=success",
            ["cancel_url"] = request.CancelUrl ?? $"{baseUrl}/activation.html?payment=cancelled",
            ["notify_url"] = $"{baseUrl}/api/payments/payfast/notify",
            ["name_first"] = userId,
            ["email_address"] = userEmail,
            ["m_payment_id"] = $"{request.PaymentType.ToUpper()}_{userId}_{DateTime.UtcNow.Ticks}",
            ["amount"] = request.Amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            ["item_name"] = request.ItemName,
            ["item_description"] = $"ScaleEngine {request.PaymentType} payment",
            ["custom_str1"] = userId,
            ["custom_str2"] = request.PaymentType
        };

        // Compute PayFast MD5 signature
        var signatureString = new StringBuilder();
        foreach (var kvp in parameters.Where(k => !string.IsNullOrEmpty(k.Value)))
        {
            signatureString.Append($"{kvp.Key}={Uri.EscapeDataString(kvp.Value.Trim())}&");
        }
        var stringToHash = signatureString.ToString().TrimEnd('&');
        if (!string.IsNullOrEmpty(_options.PassPhrase))
        {
            stringToHash += $"&passphrase={Uri.EscapeDataString(_options.PassPhrase.Trim())}";
        }

        var md5Bytes = MD5.HashData(Encoding.UTF8.GetBytes(stringToHash));
        var signature = Convert.ToHexString(md5Bytes).ToLowerInvariant();
        parameters["signature"] = signature;

        return new PayFastInitiateResponseDto
        {
            ProcessUrl = _options.ProcessUrl,
            Parameters = parameters
        };
    }

    public async Task<PaymentResultDto> ProcessDirectPaymentAsync(string userId, DirectPaymentRequest request)
    {
        var txId = $"TX_{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        var now = DateTime.UtcNow;

        if (request.PaymentType == "activation")
        {
            activationService.Activate(userId, new Models.Account.ActivateAccountRequest
            {
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod
            });

            portalRepository.AddTransaction(userId, new Models.Portal.WalletTransactionDto(
                DateOnly.FromDateTime(now),
                "Account Activation",
                -request.Amount,
                $"Activation fee paid via {request.PaymentMethod} ({txId})"
            ));
        }
        else
        {
            // eCommerce Purchase
            var productName = request.ProductName ?? "Business Starter Kit";
            commerceService.RecordPurchase(userId, new Models.Portal.CreatePurchaseRequest(
                productName,
                request.Amount / Math.Max(1, request.Quantity),
                productName.Contains("Business", StringComparison.OrdinalIgnoreCase),
                request.Quantity,
                request.PaymentMethod
            ));

            commerceService.RecordOrder(userId, new Models.Portal.CreateOrderRequest(
                request.Amount,
                0m,
                request.PaymentMethod
            ));
        }

        // Send email receipt confirmation
        _ = emailService.SendEmailAsync(
            $"{userId}@scaleengine.io",
            $"Payment Receipt: {request.PaymentType.ToUpper()} ({txId})",
            $"<h3>ScaleEngine Payment Confirmation</h3><p>Amount: <strong>R{request.Amount:F2}</strong></p><p>Method: {request.PaymentMethod}</p><p>Transaction ID: <code>{txId}</code></p>"
        );

        logger.LogInformation("Processed payment {TxId} of R{Amount} via {Method} for {User}", txId, request.Amount, request.PaymentMethod, userId);

        return await Task.FromResult(new PaymentResultDto
        {
            Succeeded = true,
            Message = $"{request.PaymentType.ToUpper()} payment processed successfully.",
            TransactionId = txId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaidAt = now.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    public async Task<bool> HandlePayFastNotifyAsync(IFormCollection form)
    {
        var paymentStatus = form["payment_status"].ToString();
        var userId = form["custom_str1"].ToString();
        var paymentType = form["custom_str2"].ToString();
        var amountStr = form["amount_gross"].ToString();
        var pfPaymentId = form["pf_payment_id"].ToString();

        decimal.TryParse(amountStr, out var amount);

        logger.LogInformation("[PayFast IPN] Status: {Status}, User: {User}, Type: {Type}, Amount: {Amount}, PfId: {PfId}",
            paymentStatus, userId, paymentType, amount, pfPaymentId);

        if (paymentStatus.Equals("COMPLETE", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(userId))
        {
            if (paymentType.Equals("activation", StringComparison.OrdinalIgnoreCase))
            {
                activationService.Activate(userId, new Models.Account.ActivateAccountRequest
                {
                    Amount = amount > 0 ? amount : 90m,
                    PaymentMethod = "PayFast"
                });
            }
            return await Task.FromResult(true);
        }

        return await Task.FromResult(false);
    }
}
