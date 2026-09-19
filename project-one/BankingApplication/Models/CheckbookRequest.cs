namespace BankingApp.Models
{
    public class CheckbookRequest : ServiceRequest
    {
        public int AccountId { get; private set; }
        private CheckbookRequest(){}
        public CheckbookRequest(
            int customerId,
            int accountId)
            : base(customerId)
        {
            if (accountId <= 0)
            {
                throw new ArgumentException(
                    "Account ID must be valid.");
            }

            this.AccountId = accountId;
        }

        public void MarkCompleted()
        {
            if (this.Status != RequestStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Checkbook request must be approved before it can be completed.");
            }

            this.Status = RequestStatus.Completed;
        }
    }
}