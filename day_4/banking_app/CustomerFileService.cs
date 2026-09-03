using System.IO;

namespace Banking
{
    public class CustomerFileService
    {
        public void SaveCustomer(Customer customer, Accounts account)
        {
            int accountNumber = 1;

            while (File.Exists("account" + accountNumber + ".txt"))
            {
                accountNumber++;
            }

            string fileName = "account" + accountNumber + ".txt";

            account.AccountNumber = accountNumber;

            FileStream file = new FileStream(
                fileName,
                FileMode.Create,
                FileAccess.Write
            );

            StreamWriter info = new StreamWriter(file);

            #region Customer Information

            info.WriteLine("CUSTOMER INFORMATION");
            info.WriteLine("--------------------");

            info.WriteLine("First Name: " + customer.FirstName);
            info.WriteLine("Last Name: " + customer.LastName);
            info.WriteLine("Username: " + customer.Username);
            info.WriteLine("Password: " + customer.Password);

            #endregion


            info.WriteLine();


            #region Address Information

            info.WriteLine("ADDRESS");
            info.WriteLine("-------");

            info.WriteLine("Street: " + customer.Address.Street);
            info.WriteLine("City: " + customer.Address.City);
            info.WriteLine("State: " + customer.Address.State);
            info.WriteLine("Zip Code: " + customer.Address.ZipCode);

            #endregion


            info.WriteLine();


            #region Account Information

            info.WriteLine("ACCOUNT INFORMATION");
            info.WriteLine("-------------------");

            info.WriteLine("Account Number: 000" + account.AccountNumber);
            info.WriteLine("Account Holder: " + account.AccountHolderName);
            info.WriteLine("Account Type: " + account.AccountType);
            info.WriteLine("Account Balance: $" + account.AccountBalance);
            info.WriteLine("Account Active: " + account.IsActive);

            #endregion


            #region Checking Account Information

            if (account is Checking checking)
            {
                info.WriteLine(
                    "Overdraft Enabled: " + checking.IsODEnabled
                );
            }

            #endregion


            info.Close();
            file.Close();
        }
    }
}