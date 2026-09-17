namespace BankingApp.Models
{
    public class AddressChangeRequest : ServiceRequest
    {
        public Address NewAddress { get; private set; }

        public AddressChangeRequest(
            int customerId,
            Address newAddress)
            : base(customerId)
        {
            if (newAddress == null)
            {
                throw new ArgumentNullException(
                    nameof(newAddress),
                    "New address is required.");
            }

            this.NewAddress = newAddress;
        }

        public void MarkCompleted()
        {
            if (this.Status != RequestStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Address change request must be approved before completion.");
            }

            this.Status = RequestStatus.Completed;
        }
    }
}