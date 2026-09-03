namespace Banking
{
    public class Savings : Accounts
    {
        public override double Withdraw(int amount)
        {
            if(amount < 5000)
            {
                throw new Exception("Sorry you cannot withdraw less that 5000.")
            }
            else
            {
                return base.Withdraw(amount);
            }
        }
    }
}