namespace BankingApp.Models
{
    public abstract class AccountServiceRequest : ServiceRequest
    {
        public int AccountId { get; private set; }

        protected AccountServiceRequest()
        {
        }

        protected AccountServiceRequest(int customerId, int accountId)
            : base(customerId)
        {
            if (accountId <= 0)
            {
                throw new ArgumentException(
                    "Account ID must be valid.");
            }

            this.AccountId = accountId;
        }
    }
}
