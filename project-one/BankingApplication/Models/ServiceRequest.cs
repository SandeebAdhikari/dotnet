namespace BankingApp.Models
{
    public abstract class ServiceRequest
    {
        public int RequestId { get; private set; }

        public int CustomerId { get; private set; }

        public DateTime RequestedDate { get; private set; }

        public RequestStatus Status { get; protected set; }

        protected ServiceRequest(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException(
                    "Customer ID must be valid.");
            }

            this.CustomerId = customerId;
            this.RequestedDate = DateTime.UtcNow;
            this.Status = RequestStatus.Pending;
        }

        public void Approve()
        {
            if (this.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending requests can be approved.");
            }

            this.Status = RequestStatus.Approved;
        }

        public void Reject()
        {
            if (this.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending requests can be rejected.");
            }

            this.Status = RequestStatus.Rejected;
        }

        public void Cancel()
        {
            if (this.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending requests can be cancelled.");
            }

            this.Status = RequestStatus.Cancelled;
        }
    }
}