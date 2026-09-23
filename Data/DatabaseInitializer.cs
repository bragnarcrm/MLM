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