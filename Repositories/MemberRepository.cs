using VitalityPortal.Models.Members;

namespace VitalityPortal.Repositories;

public sealed class MemberRepository : IMemberRepository
{
    private readonly List<CreateMemberRequest> members = [];

    public void Add(CreateMemberRequest member) => members.Add(member);
}