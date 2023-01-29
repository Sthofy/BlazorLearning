namespace BlazorEcommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        public Task<ServiceResponse<int>> Register(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExist(string email)
        {
            throw new NotImplementedException();
        }
    }
}
