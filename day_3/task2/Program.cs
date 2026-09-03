Console.WriteLine("Welcome to guest management system");
Console.WriteLine("a. New Guest");
Console.WriteLine("b. View Guest Details");

string choice = Console.ReadLine();

switch (choice)
{
    case "a":
        Console.Write("First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Phone Number: ");
        string phone = Console.ReadLine();

        Console.Write("Social Security Number: ");
        string ssn = Console.ReadLine();

        Console.Write("Notes: ");
        string notes = Console.ReadLine();

        string guestDetails =
            "SSNO: " + ssn + "\n" +
            "First Name : " + firstName + "\n" +
            "Last Name  : " + lastName + "\n" +
            "Email      : " + email + "\n" +
            "Phone      : " + phone + "\n" +
            "Notes      : " + notes;

        File.WriteAllText(ssn + ".txt", guestDetails);

        Console.WriteLine();
        Console.WriteLine("Guest information saved.");

        break;

    case "b":
        Console.Write("Please enter SSNno: ");
        string searchSsn = Console.ReadLine();

        string fileName = searchSsn + ".txt";

        if (File.Exists(fileName))
        {
            Console.WriteLine();
            Console.WriteLine(File.ReadAllText(fileName));
        }
        else
        {
            Console.WriteLine("Guest not found.");
        }

        break;

    default:
        Console.WriteLine("Invalid option.");
        break;
}
