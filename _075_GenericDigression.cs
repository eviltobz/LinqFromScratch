using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqFromScratch.V75
{
   [TestClass]
   public class _075_GenericDigression
   {
      // For the previous page, I wanted to output collections of ints in the same way
      // that I'd been displaying the more complex businessy types of object. I ran
      // into an issue with passing ints into IEnumerable<object>

      private readonly IEnumerable<object> objects = SampleData.DepartmentList;
      private readonly IEnumerable<int> numbers = SampleData.IntList;
      private readonly IEnumerable<DateTime> dates = new[]
      {
            DateTime.MinValue, DateTime.MaxValue,
      };

      [TestMethod]
      public void _1_CollectionsOfObjectArentCollectionsOfValueTypes()
      {
         // IEnumerable<object> is fine here
         Display.List(objects, "Objects");

         // Display.List(numbers, "Numbers");
         // But IEnumerable<int> doesn't work here even though everything in .Net inherits
         // from object. Value types and reference types are handled differently, so whilst
         // you can cast int to object individually, you can't go straight from
         // IEnumerable<int> to IEnumerable<object>, or any other value types, e.g.
         // Display.List(dates, "some dates...");

         // Hence the specialised ListOfInts method
         Display.ListOfInts(numbers, "Numbers");
      }

      [TestMethod]
      public void _2_ValueCollectionsCanBeExplicitlyConvertedToObjectCollections()
      {
         // You can cast each item in turn and add them to a new collection of objects
         // The Select & Cast methods here are both using LINQ to Objects, but the key
         // thing is that they're just running casts to box the ints.
         IEnumerable<object> num2 = numbers.Select(n => n as object);
         Display.List(num2, "Numbers as objects");

         IEnumerable<object> num3 = numbers.Cast<object>();
         Display.List(num3, "Numbers as objects, in different ways");
         // It gets the job done, but is icky, and repetitive.
      }

      [TestMethod]
      public void _3_GenericCollectionWin()
      {
         // Swapping our display method to being a generic method itself allows it to work
         // for both. The JIT compiler will create concrete versions of generic methods as it
         // needs them.
         Display.AnyList(objects, "Objects");
         Display.AnyList(numbers, "Numbers");
         Display.AnyList(dates, "dates");
      }
   }
}
