using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VitalityPortal.Data;
using VitalityPortal.Models.Auth;
using VitalityPortal.Repositories;
using VitalityPortal.Services;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
	?? throw new InvalidOperationException("JWT configuration is missing.");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PortalDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PortalDatabase")));
builder.Services.AddIdentityCore<ApplicationUser>(options => {
	options.Password.RequiredLength = 8;
	options.Password.RequireDigit = true;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<PortalDbContext>().AddSignInManager();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IMemberRepository, MemberRepository>();
builder.Services.AddSingleton<IWithdrawalRepository, WithdrawalRepository>();
builder.Services.AddScoped<IPortalRepository, PortalRepository>();
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INetworkService, NetworkService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IWalletActivityService, WalletActivityService>();
builder.Services.AddScoped<ICommerceService, CommerceService>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ICredentialService, CredentialService>();
builder.Services.AddScoped<IActivationService, ActivationService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<ICommissionEngine, CommissionEngine>();
builder.Services.AddSingleton<RankCalculationService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<RankCalculationService>());
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddScoped<IAiAssistantService, AiAssistantService>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<VitalityPortal.Models.Payments.PayFastOptions>(builder.Configuration.GetSection(VitalityPortal.Models.Payments.PayFastOptions.SectionName));
builder.Services.Configure<VitalityPortal.Models.Payments.GmailSmtpOptions>(builder.Configuration.GetSection(VitalityPortal.Models.Payments.GmailSmtpOptions.SectionName));
builder.Services.Configure<VitalityPortal.Models.Payments.GoogleAuthOptions>(builder.Configuration.GetSection(VitalityPortal.Models.Payments.GoogleAuthOptions.SectionName));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
		ValidateIssuer = true,
		ValidIssuer = jwtOptions.Issuer,
		ValidateAudience = true,
		ValidAudience = jwtOptions.Audience,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

await DatabaseInitializer.InitializeAsync(app.Services);
app.Run();