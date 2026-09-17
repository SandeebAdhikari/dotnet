using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class User
    {
        private string _username = "";

        public int UserId { get; private set; }

        [Required]
        [StringLength(
            30,
            MinimumLength = 4,
            ErrorMessage = "Username must be between 4 and 30 characters.")]
        [RegularExpression(
            @"^[a-zA-Z0-9_.]+$",
            ErrorMessage = "Username can only contain letters, numbers, underscores, and periods.")]
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value?.Trim() ?? "";
            }
        }

        [Required]
        public string PasswordHash { get; set; } = "";

        public bool IsActive { get; private set; } = true;

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}