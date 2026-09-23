using VitalityPortal.Models.Portal;

namespace VitalityPortal.Models.Support;

public sealed record SupportPageViewModel(string Type, string Title, string Description, string EmptyMessage, IReadOnlyList<PortalContentDto> Items);