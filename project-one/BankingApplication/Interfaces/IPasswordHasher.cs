using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(
            User user,
            string password);

        bool Verify(
            User user,
            string passwordHash,
            string providedPassword);
    }
}