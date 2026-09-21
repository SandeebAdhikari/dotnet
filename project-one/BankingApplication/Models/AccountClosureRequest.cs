namespace BankingApp.Models
{
    public class AccountClosureRequest : AccountServiceRequest
    {
        private AccountClosureRequest()
        {
        }

        public AccountClosureRequest(
            int customerId,
            int accountId)
            : base(customerId, accountId)
        {
        }
    }
}
