namespace VitalityPortal.Models.Portal;

public sealed record PaginatedDto<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
public sealed record DashboardSummaryDto(decimal TotalEarned, decimal TotalWithdrawal, decimal EwalletBalance, int DirectReferrals, string CurrentRank, IReadOnlyList<ReferralDto> Referrals, IReadOnlyList<WithdrawalDto> PendingWithdrawals);
public sealed record ReferralDto(string UserId, string FullName, string Mobile, string SponsorId, string Rank, decimal MonthlySales, string Status, DateOnly JoinedOn, int Id = 0);
public sealed record TreeNodeDto(string UserId, string Name, string Status, IReadOnlyList<TreeNodeDto> Children);
public sealed record IncomeSummaryDto(decimal AllCommission, decimal DirectReferralBonus, decimal LevelBonus, decimal RankBonus, decimal LevelTurnover, decimal MonthlySalary);
public sealed record CommissionDto(DateOnly Date, string FromUserId, string Name, decimal Amount, decimal Vat, decimal Total, string Status, int? Level);
public sealed record LevelTurnoverDto(int Level, decimal Turnover);
public sealed record WalletBalanceDto(decimal AdminTransfer, decimal TotalEarnings, decimal AmountWithdrawn, decimal AvailableBalance, decimal PendingEarnings = 0m);
public sealed record WalletTransactionDto(DateOnly Date, string Category, decimal Amount, string Comments);
public sealed record WithdrawalDto(decimal Amount, string PaymentMethod, DateTime RequestedAt, string Status, decimal Fee = 20m, int Id = 0);
public sealed record PayoutSettingsDto(string AccountHolder, string AccountNumber, string BankName, string AccountType, string Address, string Country, string PostalCode);
public sealed record PayoutSettingsRequest(string AccountHolder, string AccountNumber, string BankName, string AccountType, string Address, string Country, string PostalCode);
public sealed record PurchaseDto(string ProductName, decimal Amount, bool IsBusinessKit, int Quantity, decimal Total, string PaymentMethod, string Status, DateOnly Date);
public sealed record OrderDto(string OrderId, DateOnly Date, decimal Amount, decimal ShippingFee, decimal Total, string PaymentMethod, string Status, string CourierName, string TrackingNumber, string ShippingStatus, DateOnly? EstimatedDelivery, string InvoiceUrl);
public sealed record PortalMessageDto(int Id, string SenderUserId, string SenderName, string Recipient, string Subject, string Body, DateTime SentAt, bool IsRead);
public sealed record SendMessageRequest(string Recipient, string Subject, string Body);
public sealed record ZoomMeetingDto(string Title, string Description, string JoinLink, TimeOnly Time, DateOnly Date);
public sealed record PortalContentDto(int Id, string Type, string Title, string Body, string? LinkUrl, DateTime PublishedAt, bool IsRead);
public sealed record CreateCommissionRequest(string Category, string FromUserId, string Name, decimal Amount, decimal Vat, int? Level, string OwnerUserId);
public sealed record RankHistoryDto(string OldRank, string NewRank, decimal Incentive, DateOnly Date);
public sealed record CreatePurchaseRequest(string ProductName, decimal Amount, bool IsBusinessKit, int Quantity, string PaymentMethod);
public sealed record CreateOrderRequest(decimal Amount, decimal ShippingFee, string PaymentMethod);
public sealed record CreateContentRequest(string Type, string Title, string Body, string? LinkUrl);
public sealed record GenealogySearchResultDto(string UserId, string FullName, string SponsorId, int Level, string Status, string Rank);
public sealed record TeamLevelStatDto(int Level, int MemberCount, int ActiveCount, int InactiveCount, decimal TeamSales);
public sealed record TeamPerformanceDto(int TotalMembers, int ActiveCount, int InactiveCount, decimal TotalTeamSales, IReadOnlyList<TeamLevelStatDto> Levels);
public sealed record ReassignReferralRequest(string NewSponsorId);
public sealed record ImportMemberRow(string Username, string FullName, string Mobile);
public sealed record BulkImportResultDto(int Imported, IReadOnlyList<string> Errors);

// KYC DTOs
public sealed record KycStatusDto(string Status, string IdType, string IdNumber, string IdDocumentName, string AddressDocumentName, string? ReviewNotes, DateTime? SubmittedAt, DateTime? ReviewedAt);
public sealed record SubmitKycRequest(string IdType, string IdNumber, string? IdDocumentBase64, string? IdDocumentFileName, string? AddressDocumentBase64, string? AddressDocumentFileName);

// Support Ticket DTOs
public sealed record SupportTicketDto(int Id, string TicketNumber, string Subject, string Category, string Status, string Priority, DateTime CreatedAt, DateTime UpdatedAt, int MessageCount);
public sealed record TicketMessageDto(int Id, int TicketId, string SenderUserId, string SenderName, bool IsStaff, string Message, DateTime CreatedAt);
public sealed record SupportTicketDetailDto(int Id, string TicketNumber, string Subject, string Category, string Status, string Priority, DateTime CreatedAt, DateTime UpdatedAt, IReadOnlyList<TicketMessageDto> Messages);
public sealed record CreateTicketRequest(string Subject, string Category, string Priority, string InitialMessage);
public sealed record ReplyTicketRequest(string Message);

// Auto-Ship Subscription DTOs
public sealed record AutoShipDto(int Id, string ProductName, decimal Amount, int Quantity, decimal Total, int FrequencyDays, DateOnly NextRunDate, string PaymentMethod, string Status, DateOnly CreatedAt);
public sealed record CreateAutoShipRequest(string ProductName, int Quantity, int FrequencyDays, string PaymentMethod);
public sealed record UpdateAutoShipStatusRequest(string Status);

// Team Chat DTOs
public sealed record TeamChatContactDto(string UserId, string FullName, string Rank, string Role, int UnreadCount, DateTime? LastMessageAt);
public sealed record TeamChatMessageDto(int Id, string SenderUserId, string SenderName, string RecipientUserId, string RecipientName, string Message, DateTime SentAt, bool IsMine);
public sealed record SendTeamChatRequest(string RecipientUserId, string Message);