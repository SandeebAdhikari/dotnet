using EmpManagement;

Console.WriteLine("Enter employee number:");
int employeeNumber = Convert.ToInt32(Console.ReadLine());

Employee? loadedEmployee = Employee.LoadObject(employeeNumber);

Employee empObj;
if (loadedEmployee == null)
{
    Console.WriteLine("Employee file not found. Creating a new employee.");

    Console.WriteLine("Enter employee name:");
    string employeeName = Console.ReadLine() ?? "";

    Console.WriteLine("Enter available leave:");
    int availableLeave = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine("Is the employee active (true/false)?");
    bool isActive = Convert.ToBoolean(Console.ReadLine());

    Console.WriteLine("Enter employee password:");
    string password = Console.ReadLine() ?? "";

    empObj = new Employee
    {
        empNo = employeeNumber,
        empName = employeeName,
        empAvailableLeave = availableLeave,
        empIsActive = isActive,
        empPassword = password
    };

    empObj.SaveObject();
    Console.WriteLine("Employee file created.");
}
else
{
    empObj = loadedEmployee;
}
bool continueWork = true;
while(continueWork)
{
    Console.WriteLine("Employee Details ");
    Console.WriteLine("E No : " + empObj.empNo);
    Console.WriteLine("Name : " + empObj.empName);
    Console.WriteLine("Leaves: " + empObj.empAvailableLeave);
    Console.WriteLine("Salary: " + empObj.empSalary);
    Console.WriteLine("Is Active : " + empObj.empIsActive);
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("Please select from the option:");
    Console.WriteLine("1. Apply Leave");
    Console.WriteLine("2. Apprise Salary");
    Console.WriteLine("3. Edit first Name");
    Console.WriteLine("4. Exit");
    int choice = Convert.ToInt32(Console.ReadLine());

    switch (choice)
    {
        case 1: 
            Console.WriteLine("Enter number of days for leave");
            int leaves = Convert.ToInt32(Console.ReadLine());
            empObj.ApplyLeave(leaves);
            Console.WriteLine("Leave approved");
            break;
        case 2:
            empObj.AppriseSalary();
            Console.WriteLine("Congratulation");
            break;
        case 3:
            Console.WriteLine("Enter New Name");
            string newName = Console.ReadLine() ?? "";
            empObj.empName = newName;
            Console.WriteLine("Name Changed!!");
            break;
        case 4:
            continueWork = false;
            Console.WriteLine("Thank you, see you again");
            Console.WriteLine(empObj.SaveObject());
            break;
        default:
            Console.WriteLine("Please choose the right option!!");
            break;
    }
}
