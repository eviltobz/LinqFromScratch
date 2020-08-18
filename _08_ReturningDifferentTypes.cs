using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqFromScratch.V8
{
   [TestClass]
   public class _08_ReturningDifferentInnerTypes
   {
      [TestMethod]
      public void _1_SimpleAlteredStructure()
      {
         var justTheNames = SampleData.EmployeeList.Project(x => x.Name);
         Display.List(justTheNames, "Just The Names");

         var idAndName = SampleData.EmployeeList.Project(x =>
            new Tuple<int, string>(x.EmployeeId, x.Name));
         Display.List(idAndName, "Ids & Names");
         // This is using Tuple just to be lazy & not create some other type, but we could go
         // to all sorts of different complex types here.
      }


      [TestMethod]
      public void _2_CreatingAnonymousTypes()
      {
         // Here we're using Anonymous Types to create a structure that we've not explicitly
         // defined elsewhere in the code, with a combination of a straight mapping of one
         // property & some manipulation of another.
         var projectedType = SampleData.EmployeeList.Project(x => new
         {
            NewId = x.EmployeeId,
            FirstName = x.Name.Substring(0, x.Name.IndexOf(' ')),
            LastName = x.Name.Substring(x.Name.IndexOf(' '))
         });

         Display.List(projectedType, "NewId, FirstName & LastName");

         // As mentioned in the session, this is statically typed, not dynamic, the output
         // will show the anonymous type being used, and hacking around this foreach loop
         // will let you see the intellisense for the returned data.
         foreach (var funkyNewType in projectedType)
         {
            var intellisenseMeddling = funkyNewType.FirstName;
         }
         // However, the anonymous type only exists in the same scope as it was declared
         // so we can't usefully pass it as an argument to other methods, or return it.
         // It's great for very localised, simple usage, but if it needs to go a little
         // further you'll need to create a full type, or use something like a Tuple.
         // Tuples carry no real context, so can be good for returning a few things in a
         // private method, but shouldn't tend to leak outside to anything else, IMHO.
      }
   }

   public static class Extensions
   {
      // Now we're going for Linq's Select method which performs an operation known as
      // projection in the database world - limiting the returned data to the fields we want.
      // And this is the first example with 2 generic types, T & TReturn.
      public static IEnumerable<TReturn> Project<T, TReturn>(this IEnumerable<T> originalList,
          Func<T, TReturn> conversion)
      {
         foreach (var item in originalList)
         {
            yield return conversion(item);
         }
      }
      // Damn, that was easy.
      // It legitimately took me by surprise how simple that was when I wrote it. So, to
      // balance that out I came up with the idea for doing the next file.
   }
}
