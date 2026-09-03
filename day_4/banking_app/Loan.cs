namespace Banking
{
    public class Loans: Accounts
    {
        public override double Withdraw(int amount)
        {
            throw new Exception("Sorry withdrawal is not allowed, Please contact Bank!!");
        }
    }
}

