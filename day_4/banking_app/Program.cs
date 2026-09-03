using Banking;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Welcome to Banking System");
    Console.WriteLine("Please Select One of the options to move forward..");
    Console.WriteLine("a. Create an Account");
    Console.WriteLine("b. Check Balance");
    Console.WriteLine("c. Exit");
    Console.Write("Enter Your Choice: ");

    string userChoice = Console.ReadLine().Trim().ToLower();


    if (userChoice == "a")
    {
        #region Customer Detail

        Customer customer = new Customer();

        Console.Write("Enter your first name: ");
        customer.FirstName = Console.ReadLine();

        Console.Write("Enter your last name: ");
        customer.LastName = Console.ReadLine();

        Console.Write("Enter your user name: ");
        customer.Username = Console.ReadLine();

        Console.Write("Enter your password: ");
        customer.Password = Console.ReadLine();

        #endregion


        #region Address Detail

        Address address = new Address();

        Console.Write("Enter your street address: ");
        address.Street = Console.ReadLine();

        Console.Write("Enter your city: ");
        address.City = Console.ReadLine();

        Console.Write("Enter your state: ");
        address.State = Console.ReadLine();

        Console.Write("Enter your zip code: ");
        address.ZipCode = Console.ReadLine();

        customer.Address = address;

        #endregion


        #region Account Type

        Console.WriteLine();
        Console.WriteLine("Please Select Account Type:");
        Console.WriteLine("a. Savings");
        Console.WriteLine("b. Checking");
        Console.WriteLine("c. Loan");
        Console.Write("Enter an account choice (a, b, or c): ");

        string accountChoice = Console.ReadLine().Trim().ToLower();

        switch (accountChoice)
        {
            case "a":

                account = new Savings();

                account.AccountType = TypeOfAccount.Savings;

                break;


            case "b":

                Checking checkingAccount = new Checking();

                checkingAccount.AccountType =
                    TypeOfAccount.Checkings;

                Console.Write(
                    "Do you want overdraft enabled? (y/n): "
                );

                string overdraftChoice =
                    Console.ReadLine().Trim().ToLower();

                if (overdraftChoice == "y")
                {
                    checkingAccount.IsODEnabled = true;
                }
                else
                {
                    checkingAccount.IsODEnabled = false;
                }

                account = checkingAccount;

                break;


            case "c":

                account = new Loans();

                account.AccountType =
                    TypeOfAccount.Loans;

                break;


            default:

                Console.WriteLine(
                    "Invalid Account Choice."
                );

                continue;
        }

        #endregion


        #region Account Detail

        account.AccountHolderName =
            customer.FirstName + " " + customer.LastName;

        account.AccountBalance = 0;

        account.IsActive = true;

        Console.Write(
            "Enter initial deposit amount: $"
        );

        int depositAmount =
            Convert.ToInt32(Console.ReadLine());

        account.Deposit(depositAmount);

        #endregion


        #region Save Customer And Account

        CustomerFileService customerFileService =
            new CustomerFileService();

        customerFileService.SaveCustomer(
            customer,
            account
        );

        #endregion


        #region Display Account

        Console.WriteLine();
        Console.WriteLine(
            "Account Created Successfully"
        );

        Console.WriteLine(
            "----------------------------"
        );

        Console.WriteLine(
            "Account Number: 000" +
            account.AccountNumber
        );

        Console.WriteLine(
            "Account Holder: " +
            account.AccountHolderName
        );

        Console.WriteLine(
            "Account Type: " +
            account.AccountType
        );

        Console.WriteLine(
            "Account Balance: $" +
            account.AccountBalance
        );

        Console.WriteLine(
            "Account Active: " +
            account.IsActive
        );

        if (account is Checking checking)
        {
            Console.WriteLine(
                "Overdraft Enabled: " +
                checking.IsODEnabled
            );
        }

        #endregion
    }


    else if (userChoice == "b")
    {
        #region Check Balance

        if (account == null)
        {
            Console.WriteLine();
            Console.WriteLine(
                "No account has been created yet."
            );
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Account Details");
            Console.WriteLine("----------------");

            Console.WriteLine(
                "Account Number: 000" +
                account.AccountNumber
            );

            Console.WriteLine(
                "Account Holder: " +
                account.AccountHolderName
            );

            Console.WriteLine(
                "Account Type: " +
                account.AccountType
            );

            Console.WriteLine(
                "Available Balance: $" +
                account.AvailableBalance(0)
            );
        }

        #endregion
    }


    else if (userChoice == "c")
    {
        #region Exit

        Console.Write(
            "Are you sure you want to quit? (y/n): "
        );

        string exitChoice =
            Console.ReadLine().Trim().ToLower();

        if (exitChoice == "y")
        {
            Console.WriteLine(
                "Thank you for using Banking System."
            );

            break;
        }
        else
        {
            Console.WriteLine(
                "Returning to Main Menu..."
            );
        }

        #endregion
    }


    else
    {
        Console.WriteLine(
            "Invalid Choice. Please try again."
        );
    }
}