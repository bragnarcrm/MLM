using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VitalityPortal.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS Referrals (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT NOT NULL, FullName TEXT NOT NULL, Mobile TEXT NOT NULL, SponsorId TEXT NOT NULL, Rank TEXT NOT NULL, MonthlySales TEXT NOT NULL, Status TEXT NOT NULL, JoinedAt TEXT NOT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS Withdrawals (Id INTEGER PRIMARY KEY AUTOINCREMENT, Amount TEXT NOT NULL, PaymentMethod TEXT NOT NULL, RequestedAt TEXT NOT NULL, Status TEXT NOT NULL, Fee TEXT NOT NULL);");
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Withdrawals ADD COLUMN UserId TEXT NOT NULL DEFAULT 'samkelisojam';"); } catch { }
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS PayoutSettings (Id INTEGER PRIMARY KEY AUTOINCREMENT, AccountHolder TEXT NOT NULL, AccountNumber TEXT NOT NULL, BankName TEXT NOT NULL, AccountType TEXT NOT NULL, Address TEXT NOT NULL, Country TEXT NOT NULL, PostalCode TEXT NOT NULL);");
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE PayoutSettings ADD COLUMN UserId TEXT NOT NULL DEFAULT 'samkelisojam';"); } catch { }
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS PortalMessages (Id INTEGER PRIMARY KEY AUTOINCREMENT, SenderUserId TEXT NOT NULL DEFAULT 'system', SenderName TEXT NOT NULL DEFAULT 'System', Recipient TEXT NOT NULL, Subject TEXT NOT NULL, Body TEXT NOT NULL, SentAt TEXT NOT NULL, IsRead INTEGER NOT NULL);");
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE PortalMessages ADD COLUMN SenderUserId TEXT NOT NULL DEFAULT 'system';"); } catch { }
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE PortalMessages ADD COLUMN SenderName TEXT NOT NULL DEFAULT 'System';"); } catch { }
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS Commissions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Category TEXT NOT NULL, EarnedAt TEXT NOT NULL, FromUserId TEXT NOT NULL, Name TEXT NOT NULL, Amount TEXT NOT NULL, Vat TEXT NOT NULL, Status TEXT NOT NULL, Level INTEGER NULL);");
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Commissions ADD COLUMN OwnerUserId TEXT NOT NULL DEFAULT 'samkelisojam';"); } catch { }
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS Purchases (Id INTEGER PRIMARY KEY AUTOINCREMENT, ProductName TEXT NOT NULL, Amount TEXT NOT NULL, IsBusinessKit INTEGER NOT NULL, Quantity INTEGER NOT NULL, PaymentMethod TEXT NOT NULL, Status TEXT NOT NULL, PurchasedAt TEXT NOT NULL);");
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Purchases ADD COLUMN UserId TEXT NOT NULL DEFAULT 'samkelisojam';"); } catch { }
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS Orders (Id INTEGER PRIMARY KEY AUTOINCREMENT, OrderNumber TEXT NOT NULL, Amount TEXT NOT NULL, ShippingFee TEXT NOT NULL, PaymentMethod TEXT NOT NULL, Status TEXT NOT NULL, OrderedAt TEXT NOT NULL);");
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders ADD COLUMN UserId TEXT NOT NULL DEFAULT 'samkelisojam';"); } catch { }
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders ADD COLUMN CourierName TEXT NULL DEFAULT 'The Courier Guy';"); } catch { }
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders ADD COLUMN TrackingNumber TEXT NULL DEFAULT 'TCG-89102ZA';"); } catch { }
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders ADD COLUMN ShippingStatus TEXT NULL DEFAULT 'Packed';"); } catch { }
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders ADD COLUMN EstimatedDelivery TEXT NULL;"); } catch { }
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS PortalContent (Id INTEGER PRIMARY KEY AUTOINCREMENT, Type TEXT NOT NULL, Title TEXT NOT NULL, Body TEXT NOT NULL, LinkUrl TEXT NULL, PublishedAt TEXT NOT NULL, IsRead INTEGER NOT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS KycRecords (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT NOT NULL, IdType TEXT NOT NULL, IdNumber TEXT NOT NULL, IdDocumentName TEXT NOT NULL, AddressDocumentName TEXT NOT NULL, Status TEXT NOT NULL, ReviewNotes TEXT NULL, SubmittedAt TEXT NULL, ReviewedAt TEXT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS SupportTickets (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT NOT NULL, TicketNumber TEXT NOT NULL, Subject TEXT NOT NULL, Category TEXT NOT NULL, Status TEXT NOT NULL, Priority TEXT NOT NULL, CreatedAt TEXT NOT NULL, UpdatedAt TEXT NOT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS SupportTicketMessages (Id INTEGER PRIMARY KEY AUTOINCREMENT, TicketId INTEGER NOT NULL, SenderUserId TEXT NOT NULL, SenderName TEXT NOT NULL, IsStaff INTEGER NOT NULL, Message TEXT NOT NULL, CreatedAt TEXT NOT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS AutoShips (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT NOT NULL, ProductName TEXT NOT NULL, Amount TEXT NOT NULL, Quantity INTEGER NOT NULL, FrequencyDays INTEGER NOT NULL, NextRunDate TEXT NOT NULL, PaymentMethod TEXT NOT NULL, Status TEXT NOT NULL, CreatedAt TEXT NOT NULL, LastRunDate TEXT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS TeamChatMessages (Id INTEGER PRIMARY KEY AUTOINCREMENT, SenderUserId TEXT NOT NULL, SenderName TEXT NOT NULL, RecipientUserId TEXT NOT NULL, RecipientName TEXT NOT NULL, Message TEXT NOT NULL, SentAt TEXT NOT NULL, IsRead INTEGER NOT NULL);");
        await context.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS TransactionPasswordLogs (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT NOT NULL, Action TEXT NOT NULL, Remarks TEXT NOT NULL, Status TEXT NOT NULL, IpAddress TEXT NOT NULL, OccurredAt TEXT NOT NULL);");

        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await users.FindByNameAsync("samkelisojam") is null) {
            var user = new ApplicationUser {
                UserName = "samkelisojam",
                Email = "samkelisojam285@gmail.com",
                FirstName = "samkeliso",
                LastName = "ndlangamandla",
                Country = "South Africa",
                PhoneNumber = "768313287",
                EmailConfirmed = true,
                IsActivated = true,
                Rank = "Builder"
            };
            var result = await users.CreateAsync(user, "password1");
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        // Add 3 dedicated downline users under samkelisojam
        var downlineMembers = new[]
        {
            new { Username = "nomsa_khumalo", First = "Nomsa", Last = "Khumalo", Email = "nomsa.khumalo@gmail.com", Mobile = "+27712345601", Rank = "Builder", Sales = 1450m },
            new { Username = "thabo_molefe", First = "Thabo", Last = "Molefe", Email = "thabo.molefe@gmail.com", Mobile = "+27823456702", Rank = "Newbie", Sales = 890m },
            new { Username = "sipho_dlamini", First = "Sipho", Last = "Dlamini", Email = "sipho.dlamini@gmail.com", Mobile = "+27734567803", Rank = "Builder", Sales = 2350m }
        };

        foreach (var m in downlineMembers)
        {
            if (await users.FindByNameAsync(m.Username) is null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = m.Username,
                    Email = m.Email,
                    FirstName = m.First,
                    LastName = m.Last,
                    Country = "South Africa",
                    PhoneNumber = m.Mobile,
                    EmailConfirmed = true,
                    IsActivated = true,
                    Rank = m.Rank
                };
                await users.CreateAsync(newUser, "password1");
            }

            var refExists = await context.Referrals.AnyAsync(r => r.UserId == m.Username);
            if (!refExists)
            {
                context.Referrals.Add(new ReferralEntity
                {
                    UserId = m.Username,
                    FullName = $"{m.First} {m.Last}",
                    Mobile = m.Mobile,
                    SponsorId = "samkelisojam",
                    Rank = m.Rank,
                    MonthlySales = m.Sales,
                    Status = "Active",
                    JoinedAt = DateTime.UtcNow.Date.AddDays(-2)
                });
            }
        }
        await context.SaveChangesAsync();

        var referralCount = await context.Referrals.CountAsync();
        if (referralCount < 50) {
            var names = new[] { "Amina", "Brian", "Clara", "David", "Elena", "Fikile", "Grace", "Hassan", "Ivy", "Jabu" };
            for (var index = referralCount + 1; index <= 50; index++) {
                var sponsorId = index <= 10 ? "samkelisojam" : $"demo-member-{((index - 1) % 10) + 1:00}";
                context.Referrals.Add(new ReferralEntity {
                    UserId = $"demo-member-{index:00}",
                    FullName = $"{names[(index - 1) % names.Length]} Demo {index:00}",
                    Mobile = $"+2782000{index:000}",
                    SponsorId = sponsorId,
                    Rank = index % 3 == 0 ? "Builder" : "Newbie",
                    MonthlySales = index % 4 == 0 ? 720m : index * 25m,
                    Status = index % 5 == 0 ? "Inactive" : "Active",
                    JoinedAt = DateTime.UtcNow.Date.AddDays(-index)
                });
            }
            await context.SaveChangesAsync();
        }

        if (!await context.PortalMessages.AnyAsync()) {
            context.PortalMessages.AddRange(
                new PortalMessageEntity {
                    SenderUserId = "system",
                    SenderName = "ScaleEngine System",
                    Recipient = "samkelisojam",
                    Subject = "Welcome to ScaleEngine Partner Platform!",
                    Body = "Welcome to your official ScaleEngine partner backoffice. Review your downline tree, launch starter packages, and invite partners with your referral link.",
                    SentAt = DateTime.UtcNow.AddDays(-2),
                    IsRead = true
                },
                new PortalMessageEntity {
                    SenderUserId = "bhekaragnar",
                    SenderName = "Bheka Luthuli (Upline Mentor)",
                    Recipient = "samkelisojam",
                    Subject = "Team Strategy & Weekly Training Meeting",
                    Body = "Hi Samkeliso, congratulations on your recent team additions! Make sure your direct downlines know how to complete their KYC and activate their accounts. Let me know if you need help with strategy.",
                    SentAt = DateTime.UtcNow.AddHours(-14),
                    IsRead = false
                },
                new PortalMessageEntity {
                    SenderUserId = "finance",
                    SenderName = "ScaleEngine Commission Desk",
                    Recipient = "samkelisojam",
                    Subject = "Direct Referral Bonus Credited (R49.90 + VAT)",
                    Body = "Your direct referral bonus from commtestuser2's purchase has been processed and logged to your wallet balance.",
                    SentAt = DateTime.UtcNow.AddHours(-3),
                    IsRead = false
                }
            );
            await context.SaveChangesAsync();
        }
    }
}