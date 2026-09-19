namespace BankingApp.Models
{
    public class AddressChangeRequest : ServiceRequest
    {
        public Address NewAddress { get; private set; } = null!;
        private AddressChangeRequest(){}
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
    }
}