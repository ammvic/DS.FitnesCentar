namespace FitnessCentar.Members.Interface
{
    public interface IAuthService
    {
        string GenerateJwtToken(int userId);
    }
}
