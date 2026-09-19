namespace BankingApp.Models
{
    public class AccountStatementRequest : AccountServiceRequest
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }
        private AccountStatementRequest(){}
        public AccountStatementRequest(
            int customerId,
            int accountId,
            DateTime startDate,
            DateTime endDate)
            : base(customerId, accountId)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException(
                    "Start date cannot be after end date.");
            }

            if (endDate > DateTime.UtcNow)
            {
                throw new ArgumentException(
                    "Statement end date cannot be in the future.");
            }

            this.StartDate = startDate;
            this.EndDate = endDate;
        }
    }
}
