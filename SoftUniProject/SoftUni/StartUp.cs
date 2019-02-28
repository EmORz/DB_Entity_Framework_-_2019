using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace SoftUni
{
    public class StartUp
    {
        public static StringBuilder sbGlobal = new StringBuilder();
        public static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var temp = IncreaseSalaries(context);
                Console.WriteLine(temp);
            } 
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            string[] dep = {"Engineering", "Tool Design", "Marketing", "Information Services"};
            var employees = context.Employees
                .Where(e => dep.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(x => x.LastName);
            
            foreach (var emp in employees)
            {
                emp.Salary *= 1.12m;
            }

            context.SaveChanges();
            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:f2})");
            }
            return sb.ToString().Trim();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var project in context.Projects.OrderByDescending(x => x.StartDate).Take(10).OrderBy(x => x.Name))
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().Trim();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var deletingTown = context.Towns.FirstOrDefault(x => x.Name == "Seattle");

            foreach (var emp in context.Employees.Where( x => x.Address.TownId == deletingTown.TownId))
            {
                emp.Address = null;
            }

            var deletingAdres = context.Addresses.Where(x => x.TownId == deletingTown.TownId).ToList();

            context.Addresses.RemoveRange(deletingAdres);
            context.Towns.Remove(deletingTown);
            context.SaveChanges();

            sb.AppendLine($"{deletingAdres.Count} addresses in {deletingTown.Name} were deleted");

            

            return sb.ToString().Trim();
        }


        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var projects = context.Projects.FirstOrDefault(p => p.ProjectId == 2);

            var projectEmp = context.EmployeesProjects.Where(p => p.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(projectEmp);
            context.Projects.Remove(projects);

            context.SaveChanges();

            var temp = context.Projects
                .Select(n => n.Name)
                .Take(10)
                .ToList();

            foreach (var s in temp)
            {
                sb.AppendLine(s);
            }
            return sb.ToString().Trim();

        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();


            var emp = context.Employees.Where(n => EF.Functions.Like(n.FirstName, "Sa%"))
                .Select( s => new
                {
                    firstN = s.FirstName,
                    lastN = s.LastName,
                    JobTitle = s.JobTitle,
                    Salary = s.Salary
                })
                .OrderBy(f => f.firstN)
                .ThenBy(f => f.lastN)
                .ToList();
            foreach (var e in emp)
            {
                sb.AppendLine($"{e.firstN} {e.lastN} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(e => e.Employees.Count > 5)
                .OrderBy(e => e.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(x => new
                {
                    DepartmentName = x.Name,
                    ManagerName = x.Manager.FirstName+" "+x.Manager.LastName,
                    Emp = x.Employees.Select(e => new
                    {
                        EmpFName = e.FirstName+" "+e.LastName,
                        LastN = e.LastName,
                        JobTitle = e.JobTitle
                    })
                })
                .ToList();
            foreach (var department in departments)
            {
                sbGlobal.AppendLine($"{department.DepartmentName} - {department.ManagerName}");
                foreach (var emp in department.Emp.OrderBy(e => e.EmpFName).ThenBy(e => e.LastN))
                {
                    sbGlobal.AppendLine($"{emp.EmpFName} - {emp.JobTitle}");
                }
            }
            return sbGlobal.ToString();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employee147 = context.Employees.Find(147);
            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");

            var project = context.EmployeesProjects
                .Where(x => x.EmployeeId==147)
                .Select(p => new
            {
                projectName = p.Project.Name
            })
                .OrderBy(x => x.projectName)
                .ToList();

            foreach (var p in project)
            {
                sb.AppendLine(p.projectName);
            }



            ; 
                //.Select(a => new
                // {
                //     FirstName = a.FirstName,
                //     Lastname = a.LastName,
                //     JobTitle = a.JobTitle,
                //     Projects = a.EmployeesProjects.Select(x => new
                //     {
                //         project = x.Project.Name
                //     })
                // })
                //.ToList();
            //if (employee147.Count!=0)
            //{
            //    foreach (var emp in employee147)
            //    {
            //        sbGlobal.AppendLine($"{emp.FirstName} {emp.Lastname} - {emp.JobTitle}");
            //        foreach (var e in emp.Projects.OrderBy(n => n.project))
            //        {
            //            sbGlobal.AppendLine(e.project);
            //        }
            //    }
            //}
            //else
            //{
            //    sbGlobal.AppendLine($"There is not an employee with that Id");
            //}
            
            return sb.ToString().Trim();
        }


        public static string GetAddressesByTown(SoftUniContext context)
        {
            //Find all addresses, ordered by the number of employees who live there(descending), then by town name(ascending),
            //and finally by address text(ascending).Take only the first 10 addresses.For each address return it
            //in the format "<AddressText>, <TownName> - <EmployeeCount> employees"
            StringBuilder sb = new StringBuilder();

            //var addresses = context.Addresses
            //    .OrderByDescending(x => x.Employees.Count)
            //    .ThenBy(x => x.Town.Name)
            //    .ThenBy(x => x.AddressText)
            //    .Take(10);

            foreach (var address in context.Addresses.OrderByDescending(x => x.Employees.Count)
                .ThenBy(x => x.Town.Name)
                .Take(10))
            {
                sb.AppendLine($"{address.AddressText}, {address.Town} - {address.Employees.Count} employees");
            }
            return sb.ToString();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            //Find the first 10 employees who have projects started in the period 2001 - 2003(inclusive).
            //Print each employee's first name, last name, manager’s first name and last name. Then return all of their projects
            //in the format "--<ProjectName> - <StartDate> - <EndDate>", each on a new row. If a project has no end date, print "not finished" instead.
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(p => p.EmployeesProjects.Any(s => s.Project.StartDate.Year >= 2001) && p.EmployeesProjects.Any(s => s.Project.StartDate.Year <= 2003))
                .Select( x => new
                {
                    EmployeeFullName = x.FirstName+" "+x.LastName,
                    ManagerFllName = x.Manager.FirstName+" "+x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select( p =>  new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate 
                        
                    })

                })
                .Take(10)
                .ToList();
            var format = "M/d/yyyy h:mm:ss tt";
            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.EmployeeFullName} - Manager: {emp.ManagerFllName}");
                foreach (var e in emp.Projects)
                {
                    var startDate = e.StartDate.ToString(format, CultureInfo.InvariantCulture);
                    var endDate = "";
                    if (e.EndDate.HasValue)
                    {
                        endDate = e.EndDate.Value.ToString(format, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                       endDate = "not finished";
                    }
                    sb.AppendLine("--"+e.ProjectName+" - "+startDate+" - "+endDate);
                }
            }
            return sb.ToString().Trim();
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
                .Where(x => x.Department.Name == "Research and Development")
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
