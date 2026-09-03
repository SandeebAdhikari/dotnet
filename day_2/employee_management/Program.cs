Console.WriteLine("Welcome to Employee Management");

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("Select User Type");
    Console.WriteLine("a. Admin");
    Console.WriteLine("b. Employee");
    Console.WriteLine("c. Guest");

    Console.Write("Enter your choice: ");
    string choice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    #region ADMIN
    if (choice == "a")
    {
        Console.Write("Enter username: ");
        string adminUsername = Console.ReadLine() ?? "";

        Console.Write("Enter password: ");
        string adminPassword = Console.ReadLine() ?? "";

        if (adminUsername != "revadmin" || adminPassword != "revadmin$123#")
        {
            Console.WriteLine("Invalid credentials.");
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
            continue;
        }

        bool adminRunning = true;

        while (adminRunning)
        {
            Console.WriteLine();
            Console.WriteLine("ADMIN MENU");
            Console.WriteLine("a. Create new employee");
            Console.WriteLine("b. Change employee details");
            Console.WriteLine("c. Announce Activity");
            Console.WriteLine("d. Delete Employee");
            Console.WriteLine("e. View All Employees");
            Console.WriteLine("f. Back to previous menu");
            Console.WriteLine("g. Exit");

            Console.Write("Enter your choice: ");
            string adminChoice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            switch (adminChoice)
            {
                case "a":
                    Console.WriteLine("Create New Employee");

                    Console.Write("Enter employee name: ");
                    string name = Console.ReadLine() ?? "";

                    Console.Write("Enter employee ID: ");
                    string id = Console.ReadLine() ?? "";

                    Console.Write("Enter employee department: ");
                    string department = Console.ReadLine() ?? "";

                    Console.WriteLine("Employee created successfully.");
                    break;

                case "b":
                    Console.WriteLine("Change Employee Details");

                    Console.Write("Enter employee ID: ");
                    string employeeId = Console.ReadLine() ?? "";

                    Console.Write("Enter new employee name: ");
                    string newName = Console.ReadLine() ?? "";

                    Console.WriteLine("Employee details changed successfully.");
                    break;

                case "c":
                    Console.WriteLine("Announce Activity");

                    Console.Write("Enter announcement: ");
                    string announcement = Console.ReadLine() ?? "";

                    Console.WriteLine("Announcement: " + announcement);
                    break;

                case "d":
                    Console.WriteLine("Delete Employee");

                    Console.Write("Enter employee ID: ");
                    string deleteId = Console.ReadLine() ?? "";

                    Console.WriteLine("Employee deleted successfully.");
                    break;

                case "e":
                    Console.WriteLine("View All Employees");
                    Console.WriteLine("Displaying all employees...");
                    break;

                case "f":
                    adminRunning = false;
                    break;

                case "g":
                    adminRunning = false;
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
    #endregion
    #region EMPLOYEE
    else if (choice == "b")
    {
        Console.Write("Enter username: ");
        string employeeUsername = Console.ReadLine() ?? "";

        Console.Write("Enter password: ");
        string employeePassword = Console.ReadLine() ?? "";

        if (employeeUsername != "revemp2409" || employeePassword != "revadmin$123#emp")
        {
            Console.WriteLine("Invalid credentials.");
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
            continue;
        }

        bool employeeRunning = true;

        while (employeeRunning)
        {
            Console.WriteLine();
            Console.WriteLine("EMPLOYEE MENU");
            Console.WriteLine("a. View my details");
            Console.WriteLine("b. Apply leave");
            Console.WriteLine("c. Submit reimbursement");
            Console.WriteLine("d. View project details");
            Console.WriteLine("e. View today's task and activities");
            Console.WriteLine("f. Previous menu");
            Console.WriteLine("g. Exit");

            Console.Write("Enter your choice: ");
            string employeeChoice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            switch (employeeChoice)
            {
                case "a":
                    Console.WriteLine("Viewing employee details...");
                    break;

                case "b":
                    Console.WriteLine("Apply Leave");

                    Console.Write("Enter reason for leave: ");
                    string reason = Console.ReadLine() ?? "";

                    Console.WriteLine("Leave application submitted.");
                    break;

                case "c":
                    Console.WriteLine("Submit Reimbursement");

                    Console.Write("Enter reimbursement amount: ");
                    string amount = Console.ReadLine() ?? "";

                    Console.WriteLine("Reimbursement submitted.");
                    break;

                case "d":
                    Console.WriteLine("Viewing project details...");
                    break;

                case "e":
                    Console.WriteLine("Viewing today's tasks and activities...");
                    break;

                case "f":
                    employeeRunning = false;
                    break;

                case "g":
                    employeeRunning = false;
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
    #endregion
    #region GUEST
    else if (choice == "c")
    {
        bool guestRunning = true;

        while (guestRunning)
        {
            Console.WriteLine();
            Console.WriteLine("GUEST MENU");
            Console.WriteLine("a. About the organization");
            Console.WriteLine("b. View Open Positions");
            Console.WriteLine("c. Contact information");
            Console.WriteLine("d. Previous menu");
            Console.WriteLine("e. Exit");

            Console.Write("Enter your choice: ");
            string guestChoice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            switch (guestChoice)
            {
                case "a":
                    Console.WriteLine("About the Organization");
                    Console.WriteLine("Welcome to our organization.");
                    break;

                case "b":
                    Console.WriteLine("Open Positions");
                    Console.WriteLine("Software Developer");
                    Console.WriteLine("Business Analyst");
                    break;

                case "c":
                    Console.WriteLine("Contact Information");
                    Console.WriteLine("Email: company@email.com");
                    Console.WriteLine("Phone: 123-456-7890");
                    break;

                case "d":
                    guestRunning = false;
                    break;

                case "e":
                    guestRunning = false;
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
    #endregion
    else
    {
        Console.WriteLine("Invalid choice");
    }
}

Console.WriteLine("Application closed.");
