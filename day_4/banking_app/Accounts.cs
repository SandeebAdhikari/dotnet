namespace Banking
{
    public enum TypeOfAccount
    {
        Savings,
        Checkings,
        Loans
    }
    public abstract class Accounts
    {
        #region Properties
        public int AccountNumber { get; set; }
        public string AccountHolderName{get; set;} = "";
        public TypeOfAccount AccountType { get; set; }
        public double AccountBalance { get; set; }
        public bool IsActive { get; set; }
        #endregion


        public virtual double Withdraw(int amount)
        {
            //we should still have common validations palced here
            //eg none of the account can withdraw negative amount
            if(amount < 100)
            {
                throw new Exception("Sorry you cannot withdraw less than $100");
            }
            else
            {
                AccountBalance -= amount;
                return AccountBalance;
            }
            
        }

        public double Deposit(int amount)
        {
            AccountBalance = AccountBalance + amount;
            return AccountBalance;
        }

        public double AvailableBalance(int amount)
        {
            return AccountBalance;
        }
    }
}