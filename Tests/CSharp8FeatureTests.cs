
using NUnit.Framework;

namespace Tests
{
    class CSharp8Features
    {
        
        public class DefaultInterfaceMembers
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

        class PatternMatching
        {
            [Test] public void SwitchExpressions()
            {

            }

            [Test] public void PropertyPatterns()
            {

            }

            [Test] public void TuplePatterns()
            {

            }

            [Test] public void PositionalPatterns()
            {
                
            }
        }
    }
}