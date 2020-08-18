using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqFromScratch.V9
{
   [TestClass]
   public class _09_ReturningDifferentOuterTypes_Part1
   {
      [TestMethod]
      public void _1_Ordering()
      {
         var unorderedInts = new int[] { 1, 5, 4, 2, 3 };
         var orderedInts = unorderedInts.SimpleOrder(x => x);
         // Ordering, by its very nature, can't be lazy, so orderedInts is populated at this point
         Display.ListOfInts(orderedInts, "Sorted ints");

         Display.List(SampleData.EmployeeList, "Unsorted Employees");
         var orderedEmployees = SampleData.EmployeeList.SimpleOrder(e => e.Name);
         Display.List(orderedEmployees, "Employees sorted by Name");
      }

      [TestMethod]
      public void _2_SecondaryOrdering()
      {
         var firstByDept = SampleData.EmployeeList.SimpleOrder(e => e.DepartmentId);
         Display.List(firstByDept, "Sorted By DepartmentId");

         var thenByName = SampleData.EmployeeList
            .SimpleOrder(e => e.DepartmentId)
            .SimpleOrder(e => e.Name);
         Display.List(thenByName, "Then By Name");

         // This could be pretty icky, as it'd have to keep track of the current thing(s?) to know
         // if to apply the secondary. It should be able to go arbitrary levels deep, and all we're
         // starting from is IEnumerable<T> so we don't have any way to track all the extra gubbins
         // that we'll need!
      }
   }

   public static class Extensions_Part1
   {
      // For simplicity's sake here, the field we order on has to implement IComparable. To achieve
      // this we're adding a generic constraint
      public static IEnumerable<T> SimpleOrder<T, TOrderBy>(this IEnumerable<T> original,
         Func<T, TOrderBy> selector) where TOrderBy : IComparable<TOrderBy>
      {
         var comparer = new SimpleComparer<T, TOrderBy>(selector);
         var asArray = original.ToArray();
         // I can't be bothered to add in implementing my own sorting algorithm too. Sue me!
         Array.Sort(asArray, comparer);
         return asArray;
      }
   }

   internal class SimpleComparer<T, TOrderBy> : IComparer<T> where TOrderBy : IComparable<TOrderBy>
   {
      private Func<T, TOrderBy> selector;

      public SimpleComparer(Func<T, TOrderBy> selector) => this.selector = selector;

      public int Compare(T x, T y) => selector(x).CompareTo(selector(y));
   }

   [TestClass]
   public class _09_ReturningDifferentOuterTypes_Part2
   {
      [TestMethod]
      public void _3_Ordering_WithADifferentType()
      {
         // We're calling a different method & getting a different type back, but otherwise
         // this is the same as the first example.
         var unorderedInts = new int[] { 1, 5, 4, 2, 3 };
         var orderedInts = unorderedInts.ProperOrder(x => x);
         Display.ListOfInts(orderedInts, "Sorted ints");

         Display.List(SampleData.EmployeeList, "Unsorted Employees");
         var orderedEmployees = SampleData.EmployeeList.ProperOrder(e => e.Name);
         Display.List(orderedEmployees, "Sorted Employees");
      }

      [TestMethod]
      public void _4_SecondaryOrdering()
      {
         var firstByDept = SampleData.EmployeeList.ProperOrder(e => e.DepartmentId);
         Display.List(firstByDept, "Sorted By DepartmentId");

         var thenByName = SampleData.EmployeeList
            .ProperOrder(e => e.DepartmentId)
            .AndThen(e => e.Name);
         Display.List(thenByName, "Then By Name");
      }

      [TestMethod]
      public void _5_TertiaryOrdering()
      {
         Display.List(SampleData.SortableData, "Unordered");
         var first = SampleData.SortableData.ProperOrder(x => x.Item1);
         Display.List(first, "By Item1");

         var second = SampleData.SortableData.ProperOrder(x => x.Item1).AndThen(x => x.Item2);
         Display.List(second, "Then by Item2");

         // We don't have to keep chaining them inline mind, we can use those intermediate variables.
         // This would allow us to build additional ordering on a collection as it passes through
         // different functions.
         var third = second.AndThen(x => x.Item3);
         Display.List(third, "Then by Item3");

         var fourth = third.AndThen(x => x.Item4);
         Display.List(fourth, "Then by Item4");

         // And we've not changed the earlier intermediate variables
         Display.List(second, "And the one by Item2 is still the same");
      }
   }

   // This quickly gets a LOT more complex than the previous parts. Our extension methods are nice
   // and simple, but then we're implementing new collection classes for them to interact with, and
   // that's where it all goes a bit tricky.
   // Don't worry if you don't really follow it. You can poke around in the code and step through
   // locally if you want to get your head around it better.
   public static class Extensions_Part2
   {
      // We're continuing to require the ordered field to implement IComparable & are using the
      // SimpleComparer. A full implementation would allow us to pass something like a
      // Func<T, T, int> so we could specify an ordering function in the call
      public static IOrderedEnumerable<T> ProperOrder<T, TOrderBy>(this IEnumerable<T> original,
         Func<T, TOrderBy> selector) where TOrderBy : IComparable<TOrderBy>
      {
         IComparer<T> comparer = new SimpleComparer<T, TOrderBy>(selector);
         return ProperOrderedCollection<T>.Create(original, comparer);
      }

      public static IOrderedEnumerable<T> AndThen<T, TOrderBy>(this IOrderedEnumerable<T> original,
         Func<T, TOrderBy> selector) where TOrderBy : IComparable<TOrderBy>
      {
         IComparer<T> comparer = new SimpleComparer<T, TOrderBy>(selector);
         return original.CreateSubOrder(comparer);
      }
   }

   public interface IOrderedEnumerable<T> : IEnumerable<T>
   {
      IOrderedEnumerable<T> CreateSubOrder(IComparer<T> selector);
   }

   public class ProperOrderedCollection<T> : IOrderedEnumerable<T>
   {
      private readonly IEnumerable<T> original;
      private readonly ProperOrderedCollection<T> parent;
      IComparer<T> comparer;

      // Private constructor, the external creation is controlled by the factory methods below
      private ProperOrderedCollection(IEnumerable<T> original, ProperOrderedCollection<T> parent,
         IComparer<T> comparer)
      {
         this.original = original;
         this.parent = parent;
         this.comparer = comparer;
      }
      public static ProperOrderedCollection<T> Create(IEnumerable<T> original, IComparer<T> comparer)
      {
         return new ProperOrderedCollection<T>(original, null, comparer);
      }

      // This is the implementation of the method on the interface that the extension method calls
      public IOrderedEnumerable<T> CreateSubOrder(IComparer<T> comparer)
      {
         return new ProperOrderedCollection<T>(null, this, comparer);
      }

      // And for reference, this is the full LINQ signature, rather that my own cut-down version. It
      // requires the keySelector, with an optional comparer based on the TKey returned from the
      // selector. This means that the field doesn't need to implement IComparable. (And there's a
      // descending flag too.) My partial implementation requiring an IComparable field reduces the
      // complexity a little.
      public IOrderedEnumerable<T> CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector,
         IComparer<TKey> comparer, bool descending)
      {
         throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
      public IEnumerator<T> GetEnumerator()
      {
         return GetOrderedEnumerator();
      }

      // Oooh, we're returning our own instance of IEnumerator<T>, almost like it was a planned
      // callback from earlier in the session. (It wasn't. It's just a fluke.)
      private BaseOrderedEnumerator<T> GetOrderedEnumerator()
      {
         if (original is null)
            return new NestedOrderedEnumerator<T>(parent.GetOrderedEnumerator(), comparer);

         return new FirstOrderedEnumerator<T>(original, comparer);
      }
   }


   // Whilst working on this, it became quite gnarly due to the code having to check if it was the
   // original IEnumerable, or a nested version. Any time that you are checking different fields &
   // making different choices, that's a sign that you may really have different, related classes.
   // (Yes, the code instantiting these enumerators is doing that. But only in a single place, and
   // something needs to make the choice somewhere)
   // So here we have a base class with some common code, then 2 different implementations of it

   internal class FirstOrderedEnumerator<T> : BaseOrderedEnumerator<T>
   {
      public FirstOrderedEnumerator(IEnumerable<T> data, IComparer<T> comparer) : base(comparer)
      {
         SortAndStoreParentData(data);
      }

      internal override bool GetNextBatch() => BuildNextBatchFromSortedData();
   }

   internal class NestedOrderedEnumerator<T> : BaseOrderedEnumerator<T>
   {
      private BaseOrderedEnumerator<T> parent;

      public NestedOrderedEnumerator(BaseOrderedEnumerator<T> parent, IComparer<T> comparer) 
         : base(comparer)
      {
         this.parent = parent;
      }

      internal override bool GetNextBatch()
      {
         if (BuildNextBatchFromSortedData())
            return true;

         if (parent.GetNextBatch())
         {
            SortAndStoreParentData(parent.CurrentBatchData);
            BuildNextBatchFromSortedData();
            return true;
         }

         return false;
      }
   }

   internal abstract class BaseOrderedEnumerator<T> : IEnumerator<T>
   {
      #region Interface noise
      public virtual void Dispose() { }
      public virtual void Reset() { } // We'd need to implement this in production code
      object IEnumerator.Current => this.Current;
      #endregion

      #region Implementation of IEnumerator<T> - This allows us to iterate the final, ordered collection
      // This is no more complex than the earlier IEnumerator that we saw
      public T Current { get; internal set; }

      // This, on the other hand, is a bunch more convoluted.
      public bool MoveNext()
      {
         // Can we MoveNext in all the things in the same order position
         if (CurrentBatchEnumerator.MoveNext())
         {
            Current = CurrentBatchEnumerator.Current;
            return true;
         }

         // If not, can we get a group of items at the next ordered position, and MoveNext on that?
         if (GetNextBatch())
            return MoveNext();

         return false;
      }
      #endregion

      internal abstract bool GetNextBatch();
      internal List<T> CurrentBatchData { get; private set; }

      private IEnumerator<T> CurrentBatchEnumerator { get; set; }
      private readonly IComparer<T> comparer;
      private int index;
      private T[] sorted;

      protected BaseOrderedEnumerator(IComparer<T> comparer)
      {
         this.comparer = comparer;
         CurrentBatchEnumerator = new List<T>().GetEnumerator();
      }

      protected void SortAndStoreParentData(IEnumerable<T> data)
      {
         sorted = data.ToArray();
         Array.Sort(sorted, comparer);
         index = 0;
      }

      protected bool BuildNextBatchFromSortedData()
      {
         if (!HasDataToBuildBatch())
            return false;

         CurrentBatchData = new List<T>();
         var currentKey = sorted[index];
         while (StillHasMatchingItems(currentKey))
         {
            CurrentBatchData.Add(sorted[index]);
            index++;
         }

         CurrentBatchEnumerator = CurrentBatchData.GetEnumerator();
         return true;
      }

      private bool StillHasMatchingItems(T key) =>
         index < sorted.Length
         && comparer.Compare(key, sorted[index]) == 0;

      private bool HasDataToBuildBatch() =>
         !(sorted is null)
         && index < sorted.Length;
   }


   // Time to change gear then. We're used to thinking of using LINQ To Objects over
   // IEnumerables but we also have other places we can use the syntax, LINQ To Xml,
   // LINQ To SQL/Entities etc. which all work on IQueryable instead. Here we're not
   // (necessarily) just iterating over a simple collection in memory, and we want to
   // be a bit more clever with the way we build and execute queries when hitting DBs
   // etc. Sadly, this is bloody hard, and basically means writing a compiler, so
   // that's a good place to stop :)
}
