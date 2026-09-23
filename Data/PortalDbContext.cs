using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VitalityPortal.Data;

public sealed class PortalDbContext(DbContextOptions<PortalDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<ReferralEntity> Referrals => Set<ReferralEntity>();
	public DbSet<WithdrawalEntity> Withdrawals => Set<WithdrawalEntity>();
	public DbSet<PayoutSettingsEntity> PayoutSettings => Set<PayoutSettingsEntity>();
	public DbSet<PortalMessageEntity> PortalMessages => Set<PortalMessageEntity>();
	public DbSet<CommissionEntity> Commissions => Set<CommissionEntity>();
	public DbSet<PurchaseEntity> Purchases => Set<PurchaseEntity>();
	public DbSet<OrderEntity> Orders => Set<OrderEntity>();
	public DbSet<PortalContentEntity> PortalContent => Set<PortalContentEntity>();
	public DbSet<WalletTransactionEntity> WalletTransactions => Set<WalletTransactionEntity>();
	public DbSet<RankHistoryEntity> RankHistory => Set<RankHistoryEntity>();
	public DbSet<KycRecordEntity> KycRecords => Set<KycRecordEntity>();
	public DbSet<SupportTicketEntity> SupportTickets => Set<SupportTicketEntity>();
	public DbSet<SupportTicketMessageEntity> SupportTicketMessages => Set<SupportTicketMessageEntity>();
	public DbSet<AutoShipEntity> AutoShips => Set<AutoShipEntity>();
	public DbSet<TeamChatMessageEntity> TeamChatMessages => Set<TeamChatMessageEntity>();
	public DbSet<TransactionPasswordLogEntity> TransactionPasswordLogs => Set<TransactionPasswordLogEntity>();
}