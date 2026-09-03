using Banking;

Savings sav = new Savings();

sav.AccountBalance = 10000;
Console.WriteLine("Available balance" + sav.AccountBalance);

try
{
    sav.Withdraw(7000);
}
catch(Exception es)
{
    Console.WriteLine(es.Message);
}

Console.WriteLine("Available Balance" + sav.AccountBalance);

