namespace Banking
{
    class Customer
    {
        public string FirstName{get; set;} = "";
        public string LastName{get; set;} = "";
        public string Username{get; set;} = "";
        public string Password{get; set;} = "";
        public Address Address{get; set; } = new Address();
        
    }
}