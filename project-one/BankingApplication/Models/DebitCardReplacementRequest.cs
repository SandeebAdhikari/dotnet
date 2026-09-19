namespace BankingApp.Models
{
    public class DebitCardReplacementRequest : ServiceRequest
    {
        public int AccountId { get; private set; }

        public DebitCardReplacementReason Reason { get; private set; }
        private DebitCardReplacementRequest(){}
        public DebitCardReplacementRequest(
            int customerId,
            int accountId,
            DebitCardReplacementReason reason)
            : base(customerId)
        {
            if (accountId <= 0)
            {
                throw new ArgumentException(
                    "Account ID must be valid.");
            }

            this.AccountId = accountId;
            this.Reason = reason;
        }

        public void MarkCompleted()
        {
            if (this.Status != RequestStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Debit card replacement request must be approved before completion.");
            }

            this.Status = RequestStatus.Completed;
        }
    }
}