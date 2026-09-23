namespace VitalityPortal.Models.Account;

public sealed record AccountStatusDto(bool IsActivated, DateTime? ActivatedAt);