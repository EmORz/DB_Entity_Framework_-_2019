using SoftUni.Data;
using System;
using System.Linq;


namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (SoftUniContext context = new SoftUniContext())
            {
                var emp = context.Employees.ToArray();
                var count = 0;
                foreach (var e in emp)
                {
                    Console.WriteLine(count+" "+e.FirstName+" => "+e.JobTitle);
                    count++;
                }
            }
         
        }
    }
}
