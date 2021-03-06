using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqFromScratch
{
   // This class contains a few simple methods to give us a useful view of our data.
   // Mostly nothing to see here, move along (until 7.5)
   public class Display
   {
      public static void Item(Object item, string title)
      {
         Console.WriteLine("Displaying " + item.GetType().ToString() +
             " for criteria " + title);
         Console.WriteLine(item.ToString());
         Console.WriteLine();
      }

      public static void ArrayList(ArrayList collection, string title)
      {
         Console.WriteLine("Displaying " + collection.GetType().ToString() +
             " for criteria " + title);
         foreach (var obj in collection)
            Console.WriteLine(obj);
         Console.WriteLine();
      }

      public static void List(IEnumerable<Object> collection, string title)
      {
         Console.WriteLine("Displaying " + collection.GetType().ToString() +
             " for criteria " + title);
         foreach (var item in collection)
            Console.WriteLine(item.ToString());
         Console.WriteLine();
      }

      // The aforementioned 7.5 bit:

      // C# gets in a strop dealing with ints as objects :(
      // Apart from the parameter type declaration this is absolutely identical to the one
      // above That has to be violating our DRY principles!
      public static void ListOfInts(IEnumerable<int> collection, string title)
      {
         Console.WriteLine("Displaying " + collection.GetType().ToString() +
             " for criteria " + title);
         foreach (var item in collection)
            Console.WriteLine(item.ToString());
         Console.WriteLine();
      }

      // Generics to the rescue. As with List & ListOfInt the bodies are identical, only the
      // signature changes, this time to specify the generic type.
      public static void AnyList<T>(IEnumerable<T> collection, string title)
      {
         Console.WriteLine("Generically displaying " + collection.GetType().ToString() +
             " for criteria " + title);
         foreach (var item in collection)
            Console.WriteLine(item.ToString());
         Console.WriteLine();
      }
   }
}
