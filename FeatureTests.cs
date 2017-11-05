using System;
using NUnit.Framework;

namespace Tests
{
  //Visual Studio Code isn't super keen on some C# 7.1 features
  public class CSharp71Features
  {

    [Test]
    public void Enabling71Features()
    {
      //In .NET Core 2.0 you need to add a node to your .csproj file

      //In Visual Studio you can change your project properties or modify your project file manually
    }

    [Test]
    public void AsyncMain()
    {
      //You can have async Main() methods now
    }

    [Test]
    public void DefaultLiteralExpressions()
    {
      //You used to have to do this
      int intValue = default(int);
      Assert.That(intValue, Is.EqualTo(0));

      //But now you can do this
      //Visual Studio Code isn't so keen on this, but full Visual Studio and the compiler both think it's totally legit
      int anotherIntValue = default;
      Assert.That(anotherIntValue, Is.EqualTo(0));

      //It's a little more useful in situations like this
      Func<int, string> func = default;
      Assert.That(func, Is.EqualTo(default(Func<int, string>)));
    }

    [Test]
    public void InferredTupleElementNames()
    {
      //We'll talk about this when we talk about tuples
    }
  }

  public class CSharp7Features
  {
    [Test]
    public void MoreExpressionBodies()
    {
      var example = new ExpressionBodyExample("foo");
      Assert.That(example, Is.Not.Null);
    }

    class ExpressionBodyExample
    {
      private string stringValue;
      public ExpressionBodyExample(string value) => this.stringValue = value;

      ~ExpressionBodyExample() => Console.WriteLine("You probably shouldn't use a finalizer");

      public string ValueTuple
      {
        get => stringValue;
        set => stringValue = value ?? "Default value";
      }
    }

    [Test]
    public void NumericLiterals()
    {
      //Binary literals were added
      var binaryValue = 0b0100;
      Assert.That(binaryValue, Is.EqualTo(4));

      //Digit separators were added
      //Separators work for ints, longs, floats, doubles, and decimals
      var oneBillion = 1_000_000_000;
      Assert.That(oneBillion, Is.EqualTo(1000000000));
    }

    [Test]
    public void LocalFunctions()
    {
      var evenNumber = 42;
      Assert.That(IsEven(evenNumber), Is.True);

      var oddNumber = 43;
      Assert.That(IsOdd(oddNumber), Is.True);

      //Local functions can have an expression body or a normal, full body
      bool IsEven(int value) => value % 2 == 0;
      bool IsOdd(int value)
      {
        return value % 2 != 0;
      }
    }

    class PatternMatching
    {
      //Pattern matching in C# isn't as robust as something like F# 
      //or Scala yet, but they are getting there.
      [Test]
      public void OutVariables()
      {
        //Before C# 7, you had to do out variables like this
        var valueToParse = "42";
        int result;
        if (int.TryParse(valueToParse, out result))
          Assert.That(result, Is.EqualTo(42));

        //Now you can streamline things a bit
        if (int.TryParse(valueToParse, out int result2))
          Assert.That(result2, Is.EqualTo(42));

        //out variables can also be implicitly typed
        if (int.TryParse(valueToParse, out var result3))
          Assert.That(result3, Is.EqualTo(42));

        //The scope of these out variables "leaks" outside the body of the if statements
        Assert.That(result, Is.EqualTo(result2));
      }

      [Test]
      public void IfExpressionPatterns()
      {
        object input = "42";

        //We used to have to do something ugly like this
        int i = 0;
        if (input is int)
        {
          i = (int)input;
        }
        else if (input is string)
        {
          var inputAsString = (string)input;
          int inputAsStringAsInt;
          if (int.TryParse(inputAsString, out inputAsStringAsInt))
          {
            i = inputAsStringAsInt;
          }
        }
        Assert.That(i, Is.EqualTo(42));
        //If input isn't an int or isn't as string that is parseable as an int then i will equal 0

        //Now we can have something much more elegant
        if (input is int x || input is string s && int.TryParse(s, out x))
        {
          //If input isn't an int or isn't a string that is parseable as an int then 
          //the body of the if will never get executed
          Assert.That(x, Is.EqualTo(42));
        }
      }

      [Test]
      public void SwitchExpressions()
      {
        for (int i = 0; i < 1000; i++)
        {
          var someObject = Helper.GetRandomObject();
          switch (someObject)
          {
            case string stringVal:
              Assert.That(stringVal, Does.Contain("Don't panic"));
              break;
            case int intVal when intVal < 50:
              Assert.That(intVal, Is.LessThan(50));
              break;
            case int intVal when intVal > 50:
              Assert.That(intVal, Is.GreaterThan(50));
              break;
            case object objValue:
              Assert.That(objValue, Is.Not.Null);
              break;
            case null:
              Assert.That(someObject, Is.Null);
              break;
            default:
              Assert.That(someObject, Is.Not.Null);
              break;
          }
        }
      }
    }

    class Tuples
    {
      //These new tuples are compiler syntactic sugar around System.ValueTuple
      //.NET Core 2.0 supports new tuples
      //.NET Framework 4.7 supports new tuples
      //Earlier .NET Framework version support new tuples, but you need VS 2017 and to add a nuget reference to System.ValueTuple

      [Test]
      public void BasicExample()
      {
        //We used to have to do this like savages
        var oldSchoolTuple = Tuple.Create(42, "foo");
        Assert.That(oldSchoolTuple.Item1, Is.EqualTo(42));
        Assert.That(oldSchoolTuple.Item2, Is.EqualTo("foo"));

        //But now we can do this instead
        var coolTuple = (42, "foo");
        Assert.That(coolTuple.Item1, Is.EqualTo(42));
        Assert.That(coolTuple.Item2, Is.EqualTo("foo"));

        //The code above is the same as this
        var coolishTuple = ValueTuple.Create(42, "foo");
        Assert.That(coolishTuple.Item1, Is.EqualTo(42));
        Assert.That(coolishTuple.Item2, Is.EqualTo("foo"));

        //Or we can be even cooler
        var coolerTuple = (intVal: 42, stringVal: "foo");
        Assert.That(coolerTuple.intVal, Is.EqualTo(42));
        Assert.That(coolerTuple.stringVal, Is.EqualTo("foo"));
      }

      [Test]
      public void InferredElementNames()
      {
        //This is a C# 7.1 feature
        var intVal = 42;
        var stringVal = "foo";

        var tuple = (intVal, stringVal);
        //Visual Studio Code is a little doubtful about the validity of this, but full Visual Studio and the compiler are both okay with it
        Assert.That(tuple.intVal, Is.EqualTo(42));
        Assert.That(tuple.stringVal, Is.EqualTo("foo"));
      }

      [Test]
      public void AsReturnTypes()
      {
        var tuple = ReturnTuple();
        Assert.That(tuple.Item1, Is.EqualTo(42));
        Assert.That(tuple.Item2, Is.EqualTo("foo"));

        var tupleWithNamedElements = ReturnTupleWithNamedElements();
        Assert.That(tupleWithNamedElements.intVal, Is.EqualTo(42));
        Assert.That(tupleWithNamedElements.stringVal, Is.EqualTo("foo"));

        //You probably don't want to return tuples as part of a public API,
        //but for internal things it might be an appropriate solution
        (int, string) ReturnTuple() => (42, "foo");
        (int intVal, string stringVal) ReturnTupleWithNamedElements() => (42, "foo");
      }

      [Test]
      public void Equality()
      {
        var tuple1 = (42, "foo");
        var tuple2 = (42, "foo");

        Assert.That(tuple1, Is.EqualTo(tuple2));
        Assert.That(tuple1.Equals(tuple2), Is.True);
        // Assert.That(tuple1 == tuple2, Is.True); //The == operator isn't defined for new tuples :(
      }

      [Test]
      public void Deconstructors()
      {
        //Not to be confused with destructors
        var tuple = (42, "foo");
        var (intVal, stringVal) = tuple;

        Assert.That(intVal, Is.EqualTo(tuple.Item1));
        Assert.That(stringVal, Is.EqualTo(tuple.Item2));

        //You can also implement deconstrutors in your own types
      }

      [Test]
      public void Discards()
      {
        //You probably shouldn't use tuples with lots of values very often, but when you do...
        var superHugeTuple = (42, "foo", 4.5, "Arthur Dent", "Life, The Universe, and Everything");
        var (answer, _, _, _, question) = superHugeTuple;

        Assert.That(answer, Is.EqualTo(42));
        Assert.That(question, Does.Contain("Universe"));
      }
    }
  }
}