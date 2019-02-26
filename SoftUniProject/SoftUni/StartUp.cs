using SoftUni.Data;
using SoftUni.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;


namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
           SoftUniContext context = new SoftUniContext();
            var temp = AddNewAddressToEmployee(context);
            Console.WriteLine(temp);


        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            nakov.Address = address;

            context.SaveChanges();
            var emp = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Select(x => x.Address.AddressText)
                .Take(10)
                .ToArray();

            foreach (var e in emp)
            {
                sb.AppendLine($"{e}");
            }
            return sb.ToString().Trim();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var emp = context.Employees
                .Where(x => x.DepartmentId == 6)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Salary = x.Salary
                })
                .OrderBy(x => x.Salary)
                .ThenByDescending( a => a.FirstName)
                .ToArray();

            foreach (var e in emp)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from Research and Development - ${e.Salary:f2}");
            }
            return sb.ToString().Trim();
        }


        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var empSalary = context.Employees
                .Where(x => x.Salary>50000)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    Salary = x.Salary
                })
                .OrderBy(x => x.FirstName)
                .ToArray();
            foreach (var e in empSalary)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }
            return sb.ToString().Trim();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
                var emp = context.Employees
                    .OrderBy(x => x.EmployeeId)
                    .Select(x => new
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        JobTitle = x.JobTitle,
                        Salary = x.Salary
                    })
                    .ToArray();

                    foreach (var employee in emp)
                    {
                        sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
                    }
                return sb.ToString();
            
        }
    }
}
