using EntityFramework.DB;

EmployeeManagementNewDbContext db = new EmployeeManagementNewDbContext();

var myDepts = from e in db.Depts
                select e;
foreach(var item in myDepts)
{
    Console.WriteLine(item.DeptNo+" " +item.DeptName);


}

var totalDept = (from e in db.Depts
                    select e.DeptNo).Count();
                Console.WriteLine("Total Deparments: " + 4);

var allEmps = from e in db.Employees
                orderby e.EmpDesignation
                select e;

foreach(var item in allEmps)
{
    Console.WriteLine(item.EmpNo + " " + item.EmpName+" " + item.EmpDesignation);
}


Console.WriteLine("Enter empno to view details:");
int eno = Convert.ToInt32(Console.ReadLine());

var details = (from e in db.Employees
                where )