using Microsoft.EntityFrameworkCore;
using VitalityPortal.Data;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Members;
using VitalityPortal.Models.Portal;

namespace VitalityPortal.Repositories;

public sealed class PortalRepository(PortalDbContext context) : IPortalRepository
{
    public IReadOnlyList<ReferralDto> GetReferrals() => context.Referrals.AsNoTracking().OrderBy(item => item.Id).Select(item => new ReferralDto(item.UserId, item.FullName, item.Mobile, item.SponsorId, item.Rank, item.MonthlySales, item.Status, DateOnly.FromDateTime(item.JoinedAt), item.Id)).ToList();
    public ReferralDto? GetReferralById(int id)
    {
        var item = context.Referrals.AsNoTracking().FirstOrDefault(referral => referral.Id == id);
        return item is null ? null : new ReferralDto(item.UserId, item.FullName, item.Mobile, item.SponsorId, item.Rank, item.MonthlySales, item.Status, DateOnly.FromDateTime(item.JoinedAt), item.Id);
    }
    public void AddReferral(ReferralDto referral)
    {
        context.Referrals.Add(new ReferralEntity { UserId = referral.UserId, FullName = referral.FullName, Mobile = referral.Mobile, SponsorId = referral.SponsorId, Rank = referral.Rank, MonthlySales = referral.MonthlySales, Status = referral.Status, JoinedAt = referral.JoinedOn.ToDateTime(TimeOnly.MinValue) });
        context.SaveChanges();
    }
    public bool UpdateReferral(int id, UpdateReferralRequest request)
    {
        var entity = context.Referrals.FirstOrDefault(referral => referral.Id == id);
        if (entity is null) return false;
        entity.FullName = request.FullName; entity.Mobile = request.Mobile; entity.Rank = request.Rank; entity.MonthlySales = request.MonthlySales; entity.Status = request.Status;
        context.SaveChanges();
        return true;
    }
    public bool DeleteReferral(int id)
    {
        var entity = context.Referrals.FirstOrDefault(referral => referral.Id == id);
        if (entity is null) return false;
        context.Referrals.Remove(entity);
        context.SaveChanges();
        return true;
    }
    public TreeNodeDto GetTree(string userId)
    {
        var all = GetReferrals();
        var self = all.FirstOrDefault(item => item.UserId == userId);
        TreeNodeDto Build(string id, string name, string status, int depthRemaining)
        {
            var children = depthRemaining <= 0
                ? []
                : all.Where(item => item.SponsorId == id).Select(item => Build(item.UserId, item.FullName, item.Status, depthRemaining - 1)).ToList();
            return new TreeNodeDto(id, name, status, children);
        }
        return Build(userId, self?.FullName ?? userId, self?.Status ?? "Active", 6);
    }

    public bool IsInDownline(string ownerId, string targetUserId)
    {
        if (ownerId == targetUserId) return true;
        var all = GetReferrals();
        var frontier = new Queue<string>();
        frontier.Enqueue(ownerId);
        var visited = new HashSet<string> { ownerId };
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            foreach (var child in all.Where(item => item.SponsorId == current))
            {
                if (child.UserId == targetUserId) return true;
                if (visited.Add(child.UserId)) frontier.Enqueue(child.UserId);
            }
        }
        return false;
    }

    public IReadOnlyList<GenealogySearchResultDto> SearchDownline(string ownerId, string? query)
    {
        var all = GetReferrals();
        var results = new List<GenealogySearchResultDto>();
        void Walk(string sponsorId, int depth)
        {
            foreach (var child in all.Where(item => item.SponsorId == sponsorId))
            {
                if (string.IsNullOrWhiteSpace(query) || child.UserId.Contains(query, StringComparison.OrdinalIgnoreCase) || child.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    results.Add(new GenealogySearchResultDto(child.UserId, child.FullName, child.SponsorId, depth, child.Status, child.Rank));
                Walk(child.UserId, depth + 1);
            }
        }
        Walk(ownerId, 1);
        return results;
    }

    public TeamPerformanceDto GetTeamPerformance(string ownerId)
    {
        var all = GetReferrals();
        var levels = new List<TeamLevelStatDto>();
        var current = new List<string> { ownerId };
        var depth = 1;
        var totalActive = 0;
        var totalInactive = 0;
        var totalSales = 0m;
        while (current.Count > 0 && depth <= 10)
        {
            var levelMembers = all.Where(item => current.Contains(item.SponsorId)).ToList();
            if (levelMembers.Count == 0) break;
            var active = levelMembers.Count(item => item.Status == "Active");
            var inactive = levelMembers.Count - active;
            var sales = levelMembers.Sum(item => item.MonthlySales);
            levels.Add(new TeamLevelStatDto(depth, levelMembers.Count, active, inactive, sales));
            totalActive += active;
            totalInactive += inactive;
            totalSales += sales;
            current = levelMembers.Select(item => item.UserId).ToList();
            depth++;
        }
        return new TeamPerformanceDto(totalActive + totalInactive, totalActive, totalInactive, totalSales, levels);
    }

    public bool ReassignReferral(string userId, string newSponsorId)
    {
        if (userId == newSponsorId) return false;
        var entity = context.Referrals.FirstOrDefault(item => item.UserId == userId);
        if (entity is null) return false;
        if (IsInDownline(userId, newSponsorId)) return false; // prevent creating a cycle
        entity.SponsorId = newSponsorId;
        context.SaveChanges();
        return true;
    }

    public BulkImportResultDto ImportMembers(string sponsorId, IReadOnlyList<ImportMemberRow> rows)
    {
        var errors = new List<string>();
        var imported = 0;
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Username) || string.IsNullOrWhiteSpace(row.FullName)) { errors.Add($"Skipped a row with a missing username or name."); continue; }
            if (context.Referrals.Any(item => item.UserId == row.Username)) { errors.Add($"{row.Username} already exists."); continue; }
            context.Referrals.Add(new ReferralEntity { UserId = row.Username, FullName = row.FullName, Mobile = row.Mobile, SponsorId = sponsorId, Rank = "Newbie", MonthlySales = 0m, Status = "Pending", JoinedAt = DateTime.UtcNow });
            imported++;
        }
        context.SaveChanges();
        return new BulkImportResultDto(imported, errors);
    }
    public IReadOnlyList<string> GetSponsorChain(string userId, int maxDepth)
    {
        var chain = new List<string>();
        var currentUserId = userId;
        for (var depth = 0; depth < maxDepth; depth++)
        {
            var referral = context.Referrals.AsNoTracking().FirstOrDefault(item => item.UserId == currentUserId);
            if (referral is null || string.IsNullOrWhiteSpace(referral.SponsorId) || referral.SponsorId == currentUserId) break;
            chain.Add(referral.SponsorId);
            currentUserId = referral.SponsorId;
        }
        return chain;
    }
    public IReadOnlyList<CommissionDto> GetCommissions(string ownerUserId, string category) => context.Commissions.AsNoTracking().Where(item => item.OwnerUserId == ownerUserId).Where(item => category == "all" || item.Category == category).OrderByDescending(item => item.EarnedAt).Select(item => new CommissionDto(DateOnly.FromDateTime(item.EarnedAt), item.FromUserId, item.Name, item.Amount, item.Vat, item.Amount + item.Vat, item.Status, item.Level)).ToList();
    public void AddCommission(CreateCommissionRequest commission)
    {
        context.Commissions.Add(new CommissionEntity { Category = commission.Category, FromUserId = commission.FromUserId, OwnerUserId = commission.OwnerUserId, Name = commission.Name, Amount = commission.Amount, Vat = commission.Vat, Level = commission.Level, EarnedAt = DateTime.UtcNow, Status = "Pending" });
        context.SaveChanges();
    }
    public bool ApproveMatureCommissions(TimeSpan holdPeriod)
    {
        var cutoff = DateTime.UtcNow - holdPeriod;
        var due = context.Commissions.Where(item => item.Status == "Pending" && item.EarnedAt <= cutoff).ToList();
        if (due.Count == 0) return false;
        foreach (var item in due) item.Status = "Approved";
        context.SaveChanges();
        return true;
    }
    public IReadOnlyList<LevelTurnoverDto> GetLevelTurnover(string ownerUserId) => context.Commissions.AsNoTracking()
        .Where(item => item.OwnerUserId == ownerUserId && item.Category == "level" && item.Level != null)
        .ToList()
        .GroupBy(item => item.Level!.Value)
        .Select(group => new LevelTurnoverDto(group.Key, group.Sum(item => item.Amount + item.Vat)))
        .OrderBy(item => item.Level)
        .ToList();
    public IReadOnlyList<WalletTransactionDto> GetTransactions(string userId) => context.WalletTransactions.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.OccurredAt).Select(item => new WalletTransactionDto(DateOnly.FromDateTime(item.OccurredAt), item.Category, item.Amount, item.Comments)).ToList();
    public void AddTransaction(string userId, WalletTransactionDto transaction)
    {
        context.WalletTransactions.Add(new WalletTransactionEntity { UserId = userId, OccurredAt = transaction.Date.ToDateTime(TimeOnly.MinValue), Category = transaction.Category, Amount = transaction.Amount, Comments = transaction.Comments });
        context.SaveChanges();
    }
    public IReadOnlyList<WithdrawalDto> GetWithdrawals(string userId) => context.Withdrawals.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.RequestedAt).Select(item => new WithdrawalDto(item.Amount, item.PaymentMethod, item.RequestedAt, item.Status, item.Fee, item.Id)).ToList();
    public void AddWithdrawal(string userId, WithdrawalDto withdrawal)
    {
        context.Withdrawals.Add(new WithdrawalEntity { UserId = userId, Amount = withdrawal.Amount, PaymentMethod = withdrawal.PaymentMethod, RequestedAt = withdrawal.RequestedAt, Status = withdrawal.Status, Fee = withdrawal.Fee });
        context.SaveChanges();
    }
    public bool CancelWithdrawal(string userId, int id)
    {
        var entity = context.Withdrawals.FirstOrDefault(withdrawal => withdrawal.Id == id && withdrawal.UserId == userId);
        if (entity is null || entity.Status != "Pending") return false;
        entity.Status = "Cancelled";
        context.SaveChanges();
        return true;
    }
    public PayoutSettingsDto GetPayoutSettings()
    {
        var settings = context.PayoutSettings.AsNoTracking().OrderByDescending(item => item.Id).FirstOrDefault();
        return settings is null ? new PayoutSettingsDto("", "", "", "Savings", "", "South Africa", "") : new PayoutSettingsDto(settings.AccountHolder, settings.AccountNumber, settings.BankName, settings.AccountType, settings.Address, settings.Country, settings.PostalCode);
    }
    public void SavePayoutSettings(PayoutSettingsDto settings)
    {
        var entity = context.PayoutSettings.OrderByDescending(item => item.Id).FirstOrDefault();
        if (entity is null) { entity = new PayoutSettingsEntity(); context.PayoutSettings.Add(entity); }
        entity.AccountHolder = settings.AccountHolder; entity.AccountNumber = settings.AccountNumber; entity.BankName = settings.BankName; entity.AccountType = settings.AccountType; entity.Address = settings.Address; entity.Country = settings.Country; entity.PostalCode = settings.PostalCode;
        context.SaveChanges();
    }
    public IReadOnlyList<PurchaseDto> GetPurchases(string userId) => context.Purchases.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.PurchasedAt).Select(item => new PurchaseDto(item.ProductName, item.Amount, item.IsBusinessKit, item.Quantity, item.Amount * item.Quantity, item.PaymentMethod, item.Status, DateOnly.FromDateTime(item.PurchasedAt))).ToList();
    public void AddPurchase(string userId, CreatePurchaseRequest purchase)
    {
        context.Purchases.Add(new PurchaseEntity { UserId = userId, ProductName = purchase.ProductName, Amount = purchase.Amount, IsBusinessKit = purchase.IsBusinessKit, Quantity = purchase.Quantity, PaymentMethod = purchase.PaymentMethod, PurchasedAt = DateTime.UtcNow });
        context.SaveChanges();
    }
    public IReadOnlyList<OrderDto> GetOrders(string userId) => context.Orders.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.OrderedAt).Select(item => new OrderDto(
        item.OrderNumber,
        DateOnly.FromDateTime(item.OrderedAt),
        item.Amount,
        item.ShippingFee,
        item.Amount + item.ShippingFee,
        item.PaymentMethod,
        item.Status ?? "Processing",
        item.CourierName ?? "The Courier Guy",
        string.IsNullOrEmpty(item.TrackingNumber) ? $"TCG-{item.Id:00000}ZA" : item.TrackingNumber,
        item.ShippingStatus ?? "Packed",
        item.EstimatedDelivery != null ? DateOnly.FromDateTime(item.EstimatedDelivery.Value) : DateOnly.FromDateTime(item.OrderedAt.AddDays(3)),
        $"/api/orders/{item.OrderNumber}/invoice"
    )).ToList();

    public void AddOrder(string userId, CreateOrderRequest order)
    {
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var tracking = $"TCG-{Random.Shared.Next(100000, 999999)}ZA";
        context.Orders.Add(new OrderEntity {
            UserId = userId,
            OrderNumber = orderNumber,
            Amount = order.Amount,
            ShippingFee = order.ShippingFee,
            PaymentMethod = order.PaymentMethod,
            OrderedAt = DateTime.UtcNow,
            CourierName = "The Courier Guy",
            TrackingNumber = tracking,
            ShippingStatus = "Placed",
            Status = "Processing",
            EstimatedDelivery = DateTime.UtcNow.AddDays(3)
        });
        context.SaveChanges();
    }
    public IReadOnlyList<RankHistoryDto> GetRankHistory(string userId) => context.RankHistory.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.ChangedAt).Select(item => new RankHistoryDto(item.OldRank, item.NewRank, item.Incentive, DateOnly.FromDateTime(item.ChangedAt))).ToList();
    public void AddRankHistory(string userId, RankHistoryDto history)
    {
        context.RankHistory.Add(new RankHistoryEntity { UserId = userId, OldRank = history.OldRank, NewRank = history.NewRank, Incentive = history.Incentive, ChangedAt = history.Date.ToDateTime(TimeOnly.MinValue) });
        context.SaveChanges();
    }
    public void SetReferralActive(string userId)
    {
        var referral = context.Referrals.FirstOrDefault(item => item.UserId == userId);
        if (referral is null) return;
        referral.Status = "Active";
        context.SaveChanges();
    }
    public IReadOnlyList<PortalMessageDto> GetInbox(string userId) => context.PortalMessages.AsNoTracking()
        .Where(item => item.Recipient == userId || item.Recipient == "all" || item.Recipient == "partners" || (userId == "samkelisojam" && (item.Recipient == "admin" || item.Recipient == "samkelisojam285@gmail.com")))
        .OrderByDescending(item => item.SentAt)
        .Select(item => new PortalMessageDto(item.Id, item.SenderUserId, string.IsNullOrEmpty(item.SenderName) ? (item.SenderUserId == "system" ? "ScaleEngine System" : item.SenderUserId) : item.SenderName, item.Recipient, item.Subject, item.Body, item.SentAt, item.IsRead))
        .ToList();

    public IReadOnlyList<PortalMessageDto> GetSentMessages(string userId) => context.PortalMessages.AsNoTracking()
        .Where(item => item.SenderUserId == userId)
        .OrderByDescending(item => item.SentAt)
        .Select(item => new PortalMessageDto(item.Id, item.SenderUserId, string.IsNullOrEmpty(item.SenderName) ? item.SenderUserId : item.SenderName, item.Recipient, item.Subject, item.Body, item.SentAt, item.IsRead))
        .ToList();

    public OperationResult AddMessage(string senderUserId, string senderName, SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Recipient)) return OperationResult.Failure("Recipient is required.");
        if (string.IsNullOrWhiteSpace(request.Subject)) return OperationResult.Failure("Subject is required.");
        if (string.IsNullOrWhiteSpace(request.Body)) return OperationResult.Failure("Message body is required.");

        context.PortalMessages.Add(new PortalMessageEntity
        {
            SenderUserId = senderUserId,
            SenderName = senderName,
            Recipient = request.Recipient.Trim(),
            Subject = request.Subject.Trim(),
            Body = request.Body.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        context.SaveChanges();
        return OperationResult.Success("Message sent successfully.");
    }

    public OperationResult MarkMessageAsRead(string userId, int messageId)
    {
        var msg = context.PortalMessages.FirstOrDefault(item => item.Id == messageId && (item.Recipient == userId || item.Recipient == "all" || item.Recipient == "partners"));
        if (msg is null) return OperationResult.Failure("Message not found.");
        msg.IsRead = true;
        context.SaveChanges();
        return OperationResult.Success("Message marked as read.");
    }

    public OperationResult DeleteMessage(string userId, int messageId)
    {
        var msg = context.PortalMessages.FirstOrDefault(item => item.Id == messageId && (item.Recipient == userId || item.SenderUserId == userId));
        if (msg is null) return OperationResult.Failure("Message not found.");
        context.PortalMessages.Remove(msg);
        context.SaveChanges();
        return OperationResult.Success("Message deleted.");
    }

    public IReadOnlyList<ZoomMeetingDto> GetMeetings() => context.PortalContent.AsNoTracking().Where(item => item.Type == "meeting").OrderBy(item => item.PublishedAt).Select(item => new ZoomMeetingDto(item.Title, item.Body, item.LinkUrl ?? string.Empty, TimeOnly.FromDateTime(item.PublishedAt), DateOnly.FromDateTime(item.PublishedAt))).ToList();
    public IReadOnlyList<PortalContentDto> GetContent(string type) => context.PortalContent.AsNoTracking().Where(item => item.Type == type).OrderByDescending(item => item.PublishedAt).Select(item => new PortalContentDto(item.Id, item.Type, item.Title, item.Body, item.LinkUrl, item.PublishedAt, item.IsRead)).ToList();
    public void AddContent(CreateContentRequest content)
    {
        context.PortalContent.Add(new PortalContentEntity { Type = content.Type, Title = content.Title, Body = content.Body, LinkUrl = content.LinkUrl, PublishedAt = DateTime.UtcNow });
        context.SaveChanges();
    }

    public KycStatusDto GetKycStatus(string userId)
    {
        var record = context.KycRecords.AsNoTracking().OrderByDescending(item => item.Id).FirstOrDefault(item => item.UserId == userId);
        if (record is null)
        {
            return new KycStatusDto("Unverified", "National ID", "", "", "", null, null, null);
        }
        return new KycStatusDto(
            record.Status,
            record.IdType,
            record.IdNumber,
            record.IdDocumentName,
            record.AddressDocumentName,
            record.ReviewNotes,
            record.SubmittedAt,
            record.ReviewedAt
        );
    }

    public OperationResult SubmitKyc(string userId, SubmitKycRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdNumber))
            return OperationResult.Failure("ID / Passport Number is required.");

        var record = context.KycRecords.OrderByDescending(item => item.Id).FirstOrDefault(item => item.UserId == userId);
        if (record is null)
        {
            record = new KycRecordEntity { UserId = userId };
            context.KycRecords.Add(record);
        }

        record.IdType = string.IsNullOrWhiteSpace(request.IdType) ? "National ID" : request.IdType;
        record.IdNumber = request.IdNumber.Trim();
        record.IdDocumentName = request.IdDocumentFileName ?? (string.IsNullOrEmpty(record.IdDocumentName) ? "id_document.pdf" : record.IdDocumentName);
        record.AddressDocumentName = request.AddressDocumentFileName ?? (string.IsNullOrEmpty(record.AddressDocumentName) ? "proof_of_address.pdf" : record.AddressDocumentName);
        record.Status = "Pending";
        record.SubmittedAt = DateTime.UtcNow;
        record.ReviewNotes = "Verification documents under compliance review.";

        context.SaveChanges();
        return OperationResult.Success("KYC documents submitted successfully. Verification is in progress.");
    }

    public void SetKycStatus(string userId, string status, string? reviewNotes = null)
    {
        var record = context.KycRecords.OrderByDescending(item => item.Id).FirstOrDefault(item => item.UserId == userId);
        if (record is null)
        {
            record = new KycRecordEntity { UserId = userId, IdType = "National ID", IdNumber = "9001015009087", IdDocumentName = "id_approved.pdf", AddressDocumentName = "proof_address.pdf", SubmittedAt = DateTime.UtcNow };
            context.KycRecords.Add(record);
        }
        record.Status = status;
        record.ReviewedAt = DateTime.UtcNow;
        record.ReviewNotes = reviewNotes ?? "KYC verified and approved by compliance.";
        context.SaveChanges();
    }

    public IReadOnlyList<SupportTicketDto> GetSupportTickets(string userId)
    {
        var tickets = context.SupportTickets.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.UpdatedAt).ToList();
        var ticketIds = tickets.Select(t => t.Id).ToList();
        var counts = context.SupportTicketMessages.AsNoTracking().Where(m => ticketIds.Contains(m.TicketId)).GroupBy(m => m.TicketId).ToDictionary(g => g.Key, g => g.Count());

        return tickets.Select(t => new SupportTicketDto(
            t.Id,
            t.TicketNumber,
            t.Subject,
            t.Category,
            t.Status,
            t.Priority,
            t.CreatedAt,
            t.UpdatedAt,
            counts.TryGetValue(t.Id, out var c) ? c : 0
        )).ToList();
    }

    public SupportTicketDetailDto? GetSupportTicketDetails(string userId, int ticketId)
    {
        var ticket = context.SupportTickets.AsNoTracking().FirstOrDefault(item => item.Id == ticketId && item.UserId == userId);
        if (ticket is null) return null;

        var messages = context.SupportTicketMessages.AsNoTracking()
            .Where(m => m.TicketId == ticketId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new TicketMessageDto(m.Id, m.TicketId, m.SenderUserId, m.SenderName, m.IsStaff, m.Message, m.CreatedAt))
            .ToList();

        return new SupportTicketDetailDto(
            ticket.Id,
            ticket.TicketNumber,
            ticket.Subject,
            ticket.Category,
            ticket.Status,
            ticket.Priority,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            messages
        );
    }

    public OperationResult CreateSupportTicket(string userId, string userName, CreateTicketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Subject)) return OperationResult.Failure("Subject is required.");
        if (string.IsNullOrWhiteSpace(request.InitialMessage)) return OperationResult.Failure("Message is required.");

        var ticketNumber = $"TICK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpperInvariant()}";
        var ticket = new SupportTicketEntity
        {
            UserId = userId,
            TicketNumber = ticketNumber,
            Subject = request.Subject.Trim(),
            Category = string.IsNullOrWhiteSpace(request.Category) ? "General" : request.Category,
            Priority = string.IsNullOrWhiteSpace(request.Priority) ? "Medium" : request.Priority,
            Status = "Open",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.SupportTickets.Add(ticket);
        context.SaveChanges();

        context.SupportTicketMessages.Add(new SupportTicketMessageEntity
        {
            TicketId = ticket.Id,
            SenderUserId = userId,
            SenderName = userName,
            IsStaff = false,
            Message = request.InitialMessage.Trim(),
            CreatedAt = DateTime.UtcNow
        });

        // Seed automated instant response from support desk
        context.SupportTicketMessages.Add(new SupportTicketMessageEntity
        {
            TicketId = ticket.Id,
            SenderUserId = "support",
            SenderName = "ScaleEngine Support Desk",
            IsStaff = true,
            Message = $"Hello {userName}, thank you for reaching out to ScaleEngine Support regarding '{ticket.Subject}'. Your ticket #{ticket.TicketNumber} has been received and assigned to our {ticket.Category} team. An agent will follow up shortly.",
            CreatedAt = DateTime.UtcNow.AddSeconds(2)
        });

        context.SaveChanges();
        return OperationResult.Success($"Ticket #{ticketNumber} created successfully.");
    }

    public OperationResult ReplySupportTicket(string userId, string userName, int ticketId, ReplyTicketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message)) return OperationResult.Failure("Reply message cannot be empty.");

        var ticket = context.SupportTickets.FirstOrDefault(item => item.Id == ticketId && item.UserId == userId);
        if (ticket is null) return OperationResult.Failure("Ticket not found.");

        ticket.Status = "In Progress";
        ticket.UpdatedAt = DateTime.UtcNow;

        context.SupportTicketMessages.Add(new SupportTicketMessageEntity
        {
            TicketId = ticketId,
            SenderUserId = userId,
            SenderName = userName,
            IsStaff = false,
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow
        });

        context.SaveChanges();
        return OperationResult.Success("Reply submitted.");
    }

    public OperationResult CloseSupportTicket(string userId, int ticketId)
    {
        var ticket = context.SupportTickets.FirstOrDefault(item => item.Id == ticketId && item.UserId == userId);
        if (ticket is null) return OperationResult.Failure("Ticket not found.");

        ticket.Status = "Closed";
        ticket.UpdatedAt = DateTime.UtcNow;
        context.SaveChanges();
        return OperationResult.Success($"Ticket #{ticket.TicketNumber} has been closed.");
    }

    public IReadOnlyList<AutoShipDto> GetAutoShips(string userId)
    {
        return context.AutoShips.AsNoTracking().Where(a => a.UserId == userId).OrderByDescending(a => a.CreatedAt).Select(a => new AutoShipDto(
            a.Id,
            a.ProductName,
            a.Amount,
            a.Quantity,
            a.Amount * a.Quantity,
            a.FrequencyDays,
            DateOnly.FromDateTime(a.NextRunDate),
            a.PaymentMethod,
            a.Status,
            DateOnly.FromDateTime(a.CreatedAt)
        )).ToList();
    }

    public OperationResult AddAutoShip(string userId, CreateAutoShipRequest request)
    {
        var unitPrice = request.ProductName.Contains("Starter", StringComparison.OrdinalIgnoreCase) ? 499m : (request.ProductName.Contains("Bundle", StringComparison.OrdinalIgnoreCase) ? 799m : 199m);
        var autoShip = new AutoShipEntity
        {
            UserId = userId,
            ProductName = request.ProductName,
            Quantity = Math.Max(1, request.Quantity),
            Amount = unitPrice,
            FrequencyDays = request.FrequencyDays > 0 ? request.FrequencyDays : 30,
            PaymentMethod = string.IsNullOrEmpty(request.PaymentMethod) ? "Card" : request.PaymentMethod,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            NextRunDate = DateTime.UtcNow.AddDays(request.FrequencyDays > 0 ? request.FrequencyDays : 30)
        };
        context.AutoShips.Add(autoShip);
        context.SaveChanges();
        return OperationResult.Success($"Auto-Ship subscription for '{autoShip.ProductName}' activated. Next recurring run on {autoShip.NextRunDate:yyyy-MM-dd}.");
    }

    public OperationResult UpdateAutoShipStatus(string userId, int id, string status)
    {
        var item = context.AutoShips.FirstOrDefault(a => a.Id == id && a.UserId == userId);
        if (item is null) return OperationResult.Failure("Subscription not found.");
        item.Status = status;
        context.SaveChanges();
        return OperationResult.Success($"Subscription status updated to '{status}'.");
    }

    public OperationResult DeleteAutoShip(string userId, int id)
    {
        var item = context.AutoShips.FirstOrDefault(a => a.Id == id && a.UserId == userId);
        if (item is null) return OperationResult.Failure("Subscription not found.");
        context.AutoShips.Remove(item);
        context.SaveChanges();
        return OperationResult.Success("Auto-Ship subscription removed.");
    }

    public IReadOnlyList<TeamChatContactDto> GetChatContacts(string userId)
    {
        var contacts = new List<TeamChatContactDto>();
        
        // Find sponsor (upline mentor)
        var myUser = context.Users.FirstOrDefault(u => u.UserName == userId);
        var myReferral = context.Referrals.FirstOrDefault(r => r.UserId == userId);
        var sponsorId = myReferral?.SponsorId ?? "bhekaragnar";

        if (sponsorId != userId)
        {
            var sponsorUser = context.Users.FirstOrDefault(u => u.UserName == sponsorId);
            var sponsorRef = context.Referrals.FirstOrDefault(r => r.UserId == sponsorId);
            var unread = context.TeamChatMessages.Count(m => m.SenderUserId == sponsorId && m.RecipientUserId == userId && !m.IsRead);
            var lastMsg = context.TeamChatMessages.Where(m => (m.SenderUserId == sponsorId && m.RecipientUserId == userId) || (m.SenderUserId == userId && m.RecipientUserId == sponsorId)).OrderByDescending(m => m.SentAt).FirstOrDefault();

            contacts.Add(new TeamChatContactDto(
                sponsorId,
                $"{sponsorUser?.FirstName ?? "Bheka"} {sponsorUser?.LastName ?? "Luthuli"}".Trim(),
                sponsorRef?.Rank ?? "Leader",
                "Upline Sponsor & Mentor",
                unread,
                lastMsg?.SentAt
            ));
        }

        // Find direct downline partners
        var downlines = context.Referrals.Where(r => r.SponsorId == userId).Take(15).ToList();
        foreach (var dl in downlines)
        {
            var unread = context.TeamChatMessages.Count(m => m.SenderUserId == dl.UserId && m.RecipientUserId == userId && !m.IsRead);
            var lastMsg = context.TeamChatMessages.Where(m => (m.SenderUserId == dl.UserId && m.RecipientUserId == userId) || (m.SenderUserId == userId && m.RecipientUserId == dl.UserId)).OrderByDescending(m => m.SentAt).FirstOrDefault();
            
            contacts.Add(new TeamChatContactDto(
                dl.UserId,
                dl.FullName,
                dl.Rank,
                "Direct Team Partner",
                unread,
                lastMsg?.SentAt
            ));
        }

        return contacts;
    }

    public IReadOnlyList<TeamChatMessageDto> GetChatThread(string userId, string otherUserId)
    {
        var messages = context.TeamChatMessages.AsNoTracking()
            .Where(m => (m.SenderUserId == userId && m.RecipientUserId == otherUserId) || (m.SenderUserId == otherUserId && m.RecipientUserId == userId))
            .OrderBy(m => m.SentAt)
            .ToList();

        // Mark incoming messages as read
        var unread = context.TeamChatMessages.Where(m => m.SenderUserId == otherUserId && m.RecipientUserId == userId && !m.IsRead).ToList();
        if (unread.Count > 0)
        {
            foreach (var u in unread) u.IsRead = true;
            context.SaveChanges();
        }

        return messages.Select(m => new TeamChatMessageDto(
            m.Id,
            m.SenderUserId,
            m.SenderName,
            m.RecipientUserId,
            m.RecipientName,
            m.Message,
            m.SentAt,
            m.SenderUserId == userId
        )).ToList();
    }

    public OperationResult SendChatMessage(string userId, string senderName, SendTeamChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message)) return OperationResult.Failure("Message cannot be empty.");
        if (string.IsNullOrWhiteSpace(request.RecipientUserId)) return OperationResult.Failure("Recipient is required.");

        var recipientRef = context.Referrals.FirstOrDefault(r => r.UserId == request.RecipientUserId);
        var recipientUser = context.Users.FirstOrDefault(u => u.UserName == request.RecipientUserId);
        var recName = recipientRef?.FullName ?? ($"{recipientUser?.FirstName} {recipientUser?.LastName}".Trim());
        if (string.IsNullOrWhiteSpace(recName)) recName = request.RecipientUserId;

        var msg = new TeamChatMessageEntity
        {
            SenderUserId = userId,
            SenderName = senderName,
            RecipientUserId = request.RecipientUserId,
            RecipientName = recName,
            Message = request.Message.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        context.TeamChatMessages.Add(msg);
        context.SaveChanges();

        return OperationResult.Success("Message sent.");
    }
}