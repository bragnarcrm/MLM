namespace VitalityPortal.Data;

public sealed class ReferralEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string SponsorId { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public decimal MonthlySales { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public sealed class WithdrawalEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Fee { get; set; }
}

public sealed class PayoutSettingsEntity
{
    public int Id { get; set; }
    public string AccountHolder { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}

public sealed class PortalMessageEntity
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}

public sealed class CommissionEntity
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
    public string FromUserId { get; set; } = string.Empty;
    public string OwnerUserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Vat { get; set; }
    public string Status { get; set; } = "Pending";
    public int? Level { get; set; }
}

public sealed class WalletTransactionEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Comments { get; set; } = string.Empty;
}

public sealed class RankHistoryEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string OldRank { get; set; } = string.Empty;
    public string NewRank { get; set; } = string.Empty;
    public decimal Incentive { get; set; }
    public DateTime ChangedAt { get; set; }
}

public sealed class PurchaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsBusinessKit { get; set; }
    public int Quantity { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
    public DateTime PurchasedAt { get; set; }
}

public sealed class OrderEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal ShippingFee { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Processing";
    public string CourierName { get; set; } = "The Courier Guy";
    public string TrackingNumber { get; set; } = string.Empty;
    public string ShippingStatus { get; set; } = "Packed"; // "Placed", "Packed", "Out for Delivery", "Delivered"
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime OrderedAt { get; set; }
}

public sealed class AutoShipEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ProductName { get; set; } = "Herbal Tea Pack";
    public decimal Amount { get; set; } = 199.00m;
    public int Quantity { get; set; } = 1;
    public int FrequencyDays { get; set; } = 30;
    public DateTime NextRunDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public string PaymentMethod { get; set; } = "Card";
    public string Status { get; set; } = "Active"; // "Active", "Paused", "Cancelled"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastRunDate { get; set; }
}

public sealed class TeamChatMessageEntity
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string RecipientUserId { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}

public sealed class PortalContentEntity
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public DateTime PublishedAt { get; set; }
    public bool IsRead { get; set; }
}

public sealed class TransactionPasswordLogEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = "Generate Transaction Password";
    public string Remarks { get; set; } = "Temporary PIN generated & sent to email";
    public string Status { get; set; } = "Active / Sent";
    public string IpAddress { get; set; } = "127.0.0.1";
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

public sealed class KycRecordEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string IdType { get; set; } = "National ID";
    public string IdNumber { get; set; } = string.Empty;
    public string IdDocumentName { get; set; } = string.Empty;
    public string AddressDocumentName { get; set; } = string.Empty;
    public string Status { get; set; } = "Unverified"; // "Unverified", "Pending", "Verified", "Rejected"
    public string? ReviewNotes { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public sealed class SupportTicketEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Category { get; set; } = "General"; // "Delivery", "Payout", "Account", "Technical", "General"
    public string Status { get; set; } = "Open"; // "Open", "In Progress", "Answered", "Closed"
    public string Priority { get; set; } = "Medium"; // "Low", "Medium", "High"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class SupportTicketMessageEntity
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public bool IsStaff { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}