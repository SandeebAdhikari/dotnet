namespace BankingApp.Models
{
    public class UserRole
    {
        public int UserId { get; private set; }

        public int RoleId { get; private set; }

        private UserRole()
        {
        }

        public UserRole(int userId, int roleId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException(
                    "User ID must be valid.");
            }

            if (roleId <= 0)
            {
                throw new ArgumentException(
                    "Role ID must be valid.");
            }

            UserId = userId;
            RoleId = roleId;
        }
    }
}