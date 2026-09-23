using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Data;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class InvoiceController(PortalDbContext context, IUserContextService users) : ControllerBase
{
    [HttpGet("{orderNumber}/invoice")]
    public IActionResult ViewInvoice(string orderNumber)
    {
        var userId = users.GetRequiredUserId(User);
        var order = context.Orders.FirstOrDefault(o => o.OrderNumber == orderNumber && o.UserId == userId);
        if (order is null) return NotFound("Order not found.");

        var user = context.Users.FirstOrDefault(u => u.UserName == userId);
        var purchases = context.Purchases.Where(p => p.UserId == userId).OrderByDescending(p => p.PurchasedAt).Take(5).ToList();

        var subtotal = Math.Round(order.Amount / 1.15m, 2);
        var vatAmount = Math.Round(order.Amount - subtotal, 2);
        var total = order.Amount + order.ShippingFee;

        var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <title>Tax Invoice - {order.OrderNumber} | ScaleEngine</title>
    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"">
    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css"">
    <style>
        body {{ background: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; color: #1e293b; padding: 40px 20px; }}
        .invoice-card {{ max-width: 800px; margin: 0 auto; background: #fff; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 10px 30px rgba(0,0,0,.06); padding: 40px; }}
        .invoice-header {{ display: flex; justify-content: space-between; border-bottom: 2px solid #0f172a; padding-bottom: 24px; margin-bottom: 28px; }}
        .brand-name {{ font-size: 24px; font-weight: 800; color: #0f172a; display: flex; align-items: center; gap: 8px; }}
        .invoice-title {{ text-align: right; }}
        .invoice-title h1 {{ font-size: 28px; font-weight: 800; color: #0f172a; margin: 0; }}
        .invoice-meta {{ display: grid; grid-template-columns: 1fr 1fr; gap: 24px; margin-bottom: 30px; }}
        .meta-box h4 {{ font-size: 12px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: .05em; margin-bottom: 8px; }}
        .meta-box p {{ margin: 0; font-size: 14px; line-height: 1.5; }}
        .table thead th {{ background: #0f172a; color: #fff; font-size: 12px; text-transform: uppercase; letter-spacing: .05em; border: none; padding: 12px; }}
        .table tbody td {{ padding: 14px 12px; vertical-align: middle; border-bottom: 1px solid #e2e8f0; }}
        .totals-table {{ max-width: 320px; margin-left: auto; margin-top: 20px; }}
        .totals-table td {{ padding: 6px 12px; }}
        .grand-total {{ font-size: 18px; font-weight: 800; color: #0f172a; border-top: 2px solid #0f172a; }}
        .badge-status {{ display: inline-block; padding: 4px 10px; border-radius: 999px; font-size: 11px; font-weight: 700; text-transform: uppercase; background: #dcfce7; color: #15803d; }}
        .print-bar {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; max-width: 800px; margin: 0 auto 20px; }}
        @media print {{
            body {{ background: #fff; padding: 0; }}
            .invoice-card {{ border: none; box-shadow: none; padding: 0; width: 100%; max-width: 100%; }}
            .print-bar {{ display: none; }}
        }}
    </style>
</head>
<body>
    <div class=""print-bar"">
        <a href=""javascript:window.close()"" class=""btn btn-outline-secondary btn-sm""><i class=""bi bi-arrow-left""></i> Back to Portal</a>
        <button onclick=""window.print()"" class=""btn btn-dark btn-sm""><i class=""bi bi-printer-fill""></i> Print / Download PDF</button>
    </div>

    <div class=""invoice-card"">
        <div class=""invoice-header"">
            <div>
                <div class=""brand-name""><i class=""bi bi-cpu-fill""></i> ScaleEngine</div>
                <div class=""text-muted small mt-1"">ScaleEngine Enterprise Platform Ltd</div>
                <div class=""text-muted small"">VAT Reg: 4920281928 &bull; Tax Number: 902910293</div>
                <div class=""text-muted small"">Sandton, Johannesburg, South Africa</div>
            </div>
            <div class=""invoice-title"">
                <h1>TAX INVOICE</h1>
                <div class=""text-muted small mt-1"">Invoice #: <strong>{order.OrderNumber}</strong></div>
                <div class=""text-muted small"">Date: <strong>{order.OrderedAt:yyyy-MM-dd HH:mm}</strong></div>
                <div class=""mt-2""><span class=""badge-status"">PAID ({order.PaymentMethod})</span></div>
            </div>
        </div>

        <div class=""invoice-meta"">
            <div class=""meta-box"">
                <h4>Billed To (Partner):</h4>
                <p><strong>{user?.FirstName} {user?.LastName}</strong> ({userId})</p>
                <p>{user?.Email}</p>
                <p>Phone: {user?.PhoneNumber}</p>
                <p>{user?.Country ?? "South Africa"}</p>
            </div>
            <div class=""meta-box"">
                <h4>Shipping & Logistics:</h4>
                <p>Courier: <strong>{order.CourierName}</strong></p>
                <p>Status: <strong class=""text-success"">{order.ShippingStatus}</strong></p>
                <p>Tracking #: <code>{order.TrackingNumber}</code></p>
                <p>Payment: {order.PaymentMethod}</p>
            </div>
        </div>

        <table class=""table"">
            <thead>
                <tr>
                    <th>Item Description</th>
                    <th class=""text-center"">Qty</th>
                    <th class=""text-end"">Unit Price (Excl. VAT)</th>
                    <th class=""text-end"">Total (Incl. VAT)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <strong>Partner Order Package ({order.OrderNumber})</strong>
                        <div class=""text-muted small"">Official ScaleEngine Business / Wellness Bundle</div>
                    </td>
                    <td class=""text-center"">1</td>
                    <td class=""text-end"">R{subtotal:F2}</td>
                    <td class=""text-end"">R{order.Amount:F2}</td>
                </tr>
            </tbody>
        </table>

        <div class=""totals-table"">
            <table class=""w-100"">
                <tr>
                    <td class=""text-muted"">Subtotal (Excl. VAT):</td>
                    <td class=""text-end"">R{subtotal:F2}</td>
                </tr>
                <tr>
                    <td class=""text-muted"">Standard VAT (15%):</td>
                    <td class=""text-end"">R{vatAmount:F2}</td>
                </tr>
                <tr>
                    <td class=""text-muted"">Shipping & Delivery:</td>
                    <td class=""text-end"">R{order.ShippingFee:F2}</td>
                </tr>
                <tr class=""grand-total"">
                    <td>Total Amount:</td>
                    <td class=""text-end"">R{total:F2}</td>
                </tr>
            </table>
        </div>

        <div class=""text-center text-muted small mt-5 pt-4 border-top"">
            Thank you for partnering with ScaleEngine. This is a computer-generated tax invoice and requires no physical signature.
        </div>
    </div>
</body>
</html>";

        return Content(html, "text/html");
    }
}
