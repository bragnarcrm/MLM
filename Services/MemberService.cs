using VitalityPortal.Models.Common;
using VitalityPortal.Models.Members;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public sealed class MemberService(IMemberRepository memberRepository, IPortalRepository portalRepository) : IMemberService
{
    public OperationResult Register(string sponsorId, CreateMemberRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return OperationResult.Failure("Passwords do not match.");
        }

        memberRepository.Add(request);
        portalRepository.AddReferral(new ReferralDto(request.Username, $"{request.FirstName} {request.LastName}", request.Mobile, sponsorId, "Newbie", 0m, "Pending", DateOnly.FromDateTime(DateTime.UtcNow)));
        return OperationResult.Success("Member registered successfully and added to your downline!");
    }

    public ReferralDto? GetById(int id) => portalRepository.GetReferralById(id);

    public OperationResult Update(int id, UpdateReferralRequest request) =>
        portalRepository.UpdateReferral(id, request) ? OperationResult.Success("Member updated successfully.") : OperationResult.Failure("Member not found.");

    public OperationResult Delete(int id) =>
        portalRepository.DeleteReferral(id) ? OperationResult.Success("Member removed successfully.") : OperationResult.Failure("Member not found.");
}