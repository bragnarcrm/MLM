using VitalityPortal.Models.Common;

namespace VitalityPortal.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly Dictionary<string, UserProfile> users = new(StringComparer.OrdinalIgnoreCase) { ["samkelisojam"] = new UserProfile() };

    public UserProfile Get(string userId) => users.TryGetValue(userId, out var user) ? user : new UserProfile { Username = userId };
    public void Register(UserProfile profile) => users[profile.Username] = profile;

    public UserProfile Update(string userId, UserProfile profile)
    {
        var currentUser = Get(userId);
        currentUser.FirstName = profile.FirstName;
        currentUser.LastName = profile.LastName;
        currentUser.Email = profile.Email;
        currentUser.Country = profile.Country;
        currentUser.Mobile = profile.Mobile;
        users[userId] = currentUser;
        return currentUser;
    }
}