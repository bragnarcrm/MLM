namespace VitalityPortal.Models.Payments;

public sealed class PayFastOptions
{
    public const string SectionName = "PayFast";
    public string MerchantId { get; set; } = "10000100";
    public string MerchantKey { get; set; } = "46f0cd694581a";
    public string PassPhrase { get; set; } = "";
    public bool IsSandbox { get; set; } = true;
    public string ProcessUrl { get; set; } = "https://sandbox.payfast.co.za/eng/process";
}

public sealed class GmailSmtpOptions
{
    public const string SectionName = "GmailSmtp";
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string SenderEmail { get; set; } = "notifications@scaleengine.io";
    public string SenderName { get; set; } = "ScaleEngine Support";
    public string AppPassword { get; set; } = "";
    public bool EnableSsl { get; set; } = true;
}

public sealed class GoogleAuthOptions
{
    public const string SectionName = "GoogleAuth";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
}

public sealed class GoogleLoginRequest
{
    public string? IdToken { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? GoogleId { get; set; }
}

public sealed class InitiatePayFastRequest
{
    public decimal Amount { get; set; }
    public string ItemName { get; set; } = "ScaleEngine Activation";
    public string PaymentType { get; set; } = "activation"; // "activation" or "purchase"
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public sealed class PayFastInitiateResponseDto
{
    public string ProcessUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = [];
}

public sealed class DirectPaymentRequest
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Card"; // "Card", "Instant EFT", "Capitec Pay", "Wallet"
    public string PaymentType { get; set; } = "activation"; // "activation" or "purchase"
    public string? ProductName { get; set; }
    public int Quantity { get; set; } = 1;
    public string? CardNumber { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCvv { get; set; }
}

public sealed class PaymentResultDto
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaidAt { get; set; } = string.Empty;
}
