using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Support;
using VitalityPortal.Repositories;

namespace VitalityPortal.Controllers;

[AllowAnonymous]
public sealed class SupportController(IPortalRepository repository) : Controller
{
    [HttpGet("tutorials.html")]
    public IActionResult Tutorials() => Page("tutorial", "Tutorials", "Learn how to use your Vitality partner portal.", "No tutorials are available yet.");

    [HttpGet("rules.html")]
    public IActionResult Rules() => Page("rules", "Rules & Regulations", "Review the current partner network rules and regulations.", "No rules have been published yet.");

    [HttpGet("notifications.html")]
    public IActionResult Notifications() => Page("notification", "Notifications", "Review your account and network notifications.", "You have no notifications.");

    [HttpGet("blog.html")]
    public IActionResult Blog() => Page("blog", "Blog", "Read the latest Vitality partner network updates.", "No blog posts are available yet.");

    private IActionResult Page(string type, string title, string description, string emptyMessage) => View("~/Views/Support/Index.cshtml", new SupportPageViewModel(type, title, description, emptyMessage, repository.GetContent(type)));
}