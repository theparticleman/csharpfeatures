
using System;
using System.Linq;
using NUnit.Framework;

namespace Tests
{
    class CSharp8Features
    {
        class CSharp7Review
        {
            [Test] public void LocalFunctions()
            {
                //You know when you have that method in a class that should
                //only be called from one other method in your class and
                //nowhere else? So you make it private and put it at the
                //bottom of your file and hope that no one ever sees it
                //and accidentally calls it when they shouldn't.
                //With local functions there's a  better option.
                var result = Double(32);
                Assert.That(result, Is.EqualTo(64));

                int Double(int input) => input * 2;
            }

            [Test] public void ValueTuples()
            {
                //System.Tuple has existed since .NET Framework 4.0.
                //It works, but it's kind of clunky.
                var oldBrokenTuple = Tuple.Create("foo", 42);
                Assert.That(oldBrokenTuple.Item1, Is.EqualTo("foo"));

                //With C# 7 they added a new ValueTuple type.
                //There is syntactic sugar around creating value tuples.
                var newAwesomeTuple = ("foo", 42);
                Assert.That(newAwesomeTuple.Item1, Is.EqualTo("foo"));

                //Value tuples also have more features than the old System.Tuple class.
                //One example is named fields.
                var namedFieldTuple = (stringVal: "foo", intVal: 42);
                Assert.That(namedFieldTuple.stringVal, Is.EqualTo("foo"));

                //You can also use a deconstructor (not destructor)
                //to get the values back out of a tuple.
                //We'll talk more about this later.
                var tuple = ("foo", 42);
                var (stringVal, intVal) = tuple;

                Assert.That(stringVal, Is.EqualTo("foo"));
            }

            [Test] public void Discards()
            {
                //Sometimes you get a value, but you don't actually care about using it.
                //Discards are great for these situations.
                //Sometimes you can use them to get rid of compiler warnings or to
                //explicitly show that you know you're getting a return value
                //you're intentionally choosing not to use it.
                var _ = MethodThatReturnsSomeValueWeDoNotCareAbout();
                //According docs.microsoft.com, the discard variable is write-only,
                //but this test appears to work. ¯\_(ツ)_/¯
                Assert.That(_, Is.EqualTo(173));

                //You can use discards in other places too, such as tuple deconstruction.
                var tuple = ("foo", 42);
                var (_, intVal) = tuple;
                Assert.That(intVal, Is.EqualTo(42));

                //Discards also work for out parameters whose values aren't used.
                Assert.That(int.TryParse("42", out var _), Is.True);

                int MethodThatReturnsSomeValueWeDoNotCareAbout() => 173;
            }

            [Test] public void PatternMatching()
            {
                //C# 7 introduced the first pattern matching functionality in C#.
                //The pattern matching in C# 7 was somewhat limited.
                //C# 8 adds several new forms of pattern matching.
                var answer = "no answer yet";
                var someValue = GetSomeRandomValue();

                //This is an example a switch pattern match.
                switch (someValue)
                {
                    case null: //the null pattern
                        answer = "the value was null";
                        break;
                    case 0: //a normal constant case
                        answer = "the value was zero";
                        break;
                    case int intVal when intVal > 42: //a type pattern with a when condition
                        answer = "the value was some number bigger than 42";
                        break;
                    case int intVal: //a type pattern
                        answer = "the value was some number";
                        break;
                    case string stringVal: //another type pattern
                        answer = "the value was " + stringVal;
                        break;
                    default: //the normal default case
                        answer = "the value was something else";
                        break;
                }

                Assert.That(answer, Is.EqualTo("the value was null"));

                object GetSomeRandomValue() => null;
            }
        }

        class IndicesAndRanges
        {
            //Indices and ranges work with arrays, strings, Span<T>, and ReadOnlySpan<T>.

            [Test] public void Indices()
            {
                //What if you want to get the last element of an array?
                var lastElementTheHardWay = testArray[testArray.Length - 1];
                Assert.That(lastElementTheHardWay, Is.EqualTo("around"));

                //You could also do it like this:
                var lastElementUsingLinq = testArray.Last();
                Assert.That(lastElementUsingLinq, Is.EqualTo("around"));

                //But with C# 8 you can use this syntax.
                var lastElementTheEasyWay = testArray[^1];
                Assert.That(lastElementTheEasyWay, Is.EqualTo("around"));

                //You can use a variable to index from the end.
                var howManyElementsFromTheEnd = 4;
                var someElementGivenNumberOfElementsFromTheEnd = testArray[^howManyElementsFromTheEnd];
                Assert.That(someElementGivenNumberOfElementsFromTheEnd, Is.EqualTo("the"));

                //You can use a calculation to index from the end.
                var anElementBasedOnSomeCalculation = testArray[^(4 * 2 - 3)]; //This won't work without the parens.
                Assert.That(anElementBasedOnSomeCalculation, Is.EqualTo("not"));

                //^0 is the same as array.Length.
                Assert.Throws<IndexOutOfRangeException>(() => { var x = testArray[^0]; });
            }

            [Test] public void Ranges()
            {
                //What if you want to get all the elements in an array except
                //the first two and the last two?

                //You could do something like this:
                var arrayMinusFirstAndLastTwo = testArray.Skip(2).Take(testArray.Length - 4).ToArray();
                Assert.That(testArray.Length, Is.EqualTo(13));
                Assert.That(arrayMinusFirstAndLastTwo.Length, Is.EqualTo(9));
                Assert.That(arrayMinusFirstAndLastTwo[0], Is.EqualTo("the"));
                Assert.That(arrayMinusFirstAndLastTwo.Last(), Is.EqualTo("other"));

                //With ranges you could be much more concise like this:
                var arrayMinusFirstAndLastTwoButBetter = testArray[2..^2];
                Assert.That(arrayMinusFirstAndLastTwoButBetter.Length, Is.EqualTo(9));
                Assert.That(arrayMinusFirstAndLastTwoButBetter[0], Is.EqualTo("the"));
                Assert.That(arrayMinusFirstAndLastTwoButBetter[^1], Is.EqualTo("other"));

                Assert.That(arrayMinusFirstAndLastTwo, Is.EqualTo(arrayMinusFirstAndLastTwoButBetter));
            }

            [Test] public void SomeMoreRangeExamples()
            {
                //If you want to do a range that includes "from the beginning"
                //or "to end" you can totally do that too.

                var firstFourElements = testArray[..4];
                Assert.That(firstFourElements, Is.EqualTo(new[] { "Always", "reward", "the", "people" }));

                var lastTwoElements = testArray[^2..];
                Assert.That(lastTwoElements, Is.EqualTo(new[] { "way", "around" }));
            }

            [Test] public void RangesAreJustVariables()
            {
                //Ranges are just variables that easy to declare using the range operator (..).
                var range = ..4;
                Assert.That(range.GetType(), Is.EqualTo(typeof(Range)));
            }

            readonly string[] testArray =
            {
                "Always",
                "reward",
                "the",
                "people",
                "and",
                "blame",
                "the",
                "system",
                "not",
                "the",
                "other",
                "way",
                "around"
            };
        }

        class NonnullableReferenceTypes
        {
            //This is called a nullable annotation context.
            #nullable enable
            class Character
            {
                //You can turn on nullable reference types for a class or a whole file.
                //You can also turn on nullable reference types for an entire project by adding
                //<Nullable>enable</Nullable>
                //to your project file. But for existing code the number of warnings
                //might be too large to deal with all at once.

                public int HitPoints { get; set; }
                public string Name { get; set; }
                //If you want to show that a variable can be null, use the ? syntax.
                public string? Class { get; set; }

                //If you want nullable reference types for the entire file,
                //don't add #nullable restore.
            }
            #nullable restore

            [Test] public void ClassWithNonnullableProperties()
            {
                var dwarf = new Character { Name = "Gimli" };
                dwarf.Name = null;
                Assert.That(dwarf.Name, Is.Null);
                //The non-nullability is only reported as a warning by the compiler.
                //You could turn on warnings as errors if you want compilation to fail.

                #nullable enable
                string? wizardName = "Gandalf";
                var wizard = new Character
                {
                    //The compiler knows that wizardName cannot
                    //be null here, so it allows the assignment.
                    //If the variable might be null here, the
                    //compiler won't allow the assignment.
                    Name = wizardName
                };
                #nullable restore
            }

            #nullable enable
            [Test] public void NullForgivingOperator()
            {
                Character? character = Find("Legolas");

                if (IsValid(character))
                {
                    //The compiler doesn't know that we've already done a null check on character.
                    //We can add the null forgiving operator (the postfix ! operator) to
                    //tell the compiler not to give us a warning in this case.
                    Assert.That(character!.Name, Is.EqualTo("Legolas"));
                }
                else
                {
                    Assert.Fail("Character should be valid");
                }

                bool IsValid(Character? character) =>
                    character != null && !string.IsNullOrEmpty(character.Name);

                Character? Find(string name) =>
                    name switch
                    {
                        "Legolas" => new Character { Name = name, HitPoints = 80 },
                        "Gimli" => new Character { Name = name, HitPoints = 100 },
                        _ => null
                    };
            }
            #nullable restore
        }

        class PatternMatching
        {
            [Test] public void SwitchExpressions()
            {
                //A lot of times you have a problem were you need to return a value
                //based on the value of some variable.
                //This example uses integers, but other examples could include
                //strings and enums.
                var intVal = 42;
                var switchAnswer = GetAnswerUsingNormalSwitch(intVal);
                var patternMatchingAnswer = GetAnswerUsingPatternMatching(intVal);
                Assert.That(patternMatchingAnswer, Is.EqualTo(switchAnswer));

                //You could implement a solution using a normal switch statement.
                string GetAnswerUsingNormalSwitch(int input)
                {
                    switch(input)
                    {
                        case 0:
                            return "zero";
                        case 2:
                            return "smallest prime";
                        case 4:
                            return "first composite number";
                        case 7:
                            return "considered lucky in some Western cultures";
                        case 8:
                            return "considered lucky in Chinese culture";
                        case 9:
                            return "first odd composite number";
                        case 11:
                            return "smallest two digit prime in base 10";
                        case 30:
                            return "smallest sphenic number"; //https://en.wikipedia.org/wiki/Sphenic_number
                        case 42:
                            return "the answer to the ultimate question of life, the universe, and everything";
                        default:
                            return "some other number";
                    }
                }

                //Or you could implement a less verbose solution using a switch expression.
                string GetAnswerUsingPatternMatching(int input) =>
                    input switch
                    {
                        0 => "zero",
                        2 => "smallest prime",
                        4 => "first composite number",
                        7 => "considered lucky in some Western cultures",
                        8 => "considered lucky in Chinese culture",
                        9 => "first odd composite number",
                        11 => "smallest two digit prime in base 10",
                        30 => "smallest sphenic number",
                        42 => "the answer to the ultimate question of life, the universe, and everything",
                        _ => "some other number"
                    };
            }

            [Test] public void PropertyPatterns()
            {
                //Let's say you have some object and want to get some answer
                //based on several possible values of that object's properties.
                var house = new House
                {
                    State = "UT",
                    GarageCarCount = 2,
                    BedroomCount = 4,
                    BathroomCount = 2,
                    EthernetEverywhere = true
                };

                var houseIsAcceptable = IsHouseAcceptable(house);
                Assert.That(houseIsAcceptable, Is.True);

                //You could do an if-else construct to do this.
                //Or you could use a property pattern.
                bool IsHouseAcceptable(House house) =>
                    house switch
                    {

                        { State: "VT" } => false,
                        { GarageCarCount: 1 } => false,
                        { BedroomCount: 1 } => false,
                        { BedroomCount: 2 } => false,
                        { BedroomCount: 3 } => false,
                        { EthernetEverywhere: false } => false,
                        _ => true
                    };
            }

            [Test] public void TuplePatterns()
            {
                //If you have two or more variables and you want to get some result based
                //on the values of all those variables, a tuple pattern could be a good candidate.

                var oldResult = GetOldResult("rock", "lizard");
                Assert.That(oldResult, Is.EqualTo("rock crushes lizard"));

                var newResult = GetNewResult("spock", "rock");
                Assert.That(newResult, Is.EqualTo("spock vaporizes rock"));

                //You could implement a solution to a problem like this using a series of if-else statements.
                //This was briefest if-else solution I could think of.
                string GetOldResult(string player1, string player2)
                {
                    if (player1 == "rock" && player2 == "lizard") return "rock crushes lizard";
                    if (player1 == "rock" && player2 == "scissors") return "rock crushes scissors";
                    if (player1 == "spock" && player2 == "rock") return "spock vaporizes rock";
                    if (player1 == "spock" && player2 == "scissors") return "spock smashes scissors";
                    if (player1 == "lizard" && player2 == "paper") return "lizard eats paper";
                    if (player1 == "lizard" && player2 == "spock") return "lizard poisons spock";
                    if (player1 == "paper" && player2 == "spock") return "paper disproves spock";
                    if (player1 == "paper" && player2 == "rock") return "paper covers rock";
                    if (player1 == "scissors" && player2 == "paper") return "scissors cut paper";
                    if (player1 == "scissors" && player2 == "lizard") return "scissors decapiate lizard";
                    return "player 1 did not win";
                }

                //But in C# 8 you can use a tuple pattern to make the solution even more concise.
                //It obviously depends on opinion and familiarity, but I think this solution is more readable.
                //It may also just be a reflection on my typing skills, but this solution
                //took about half as long to type.
                string GetNewResult(string player1, string player2)
                {
                    return (player1, player2) switch
                    {
                        ("rock", "lizard") => "rock crushes lizard",
                        ("rock", "scissors") => "rock crushes scissors",
                        ("spock", "rock") => "spock vaporizes rock",
                        ("spock", "scissors") => "spock smashes scissors",
                        ("lizard", "paper") => "lizard eats paper",
                        ("lizard", "spock") => "lizard poisons spock",
                        ("paper", "spock") => "paper disproves spock",
                        ("paper", "rock") => "paper covers rock",
                        ("scissors", "paper") => "scissors cut paper",
                        ("scissors", "lizard") => "scissors decapitate lizard",
                        ("kirk", _) => "This isn't rock paper scissor lizard kirk. Player 1 cheated.",
                        _ => "player 1 did not win"
                    };
                }
            }

            [Test] public void PositionalPatterns()
            {
                //This type of pattern matching depends on having a deconstructor.

                var house = new House
                {
                    State = "WY",
                    Price = 175_000,
                    SquareFootage = 2150,
                    EthernetEverywhere = true,
                    BedroomCount = 4,
                    BathroomCount = 2
                };

                var houseIsAcceptable = IsHouseAcceptable(house);
                Assert.That(houseIsAcceptable, Is.True);

                bool IsHouseAcceptable(House house)
                {
                    return house switch
                    {
                        //The patterns are evaluated in order.
                        //The first one that matches is what is returned.
                        var (price, _, _, _, _, _, _) when price > 200_000 => false,
                        var (_, squareFootage, _, _, _, _, _) when squareFootage < 2000 => false,
                        var (_, _, _, _, _, ethernetEverywhere, _) when !ethernetEverywhere => false,
                        var (_, _, _, _, _, _, state) when state == "VT" => false,
                        var (_, _, _, bedroomCount, bathroomCount, _, state) when
                            state == "MT" &&
                            bedroomCount >= 5 &&
                            bathroomCount >= 3 => true,
                        //You can mix and match some of the different types of pattern matching,
                        //like adding a property pattern here
                        { State: "NV" } => false,
                        _ => true
                    };
                }
            }

            class House
            {
                public decimal Price { get; set; }
                public double SquareFootage  { get; set; }
                public int GarageCarCount { get; set; }
                public int BedroomCount { get; set; }
                public int BathroomCount { get; set; }
                public bool EthernetEverywhere { get; set; }
                public string State { get; set; }

                public void Deconstruct(out decimal price, out double squareFootage,
                    out int garageCarCount, out int bedroomCount, out int bathroomCount,
                    out bool ethernetEverywhere, out string state)
                {
                    price = Price;
                    squareFootage = SquareFootage;
                    garageCarCount = GarageCarCount;
                    bedroomCount = BedroomCount;
                    bathroomCount = BathroomCount;
                    ethernetEverywhere = EthernetEverywhere;
                    state = State;
                }
            }
        }

        [Test] public void NullCoalescingAssignment()
        {
            //Suppose you have a variable that might be null and you want to assign it a value if it is null.

            //You could do it old school like this:
            string myVariable = null;
            if (myVariable == null)
            {
                myVariable = "not null";
            }
            Assert.That(myVariable, Is.Not.Null);

            //You could do it a little cooler using the null coalescing operator.
            //This operator has been in C# since 2.0.
            myVariable = null;
            myVariable = myVariable ?? "not null";
            Assert.That(myVariable, Is.Not.Null);

            //But that's a little verbose. Now you can make it even more concise.
            myVariable = null;
            myVariable ??= "not null";
            Assert.That(myVariable, Is.Not.Null);
        }

        [Test] public void UsingDeclarations()
        {

        }

        [Test] public void StaticLocalFunctions()
        {
            var x = 5;
            NonStaticLocalFunction();
            StaticLocalFunction();
            Assert.That(x, Is.EqualTo(23));

            void NonStaticLocalFunction()
            {
                x = 23;
            }

            static void StaticLocalFunction()
            {
                //This won't work because this method is static.
                // x = 75;
            }
        }

        class DefaultInterfaceMembers
        {
            [Test] public void DefaultInterfaceMembersTest()
            {
                //This probably isn't a feature you want to use heavily.
                //But it could be useful for library developers.
            }

            interface ExampleInterface
            {
                string Method1();

                string Method2()
                {
                    return Method1() + "another string";
                }
            }
        }
    }
}