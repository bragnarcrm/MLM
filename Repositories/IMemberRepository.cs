using VitalityPortal.Models.Members;

namespace VitalityPortal.Repositories;

public interface IMemberRepository
{
    void Add(CreateMemberRequest member);
}