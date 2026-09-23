using VitalityPortal.Models.Common;

namespace VitalityPortal.Repositories;

public interface IUserRepository
{
    UserProfile Get(string userId);
    void Register(UserProfile profile);
    UserProfile Update(string userId, UserProfile profile);
}