using VitalityPortal.Models.Common;
using VitalityPortal.Models.Members;
using VitalityPortal.Models.Portal;

namespace VitalityPortal.Services;

public interface IMemberService
{
    OperationResult Register(string sponsorId, CreateMemberRequest request);
    ReferralDto? GetById(int id);
    OperationResult Update(int id, UpdateReferralRequest request);
    OperationResult Delete(int id);
}