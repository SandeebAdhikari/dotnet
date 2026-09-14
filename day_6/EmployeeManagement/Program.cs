using EmployeeManagement;

List<Employee> empList = new List<Employee>();

empList.Add(new Employee { empNo = 101, empName = "Michael Jackson", empSalary = 5000, empIsPermenant = true, empDepartmentNo = 10 });
empList.Add(new Employee { empNo = 102, empName = "Donald Trump", empSalary = 2000, empIsPermenant = true, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 103, empName = "Peter Parker", empSalary = 3000, empIsPermenant = false, empDepartmentNo = 30 });
empList.Add(new Employee { empNo = 104, empName = "Shaun Michael", empSalary = 5000, empIsPermenant = true, empDepartmentNo = 10 });
empList.Add(new Employee { empNo = 105, empName = "Mary Jane", empSalary = 400, empIsPermenant = false, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 106, empName = "Britney Spears", empSalary = 6000, empIsPermenant = true, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 107, empName = "Drew Barrymore", empSalary = 9000, empIsPermenant = true, empDepartmentNo = 30 });
empList.Add(new Employee { empNo = 108, empName = "Undertaker", empSalary = 12000, empIsPermenant = false, empDepartmentNo = 30 });
empList.Add(new Employee { empNo = 109, empName = "Sunny Haynes", empSalary = 17000, empIsPermenant = true, empDepartmentNo = 30 });
empList.Add(new Employee { empNo = 110, empName = "Big Show", empSalary = 55000, empIsPermenant = false, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 111, empName = "Randy Orton", empSalary = 75000, empIsPermenant = true, empDepartmentNo = 10 });
empList.Add(new Employee { empNo = 112, empName = "Emma Watson", empSalary = 8000, empIsPermenant = true, empDepartmentNo = 10 });
empList.Add(new Employee { empNo = 113, empName = "Robert Downey", empSalary = 22000, empIsPermenant = true, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 114, empName = "Chris Evans", empSalary = 18000, empIsPermenant = false, empDepartmentNo = 30 });
empList.Add(new Employee { empNo = 115, empName = "Scarlett Johansson", empSalary = 25000, empIsPermenant = true, empDepartmentNo = 10 });
empList.Add(new Employee { empNo = 116, empName = "Tom Holland", empSalary = 11000, empIsPermenant = false, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 117, empName = "Jennifer Lawrence", empSalary = 28000, empIsPermenant = true, empDepartmentNo = 30 });
empList.Add(new Employee { empNo = 118, empName = "Will Smith", empSalary = 32000, empIsPermenant = true, empDepartmentNo = 10 });
empList.Add(new Employee { empNo = 119, empName = "Margot Robbie", empSalary = 15000, empIsPermenant = false, empDepartmentNo = 20 });
empList.Add(new Employee { empNo = 120, empName = "Dwayne Johnson", empSalary = 45000, empIsPermenant = true, empDepartmentNo = 30 });

foreach (var employee in empList)
{
    Console.WriteLine($"Employee No: {employee.empNo}");
    Console.WriteLine($"Name: {employee.empName}");
    Console.WriteLine($"Salary: {employee.empSalary}");
    Console.WriteLine($"Permanent: {employee.empIsPermenant}");
    Console.WriteLine($"Department No: {employee.empDepartmentNo}");
    Console.WriteLine("-----------------------------------");
}
