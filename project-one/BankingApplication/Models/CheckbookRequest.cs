namespace BankingApp.Models
{
    public class CheckbookRequest : AccountServiceRequest
    {
        private CheckbookRequest(){}
        public CheckbookRequest(
            int customerId,
            int accountId)
            : base(customerId, accountId)
        {
        }
    }
}
