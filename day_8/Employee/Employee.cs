using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmpManagement
{
    class Employee
    {
        public int empNo{get; set;}
        public string empName{get; set;} = "";
        [JsonPropertyName("Monthly Salary")]
        public double empSalary{get; set;}
        [JsonPropertyName("Permanent")]


        public bool empIsActive{get; set;}
        public int  empAvailableLeave{get; set;}
        public string empPassword{get; set;} = "";
        public double AppriseSalary()
        {
            empSalary = empSalary + 2000;
            return empSalary;
        }

        public int ApplyLeave(int days)
        {
            if(days > 5)
            {
                throw new Exception("Sorry cannot apply for more than 5 days");
            }
            empAvailableLeave = empAvailableLeave - days;
            return empAvailableLeave;
        }

        public string SaveObject()
        {

            var options = new JsonSerializerOptions()
            {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                 WriteIndented = true,
                 PropertyNameCaseInsensitive = true

            };

            //save this object
            string data =  JsonSerializer.Serialize(this,options);
            File.WriteAllText(this.empNo + ".json",data);
            return "Object Saved";
        }

        public static Employee? LoadObject(int employeeNumber)
        {
            string fileName = employeeNumber + ".json";

            if (!File.Exists(fileName))
            {
                return null;
            }

            string details = File.ReadAllText(fileName);
            Employee emp = JsonSerializer.Deserialize<Employee>(details)
                ?? throw new JsonException("Unable to deserialize employee data.");
            return emp;

        }

    }
}
