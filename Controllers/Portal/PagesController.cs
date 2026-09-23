using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VitalityPortal.Controllers;

[AllowAnonymous]
public sealed class PagesController : Controller
{
    private static readonly IReadOnlyDictionary<string, string> PageViews = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
        ["index"] = "Home/index", ["home"] = "Home/index", ["compensation-plan"] = "Home/compensation-plan", ["plan"] = "Home/compensation-plan",
        ["login"] = "Account/login", ["forgot-password"] = "Account/forgot-password", ["vitality-registration"] = "Account/vitality-registration", ["profile"] = "Account/profile",
        ["change-password"] = "Account/change-password", ["change-trans-password"] = "Account/change-trans-password", ["activation"] = "Account/activation", ["payfast-checkout"] = "Account/payfast-checkout",
        ["dashboard"] = "Dashboard/dashboard",
        ["add-member"] = "Network/add-member", ["my-referrals"] = "Network/my-referrals", ["referral-tree"] = "Network/referral-tree", ["tree-view"] = "Network/tree-view",
        ["my-income"] = "Income/my-income", ["my-commission"] = "Income/my-commission", ["direct-referral-bonus"] = "Income/direct-referral-bonus", ["level-bonus"] = "Income/level-bonus", ["level-turnover"] = "Income/level-turnover", ["monthly-salary"] = "Income/monthly-salary", ["rank-bonus"] = "Income/rank-bonus", ["rank-history"] = "Income/rank-history",
        ["tax-irp5"] = "Income/tax-irp5", ["irp5"] = "Income/tax-irp5",
        ["ewallet"] = "Wallet/ewallet", ["withdraw-money"] = "Wallet/withdraw-money", ["withdrawal-summary"] = "Wallet/withdrawal-summary", ["cancelled-withdrawal"] = "Wallet/cancelled-withdrawal", ["payout-settings"] = "Wallet/payout-settings",
        ["shopping"] = "Commerce/shopping", ["purchase-list"] = "Commerce/purchase-list", ["order-list"] = "Commerce/order-list",
        ["product_cart"] = "Commerce/product_cart", ["autoship"] = "Commerce/autoship",
        ["replicated-store"] = "Commerce/replicated-store", ["store"] = "Commerce/replicated-store",
        ["team-chat"] = "Communications/team-chat",
        ["messages"] = "Communications/messages", ["news"] = "Communications/news", ["zoom-meetings"] = "Communications/zoom-meetings",
        ["support"] = "Support/support", ["tutorials"] = "Support/Index", ["rules"] = "Support/Index", ["blog"] = "Support/Index", ["notifications"] = "Support/Index",
        ["ai-assistant"] = "Portal/ai-assistant", ["ai"] = "Portal/ai-assistant",
        ["compliance-audit"] = "Portal/compliance-audit", ["compliance"] = "Portal/compliance-audit",
        ["plan-builder"] = "Portal/plan-builder", ["builder"] = "Portal/plan-builder",
        ["reset-password"] = "Account/change-trans-password", ["payout-update-logs"] = "Wallet/payout-settings"
    };

    [HttpGet("/")]
    public IActionResult Index() => View("~/Views/Home/index.cshtml");

    [HttpGet("{pageName}.html")]
    public IActionResult Render(string pageName)
    {
        return PageViews.TryGetValue(pageName, out var viewName) ? View($"~/Views/{viewName}.cshtml") : NotFound();
    }
}