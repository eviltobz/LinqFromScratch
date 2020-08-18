using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqFromScratch.V65
{
   [TestClass]
   public class _065_EnumerableDigression
   {
      [TestMethod]
      public void _1_InTheBeginningTheWasTheForLoop()
      {
         // In older C-based languages there was a lot of very manual iteration going on
         Console.WriteLine("Using a plain old for:");
         for(int i = 0; i < SampleData.DepartmentList.Length; i++)
         {
            Console.WriteLine(SampleData.DepartmentList[i].ToString());
         }
         Console.WriteLine();

         // This puts the responsibility on calling code to do correct bounds-checking etc. That's
         // not toooo onerous with a plain array, but if not all of the items are populated, or we
         // want to step through all of the items in a more complex structure, say a tree, we then
         // may have a lot more work to do every time we look through it.
      }

      [TestMethod]
      public void _2_WhatIsForEach()
      {
         // One of the common OO design patterns is called "Iterator" and C# has had its own
         // implementation of this since V1.0, which we see surfaced as the foreach keyword. It can
         // operate on anything implementing IEnumerable. This means that the data structure itself
         // handles how to step through it, and calling code doesn't need to know anything about its
         // implementation.
         Console.WriteLine("Using foreach:");
         foreach (Department dept in SampleData.DepartmentList)
         {
            Console.WriteLine(dept.ToString());
         }

         Console.WriteLine();
         Console.WriteLine("What happens behind the scenes:");
         // The compiler will expand the foreach to something a little bit like this:
         using (var enumerator = (SampleData.DepartmentList as IEnumerable<Department>).GetEnumerator())
         {
            while (enumerator.MoveNext())
            {
               Department department = enumerator.Current;
               // then we have our code from the loop body
               Console.WriteLine(department.ToString());
            }
         }
         // Note, the "using" statement in turn also gets expanded to a try/finally-dispose block

         // Note also. Calling GetEnumerator on a plain array gets you something a bit different,
         // hence the cast in the section above.
         Console.WriteLine("\n(aside)");
         var nonGeneric = SampleData.DepartmentList.GetEnumerator();
         var generic = (SampleData.DepartmentList as IEnumerable<Department>).GetEnumerator();
         Console.WriteLine($"Non Generic = ${nonGeneric.GetType()}");
         Console.WriteLine($"Generic     = ${generic.GetType()}");
         generic.Dispose();
         // We can iterate over it just the same, but it's not disposable. The first example is closer
         // to what the compiler tends to do.
         while (nonGeneric.MoveNext())
         {
            var dept = nonGeneric.Current;
            Console.WriteLine(dept);
         }
      }

      [TestMethod]
      public void _3_AndWhatDoesThatEnumeratorDo()
      {
         // Using a custom collection so we can implement its parts manually
         EnumerableThingy intCollection = new EnumerableThingy();

         Console.WriteLine("First run");
         foreach (int value in intCollection)
         {
            Console.WriteLine(value.ToString());
         }
      }

      // Fun fact - We're implementing IEnumerable here, but the C# compiler doesn't
      // actually need this to allow foreach to run. It *just* needs to find something
      // called "GetEnumerator" which returns an IEnumerator<OfTheRightType>
      public class EnumerableThingy : IEnumerable<int>
      {
         // We're just using a nice simple array as the backing data structure here
         // but it could be any sort of structure, both 1 dimensional things like
         // lists, queues, stacks etc. or 2 dimensional like trees as long as we
         // have a sensible concept of walking through the structure one item at a
         // time.
         private int[] _internalDataStructure = new int[] { 1, 2, 3, 4, 5 };

         public IEnumerator<int> GetEnumerator()
         {
            EnumeratorThingy enumerator = new EnumeratorThingy(_internalDataStructure);
            return enumerator;
         }

         // Throwback from pre-generics code
         System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
         {
            throw new NotImplementedException();
         }
      }

      public class EnumeratorThingy : IEnumerator<int>
      {
         private readonly int[] _values;
         private int _index = -1;

         public EnumeratorThingy(int[] values)
         {
            _values = values;
         }

         // MoveNext needs to be called before any data can be read. This means
         // that it copes fine with collections containing no data
         public bool MoveNext()
         {
            _index++;
            return _index < _values.Length;
         }

         public int Current
         {
            get { return _values[_index]; }
         }

         ///////
         public void Dispose()
         {
            return;
         }
         /////// We're not concerned about these methods either
         public void Reset()
         {
            throw new NotImplementedException();
         }
         object System.Collections.IEnumerator.Current
         {
            get { throw new NotImplementedException(); }
         }

      }
   }
}
