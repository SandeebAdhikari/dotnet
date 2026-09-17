namespace BankingApp.Models
{
    public class AccountStatementRequest : ServiceRequest
    {
        public int AccountId { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public AccountStatementRequest(
            int customerId,
            int accountId,
            DateTime startDate,
            DateTime endDate)
            : base(customerId)
        {
            if (accountId <= 0)
            {
                throw new ArgumentException(
                    "Account ID must be valid.");
            }

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

            this.AccountId = accountId;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        public void MarkCompleted()
        {
            if (this.Status != RequestStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Statement request must be approved before completion.");
            }

            this.Status = RequestStatus.Completed;
        }
    }
}