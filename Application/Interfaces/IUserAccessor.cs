namespace Application.Interfaces
{
    public interface IUserAccessor
    {
        // Method to get the username of the currently authenticated user
        string GetUserName();
    }
}