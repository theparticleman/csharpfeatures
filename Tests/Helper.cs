using System;
using System.Collections.Generic;

namespace Tests
{
  internal class Helper
  {
    static Random rand = new Random();
    internal static object GetRandomObject()
    {
      switch (rand.Next(5))
      {
        case 0:
          return "Don't panic";
        case 1:
          return rand.Next(100);
        case 2:
          return new object();
        case 3:
          return null;
        default:
          return new List<string>();
      }
    }
  }
}