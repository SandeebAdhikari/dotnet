Console.WriteLine("Hello, World!");

#region Variables

// string firstName = "Sandeeb";
// string designation = "Software Engineer";

// int age = 33;

// double salary = 100000;
// double height = 5.6;

// bool isMarried = true;

// System.Console.WriteLine($"My name is {firstName}, I am a {designation}, my age is {age}, my salary is {salary}.");

#endregion

#region Input
// System.Console.WriteLine("!~~~~~~~~~~~~~Welcome to CITI Bank~~~~~~~~~~~~~~~~");
// string name = string.Empty;
// System.Console.WriteLine("Please enter your name: ");

// string city = string.Empty;
// System.Console.WriteLine("Please enter your city: ");
// city = System.Console.ReadLine();

// int age  = 0;
// System.Console.WriteLine("Please enter your age: ");
// age = Convert.ToInt32(System.Console.ReadLine());

// bool isMarried;
// System.Console.WriteLine("Are you married? (Yes/ No): ");;
// isMarried = Convert.ToBoolean(System.Console.ReadLine());

// Console.WriteLine("Thank you for the details. We will process you result and get back to you.");

#region: Output
// CONDITION
//a. Name
// Name should be empty or NULL, and it should contain more than 3 character, and less than 25 characters.
// Any case name should be converted to standard case

//b. City
// City shoud not be empty or null, and can only be one of the following ciites: New York, San Francisco, 

//c. Age
//age should be between 18 and 60
//cannot be a negative number or zero
//cannot be a decimal number
//cannot be left blank

// d. isMarried
// isMarried should be either true or false,  and is Mandatory to provide a value for this field. 

// bool validationsPassed = true;

// if(name == null || name == string.Empty || name.Length < 3 || name.Length > 25)
// {
//     validationsPassed = false;
//     System.Console.WriteLine("Invalid name. Please enter a valid name with 3 to 25 characters.");
// }
// else
// {
//     name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

// if(city == null || city == string.Empty || (city != "New York" && city != "Los Angeles" && city != "New York" && city!= "San Francisco"))
// {
//     System.Console.WriteLine("Invalid city, Please enter a valid city");
// }

// if( age < 18 || age > 60) 
// {
//    System.Console.WriteLine("Invalid age. Please enter an age between 18 and 60: ");
// }else if( age < 0)
// {
//     System.Console.WriteLine("Invalid age. Please enter an age between 18 and 60: ");
// }else if( age % 1 != 0){
//     validationsPassed = false;
//     System.Console.WriteLine("Invalid age. Age cannot be zero or negative.");
// }else
// {
//     validationsPassed = true;
// }
    
// if(isMarried != true && isMarried != false)
// {
//     System.Console.WriteLine("Invalid marital status. Please enter either true or false.");
// }
// else
// {
//     validationsPassed = true;
// }

// if (validationsPassed)
// {
//     System.Console.WriteLine($"Approve !! Thank you {name} from {city}, age {age}, for providing your details.");
// }
// else
// {
//     System.Console.WriteLine($"Rejected !! {name} Please correct the errors and try again.");
// }
// }
#endregion


#endregion

#region SWITCH
bool continueTransaction = true;

while (continueTransaction)
{
    Console.clear();

System.Console.WriteLine("1. Create Account");
System.Console.WriteLine("2. Check Balance");
System.Console.WriteLine("3. Withdraw Balance");
System.Console.WriteLine("4. Deposit Balance");
System.Console.WriteLine("5. Transfer Balance");
System.Console.WriteLine("6. Transfer Balance History");
System.Console.WriteLine("7. Change ATM PIN");
System.Console.WriteLine("8. Request Loan");
System.Console.WriteLine("9. Exit");

int userChoice = Convert.ToInt32(System.Console.ReadLine());

switch (userChoice)
{
     case 1: 
        Console.WriteLine("Create new account, will collect details and process account creation.");
        break;
     case 2:
        Console.WriteLine("Checking account balance");
        break;
     case 3:
        Console.WriteLine("Checking account balance");
        break;
     case 4:
        Console.WriteLine("Withdraw Balance");
        break;
     case 5:
        Console.WriteLine("Deposit Balance");
        break;
     case 6:
        Console.WriteLine("Transfer Balance");
        break;
     case 7:
        Console.WriteLine("Change ATM PIN");
        break;
     case 8:
        Console.WriteLine("Request Loan");
        break;
     case 9:
        Console.WriteLine("Exit");
        break;
}
Console.WriteLine("Press any key to continue..or 0 to exit.");
Console.ReadKey();
}
#endregion