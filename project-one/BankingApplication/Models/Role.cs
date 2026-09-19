using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class Role
    {
        public int RoleId { get; private set; }

        [Required]
        [StringLength(30)]
        public string RoleName { get; private set; } = "";

        private Role()
        {
        }

        public Role(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(
                    "Role name cannot be empty.");
            }

            RoleName = roleName.Trim();
        }
    }
}