using System;
using System.Collections.Generic;

namespace LinqFromScratch
{
   public static class SampleData
   {
      public static readonly Employee[] EmployeeList = new[] {
                new Employee(){EmployeeId = 1, DepartmentId = 1, Name = "Eddie Van Halen"},
                new Employee(){EmployeeId = 2, DepartmentId = 2, Name = "Alex Van Halen"},
                new Employee(){EmployeeId = 7, DepartmentId = 4, Name = "Wolfgang Van Halen"},
                new Employee(){EmployeeId = 3, DepartmentId = 3, Name = "David Lee Roth"},
                new Employee(){EmployeeId = 4, DepartmentId = 4, Name = "Michael Anthony"},
                new Employee(){EmployeeId = 5, DepartmentId = 3, Name = "Sammy Hagar"},
                new Employee(){EmployeeId = 6, DepartmentId = 3, Name = "Gary Cherone"},
        };

      public static readonly Department[] DepartmentList = new[] {
                new Department()
                {
                    DepartmentId = 1,
                    ParentId = null,
                    Name = "Dept. of redefining the very meaning of rock guitar"
                },
                new Department()
                {
                    DepartmentId = 2,
                    ParentId = null,
                    Name = "Dept. of fractal-complexity percussion"
                },
                new Department()
                {
                    DepartmentId = 3,
                    ParentId = 1,
                    Name = "Dept. of frontmanology"
                },
                new Department()
                {
                    DepartmentId = 4,
                    ParentId = 2,
                    Name = "Dept. of deep rumblings"
                },
        };

      public static readonly IEnumerable<int> IntList = new List<int>()
            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

      public static readonly Tuple<int, int, int, int>[] SortableData = new[]
      {
           new Tuple<int,int,int,int>(3, 3, 3, 3),
           new Tuple<int,int,int,int>(2, 3, 3, 3),
           new Tuple<int,int,int,int>(1, 3, 3, 3),

           new Tuple<int,int,int,int>(3, 2, 3, 3),
           new Tuple<int,int,int,int>(3, 1, 3, 3),

           new Tuple<int,int,int,int>(3, 3, 2, 3),
           new Tuple<int,int,int,int>(3, 3, 1, 3),

           new Tuple<int,int,int,int>(3, 3, 3, 2),
           new Tuple<int,int,int,int>(3, 3, 3, 1),
        };
   }
}
