using Sync.Domain;
using Sync.Domain.Repositories;
using Sync.Infrastructure.Services;

namespace Sync.Configuration;

public class AuthenticationService(IUserRepository userRepository, CalibreWebService calibre)
{
    public async Task<bool> AuthenticateAsync(User user)
    {
        var cachedUser = userRepository.Get(user.HostUrl, user.Username);
        if (cachedUser != null && cachedUser.Password == user.Password)
            return true;

        if (await calibre.LoginAsync(user))
        {
            if (cachedUser != null)
            {
                userRepository.Update(user);
            }
            else
            {
                userRepository.Add(user);
            }
            return true;
        }

        return false;
    }
}
