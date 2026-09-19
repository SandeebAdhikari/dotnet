namespace BankingApp.Models
{
    public class DebitCardReplacementRequest : AccountServiceRequest
    {
        public DebitCardReplacementReason Reason { get; private set; }
        private DebitCardReplacementRequest(){}
        public DebitCardReplacementRequest(
            int customerId,
            int accountId,
            DebitCardReplacementReason reason)
            : base(customerId, accountId)
        {
            this.Reason = reason;
        }
    }
}
