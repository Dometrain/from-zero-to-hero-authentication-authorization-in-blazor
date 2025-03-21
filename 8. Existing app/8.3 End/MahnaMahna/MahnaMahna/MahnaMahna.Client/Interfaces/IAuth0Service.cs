
public interface IAuth0Service
{
    Task<List<Auth0User>> GetUsersAsync();
}